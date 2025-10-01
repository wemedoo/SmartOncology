namespace sReportsV2.Domain.MongoDb.Entities.FormInstance
{
    public class FormInstanceItemLockingStatus
    {
        public bool IsLocked { get; set; }
        public IDictionary<string, FormInstanceItemLockingStatus> ChildrenLockingStatus { get; set; }

        public FormInstanceItemLockingStatus(bool isLocked)
        {
            IsLocked = isLocked;
            CreateChildrenIfEmpty();
        }

        public void UpdateChildrenLockingStatus(IDictionary<string, FormInstanceItemLockingStatus> childrenLockingStatus)
        {
            this.ChildrenLockingStatus = childrenLockingStatus;
        }

        public void UpdateChildLockingStatus(string childId, FormInstanceItemLockingStatus childLockingStatus)
        {
            CreateChildrenIfEmpty();
            ChildrenLockingStatus[childId] = childLockingStatus;
        }

        public void UpdateChildLockingStatus(string childId, bool isLocked)
        {
            CreateChildrenIfEmpty();
            if (ChildrenLockingStatus.TryGetValue(childId, out FormInstanceItemLockingStatus formInstanceItemLockingStatus))
            {
                formInstanceItemLockingStatus.IsLocked = isLocked;
            }
        }

        public bool AllChildrenLocked()
        {
            return ChildrenLockingStatus.Values.Select(x => x.IsLocked).All(x => x);
        }

        public FormInstanceItemLockingStatus GetChild(string childId)
        {
            return ChildrenLockingStatus.TryGetValue(childId, out FormInstanceItemLockingStatus formInstanceItemLockingStatus) ? formInstanceItemLockingStatus : null;
        }

        private void CreateChildrenIfEmpty()
        {
            if (ChildrenLockingStatus == null)
            {
                this.ChildrenLockingStatus = new Dictionary<string, FormInstanceItemLockingStatus>();
            }
        }
    }
}
