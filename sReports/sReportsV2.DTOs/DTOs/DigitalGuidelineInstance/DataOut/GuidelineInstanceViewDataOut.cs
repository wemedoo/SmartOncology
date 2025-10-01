using sReportsV2.DTOs.DTOs.Autocomplete.DataOut;
using System.Collections.Generic;

namespace sReportsV2.DTOs.DigitalGuidelineInstance.DataOut
{
    public class GuidelineInstanceViewDataOut
    {
        public GuidelineInstanceDataOut GuidelineInstance { get; set; }
        public List<AutocompleteOptionDataOut> Guidelines { get; set; }
        public List<AutocompleteOptionDataOut> FormInstances { get; set; }
    }
}