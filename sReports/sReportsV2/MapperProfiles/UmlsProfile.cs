using AutoMapper;
using sReportsV2.DTOs.Umls.DatOut;
using System;
using System.Collections.Generic;
using System.Linq;
using UmlsClient.UMLSClasses;
using UMLSClient.UMLSClasses;

namespace sReportsV2.MapperProfiles
{
    public class UmlsProfile : Profile
    {
        public UmlsProfile()
        {
            CreateMap<SearchResult, SearchResultDataOut>();

            CreateMap<ConceptSearchResult, ConceptSearchResultDataOut>();

            CreateMap(typeof(UMLSSearchResult), typeof(UmlsDataOut<>))
                 .ForMember("PageCount", opt => opt.Ignore());

            CreateMap<ConceptDefinition, ConceptDefinitionDataOut>();

            CreateMap(typeof(UMLSConceptDefinition), typeof(UmlsDataOut<>));

            CreateMap(typeof(UMLSAtomResult), typeof(UmlsDataOut<>));

            CreateMap<Atom, AtomDataOut>();

        }
    }
}