var taskColumnName;
var taskSwitchCount = 0;
var taskIsAscending = null;

function showTaskModal(event, id, readOnly = false, reloadTaskTable = false) {
    event.stopPropagation();
    let taskId = id ? id : 0;
    setActiveClass(taskId);

    callServer({
        type: 'GET',
        url: `/Task/ShowTaskModal?taskId=${taskId}&isReadOnlyViewMode=${readOnly}`,
        success: function (data) {
            $('#addTaskModal').html(data);
            $('#addTaskModal').modal('show');
            document.getElementById('reloadTaskTable').value = reloadTaskTable;
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleResponseError(xhr, true);
        }
    });

    removeActiveClassOnCloseModal();
}

function submitTaskForm() {
    updateDisabledOptions(false);
    $("#newTaskForm").validate();

    if ($("#newTaskForm").valid()) {
        var request = {};
        var encounterId = $("#activeEncounter").val();
        request['TaskId'] = $("#taskId").val();
        request['PatientId'] = $("#pId").val();
        request['EncounterId'] = encounterId;
        request['TaskTypeCD'] = $("#taskType").val();
        request['TaskStatusCD'] = $("#taskStatus").val();
        request['TaskPriorityCD'] = $("#taskPriority").val();
        request['TaskClassCD'] = $("#taskClass").val();
        request['TaskDescription'] = $("#taskDescription").val();
        request['TaskEntityId'] = $("#taskEntityId").val();
        request['TaskStartDateTime'] = calculateDateWithOffset("#taskStartDateTime");
        request['TaskEndDateTime'] = calculateDateWithOffset("#taskEndDateTime");
        request['ScheduledDateTime'] = calculateDateWithOffset("#scheduledDateTime");
  
        callServer({
            type: 'POST',
            url: `/Task/Create`,
            data: request,
            success: function (data, jqXHR) {
                $('#addTaskModal').modal('hide');
                if (document.getElementById('reloadTaskTable').value === "true")
                    reloadTasks(null, null, true);
                else
                    showDetails(encounterId);
                toastr.success("Success");
            },
            error: function (xhr, thrownError) {
                handleResponseError(xhr);
            }
        });
    }
    return false;
}

function setActiveClass(taskId) {
    var taskActive = $(".blue-pencil-task").attr("data-task-id");
    if (taskActive != taskId)
        $(".edit").removeClass("blue-pencil-task");
    $(`#edit-${taskId}`).toggleClass("blue-pencil-task");
}

function removeActiveClassOnCloseModal() {
    $('#addTaskModal').on('hidden.bs.modal', function () {
        $('.blue-pencil-task').removeClass('blue-pencil-task');
    });
}

function showTasksContent(taskColumnName, taskIsAscending, isFilter) {
    $("#taskTab").addClass("code-active-tab");
    $("#eocTab").addClass("remove-eoc-tab");
    $("#eocTab").removeClass("code-active-tab");
    $("#encounterTab").removeClass("code-active-tab");
    reloadTasks(taskColumnName, taskIsAscending, isFilter);
}

function reloadTasks(taskColumnName, taskIsAscending, isFilter) {
    var request = {};
    request['PatientId'] = $('#patientId').val();
    request['IsAscending'] = taskIsAscending;
    request['ColumnName'] = taskColumnName;
    request['TaskStatus'] = $('#TaskStatusTemp').val();

    callServer({
        type: 'GET',
        url: '/Task/ReloadTable',
        data: request,
        success: function (data) {
            if (!isFilter)
                showTaskFilterGroup();
            $("#eocContainer").html(data);
            addTaskSortArrows();
            setTableMaxHeight("taskTable", "taskTableContent");
        },
        error: function (xhr, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function sortTaskTable(column) {
    if (taskSwitchCount == 0) {
        if (taskColumnName == column)
            taskIsAscending = checkIfAsc(taskIsAscending);
        else
            taskIsAscending = true;
        taskSwitchCount++;
    }
    else {
        if (taskColumnName != column)
            taskIsAscending = true;
        else
            taskIsAscending = checkIfAsc(taskIsAscending);
        taskSwitchCount--;
    }
    taskColumnName = column;

    showTasksContent(taskColumnName, taskIsAscending, true);
}

function addTaskSortArrows() {
    var element = document.getElementById(taskColumnName);
    if (element != null) {
        element.classList.remove("sort-arrow");
        if (taskIsAscending) {
            element.classList.remove("sort-arrow-desc");
            element.classList.add("sort-arrow-asc");
        }
        else {
            element.classList.remove("sort-arrow-asc");
            element.classList.add("sort-arrow-desc");
        }
    }
}

function showTaskFilterGroup() {
    callServer({
        type: "GET",
        url: '/Task/ShowTaskFilterGroup',
        success: function (data) {
            $("#filterGroup").html(data);
            setTableMaxHeight("taskTable", "taskTableContent");
        },
        error: function (xhr, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function taskFilter() {
    pushState();
    showTasksContent(null, null, true);
}

function clickedTaskRow(e, id) {
    if (!$(e.target).hasClass('dropdown-button') && !$(e.target).hasClass('fa-bars') && !$(e.target).hasClass('dropdown-item') && !$(e.target).hasClass('dots') && !$(e.target).hasClass('table-more')) {
        showTaskModal(e, id, true, true);
    }
}