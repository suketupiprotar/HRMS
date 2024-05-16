let table;
let childTable;
$(document).ready(function () {
    
    bindDatatable();
});
function bindDatatable() {
    table = $("#directorDashboard")
        .DataTable({
            "ajax": {
                "url": "/Director/Dashboard",
                "type": "post",
            },
            "serverSide": true,
            "processing": true,
            "paging": true,
            "searchable": true,
            /*"order": [[1, "asc"], [2, "desc"]],*/
            "language": {
                "emptyTable": "No record found.",
                "processing":
                    '<i class="fa fa-spinner fa-spin fa-3x fa-fw" style="color:#2a2b2b;"></i><span class="sr-only">Loading...</span> '
            },
            "columns": [
                { "data": "EmployeeId", "autoWidth": true, "searchable": true },
                { "data": "FirstName", "autoWidth": true, "searchable": true },
                { "data": "LastName", "autoWidth": true, "searchable": true },
                { "data": "Email", "autoWidth": true, "searchable": true },
                { "data": "BirthDate", "autoWidth": true, "searchable": true },
                { "data": "Gender", "autoWidth": true, "searchable": true },
                { "data": "DepartmentId", "autoWidth": true, "searchable": true },
                { "data": "ReportingPerson", "autoWidth": true, "searchable": true },
                {
                    "data": null, "autoWidth": true, "searchable": false, "searchable": false,
                    "render": function (data, type, row, meta) {
                        return '<button class="btn btn-primary editButton" data-id="' + row.EmployeeId + '">Edit</button>';
                    }
                },
                {
                    "data": null, "autoWidth": true, "searchable": false,
                    "render": function (data, type, row, meta) {
                        return '<button class="btn btn-danger deleteButton" data-id="' + row.EmployeeId + '">Delete</button>';
                    }
                },
                {
                    "data": null, "autoWidth": true, "searchable": false,
                    "render": function (data, type, row, meta) {
                        return '<button class="btn btn-warning taskButton" data-id="' + row.EmployeeId + '" onclick="nestedTable(' + row.EmployeeId + ', this)">Task</button>';
                    }
                },
              
            ]
        });
}

//$(document).on("click", ".taskButton", function () {
//    var employeeId = $(this).data("id");
//    var url = "/Director/childTask/" + employeeId;
//    window.location.href = url;
//});

//$(document).on("click", ".taskButton", function () {
//    var id = $(this).data("id");
//    $.ajax({
//        type: "GET",
//        url: "/Director/childTaskView",
//        success: function (response) {
//            console.log(response);
//            window.innerhtml = response;
//            childTaskTable();
//        }
//    })
//})

//function childTaskTable() {
//    console.log("childTaskTable Called");

//}

$(document).on("click", ".editButton", function () {
    var id = $(this).data("id");
    $.ajax({
        type: "GET",
        url: "/Director/Edit/" + id,
        success: function (response) {
            document.getElementById("modalBody").innerHTML = response;
            $("#exampleModal").modal('show');
            $.validator.unobtrusive.parse($("#exampleModal"));
        },
        error: function (err) {
            alert("An error occurred while fetching data.");
        }
    });
});

$(document).on("click", ".deleteButton", function () {
    var id = $(this).data("id");
    $.ajax({
        type: "GET",
        url: "/Director/Delete/" + id,
        success: function (response) {
            if (response.success) {
                swal("Success!", "Profile Deleted Succesfully", "success").then(function () {
                    reloadTable();
                });
                
            }
            else {
                swal("Error!", "An error occurred while deleting data", "error").then(function () {
                    reloadTable();
                });
            }
        },
        error: function (err) {
            console.log(err)
            alert("An error occurred while deleting data.");
            reloadTable();
        }
    });
});

function reloadTable() {
    table.ajax.reload(null, false);
}

$(document).on("submit", "#profileForm", function (e) {
    e.preventDefault();
    var profile = new FormData(this);
    $.ajax({
        type: "POST",
        url: '/Director/Edit',
        data: profile,
        processData: false,
        contentType: false,
        success: function (response) {
            if (response.success) {
             
                swal("Success!", "Profile Updated Succesfully!", "success").then(function () {
                    $("#exampleModal").modal('hide');
                    reloadTable();
                });
            }
            else {
                swal("Error!", "Opps! Something went wrong", "Error").then(function () {
                    $("#exampleModal").modal('hide');
                    reloadTable();
                });
            }

        },
        error: function (err) {
            swal("Opps!", "Some error occured during adding data into database!", "error");
        }
    });
});


//function abcd(employeeId, ref) {
//    var r = ref.parentNode.parentNode;
//    //console.log(r);
//    if (table.row(r).child.isShown()) {
//        table.row(r).child().hide();
//    }
//    else {
//        $.ajax({
//                    type: "GET",
//                    url: "/Director/GetTasksForEmployee?empid=" + employeeId,
//                    //url: "/Director/GetTasksForEmployee/" + employeeId,
//                    success: function (response) {
//                        if (response.success) {
//                            //console.log(response)
//                            // Populate the nested DataTable with received data
//                            displayTasksDataTable(response.data,ref);
//                        } else {
//                            // Handle error
//                            console.error("Error fetching tasks:", response.message);
//                        }
//                    },
//                    error: function (err) {
//                        console.error("Error fetching tasks:", err);
//                    }
//                });
//    }
//}

//$(document).on("click", ".taskButton", function () {
//    var employeeId = $(this).data("id");
//    var tr = $(this).closest('tr');
//    var refRow = table.row(tr);
//    console.log(tr)
//    if (table.row(tr).child.isShown()) {
//        table.row(r).child().hide();
//    }
//    else {
//        $.ajax({
//            type: "GET",
//            url: "/Director/GetTasksForEmployee?empid=" + employeeId,
//            //url: "/Director/GetTasksForEmployee/" + employeeId,
//            success: function (response) {
//                if (response.success) {
//                    console.log(response)
//                    // Populate the nested DataTable with received data
//                    displayTasksDataTable(response.data);
//                } else {
//                    // Handle error
//                    console.error("Error fetching tasks:", response.message);
//                }
//            },
//            error: function (err) {
//                console.error("Error fetching tasks:", err);
//            }
//        });
//    }

//});


function nestedTable(employeeId, ref) {
    var row = $(ref).closest("tr");
    var rowData = table.row(row).data();
    var r = ref.parentNode.parentNode;
    //    //console.log(r);
    //    if (table.row(r).child.isShown()) {
    //        table.row(r).child().hide();
    //    }

    if (table.row(r).child.isShown()) {
        // If child row is already shown, hide it
        table.row(r).child.hide();
    } else {
        // If child row is not shown, fetch tasks for the employee
        $.ajax({
            type: "GET",
            url: "/Director/GetTasksForEmployee",
            data: { empid: employeeId },
            success: function (response) {
                if (response.success) {
                    // Create a new row for tasks
                    var taskRow = $("<tr>").addClass("task-row");
                    var cell = taskRow.append($("<td colspan='11'>")).find("td");

                    // Create a table for tasks
                    var taskTable = $("<table>").addClass("table table-hover table-striped");
                    cell.append(taskTable);

                    // Add header row for tasks
                    var headerRow = $("<tr>").append(
                        $("<th>").text("Task ID"),
                        $("<th>").text("Task Date"),
                        $("<th>").text("Task Name"),
                        $("<th>").text("Employee Id"),
                        $("<th>").text("Task Name"),
                        $("<th>").text("Status"),
                        $("<th>").text("Created On"),
                        $("<th>").text("Modified On"),
                        $("<th>").text("Approved or Rejected By"),
                        $("<th>").text("Action")
                        
                        // Add more headers if needed
                    );
                    taskTable.append($("<thead>").append(headerRow));

                    // Initialize DataTable for tasks
                    var taskDataTable = taskTable.DataTable({
                        data: response.data,
                        columns: [
                            { data: "TaskId" },
                            {
                                data: "TaskDate", "render": function (data, type, row) {

                                    return dateTime(data);
                                } },
                            { data: "EmployeeId" },
                            { data: "TaskName" },
                            { data: "TaskDescription" },
                            {
                                data: "Status", "render": function (data, type, row) {
                                    var badgeClass = "";
                                    var status = (data || "").trim();
                                    if (status !== "Approve" && status !== "Reject") {
                                        status = "Pending";
                                    }
                                    switch (status) {
                                        case "Approve":
                                            badgeClass = "badge text-bg-success";
                                            break;
                                        case "Reject":
                                            badgeClass = "badge text-bg-danger";
                                            break;
                                        default:
                                            badgeClass = "badge text-bg-warning";
                                            break;
                                    }
                                    return '<span class="' + badgeClass + '">' + status + '</span>';
                                } },
                            {
                                data: "CreatedOn", "render": function (data, type, row) {

                                    return dateTime(data);
                                } },
                            {
                                data: "ModifiedOn", "render": function (data, type, row) {
                                    return dateTime(data);
                                } },
                            { data: "ApprovedorRejectedBy" },
                            {
                                "data": null,
                                "autoWidth": true,
                                "render": function (data, type, row) {
                                    // Generate HTML for buttons
                                    //return '<button class="btn btn-success accept gap-3" data-id="' + row.TaskId + '">Approve</button>' +
                                    //    '<button class="btn btn-danger reject" data-id="' + row.TaskId + '">Reject</button>';

                                    var disabled = row.Status.trim().toLowerCase() === "reject" ? "disabled" : "";
                                    return '<button class="btn btn-success gap-3 accept" data-id="' + row.TaskId + '" ' + disabled + '>Approve</button>' +
                                        '<button class="btn btn-danger reject" data-id="' + row.TaskId + '">Reject</button>';

                                } }
                            
                            // Add more columns if needed
                        ]
                    });

                    
                    // Display the task row under the employee row
                    table.row(r).child(taskRow).show();
                } else {
                    console.error("Error fetching tasks:", response.message);
                }
            },
            error: function (err) {
                console.error("Error fetching tasks:", err);
            }
        });
    }
}

function dateTime(data) {
    var dateObj = new Date(parseInt(data.match(/\d+/)[0]));
    var dateString = dateObj.toDateString();
    var timeString = dateObj.toLocaleTimeString();
    return dateString + ' ' + timeString;
}

//function displayTasksDataTable(tasks,ref) {
//    console.log(tasks)
//    console.log(ref)
//    var r = ref.parentNode.parentNode;
//    var taskTable = $("<table>").addClass("table table-striped").DataTable({
//        data: tasks,
//        columns: [
//            { data: "TaskId" },
//            { data: "TaskName" },
//            { data: "TaskDescription" },
//        ]
//    });
//    if (table.row(r).child.isShown()) {
//        table.row(r).child(taskTable).hide();
//    }
//    else {
//        table.row(r).child(taskTable).show();
//    }
//    console.log(taskTable);
//    // Create DataTable for tasks
//    //childTable = $("<table>").addClass("table table-striped").appendTo("#tasksContainer").DataTable({
//    //    data: tasks,
//    //    columns: [
//    //        { data: "TaskId" },
//    //        { data: "TaskName" },
//    //        { data: "TaskDescription" },
//    //        // Add other columns as needed
//    //    ],
//    //    // Configure other DataTable options as required
//    //});

//    // Show the modal or display the DataTable in a container
//    // For example:
//    $("#tasksModal").modal("show");
//}

$(document).on("click", ".accept, .reject", function () {
    var taskId = $(this).data("id");
    var status = $(this).text().trim();
    $.ajax({
        type: 'POST',
        data: { taskId: taskId, status: status },
        //url: '@Url.Action("ApproveOrRejectTask", "Director")',
        url: '/Director/ApproveOrRejectTask',
        success: function (response) {

            if (response.success) {
                if (response.taskStatus == "Approve") {
                    swal("Success!", "Task Approved Succesfully!", "success").then(function () {
                        reloadTable();
                    });

                }
                else if (response.taskStatus == "Reject") {
                    swal("Success!", "Task Rejected Succesfully!", "success").then(function () {
                        reloadTable();
                    });
                }
                reloadTable();
            }

        },
        Error: function (ex) {

        }

    });
});

