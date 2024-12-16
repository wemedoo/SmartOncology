using NHapi.Base.Model;
using NHapi.Base.Parser;
using NHapi.Base.Util;
using NHapi.Base.Validation.Implementation;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Exceptions;
using sReportsV2.Common.Helpers;
using sReportsV2.Cache.Singleton;
using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Sql.Entities.HL7;
using sReportsV2.HL7.Validation;
using System;
using System.Collections.Generic;
using sReportsV2.HL7.DTOs;
using sReportsV2.HL7.Constants;
using sReportsV2.HL7.Components;

namespace sReportsV2.HL7.Handlers.OutgoingHandlers
{
    public abstract class HL7OutgoingMessageHandler
    {
        public OutgoingMessageMetadataDTO MessageMetadata { get; set; }

        public virtual HL7MessageTypeMetadataDTO MessageType
        {
            get
            {
                return new HL7MessageTypeMetadataDTO();
            }
        }

        protected HL7OutgoingMessageHandler(OutgoingMessageMetadataDTO messageMetadata)
        {
            MessageMetadata = messageMetadata;
            SetAdditionalMetadataParamaters();
        }

        public void ProcessMessage(SReportsContext dbContext)
        {
            try
            {
                IMessage message = CreateMessage();
                string parsedMessage = ParseMessage(message);
                MessageMetadata.HL7MessageLogId = AddHL7MessageLog(parsedMessage, "Sending", dbContext);
                HL7MllpClient hL7MllpClient = new HL7MllpClient(parsedMessage, MessageMetadata.Configuration);
                string responseMessage = hL7MllpClient.SendHL7Message();
                LogResponse(responseMessage, dbContext);
            }
            catch (Exception ex)
            {
                string excepetionErrorMessage = ExceptionHelper.GetExceptionStackMessages(ex);
                LogHelper.Error("Error while processing outgoing message --> " + excepetionErrorMessage);
                AddErrorMessageLog(
                    excepetionErrorMessage,
                    SingletonDataContainer.Instance.GetCodeId(
                        (int)CodeSetList.ErrorType, 
                        ex is HL7RejectMessageException ? 
                            HL7Constants.APPLICATION_REJECT_CODE : HL7Constants.APPLICATION_ERROR_CODE),
                    dbContext
                );
            }
        }

        protected abstract IMessage CreateMessage();

        protected DefaultValidation GetValidations()
        {
            return new CustomValidations(
                new List<NHapi.Base.Validation.IMessageRule>
                {
                    new OutgoingOruRequiredMessageRule()
                }
            );
        }

        private void SetAdditionalMetadataParamaters()
        {
            this.MessageMetadata.SourceSystemCD = SingletonDataContainer.Instance.GetCodeId(
                    (int)CodeSetList.SourceSystem,
                    string.Empty
                    );
            this.MessageMetadata.TransactionType = HL7Constants.HL7;
            this.MessageMetadata.TransactionDirectionCD = SingletonDataContainer.Instance.GetCodeId((int)CodeSetList.TransactionDirection, HL7Constants.Outbound);
            this.MessageMetadata.TransactionDatetime = DateTime.Now;
        }

        private string ParseMessage(IMessage message)
        {
            try
            {
                PipeParser pipeParser = new PipeParser { ValidationContext = GetValidations() };

                string encodingMessage = pipeParser.Encode(message);
                
                return encodingMessage;
            }
            catch (Exception ex)
            {
                throw new HL7RejectMessageException($"Message rejected, reason: { ExceptionHelper.GetExceptionStackMessages(ex) }");
            }
        }

        private int AddHL7MessageLog(string messageFromStream, string direction, SReportsContext dbContext)
        {
            LogHelper.Info($"{direction} HL7 message: \n{messageFromStream}");
            HL7MessageLog hL7MessageLog = new HL7MessageLog(GetMessageControlId(messageFromStream), messageFromStream);
            dbContext.HL7MessageLogs.Add(hL7MessageLog);
            dbContext.SaveChanges();

            return hL7MessageLog.HL7MessageLogId;
        }

        private void LogResponse(string responseMessage, SReportsContext dbContext)
        {
            if (!string.IsNullOrEmpty(responseMessage))
            {
                MessageMetadata.HL7MessageLogId = AddHL7MessageLog(responseMessage, "Received", dbContext);
                Tuple<string, string> msaFields = ParseResponseMessage(responseMessage);
                string acknoledgementCode = msaFields.Item1;
                if (acknoledgementCode == HL7Constants.APPLICATION_ACCEPT_CODE)
                {
                    AddSuccessMessageLog(dbContext);
                }
                else
                {
                    AddErrorMessageLog(
                        msaFields.Item2, 
                        SingletonDataContainer.Instance.GetCodeId((int)CodeSetList.ErrorType, acknoledgementCode),
                        dbContext
                        );
                }
            }
        }

        private void AddErrorMessageLog(string errorText, int? errorTypeCD, SReportsContext dbContext)
        {
            if (MessageMetadata.HL7MessageLogId != 0)
            {
                dbContext.ErrorMessageLogs.Add(CreateErrorMessageLog(errorText, errorTypeCD));
                dbContext.SaveChanges();
            }
        }

        private ErrorMessageLog CreateErrorMessageLog(string errorText, int? errorTypeCD)
        {
            return new ErrorMessageLog()
            {
                HL7MessageLogId = MessageMetadata.HL7MessageLogId,
                ErrorText = errorText,
                ErrorTypeCD = errorTypeCD,
                HL7EventType = MessageMetadata.HL7EventType,
                SourceSystemCD = MessageMetadata.SourceSystemCD,
                TransactionDatetime = MessageMetadata.TransactionDatetime
            };
        }

        private void AddSuccessMessageLog(SReportsContext dbContext)
        {
            dbContext.Transactions.Add(CreateSuccessMessageLog());
            dbContext.SaveChanges();
        }

        private Transaction CreateSuccessMessageLog()
        {
            return new Transaction()
            {
                PatientId = MessageMetadata.Patient.PatientId,
                EncounterId = MessageMetadata.Encounter.EncounterId,
                HL7MessageLogId = MessageMetadata.HL7MessageLogId,
                TransactionType = MessageMetadata.TransactionType,
                HL7EventType = MessageMetadata.HL7EventType,
                SourceSystemCD = MessageMetadata.SourceSystemCD,
                TransactionDirectionCD = MessageMetadata.TransactionDirectionCD,
                TransactionDatetime = MessageMetadata.TransactionDatetime
            };
        }

        private string GetMessageControlId(string messageString)
        {
            return messageString.Split(HL7Constants.FIELD_DELIMITER)[9];
        }

        private Tuple<string, string> ParseResponseMessage(string responseMessage)
        {
            PipeParser pipeParser = new PipeParser();
            IMessage parsedMessage = pipeParser.Parse(responseMessage);
            Terser terser = new Terser(parsedMessage);
            string acknolegementCode = terser.Get("MSA-1");
            string errorText = terser.Get("MSA-3");
            return new Tuple<string, string>(acknolegementCode, errorText);
        }
    }
}