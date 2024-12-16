using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.Common.DataOut;
using sReportsV2.DTOs.Organization;
using System.Linq;

namespace sReportsV2.DTOs.Form.DataOut
{
    public class ConsensusUserDataOut
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Institution { get; set; }
        public string InstitutionAddress { get; set; }
        public AddressDTO Address { get; set; }

        public ConsensusUserDataOut()
        {
        }

        public ConsensusUserDataOut(ConsensusUserDataOut outsideUser)
        {
            this.Id = outsideUser.Id;
            this.FirstName = outsideUser.FirstName;
            this.LastName = outsideUser.LastName;
            this.Email = outsideUser.Email;
            this.Institution = outsideUser.Institution;
            this.InstitutionAddress = outsideUser.InstitutionAddress;
            this.Address = outsideUser.Address;
        }

        public ConsensusUserDataOut(UserDataOut insideUser)
        {
            OrganizationDataOut organization = insideUser.Organizations?.FirstOrDefault()?.Organization;
            this.Id = insideUser.Id;
            this.FirstName = insideUser.FirstName;
            this.LastName = insideUser.LastName;
            this.Email = insideUser.Email;
            this.Institution = organization != null ? organization.Name : string.Empty;
            this.InstitutionAddress = organization != null ? organization.Address.GetAddressFormated() : string.Empty;
            this.Address = insideUser.Addresses?.FirstOrDefault();
        }
    }
}