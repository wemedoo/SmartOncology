using sReportsV2.DTOs.DTOs.Autocomplete.DataOut;
using sReportsV2.DTOs.DTOs.FormInstance.DataOut;
using System.Collections.Generic;

namespace sReportsV2.DTOs.Encounter.DataOut
{
    public class EncounterDetailsPatientTreeDataOut
    {
        public EncounterDataOut Encounter { get; set; }
        public List<FormInstanceMetadataDataOut> FormInstances { get; set; }
        public List<AutocompleteOptionDataOut> Forms { get; set; }
    }
}