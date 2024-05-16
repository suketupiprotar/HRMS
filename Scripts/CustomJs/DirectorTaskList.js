
$(document).ready(function () {
    bindDatatable();
});
let table;
function bindDatatable() {
     table = $("#directorTaskList")
        .DataTable({
            
            "ajax": {
                "url": "/Director/TaskList",
                "type": "post",
                //error: function (err) {
                //    console.log(err)
                //    alert("An error occurred");
                //}
                
            },
            "serverSide": true,
            "processing": true,
            "paging": true,
            "searchable": true,
            //"order": [[1, "asc"], [2, "desc"]],
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
                    "render": function (data, type, row) {
                        console.log(row)
                        if (row.ApprovedorRejectedBy == null) {
                            return "Pending";
                        }
                        else {
                            return row.ApprovedorRejectedBy;
                        }
                    }
                },
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
                    
                    }
                }
            ]
        });
}


function reloadTable() {
    table.ajax.reload(null, false);
}

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


//new Date(parseInt(item.BirthDate.match(/\d+/)[0])).toDateString()                     