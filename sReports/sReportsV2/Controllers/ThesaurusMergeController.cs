using Serilog;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Constants;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.Common.Entities.User;
using sReportsV2.Common.Extensions;
using sReportsV2.Cache.Singleton;
using sReportsV2.DTOs.ThesaurusEntry.DataIn;
using System;
using Microsoft.AspNetCore.Mvc;
using sReportsV2.Common.Enums;

namespace sReportsV2.Controllers
{
    public partial class ThesaurusEntryController : BaseController
    {
        [SReportsAuthorize(Module = ModuleNames.Thesaurus, Permission = PermissionNames.Update)]
        public ActionResult TakeBoth(int currentId)
        {
            thesaurusEntryBLL.TakeBoth(currentId);
            RefreshCache(currentId, ModifiedResourceType.Thesaurus);
            return Ok();
        }

        [SReportsAuthorize(Module = ModuleNames.Thesaurus, Permission = PermissionNames.Update)]
        public ActionResult MergeThesauruses(ThesaurusMergeDataIn thesaurusMergeDataIn)
        {
            var thesaurusMergeStates = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.ThesaurusMergeState);
            int? pendingStateCD = thesaurusMergeStates?.Find(x => x.Thesaurus.Translations.Exists(m => m.PreferredTerm == CodeAttributeNames.Pending))?.Id;
            thesaurusMergeDataIn = Ensure.IsNotNull(thesaurusMergeDataIn, nameof(thesaurusMergeDataIn));
            thesaurusMergeDataIn.StateCD = pendingStateCD;
            thesaurusEntryBLL.MergeThesauruses(thesaurusMergeDataIn, Mapper.Map<UserData>(userCookieData));
            RefreshCache(thesaurusMergeDataIn.TargetId, ModifiedResourceType.Thesaurus);
            return Ok();
        }

        [SReportsAuthorize(Module = ModuleNames.Thesaurus, Permission = PermissionNames.Update)]
        [SReportsAuditLog]
        public ActionResult MergeThesaurusOccurences()
        {
            Log.Information($"MergeThesaurusOccurences has been started");
            _asyncRunner.Run<IThesaurusEntryBLL>((thesaurusEntryBLL) =>
            {
                MergeThesaurusOccurencesAction(thesaurusEntryBLL);
            });
            return Json("Merge Process of thesaurus occurences has been started! Administration team will let you know when the operation is finished.");
        }

      
        private void MergeThesaurusOccurencesAction(IThesaurusEntryBLL thesaurusEntryBLLObject)
        {
            try
            {
                int numOfUpdatedEntries = thesaurusEntryBLLObject.MergeThesaurusOccurences(userCookieData);
                if (numOfUpdatedEntries > 0)
                {
                    RefreshCache();
                }
                Log.Information($"MergeThesaurusOccurences has been finished");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "MergeThesaurusOccurrences thrown an error: {Message}", ex.Message);
            }
        }
    }
}