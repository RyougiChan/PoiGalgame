(function () {
    'use strict';

    if (!window.KsPlayerStatus) KsPlayerStatus = window.KsPlayerStatus = new Map();

    KsPlayerStatus.set('HP', 'Health Point');
    KsPlayerStatus.set('MP', 'Mana Point');
    KsPlayerStatus.set('SP', 'SKill Point'); 

})();