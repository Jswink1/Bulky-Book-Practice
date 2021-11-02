var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {

    /*#tblData is the id of the table element*/
    dataTable = $('#tblData').DataTable({

        /*define the ajax to load our API URL path in the controller*/
        "ajax": {
            "url": "/Admin/CoverType/GetAll"
        },
        /*define the columns*/
        "columns": [
            { "data": "name", "width": "60%" },
            {
                "data": "id",
                "render": function (data) {
                    /*Double quotes arent allowed here so use the backticks.*/
                    /*Create these buttons in the .cshtml file first so you can use intellisense*/
                    return `
                                <div class="text-center">
                                    <a href="/Admin/CoverType/Upsert/${data}" class="btn btn-success text-white" style="cursor:pointer;">
                                        <i class="fas fa-edit"></i>
                                    </a>
                                    <a onclick=Delete("/Admin/CoverType/Delete/${data}") class="btn btn-danger text-white" style="cursor:pointer">
                                        <i class="fas fa-trash-alt"></i>
                                    </a>
                                </div>
                           `
                }, "width": "40%"
            }
        ]
    })
};

/*SweetAlert to confirm delete*/
function Delete(url) {
    swal({
        title: "Are you sure you want to Delete?",
        text: "You will not be able to restore the data!",
        icon: "warning",
        buttons: true,
        dangerMode: true
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                type: "DELETE",
                url: url,
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        dataTable.ajax.reload();
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            });
        }
    });
}