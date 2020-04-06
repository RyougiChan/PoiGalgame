(() => {
    'use strict';

    $(".y-widget" ).parent().draggable({ opacity: 0.7, helper: "clone" });

    $('.color-picker').farbtastic(function (c) {
        console.log(c);
        let $target_con = $(this.wheel).parent().parent().parent().parent();
        let $hex_color = $($($target_con.children().get(1)).children().get(0));
        $hex_color.val(c);
        $hex_color.css('color', c);
    });

    $('.ks-color-picker').on('click', function(evt) {
        let color_picker = $(evt.target).children().get(0);
        if(!color_picker) return;
        if($(color_picker).css('display') === 'block') {
            $(color_picker).fadeOut(100);
        } else {
            $(color_picker).fadeIn(100);
        }
    });

    $('.ks-action').delegate('.remove', 'click', function(evt) {
        $(this).parent().remove();
    });
})();