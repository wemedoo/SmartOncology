function handleSuccessFormSubmitFromEngine(data, projectId, showUserProjects, callback) {
    $(document).off('click', '.dropdown-matrix');
    if (getFormInstanceId()) {
        reloadAfterFormInstanceChange(callback);
    } else {
        window.location.href = getEditFormInstanceUrl(data, projectId, showUserProjects);
    }
}

function getEditFormInstanceUrl(data, projectId, showUserProjects) {
    let apiUrl;
    if (projectId != "") {
        if (showUserProjects) {
            apiUrl = `/FormInstance/EditForUserProject?VersionId=${data.formVersionId}&FormInstanceId=${data.formInstanceId}&ProjectId=${projectId}`;
        } else {
            apiUrl = `/FormInstance/EditForProject?VersionId=${data.formVersionId}&FormInstanceId=${data.formInstanceId}&ProjectId=${projectId}`;
        }
    } else {
        apiUrl = `/FormInstance/Edit?VersionId=${data.formVersionId}&FormInstanceId=${data.formInstanceId}`;
    }
    return apiUrl;
}

function handleBackInFormAction() {
    let versionId = $('input[name=VersionId]').val();
    let thesaurusId = $('input[name=thesaurusId]').val();
    let formDefinitionId = $('input[name=formDefinitionId]').val();

    unsavedChangesCheck("#fid",
        function () {
            window.location.href = `/FormInstance/GetAllByFormThesaurus?versionId=${versionId}&thesaurusId=${thesaurusId}&formDefinitionId=${formDefinitionId}`;
        },
        function () {
            window.location.href = `/FormInstance/GetAllByFormThesaurus?versionId=${versionId}&thesaurusId=${thesaurusId}&formDefinitionId=${formDefinitionId}`;
        }
    );
}

function isPatientModule() {
    return false;
}

// END: FORM INSTANCE METHODS