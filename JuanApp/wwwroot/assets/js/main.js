$(document).ready(function () {
    $('.addtobasket').click(function (e) {
        e.preventDefault();
        let url = $(this).attr('href');
        fetch(url)
            .then(response => response.text())
            .then(data => {
                $('.minicart-content-box').html(data);
            });
    })
    //$('.addtobasket').click(function (e) {
    //    e.preventDefault();
    //    let url = $(this).attr('href');
    //    fetch(url)
    //        .then(response => response.text())
    //        .then(data => {
    //            $('.vacib').html(data);
    //        });
    //})
})
