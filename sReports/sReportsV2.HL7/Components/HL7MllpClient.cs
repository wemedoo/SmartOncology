using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using sReportsV2.Common.Helpers;
using sReportsV2.HL7.Constants;
using System.Configuration;
using Microsoft.Extensions.Configuration;

namespace sReportsV2.HL7.Components
{
    public class HL7MllpClient
    {
        public string OutgoingMessage { get; set; }
        public IConfiguration Configuration { get; set; }
        private string MllpServerIP { get; set; }
        private int MllpServerPort { get; set; }

        public HL7MllpClient(string outgoingMessage, IConfiguration configuration)
        {
            this.OutgoingMessage = outgoingMessage;
            this.Configuration = configuration;
            SetConnectionParameters();
        }

        public string SendHL7Message()
        {
            TcpClient ourTcpClient = null;
            NetworkStream networkStream = null;
            string responseMessage = string.Empty;
            try
            {
                bool dataAreSent = ConnectAndSend(out ourTcpClient, out networkStream);

                if (dataAreSent)
                {
                    responseMessage = GetResponse(networkStream);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error($"Error while sending data to HL7 server ({MllpServerIP}:{MllpServerPort}), error: {ex.Message}");
                throw;
            }
            finally
            {
                networkStream?.Close();
                ourTcpClient?.Close();
            }

            return responseMessage;
        }

        private void SetConnectionParameters()
        {
            this.MllpServerIP = Configuration["HL7ExternalInstanceIP"];
            int.TryParse(Configuration["HL7ExternalInstancePort"], out int mllpServerPort);
            this.MllpServerPort = mllpServerPort;
        }

        private bool ConnectAndSend(out TcpClient ourTcpClient, out NetworkStream networkStream)
        {
            IPAddress ipAdress = IPAddress.Parse(MllpServerIP);

            ourTcpClient = new TcpClient();
            ourTcpClient.Connect(new IPEndPoint(ipAdress, MllpServerPort));

            networkStream = ourTcpClient.GetStream();

            byte[] sendMessageByteBuffer = Encoding.UTF8.GetBytes(PrepareMessageInMllpFormat(OutgoingMessage));

            bool dataAreSent = networkStream.CanWrite;
            if (dataAreSent)
            {
                networkStream.Write(sendMessageByteBuffer, 0, sendMessageByteBuffer.Length);

                LogHelper.Info($"Data is sent to HL7 server ({MllpServerIP}:{MllpServerPort}) successfully");
            }

            return dataAreSent;
        }

        private string PrepareMessageInMllpFormat(string outgoingMessage)
        {
            return $"{HL7Constants.START_OF_BLOCK}{outgoingMessage}{HL7Constants.END_OF_BLOCK}{HL7Constants.CARRIAGE_RETURN}";
        }

        private string GetResponse(NetworkStream networkStream)
        {
            var receivedByteBuffer = new byte[200];
            string responseMessage = string.Empty;

            int bytesReceived = networkStream.Read(receivedByteBuffer, 0, receivedByteBuffer.Length);
            if (bytesReceived > 0)
            {
                string hl7Data = Encoding.UTF8.GetString(receivedByteBuffer, 0, bytesReceived);

                int startOfMllpEnvelope = hl7Data.IndexOf(HL7Constants.START_OF_BLOCK);
                if (startOfMllpEnvelope >= 0)
                {
                    int end = hl7Data.IndexOf(HL7Constants.END_OF_BLOCK);
                    if (end >= startOfMllpEnvelope)
                    {
                        responseMessage = hl7Data.Substring(startOfMllpEnvelope + 1, end - startOfMllpEnvelope);
                    }
                }
            }

            return responseMessage;
        }
    }
}
