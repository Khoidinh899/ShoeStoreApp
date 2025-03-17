$("#btn-add-variant").click(function () {
    console.log("1")
    var count = $(".variant-item").length
    let html = `
                            <div class="variant-item border rounded border-1 px-3 py-2 mb-2">
                                <input type="hidden" class="color-input" name="color"/>                                
                                <div class="variant-title">
                                    Biến thể ${count + 1}
                                </div>
                                <div class="variant-color-container container-fluid row">
                                    <div class="form-check col-4">
                                        <input
                                               class="form-check-input"
                                               type="checkbox"
                                               value="black"
                                               id="color-black" />
                                        <label class="form-check-label"
                                               for="color-black">
                                            Đen
                                        </label>
                                    </div>
                                    <div class="form-check col-4">
                                        <input 
                                               class="form-check-input"
                                               type="checkbox"
                                               value="white"
                                               id="color-white" />
                                        <label class="form-check-label"
                                               for="color-white">
                                            Trắng
                                        </label>
                                    </div>
                                    <div class="form-check col-4">
                                        <input  class="form-check-input"
                                               type="checkbox"
                                               value="blue"
                                               id="color-blue" />
                                        <label class="form-check-label"
                                               for="color-blue">
                                            Xanh dương
                                        </label>
                                    </div>
                                    <div class="form-check col-4">
                                        <input  class="form-check-input"
                                               type="checkbox"
                                               value="green"
                                               id="color-green" />
                                        <label class="form-check-label"
                                               for="color-green">
                                            Xanh lục
                                        </label>
                                    </div>
                                    <div class="form-check col-4">
                                        <input class="form-check-input"
                                               type="checkbox"
                                               value="pink"
                                               id="color-pink" />
                                        <label class="form-check-label"
                                               for="color-pink">
                                            Hồng
                                        </label>
                                    </div>
                                    <div class="form-check col-4">
                                        <input class="form-check-input"
                                               type="checkbox"
                                               value="red"
                                               id="color-red" />
                                        <label class="form-check-label"
                                               for="color-red">
                                            Đỏ
                                        </label>
                                    </div>
                                    <div class="form-check col-4">
                                        <input  class="form-check-input"
                                               type="checkbox"
                                               value="orange"
                                               id="color-orange" />
                                        <label class="form-check-label"
                                               for="color-orange">
                                            Cam
                                        </label>
                                    </div>
                                    <div class="form-check col-4">
                                        <input  class="form-check-input"
                                               type="checkbox"
                                               value="gray"
                                               id="color-gray" />
                                        <label class="form-check-label"
                                               for="color-gray">
                                            Xám
                                        </label>
                                    </div>
                                    <div class="form-check col-4">
                                        <input class="form-check-input"
                                               type="checkbox"
                                               value="yellow"
                                               id="color-yellow" />
                                        <label class="form-check-label"
                                               for="color-yellow">
                                            Vàng
                                        </label>
                                    </div>
                                </div>
                                <div class="col-6">
                                    <label for="variant-img"
                                           class="form-label">Ảnh minh họa</label>
                                    <input name="file" class="form-control"
                                           type="file"
                                           id="variant-img" />
                                </div>
                                <div class="btn-icon btn-delete text-secondary">
                                    <i class="fa-solid fa-trash"></i>
                                </div>
                            </div>
    `
    $(".variant-container").append(html)
    $("input[type='checkbox']").unbind("change")
    $("input[type='checkbox']").change(function () {
        let item = $(this.closest('.variant-item'))
        let input = item.find('.color-input')
        let colors = $.map(item.find("input[type='checkbox']:checked"), e => $(e).val()).join(' / ')
        input.val(colors)
        console.log(colors)
    })
    $(".btn-delete").click(function () {
        console.log("1")
        let item = $(this.closest('.variant-item'))
        item.remove()
    })
})
$("input[type='checkbox']").change(function () {
    let item = $(this.closest('.variant-item'))
    let input = item.find('.color-input')
    let colors = $.map(item.find("input[type='checkbox']:checked"), e => $(e).val()).join(' / ')
    input.val(colors)
    console.log(colors)
})

$("#deleteButton").click(function () {
    var selectedProductIds = [];
    $("table input:checked").each(function () {
        selectedProductIds.push($(this).closest("tr").find("td:eq(1)").text()); // Assuming ID is in the second column (index 1)
    });
    var productIds = $("#productSelectedIds")
    productIds.val(selectedProductIds.join(','))
})