using System.Collections.Generic;

namespace DocumentGenerator
{
    public class GeneratedFieldInstanceValue
    {
        public List<string> Values { get; set; }

        public GeneratedFieldInstanceValue(List<string> values)
        {
            Values = values;
        }
    }
}
