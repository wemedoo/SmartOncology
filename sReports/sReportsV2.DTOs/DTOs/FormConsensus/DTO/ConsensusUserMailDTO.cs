using sReportsV2.DTOs.DTOs.User.DataOut;

namespace sReportsV2.DTOs.DTOs.FormConsensus.DTO
{
    public class ConsensusUserMailDTO
    {
        public string ConsensusId { get; set; }
        public bool IsOutsideUser { get; set; }
        public string IterationId { get; set; }
        public UserViewDataOut User { get; set; }
        public string CustomMessage { get; set; }
        public string FullFormNameWithVersion { get; set; }

    }
}
