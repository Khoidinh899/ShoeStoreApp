$("#deleteButton").click(function () {
    var selectedProductIds = [];
    $("table input:checked").each(function () {
        selectedProductIds.push($(this).closest("tr").find("td:eq(1)").text()); // Assuming ID is in the second column (index 1)
    });
    var productIds = $("#productSelectedIds")
    productIds.val(selectedProductIds.join(','))
})

$('#btn-import-csv').click((e) => {
    $('#import-csv').click();
})