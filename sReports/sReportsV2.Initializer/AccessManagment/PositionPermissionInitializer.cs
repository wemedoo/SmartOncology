using sReportsV2.Common.Constants;
using sReportsV2.Common.Enums;
using sReportsV2.Domain.Sql.Entities.AccessManagment;
using sReportsV2.SqlDomain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using sReportsV2.DAL.Sql.Interfaces;
using sReportsV2.Domain.Sql.Entities.Common;

namespace sReportsV2.Initializer.AccessManagment
{
    public class PositionPermissionInitializer
    {
        private readonly IModuleDAL moduleDAL;
        private readonly IPermissionDAL permissionDAL;
        private readonly IPermissionModuleDAL permissionModuleDAL;
        private readonly ICodeDAL codeDAL;
        private readonly IPositionPermissionDAL positionPermissionDAL;
        private Dictionary<string, int> modules;
        private Dictionary<string, int> permissions;
        private Dictionary<string, List<int>> permissionsPerModules;

        public PositionPermissionInitializer(
               IModuleDAL moduleDAL,
               IPermissionDAL permissionDAL,
               IPermissionModuleDAL permissionModuleDAL,
               ICodeDAL codeDAL,
               IPositionPermissionDAL positionPermissionDAL)
        {
            this.moduleDAL = moduleDAL;
            this.permissionDAL = permissionDAL;
            this.permissionModuleDAL = permissionModuleDAL;
            this.codeDAL = codeDAL;
            this.positionPermissionDAL = positionPermissionDAL;
            UpdateHelperProperties();
        }

        public void SetInitialData()
        {
            SetInitialModules();
            SetInitialPermissions();
            UpdateHelperProperties();
            SetInitialPermissionsForModules();
            SetInitialPositionPermission();
        }

        private void UpdateHelperProperties()
        {
            List<Module> modulesDB = moduleDAL.GetAll();
            this.modules = modulesDB.ToDictionary(x => x.Name, x => x.ModuleId);
            this.permissions = permissionDAL.GetAll().ToDictionary(x => x.Name, x => x.PermissionId);
            this.permissionsPerModules = modulesDB.ToDictionary(x => x.Name, x => x.Permissions.Select(p => p.PermissionId).ToList());
        }

        private void SetInitialModules()
        {
            List<Module> allModules = GetAllModules();
            List<Module> modulesForInsert = new List<Module>();

            foreach (Module module in allModules)
            {
                if (IsNewModule(module.Name))
                {
                    modulesForInsert.Add(module);
                }
            }
            moduleDAL.InsertMany(modulesForInsert);
        }

        private bool IsNewModule(string moduleName)
        {
            return !modules.TryGetValue(moduleName, out _);
        }

        private void SetInitialPermissions()
        {
            List<Permission> allPermissions = GetAllPermissions();
            List<Permission> permissionsForInsert = new List<Permission>();

            foreach (Permission permission in allPermissions)
            {
                if (IsNewPermissions(permission.Name))
                {
                    permissionsForInsert.Add(permission);
                }
            }
            permissionDAL.InsertMany(permissionsForInsert);
        }

        private bool IsNewPermissions(string permissionName)
        {
            return !permissions.TryGetValue(permissionName, out _);
        }

        private void SetInitialPermissionsForModules()
        {
            List<PermissionModule> permissionForModulesForInsert = new List<PermissionModule>();
            foreach (KeyValuePair<string, List<string>> permissionsForModule in GetAllPermissionsForModules())
            {
                string moduleName = permissionsForModule.Key;
                if (modules.TryGetValue(moduleName, out int moduleId))
                {
                    foreach (string permissionName in permissionsForModule.Value)
                    {
                        if (permissions.TryGetValue(permissionName, out int permissionId) && IsNewPermissionForModule(moduleName, permissionId))
                        {
                            permissionForModulesForInsert.Add(new PermissionModule
                            {
                                ModuleId = moduleId,
                                PermissionId = permissionId
                            });
                        }
                    }
                }
            }
            permissionModuleDAL.InsertMany(permissionForModulesForInsert);
        }

        private bool IsNewPermissionForModule(string moduleName, int permissionId)
        {
            bool isNewPermissionForModule = false;
            if (permissionsPerModules.TryGetValue(moduleName, out List<int> permissionsForModuleIds))
            {
                isNewPermissionForModule = !permissionsForModuleIds.Contains(permissionId);
            }
            return isNewPermissionForModule;
        }

        #region Get Modules and Permissions

        private List<Module> GetAllModules()
        {
            Module administrationModule = new Module
            {
                Name = ModuleNames.Administration,
                Description = "Within this module administrator can edit user roles, global, user and document related properties and release or block functionality of the system."
            };
            Module designerModule = new Module
            {
                Name = ModuleNames.Designer,
                Description = "Designer module has functionalities that are related to defining medical forms and documents (creating and editing, reviewing with comments and consensus finding process)"
            };
            Module engineModule = new Module
            {
                Name = ModuleNames.Engine,
                Description = "Engine module has functionalities that are related to creating documents, viewing its' content, exporting in appropriate format and downloading."
            };
            Module thesaurusModule = new Module
            {
                Name = ModuleNames.Thesaurus,
                Description = "Thesaurus module has functionalities that are related to creating thesaurus with defining its own codes or integrate with UMLS."
            };
            Module patientModule = new Module
            {
                Name = ModuleNames.Patients,
                Description = "Patients module has functionalities that are related to editing patient's data and adding new episode of cares and encounters."
            };
            Module simulatorModule = new Module
            {
                Name = ModuleNames.Simulator,
                Description = "Simulator module has functionalities that are related to parametarize probabilities of some values and establish relations between them."
            };
            Module codeSetModule = new Module
            {
                Name = ModuleNames.CodeSet,
                Description = "Code set module has functionalities that are related to viewing, creating and editing code set's, codes, code associations and code aliases data."
            };
            Module projectManagementModule = new Module
            {
                Name = ModuleNames.ProjectManagement,
                Description = "Project Management module has functionalities that are related to viewing, creating and editing Projects, as well as its Personnels and Documentation."
            };
            Module clinicalPathwayModule = new Module
            {
                Name = ModuleNames.ClinicalPathway,
                Description = "Clinical Pathway module has functionalities that are related to viewing, creating and editing Clinical Pathway module."
            };
            Module clinicalOncologyModule = new Module
            {
                Name = ModuleNames.ClinicalOncology,
                Description = "ClinicalOncology module has functionalities that are related to viewing, creating and editing Clinical Oncology modules."
            };

            List<Module> moduleList = new List<Module>
                {
                    designerModule,
                    engineModule,
                    thesaurusModule,
                    patientModule,
                    simulatorModule,
                    administrationModule,
                    codeSetModule,
                    projectManagementModule,
                    clinicalPathwayModule,
                    clinicalOncologyModule
                };

            return moduleList;
        }

        private List<Permission> GetAllPermissions()
        {
            return new List<Permission>
            {
                new Permission
                {
                    Name = PermissionNames.Create,
                    Description = "Create entity"
                },
                new Permission
                {
                    Name = PermissionNames.Update,
                    Description = "Update entity"
                },
                new Permission
                {
                    Name = PermissionNames.Delete,
                    Description = "Delete entity"
                },
                new Permission
                {
                    Name = PermissionNames.View,
                    Description = "View entity"
                },
                new Permission
                {
                    Name = PermissionNames.ShowJson,
                    Description = "Show Json"
                },
                new Permission
                {
                    Name = PermissionNames.ChangeState,
                    Description = "Change State"
                },
                new Permission
                {
                    Name = PermissionNames.FindConsensus,
                    Description = "Find Consensus"
                },
                new Permission
                {
                    Name = PermissionNames.ViewAdministrativeData,
                    Description = "View Administrative Data"
                },
                new Permission
                {
                    Name = PermissionNames.ViewComments,
                    Description = "View Comments"
                },
                new Permission
                {
                    Name = PermissionNames.AddComment,
                    Description = "Add Comment"
                },
                new Permission
                {
                    Name = PermissionNames.ChangeCommentStatus,
                    Description = "Change Comment Status"
                },
                new Permission
                {
                    Name = PermissionNames.Download,
                    Description = "Download document"
                },
                new Permission
                {
                    Name = PermissionNames.CreateCode,
                    Description = "Create Code"
                },
                new Permission
                {
                    Name = PermissionNames.UMLS,
                    Description = "UMLS"
                },
                new Permission
                {
                    Name = PermissionNames.AddEpisodeOfCare,
                    Description = "Add Episode Of Care"
                },
                new Permission
                {
                    Name = PermissionNames.AddEncounter,
                    Description = "Add Encounter"
                },
                new Permission
                {
                    Name = PermissionNames.SignFormInstance,
                    Description = "Sign Form Instance"
                },
                new Permission
                {
                    Name = PermissionNames.ChangeFormInstanceState,
                    Description = "Change Form Instance State"
                },
                new Permission
                {
                    Name = PermissionNames.LockChapter,
                    Description = "Lock Chapter"
                },
                new Permission
                {
                    Name = PermissionNames.UnlockChapter,
                    Description = "Unlock Chapter"
                },
                new Permission
                {
                    Name = PermissionNames.LockPage,
                    Description = "Lock Page"
                },
                new Permission
                {
                    Name = PermissionNames.UnlockPage,
                    Description = "Unlock Page"
                },
                new Permission
                {
                    Name = PermissionNames.ViewCode,
                    Description = "View Code Entity"
                },
                new Permission
                {
                    Name = PermissionNames.CreateCodeEntry,
                    Description = "Create Code Entity"
                },
                new Permission
                {
                    Name = PermissionNames.UpdateCode,
                    Description = "Update Code Entity"
                },
                new Permission
                {
                    Name = PermissionNames.DeleteCode,
                    Description = "Delete Code Entity"
                },
                new Permission
                {
                    Name = PermissionNames.ViewAlias,
                    Description = "View Alias Entity"
                },
                new Permission
                {
                    Name = PermissionNames.CreateAlias,
                    Description = "Create Alias Entity"
                },
                new Permission
                {
                    Name = PermissionNames.UpdateAlias,
                    Description = "Update Alias Entity"
                },
                new Permission
                {
                    Name = PermissionNames.DeleteAlias,
                    Description = "Delete Alias Entity"
                },
                new Permission
                {
                    Name = PermissionNames.ViewAssociation,
                    Description = "View Association Entity"
                },
                new Permission
                {
                    Name = PermissionNames.CreateAssociation,
                    Description = "Create Association Entity"
                },
                new Permission
                {
                    Name = PermissionNames.UpdateAssociation,
                    Description = "Update Association Entity"
                },
                new Permission
                {
                    Name = PermissionNames.DeleteAssociation,
                    Description = "Delete Association Entity"
                },
                new Permission
                {
                    Name = PermissionNames.AddPersonnel,
                    Description = "Add Personnel"
                },
                new Permission
                {
                    Name = PermissionNames.DeletePersonnel,
                    Description = "Delete Personnel"
                },
                new Permission
                {
                    Name = PermissionNames.AddDocument,
                    Description = "Add Document"
                },
                new Permission
                {
                    Name = PermissionNames.DeleteDocument,
                    Description = "Delete Document"
                },
                new Permission
                {
                    Name = PermissionNames.AddPatient,
                    Description = "Add Patient"
                },
                new Permission
                {
                    Name = PermissionNames.DeletePatient,
                    Description = "Delete Patient"
                },
                new Permission
                {
                    Name = PermissionNames.ViewPatientList,
                    Description = "View Patient List"
                },
                new Permission
                {
                    Name = PermissionNames.CreatePatientList,
                    Description = "Create Patient List"
                },
                new Permission
                {
                    Name = PermissionNames.UpdatePatientList,
                    Description = "Update Patient List"
                },
                new Permission
                {
                    Name = PermissionNames.DeletePatientList,
                    Description = "Delete Patient List"
                },
                new Permission
                {
                    Name = PermissionNames.AddPatientListUsers,
                    Description = "Add Patient List Users"
                },
                new Permission
                {
                    Name = PermissionNames.RemovePatientListUsers,
                    Description = "Remove Patient List Users"
                },
                new Permission
                {
                    Name = PermissionNames.AddPatientToPatientList,
                    Description = "Add Patient to Patient List"
                },
                new Permission
                {
                    Name = PermissionNames.RemovePatientFromPatientList,
                    Description = "Remove Patient from Patient List"
                },
                new Permission
                {
                    Name = PermissionNames.RemoveEpisodeOfCare,
                    Description = "Remove Episode Of Care"
                },
                new Permission
                {
                    Name = PermissionNames.UpdateEpisodeOfCare,
                    Description = "Update Episode Of Care"
                },
                new Permission
                {
                    Name = PermissionNames.ViewEpisodeOfCare,
                    Description = "View Episode Of Care"
                },
                new Permission
                {
                    Name = PermissionNames.RemoveEncounter,
                    Description = "Remove Encounter"
                },
                new Permission
                {
                    Name = PermissionNames.ViewEncounter,
                    Description = "View Encounter"
                },
                new Permission
                {
                    Name = PermissionNames.UpdateEncounter,
                    Description = "Update Encounter"
                },
                new Permission
                {
                    Name = PermissionNames.UpdateDocument,
                    Description = "Update Document"
                },
                new Permission
                {
                    Name = PermissionNames.ViewDocument,
                    Description = "View Document"
                },
            };
        }

        private Dictionary<string, List<string>> GetAllPermissionsForModules()
        {
            List<string> generalPermissions = GetAllGeneralPermissions();
            return new Dictionary<string, List<string>>
            {
                {
                    ModuleNames.Designer,
                    new List<string>(generalPermissions)
                    {
                        PermissionNames.ShowJson,
                        PermissionNames.ChangeState,
                        PermissionNames.FindConsensus,
                        PermissionNames.ViewAdministrativeData,
                        PermissionNames.ViewComments,
                        PermissionNames.AddComment,
                        PermissionNames.ChangeCommentStatus
                    }
                },
                {
                    ModuleNames.Engine,
                    new List<string>(generalPermissions)
                    {
                        PermissionNames.Download,
                        PermissionNames.SignFormInstance,
                        PermissionNames.ChangeFormInstanceState,
                        PermissionNames.ViewAdministrativeData,
                        PermissionNames.LockPage,
                        PermissionNames.UnlockPage,
                        PermissionNames.LockChapter,
                        PermissionNames.UnlockChapter
                    }
                },
                {
                    ModuleNames.Thesaurus,
                    new List<string>(generalPermissions)
                    {
                        PermissionNames.CreateCode,
                        PermissionNames.UMLS,
                        PermissionNames.ViewAdministrativeData
                    }
                },
                {
                    ModuleNames.Patients,
                    new List<string>(generalPermissions)
                    {
                        PermissionNames.AddEpisodeOfCare,
                        PermissionNames.AddEncounter,
                        PermissionNames.ViewPatientList,
                        PermissionNames.CreatePatientList,
                        PermissionNames.UpdatePatientList,
                        PermissionNames.DeletePatientList,
                        PermissionNames.AddPatientListUsers,
                        PermissionNames.RemovePatientListUsers,
                        PermissionNames.AddPatientToPatientList,
                        PermissionNames.RemovePatientFromPatientList,
                        PermissionNames.UpdateEpisodeOfCare,
                        PermissionNames.RemoveEpisodeOfCare,
                        PermissionNames.ViewEpisodeOfCare,
                        PermissionNames.UpdateEncounter,
                        PermissionNames.RemoveEncounter,
                        PermissionNames.ViewEncounter,
                        PermissionNames.AddDocument,
                        PermissionNames.UpdateDocument,
                        PermissionNames.DeleteDocument,
                        PermissionNames.ViewDocument
                    }
                },
                {
                    ModuleNames.Simulator,
                    new List<string>(generalPermissions)
                    {
                    }
                },
                {
                    ModuleNames.Administration,
                    new List<string>(generalPermissions)
                    {
                    }
                },
                {
                    ModuleNames.CodeSet,
                    new List<string>(generalPermissions)
                    {
                        PermissionNames.ViewCode,
                        PermissionNames.CreateCodeEntry,
                        PermissionNames.UpdateCode,
                        PermissionNames.DeleteCode,
                        PermissionNames.ViewAlias,
                        PermissionNames.CreateAlias,
                        PermissionNames.UpdateAlias,
                        PermissionNames.DeleteAlias,
                        PermissionNames.ViewAssociation,
                        PermissionNames.CreateAssociation,
                        PermissionNames.UpdateAssociation,
                        PermissionNames.DeleteAssociation
                    }
                },
                {
                    ModuleNames.ProjectManagement,
                    new List<string>(generalPermissions)
                    {
                        PermissionNames.AddPersonnel,
                        PermissionNames.DeletePersonnel,
                        PermissionNames.AddDocument,
                        PermissionNames.DeleteDocument,
                        PermissionNames.AddPatient,
                        PermissionNames.DeletePatient,
                    }
                },
                {
                    ModuleNames.ClinicalPathway,
                    new List<string>(generalPermissions)
                    {
                    }
                },
                {
                    ModuleNames.ClinicalOncology,
                    new List<string>(generalPermissions)
                    {
                    }
                }
            };
        }

        private List<string> GetAllGeneralPermissions()
        {
            return new List<string>
            {
                PermissionNames.Create,
                PermissionNames.Update, 
                PermissionNames.Delete,
                PermissionNames.View
            };
        }

        #endregion /Get Modules and Permissions

        private void SetInitialPositionPermission()
        {
            List<Code> positionCodes = codeDAL.GetByCodeSetId((int)CodeSetList.Role);
            List<PositionPermission> positionPermissionsForInsert = new List<PositionPermission>();
            if (IsNewPositionPermission(positionCodes))
            {
                Code roleSuperAdministrator = GetPositionCode(positionCodes, PredifinedRole.SuperAdministrator);
                if (roleSuperAdministrator != null)
                {
                    positionPermissionsForInsert.AddRange(PreparePositionPermissionForInsert(roleSuperAdministrator.CodeId, permissionModuleDAL.GetAllByModule(ModuleNames.Administration)));
                }

                Code roleDoctor = GetPositionCode(positionCodes, PredifinedRole.Doctor);
                if (roleDoctor != null)
                {
                    positionPermissionsForInsert.AddRange(PreparePositionPermissionForInsert(roleDoctor.CodeId, permissionModuleDAL.GetAllByModule(ModuleNames.Patients)));
                }
            }

            positionPermissionDAL.InsertMany(positionPermissionsForInsert);
        }

        private bool IsNewPositionPermission(List<Code> positionCodes)
        {
            return positionPermissionDAL.Count() == 0 && positionCodes.Count > 0;
        }

        private Code GetPositionCode(List<Code> positionCodes, PredifinedRole roleName)
        {
            return positionCodes.Find(p => p.ThesaurusEntry.Translations.Exists(t => t.PreferredTerm == roleName.ToString()));
        }

        private List<PositionPermission> PreparePositionPermissionForInsert(int positionId, List<PermissionModule> permissionModules)
        {
            return permissionModules.Select(p => new PositionPermission() { PositionCD = positionId, PermissionModuleId = p.PermissionModuleId }).ToList();
        }
    }
}
