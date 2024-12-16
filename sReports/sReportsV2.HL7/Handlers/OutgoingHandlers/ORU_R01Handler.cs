using NHapi.Base.Model;
using NHapi.Model.V231.Datatype;
using NHapi.Model.V231.Group;
using NHapi.Model.V231.Message;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.Domain.Sql.Entities.Patient;
using sReportsV2.HL7.DTOs;
using System.Collections.Generic;
using System.Linq;

namespace sReportsV2.HL7.Handlers.OutgoingHandlers
{
    public class ORU_R01Handler : HL7OutgoingMessageHandler
    {
        public ORU_R01Handler(OutgoingMessageMetadataDTO messageMetadataDTO) : base(messageMetadataDTO)
        {
        }

        public override HL7MessageTypeMetadataDTO MessageType
        {
            get
            {
                return new HL7MessageTypeMetadataDTO {
                    MessageType = "ORU",
                    TriggerEvent = "R01",
                    MessageStructure = "ORU_R01"
                };
            }
        }

        protected override IMessage CreateMessage()
        {
            ORU_R01 message = new ORU_R01();

            Patient patient = MessageMetadata.Patient;
            FormInstance formInstance = MessageMetadata.FormInstance;

            ORU_R01_PATIENT_RESULT patientResult = message.GetPATIENT_RESULT(0);
            ORU_R01_PATIENT patientInfo = patientResult.PATIENT;
            ORU_R01_ORDER_OBSERVATION orderObservation = patientResult.GetORDER_OBSERVATION();
            ORU_R01_OBSERVATION oruOrderObservation = orderObservation.GetOBSERVATION(0);

            HL7OutgoingHelper.ConfigureSegment(message.MSH, "Smaragd", MessageMetadata.OrganizationAlias, MessageType);
            HL7OutgoingHelper.ConfigureSegment(patientInfo.PID, patient);
            HL7OutgoingHelper.ConfigureSegment(patientInfo.VISIT.PV1, MessageMetadata.Encounter);
            HL7OutgoingHelper.ConfigureSegment(orderObservation.ORC);
            HL7OutgoingHelper.ConfigureSegment(orderObservation.OBR, formInstance, MessageMetadata.FormAlias);
            HL7OutgoingHelper.ConfigureSegment(oruOrderObservation.OBX, new ED(message, "PDF Report Content"), formInstance, MessageMetadata.PdfDocument, MessageMetadata.FormAlias);

            return message;
        }
    }
}