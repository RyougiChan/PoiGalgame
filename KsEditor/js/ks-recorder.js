(function () {
    'use strict';

    if (!window.KsRecorder) KsRecorder = window.KsRecorder = new Map();

    KsRecorder.set('max_line_id', 0);
    KsRecorder.set('max_action_id', 0);
    KsRecorder.set('max_judge_id', 0);
    KsRecorder.set('max_adjuster_id', 0);
    KsRecorder.set('max_selector_id', 0);
    KsRecorder.set('max_events_id', 0);
    KsRecorder.set('max_group_id', 0);
    KsRecorder.set('max_common_event_id', 0);
    KsRecorder.set('max_jsplumb_uuid', 1000);

})();