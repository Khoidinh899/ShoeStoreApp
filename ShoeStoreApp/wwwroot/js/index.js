

$('.card').mouseover(function (e) {
    $(e.currentTarget).find('.img-container').removeClass('hidden');
}).mouseout(function (e) {
    $(e.currentTarget).find('.img-container').addClass('hidden');
})

// sticky
let topLimit = $('#filter-sidebar').offset().top;
$(window).scroll(function () {
    console.log(topLimit <= $(window).scrollTop())
    if (topLimit <= $(window).scrollTop()) {
        $('#filter-sidebar').addClass('stickIt')
    } else {
        $('#filter-sidebar').removeClass('stickIt')
    }
})

$('.variant').mouseover(function (e) {
    let parentCard = $(e.currentTarget).closest('.card');
    let imgSrc = $(e.currentTarget).find('img').attr('src');
    let displayImg = parentCard.find('.display-img');
    displayImg.attr('src', imgSrc)
}).mouseout(function (e) {
    let parentCard = $(e.currentTarget).closest('.card');
    let displayImg = parentCard.find('.display-img');
    displayImg.attr('src', displayImg.data('originalimg'));
})




function filter() {
    let queryString = 'search?';
    queryString += $.map($("input[name='gender[]']:checked"), e => 'gender=' + $(e).val()).join("&") + '&';
    queryString += $.map($("input[name='color[]']:checked"), e => 'color=' + $(e).val()).join("&") + '&';
    queryString += $.map($("input[name='brand[]']:checked"), e => 'brand=' + $(e).val()).join("&");
    queryString += $.map($("input[name='price[]']:checked"), e => 'price=' + $(e).val()).join("&");
    while (queryString.at(queryString.length - 1) === "&")
        queryString = queryString.substring(0, queryString.length - 1);
    window.location.href = queryString;
}

$('.category-link').click(function () {
    $('#category-input').val($(this).data('category'));
    $("#filter-sidebar").submit();
    return false;
})

$('.sort-link').click(function () {
    $('#sort-input').val($(this).data('sort'));
    $("#filter-sidebar").submit();
    return false;
})