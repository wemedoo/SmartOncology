using AutoMapper;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Entities.Common;
using sReportsV2.Domain.Entities.DigitalGuideline;
using sReportsV2.Domain.Entities.DigitalGuidelineInstance;
using sReportsV2.Domain.Entities.FieldEntity;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.DigitalGuideline.DataIn;
using sReportsV2.DTOs.DigitalGuideline.DataOut;
using sReportsV2.DTOs.DigitalGuidelineInstance.DataIn;
using sReportsV2.DTOs.DigitalGuidelineInstance.DataOut;
using sReportsV2.DTOs.DTOs.Autocomplete.DataOut;
using sReportsV2.DTOs.DTOs.FormInstance.DataOut;
using sReportsV2.DTOs.EpisodeOfCare;
using sReportsV2.DTOs.FormInstance.DataOut;
using sReportsV2.DTOs.Patient;
using sReportsV2.DTOs.ThesaurusEntry.DataOut;
using sReportsV2.DTOs.User.DTO;
using sReportsV2.SqlDomain.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace sReportsV2.BusinessLayer.Implementations
{
    public class DigitalGuidelineInstanceBLL : IDigitalGuidelineInstanceBLL
    {
        private readonly IDigitalGuidelineDAL digitalGuidelineDAL;
        private readonly IDigitalGuidelineInstanceDAL digitalGuidelineInstanceDAL;
        private readonly IThesaurusDAL thesaurusDAL;
        private readonly IPatientDAL patientDAL;
        private readonly IFormInstanceDAL formInstanceDAL;
        private readonly IEpisodeOfCareDAL episodeOfCareDAL;
        private readonly IMapper mapper;

        public DigitalGuidelineInstanceBLL(IDigitalGuidelineDAL digitalGuidelineDAL, IDigitalGuidelineInstanceDAL digitalGuidelineInstanceDAL, IThesaurusDAL thesaurusDAL, IPatientDAL patientDAL, IEpisodeOfCareDAL episodeOfCareDAL, IFormInstanceDAL formInstanceDAL, IMapper mapper)
        {
            this.digitalGuidelineDAL = digitalGuidelineDAL;
            this.digitalGuidelineInstanceDAL = digitalGuidelineInstanceDAL;
            this.thesaurusDAL = thesaurusDAL;
            this.patientDAL = patientDAL;
            this.formInstanceDAL = formInstanceDAL;
            this.episodeOfCareDAL = episodeOfCareDAL;
            this.mapper = mapper;
        }

        public ResourceCreatedDTO InsertOrUpdate(GuidelineInstanceDataIn guidelineInstanceDataIn)
        {
            guidelineInstanceDataIn = Ensure.IsNotNull(guidelineInstanceDataIn, nameof(guidelineInstanceDataIn));
            SetGuidelineInstanceNodeValues(guidelineInstanceDataIn);
            GuidelineInstance guidelineInstance = mapper.Map<GuidelineInstance>(guidelineInstanceDataIn);
            digitalGuidelineInstanceDAL.Insert(guidelineInstance);

            return new ResourceCreatedDTO()
            {
                Id = guidelineInstance?.Id,
                LastUpdate = guidelineInstance.LastUpdate.Value.ToString("o")
            };
        }

        public bool Delete(string guidelineInstanceId)
        {
            return digitalGuidelineInstanceDAL.Delete(guidelineInstanceId);
        }

        public List<AutocompleteOptionDataOut> GetConditions(string nodeId, string digitalGuidelineId)
        {
            List<AutocompleteOptionDataOut> conditionList = new List<AutocompleteOptionDataOut>();
            foreach (GuidelineEdgeElementData item in digitalGuidelineDAL.GetEdges(nodeId, digitalGuidelineId).Item1)
            {
                if (!conditionList.Select(x => x.text).Contains(item.Condition) && item.Condition != null)
                    conditionList.Add(new AutocompleteOptionDataOut(item));
            }
            return conditionList;
        }

        public GuidelineDataOut GetGraph(string guidelineInstanceId, string guidelineId)
        {
            GuidelineInstance guidelineInstance = digitalGuidelineInstanceDAL.GetById(guidelineInstanceId);
            GuidelineDataOut dataOut = mapper.Map<GuidelineDataOut>(digitalGuidelineDAL.GetById(guidelineId));
            foreach (var item in dataOut.GuidelineElements.Nodes)
            {
                NodeValue nodeValue = guidelineInstance.GetNodeValueById(item.Data.Id);
                if (nodeValue != null)
                {
                    item.Data.Value = nodeValue.Value;
                    item.Data.State = nodeValue.State;
                }
            }
            return dataOut;
        }

        public PatientDataOut GetGuidelineInstance(int episodeOfCareId)
        {
            EpisodeOfCareDataOut episodeOfCareDataOut = mapper.Map<EpisodeOfCareDataOut>(episodeOfCareDAL.GetById(episodeOfCareId));
            PatientDataOut patientDataOut = mapper.Map<PatientDataOut>(patientDAL.GetById(episodeOfCareDataOut.PatientId));
            episodeOfCareDataOut.ListGuidelines = mapper.Map<List<GuidelineInstanceDataOut>>(digitalGuidelineInstanceDAL.GetGuidelineInstancesByEOC(episodeOfCareId));
            patientDataOut.ReplaceEpisodeOfCare(episodeOfCareId, episodeOfCareDataOut);

            return patientDataOut;
        }

        public List<GuidelineInstanceDataOut> GetGuidelineInstancesByEOC(int episodeOfCareId)
        {
            return mapper.Map<List<GuidelineInstanceDataOut>>(digitalGuidelineInstanceDAL.GetGuidelineInstancesByEOC(episodeOfCareId));
        }

        public string GetValueFromDocument(string formInstanceId, int thesaurusId)
        {
            FormInstance formInstance = formInstanceDAL.GetById(formInstanceId);
            Field field = formInstanceDAL.GetFieldByThesaurus(formInstance, thesaurusId);
            return field != null && field.HasValue() ? field.GetFirstFieldInstanceValue() : "";
        }

        public GuidelineInstanceViewDataOut ListDigitalGuidelineDocuments(int episodeOfCareId, UserCookieData userCookieData)
        {
            GuidelineInstanceViewDataOut result = new GuidelineInstanceViewDataOut()
            {
                GuidelineInstance = new GuidelineInstanceDataOut()
                {
                    EpisodeOfCareId = episodeOfCareId
                },
                FormInstances = formInstanceDAL.GetByEpisodeOfCareId(episodeOfCareId, userCookieData.ActiveOrganization).Select(x => new AutocompleteOptionDataOut(x, userCookieData)).ToList()
            };

            return result;
        }

        public GuidelineInstanceViewDataOut ListDigitalGuidelines(int? episodeOfCareId, UserCookieData userCookieData)
        {
            GuidelineInstanceViewDataOut result = new GuidelineInstanceViewDataOut()
            {
                GuidelineInstance = new GuidelineInstanceDataOut()
                {
                    EpisodeOfCareId = episodeOfCareId.GetValueOrDefault()
                },
                Guidelines = digitalGuidelineDAL.GetAll().Select(x => new AutocompleteOptionDataOut(x, userCookieData)).ToList()
            };

            return result;
        }

        public void MarksAsCompleted(string value, string nodeId, string guidelineInstanceId)
        {
            GuidelineInstance guidelineInstance = digitalGuidelineInstanceDAL.GetById(guidelineInstanceId);
            Guideline guideline = digitalGuidelineDAL.GetById(guidelineInstance.DigitalGuidelineId);
            SetNodeValue(guidelineInstance, value, nodeId);
            guidelineInstance.SetNextNodeState(nodeId, guideline);
            digitalGuidelineInstanceDAL.Insert(mapper.Map<GuidelineInstance>(guidelineInstance));
        }

        public GuidelineElementDataDataOut PreviewInstanceNode(GuidelineElementDataDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            GuidelineElementDataDataOut data = mapper.Map<GuidelineElementDataDataOut>(dataIn);
            data.Thesaurus = mapper.Map<ThesaurusEntryDataOut>(this.thesaurusDAL.GetById(dataIn.ThesaurusId));

            return data;
        }

        public void SaveCondition(string condition, string nodeId, string guidelineInstanceId, string digitalGuidelineId)
        {
            GuidelineInstance guidelineInstance = digitalGuidelineInstanceDAL.GetById(guidelineInstanceId);
            foreach (GuidelineEdgeElementData item in digitalGuidelineDAL.GetEdges(nodeId, digitalGuidelineId).Item1)
            {
                if (item.SatisfyCondition(condition))
                    guidelineInstance.NodeValues.Find(x => x.Id.Equals(item.Target)).State = NodeState.Active;
            }

            guidelineInstance.NodeValues.Find(x => x.Id.Equals(nodeId)).State = NodeState.Completed;
            digitalGuidelineInstanceDAL.Insert(mapper.Map<GuidelineInstance>(guidelineInstance));
        }

        private void SetGuidelineInstanceNodeValues(GuidelineInstanceDataIn guidelineInstanceDataIn)
        {
            Guideline guideline = digitalGuidelineDAL.GetById(guidelineInstanceDataIn.DigitalGuidelineId);
            foreach (GuidelineElement element in guideline.GuidelineElements.Nodes)
            {
                NodeValue nodeValue = new NodeValue
                {
                    Id = element.Data.Id,
                    State = NodeState.NotStarted
                };
                guidelineInstanceDataIn.NodeValues.Add(nodeValue);
            }
            GuidelineInstance guidelineInstance = mapper.Map<GuidelineInstance>(guidelineInstanceDataIn);
            guidelineInstance.SetStartNode(digitalGuidelineDAL.GetEdges(guidelineInstance.NodeValues[0].Id, guidelineInstance.DigitalGuidelineId).Item1);
        }

        private void SetNodeValue(GuidelineInstance guidelineInstance, string value, string nodeId)
        {
            guidelineInstance.SetNodeValue(value, nodeId);
            digitalGuidelineInstanceDAL.Insert(mapper.Map<GuidelineInstance>(guidelineInstance));
        }
    }
}
