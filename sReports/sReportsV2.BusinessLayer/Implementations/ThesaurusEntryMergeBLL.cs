using sReportsV2.BusinessLayer.Helpers;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Entities.User;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Services.Implementations;
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.ThesaurusEntry;
using sReportsV2.DTOs.ThesaurusEntry.DataIn;
using sReportsV2.DTOs.User.DataIn;
using sReportsV2.DTOs.User.DTO;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sReportsV2.BusinessLayer.Implementations
{
    public partial class ThesaurusEntryBLL : IThesaurusEntryBLL
    {
        private const string MergeCodes = "Codes";
        private const string MergeDefinitions = "Definition";
        private const string MergeAbbreviations = "Abbreviations";
        private const string MergeSynonyms = "Synonyms";
        private const string MergeTranslations = "Translations";

        public void TakeBoth(int currentId)
        {
            int? productionStateCD = codeDAL.GetByCodeSetIdAndPreferredTerm((int)CodeSetList.ThesaurusState, CodeAttributeNames.Production);
            thesaurusDAL.UpdateState(currentId, productionStateCD);
        }

        public async Task MergeThesauruses(ThesaurusMergeDataIn thesaurusMergeDataIn, UserData userData)
        {
            ThesaurusEntry sourceThesaurus = thesaurusDAL.GetById(thesaurusMergeDataIn.CurrentId);
            ThesaurusEntry targetThesaurus = thesaurusDAL.GetById(thesaurusMergeDataIn.TargetId) ?? throw new ArgumentNullException($"Target thesaurus with given id (id=${thesaurusMergeDataIn.TargetId}) does not exist");

            await MergeValues(sourceThesaurus, targetThesaurus, thesaurusMergeDataIn.ValuesForMerge, userData);

            thesaurusMergeDAL.InsertOrUpdate(mapper.Map<ThesaurusMerge>(thesaurusMergeDataIn));
        }

        public async Task<int> MergeThesaurusOccurences(UserCookieData userCookieData)
        {
            DateTime startTime = DateTimeExtension.GetCurrentDateTime(userCookieData.TimeZoneOffset);
            try
            {
                UserData userData = mapper.Map<UserData>(userCookieData);
                int numOfUpdatedCachedEntries = 0;
                int? pendingStateCD = codeDAL.GetByCodeSetIdAndPreferredTerm((int)CodeSetList.ThesaurusMergeState, CodeAttributeNames.Pending);

                foreach (ThesaurusMerge thesaurusMerge in thesaurusMergeDAL.GetAllByState(pendingStateCD))
                {
                    numOfUpdatedCachedEntries += Replace(thesaurusMerge, userData);
                    await TryDelete(thesaurusMerge.OldThesaurus, applyUsingThesaurusInCodeCheck: false, organizationTimeZone: userCookieData.OrganizationTimeZone).ConfigureAwait(false);
                }
                DateTime endTime = DateTimeExtension.GetCurrentDateTime(userCookieData.TimeZoneOffset);
                SendEmail(userCookieData, startTime, endTime);

                return numOfUpdatedCachedEntries;
            }
            catch (Exception ex)
            {
                DateTime endTime = DateTimeExtension.GetCurrentDateTime(userCookieData.TimeZoneOffset);
                SendEmail(userCookieData, startTime, endTime, noErrors: false);
                throw;
            }
        }

        #region Merge Thesaurus Values
        private async Task MergeValues(ThesaurusEntry sourceThesaurus, ThesaurusEntry targetThesaurus, List<string> valuesForMerge, UserData userData)
        {
            if (valuesForMerge != null)
            {
                UpdateMergeListIfThereAreDependentActions(valuesForMerge);
                //TODO: Merge SKOS Data
                foreach (string value in valuesForMerge)
                {
                    switch (value)
                    {
                        case MergeTranslations:
                            targetThesaurus.MergeTranslations(sourceThesaurus);
                            break;
                        case MergeCodes:
                            targetThesaurus.MergeCodes(sourceThesaurus);
                            break;
                        case MergeSynonyms:
                            targetThesaurus.MergeSynonyms(sourceThesaurus);
                            break;
                        case MergeAbbreviations:
                            targetThesaurus.MergeAbbreviations(sourceThesaurus);
                            break;
                        case MergeDefinitions:
                            targetThesaurus.MergeDefinitions(sourceThesaurus);
                            break;
                        default:
                            break;
                    }
                }

                await CreateThesaurus(mapper.Map<ThesaurusEntryDataIn>(targetThesaurus), userData).ConfigureAwait(false);
            }
        }

        private void UpdateMergeListIfThereAreDependentActions(List<string> valuesForMerge)
        {
            if (valuesForMerge.Contains(MergeTranslations))
            {
                valuesForMerge.Remove(MergeSynonyms);
                valuesForMerge.Remove(MergeAbbreviations);
                valuesForMerge.Remove(MergeDefinitions);
            }
        }
        #endregion /Merge Thesaurus Values

        #region Merge Thesaurus Occurences
        private int Replace(ThesaurusMerge thesaurusMerge, UserData userData)
        {
            thesaurusMerge.FailedCollections = new List<string>();
            thesaurusMerge.CompletedCollections = new List<string>();
            int numOfCachedReplacements = 0;
            numOfCachedReplacements += ReplaceThesaurus(formDAL, thesaurusMerge, userData: userData);
            numOfCachedReplacements += ReplaceThesaurus(formInstanceDAL, thesaurusMerge);
            numOfCachedReplacements += ReplaceThesaurus(formDistributionDAL, thesaurusMerge);
            numOfCachedReplacements += ReplaceThesaurus(episodeOfCareDAL, thesaurusMerge);
            numOfCachedReplacements += ReplaceThesaurus(codeDAL, thesaurusMerge, cachedData: true);
            numOfCachedReplacements += ReplaceThesaurus(codeSetDAL, thesaurusMerge, cachedData: true);

            int? completedStateCD = codeDAL.GetByCodeSetIdAndPreferredTerm((int)CodeSetList.ThesaurusMergeState, CodeAttributeNames.Completed);
            int? notCompletedStateCD = codeDAL.GetByCodeSetIdAndPreferredTerm((int)CodeSetList.ThesaurusMergeState, CodeAttributeNames.NotCompleted);

            thesaurusMerge.StateCD = thesaurusMerge.FailedCollections.Count == 0 ? completedStateCD : notCompletedStateCD;
            thesaurusMerge.SetLastUpdate();

            return numOfCachedReplacements;
        }

        private int ReplaceThesaurus(IReplaceThesaurusDAL replaceThesaurusDAL, ThesaurusMerge thesaurusMerge, bool cachedData = false, UserData userData = null)
        {
            int numOfReplacements = replaceThesaurusDAL.ReplaceThesaurus(thesaurusMerge, userData);
            thesaurusMerge.SaveMergeOccurenceOutcome(replaceThesaurusDAL.ThesaurusExist(thesaurusMerge.OldThesaurus), replaceThesaurusDAL.GetType().Name);
            return cachedData ? numOfReplacements : 0;
        }

        private void SendEmail(UserCookieData userCookieData, DateTime startTime, DateTime endTime, bool noErrors = true)
        {
            string email = userCookieData.Email;
            if (!string.IsNullOrEmpty(email))
            {
                string mailContent = EmailHelpers.GetThesaurusMergeEmailContent(userCookieData, startTime, endTime, noErrors);
                Task.Run(() => emailSender.SendAsync(new EmailDTO(email, mailContent, $"{EmailSenderNames.SoftwareName} Merge Thesaurus Report")));
            }
            
        }
        #endregion /Merge Thesaurus Occurences
    }
}
