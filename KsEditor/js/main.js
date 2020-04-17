(() => {
    'use strict';

    let y_area_scale = 1;
    let jsplumb_connect_uuids = new Map();
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
            if($(`#${id}`).attr('data-ep-uuids')) {
                // This node has endpoint
                let uuids = JSON.parse($(`#${id}`).attr('data-ep-uuids'));
                jsPlumb.addEndpoint(id, {
                    anchor: 'Top',
                    uuid: uuids[0]
                }, jsPlumbConfig);
                jsPlumb.addEndpoint(id, {
                    anchor: 'Bottom',
                    uuid: uuids[1]
                }, jsPlumbConfig);
            } else {
                let action_ep_uuids = [];
                KsRecorder.set('max_jsplumb_uuid', KsRecorder.get('max_jsplumb_uuid') + 1);
                action_ep_uuids.push(KsRecorder.get('max_jsplumb_uuid'));
                jsPlumb.addEndpoint(id, {
                    anchor: 'Top',
                    uuid: KsRecorder.get('max_jsplumb_uuid')
                }, jsPlumbConfig);
    
                KsRecorder.set('max_jsplumb_uuid', KsRecorder.get('max_jsplumb_uuid') + 1);
                action_ep_uuids.push(KsRecorder.get('max_jsplumb_uuid'));
                jsPlumb.addEndpoint(id, {
                    anchor: 'Bottom',
                    uuid: KsRecorder.get('max_jsplumb_uuid')
                }, jsPlumbConfig);
    
                $(`#${id}`).attr('data-ep-uuids', JSON.stringify(action_ep_uuids));
            }
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
            Connector: ['Flowchart'], // Bezier: 贝塞尔曲线 Flowchart: 具有90度转折点的流程线 StateMachine: 状态机 Straight: 直线
            Anchor: ['Left', 'Right'],
            //PaintStyle: { stroke: '#007fff', strokeWidth: 3 },
            Overlays: [['Arrow', { width: 12, length: 12, location: 0.5 }]]
        });

        jsPlumb.bind("connection", function (connInfo, originalEvent) {
            console.log('connInfo', connInfo)
            if (connInfo.connection.sourceId == connInfo.connection.targetId) {
                jsPlumb.deleteConnection(connInfo.connection);
                alert("cannot connect self");
            }

            let uuids = connInfo.connection.getUuids();
            jsplumb_connect_uuids.set(JSON.stringify(uuids), uuids);

            let source_id = `#${connInfo.sourceId}`,
                target_id = `#${connInfo.targetId}`;

            $(source_id).attr('data-next-action-id', $(target_id).attr('id').slice($(target_id).attr('id').lastIndexOf('-') + 1));
            $('#y-area-draggable > input[name=jsplumb-connect-uuids]').val(
                JSON.stringify([...jsplumb_connect_uuids])
            );
            KsCode.updateAction(source_id);
        });

        jsPlumb.bind("connectionDetached", function (conn, originalEvent) {
            // console.log('conn', conn)
            let uuids = conn.connection.getUuids();
            jsplumb_connect_uuids.delete(JSON.stringify(uuids));

            $(`#${conn.source_id}`).removeAttr('data-next-action-id');
            $('#y-area-draggable > input[name=jsplumb-connect-uuids]').val(
                JSON.stringify([...jsplumb_connect_uuids])
            );
            KsCode.updateAction(`#${conn.source_id}`);
        });

        $('#y-area-operation').on('wheel', (evt) => {
            y_area_scale += event.deltaY * -0.001;
            y_area_scale = Math.min(Math.max(.125, y_area_scale), 4);
            $('#y-area-scaleable').css('transform', `scale(${y_area_scale})`);
            $('#y-area-zoom-v > input[name=scale-value]').val((Math.floor(y_area_scale * 100)));
            $('#y-area-zoom-v > input[name=scale-value]').attr('value', Math.floor(y_area_scale * 100));
            jsPlumb.setZoom(y_area_scale);
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
                    refreshAction($('.ks-action').last());
                    // GV.Observer.observe ($('.ks-action').last()[0], GV.obsConfig);
                    addEndpoints($('.ks-action').last().attr('id'));
                    // jsPlumb.draggable($('.ks-action').last().attr('id'))
                }
            }
        });

        $('#y-button-import input[type=file]').on('change', function (file_evt) {
            let files = file_evt.target.files;
            if (files.length) {
                let file = files[0];
                let reader = new FileReader();
                reader.onload = function () {
                    $('#y-area-draggable').html(this.result);
                    // console.log(this.result);
                    y_area_scale= parseInt($('#y-area-draggable').find('input[name=scale-value]').val()) / 100;
                    jsplumb_connect_uuids = new Map(
                        JSON.parse($('#y-area-draggable > input[name=jsplumb-connect-uuids]').val())
                    );
                    
                    refreshAction($('#y-area-draggable')[0]);
                };
                reader.readAsText(file);
            }
        });

        /**
         * reflesh all widget on container
         * 
         * @param {*} container 
         */
        let refreshAction = function (container) {
            if (!container) return;

            if ($(container)[0].classList.contains('ks-action')) {
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
                    start: function (event, ui) {
                        ui.position.left = 0;
                        ui.position.top = 0;
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

                if ($(container).find('.color-picker').length) {
                    $(container).find('.color-picker').farbtastic(function (c) {
                        let $target_con = $(this.wheel).parent().parent().parent().parent();
                        let $hex_color = $($($target_con.children().get(1)).children().get(0));
                        $hex_color.val(c);
                        $hex_color.css('color', c);
                    });
                }
            } else {
                let $ks_actions = $(container).find('.ks-action');
                
                for(let i = 0; i < $ks_actions.length; i++) {
                    refreshAction($ks_actions[i]);
                    addEndpoints($ks_actions[i].id);
                }
            }

            rebuildConnection(jsplumb_connect_uuids);
            jsPlumb.repaintEverything();
        };

        /**
         * rebuild action connection via given uuids map
         * 
         * @param {*} jsplumb_connect_uuids 
         */
        let rebuildConnection = function(jsplumb_connect_uuids) {
            if(jsplumb_connect_uuids.size) {
                jsplumb_connect_uuids.forEach(function(v, k) {
                    jsPlumb.connect( {uuids:[v[0],v[1]]} );
                });
            }
        };
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
            ui.position.left = 0;
            ui.position.top = 0;
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
            let file_name = 'Chapter-' + (new Date().getTime()) + '.ks';
            download(ks_content, file_name);
        }
        let $sn = $($('#y-area-draggable').prop('outerHTML'));
        $sn.find('.jtk-endpoint').remove();
        $sn.find('svg').remove();
        let ks_html_content = $sn.html();
        if (ks_html_content) {
            let file_name = 'Chapter-' + (new Date().getTime()) + '.visual.ks';
            download(ks_html_content, file_name);
        }
    });

    $('#main-container').on('click', '.add-ks-line', function (evt) {
        let $ks_action = $(evt.target).parent().parent().parent();
        $ks_action.append(KsConstant.builder.get_KS_LINE_TEMPLATE());
        $ks_action.find('.ks-select').selectmenu();
        jsPlumb.repaintEverything();
    });


    let download = function(ks_content, file_name) {
        if (ks_content) {
            let f = new File([ks_content], file_name, { type: 'text/plain;charset=utf-8' }),
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
    };

})();