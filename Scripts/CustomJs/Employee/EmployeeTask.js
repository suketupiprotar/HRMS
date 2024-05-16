

let table;
$(document).ready(function () {
  
    bindDatatable();
});

function bindDatatable() {
    table = $("#EmployeeTask")
        .DataTable({
            "ajax": {
                "url": "/Employee/EmployeeTask",
                "type": "post",
            },
            "serverSide": true,
            "processing": true,
            "paging": true,
            "searchable": true,
            "order": [[0, "asc"], [2, "desc"]],
            "language": {
                "emptyTable": "No record found.",
                "processing":
                    '<i class="fa fa-spinner fa-spin fa-3x fa-fw" style="color:#2a2b2b;"></i><span class="sr-only">Loading...</span> '
            },
            "columns": [
                { "data": "TaskId", "autoWidth": true, "searchable": true },
                {
                    "data": "TaskDate", "autoWidth": true, "searchable": true, "render": function (data, type, row) {

                        var dateObj = new Date(parseInt(data.match(/\d+/)[0]));
                        var dateString = dateObj.toDateString();
                        var timeString = dateObj.toLocaleTimeString();
                        return dateString;
                    } },
                { "data": "EmployeeId", "autoWidth": true, "searchable": true },
                { "data": "TaskName", "autoWidth": true, "searchable": true },
                { "data": "TaskDescription", "autoWidth": true, "searchable": true },
                /*{ "data": "Status", "autoWidth": true },*/
                {
                    "data": "Status",
                    "autoWidth": true,
                    "render": function (data, type, row) {
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
                    }
                },
                /*{ "data": "CreatedOn", "autoWidth": true },*/
                {
                    "data": "CreatedOn",
                    "autoWidth": true,
                    "render": function (data, type, row) {

                        var dateObj = new Date(parseInt(data.match(/\d+/)[0]));
                        var dateString = dateObj.toDateString();
                        var timeString = dateObj.toLocaleTimeString();
                        return dateString + ' ' + timeString;
                    }
                },
                {
                    "data": "ModifiedOn",
                    "autoWidth": true,
                    "render": function (data, type, row) {
                        // Parse the date string and format it
                        var dateObj = new Date(parseInt(data.match(/\d+/)[0]));
                        var dateString = dateObj.toDateString();
                        var timeString = dateObj.toLocaleTimeString();
                        return dateString + ' ' + timeString;
                    }
                },
                //{ "data": "ApprovedorRejectedBy", "autoWidth": true },
                {
                    "data": "ApprovedorRejectedBy",
                    "autoWidth": true,
                   
                },
                {
                    "data": null,
                    "autoWidth": true,
                    "render": function (data, type, row) {
                        // Generate HTML for buttons
                        //return '<button class="btn btn-success accept gap-3" data-id="' + row.TaskId + '">Approve</button>' +
                        //    '<button class="btn btn-danger reject" data-id="' + row.TaskId + '">Reject</button>';
                        return '<button class="btn btn-success gap-3 employeeTaskCreateOrUpdate" data-id="' + row.TaskId + '">Edit</button>' +
                            '<button class="btn btn-danger deleteButton" data-id="' + row.TaskId + '">Delete</button>';
                    }
                }
            ]
        });
}

$(document).on("click", ".employeeTaskCreateOrUpdate", function () {
    var id = $(this).data("id");
    $.ajax({
        type: "GET",
        url: "/Employee/TaskCreate/" + id,
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

$(document).on("submit", "#taskForm", function (e) {
    e.preventDefault();
 
    var taskData = new FormData(this);

    $.ajax({
        type: "POST",
        url: '/Employee/TaskCreate',
        data: taskData,
        processData: false,
        contentType: false,
        success: function (response) {
            if (response.success) {
                swal("Success!", "Data Added Successfully", "success").then(function () {
                    $("#exampleModal").modal('hide');
                    table.ajax.reload(null, false);
                });
            }

        },
        error: function (err) {
            swal("Opps!", "Some error occured during adding data into database!", "error");
        }
    });
});


$(document).on("click", ".deleteButton", function () {
    var id = $(this).data("id");
    $.ajax({
        type: "GET",
        url: "/Employee/EmployeeTaskDelete/" + id,
        success: function (response) {
            if (response.success) {
                swal("Success!", "Task Deleted Succesfully", "success").then(function () {
                    table.ajax.reload(null, false);
                });  
            }
            else {
                swal("Error!", "An error occurred while deleting data", "error").then(function () {
                    table.ajax.reload(null, false);
                });
               
            }
        },
        error: function (err) {
            alert("An error occurred while deleting data.");
        }
    });
});
