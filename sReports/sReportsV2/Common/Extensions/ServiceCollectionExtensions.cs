using Microsoft.Extensions.DependencyInjection;
using sReportsV2.MapperProfiles;

namespace sReportsV2.Common.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper((serviceProvider, automapper) =>
            {
                automapper.AddProfile<CommonProfile>();
                automapper.AddProfile<UserProfile>();
                automapper.AddProfile<OrganizationProfile>();
                automapper.AddProfile<EntityProfile>();
                automapper.AddProfile<PersonnelTeamProfile>();
                automapper.AddProfile<CodeProfile>();
                automapper.AddProfile<CodeSetProfile>();
                automapper.AddProfile<ThesaurusEntrySqlProfile>();
                automapper.AddProfile<GlobalThesaurusUserProfile>();
                automapper.AddProfile<FormProfile>();
                automapper.AddProfile<CommentProfile>();
                automapper.AddProfile<FormInstanceProfile>();
                automapper.AddProfile<RoleProfile>();
                automapper.AddProfile<DocumentPropertiesProfile>();
                automapper.AddProfile<PatientProfile>();
                automapper.AddProfile<UmlsProfile>();
                automapper.AddProfile<EpisodeOfCareProfile>();
                automapper.AddProfile<EncounterProfile>();
                automapper.AddProfile<FormDistributionProfile>();
                automapper.AddProfile<FieldProfile>();
                automapper.AddProfile<DigitalGuidelineProfile>();
                automapper.AddProfile<DigitalGuidelineInstanceProfile>();
                automapper.AddProfile<ChemotherapySchemaProfile>();
                automapper.AddProfile<AliasProfile>();
                automapper.AddProfile<CodeAssociationProfile>();
                automapper.AddProfile<FieldInstanceHistoryProfile>();
                automapper.AddProfile<TaskProfile>();
                automapper.AddProfile<ClinicalTrialProfile>();
                automapper.AddProfile<ProjectProfile>();
                automapper.AddProfile<AdministrationApiProfile>();

                automapper.ConstructServicesUsing(serviceProvider.GetService);
            }, typeof(Startup).Assembly);

            return services;
        }
    }
}
