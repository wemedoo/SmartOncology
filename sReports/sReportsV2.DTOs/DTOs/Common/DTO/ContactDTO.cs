using System;
using System.Collections.Generic;

namespace sReportsV2.DTOs.Common.DTO
{
    public class ContactDTO
    {
        public int Id { get; set; }
        public int? ContactRoleId { get; set; }
        public int? ContactRelationshipId { get; set; }
        public int? GenderId { get; set; }
        public DateTime? BirthDate { get; set; }
        public string NameGiven { get; set; }
        public string NameFamily { get; set; }
        public List<AddressDTO> Addresses { get; set; }
        public string Gender { get; set; }
        public List<TelecomDTO> Telecoms { get; set; }
        public string RowVersion { get; set; }
    }
}