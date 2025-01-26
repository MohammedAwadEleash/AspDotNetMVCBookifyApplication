var currentCopies = [];

var selectedCopies = [];
var isEditMode = false;
function onAddCopySuccess(copy)
{
    
    $('#Value').val('');

    var bookId = $(copy).find('.js-copy').data('book-id');
    if (selectedCopies.find(c => c.bookId == bookId))
    {
        showErrorMessage("You cannot add  more than one copy for the same book");
        return

    }

    $('#CopiesForm').prepend(copy);
    $('#CopiesForm').find(':submit').removeClass('d-none');  



    prepareInput();
}

function prepareInput() {
    selectedCopies = [];
    var copies = $('.js-copy');
    $.each(copies, function (i, input) {
 
        var input = $(input)

        selectedCopies.push({ sereial: input.val(), bookId: input.data('book-id') });


        input.attr('name', `SelectedCopies[${i}]`).attr('id', `SelectedCopies_${i}_`)

    });


}

$(document).ready(function () {

    

    if ($('.js-copy').length > 0) {
        isEditMode = true;
        prepareInput();
        currentCopies = selectedCopies;

    }

    $('.js-search').click(function (e) {



        // preventDefault is function prevnt element'default (this our case : it prevent submit butn of submit again)
       

        e.preventDefault();
        var serial = $('#Value').val();
        if (selectedCopies.find(c => c.sereial == serial)) {


            showErrorMessage("You cannot add the same copy");
            return;
        }
        if (selectedCopies.length >= maxAllowedCopies) {

            showErrorMessage(`You cannot add more than ${maxAllowedCopies} book(s)`);
            return;


        }

        $('#SearchForm').submit();

    });
    $('body').delegate('.js-remove', 'click', function () {

        var btn = $(this);
        var container = btn.parents('.js-copy-container');

        if (isEditMode) {

            btn.toggleClass('btn-light-danger btn-light-warning js-remove js-readd ').text('Re-Add');
            container.find('img').css('opacity', '0.3');
            container.find('h4').css('text-decoration', 'line-through');
            container.find('.js-copy').toggleClass('js-copy js-removed').removeAttr('name').removeAttr('id');
                 
        }
        else {

            $(this).parents('.js-copy-container').remove()

        }

        prepareInput();
        if ($.isEmptyObject(selectedCopies) || JSON.stringify(currentCopies) == JSON.stringify(selectedCopies))
            $('#CopiesForm').find(':submit').addClass('d-none');
        else
            $('#CopiesForm').find(':submit').removeClass('d-none');



    });





    $('body').delegate('.js-readd', 'click', function () {

        var btn = $(this);
        var container = btn.parents('.js-copy-container');

     

            btn.toggleClass('btn-light-danger btn-light-warning js-remove js-readd ').text('Remove');
            container.find('img').css('opacity', '1');
            container.find('h4').css('text-decoration', 'none');
        container.find('.js-removed').toggleClass('js-copy js-removed');

        

        

        prepareInput();

        if (JSON.stringify(currentCopies) == JSON.stringify(selectedCopies))
            $('#CopiesForm').find(':submit').addClass('d-none');
           else
           $('#CopiesForm').find(':submit').removeClass('d-none');



    });

});
