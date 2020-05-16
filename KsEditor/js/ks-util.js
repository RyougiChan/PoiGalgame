(function () {
    'use strict';

    if (!window.KsUtil) KsUtil = window.KsUtil = {
        /**
         * reflesh all widget on container
         * 
         * @param {String|Element} container 
         */
        refreshAction(container) {
            if (!container) return;

            if($(container)[0]) {
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
                            let new_left = ui.originalPosition.left + change_left / ((GV.y_area_scale));
    
                            let change_top = ui.position.top - ui.originalPosition.top;
                            let new_top = ui.originalPosition.top + change_top / GV.y_area_scale;
    
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
                            $hex_color.attr('value', c);
                            $hex_color.css('color', c);
                        });
                    }
    
                    KsCode.updateAction(container);

                    if(2 * $all_actions.length > $('#y-area-scaleable').find('.jtk-endpoint').length && !GV.is_removing_action) {
                        KsUtil.addEndpoints($(container)[0].id);
                    }
                    jsPlumb.repaintEverything();
                } else {
                    GV.adjuster_id = 0;
                    GV.judge_id = 0;
                    GV.group_id = 0;
                    let $ks_actions = $(container).find('.ks-action');
                    for (let i = 0; i < $ks_actions.length; i++) {
                        this.refreshAction($ks_actions[i]);
                    }
                }
            }

            // this.rebuildConnection(GV.jsplumb_connect_uuids);
            // jsPlumb.repaintEverything();
        },
        /**
         * rebuild action connection via given uuids map
         * 
         * @param {Map} jsplumb_connect_uuids 
         */
        rebuildConnection(jsplumb_connect_uuids) {
            if (jsplumb_connect_uuids.size) {
                jsplumb_connect_uuids.forEach(function (v, k) {
                    jsPlumb.connect({ uuids: [v[0], v[1]] });
                });
            }
        },
        /**
         * Download File
         * @param {String} ks_content Content of file
         * @param {String} file_name Name of file
         */
        download(ks_content, file_name) {
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
        },
        /**
         * Get name of file without extension
         * 
         * @param {String} fullpath Full path of file
         */
        getFileName(fullpath) {
            let spilt_char = '/';
            if(navigator.platform.toUpperCase().indexOf('WIN') > -1) {
                spilt_char = '\\';
            } 

            let fullname = fullpath.slice(fullpath.lastIndexOf(spilt_char) + 1);
                return fullname.substring(0, fullname.lastIndexOf('.'));
        },
        /**
        * Get name of file with extension
        * 
        * @param {String} fullpath Full path of file
        */
        getFullFileName(fullpath) {
            let spilt_char = '/';
            if(navigator.platform.toUpperCase().indexOf('WIN') > -1) {
                spilt_char = '\\';
            } 

            return fullpath.slice(fullpath.lastIndexOf(spilt_char) + 1);
        },
        /**
         * Add jsPlumb endpoints for element with ID = @param id
         * 
         * @param {string} id ID of the element to attach endpoints
         */
        addEndpoints (id) {
            if ($(`#${id}`).attr('data-ep-uuids')) {
                // This node has endpoint
                let uuids = JSON.parse($(`#${id}`).attr('data-ep-uuids'));
                console.log('Rebuild endpoint: ', uuids);
                jsPlumb.addEndpoint(id, {
                    anchor: 'Top',
                    uuid: uuids[0]
                }, GV.jsPlumbConfig);
                jsPlumb.addEndpoint(id, {
                    anchor: 'Bottom',
                    uuid: uuids[1]
                }, GV.jsPlumbConfig);
            } else {
                let action_ep_uuids = [];
                KsRecorder.set('max_jsplumb_uuid', KsRecorder.get('max_jsplumb_uuid') + 1);
                action_ep_uuids.push(KsRecorder.get('max_jsplumb_uuid'));
                jsPlumb.addEndpoint(id, {
                    anchor: 'Top',
                    uuid: KsRecorder.get('max_jsplumb_uuid')
                }, GV.jsPlumbConfig);

                KsRecorder.set('max_jsplumb_uuid', KsRecorder.get('max_jsplumb_uuid') + 1);
                action_ep_uuids.push(KsRecorder.get('max_jsplumb_uuid'));
                jsPlumb.addEndpoint(id, {
                    anchor: 'Bottom',
                    uuid: KsRecorder.get('max_jsplumb_uuid')
                }, GV.jsPlumbConfig);

                $(`#${id}`).attr('data-ep-uuids', JSON.stringify(action_ep_uuids));
            }
        },
        saveRoundAction($cur_active) {
            //////////// remove jquery-ui components
            let $scaleable_area = $('#y-area-scaleable');
            let all_selects = $('#y-area-scaleable').find('.ks-select');
            if (all_selects.length) {
                for (let i = 0; i < all_selects.length; i++) {
                    let $select = $(all_selects[i]),
                        $selected_option = $select.find(`option[value='${$select.val()}']`) || $select.find(`option:contains('${$select.val()}')`);

                    $select.find('option').removeAttr('selected');
                    $selected_option.attr('selected', 'selected');
                }
            }

            $scaleable_area.children('input[name="ks-recorder"]').attr('value', JSON.stringify([...KsRecorder]));
            let $sn = $scaleable_area;
            let this_round_id = parseInt($cur_active.attr('data-round-id'));
            $sn.find('.jtk-endpoint').remove();
            $sn.find('.ui-selectmenu-button').remove();
            $sn.find('.ui-accordion-header-icon').remove();
            $sn.find('.ks-accordion h3').removeAttr('class');
            $sn.find('svg').remove();
            $sn.find('.ks-action').attr('data-round-id', this_round_id);
            //////////// remove jquery-ui components

            let save_html = $sn.html();
            let connect_uuids = $('#y-area-draggable').children('input[name="jsplumb-connect-uuids"]').attr('value');
            // console.log(connect_uuids)
            $cur_active.children('input[name="data-jsplumb-connect-uuids"]').attr('value', connect_uuids);
            $cur_active.children('input[name="data-scale-value"]').attr('value', $('#y-area-draggable').find('input[name="scale-value"]').attr('value'));
            $cur_active.children('input[name="data-scale-pos"]').attr('value', `[${parseInt($('#y-area-scaleable').css('left'))}, ${parseInt($('#y-area-scaleable').css('top'))}]`);
            $cur_active.children('input[name="data-draggable-data"]').attr('value', save_html);

            
            if(save_html.trim()) {
                let previous_round_id = this_round_id - 1,
                next_round_id = this_round_id + 1,
                this_round_first_action_id = $('#y-area-scaleable').attr('data-round-first-action-id'),
                this_round_last_action_id = $('#y-area-scaleable').attr('data-round-last-action-id'),
                next_round_first_action_id = $(`.ks-round-selection-container .ks-round-selection td[data-round-id=${next_round_id}]`).attr('data-round-first-action-id'),
                next_round_last_action_id = $(`.ks-round-selection-container .ks-round-selection td[data-round-id=${next_round_id}]`).attr('data-round-last-action-id');
                let previous_round_first_action_id;
                let previous_round_last_action_id;
                if(previous_round_id > 0) {
                    previous_round_first_action_id = $(`#ks-round-selection-container .ks-round-selection td[data-round-id=${previous_round_id}]`).attr('data-round-first-action-id');
                    previous_round_last_action_id = $(`#ks-round-selection-container .ks-round-selection td[data-round-id=${previous_round_id}]`).attr('data-round-last-action-id');
                }

                if(this_round_first_action_id && previous_round_last_action_id) {
                    let $saved_previous_round_data = $(`#ks-round-selection-container .ks-round-selection td[data-round-id=${previous_round_id}] input[name="data-draggable-data"]`);
                    let $h = $(`<div>${$saved_previous_round_data.attr('value')}</div>`);
                    $h.find(`#${previous_round_last_action_id}`).attr('data-next-action-id', 
                        this_round_first_action_id);
                    $saved_previous_round_data.attr('value', $h.html());
                    let $tmp = $(`<div>${save_html}</div>`);
                    $tmp.find(`#${this_round_first_action_id}`).attr('data-previous-action-id', 
                        previous_round_last_action_id)
                    $cur_active.children('input[name="data-draggable-data"]').attr('value',
                        $tmp.html());
                }
                if(this_round_last_action_id && next_round_first_action_id) {
                    let $saved_next_round_data = $(`#ks-round-selection-container .ks-round-selection td[data-round-id=${next_round_id}] input[name="data-draggable-data"]`);
                    let $h = $(`<div>${$saved_next_round_data.attr('value')}</div>`);
                    $h.find(`#${next_round_first_action_id}`).attr('data-previous-action-id', this_round_last_action_id);
                    $saved_next_round_data.attr('value', $h.html());
                }
            }

            // IMPORTANT: delete all endpoints and connections
            jsPlumb.deleteEveryEndpoint();
            jsPlumb.deleteEveryConnection();

        },
        loadRoundAction($round_td_item) {
            // IMPORTANT: delete all endpoints and connections
            jsPlumb.deleteEveryEndpoint();
            jsPlumb.deleteEveryConnection();

            $('#y-area-codetext').html('');

            let new_scaleable = $round_td_item.children('input[name="data-draggable-data"]').val();
            $('#y-area-draggable > #y-area-scaleable').html(new_scaleable);
            GV.y_area_scale = parseInt($round_td_item.children('input[name="data-scale-value"]').attr('value')) / 100;
            KsUtil.setZoomAreaScale(GV.y_area_scale);
            KsUtil.setZoomAreaPosition(JSON.parse($round_td_item.children('input[name="data-scale-pos"]').attr('value')));

            if(new_scaleable.trim()) {
                ///////////// Rebuild jquery-ui components

                // Get connection info
                let save_uuids = $round_td_item.children('input[name="data-jsplumb-connect-uuids"]').attr('value');

                $('#y-area-draggable > input[name="jsplumb-connect-uuids"]').attr('value', save_uuids);
                $('#y-area-codetext').html('');
                KsUtil.refreshAction($('#y-area-draggable')[0]);

                if (save_uuids) {
                    let save_uuids_list = JSON.parse(save_uuids);

                    KsUtil.rebuildConnection(new Map(save_uuids_list));
                }

                jsPlumb.repaintEverything();
                ///////////// Rebuild jquery-ui components
            } else {
                $('#y-area-scaleable').removeAttr('data-round-first-action-id');
                $('#y-area-scaleable').removeAttr('data-round-last-action-id');
            }
        },
        getFirstAction() {
            let $all_actions = $('#y-area-draggable .ks-action');
            if($all_actions.length) {
                let connect_uuids_arr = [...GV.jsplumb_connect_uuids.values()].flat();
                if(connect_uuids_arr.length) {
                    let single_uuids = [];
                    for(let i = 0; i < $all_actions.length; i++) {
                        if($($all_actions.get(i)).attr('data-ep-uuids')) {
                            let action_uuids = JSON.parse($($all_actions.get(i)).attr('data-ep-uuids'));
                            if(connect_uuids_arr.indexOf(action_uuids[0]) < 0) single_uuids.push(action_uuids[0]);
                            if(connect_uuids_arr.indexOf(action_uuids[1]) < 0) single_uuids.push(action_uuids[1]);
                        }
                    }
                    for(let i = 0; i < single_uuids.length; i++) {
                        if((single_uuids[i] & 1) == 1) {
                            return $(jsPlumb.getEndpoint(single_uuids[i]).element);
                        }
                    }
                } else {
                    return $all_actions.first();
                }
            }
        },
        getLastAction() {
            let $all_actions = $('#y-area-draggable .ks-action');
            if($all_actions.length) {
                let connect_uuids_arr = [...GV.jsplumb_connect_uuids.values()].flat();
                if(connect_uuids_arr.length) {
                    let connect_uuids_arr = [...GV.jsplumb_connect_uuids.values()].flat();
                    let single_uuids = [];
                    for(let i = 0; i < $all_actions.length; i++) {
                        if($($all_actions.get(i)).attr('data-ep-uuids')) {
                            let action_uuids = JSON.parse($($all_actions.get(i)).attr('data-ep-uuids'));
                            if(connect_uuids_arr.indexOf(action_uuids[0]) < 0) single_uuids.push(action_uuids[0]);
                            if(connect_uuids_arr.indexOf(action_uuids[1]) < 0) single_uuids.push(action_uuids[1]);
                        }
                    }
                    for(let i = 0; i < single_uuids.length; i++) {
                        if((single_uuids[i] & 1) == 0) {
                            return $(jsPlumb.getEndpoint(single_uuids[i]).element);
                        }
                    }
                } else {
                    return $all_actions.first();
                }
            }
        },
        setZoomAreaScale() {
            GV.y_area_scale = Math.min(Math.max(.125, GV.y_area_scale), 4);
            $('#y-area-draggable >#y-area-scaleable').css('transform', `scale(${GV.y_area_scale})`);
            $('#y-area-zoom-v > input[name=scale-value]').val((Math.floor(GV.y_area_scale * 100)));
            $('#y-area-zoom-v > input[name=scale-value]').attr('value', Math.floor(GV.y_area_scale * 100));
            jsPlumb.setZoom(GV.y_area_scale);
        },
        setZoomAreaPosition(pos) {
            if(pos instanceof Array || pos.length > 1) {
                $('#y-area-scaleable').css('left', pos[0]);
                $('#y-area-scaleable').css('top', pos[1]);
            }
        }
    };
    
})();