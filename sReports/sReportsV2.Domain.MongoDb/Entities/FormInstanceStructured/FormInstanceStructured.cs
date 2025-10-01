using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Entities.Common;
using sReportsV2.Domain.Entities.FieldEntity;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.Domain.Extensions;

namespace sReportsV2.Domain.Entities.Form
{
    public partial class Form
    {
        public Form(FormInstance.FormInstance formInstance, Form form)
        {
            formInstance = Ensure.IsNotNull(formInstance, nameof(formInstance));
            form = Ensure.IsNotNull(form, nameof(form));

            this.Id = formInstance.FormDefinitionId;
            this.About = form.About;
            this.Chapters = form.Chapters;
            this.Title = formInstance.Title;
            this.Version = formInstance.Version;
            this.Language = formInstance.Language;
            this.ThesaurusId = form.ThesaurusId;
            this.Notes = formInstance.Notes;
            this.Date = formInstance.Date;
            this.FormState = formInstance.FormState;
            this.UserId = formInstance.UserId;
            this.SetFieldInstances(formInstance.FieldInstances);
            this.CustomHeaderFields = form.CustomHeaderFields;
            this.SetInitialOrganizationId(formInstance.OrganizationId);
        }

        #region Set Form Instance
        public void SetFieldInstances(List<FieldInstance> fieldInstances)
        {
            fieldInstances = Ensure.IsNotNull(fieldInstances, nameof(fieldInstances));

            foreach (var repetitiveFieldSetList in GetAllListOfFieldSets())
            {
                var firstFieldSet = repetitiveFieldSetList?.FirstOrDefault();

                if (firstFieldSet?.ListOfFieldSets?.Count > 0)
                {
                    foreach (var fieldSet in firstFieldSet.ListOfFieldSets)
                    {
                        SetFieldInstances(new List<FieldSet> { fieldSet }, fieldInstances);
                    }
                }
                else
                {
                    SetFieldInstances(repetitiveFieldSetList, fieldInstances);
                }
            }
        }

        private void SetFieldInstances(List<FieldSet> repetitiveFieldSetList, List<FieldInstance> fieldInstances)
        {
            IEnumerable<FieldInstance> fieldInstancesRelatedToFieldSet = fieldInstances.Where(x => x.FieldSetId == repetitiveFieldSetList.FirstOrDefault()?.Id);

            PrepareFieldSet(repetitiveFieldSetList, fieldInstancesRelatedToFieldSet);

            foreach (Field field in repetitiveFieldSetList.SelectMany(x => x.Fields))
            {
                List<FieldInstanceValue> fieldInstanceValues = fieldInstancesRelatedToFieldSet
                     .FirstOrDefault(fI => fI.FieldSetInstanceRepetitionId == field.FieldSetInstanceRepetitionId
                             && fI.FieldId == field.Id)?
                         .FieldInstanceValues;

                if (fieldInstanceValues.HasAnyFieldInstanceValue())
                {
                    field.FieldInstanceValues = fieldInstanceValues;
                }
                else
                {
                    field.FieldInstanceValues = new List<FieldInstanceValue>()
                    {
                        new FieldInstanceValue(string.Empty)
                    };
                }
            }
        }

        private void PrepareFieldSet(List<FieldSet> repetitiveFieldSetList, IEnumerable<FieldInstance> fieldInstancesRelatedToFieldSet)
        {
            if (repetitiveFieldSetList.Count == 1)
            {
                IList<string> fieldSetInstanceRepetitionIds = fieldInstancesRelatedToFieldSet.Select(x => x.FieldSetInstanceRepetitionId).Distinct().ToList();

                FieldSet firstFieldSetInRepetition = repetitiveFieldSetList.First();

                if (fieldSetInstanceRepetitionIds.Any())
                {
                    firstFieldSetInRepetition.SetFieldSetInstanceRepetitionIds(fieldSetInstanceRepetitionIds[0]);
                    fieldSetInstanceRepetitionIds.RemoveAt(0);
                }
                else
                {
                    firstFieldSetInRepetition.SetFieldSetInstanceRepetitionIds(GuidExtension.NewGuidStringWithoutDashes());
                }

                foreach (string fieldSetInstanceRepetitionId in fieldSetInstanceRepetitionIds)
                {
                    FieldSet repetitiveFieldSet = firstFieldSetInRepetition.Clone();
                    repetitiveFieldSet.SetFieldSetInstanceRepetitionIds(fieldSetInstanceRepetitionId);
                    repetitiveFieldSetList.Add(repetitiveFieldSet);
                }
            }
        }

        #endregion /Set Form Instance

        #region Referrable logic
        public List<Field> GetAllFieldsFromNonRepetititveFieldSets()
        {
            return this.Chapters
                        .SelectMany(chapter => chapter.Pages
                            .SelectMany(page => page.ListOfFieldSets.Where(x => !x[0].IsRepetitive)
                                .SelectMany(list => list.SelectMany(set => set.Fields)
                                )
                            )
                        ).ToList();
        }

        public void SetValuesFromReferrals(List<Form> formInstances)
        {
            formInstances = Ensure.IsNotNull(formInstances, nameof(formInstances));
            this.SetFieldsReferralFromRepetitiveFieldSets(formInstances);
            List<Field> allFieldsFromNonRepetitiveFieldsets = formInstances
                .SelectMany(x => x.GetAllFieldsFromNonRepetititveFieldSets())
                .ToList();
            this.SetFieldsReferralFromNonRepetitiveFieldSets(allFieldsFromNonRepetitiveFieldsets);
        }

        private void SetFieldsReferralFromNonRepetitiveFieldSets(List<Field> allReferralsFields)
        {
            foreach (Field field in this.GetAllFieldsFromNonRepetititveFieldSets())
            {
                Field referralField = allReferralsFields.Find(x => x.IsReferrable(field));
                field.SetReferral(referralField);
            }
        }

        private void SetFieldsReferralFromRepetitiveFieldSets(List<Form> formInstances)
        {
            formInstances = Ensure.IsNotNull(formInstances, nameof(formInstances));
            List<List<FieldSet>> referralsRepetiveFieldSets = formInstances.SelectMany(x => x.GetFieldSetsByRepetitivity(true)).ToList();
            foreach (List<FieldSet> formFieldSet in this.GetFieldSetsByRepetitivity(true))
            {
                foreach (List<FieldSet> referralFieldSet in referralsRepetiveFieldSets)
                {
                    if (formFieldSet[0].IsReferable(referralFieldSet[0]))
                    {
                        int startingNewFSIndex = formFieldSet.Count;
                        SetFieldsReferralForExistingRepetitiveFieldSets(startingNewFSIndex, formFieldSet, referralFieldSet);
                        SetFieldsReferralForAddedRepetitiveFieldSets(startingNewFSIndex, formFieldSet, referralFieldSet);
                    }
                }
            }
        }

        private void SetFieldsReferralForExistingRepetitiveFieldSets(int startingNewFSIndex, List<FieldSet> formFieldSet, List<FieldSet> referralFieldSet)
        {
            for (int i = 0; i < startingNewFSIndex; i++)
            {
                FieldSet targetFieldSet = formFieldSet[i];
                targetFieldSet.SetReferralFields(referralFieldSet[i]);
            }
        }

        private void SetFieldsReferralForAddedRepetitiveFieldSets(int startingNewFSIndex, List<FieldSet> formFieldSet, List<FieldSet> referralFieldSet)
        {
            for (int i = startingNewFSIndex; i < referralFieldSet.Count; i++)
            {
                FieldSet fieldSetToBeAdded = referralFieldSet[i].Clone();
                fieldSetToBeAdded.SetFieldSetInstanceRepetitionIds(GuidExtension.NewGuidStringWithoutDashes());
                foreach (FieldInstanceValue fieldInstanceValue in fieldSetToBeAdded.Fields.SelectMany(f => f.FieldInstanceValues))
                {
                    fieldInstanceValue.FieldInstanceRepetitionId = GuidExtension.NewGuidStringWithoutDashes();
                }
                formFieldSet.Add(fieldSetToBeAdded);
            }
        }

        public List<ReferalInfo> GetValuesFromReferrals(List<Form> formInstances, Dictionary<int, Dictionary<int, string>> missingValuesDict)
        {
            List<ReferalInfo> result = new List<ReferalInfo>();

            result.AddRange(this.GetReferalInfoFromRepetitiveFieldSets(formInstances, missingValuesDict));
            result.AddRange(this.GetReferalInfoFromNonRepetitiveFieldSets(formInstances, missingValuesDict));

            return result;
        }

        private List<ReferalInfo> GetReferalInfoFromRepetitiveFieldSets(List<Form> formInstances, Dictionary<int, Dictionary<int, string>> missingValuesDict)
        {
            formInstances = Ensure.IsNotNull(formInstances, nameof(formInstances));

            List<ReferalInfo> result = new List<ReferalInfo>();
            List<int> thesaurusesAdded = new List<int>();

            foreach (Form instance in formInstances)
            {
                ReferalInfo referalInfo = new ReferalInfo(instance);

                foreach (List<FieldSet> formFieldSet in this.GetFieldSetsByRepetitivity(true))
                {
                    foreach (List<FieldSet> referralFieldSet in instance.GetFieldSetsByRepetitivity(true))
                    {
                        if (formFieldSet[0].IsReferable(referralFieldSet[0]) && !thesaurusesAdded.Contains(formFieldSet[0].ThesaurusId))
                        {
                            thesaurusesAdded.Add(formFieldSet[0].ThesaurusId);
                            AddReferrableFieldsFromRepetitiveFieldSets(referalInfo, referralFieldSet, missingValuesDict);
                        }
                    }
                }

                result.Add(referalInfo);
            }

            return result;
        }

        private void AddReferrableFieldsFromRepetitiveFieldSets(ReferalInfo referalInfo, List<FieldSet> referralFieldSet, Dictionary<int, Dictionary<int, string>> missingValuesDict)
        {
            Ensure.IsNotNull(referalInfo, nameof(referalInfo));
            Ensure.IsNotNull(referralFieldSet, nameof(referralFieldSet));

            for (int i = 0; i < referralFieldSet.Count; i++)
            {
                foreach (Field referralField in referralFieldSet[i].Fields)
                {
                    AddReferrableFieldToReferralInfo(referralField.HasValue(), referalInfo, referralField, missingValuesDict, i);
                }
            }
        }

        private List<ReferalInfo> GetReferalInfoFromNonRepetitiveFieldSets(List<Form> formInstances, Dictionary<int, Dictionary<int, string>> missingValuesDict)
        {
            formInstances = Ensure.IsNotNull(formInstances, nameof(formInstances));
            List<ReferalInfo> result = new List<ReferalInfo>();
            List<ReferralForm> allReferrals = formInstances
                .Select(x => new ReferralForm(x))
                .ToList();


            foreach (ReferralForm referral in allReferrals)
            {
                ReferalInfo referalInfo = new ReferalInfo(referral);

                foreach (Field field in this.GetAllFieldsFromNonRepetititveFieldSets())
                {
                    Field referralField = referral.Fields.Find(x => x.ThesaurusId == field.ThesaurusId && x.Type == field.Type);

                    if (referralField != null/* && !addedThesauruses.Contains(referralField.ThesaurusId)*/)
                    {
                        AddReferrableFieldToReferralInfo(
                            (IsFieldString(referralField) || IsFieldSelectable(referralField)) && referralField.HasValue(),
                            referalInfo,
                            referralField,
                            missingValuesDict
                            );
                    }
                }
                result.Add(referalInfo);
            }

            return result;
        }

        private void AddReferrableFieldToReferralInfo(bool addReferralField, ReferalInfo referalInfo, Field referralField, Dictionary<int, Dictionary<int, string>> missingValuesDict, int fieldSetPosition = -1)
        {
            Ensure.IsNotNull(referalInfo, nameof(referalInfo));
            Ensure.IsNotNull(referralField, nameof(referralField));

            if (addReferralField)
            {
                string repetitiveSuffix = fieldSetPosition > -1 ? $"({fieldSetPosition})" : string.Empty;
                referalInfo.ReferrableFields.Add(new KeyValue
                {
                    Key = $"{referralField.Label}{repetitiveSuffix}",
                    Value = referralField.GetReferrableValue(missingValuesDict),
                    ThesaurusId = referralField.ThesaurusId
                });
            }
        }

        private bool IsFieldString(Field referralField)
        {
            return referralField is FieldString;
        }

        private bool IsFieldSelectable(Field referralField)
        {
            return referralField is FieldSelectable;
        }

        private List<List<FieldSet>> GetFieldSetsByRepetitivity(bool isRepetitive)
        {
            return this.Chapters
                        .SelectMany(chapter => chapter.Pages
                            .SelectMany(page => page.ListOfFieldSets
                            )
                        ).Where(list => list[0].IsRepetitive == isRepetitive).ToList();

        }

        #endregion /Referrable logic

        public (Dictionary<string, string>, Dictionary<string, string>) GetFieldToFieldSetMapping()
        {
            Dictionary<string, string> fieldToFieldSetMapping = new Dictionary<string, string>();
            Dictionary<string, string> fieldSetInstanceRepetitionIds = new Dictionary<string, string>();
            foreach (FieldSet fieldSet in CollectAllFieldSets())
            {
                fieldSetInstanceRepetitionIds.Add(fieldSet.Id, GuidExtension.NewGuidStringWithoutDashes());
                foreach (Field field in fieldSet.Fields)
                {
                    fieldToFieldSetMapping[field.Id] = fieldSet.Id;
                }
            }
            return (fieldToFieldSetMapping, fieldSetInstanceRepetitionIds);
        }
    }
}
