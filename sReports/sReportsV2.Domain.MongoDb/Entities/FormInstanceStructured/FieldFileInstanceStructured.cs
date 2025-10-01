using sReportsV2.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.FieldEntity
{
    public partial class FieldFile
    {
        protected override string FormatPatholinkValue(string selectedOptionId)
        {
            return this.FieldInstanceValues.FirstOrDefault()?.GetFirstValue().GetFileNameFromUri();
        }
    }
}
