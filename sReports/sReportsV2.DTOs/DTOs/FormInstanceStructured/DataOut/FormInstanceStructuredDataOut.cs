using sReportsV2.Common.Constants;
using sReportsV2.Common.Enums;
using sReportsV2.Domain.MongoDb.Entities.FormInstance;
using sReportsV2.DTOs.DTOs.FormInstance.DataIn;
using sReportsV2.DTOs.DTOs.FormInstance.DataOut;
using sReportsV2.DTOs.DTOs.FormInstanceStructured.DataOut;
using sReportsV2.DTOs.Field.DataOut;
using System;
using System.Collections.Generic;
using System.Linq;

namespace sReportsV2.DTOs.Form.DataOut
{
    public partial class FormDataOut
    {
        public bool IsParameterize { get; set; }
        public string Notes { get; set; }
        public DateTime? Date { get; set; }
        public FormState? FormState { get; set; }
        public bool DoesAllMandatoryFieldsHaveValue { get; set; }
        public string ActiveChapterId { get; set; }
        public int? ActivePageLeftScroll { get; set; }
        public string ActivePageId { get; set; }
        public List<FieldDataOut> RequiredFieldsWithoutValue { get; set; } = new List<FieldDataOut>();
        private Dictionary<string, List<DependentOnInstanceInfoDataOut>> parentFieldInstanceDependencies = new Dictionary<string, List<DependentOnInstanceInfoDataOut>>();

        public Dictionary<string, List<DependentOnInstanceInfoDataOut>> ParentFieldInstanceDependencies
        {
            get => parentFieldInstanceDependencies;
            set => parentFieldInstanceDependencies = value ?? new Dictionary<string, List<DependentOnInstanceInfoDataOut>>();
        }

        #region Form Instance Methods
        public void SetDoesAllMandatoryFieldsHaveValue()
        {
            foreach (FormFieldSetDataOut fieldSet in GetAllFieldSets())
            {
                fieldSet.SetDoesAllMandatoryFieldsHaveValue();
            }

            foreach (FormPageDataOut page in Chapters.SelectMany(ch => ch.Pages))
            {
                page.DoesAllMandatoryFieldsHaveValue = page.ListOfFieldSets
                    .SelectMany(fs => fs)
                    .All(fs => fs.DoesAllMandatoryFieldsHaveValue || !fs.GetVisibleAndRequiredFields().Any());
            }

            foreach (FormChapterDataOut chapter in Chapters)
            {
                chapter.DoesAllMandatoryFieldsHaveValue = chapter.Pages.Select(p => p.DoesAllMandatoryFieldsHaveValue).All(p => p);
            }

            this.DoesAllMandatoryFieldsHaveValue = Chapters.Select(ch => ch.DoesAllMandatoryFieldsHaveValue).All(ch => ch);
        }


        public int CountAllFieldsWhichCanSaveWithoutValue(string chapterId)
        {
            return this.Chapters
                        .Where(chapter => chapter.Id == chapterId)
                        .SelectMany(chapter => chapter.Pages
                            .SelectMany(page =>
                                page.ListOfFieldSets
                                    .SelectMany(fieldSet => fieldSet.SelectMany(field => field.Fields
                                        .Where(x => x.FieldInstanceValues.Any()
                                            && x.FieldInstanceValues.FirstOrDefault()?.Values.Count == 0
                                            && x.AllowSaveWithoutValue.HasValue && x.AllowSaveWithoutValue.Value
                                            && x.IsVisible)
                                    ))
                                .Concat(
                                page.ListOfFieldSets
                                    .Select(lfs => lfs.FirstOrDefault()?.ListOfFieldSets)
                                    .Where(list => list != null)
                                    .SelectMany(fieldSet => fieldSet.SelectMany(field => field.Fields
                                        .Where(x => x.FieldInstanceValues.Any()
                                            && x.FieldInstanceValues.FirstOrDefault()?.Values.Count == 0
                                            && x.AllowSaveWithoutValue.HasValue && x.AllowSaveWithoutValue.Value
                                            && x.IsVisible)
                                    )))
                            )
                        )
                        .Count();
        }

        public List<FormFieldSetDataOut> GetFieldSetDefinitions()
        {
            return this.Chapters
                        .SelectMany(chapter => chapter.Pages
                            .SelectMany(page => page.ListOfFieldSets
                                .Select(fieldSet => fieldSet.FirstOrDefault())
                                    .Where(fS => fS != null)
                            )
                        )
                        .ToList();
        }

        public (FormFieldSetDataOut FieldSet, bool FoundInNested) GetFieldSet(string fieldSetId)
        {
            var fieldSet = this.Chapters
                       .SelectMany(chapter => chapter.Pages)
                       .SelectMany(page => page.ListOfFieldSets)
                       .SelectMany(innerList => innerList)
                       .FirstOrDefault(fieldSet => fieldSet != null && fieldSet.Id == fieldSetId);

            if (fieldSet != null)
            {
                return (fieldSet, false);
            }

            fieldSet = this.Chapters
                      .SelectMany(chapter => chapter.Pages)
                      .SelectMany(page => page.ListOfFieldSets)
                        .Select(lfs => lfs.FirstOrDefault().ListOfFieldSets)
                      .SelectMany(innerList => innerList)
                      .FirstOrDefault(fieldSet => fieldSet != null && fieldSet.Id == fieldSetId);

            return (fieldSet, fieldSet != null);
        }

        public List<FormFieldSetDataOut> GetAllFieldSets()
        {
            return this.Chapters
                        .SelectMany(chapter => chapter.Pages
                            .SelectMany(page => page.ListOfFieldSets
                                .SelectMany(fieldSet => fieldSet)
                            )
                        )
                        .ToList();
        }

        public List<FieldDataOut> GetAllFieldsWhichCannotSaveWithoutValue(List<string> fieldsIds)
        {
            return this.Chapters
                .SelectMany(chapter => chapter.Pages
                    .SelectMany(page =>
                        page.ListOfFieldSets
                            .SelectMany(list => list.SelectMany(set => set.Fields
                                .Where(y => fieldsIds.Contains(y.Id) && y.AllowSaveWithoutValue.HasValue && !y.AllowSaveWithoutValue.Value)
                            ))
                        .Concat(
                        page.ListOfFieldSets
                            .Select(lfs => lfs.FirstOrDefault()?.ListOfFieldSets)
                            .Where(list => list != null)
                            .SelectMany(list => list.SelectMany(set => set.Fields
                                .Where(y => fieldsIds.Contains(y.Id) && y.AllowSaveWithoutValue.HasValue && !y.AllowSaveWithoutValue.Value)
                            )))
                    )
                )
                .ToList();
        }

        public List<FieldDataOut> GetAllFieldsWhichCanSaveWithoutValue(List<string> fieldsIds)
        {
            return this.Chapters
                .SelectMany(chapter => chapter.Pages
                    .SelectMany(page =>
                        page.ListOfFieldSets
                            .SelectMany(list => list.SelectMany(set => set.Fields.Where(y => fieldsIds.Contains(y.Id))))
                            .Concat(
                            page.ListOfFieldSets
                                .Select(lfs => lfs.FirstOrDefault()?.ListOfFieldSets)
                                .Where(list => list != null)
                                .SelectMany(list => list.SelectMany(set => set.Fields
                                    .Where(y => fieldsIds.Contains(y.Id)))))
                    )
                )
                .ToList();
        }

        public void SetIfChaptersAndPagesAreLocked(FormInstanceItemLockingStatus formInstanceLockingStatus)
        {
            foreach (FormChapterDataOut chapter in Chapters)
            {
                FormInstanceItemLockingStatus chapterInstanceLockingStatus = formInstanceLockingStatus.GetChild(chapter.Id);
                if (chapterInstanceLockingStatus != null)
                {
                    chapter.IsLocked = chapterInstanceLockingStatus.IsLocked;

                    bool canBeLockedNext = true;
                    foreach (FormPageDataOut page in chapter.Pages)
                    {
                        FormInstanceItemLockingStatus pageInstanceLockingStatus = chapterInstanceLockingStatus.GetChild(page.Id);
                        if (pageInstanceLockingStatus != null)
                        {
                            page.IsLocked = pageInstanceLockingStatus.IsLocked;
                            page.CanBeLockedNext = canBeLockedNext;
                            canBeLockedNext &= page.IsLocked;

                            foreach (FormFieldSetDataOut fieldSet in page.ListOfFieldSets.SelectMany(x => x))
                            {
                                if (fieldSet.ListOfFieldSets.Count > 0)
                                {
                                    foreach (FormFieldSetDataOut item in fieldSet.ListOfFieldSets)
                                    {
                                        FormInstanceItemLockingStatus fieldSetInstanceLockingStatus = pageInstanceLockingStatus.GetChild(item.FieldSetInstanceRepetitionId);
                                        item.IsLocked = fieldSetInstanceLockingStatus != null && fieldSetInstanceLockingStatus.IsLocked;
                                    }
                                }
                                else
                                {
                                    FormInstanceItemLockingStatus fieldSetInstanceLockingStatus = pageInstanceLockingStatus.GetChild(fieldSet.FieldSetInstanceRepetitionId);
                                    fieldSet.IsLocked = fieldSetInstanceLockingStatus != null && fieldSetInstanceLockingStatus.IsLocked;
                                }
                            }
                        }
                    }
                }
            }
        }
        public void SetActiveChapterAndPageId(FormInstanceReloadDataIn formInstanceReloadData)
        {
            if (formInstanceReloadData == null)
            {
                formInstanceReloadData = new FormInstanceReloadDataIn();
            }
            if (string.IsNullOrEmpty(formInstanceReloadData?.ActiveChapterId))
            {
                formInstanceReloadData.ActiveChapterId = this.Chapters.FirstOrDefault()?.Id;
            }
            if (string.IsNullOrEmpty(formInstanceReloadData?.ActivePageId))
            {
                formInstanceReloadData.ActivePageId = this.Chapters.FirstOrDefault()?.Pages?.FirstOrDefault()?.Id;
            }
            this.ActiveChapterId = formInstanceReloadData.ActiveChapterId;
            this.ActivePageId = formInstanceReloadData.ActivePageId;
            this.ActivePageLeftScroll = formInstanceReloadData.ActivePageLeftScroll;
        }
        public bool IsFormInstanceLockedOrUnlocked()
        {
            return this.FormState == sReportsV2.Common.Enums.FormState.Locked ||
                this.FormState == sReportsV2.Common.Enums.FormState.Unlocked;
        }

        public bool IsFormInstanceLocked()
        {
            return this.FormState == sReportsV2.Common.Enums.FormState.Locked;
        }

        public bool IsFormInstanceInActiveState()
        {
            return !this.FormState.HasValue || (this.FormState.Equals(sReportsV2.Common.Enums.FormState.OnGoing) || this.FormState.Equals(sReportsV2.Common.Enums.FormState.InError) || this.FormState.Equals(sReportsV2.Common.Enums.FormState.Unlocked));

        }

        public string GetTimeZone()
        {
            return Organizations?.FirstOrDefault()?.TimeZone ?? string.Empty;
        }

        public void SetConnectionsForConnectedField()
        {
            List<FieldDataOut> fields = this.GetAllFields();
            foreach (FieldDataOut field in fields.Where(f => f.Type == FieldTypes.Connected))
            {
                FieldConnectedDataOut fieldConnected = field as FieldConnectedDataOut;
                List<FieldConnectedOptionDataOut> datasource = new List<FieldConnectedOptionDataOut>();
                foreach (FieldDataOut connectedField in fields.Where(f => fieldConnected.ConnectedFieldIds.Contains(f.Id)).OrderBy(f => f.Id))
                {
                    datasource.AddRange(connectedField.GetConnectedFieldDataSource());
                }
                fieldConnected.ConnectedFieldDataSource = datasource;
            }
        }

        #endregion /Form Instance Methods

        #region Dependency Handling
        public List<FieldInstanceDTO> CreateParentDependableStructure(List<FieldDataOut> populatedFieldInstances)
        {
            List<FieldInstanceDTO> populatedParentFieldInstances = new List<FieldInstanceDTO>();

            ParentFieldInstanceDependencies = new Dictionary<string, List<DependentOnInstanceInfoDataOut>>();

            Dictionary<string, bool> fieldSetRepetitive = new Dictionary<string, bool>();
            foreach (var fieldSet in GetFieldSetDefinitions())
            {
                if (!fieldSetRepetitive.ContainsKey(fieldSet.Id))
                    fieldSetRepetitive.Add(fieldSet.Id, fieldSet.IsRepetitive);
            }

            foreach (FieldDataOut childDependentField in populatedFieldInstances.Where(f => HasDependentOn(f)))
            {
                childDependentField.AddMissingPropertiesInDependency(this);
                DependentOnInfoDataOut dependentOnInfo = childDependentField.DependentOn;
                foreach (var grouping in dependentOnInfo.DependentOnFieldInfos.GroupBy(x => x.FieldId))
                {
                    DependentOnFieldInfoDataOut dependendOnField = grouping.First();
                    FieldDataOut parentField = GetParentField(childDependentField, populatedFieldInstances, dependendOnField);
                    if (parentField != null)
                    {
                        foreach (FieldInstanceValueDataOut parentFieldInstanceValue in parentField.FieldInstanceValues)
                        {
                            populatedParentFieldInstances.Add(new FieldInstanceDTO(parentField, parentFieldInstanceValue));

                            IEnumerable<DependentOnInstanceInfoDataOut> upcomingChildFieldInstanceDependencies = GetUpcomingChildFieldInstanceDependencies(
                                childDependentField,
                                parentFieldInstanceValue.FieldInstanceRepetitionId,
                                fieldSetRepetitive[childDependentField.FieldSetId]
                                );
                            AppendChildDependencies(parentFieldInstanceValue.FieldInstanceRepetitionId, upcomingChildFieldInstanceDependencies);
                        }
                    }
                }
            }

            return populatedParentFieldInstances;

        }

        private FieldDataOut GetParentField(FieldDataOut childDependentField, List<FieldDataOut> populatedFieldInstances, DependentOnFieldInfoDataOut dependendOnField)
        {
            FieldDataOut parentField = populatedFieldInstances.Find(f =>
                f.Id == dependendOnField.FieldId
                && f.FieldSetInstanceRepetitionId == childDependentField.FieldSetInstanceRepetitionId
                )
                ?? populatedFieldInstances.Find(f => f.Id == dependendOnField.FieldId);
            return parentField;
        }

        private IEnumerable<DependentOnInstanceInfoDataOut> GetUpcomingChildFieldInstanceDependencies(FieldDataOut childDependentField, string parentFieldInstanceRepetitionId, bool isChildDependentFieldSetRepetitive)
        {
            return childDependentField
                .FieldInstanceValues
                .Select(x => new DependentOnInstanceInfoDataOut(childDependentField.DependentOn)
                {
                    ChildFieldInstanceRepetitionId = x.FieldInstanceRepetitionId,
                    ChildFieldSetInstanceRepetitionId = childDependentField.FieldSetInstanceRepetitionId,
                    ChildFieldInstanceCssSelector = childDependentField.GetChildFieldInstanceCssSelector(x.FieldInstanceRepetitionId),
                    ParentFieldInstanceCssSelector = childDependentField.GetParentFieldInstanceCssSelector(parentFieldInstanceRepetitionId),
                    IsChildDependentFieldSetRepetitive = isChildDependentFieldSetRepetitive
                });
        }

        private bool HasDependentOn(FieldDataOut fieldDataOut)
        {
            return fieldDataOut.DependentOn != null && fieldDataOut.DependentOn.DependentOnFieldInfos != null && fieldDataOut.DependentOn.DependentOnFieldInfos.Any();
        }

        private void AppendChildDependencies(string parentFieldInstanceRepetitionId, IEnumerable<DependentOnInstanceInfoDataOut> upcomingChildFieldInstanceDependencies)
        {
            if (ParentFieldInstanceDependencies.TryGetValue(parentFieldInstanceRepetitionId, out List<DependentOnInstanceInfoDataOut> childFieldInstanceDependencies))
            {
                childFieldInstanceDependencies.AddRange(upcomingChildFieldInstanceDependencies);
            }
            else
            {
                ParentFieldInstanceDependencies.Add(
                    parentFieldInstanceRepetitionId,
                    new List<DependentOnInstanceInfoDataOut>(upcomingChildFieldInstanceDependencies)
                    );
            }
        }
        #endregion /Dependency Handling
    }
}