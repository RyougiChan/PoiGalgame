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
                        value: 0,
                        min: -100,
                        max: 100
                    },
                    {
                        name: 'Angry',
                        display_name: 'Angry',
                        value: 0,
                        min: -100,
                        max: 100
                    },
                    {
                        name: 'Hate',
                        display_name: 'Hate',
                        value: 0,
                        min: -100,
                        max: 100
                    },
                ]
            },
            {
                group_name: "Skill",
                group_items: [
                    {
                        name: 'Swordcraft',
                        display_name: 'Swordcraft',
                        value: 0,
                        min: -100,
                        max: 100
                    },
                    {
                        name: 'Mount',
                        display_name: 'Mount',
                        value: 0,
                        min: -100,
                        max: 100
                    },
                    {
                        name: 'Wrestling',
                        display_name: 'Wrestling',
                        value: 0,
                        min: -100,
                        max: 100
                    }
                ]
            }
        ],
        is_actived: false
    };
    
})();