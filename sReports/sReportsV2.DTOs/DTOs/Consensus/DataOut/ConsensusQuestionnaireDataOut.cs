using sReportsV2.Common.Enums;
using sReportsV2.Domain.Entities.Consensus;
using sReportsV2.DTOs.DTOs.FormConsensus.DataIn;
using sReportsV2.DTOs.Form.DataOut;

namespace sReportsV2.DTOs.DTOs.Consensus.DataOut
{
    public class ConsensusQuestionnaireDataOut
    {
        public int UserId { get; set; } 
        public bool IsOutsideUser { get; set; }
        public ConsensusDataOut Consensus { get; set; }
        public string ConsensusInstanceId { get; set; }
        public string ShowQuestionnaireType { get; set; }
        public string ViewType { get; set; }
        public string IterationId { get; set; }
        public IterationState IterationState { get; set; }
        public bool Completed { get; set; }
        public bool CanChange { get; set; }

        public ConsensusQuestionnaireDataOut()
        {
        }

        public ConsensusQuestionnaireDataOut(ConsensusDataOut consensus)
        {
            Consensus = consensus;
        }

        public ConsensusQuestionnaireDataOut(ConsensusDataOut consensus, ConsensusInstance instance, ConsensusInstanceUserDataIn consensusInstanceUser, string showQuestionnaireType, int? loggedUserId) : this(consensus)
        {
            ConsensusInstanceId = instance != null ? instance.Id : string.Empty;
            IterationId = consensusInstanceUser.IterationId;
            IterationState = consensus?.GetIterationById(consensusInstanceUser.IterationId)?.State ?? IterationState.NotStarted;
            UserId = consensusInstanceUser.UserId;
            IsOutsideUser = consensusInstanceUser.IsOutsideUser;
            ShowQuestionnaireType = showQuestionnaireType;
            Completed = instance != null && instance.IsCompleted();
            ViewType = consensusInstanceUser.ViewType;
            CanChange = 
                this.ViewType.Equals(sReportsV2.Common.Constants.EndpointConstants.Create) 
                && !this.Completed 
                && this.IterationState != IterationState.Terminated 
                && DoesConsensusInstanceBelongsToUser(loggedUserId);
        }

        private bool DoesConsensusInstanceBelongsToUser(int? loggedUserId)
        {
            if (IsOutsideUser)
            {
                return loggedUserId == null;
            }
            else
            {
                return this.UserId == loggedUserId;
            }
        }
    }
}
