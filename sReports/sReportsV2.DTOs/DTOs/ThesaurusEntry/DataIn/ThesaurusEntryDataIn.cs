using sReportsV2.DTOs.O4CodeableConcept.DataIn;
using sReportsV2.DTOs.ThesaurusEntry.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace sReportsV2.DTOs.ThesaurusEntry
{
    [DataContract]
    public class ThesaurusEntryDataIn
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "umlsCode")]
        public string UmlsCode { get; set; }

        [DataMember(Name = "umlsName")]
        public string UmlsName { get; set; }

        [DataType(DataType.Html)]
        [DataMember(Name = "umlsDefinitions")]
        public string UmlsDefinitions { get; set; }

        [DataMember(Name = "parentId")]
        public string ParentId { get; set; }

        [DataMember(Name = "translations")]
        public List<ThesaurusEntryTranslationDataIn> Translations { get; set; }

        [DataMember(Name = "stateCD")]
        public int? StateCD { get; set; }

        public List<ThesaurusEntryCodingSystemDTO> CodingSystems { get; set; }

        public List<O4CodeableConceptDataIn> Codes { get; set; }

        public DateTimeOffset? LastUpdate { get; set; }

        [DataMember(Name = "preferredLanguage")]
        public string PreferredLanguage { get; set; }

        public string UriClassLink { get; set; }
        public string UriClassGUI { get; set; }
        public string UriSourceLink { get; set; }
        public string UriSourceGUI { get; set; }

    }
}