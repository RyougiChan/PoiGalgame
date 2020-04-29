(function () {
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
            Overlays: [['Arrow', { width: 12, length: 12, location: 1.0 }]]
        });

        // jsPlumb.bind('click', function (conn, originalEvent) {
        //     if(confirm('Detach this connection?')) {
        //         jsPlumb.deleteConnection(conn);
        //     }
        // });

        jsPlumb.bind("connection", function (connInfo, originalEvent) {
            if (connInfo.connection.sourceId == connInfo.connection.targetId) {
                jsPlumb.deleteConnection(connInfo.connection);
                alert("Cannot connect self");
            }

            let uuids = connInfo.connection.getUuids();
            GV.jsplumb_connect_uuids.set(JSON.stringify(uuids), uuids);

            let source_id = `#${connInfo.sourceId}`,
                target_id = `#${connInfo.targetId}`;

            $(source_id).attr('data-next-action-id', $(target_id).attr('id').slice($(target_id).attr('id').lastIndexOf('-') + 1));
            $(target_id).attr('data-previous-action-id', $(source_id).attr('id').slice($(source_id).attr('id').lastIndexOf('-') + 1));
            $('#y-area-draggable > input[name=jsplumb-connect-uuids]').val(
                JSON.stringify([...GV.jsplumb_connect_uuids])
            );
            // KsCode.updateAction(source_id);
            KsUtil.refreshAction(source_id);
            KsUtil.refreshAction(target_id);
        });

        jsPlumb.bind("connectionDetached", function (conn, originalEvent) {
            let uuids = conn.connection.getUuids();
            GV.jsplumb_connect_uuids.delete(JSON.stringify(uuids));

            $(`#${conn.sourceId}`).removeAttr('data-next-action-id');
            $(`#${conn.targetId}`).removeAttr('data-previous-action-id');
            $('#y-area-draggable > input[name=jsplumb-connect-uuids]').val(
                JSON.stringify([...GV.jsplumb_connect_uuids])
            );
            KsCode.updateAction(`#${conn.sourceId}`);
            KsCode.updateAction(`#${conn.targetId}`);
            KsUtil.refreshAction(`#${conn.sourceId}`);
            KsUtil.refreshAction(`#${conn.targetId}`);
        });

        $('#y-area-operation').on('wheel', (evt) => {
            GV.y_area_scale += event.deltaY * -0.001;
            GV.y_area_scale = Math.min(Math.max(.125, GV.y_area_scale), 4);
            $('#y-area-scaleable').css('transform', `scale(${GV.y_area_scale})`);
            $('#y-area-zoom-v > input[name=scale-value]').val((Math.floor(GV.y_area_scale * 100)));
            $('#y-area-zoom-v > input[name=scale-value]').attr('value', Math.floor(GV.y_area_scale * 100));
            jsPlumb.setZoom(GV.y_area_scale);
        });

        $('#y-area-draggable').droppable({
            drop: function (evt, ui) {
                let $dragged = $(ui.draggable[0]),
                    widget_name = $dragged.data('widget');

                if (widget_mapper.has(widget_name)) {
                    $('#y-area-scaleable').append(widget_mapper.get(widget_name));
                    // Append code
                    // KsCode.updateAction($('.ks-action').last());
                    // Active event listener
                    KsUtil.refreshAction($('.ks-action').last());
                    // GV.Observer.observe ($('.ks-action').last()[0], GV.obsConfig);
                    // KsUtil.addEndpoints($('.ks-action').last().attr('id'));
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
                    // Get scale
                    GV.y_area_scale = parseInt($('#y-area-draggable').find('input[name=scale-value]').val()) / 100;
                    jsPlumb.setZoom(GV.y_area_scale);

                    KsRecorder = new Map(JSON.parse($('#y-area-draggable > input[name="ks-recorder"]').val()));

                    // Get connection info
                    let save_uuids = $('#y-area-draggable > input[name=jsplumb-connect-uuids]').val();
                    if (save_uuids) {
                        let save_uuids_list = JSON.parse(save_uuids);

                        GV.jsplumb_connect_uuids = new Map(save_uuids_list);
                        // KsRecorder.set('max_jsplumb_uuid', Math.max.apply(null, [...GV.jsplumb_connect_uuids.values()].flat()));
                    }

                    $('#y-area-codetext').html('');
                    KsUtil.refreshAction($('#y-area-draggable')[0]);
                    KsUtil.rebuildConnection(GV.jsplumb_connect_uuids);
                    jsPlumb.repaintEverything();
                };
                reader.readAsText(file);
            }
        });

    });

    window.onbeforeunload = function (e) {
        let dialogText = 'Please make sure that you hava exported the script!';
        e.returnValue = dialogText;
        return dialogText;
    };

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
            let new_left = ui.originalPosition.left + change_left / ((GV.y_area_scale));

            let change_top = ui.position.top - ui.originalPosition.top;
            let new_top = ui.originalPosition.top + change_top / GV.y_area_scale;

            ui.position.left = new_left;
            ui.position.top = new_top;
            jsPlumb.repaintEverything();

        },
        stack: '#y-area-scaleable .ks-widget'
    });

    $('#y-area-draggable').contextmenu(function (evt) {
        return false;
    });

    $('#y-area-draggable').on('change', '#y-area-zoom-v > input', (evt) => {
        let v = parseInt($(evt.target).val());

        if (!isNaN(v)) {
            GV.y_area_scale = v / 100;
            $('#y-area-scaleable').css('transform', `scale(${GV.y_area_scale})`);
        }
    });

    $('#y-button-export').on('click', function (evt) {
        let all_selects = $('#y-area-draggable').find('.ks-select');
        if (all_selects.length) {
            for (let i = 0; i < all_selects.length; i++) {
                let $select = $(all_selects[i]),
                    $selected_option = $select.find(`option[value='${$select.val()}']`) || $select.find(`option:contains('${$select.val()}')`);

                $select.find('option').removeAttr('selected');
                $selected_option.attr('selected', 'selected');
            }
        }

        $('#y-area-draggable > input[name="ks-recorder"]').attr('value', JSON.stringify([...KsRecorder]));
        // let ks_content = $('#y-area-codetext').text().replace(/\s{2,}/g, ' ').replace(/\s+\[/g, '[').replace(/\]/g, ']\n');
        let ks_content = $('#y-area-codetext').text().replace(/\n\s+\n/g, '\n').replace(/ {16}/g, '    ');
        if (ks_content) {
            let file_name = 'Chapter-' + (new Date().getTime()) + '.ks';

            KsUtil.download('[chs]' + ks_content.trimEnd() + '\n[che]\n', file_name);
        }
        let $sn = $($('#y-area-draggable').prop('outerHTML'));
        $sn.find('.jtk-endpoint').remove();
        $sn.find('.ui-selectmenu-button').remove();
        $sn.find('.ui-accordion-header-icon').remove();
        $sn.find('.ks-accordion h3').removeAttr('class');
        $sn.find('svg').remove();
        let ks_html_content = $sn.html();
        if (ks_html_content) {
            let file_name = 'Chapter-' + (new Date().getTime()) + '.visual.ks';
            KsUtil.download(ks_html_content, file_name);
        }
    });

    $('#main-container').on('click', '.add-ks-line', function (evt) {
        let $ks_action = $(evt.target).parent().parent().parent();
        $ks_action.append(KsConstant.builder.get_KS_LINE_TEMPLATE());
        $ks_action.find('.ks-select').selectmenu();
        jsPlumb.repaintEverything();
    });

    tl.pg.init({
        pg_caption: 'GUIDE'
    });
})();