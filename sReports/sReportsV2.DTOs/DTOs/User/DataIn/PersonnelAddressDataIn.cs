using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.CustomAttributes;

namespace sReportsV2.DTOs.DTOs.User.DataIn
{
    [ObjectPropertiesRequired(new string[] { "City", "AddressTypeCD", "Street", "CountryCD" }, new string[] { "City", "Address Type", "Street", "Country" })]
    public class PersonnelAddressDataIn : AddressDTO
    {
        public int PersonnelId { get; set; }
    }
}
