using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.CustomAttributes;

namespace sReportsV2.DTOs.DTOs.Patient.DataIn
{
    [ObjectPropertiesRequired(new string[] { "City", "AddressTypeCD", "Street", "CountryCD" }, new string[] { "City", "Address Type", "Street", "Country" })]
    public class PatientAddressDataIn : AddressDTO
    {
        public int PatientId { get; set; }
    }
}
