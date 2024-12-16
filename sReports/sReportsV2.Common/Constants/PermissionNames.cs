namespace sReportsV2.Common.Constants
{
    public static class PermissionNames
    {
        // general permissions
        public const string Create = "Create";
        public const string Update = "Update";
        public const string Delete = "Delete";
        public const string View = "View";

        // administration permissions
        // ...

        // simulator permissions
        // ...

        // designer permisssions
        public const string ShowJson = "ShowJson";
        public const string ChangeState = "ChangeState";
        public const string FindConsensus = "FindConsensus";
        public const string ViewAdministrativeData = "ViewAdministrativeData";
        public const string ViewComments = "ViewComments";
        public const string AddComment = "AddComment";
        public const string ChangeCommentStatus = "ChangeCommentStatus";

        // engine permisssions
        public const string Download = "Download";
        public const string SignFormInstance = "SignFormInstance";
        public const string ChangeFormInstanceState = "ChangeFormInstanceState";
        public const string LockPage = "LockPage";
        public const string UnlockPage = "UnlockPage";
        public const string LockChapter = "LockChapter";
        public const string UnlockChapter = "UnlockChapter";

        // thesaurus permissions
        public const string CreateCode = "CreateCode";
        public const string UMLS = "UMLS";

        // patient permissions
        public const string AddEpisodeOfCare = "AddEpisodeOfCare";
        public const string RemoveEpisodeOfCare = "RemoveEpisodeOfCare";
        public const string UpdateEpisodeOfCare = "EditEpisodeOfCare";
        public const string ViewEpisodeOfCare = "ViewEpisodeOfCare";
        public const string AddEncounter = "Add Encounter";
        public const string RemoveEncounter = "RemoveEncounter";
        public const string ViewEncounter = "ViewEncounter";
        public const string UpdateEncounter = "UpdateEncounter";
        public const string ViewPatientList = "View Patient List";
        public const string CreatePatientList = "Create Patient List";
        public const string UpdatePatientList = "Update Patient List";
        public const string DeletePatientList = "Delete Patient List";
        public const string AddPatientListUsers = "Add Patient List Users";
        public const string RemovePatientListUsers = "Remove Patient List Users";
        public const string AddPatientToPatientList = "Add Patient to Patient List";
        public const string RemovePatientFromPatientList = "Remove Patient from Patient List";
        public const string UpdateDocument = "UpdateDocument";
        public const string ViewDocument = "ViewDocument";

        // code set permissions
        public const string ViewCode = "ViewCode";
        public const string CreateCodeEntry = "CreateCodeEntry";
        public const string UpdateCode = "UpdateCode";
        public const string DeleteCode = "DeleteCode";
        public const string ViewAlias = "ViewAlias";
        public const string CreateAlias = "CreateAlias";
        public const string UpdateAlias = "UpdateAlias";
        public const string DeleteAlias = "DeleteAlias";
        public const string ViewAssociation = "ViewAssociation";
        public const string CreateAssociation = "CreateAssociation";
        public const string UpdateAssociation = "UpdateAssociation";
        public const string DeleteAssociation = "DeleteAssociation";

        // project management permissions
        public const string AddPersonnel = "AddPersonnel";
        public const string DeletePersonnel = "DeletePersonnel";
        public const string AddDocument = "AddDocument";
        public const string DeleteDocument = "DeleteDocument";
        public const string AddPatient = "AddPatient";
        public const string DeletePatient = "DeletePatient";

    }
}
