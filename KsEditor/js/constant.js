(function () {
    'use strict';

    if (!window.KsConstant) KsConstant = window.KsConstant = {};

    Object.defineProperties(KsConstant, {
        KS_LINE_TEMPLATE: {
            value: `
            <div class="ks-line">
                <input type="text" class="ks-input" placeholder="Line Text"><span class="ks-remove">×</span>
            </div>
            `,
            writable: false
        },
        BG_ACCORDION_NODE: {
            value: `
            <div class="ks-action-cont ks-action-grid">
                <div class="ks-action-grid-item ks-bg-control">
                    <div class="ks-button ks-square ks-blue"><span>BG</span><input type="file" name="ks-[[action_id]]_bg" accept="image/*" /></div>
                    <select id="ks-select_[[action_id]]_bglayer" class="ks-select">
                        <option selected="selected">LAYER</option>
                        <option>Foreground3</option>
                        <option>Foreground2</option>
                        <option>Foreground1</option>
                        <option>Ground</option>
                        <option>Background1</option>
                        <option>Background2</option>
                        <option>Background3</option>
                    </select>
                </div>
                <span class="ks-remove">×</span>
            </div>
            `
            ,writable: false
        },
        BGM_ACCORDION_NODE: {
            value: `
                    <div class="ks-action-cont ks-action-grid">
                        <div class="ks-action-grid-item ks-bgm-control">
                            <div class="ks-button ks-square ks-blue"><span>BGM</span><input type="file" name="ks-[[action_id]]_bgm" accept="audio/*" /></div>
                            <select id="ks-select_[[action_id]]_bgmaction" class="ks-select">
                                <option selected="selected">play</option>
                                <option>pause</option>
                                <option>resume</option>
                                <option>stop</option>
                            </select>
                            <input id="ks-[[action_id]]_bgmvolume" class="no-border-input" type="text" value="" placeholder="Volume">
                            <label class="ks-input-label" for="ks-[[action_id]]_bgmloop">loop</label>
                            <input id="ks-[[action_id]]_bgmloop" type="checkbox">
                        </div>
                        <span class="ks-remove">×</span>
                    </div>
                    `,
            writable: false
        },
        VOICE_ACCORDION_NODE: {
            value: `
            <div class="ks-action-cont ks-action-grid">
                <div class="ks-action-grid-item ks-voice-control">
                    <div class="ks-button ks-square ks-blue"><span>VOICE</span><input type="file" name="ks-[[action_id]]_voice" accept="audio/*" /></div>
                    <select id="ks-select_[[action_id]]_voiceaction" class="ks-select">
                        <option selected="selected">play</option>
                        <option>pause</option>
                        <option>resume</option>
                        <option>stop</option>
                    </select>
                    <input id="ks-[[action_id]]_voicevolume" class="no-border-input" type="text" value="" placeholder="Volume">
                    <label class="ks-input-label" for="ks-[[action_id]]_voiceloop">loop</label>
                    <input id="ks-[[action_id]]_voiceloop" type="checkbox">
                </div>
                <span class="ks-remove">×</span>
            </div>
            `,
            writable: false
        },
        LINE_ACCORDION_NODE: {
            value: `
            <div class="ks-line" id="ks-action-[[action_id]]-selector-item-[[item_id]]_line-[[line_id]]">
                <input type="text" class="ks-input" placeholder="Line Text">
                <span class="ks-remove">×</span>
            </div>
            `,
            writable: false
        },
        OR_JUDGE_ITEM_NODE: {
            value:`
            <h3>[[item_id]]
                <span class="ks-accordion-op ks-accordion-remove">×</span>
                <span class="ks-accordion-op ks-accordion-add-andjudge">+</span>
            </h3>
            <div class="ks-accordion-item ks-accordion-andjudge-list">
                <div class="ks-accordion-andjudge-item" id="ks-accordion-add-andjudge-[[item_id]]-item-1">
                    <select class="ks-select">
                        <option selected="selected">Health Point</option>
                        <option>Mana Point</option>
                    </select>
                    =
                    <input type="number" class="ks-input" placeholder="VALUE">
                    <span class="ks-remove">×</span>
                </div>
            </div>
            `,
            writable: false
        },
        AND_JUDGE_ITEM_NODE: {
            value:`
            <div class="ks-accordion-andjudge-item" id="ks-accordion-add-andjudge-[[judge_id]]-item-[[judge_item_id_num]]">
                <select class="ks-select">
                    <option selected="selected">Health Point</option>
                    <option>Mana Point</option>
                </select>
                =
                <input type="number" class="ks-input" placeholder="VALUE">
                <span class="ks-remove">×</span>
            </div>
            `,
            writable: false
        },
        SELECTOR_ITEM_NODE: {
            value: `
            <h3>[[item_id]]
                <span class="ks-accordion-op ks-accordion-remove">×</span>
                <span class="ks-accordion-op ks-accordion-addline">+</span>
                <span class="ks-accordion-op ks-accordion-addbgm">bgm</span>
                <span class="ks-accordion-op ks-accordion-addvoice">voice</span>
                <span class="ks-accordion-op ks-accordion-addbg">bg</span>
            </h3>
            <div class="ks-accordion-item">
                <input type="text" class="ks-input ks-accordion-item_text" placeholder="Item Text">
            </div>
            `,
            writable: false
        },
        JUDGE_EVENT_ITEM_NODE: {
            value: `
            <div class="ks-action-cont ks-action-grid ks-action-event">
                <div class="ks-action-grid-item ks-action-event-item" id="ks-action-[[action_id]]-event-[[item_id]]">
                    <select class="ks-select">
                        <option selected="selected">Character xxx death</option>
                        <option>Trigger event-0001: The enemy withdrew </option>
                    </select>
                </div>
                <span class="ks-remove">×</span>
            </div>
            `,
            writable: false
        }
    });

})();
