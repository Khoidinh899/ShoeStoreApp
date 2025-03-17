var data;

$(document).ready(() => {
    fetch('/data/hanhchinh.json')
        .then((response) => response.json())
        .then((json) => {
            data = json;
            Object.keys(json).forEach(function (key) {
                let html = `<option data-id="${key}" value="${json[key].name_with_type}">${json[key].name_with_type}</option>`
                $('#select-cỉty').append(html);
            })
        });
})

$('#select-cỉty').change(function () {
    let city = $(this).find(":selected").data("id");
    let districts = data[city]["quan-huyen"];
    $('#select-district').html("");
    $('#select-ward').html("");
    Object.keys(districts).forEach(function (key) {
        let html = `<option data-id="${key}"  value="${districts[key].name_with_type}">${districts[key].name_with_type}</option>`
        $('#select-district').append(html);
    })
    let district = $('#select-district').find(":selected").data("id");
    let wards = data[city]["quan-huyen"][district]["xa-phuong"];
    $('#select-ward').html("");
    Object.keys(wards).forEach(function (key) {
        let html = `<option data-id="${key}"  value="${wards[key].name_with_type}">${wards[key].name_with_type}</option>`
        $('#select-ward').append(html);
    })
    $('#select-ward').attr('disabled', false);
    $('#select-district').attr('disabled', false);
})

$('#select-district').change(function () {
    let city = $('#select-cỉty').find(":selected").data("id");
    let district = $(this).find(":selected").data("id");
    let wards = data[city]["quan-huyen"][district]["xa-phuong"];
    $('#select-ward').html("");
    Object.keys(wards).forEach(function (key) {
        let html = `<option value="${wards[key].name_with_type}">${wards[key].name_with_type}</option>`
        $('#select-ward').append(html);
    })
    $('#select-ward').attr('disabled', false);
})