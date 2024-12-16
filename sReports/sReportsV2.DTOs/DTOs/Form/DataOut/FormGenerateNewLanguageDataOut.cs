using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.Form.DataOut
{
    public class FormGenerateNewLanguageDataOut
    {
        public string FormId { get; set; }  
        public string Title { get; set; }  
        public List<EnumDTO> PossibleLanguages { get; set; }
    }
}
