using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NHapi.Base.Model;
using NHapi.Base.Parser;
using NHapi.Base.Validation.Implementation;
using NHapi.Model.V231.Segment;
using sReportsV2.Cache.Singleton;
using sReportsV2.Common.Configurations;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Exceptions;
using sReportsV2.Common.Helpers;
using sReportsV2.Domain.Sql.Entities.HL7;
using sReportsV2.HL7.Constants;
using sReportsV2.HL7.DTO;
using sReportsV2.HL7.Handlers.IncomingHandlers;
using sReportsV2.HL7.Handlers;
using sReportsV2.HL7.Validation;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using sReportsV2.DAL.Sql.Sql;
using System.Net;
using sReportsV2.Common.Extensions;

namespace sReportsV2.HL7.Components
{
    public class HL7MllpServer : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public HL7MllpServer(IServiceScopeFactory serviceScopeFactory)
        {
            this._serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            TcpListener listener = null;
            try
            {
                listener = new TcpListener(IPAddress.Any, GetPort());
                string endpoint = listener.LocalEndpoint.ToString();
                listener.Start();
                LogHelper.Info($"Tcp server is started and accepts requests on {endpoint}");

                while (!stoppingToken.IsCancellationRequested)
                {
                    var client = await listener.AcceptTcpClientAsync(stoppingToken);
                    _ = Task.Run(() => ProcessClientConnection(client, stoppingToken), stoppingToken);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("Error while starting Tcp server");
                LogHelper.Error(ex.GetExceptionStackMessages());
            }
            finally
            {
                listener?.Stop();
            }
        }

        private async Task ProcessClientConnection(TcpClient tcpClientConnection, CancellationToken stoppingToken)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                SReportsContext dbContext = scope.ServiceProvider.GetRequiredService<SReportsContext>();

                NetworkStream netStream = null;
                IMessage parsedMessage = null;
                int? hl7MessageLogId = null;
                string messageFromStream = string.Empty;
                try
                {
                    LogHelper.Info("A client connection was initiated from " + tcpClientConnection.Client.RemoteEndPoint);

                    var receivedByteBuffer = new byte[200];
                    netStream = tcpClientConnection.GetStream();
                    int bytesReceived;
                    var hl7Data = string.Empty;
                    StringBuilder textBuilder = new StringBuilder();

                    while ((bytesReceived = await netStream.ReadAsync(receivedByteBuffer, 0, receivedByteBuffer.Length, stoppingToken)) > 0)
                    {
                        textBuilder.Append(Encoding.UTF8.GetString(receivedByteBuffer, 0, bytesReceived));
                        hl7Data= textBuilder.ToString();
                        int startOfMllpEnvelope = hl7Data.IndexOf(HL7Constants.START_OF_BLOCK);
                        if (startOfMllpEnvelope >= 0)
                        {
                            int end = hl7Data.IndexOf(HL7Constants.END_OF_BLOCK);
                            if (end >= startOfMllpEnvelope)
                            {
                                messageFromStream = hl7Data.Substring(startOfMllpEnvelope + 1, end - startOfMllpEnvelope);
                                hl7MessageLogId = LogIncomingMessage(messageFromStream, dbContext);
                                string messageType = GetMessageType(messageFromStream);
                                parsedMessage = ParseMessage(messageFromStream, messageType);
                                IncomingMessageMetadataDTO messageMetadata = CreateMessageMetadata(
                                    hl7MessageLogId.Value,
                                    parsedMessage,
                                    messageType,
                                    GetDateTimeOfMessage(parsedMessage)
                                );
                                string ackMessageResponse = await ProcessMessage(messageMetadata, dbContext);
                                LogHelper.Info("ACK message: \n" + ackMessageResponse);
                                WriteResponse(ackMessageResponse, netStream);
                            }
                        }
                    }
                }
                catch (HL7RejectMessageException hl7Exception)
                {
                    LogMessageWithError(CreateMessageMetadataWithError(hl7MessageLogId, messageFromStream, hl7Exception.Message, HL7Constants.APPLICATION_REJECT_CODE), dbContext);
                    WriteResponse(ProcessErrorMessage(parsedMessage, HL7Constants.APPLICATION_REJECT_CODE, "Message is rejected by SO"), netStream);
                }
                catch (Exception e)
                {
                    LogMessageWithError(CreateMessageMetadataWithError(hl7MessageLogId, messageFromStream, e.Message, HL7Constants.APPLICATION_ERROR_CODE), dbContext);
                    WriteResponse(ProcessErrorMessage(parsedMessage, HL7Constants.APPLICATION_ERROR_CODE, "Application error"), netStream);
                }
                finally
                {
                    netStream?.Close();
                    netStream?.Dispose();
                    tcpClientConnection?.Close();
                }
            }
            
        }

        private int LogIncomingMessage(string messageFromStream, SReportsContext dbContext)
        {
            LogHelper.Info("hl7 message: \n" + messageFromStream);
            HL7MessageLog hL7MessageLog = new HL7MessageLog(GetMessageControlId(messageFromStream), messageFromStream);
            dbContext.HL7MessageLogs.Add(hL7MessageLog);
            dbContext.SaveChanges();

            return hL7MessageLog.HL7MessageLogId;
        }

        private void LogMessageWithError(IncomingMessageMetadataDTO messageMetadata, SReportsContext dbContext)
        {
            LogHelper.Error(messageMetadata.ErrorText);
            if (messageMetadata.HL7MessageLogId != 0)
            {
                dbContext.ErrorMessageLogs.Add(CreateErrorMessageLog(messageMetadata));
                dbContext.SaveChanges();
            }
        }

        private IMessage ParseMessage(string messageString, string messageType)
        {
            try
            {
                var ourPipeParser = new PipeParser { ValidationContext = GetValidationComponent(messageType) };
                return ourPipeParser.Parse(messageString);
            }
            catch (Exception ex)
            {
                string exMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                throw new HL7RejectMessageException(exMessage);
            }
        }
        
        private async Task<string> ProcessMessage(IncomingMessageMetadataDTO messageMetadata, SReportsContext dbContext)
        {
            HL7IncomingMessageHandler hL7MessageHandler = HL7IncomingMessageHandlerFactory.GetHandler(messageMetadata);
            await hL7MessageHandler?.Process(dbContext);
            MSH mshSegment = GetMshSegment(messageMetadata.ParsedMessage);
            return GetSimpleAcknowledgementMessage(mshSegment);
        }

        #region HL7 Helper methods
        private int GetPort()
        {
            try
            {
                string mllpPortEnvironmentVariable = Environment.GetEnvironmentVariable(HL7Constants.MLLP_PORT, EnvironmentVariableTarget.Machine) 
                    ?? throw new InvalidOperationException("No value found for the MLLP port environment variable.");

                return int.Parse(mllpPortEnvironmentVariable);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Error while retrieving port for MLLP server: " + ex.GetExceptionStackMessages);
            }
        }

        private IncomingMessageMetadataDTO CreateMessageMetadataWithError(int? hl7MessageLogId, string messageFromStream, string exceptionMessage, string applicationCode)
        {
            if (!hl7MessageLogId.HasValue || string.IsNullOrEmpty(messageFromStream)) return new IncomingMessageMetadataDTO() { ErrorText = exceptionMessage };

            return new IncomingMessageMetadataDTO(
                hl7MessageLogId.Value,
                GetMessageType(messageFromStream),
                SingletonDataContainer.Instance.GetCodeId((int)CodeSetList.SourceSystem, string.Empty),
                GetDateTimeOfMessage(messageFromStream)
            )
            {
                ErrorText = exceptionMessage,
                ErrorTypeCD = SingletonDataContainer.Instance.GetCodeId((int)CodeSetList.ErrorType, applicationCode)
            };
        }

        private DateTime? GetDateTimeOfMessage(string messageFromStream)
        {
            try
            {
                string dateTimeOfMessageString = messageFromStream.Split(HL7Constants.FIELD_DELIMITER)[6];
                return DateTime.ParseExact(dateTimeOfMessageString, HL7Constants.HL7_DATE_TIME_FORMAT, null);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private ErrorMessageLog CreateErrorMessageLog(IncomingMessageMetadataDTO messageMetadata)
        {
            DateTimeOffset transactionDateTime = new DateTimeOffset(messageMetadata.TransactionDatetime.Value.ToUniversalTime());

            return new ErrorMessageLog()
            {
                HL7MessageLogId = messageMetadata.HL7MessageLogId,
                ErrorText = messageMetadata.ErrorText,
                ErrorTypeCD = messageMetadata.ErrorTypeCD,
                HL7EventType = messageMetadata.HL7EventType,
                SourceSystemCD = messageMetadata.SourceSystemCD,
                TransactionDatetime = messageMetadata.TransactionDatetime.HasValue
                ? transactionDateTime.GetDateTimeByTimeZone(GlobalConfig.GetTimeZoneId())
                : (DateTimeOffset?)null
            };
        }

        private string GetMessageControlId(string messageString)
        {
            return messageString.Split(HL7Constants.FIELD_DELIMITER)[9];
        }

        private string GetMessageType(string messageString)
        {
            return messageString.Split(HL7Constants.FIELD_DELIMITER)[8];
        }

        private DefaultValidation GetValidationComponent(string messageType)
        {
            switch (messageType)
            {
                case HL7Constants.ADT_A01:
                case HL7Constants.ADT_A03:
                case HL7Constants.ADT_A04:
                case HL7Constants.ADT_A08:
                    return new CustomValidations(
                        new List<NHapi.Base.Validation.IMessageRule>
                        {
                            new RequiredValuesMessageRule(),
                            new RequiredMRNIdentifierMessageRule(),
                            new RequiredEncounterTypeMatch()
                        }
                    );
                case HL7Constants.ADT_A28:
                case HL7Constants.ADT_A31:
                    return new CustomValidations(
                        new List<NHapi.Base.Validation.IMessageRule>
                        {
                            new OnlyPatientDataRequiredMessageRule(),
                            new RequiredMRNIdentifierMessageRule()
                        }
                    );
                default:
                    return new DefaultValidation();
            }
        }

        private DateTime GetDateTimeOfMessage(IMessage message)
        {
            MSH mshSegment = GetMshSegment(message);
            return HL7IncomingHelper.TsToDateTime(mshSegment?.DateTimeOfMessage.TimeOfAnEvent);
        }

        private IncomingMessageMetadataDTO CreateMessageMetadata(int hl7MessageLogId, IMessage parsedMessage, string messageType, DateTime transactionDatetime)
        {
            return new IncomingMessageMetadataDTO(
                hl7MessageLogId,
                messageType,
                SingletonDataContainer.Instance.GetCodeId((int)CodeSetList.SourceSystem, string.Empty),
                transactionDatetime
            )
            {
                ParsedMessage = parsedMessage,
                TransactionType = HL7Constants.HL7,
                TransactionDirectionCD = SingletonDataContainer.Instance.GetCodeId((int)CodeSetList.TransactionDirection, HL7Constants.Inbound)
            };
        }

        private string ProcessErrorMessage(IMessage hl7Message, string acknowledgmentCode, string errorMessage)
        {
            return GetSimpleAcknowledgementMessage(GetMshSegment(hl7Message), acknowledgmentCode, errorMessage);
        }

        private MSH GetMshSegment(IMessage hl7Message)
        {
            return hl7Message?.GetType().GetProperty("MSH").GetValue(hl7Message) as MSH;
        }

        private string GetSimpleAcknowledgementMessage(MSH msh, string acknowledgmentCode = HL7Constants.APPLICATION_ACCEPT_CODE, string message = "Success")
        {
            var ackMessage = new StringBuilder();
            ackMessage = ackMessage.Append(HL7Constants.START_OF_BLOCK)
                .Append(FormatMshSegment(msh))
                .Append(HL7Constants.CARRIAGE_RETURN)
                .Append(FormatMsaSegment(msh, acknowledgmentCode, message))
                .Append(HL7Constants.CARRIAGE_RETURN)
                .Append(HL7Constants.END_OF_BLOCK)
                .Append(HL7Constants.CARRIAGE_RETURN);

            return ackMessage.ToString();
        }

        private string FormatMshSegment(MSH msh)
        {
            char msh_1 = HL7Constants.FIELD_DELIMITER;
            string msh_2 = "^~\\&";
            string msh_3 = msh?.SendingApplication.NamespaceID.Value;
            string msh_4 = msh?.SendingFacility.NamespaceID.Value;
            string msh_5 = msh?.ReceivingApplication.NamespaceID.Value;
            string msh_6 = msh?.ReceivingFacility.NamespaceID.Value;
            string msh_7 = msh?.DateTimeOfMessage.TimeOfAnEvent.Value;
            string msh_9 = msh?.MessageType.MessageType.Value + (string.IsNullOrEmpty(msh?.MessageType.TriggerEvent.Value) ? string.Empty : $"{HL7Constants.COMPONENT_SEPARATOR}{msh?.MessageType.TriggerEvent.Value}");
            string msh_10 = msh?.MessageControlID.Value;
            string msh_11 = msh?.ProcessingID.ProcessingID.Value;
            string msh_12 = msh?.VersionID.VersionID.Version;
            string msh_15 = msh?.AcceptAcknowledgmentType.Value;
            string msh_16 = msh?.ApplicationAcknowledgmentType.Value;

            return $"MSH{msh_1}{msh_2}|{msh_3}|{msh_4}|{msh_5}|{msh_6}|{msh_7}||{msh_9}|{msh_10}|{msh_11}|{msh_12}|||{msh_15}|{msh_16}||";
        }

        private string FormatMsaSegment(MSH msh, string acknowledgmentCode, string message)
        {
            string msa_1 = acknowledgmentCode;
            string msa_2 = msh?.MessageControlID.Value;
            string msa_3 = message;

            return $"MSA|{msa_1}|{msa_2}|{msa_3}|";
        }

        private void WriteResponse(string ackMessage, NetworkStream netStream)
        {
            if (netStream != null && netStream.CanWrite)
            {
                byte[] buffer = Encoding.UTF8.GetBytes(ackMessage);
                netStream.Write(buffer, 0, buffer.Length);
                LogHelper.Info("(N)Ack message was sent back to the client...");
            }
        }

        #endregion HL7 Helper methods
    }
}
