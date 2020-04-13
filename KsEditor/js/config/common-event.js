(function () {
    'use strict';

    if (!window.KsCommonEvent) KsCommonEvent = window.KsCommonEvent = new Map();

    KsCommonEvent.set(10000, 'Character xxx death');
    KsCommonEvent.set(10001, 'The enemy withdrew');
    KsCommonEvent.set(10002, 'Southern flood ');

})();