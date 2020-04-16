(() => {
    'use strict';

    let widget_mapper = new Map();
    widget_mapper.set('ks-action', KsConstant.builder.get_COMMON_ACTION_NODE);
    widget_mapper.set('ks-selector', KsConstant.builder.get_SELECTOR_ACTION_NODE);
    widget_mapper.set('ks-judge', KsConstant.builder.get_JUDGE_ACTION_NODE);
    widget_mapper.set('ks-adjuster', KsConstant.builder.get_ADJUSTER_ACTION_NODE);
    widget_mapper.set('ks-bgm', KsConstant.builder.get_BGM_ACTION_NODE);
    widget_mapper.set('ks-bfg', KsConstant.builder.get_BFG_ACTION_NODE);
    widget_mapper.set('ks-line', KsConstant.builder.get_LINE_ACTION_NODE);
    widget_mapper.set('ks-video', KsConstant.builder.get_VIDEO_ACTION_NODE);

    jsPlumb.ready(function () {
        let jsPlumbConfig = {
            isSource: true,
            isTarget: true,
            endpointStyle: { outlineStroke: '#007fff', outlineWidth: 5, radius: 5 },
            overlays: [['Arrow', { width: 12, length: 12, location: 0.5 }]] 
        };
        let addEndpoints = function (id) {
            jsPlumb.addEndpoint(id, {
                anchor: 'Top'
            }, jsPlumbConfig);
            jsPlumb.addEndpoint(id, {
                anchor: 'Bottom'
            }, jsPlumbConfig);
        };
        // Custom default setting of jsPlumb
        jsPlumb.importDefaults({
            PaintStyle: {
                strokeWidth: 4,
                stroke: 'rgba(0, 127, 255, 0.5)'
            },
            DragOptions: { cursor: "crosshair" },
            Endpoints: [["Dot", { radius: 5 }], ["Dot", { radius: 5 }]],
            EndpointStyles: [{ outlineStroke: "#007fff" }, { outlineStroke: "#007fff" }],

            MaxConnections: 1,
            Connector: ['Bezier'],
            Anchor: ['Left', 'Right'],
            //PaintStyle: { stroke: '#007fff', strokeWidth: 3 },
            Overlays: [['Arrow', { width: 12, length: 12, location: 0.5 }]]
        });

        $('#y-area-draggable').droppable({
            drop: function (evt, ui) {
                let $dragged = $(ui.draggable[0]),
                    widget_name = $dragged.data('widget');
    
                if (widget_mapper.has(widget_name)) {
                    $('#y-area-scaleable').append(widget_mapper.get(widget_name));
                    // Append code
                    KsCode.updateAction($('.ks-action').last());
                    // Active event listener
                    refreshWidget($('.ks-action').last());
                    // GV.Observer.observe ($('.ks-action').last()[0], GV.obsConfig);
                    addEndpoints($('.ks-action').last().attr('id'));
                    // jsPlumb.draggable($('.ks-action').last().attr('id'))
                }
            }
        });
    });

    let drag_move = {};
    $('#y-area-draggable').on('mousedown', function (evt) {
        switch (evt.which) {
            case 3:
                // right button
                drag_move = {
                    startX: parseInt($('#y-area-scaleable').css('left')),
                    startY: parseInt($('#y-area-scaleable').css('top')),
                    pageX: evt.pageX,
                    pageY: evt.pageY
                };
                let down_time = new Date().getTime();

                $('#y-area-draggable').on('mousemove', function (move_evt) {
                    switch (move_evt.which) {
                        case 3:
                            let move_time = new Date().getTime();
                            if (move_time - down_time > 100) {
                                down_time = new Date().getTime();
                                // right button
                                drag_move.total_move_x = move_evt.pageX - drag_move.pageX;
                                drag_move.total_move_y = move_evt.pageY - drag_move.pageY;

                                let tmp_moving_X = drag_move.startX + drag_move.total_move_x * 1;
                                let tmp_moving_Y = drag_move.startY + drag_move.total_move_y * 1;
                                $('#y-area-scaleable').css('left', tmp_moving_X + 'px');
                                $('#y-area-scaleable').css('top', tmp_moving_Y + 'px');
                            }
                            break;
                    }
                });
                break;
        }
    });

    // $('#y-area-scaleable').draggable({
    //     grid: [ 50, 50 ]
    // });
    
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
            jsPlumb.repaintEverything();

        },
        stack: '#y-area-scaleable .ks-widget'
    });

    $('#y-area-draggable').contextmenu(function (evt) {
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

    $('#y-button-export').on('click', function (evt) {
        /**
         * @todo Obtain KS content here
         */
        let ks_content = $('#y-area-codetext').text().replace(/\s{2,}/g, ' ').replace(/\s+\[/g, '[').replace(/\]/g, ']\n');
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

    $('#main-container').on('click', '.add-ks-line', function (evt) {
        let $ks_action = $(evt.target).parent().parent().parent();
        $ks_action.append(KsConstant.builder.get_KS_LINE_TEMPLATE());
        $ks_action.find('.ks-select').selectmenu();
    });

    /**
     * reflesh all widget on container
     * 
     * @param {*} container 
     */
    let refreshWidget = function (container) {
        if (!container) return;

        let $all_actions = $('#y-area-draggable .ks-action'),
            max_index = 0;
        for (let i = 0; i < $all_actions.length; i++) {
            let t_index = parseInt($($all_actions[i]).css('z-index'));
            max_index = t_index > max_index ? t_index : max_index;
        }

        $(container).css('z-index', max_index + 1);

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

                jsPlumb.repaintEverything();
            },
            stack: '#y-area-scaleable .ks-widget'
        });

        if ($(container).find('.color-picker').length) {
            $(container).find('.color-picker').farbtastic(function (c) {
                let $target_con = $(this.wheel).parent().parent().parent().parent();
                let $hex_color = $($($target_con.children().get(1)).children().get(0));
                $hex_color.val(c);
                $hex_color.css('color', c);
            });
        }
    };



})();