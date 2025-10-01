$(document).ready(function () {
    saveInitialFormData("#roleForm");
});

addUnsavedChangesEventHandler("#roleForm");

function reloadTable() {
    let requestObject = {};
    checkUrlPageParams();
    setTableProperties(requestObject);

    if (!requestObject.Page) {
        requestObject.Page = 1;
    }

    callServer({
        type: 'GET',
        url: '/RoleAdministration/ReloadTable',
        data: requestObject,
        success: function (data) {
            setTableContent(data);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function createEntity() {
    window.location.href = "/RoleAdministration/Create";
}

function editEntity(event, id) {
    window.location.href = `/RoleAdministration/Edit?roleCD=${id}`;
    event.preventDefault();
}

function viewEntity(event, id) {
    window.location.href = `/RoleAdministration/View?roleCD=${id}`;
    event.preventDefault();
}

$(document).on('change', '.check-permissions-per-module', function () {
    checkUncheckPermissionsPerModule($(this));
});

function checkUncheckPermissionsPerModule($checkbox) {
    let moduleId = $checkbox.val();
    let shouldCheckPermission = false;
    if ($checkbox.is(":checked")) {
        shouldCheckPermission = true;
    }
    $(`.module-${moduleId}-permission`).prop("checked", shouldCheckPermission);
}

function submitRoleForm(form, event) {
    event.preventDefault();
    $('#roleForm').validate();
    if ($(form).valid()) {
        callServer({
            type: 'POST',
            url: '/RoleAdministration/Edit',
            data: getRole(),
            success: function (data) {
                toastr.success('Success');
                if (isNewRoleCreated()) {
                    editEntity(event, data.id);
                }
                saveInitialFormData("#roleForm");
            },
            error: function (xhr, textStatus, thrownError) {
                handleResponseError(xhr);
            }
        });
    }
}

function getRole() {
    let role = {};

    let roleCD = $('#roleCD').val();

    role['Id'] = roleCD;
    role['CheckedPermissionModules'] = getSelectedPermissions(roleCD);

    return role;
}

function getSelectedPermissions(roleCD) {
    let permissions = [];
    $('.position-permission:checked').each(function (index, element) {
        permissions.push(
            {
                permissionModuleId: $(element).val(),
                positionId: roleCD
            }
        );
    });

    return permissions;
}

function cancelEditRole() {
    unsavedChangesCheck("#roleForm",
        function () {
            window.location.href = '/RoleAdministration/GetAll';
        },
        function () {
            window.location.href = '/RoleAdministration/GetAll';
        }
    );
}

function isNewRoleCreated() {
    return !$('#roleCD').val();
}