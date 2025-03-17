$("#btnSuccess").click(function () {
    var selectedOrders = [];
    $("table input:checked").each(function () {
        selectedOrders.push($(this).closest("tr").find("td:eq(1)").text()); // Assuming ID is in the second column (index 1)
    });
    console.log(selectedOrders)
    var productIds = $("#orderSelectedIdSuccess")
    productIds.val(selectedOrders.join(','))
})

$("#btnPending").click(function () {
    var selectedOrders = [];
    $("table input:checked").each(function () {
        selectedOrders.push($(this).closest("tr").find("td:eq(1)").text()); // Assuming ID is in the second column (index 1)
    });
    console.log(selectedOrders)
    var productIds = $("#orderSelectedIdPending")
    productIds.val(selectedOrders.join(','))
})

$("#btnCancelled").click(function () {
    var selectedOrders = [];
    $("table input:checked").each(function () {
        selectedOrders.push($(this).closest("tr").find("td:eq(1)").text()); // Assuming ID is in the second column (index 1)
    });
    console.log(selectedOrders)
    var productIds = $("#orderSelectedIdCancelled")
    productIds.val(selectedOrders.join(','))
})