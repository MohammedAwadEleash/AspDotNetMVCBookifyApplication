
$(document).ready(function () {
    $('.js-renew').click(function(){

        var btn = $(this);
        var subscriberkey = btn.data('key');
        
        bootbox.confirm({
            message: "Are you sure that you need to renew this subscription?",
            buttons: {
                confirm: {
                    label: 'Yes',
                    className: 'btn-success'
                },
                cancel: {
                    label: 'No',
                    className: 'btn-secondary'
                }
            },
            callback: function (result) {
                if (result) {
                    $.post({
                        url: '/Subscribers/RenewalSubscription?sKey=' + subscriberkey ,
                        data: {
                            '__RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                        },
                        success: function (row) {

                            $('#SubscriptionsTable').find('tbody').append(row);


                            var activeIcon = $('#ActiveStatusIcon');
                            activeIcon.removeClass('d-none');
                            activeIcon.siblings().remove();
                            activeIcon.parents('.card').removeClass('bg-warning').addClass('bg-success');
                            $("#CardStatus").text("Active Subsciber");
                            $("#StatusBadge").removeClass('badge-light-warning').addClass('badge-light-success').text("Active Subsciber");

                            $('#RentalButton').removeClass('d-none');




                            showSuccessMessage();
                        },
                        error: function () {
                            showErrorMessage();
                        }
                    });
                }
            }
        });

    });





    $('.js-cancel-rental').click(function () {

        var btn = $(this);

        bootbox.confirm({
            message: "Are you sure that you need to cancel this rental?",
            buttons: {
                confirm: {
                    label: 'Yes',
                    className: 'btn-success'
                },
                cancel: {
                    label: 'No',
                    className: 'btn-secondary'
                }
            },
            callback: function (result) {
                if (result) {
                    $.post({
                        url: `/Rentals/MarkAsDeleted/${btn.data('id')}` ,
                        data: {
                            '__RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                        },
                        success: function (numOfCopies) {

                            btn.parents('tr').remove();

                            if ($('#RentalsTable tbody tr').length === 0) {

              
                                    $('#RentalsTable').animate({ height: "toggle", opacity: "toggle" }, 3000, function () { 
                                            $('#Alert').animate({ height: "toggle", opacity: "toggle" }, 3000); 
                                        }); 
                            }



                        

                            var currentCount = parseFloat($('#TotalCopies').text())
                            var totalCopies = currentCount - numOfCopies;
                            $('#TotalCopies').text(totalCopies);


                        },
                        error: function () {
                            showErrorMessage();
                        }
                    });
                }
            }
        });

    });


});



