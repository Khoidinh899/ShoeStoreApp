
$('.size-card').click(function (e) {

    if ($(e.currentTarget).hasClass('disabled'))
        return false;

    $('.size-card').each(function(index, element) {
        if ($(element).hasClass('selected')) {
            $(element).removeClass('selected');
        }
    })
    $(e.currentTarget).addClass('selected');

    let itemid = $(e.currentTarget).data("itemid");
    $('#input-itemid').val(itemid);

    $('.quantity-display').each(function (index, element) {
        if ($(element).data("itemid") === itemid) {
            $(element).removeClass('hidden');
        }
        else
            $(element).addClass('hidden');
         })
 })
$('.img-container').click(function(e) {
    $('.img-container').each(function (index, element) {
        if ($(element).hasClass('selected')) {
            $(element).removeClass('selected');
        }
    })
        let variantId = $(e.currentTarget).find('.color-img').data("variantid");
$('.size-container').each(function(index, element) {
            if ($(element).data('variantid') === variantId) {
    $(element).removeClass('hidden');
            }
else
$(element).addClass('hidden');
        })
$(e.currentTarget).addClass('selected');
$('#main-img').attr('src', $(e.currentTarget).find('.color-img').attr('src'))
    })

$('.variant').mouseover(function(e) {
    let parentCard = $(e.currentTarget).closest('.card');
    let imgSrc = $(e.currentTarget).find('img').attr('src');
    let displayImg = parentCard.find('.display-img');
    displayImg.attr('src', imgSrc)
    }).mouseout(function(e) {
    let parentCard = $(e.currentTarget).closest('.card');
    let displayImg = parentCard.find('.display-img');
    displayImg.attr('src', displayImg.data('originalimg'));
    })
