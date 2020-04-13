(function () {
    'use strict';

    if (!window.KsRecorder) KsRecorder = window.KsRecorder = new Map();

    KsRecorder.set('max_line_id', 0);
    KsRecorder.set('max_action_id', 4);
    KsRecorder.set('max_common_event_id', 0);

})();