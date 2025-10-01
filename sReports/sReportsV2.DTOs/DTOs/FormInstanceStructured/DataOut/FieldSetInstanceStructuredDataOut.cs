using sReportsV2.DTOs.Field.DataOut;
using System;
using System.Collections.Generic;
using System.Linq;

namespace sReportsV2.DTOs.Form.DataOut
{
    public partial class FormFieldSetDataOut
    {
        public string FieldSetInstanceRepetitionId { get; set; }
        private Dictionary<string, List<DependentOnInstanceInfoDataOut>> allParentFieldInstanceDependencies = new Dictionary<string, List<DependentOnInstanceInfoDataOut>>();
        private Dictionary<string, List<DependentOnInstanceInfoDataOut>> parentFieldInstanceDependencies = new Dictionary<string, List<DependentOnInstanceInfoDataOut>>();

        public Dictionary<string, List<DependentOnInstanceInfoDataOut>> AllParentFieldInstanceDependencies
        {
            get => allParentFieldInstanceDependencies;
            set => allParentFieldInstanceDependencies = value ?? new Dictionary<string, List<DependentOnInstanceInfoDataOut>>();
        }

        public Dictionary<string, List<DependentOnInstanceInfoDataOut>> ParentFieldInstanceDependencies
        {
            get => parentFieldInstanceDependencies;
            set => parentFieldInstanceDependencies = value ?? new Dictionary<string, List<DependentOnInstanceInfoDataOut>>();
        }

        public bool IsLocked { get; set; }
        public bool DoesAllMandatoryFieldsHaveValue { get; set; }

        public void SetParentFieldInstanceDependencies(FormDataOut formDataOut)
        {
            IEnumerable<string> fieldInstanceRepetitionIdsINFieldSet = this.Fields.SelectMany(f => f.FieldInstanceValues).Select(fIv => fIv.FieldInstanceRepetitionId);

            this.AllParentFieldInstanceDependencies = formDataOut.ParentFieldInstanceDependencies;
            this.ParentFieldInstanceDependencies = formDataOut
                .ParentFieldInstanceDependencies
                .Where(x => fieldInstanceRepetitionIdsINFieldSet.Contains(x.Key))
                .ToDictionary(x => x.Key, x => x.Value);
        }

        public IEnumerable<FieldDataOut> GetFieldsForDependencyFormula(string openedFieldId)
        {
            this.Fields = this.Fields ?? new List<FieldDataOut>();
            return this.Fields.Where(f => f.Id != openedFieldId && f.CanBeInDependencyFormula() && !f.IsFieldRepetitive);
        }

        public bool CanFieldSetBeIncludedInDependencyFormula(FieldDataOut openedField)
        {
            return this.Id == openedField.FieldSetId || this.Fields.Exists(f => f.Id == openedField.Id) || !this.IsRepetitive;
        }

        public void SetDoesAllMandatoryFieldsHaveValue()
        {
            IEnumerable<FieldDataOut> visibleAndRequiredFields = GetVisibleAndRequiredFields();
            if (visibleAndRequiredFields.Any())
            {
                DoesAllMandatoryFieldsHaveValue = visibleAndRequiredFields.All(f => !string.IsNullOrEmpty(f.GetValue()));
            }
            else
            {
                DoesAllMandatoryFieldsHaveValue = Fields.Exists(f => f.IsVisible);
            }
        }

        public IEnumerable<FieldDataOut> GetVisibleAndRequiredFields()
        {
            return Fields
                .Where(f => f.IsRequired && f.IsVisible);
        }

        public bool ShouldLockActionBeShown(bool hasPermissionToLock)
        {
            return IsLocked && hasPermissionToLock && DoesAllMandatoryFieldsHaveValue;
        }

        public bool ShouldShowIndicatorWhenLocked(bool permissionIsTrue)
        {
            return !IsLocked && permissionIsTrue && GetVisibleAndRequiredFields().Any();
        }
    }
}
