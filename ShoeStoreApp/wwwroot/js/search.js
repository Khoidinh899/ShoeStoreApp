$('#search').keyup(function () {
    console.log($(this).val());
    if ($(this).val() !== "") {
        $.get(window.location.origin + "/Products/Search?q=" + $(this).val(), function (data) {
            $("#search-hint").html("");
            data.forEach(e => {
                console.log(e)
                let html = `<li><a class="dropdown-item">${e.name}</a></li>`
                $("#search-hint").append(html);
                $('#search-hint .dropdown-item').click(function () {
                    console.log("ASD")
                    $("#search").val($(this).text());
                })
            })
        })
    }
}
)

$('#search').focusin(function () {
    $("#search-form .dropdown-menu").addClass("show");
})

$('#search').focusout(function () {
    setTimeout(() => $("#search-form .dropdown-menu").removeClass("show"), 200)

})

$('#search-hint .dropdown-item').click(function () {
    $("#search").val($(this).text());
})

$('#search-btn').click(() => {
    $("#search-form").submit();
})