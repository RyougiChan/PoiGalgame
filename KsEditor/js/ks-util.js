(function () {
    'use strict';

    if (!window.KsUtil) KsUtil = window.KsUtil = {
        /**
         * reflesh all widget on container
         * 
         * @param {*} container 
         */
        refreshAction(container) {
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
                        $hex_color.css('color', c);
                    });
                }
            } else {
                let $ks_actions = $(container).find('.ks-action');

                for (let i = 0; i < $ks_actions.length; i++) {
                    this.refreshAction($ks_actions[i]);
                    this.addEndpoints($ks_actions[i].id);
                }
            }

            this.rebuildConnection(GV.jsplumb_connect_uuids);
            jsPlumb.repaintEverything();
        },
        /**
         * rebuild action connection via given uuids map
         * 
         * @param {*} jsplumb_connect_uuids 
         */
        rebuildConnection(jsplumb_connect_uuids) {
            if (jsplumb_connect_uuids.size) {
                jsplumb_connect_uuids.forEach(function (v, k) {
                    jsPlumb.connect({ uuids: [v[0], v[1]] });
                });
            }
        },
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
        getFileName(fullpath) {
            let spilt_char = '/';
            if(navigator.platform.toUpperCase().indexOf('WIN') > -1) {
                spilt_char = '\\';
            } 

            let fullname = fullpath.slice(fullpath.lastIndexOf(spilt_char) + 1);
                return fullname.substring(0, fullname.lastIndexOf('.'));
        },
        getFullFileName(fullpath) {
            let spilt_char = '/';
            if(navigator.platform.toUpperCase().indexOf('WIN') > -1) {
                spilt_char = '\\';
            } 

            return fullpath.slice(fullpath.lastIndexOf(spilt_char) + 1);
        },
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