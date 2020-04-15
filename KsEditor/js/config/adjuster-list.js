(function () {
    'use strict';

    if (!window.KsAdjusterList) KsAdjusterList = window.KsAdjusterList = {
        groups: [
            {
                group_name: 'Mood',
                group_items: [
                    {
                        name: 'Sorrow',
                        display_name: 'Sorrow',
                        value: 50,
                        min: 0,
                        max: 100
                    },
                    {
                        name: 'Angry',
                        display_name: 'Angry',
                        value: 50,
                        min: 0,
                        max: 100
                    },
                    {
                        name: 'Hate',
                        display_name: 'Hate',
                        value: 50,
                        min: 0,
                        max: 100
                    },
                ]
            }
        ],
        is_actived: false
    };
    
})();