using sReportsV2.DTOs.DTOs.Oomnia.DTO;
using System.Collections.Generic;
using System.Linq;

namespace sReportsV2.BusinessLayer.Helpers
{
    public class FormInstanceExternalRequestsCache
    {
        private static FormInstanceExternalRequestsCache instance;
        private readonly Dictionary<string, Queue<PassFormInstanceToOomniaApiDTO>> oomniaPendingRequests;

        private FormInstanceExternalRequestsCache()
        {
            this.oomniaPendingRequests = new Dictionary<string, Queue<PassFormInstanceToOomniaApiDTO>>();
        }

        public static FormInstanceExternalRequestsCache Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new FormInstanceExternalRequestsCache();
                }
                return instance;
            }
        }

        public bool HasPendingRequests(string formInstanceId)
        {
            return oomniaPendingRequests.TryGetValue(formInstanceId, out Queue<PassFormInstanceToOomniaApiDTO> formInstancePendingRequests) && formInstancePendingRequests.Any();
        }

        public void AddPendingRequest(string formInstanceId, PassFormInstanceToOomniaApiDTO request)
        {
            if (oomniaPendingRequests.TryGetValue(formInstanceId, out Queue<PassFormInstanceToOomniaApiDTO> formInstancePendingRequests))
            {
                formInstancePendingRequests.Enqueue(request);
            }
            else
            {
                oomniaPendingRequests[formInstanceId] = new Queue<PassFormInstanceToOomniaApiDTO>(new List<PassFormInstanceToOomniaApiDTO> { request });
            }
        }

        public void RemovePendingRequest(string formInstanceId)
        {
            if (oomniaPendingRequests.TryGetValue(formInstanceId, out Queue<PassFormInstanceToOomniaApiDTO> formInstancePendingRequests))
            {
                formInstancePendingRequests.Dequeue();
                if (!formInstancePendingRequests.Any())
                {
                    oomniaPendingRequests.Remove(formInstanceId);
                }
            }
        }

        public PassFormInstanceToOomniaApiDTO GetPendingRequest(string formInstanceId)
        {
            PassFormInstanceToOomniaApiDTO pendingRequest = null;
            if (oomniaPendingRequests.TryGetValue(formInstanceId, out Queue<PassFormInstanceToOomniaApiDTO> formInstancePendingRequests))
            {
                pendingRequest = formInstancePendingRequests.Peek();
            }
            return pendingRequest;
        }

    }
}
