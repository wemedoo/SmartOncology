using sReportsV2.HL7.Constants;
using sReportsV2.HL7.DTOs;
using sReportsV2.HL7.Handlers.OutgoingHandlers;

namespace sReportsV2.HL7
{
    public static class HL7OutgoingMessageHandlerFactory
    {
        public static HL7OutgoingMessageHandler GetHandler(OutgoingMessageMetadataDTO messageMetadataDTO)
        {
            switch (messageMetadataDTO.HL7EventType)
            {
                case HL7Constants.ORU_R01:
                default:
                    return new ORU_R01Handler(messageMetadataDTO);
            }
        }
    }
}