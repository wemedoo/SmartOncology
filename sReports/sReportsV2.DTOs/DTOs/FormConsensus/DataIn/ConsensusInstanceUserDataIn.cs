namespace sReportsV2.DTOs.DTOs.FormConsensus.DataIn
{
    public class ConsensusInstanceUserDataIn
    {
        public int UserId { get; set; } 
        public string ConsensusId { get; set; } 
        public string ConsensusInstanceId { get; set; } 
        public string FormId { get; set; } 
        public bool IsOutsideUser { get; set; } 
        public string IterationId { get; set; } 
        public string ViewType { get; set; } 
        public string ShowQuestionnaireType { get; set; } 
    }
}
