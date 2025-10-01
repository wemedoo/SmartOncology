using sReportsV2.Common.Enums;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.DocumentProperties.DataIn;
using sReportsV2.DTOs.DTOs.DocumentProperties.DataIn;
using sReportsV2.DTOs.DTOs.Form.DataIn;
using sReportsV2.DTOs.Field.DataIn;
using System;
using System.Collections.Generic;
using System.Linq;

namespace sReportsV2.DTOs.Form.DataIn
{
    public class FormDataIn : IViewModeDataIn
    {
        public string Id { get; set; }
        public FormAboutDataIn About { get; set; }
        public string Title { get; set; }
        public sReportsV2.Domain.Entities.Form.Version Version { get; set; }
        public List<FormChapterDataIn> Chapters { get; set; } = new List<FormChapterDataIn>();
        public FormDefinitionState State { get; set; }
        public string Language { get; set; }
        public int ThesaurusId { get; set; }
        public DateTime? LastUpdate { get; set; }
        public DocumentPropertiesDataIn DocumentProperties { get; set; }
        public DocumentLoincPropertiesDataIn DocumentLoincProperties { get; set; }
        public FormEpisodeOfCareDataDataIn EpisodeOfCare { get; set; }
        public bool DisablePatientData { get; set; }
        public string OomniaId { get; set; }
        public List<CustomHeaderFieldDataIn> CustomHeaderFields { get; set; } = new List<CustomHeaderFieldDataIn>();
        public bool IsReadOnlyViewMode { get; set; }
        public bool AvailableForTask { get; set; }
        public List<int> NullFlavors { get; set; } = new List<int>();
        public List<int> OrganizationIds { get; set; } = new List<int>();
        public bool Invalid { get; set; }

        public List<FieldDataIn> GetAllFields()
        {
            return this.Chapters
                        .SelectMany(chapter => chapter.Pages
                            .SelectMany(page => page.ListOfFieldSets
                                .SelectMany(listOfFS => listOfFS
                                   .SelectMany(set => set.Fields
                                    )
                                )
                            )
                        ).ToList();
        }

        public List<string> ValidateFieldsIds()
        {
            List<string> listAllFieldIds = new List<string>();
            List<string> listDuplicateIds = new List<string>();
            foreach (FieldDataIn field in this.GetAllFields())
            {
                if (listAllFieldIds.Contains(field.Id))
                    listDuplicateIds.Add(field.Id);

                listAllFieldIds.Add(field.Id);
            }

            return listDuplicateIds;
        }

    }
}