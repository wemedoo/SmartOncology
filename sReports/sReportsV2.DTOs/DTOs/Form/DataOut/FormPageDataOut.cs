using sReportsV2.Common.CustomAttributes;
using sReportsV2.DTOs.Field.DataOut;
using System.Collections.Generic;
using System.Linq;

namespace sReportsV2.DTOs.Form.DataOut
{
    public class FormPageDataOut
    {
        [DataProp]
        public string Id { get; set; }
        [DataProp]
        public string Title { get; set; }
        [DataProp]
        public bool IsVisible { get; set; }
        [DataProp]
        public string Description { get; set; }
        [DataProp]
        public int ThesaurusId { get; set; }
        [DataProp]
        public FormPageImageMapDataOut ImageMap { get; set; }
        public List<List<FormFieldSetDataOut>> ListOfFieldSets { get; set; } = new List<List<FormFieldSetDataOut>>();
        [DataProp]
        public FormLayoutStyleDataOut LayoutStyle { get; set; }
        public bool DoesAllMandatoryFieldsHaveValue { get; set; }
        public bool IsLocked { get; set; }
        public bool CanBeLockedNext { get; set; }

        public void SetDoesAllMandatoryFieldsHaveValue()
        {
            DoesAllMandatoryFieldsHaveValue = ListOfFieldSets
                .SelectMany(repFs => repFs
                    .SelectMany(fs => fs.Fields)
                        ).Where(f => f.IsRequired && f.IsVisible)
                            .All(f => !string.IsNullOrEmpty(f.GetValue()));
        }

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
            return DoesAllMandatoryFieldsHaveValue && !IsLocked && CanBeLockedNext && hasPermissionToLock;
        }
    }
}