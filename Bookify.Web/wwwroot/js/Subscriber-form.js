$(document).ready(function () {

    $("#GovernorateId").on("change", function () {


        var areaList = $('#AreaId');
        areaList.empty();
        areaList.append('<option></option>')

        var governorateId = $(this).val();
 
        if (governorateId != '') {

            $.ajax({
                url: '/Subscribers/GetAreas?gevernorateId=' + governorateId,
                type: 'Get',
               
               
                success: function (areas) {

                    $.each(areas, function (index, area) {

                        areaList.append('<option value="' + area.value + '">' + area.text + '</option>');

                        ///  or use : 
                        // var item = $('<option></option>').attr("value", area.value).text(area.text);
                        //areaList.append(item);
                    });
                },
                error: function () {
                    alert('Error fetching dropdown data');

                }
            });

        }
    })

})