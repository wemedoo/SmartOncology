function handleSuccessFormSubmitFromEngine(data, projectId, showUserProjects) {
    $(document).off('click', '.dropdown-matrix');
    if (getFormInstanceId()) {
        reloadAfterFormInstanceChange();
    } else {
        if (projectId != "") {
            if (showUserProjects == "true") {
                window.location.href = `/FormInstance/EditForUserProject?VersionId=${data.formVersionId}&FormInstanceId=${data.formInstanceId}&ProjectId=${projectId}`;
            }
            else {
                window.location.href = `/FormInstance/EditForProject?VersionId=${data.formVersionId}&FormInstanceId=${data.formInstanceId}&ProjectId=${projectId}`;
            }
        }
        else {
            window.location.href = `/FormInstance/Edit?VersionId=${data.formVersionId}&FormInstanceId=${data.formInstanceId}`;
        }
    }
}

function handleBackInFormAction() {
    let versionId = $('input[name=VersionId]').val();
    let thesaurusId = $('input[name=thesaurusId]').val();
    let formDefinitionId = $('input[name=formDefinitionId]').val();

    if (!compareForms("#fid")) {
        if (confirm("You have unsaved changes. Are you sure you want to cancel?")) {
            saveInitialFormData("#fid");
            window.location.href = `/FormInstance/GetAllByFormThesaurus?versionId=${versionId}&thesaurusId=${thesaurusId}&formDefinitionId=${formDefinitionId}`;
        }
    } else {
        window.location.href = `/FormInstance/GetAllByFormThesaurus?versionId=${versionId}&thesaurusId=${thesaurusId}&formDefinitionId=${formDefinitionId}`;
    }
}

function isPatientModule() {
    return false;
}

// END: FORM INSTANCE METHODS