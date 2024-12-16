using Autofac;
using sReportsV2.BusinessLayer.Implementations;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.DAL.Sql.Implementations;
using sReportsV2.DAL.Sql.Interfaces;
using sReportsV2.SqlDomain.Implementations;
using sReportsV2.SqlDomain.Interfaces;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.Domain.Services.Implementations;
using sReportsV2.BusinessLayer.Components.Implementations;
using sReportsV2.BusinessLayer.Components.Interfaces;
using sReportsV2.Common.Constants;
using Microsoft.Extensions.Configuration;

namespace sReportsV2.Configs
{
    public static class AutofacConfiguration
    {
        public static void Configure(ContainerBuilder builder, IConfiguration configuration)
        {
            RegisterCustomComponents(builder, configuration);
            RegisterBLLs(builder, configuration);
            RegisterDALs(builder);
        }

        private static void RegisterCustomComponents(ContainerBuilder builder, IConfiguration configuration)
        {
            if (configuration["EmailSender"] == EmailSenderNames.SmtpEmailSender)
            {
                builder.RegisterType<SmtpEmailSender>().As<IEmailSender>();
            }
            else
            {
                builder.RegisterType<SendGridEmailSender>().As<IEmailSender>();
            }
            builder.RegisterType<AsyncRunner>().As<IAsyncRunner>().SingleInstance();
        }

        private static void RegisterBLLs(ContainerBuilder builder, IConfiguration configuration)
        {
            builder.RegisterType<UserBLL>().As<IUserBLL>();
            builder.RegisterType<PositionPermissionBLL>().As<IPositionPermissionBLL>();
            builder.RegisterType<OrganizationBLL>().As<IOrganizationBLL>();
            builder.RegisterType<ThesaurusEntryBLL>().As<IThesaurusEntryBLL>();
            builder.RegisterType<CodeBLL>().As<ICodeBLL>();
            builder.RegisterType<FormInstanceBLL>().As<IFormInstanceBLL>();
            builder.RegisterType<FormBLL>().As<IFormBLL>();
            builder.RegisterType<CommentBLL>().As<ICommentBLL>();
            builder.RegisterType<PatientBLL>().As<IPatientBLL>();
            builder.RegisterType<PdfBLL>().As<IPdfBLL>();
            builder.RegisterType<ConsensusBLL>().As<IConsensusBLL>();
            builder.RegisterType<FormDistributionBLL>().As<IFormDistributionBLL>();
            builder.RegisterType<EncounterBLL>().As<IEncounterBLL>();
            builder.RegisterType<EpisodeOfCareBLL>().As<IEpisodeOfCareBLL>();
            builder.RegisterType<DigitalGuidelineBLL>().As<IDigitalGuidelineBLL>();
            builder.RegisterType<DigitalGuidelineInstanceBLL>().As<IDigitalGuidelineInstanceBLL>();
            builder.RegisterType<GlobalUserBLL>().As<IGlobalUserBLL>();
            builder.RegisterType<ChemotherapySchemaBLL>().As<IChemotherapySchemaBLL>();
            builder.RegisterType<ChemotherapySchemaInstanceBLL>().As<IChemotherapySchemaInstanceBLL>();
            builder.RegisterType<CodeSetBLL>().As<ICodeSetBLL>();
            builder.RegisterType<FhirBLL>().As<IFhirBLL>();
            builder.RegisterType<CodeAliasBLL>().As<ICodeAliasBLL>();
            builder.RegisterType<PersonnelTeamBLL>().As<IPersonnelTeamBLL>();
            builder.RegisterType<PersonnelTeamRelationBLL>().As<IPersonnelTeamRelationBLL>();
            builder.RegisterType<CodeAssociationBLL>().As<ICodeAssociationBLL>();
            builder.RegisterType<DiagnosticReportBLL>().As<IDiagnosticReportBLL>();
            builder.RegisterType<TaskBLL>().As<ITaskBLL>();
            builder.RegisterType<TrialManagementBLL>().As<ITrialManagementBLL>();
            builder.RegisterType<ProjectManagementBLL>().As<IProjectManagementBLL>();
            builder.RegisterType<PatientListBLL>().As<IPatientListBLL>();
            builder.RegisterType<PatholinkBLL>().As<IPatholinkBLL>();
            builder.RegisterType<AdministrationApiBLL>().As<IAdministrationApiBLL>();

            _ = bool.TryParse(configuration["UseFileStorage"], out bool useFileStorage);
            if (useFileStorage)
            {
                builder.RegisterType<FileStorageBLL>().As<IBlobStorageBLL>();
            }
            else
            {
                builder.RegisterType<CloudStorageBLL>().As<IBlobStorageBLL>();
            }
        }
        private static void RegisterDALs(ContainerBuilder builder)
        {
            builder.RegisterType<PersonnelDAL>().As<IPersonnelDAL>().InstancePerLifetimeScope();
            builder.RegisterType<OrganizationDAL>().As<IOrganizationDAL>().InstancePerLifetimeScope();
            builder.RegisterType<GlobalThesaurusUserDAL>().As<IGlobalThesaurusUserDAL>().InstancePerLifetimeScope();
            builder.RegisterType<ThesaurusDAL>().As<IThesaurusDAL>().InstancePerLifetimeScope();
            builder.RegisterType<ThesaurusTranslationDAL>().As<IThesaurusTranslationDAL>().InstancePerLifetimeScope();
            builder.RegisterType<AdministrativeDataDAL>().As<IAdministrativeDataDAL>().InstancePerLifetimeScope();
            builder.RegisterType<O4CodeableConceptDAL>().As<IO4CodeableConceptDAL>().InstancePerLifetimeScope();
            builder.RegisterType<CodeSystemDAL>().As<ICodeSystemDAL>().InstancePerLifetimeScope();
            builder.RegisterType<FormDAL>().As<IFormDAL>().InstancePerLifetimeScope();
            builder.RegisterType<FormInstanceDAL>().As<IFormInstanceDAL>().InstancePerLifetimeScope();
            builder.RegisterType<CommentDAL>().As<ICommentDAL>().InstancePerLifetimeScope();
            builder.RegisterType<CodeDAL>().As<ICodeDAL>().InstancePerLifetimeScope();
            builder.RegisterType<OutsideUserDAL>().As<IOutsideUserDAL>().InstancePerLifetimeScope();
            builder.RegisterType<ConsensusDAL>().As<IConsensusDAL>().InstancePerLifetimeScope();
            builder.RegisterType<ConsensusInstanceDAL>().As<IConsensusInstanceDAL>().InstancePerLifetimeScope();
            builder.RegisterType<EncounterDAL>().As<IEncounterDAL>().InstancePerLifetimeScope();
            builder.RegisterType<EpisodeOfCareDAL>().As<IEpisodeOfCareDAL>().InstancePerLifetimeScope();
            builder.RegisterType<PatientDAL>().As<IPatientDAL>().InstancePerLifetimeScope();
            builder.RegisterType<OrganizationRelationDAL>().As<IOrganizationRelationDAL>();
            builder.RegisterType<ModuleDAL>().As<IModuleDAL>().InstancePerLifetimeScope();
            builder.RegisterType<PermissionDAL>().As<IPermissionDAL>().InstancePerLifetimeScope();
            builder.RegisterType<PermissionModuleDAL>().As<IPermissionModuleDAL>().InstancePerLifetimeScope();
            builder.RegisterType<ThesaurusMergeDAL>().As<IThesaurusMergeDAL>().InstancePerLifetimeScope();
            builder.RegisterType<FormDistributionDAL>().As<IFormDistributionDAL>().InstancePerLifetimeScope();
            builder.RegisterType<DigitalGuidelineDAL>().As<IDigitalGuidelineDAL>().InstancePerLifetimeScope();
            builder.RegisterType<DigitalGuidelineInstanceDAL>().As<IDigitalGuidelineInstanceDAL>().InstancePerLifetimeScope();
            builder.RegisterType<GlobalThesaurusUserDAL>().As<IGlobalThesaurusUserDAL>();
            builder.RegisterType<GlobalThesaurusRoleDAL>().As<IGlobalThesaurusRoleDAL>();
            builder.RegisterType<ChemotherapySchemaDAL>().As<IChemotherapySchemaDAL>();
            builder.RegisterType<ChemotherapySchemaInstanceDAL>().As<IChemotherapySchemaInstanceDAL>();
            builder.RegisterType<ChemotherapySchemaInstanceHistoryDAL>().As<IChemotherapySchemaInstanceHistoryDAL>();
            builder.RegisterType<LiteratureReferenceDAL>().As<ILiteratureReferenceDAL>();
            builder.RegisterType<MedicationDAL>().As<IMedicationDAL>();
            builder.RegisterType<MedicationInstanceDAL>().As<IMedicationInstanceDAL>();
            builder.RegisterType<MedicationReplacementDAL>().As<IMedicationReplacementDAL>();
            builder.RegisterType<BodySurfaceCalculationFormulaDAL>().As<IBodySurfaceCalculationFormulaDAL>();
            builder.RegisterType<RouteOfAdministrationDAL>().As<IRouteOfAdministrationDAL>();
            builder.RegisterType<MedicationDoseTypeDAL>().As<IMedicationDoseTypeDAL>();
            builder.RegisterType<MedicationDoseDAL>().As<IMedicationDoseDAL>();
            builder.RegisterType<MedicationDoseInstanceDAL>().As<IMedicationDoseInstanceDAL>();
            builder.RegisterType<UnitDAL>().As<IUnitDAL>();
            builder.RegisterType<CustomFieldFilterDAL>().As<ICustomFieldFilterDAL>().InstancePerLifetimeScope();
            builder.RegisterType<CodeSetDAL>().As<ICodeSetDAL>().InstancePerLifetimeScope();
            builder.RegisterType<CodeAliasViewDAL>().As<ICodeAliasViewDAL>().InstancePerLifetimeScope();
            builder.RegisterType<InboundAliasDAL>().As<IInboundAliasDAL>().InstancePerLifetimeScope();
            builder.RegisterType<OutboundAliasDAL>().As<IOutboundAliasDAL>().InstancePerLifetimeScope();
            builder.RegisterType<CodeAssociationDAL>().As<ICodeAssociationDAL>().InstancePerLifetimeScope();
            builder.RegisterType<PersonnelTeamDAL>().As<IPersonnelTeamDAL>().InstancePerLifetimeScope();
            builder.RegisterType<PersonnelTeamRelationDAL>().As<IPersonnelTeamRelationDAL>().InstancePerLifetimeScope();
            builder.RegisterType<PositionPermissionDAL>().As<IPositionPermissionDAL>().InstancePerLifetimeScope();
            builder.RegisterType<FieldInstanceHistoryDAL>().As<IFieldInstanceHistoryDAL>().InstancePerLifetimeScope();
            builder.RegisterType<TaskDAL>().As<ITaskDAL>().InstancePerLifetimeScope();
            builder.RegisterType<TrialManagementDAL>().As<ITrialManagementDAL>().InstancePerLifetimeScope();
            builder.RegisterType<ProjectManagementDAL>().As<IProjectManagementDAL>().InstancePerLifetimeScope();
            builder.RegisterType<FormCodeRelationDAL>().As<IFormCodeRelationDAL>().InstancePerLifetimeScope();
            builder.RegisterType<PatientListDAL>().As<IPatientListDAL>().InstancePerLifetimeScope();
            builder.RegisterType<AdministrationApiDAL>().As<IAdministrationApiDAL>().InstancePerLifetimeScope();
        }
    }
}