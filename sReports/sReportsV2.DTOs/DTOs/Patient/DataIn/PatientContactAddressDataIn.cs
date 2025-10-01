using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.CustomAttributes;

namespace sReportsV2.DTOs.DTOs.Patient.DataIn
{
    [ObjectPropertiesRequired(new string[] { "City", "AddressTypeCD", "Street", "CountryCD" }, new string[] { "City", "Address Type", "Street", "Country" })]
    public class PatientContactAddressDataIn : AddressDTO
    {
        public int PatientContactId { get; set; }
    }
}
