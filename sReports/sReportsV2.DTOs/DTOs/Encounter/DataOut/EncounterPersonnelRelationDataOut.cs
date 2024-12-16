using sReportsV2.DTOs.User.DataOut;

namespace sReportsV2.DTOs.DTOs.Encounter.DataOut
{
    public class EncounterPersonnelRelationDataOut
    {
        public int Id { get; set; }
        public int RelationTypeId { get; set; }
        public int DoctorId { get; set; }
        public UserShortInfoDataOut Doctor { get; set; }
    }
}
