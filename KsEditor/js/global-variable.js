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
        }
    });

})();