(() => {
    'use strict';
    /*
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
    */

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
                widget_name = $dragged.data('widget'),
                mapper = new Map();
            mapper.set('ks-action', KsConstant.builder.get_COMMON_ACTION_NODE);
            mapper.set('ks-selector', KsConstant.builder.get_SELECTOR_ACTION_NODE);
            mapper.set('ks-judge', KsConstant.builder.get_JUDGE_ACTION_NODE);
            mapper.set('ks-adjuster', KsConstant.builder.get_ADJUSTER_ACTION_NODE);
            mapper.set('ks-bgm', KsConstant.builder.get_BGM_ACTION_NODE);
            mapper.set('ks-bfg', KsConstant.builder.get_BFG_ACTION_NODE);
            mapper.set('ks-line', KsConstant.builder.get_LINE_ACTION_NODE);
            mapper.set('ks-video', KsConstant.builder.get_LINE_ACTION_NODE);
            console.log(widget_name)
            if(mapper.has(widget_name)) {
                $('#y-area-scaleable').append(mapper.get(widget_name));
                    refreshWidget($('.ks-action').last());
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

        },
        stack: '#y-area-scaleable .ks-widget'
    });

    $('#main-container').delegate('.add-ks-line', 'click', function (evt) {
        let $ks_action = $(evt.target).parent().parent().parent();
        let line_id = KsRecorder.get('max_line_id') + 1;
        KsRecorder.set('max_line_id', line_id);
        $ks_action.append(KsConstant.KS_LINE_TEMPLATE
            .replace(/\[\[line_id\]\]/g, line_id));
        $ks_action.find('.ks-select').selectmenu();
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

    /**
     * reflesh all widget on container
     * 
     * @param {*} container 
     */
    let refreshWidget = function (container) {
        if (!container) return;

        $(container).find('.ks-select').selectmenu();
        $(container).find('.ks-accordion').accordion();

        $(container).draggable({
            scroll: false,
            drag: function (event, ui) {

                let change_left = ui.position.left - ui.originalPosition.left;
                let new_left = ui.originalPosition.left + change_left / ((y_area_scale));

                let change_top = ui.position.top - ui.originalPosition.top;
                let new_top = ui.originalPosition.top + change_top / y_area_scale;

                ui.position.left = new_left;
                ui.position.top = new_top;

            },
            stack: '#y-area-scaleable .ks-widget'
        });

        if($(container).find('.color-picker').length) {
            $(container).find('.color-picker').farbtastic(function (c) {
                let $target_con = $(this.wheel).parent().parent().parent().parent();
                let $hex_color = $($($target_con.children().get(1)).children().get(0));
                $hex_color.val(c);
                $hex_color.css('color', c);
            });
        }
    }

})();