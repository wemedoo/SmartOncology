using sReportsV2.Common.Enums;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Entities.FormInstance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.FieldEntity
{
    public partial class FieldDatetime
    {
        protected override int GetMissingValueCodeSetId()
        {
            return (int)CodeSetList.MissingValueDateTime;
        }

        protected override string GetDisplayValue(FieldInstanceValue fieldInstanceValue)
        {
            return base.GetDisplayValue(fieldInstanceValue).RenderDatetime();
        }
    }
}
