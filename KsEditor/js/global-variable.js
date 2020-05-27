(function () {
    'use strict';

    if (!window.GV) GV = window.GV = {};

    Object.defineProperties(GV, {
        Observer: {
            writable: true
        },
        obsConfig: {
            value: {
                childList: true, characterData: true, attributes: true, subtree: true
            }
        },
        jsplumb_connect_uuids: {
            value: new Map(),
            writable: true
        },
        jsPlumbConfig: {
            value: {
                isSource: true,
                isTarget: true,
                maxConnections: 1,
                endpointStyle: { outlineStroke: '#007fff', outlineWidth: 5, radius: 5 },
                overlays: [['Arrow', { width: 12, length: 12, location: 0.5 }]]
            },
            writable: true
        },
        y_area_scale : {
            value: 1,
            writable: true
        },
        adjuster_id : {
            value: 0,
            writable: true
        },
        judge_id : {
            value: 0,
            writable: true
        },
        group_id : {
            value: 0,
            writable: true
        },
        is_removing_action: {
            value: false,
            writable: true
        },
        start_action_id: {
            value: '',
            writable: true
        }
    });

})();