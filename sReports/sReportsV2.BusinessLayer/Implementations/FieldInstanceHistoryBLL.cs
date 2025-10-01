using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sReportsV2.Domain.Entities.FieldInstanceHistory;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.DTOs.DTOs.FieldInstanceHistory.DataOut;
using sReportsV2.DTOs.DTOs.FieldInstanceHistory.FieldInstanceHistoryDataIn;
using sReportsV2.DTOs.Pagination;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.DTOs.Common.DataOut;

namespace sReportsV2.BusinessLayer.Implementations
{
    public partial class FormInstanceBLL
    {
        public async Task<PaginationDataOut<FieldInstanceHistoryDataOut, FieldInstanceHistoryFilterDataIn>> GetAllFieldHistoriesFiltered(FieldInstanceHistoryFilterDataIn fieldInstanceHistoryFilter)
        {
            fieldInstanceHistoryFilter = Ensure.IsNotNull(fieldInstanceHistoryFilter, nameof(fieldInstanceHistoryFilter));
            FieldInstanceHistoryFilterData filter = mapper.Map<FieldInstanceHistoryFilterData>(fieldInstanceHistoryFilter);

            List<FieldInstanceHistoryDataOut> histories = mapper.Map<List<FieldInstanceHistoryDataOut>>(await fieldInstanceHistoryDAL.GetAllFilteredAsync(filter).ConfigureAwait(false));

            await SetUserChangeInfos(histories).ConfigureAwait(false);

            int count = await fieldInstanceHistoryDAL.CountFilteredAsync(filter).ConfigureAwait(false);
            return new PaginationDataOut<FieldInstanceHistoryDataOut, FieldInstanceHistoryFilterDataIn>()
            {
                Data = histories,
                Count = count,
                DataIn = fieldInstanceHistoryFilter
            };
        }

        public async Task InsertOrUpdateManyFieldHistoriesAsync(FormInstance formInstance)
        {
            if (formInstance != null)
            {
                Dictionary<string, FieldInstanceHistory> lastFieldChanges = await GetLastFieldChanges(formInstance.Id).ConfigureAwait(false);
                IEnumerable<FieldSet> fieldSets = await formDAL.GetAllFieldSetsByFormId(formInstance.FormDefinitionId).ConfigureAwait(false);

                List<FieldInstanceHistory> fieldInstanceHistoriesToInsertOnCreate = new List<FieldInstanceHistory>();
                List<FieldInstanceHistory> fieldInstanceHistoriesToInsertOnUpdate = new List<FieldInstanceHistory>();

                int userId = formInstance.GetUserIdWhoMadeAction();

                foreach (FieldInstance fieldInstance in formInstance.FieldInstances)
                {
                    foreach(FieldInstanceValue repetitiveFieldInstanceValue in fieldInstance.FieldInstanceValues)
                    {
                        lastFieldChanges.TryGetValue(repetitiveFieldInstanceValue.FieldInstanceRepetitionId, out FieldInstanceHistory lastFieldInstanceHistoryChange);

                        if (lastFieldInstanceHistoryChange != null)
                        {
                            UpdateFieldInstanceHistoryIfChanged(
                                lastFieldInstanceHistoryChange,
                                userId,
                                ref fieldInstanceHistoriesToInsertOnUpdate,
                                repetitiveFieldInstanceValue);
                            lastFieldChanges.Remove(repetitiveFieldInstanceValue.FieldInstanceRepetitionId);
                        }
                        else
                        {
                            CreateNewFieldInstanceHistory(
                                fieldInstance,
                                formInstance,
                                fieldSets,
                                ref fieldInstanceHistoriesToInsertOnCreate,
                                repetitiveFieldInstanceValue);
                        }
                    }
                }

                AddChangeForDeletedRepetitiveFields(lastFieldChanges, ref fieldInstanceHistoriesToInsertOnUpdate, userId);

                if (fieldInstanceHistoriesToInsertOnUpdate.Count > 0)
                    await fieldInstanceHistoryDAL.UpdateManyAsync(fieldInstanceHistoriesToInsertOnUpdate).ConfigureAwait(false);
                if (fieldInstanceHistoriesToInsertOnCreate.Count > 0)
                    await fieldInstanceHistoryDAL.InsertManyAsync(fieldInstanceHistoriesToInsertOnCreate).ConfigureAwait(false);
            }
            else
                throw new ArgumentNullException($"{System.Reflection.MethodBase.GetCurrentMethod().Name} Received null argument: {nameof(formInstance)}.");
        }

        public async Task UpdateManyFieldHistoriesOnDeleteAsync(string formInstanceId, int userId)
        {
            if (!string.IsNullOrWhiteSpace(formInstanceId) && userId != 0)
            {
                List<FieldInstanceHistory> fieldInstanceHistories = await fieldInstanceHistoryDAL.GetAllFilteredAsync(new FieldInstanceHistoryFilterData() { FormInstanceId = formInstanceId }).ConfigureAwait(false);
                await UpdateManyFieldHistoriesOnDelete(fieldInstanceHistories, userId).ConfigureAwait(false);
            }
            else
                throw new ArgumentException($"{System.Reflection.MethodBase.GetCurrentMethod().Name} Received incorrect arguments.");
        }

        private void UpdateFieldInstanceHistoryIfChanged(FieldInstanceHistory existingHistory, int userId, ref List<FieldInstanceHistory> fieldInstanceHistoriesToInsertOnUpdate, FieldInstanceValue fieldInstanceValue)
        {
            if (existingHistory.LastValueChanged(fieldInstanceValue.Values) || existingHistory.IsDeleted)
            {
                fieldInstanceHistoriesToInsertOnUpdate.Add(existingHistory.CreateNewHistoryLog(userId, false, fieldInstanceValue.Values, fieldInstanceValue.ValueLabel, fieldInstanceValue.IsSpecialValue));
            }
        }

        private void CreateNewFieldInstanceHistory(FieldInstance fieldInstance, FormInstance formInstance, IEnumerable<FieldSet> fieldSets, ref List<FieldInstanceHistory> fieldInstanceHistoriesToInsertOnCreate, FieldInstanceValue repetitiveFieldInstanceValue)
        {
            string fieldLabel = fieldSets.FirstOrDefault(x => x.Id == fieldInstance.FieldSetId)?.GetFieldById(fieldInstance.FieldId)?.Label;
            string fieldSetLabel = fieldSets.FirstOrDefault(x => x.Id == fieldInstance.FieldSetId)?.Label;

            fieldInstanceHistoriesToInsertOnCreate.Add(new FieldInstanceHistory(
                formInstance, 
                fieldLabel, 
                fieldSetLabel, 
                fieldInstance, 
                repetitiveFieldInstanceValue));
        }

        private async Task UpdateManyFieldHistoriesOnDelete(IEnumerable<FieldInstanceHistory> fieldInstanceHistories, int userId)
        {
            List<FieldInstanceHistory> fieldInstanceHistoriesToUpdate = new List<FieldInstanceHistory>();

            foreach (FieldInstanceHistory fieldInstanceHistory in fieldInstanceHistories)
            {
                fieldInstanceHistoriesToUpdate.Add(fieldInstanceHistory.CreateNewHistoryLog(userId, isDeleted: true));
            }
            if (fieldInstanceHistoriesToUpdate.Count > 0)
                await fieldInstanceHistoryDAL.UpdateManyAsync(fieldInstanceHistoriesToUpdate).ConfigureAwait(false);
        }

        private async Task SetUserChangeInfos(List<FieldInstanceHistoryDataOut> histories)
        {
            List<int> distinctUserIds = histories.Select(y => y.UserId).Distinct().ToList();
            List<UserDataOut> usersWhoMadeChanges = mapper.Map<List<UserDataOut>>(
                await userDAL.GetAllByIdsAsync(distinctUserIds).ConfigureAwait(false)
                );
            histories.ForEach(x => x.UserCompleteName = usersWhoMadeChanges.Find(z => z.Id == x.UserId).ToString());
        }

        private async Task<Dictionary<string, FieldInstanceHistory>> GetLastFieldChanges(string formInstanceId)
        {
            List<FieldInstanceHistory> existingFieldInstanceHistories = await fieldInstanceHistoryDAL.GetAllFilteredAsync(new FieldInstanceHistoryFilterData() { FormInstanceId = formInstanceId, IncludeIsDeletedInQuery = false }).ConfigureAwait(false);
            return existingFieldInstanceHistories
                .GroupBy(x => x.FieldInstanceRepetitionId)
                .ToDictionary(
                    x => x.Key, 
                    x => x.OrderByDescending(change => change.EntryDatetime).FirstOrDefault()
                    );
        }

        private void AddChangeForDeletedRepetitiveFields(Dictionary<string, FieldInstanceHistory> lastFieldChanges, ref List<FieldInstanceHistory> fieldInstanceHistoriesToInsertOnUpdate, int userId)
        {
            foreach (KeyValuePair<string, FieldInstanceHistory> deletedFields in lastFieldChanges.Where(x => !x.Value.IsDeleted))
            {
                fieldInstanceHistoriesToInsertOnUpdate.Add(deletedFields.Value.CreateNewHistoryLog(userId, isDeleted: true));
            }
        }
    }
}
