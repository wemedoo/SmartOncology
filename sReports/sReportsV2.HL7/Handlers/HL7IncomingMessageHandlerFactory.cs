using sReportsV2.Common.Exceptions;
using sReportsV2.HL7.Constants;
using sReportsV2.HL7.DTO;
using sReportsV2.HL7.Handlers.IncomingHandlers;

namespace sReportsV2.HL7.Handlers
{
    public static class HL7IncomingMessageHandlerFactory
    {
        public static HL7IncomingMessageHandler GetHandler(IncomingMessageMetadataDTO messageMetadata)
        {
            switch (messageMetadata.HL7EventType)
            {
                case HL7Constants.ADT_A01:
                case HL7Constants.ADT_A04: return new ADT_A01Handler(messageMetadata);
                case HL7Constants.ADT_A03: return new ADT_A03Handler(messageMetadata);
                case HL7Constants.ADT_A08: return new ADT_A08Handler(messageMetadata);
                case HL7Constants.ADT_A28: return new ADT_A28Handler(messageMetadata);
                case HL7Constants.ADT_A31: return new ADT_A31Handler(messageMetadata);
                default:
                    throw new HL7RejectMessageException($"Handling of [{messageMetadata.HL7EventType}] message has not been supported in SO HL7 yet");
            }
        }
    }
}