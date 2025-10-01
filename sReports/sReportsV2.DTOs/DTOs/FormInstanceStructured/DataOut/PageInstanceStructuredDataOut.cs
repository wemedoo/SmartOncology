using sReportsV2.DTOs.Field.DataOut;
using System;
using System.Collections.Generic;
using System.Linq;

namespace sReportsV2.DTOs.Form.DataOut
{
    public partial class FormPageDataOut
    {
        public bool DoesAllMandatoryFieldsHaveValue { get; set; }
        public bool IsLocked { get; set; }
        public bool CanBeLockedNext { get; set; }

        public IEnumerable<FormFieldSetDataOut> GetFieldsForDependencyFormula(FieldDataOut openedField)
        {
            this.ListOfFieldSets = this.ListOfFieldSets ?? new List<List<FormFieldSetDataOut>>();
            return this.ListOfFieldSets
                .SelectMany(fs => fs)
                .Where(fs => fs.CanFieldSetBeIncludedInDependencyFormula(openedField)
                    && fs.GetFieldsForDependencyFormula(openedField.Id).Any()
                    );
        }
        public bool ShouldLockActionBeShown(bool hasPermissionToLock)
        {
            return DoesAllMandatoryFieldsHaveValue && IsLocked && hasPermissionToLock;
        }
    }
}
