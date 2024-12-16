using Newtonsoft.Json;
using sReportsV2.DTOs.Autocomplete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace sReportsV2.DTOs.Autocomplete
{
    public class AutocompleteResultDataOut
    {
        public List<AutocompleteDataOut> results { get; set; }

        public AutocompletePaginatioDataOut pagination { get; set; }

    }
}