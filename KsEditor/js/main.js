(() => {
    'use strict';

    let y_area_scale = 1;
    $('#y-area-operation').on('wheel', (evt) => {
        y_area_scale += event.deltaY * -0.001;
console.log(y_area_scale)
        // Restrict scale
        y_area_scale = Math.min(Math.max(.125, y_area_scale), 4);

        // Apply scale transform
        $('#y-area-scaleable').css('transform', `scale(${y_area_scale})`);
    });

    $('#y-area-draggable').droppable({
        drop: function (evt, ui) {
            let $dragged = $(ui.draggable[0]),
                widget_name = $dragged.data('widget');
            switch (widget_name) {
                case 'ks-action':
                    break;
            }
        }
    });

    $('.ks-widget').draggable({ 
        containment: "#y-area-draggable", 
        scroll: false,
        start: function(event, ui) {
            // ui.position.left = 0;
            // ui.position.top = 0;
        },
        drag: function(event, ui) {
    
            let changeLeft = ui.position.left - ui.originalPosition.left;
            let newLeft = ui.originalPosition.left + changeLeft / (( y_area_scale));
    
            let changeTop = ui.position.top - ui.originalPosition.top;
            let newTop = ui.originalPosition.top + changeTop / y_area_scale;
    
            ui.position.left = newLeft;
            ui.position.top = newTop;
    
        }
    });

})();