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
                    if(!$(container).next('.jtk-endpoint').length || 
                        (2 * $all_actions.length > $('#y-area-scaleable').find('.jtk-endpoint').length)) {
                        this.addEndpoints($(container)[0].id);
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
    };
    
})();