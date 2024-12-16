using Newtonsoft.Json;
using sReportsV2.DTOs.Autocomplete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.Autocomplete.DataOut
{
    public class RelationFieldAutocompleteResultDataOut
    {
        public List<RelationFieldAutocompleteDataOut> results { get; set; }

        public AutocompletePaginatioDataOut pagination { get; set; }
    }
}
