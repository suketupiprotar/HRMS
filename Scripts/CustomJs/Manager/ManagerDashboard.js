let table;

$(document).ready(function () {
    bindDatatable();
});

function reloadTable() {
    table.ajax.reload(null, false);
}
function bindDatatable() {
    table = $("#ManagerDashboard")
        .DataTable({
            "ajax": {
                "url": "/Manager/Dashboard",
                "type": "post",
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
                        return '<button class="btn btn-warning taskButton" data-id="' + row.EmployeeId + '">Task</button>';
                    }
                },

            ]
        });
}

$(document).on("click", ".editButton", function () {
    var id = $(this).data("id");
    $.ajax({
        type: "GET",
        url: "/Manager/Edit/" + id,
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


//Delete Employee(Person)
$(document).on("click", ".deleteButton", function () {
    var id = $(this).data("id");
    debugger
    $.ajax({
        type: "GET",
        url: "/Manager/Delete/" + id,
        success: function (response) {
            console.log(response)
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
            alert("An error occurred while deleting data.");
        }
    });
});

//Edit Profile
$(document).on("submit", "#profileForm", function (e) {
    e.preventDefault();
    var profile = new FormData(this);
    $.ajax({
        type: "POST",
        url: '/Manager/Edit',
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