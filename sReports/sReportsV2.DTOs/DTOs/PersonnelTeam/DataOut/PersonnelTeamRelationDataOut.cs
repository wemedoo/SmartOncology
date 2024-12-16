using sReportsV2.DTOs.Common.DataOut;
using sReportsV2.DTOs.CodeEntry.DataOut;

namespace sReportsV2.DTOs.DTOs.PersonnelTeam.DataOut
{
    public class PersonnelTeamRelationDataOut
    {
        public int PersonnelTeamRelationId { get; set; }
        public CodeDataOut RelationType { get; set; }
        public int PersonnelTeamId { get; set; }
        public int UserId { get; set; }
        public UserDataOut User { get; set; }
    }
}
