using System;

namespace sReportsV2.DTOs.DTOs.User.DataOut
{
    public class UserViewDataOut
    {
        public int Id { get; set; }
        public DateTimeOffset? LastUpdate { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int? StateCD { get; set; }
        public string PersonnelPositions { get; set; }
        public string PersonnelOrganizations { get; set; }

        public bool IsUserBlocked(int? blockedUserStateCD)
        {
            return StateCD == blockedUserStateCD;
        }
    }
}
