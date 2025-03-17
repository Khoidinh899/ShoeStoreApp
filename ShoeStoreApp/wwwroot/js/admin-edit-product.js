$(document).ready(function () {
    var numberOfVariant = $("#numberVariant").val()
    for (let i = 1; i <= numberOfVariant; i++) {
        var tag = "#variantColor-"+ "" + i 
        var variantColorTag = $(tag);
        var varianColorArray = variantColorTag.val().split(' / ')
        var tagCheckBox = ".form-check-input-" + "" + i
        $(tagCheckBox).each(function () {
            var checkboxValue = $(this).val();
            if (varianColorArray.includes(checkboxValue)) {
                $(this).prop("checked", true);
            }
        });
    }
    
})
$("input[type='checkbox']").change(function () {
    let item = $(this.closest('.variant-item'))
    let colors = $.map(item.find("input[type='checkbox']:checked"), e => $(e).val()).join(' / ')
    var numberOfVariant = $("#numberVariant").val()
    for (let i = 1; i <= numberOfVariant; i++)
    {
        let tagColor = "form-check-input-" + i
        if ($(this).hasClass(tagColor)) {
            var tag = "#variantColor-" + "" + i
            var variantColorTag = $(tag);
            variantColorTag.val(colors)
            console.log(variantColorTag.val())
        }
    }
})
$("#btn-edit-product").click(function () {
    var numberOfVariant = $("#numberVariant").val()
    for (let i = 1; i <= numberOfVariant; i++) {
        let tag = "#variantSizeForColor-" + "" + i
        let variantSizeForColor = $(tag)
        let sizeSave = ""
        for (let j = 36; j <= 45; j++) {
            let tagSize = "#variantSizeForColor-" + "" + i + "-" + j
            sizeSave = sizeSave + "" + j + "-" + $(tagSize).val() + " / "
        }
        variantSizeForColor.val(sizeSave.slice(0, -3))
    }
})
$(".variant-img").change(function () {
    var numberOfVariant = $("#numberVariant").val()
    let productImgs = ""
    for (let i = 1; i <= numberOfVariant; i++)
    {
        let tag = $("#variant-img-" + "" + i)
        let variantImg = $(this)
        if (variantImg.attr("id") === tag.attr("id")) {
            productImgs = i + " / "
        }
    }
    productImgs = $("#saveProductEditImage").val() + productImgs
    $("#saveProductEditImage").val(productImgs)
    console.log($("#saveProductEditImage").val())
})