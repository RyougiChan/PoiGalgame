(() => {
    'use strict';

    const KS_LINE_TEMPLATE = `
    <div class="ks-line">
        <input type="text" class="ks-input" placeholder="Line Text"><span class="ks-remove">×</span>
    </div>
    `;

    let y_area_scale = 1;
    $('#y-area-operation').on('wheel', (evt) => {
        y_area_scale += event.deltaY * -0.001;
        y_area_scale = Math.min(Math.max(.125, y_area_scale), 4);
        $('#y-area-scaleable').css('transform', `scale(${y_area_scale})`);
        $('#y-area-zoom-v > input').val((Math.floor(y_area_scale * 100)));
    });

    $('#y-area-zoom-v > input').on('change', (evt) => {
        let v = parseInt($(evt.target).val());
        if(!isNaN(v)) {
            y_area_scale = v / 100;
            $('#y-area-scaleable').css('transform', `scale(${y_area_scale})`);
        }
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
        // containment: "#y-area-draggable", 
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
    $('.add-ks-line').on('click', function(evt) {
        let $ks_action = $(evt.target).parent().parent().parent();
        $ks_action.append(KS_LINE_TEMPLATE);
    });

    let KsCode = (() => {
        return {
            add: {
                action() {
                
                },
                line() {
    
                },
            },
            update: {
                action() {
                
                },
                line() {
    
                },
            },
            remove: {
                action() {
                
                },
                line() {
    
                },
            }
        }
    })();

})();