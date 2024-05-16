$(document).on("click", ".ownProfileEdit", function () {
    var id = $(this).data("id");
    $.ajax({
        type: "GET",
        url: "/Manager/Edit/" + id,
        success: function (response) {
            document.getElementById("ownProfileModalBody").innerHTML = response;
            $("#ownProfileModal").modal('show');
            $.validator.unobtrusive.parse($("#ownProfileModal"));
        },
        error: function (err) {
            alert("An error occurred while fetching data.");
        }
    });
});

$(document).on("submit", "#ownProfileForm", function (e) {
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
                    $("#ownProfileModal").modal('hide');
                });
            }
            else {
                swal("Error!", "Opps! Something went wrong", "Error").then(function () {
                    $("#ownProfileModal").modal('hide');
                   
                });
            }

        },
        error: function (err) {
            swal("Opps!", "Some error occured during adding data into database!", "error");
        }
    });
});