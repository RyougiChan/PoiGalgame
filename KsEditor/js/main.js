(() => {
    'use strict';

    // prohibite window reflesh with F5
    $(window).keydown(function (event) {
        switch (event.keyCode) {
            case 116:
                event.keyCode = 0;
                event.cancelBubble = true;
                return false;
        }
    });

    // prohibite window reflesh with contextmenu
    $(document).contextmenu(function(evt) {
        return false;
    });

    let y_area_scale = 1;
    $('#y-area-operation').on('wheel', (evt) => {
        y_area_scale += event.deltaY * -0.001;
        y_area_scale = Math.min(Math.max(.125, y_area_scale), 4);
        $('#y-area-scaleable').css('transform', `scale(${y_area_scale})`);
        $('#y-area-zoom-v > input').val((Math.floor(y_area_scale * 100)));
    });

    $('#y-area-zoom-v > input').on('change', (evt) => {
        let v = parseInt($(evt.target).val());
        if (!isNaN(v)) {
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
    $('#y-button-export').on('click', function (evt) {
        /**
         * @todo Obtain KS content here
         */
        let ks_content = 'TEST KS Content';
        if (ks_content) {
            let file_name = 'Chapter-' + (new Date().getTime()) + '.ks',
                f = new File([ks_content], file_name, { type: 'text/plain;charset=utf-8' }),
                object_url = URL.createObjectURL(f),
                click = function (node) {
                    let event = new MouseEvent('click');
                    node.dispatchEvent(event);
                },
                save_link = document.createElementNS('http://www.w3.org/1999/xhtml', 'a');
            setTimeout(function () {
                save_link.href = object_url;
                save_link.download = f.name;
                click(save_link);
            });
        }
    });

    $('.ks-widget').draggable({
        // containment: "#y-area-draggable", 
        scroll: false,
        start: function (event, ui) {
            // ui.position.left = 0;
            // ui.position.top = 0;
        },
        drag: function (event, ui) {

            let change_left = ui.position.left - ui.originalPosition.left;
            let new_left = ui.originalPosition.left + change_left / ((y_area_scale));

            let change_top = ui.position.top - ui.originalPosition.top;
            let new_top = ui.originalPosition.top + change_top / y_area_scale;

            ui.position.left = new_left;
            ui.position.top = new_top;

        }
    });
    $('.add-ks-line').on('click', function (evt) {
        let $ks_action = $(evt.target).parent().parent().parent();
        $ks_action.append(KsConstant.KS_LINE_TEMPLATE);
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