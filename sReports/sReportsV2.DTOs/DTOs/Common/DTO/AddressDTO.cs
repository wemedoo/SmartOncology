using System.ComponentModel.DataAnnotations;

namespace sReportsV2.DTOs.Common
{
    public class AddressDTO
    {
        public int Id { get; set; }
        [StringLength(100)]
        public string City { get; set; }
        [StringLength(50)]
        public string State { get; set; }
        [StringLength(10)]
        public string PostalCode { get; set; }
        [StringLength(200)]
        public string Street { get; set; }
        public string StreetNumber { get; set; }
        public int? AddressTypeCD { get; set; }
        public string Country { get; set; }
        public int? CountryCD { get; set; }
        public string RowVersion { get; set; }
        public string GetAddressFormated() 
        {
            return $"{this.Street}, {this.StreetNumber}";
        }
        public string GetAddressPreview()
        {
            return $"{City}, {PostalCode}, {Country}";
        }
    }
}