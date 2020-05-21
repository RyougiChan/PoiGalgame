(function () {
    'use strict';

    if (!window.KsConstant) KsConstant = window.KsConstant = {};

    let built_options_player_status = '';
    let built_options_common_event = '';
    let built_adjuster_group_list = '';
    let built_adjuster_values_list = '';
    let built_actor_list = '';

    KsPlayerStatus.forEach(function (v, k) {
        built_options_player_status += `<option value="${k}">${v}</option>`
    });
    
    KsCommonEvent.forEach(function (v, k) {
        built_options_common_event += `<option value="${k}">${v}</option>`
    });

    KsActor.forEach(function(v){
        built_actor_list += `<option>${v}</option>`;
    });

    built_adjuster_values_list = '<input type="hidden" name="is-adjuster-actived" value="false">';
    KsAdjusterList.groups.forEach(function (group) {
        let group_name = group.group_name;
        let group_items = group.group_items;

        built_adjuster_group_list += `<div class="ks-adjuster-item-group" name=ks-adjuster-group_${group_name}>`;
        group_items.forEach(function (item) {
            built_adjuster_group_list += `
            <div class="ks-adjuster-item" id="ks-adjuster-item_${item.name}">
                <label for="ks-adjuster-item_${item.name}">${item.display_name}:</label>
                <input name="ks-adjuster-item_${item.name}" class="ks-input" type="number" min="${item.min}" max="${item.max}"
                    value="${item.value}" />
                <div class="ks-adjuster-slider"></div>
            </div>
            `;
            built_adjuster_values_list += `<input type="hidden" name="ks-adjuster-item_${item.name}" value="${item.value}">`;
        });
        built_adjuster_group_list += `</div>`;
    });

    let builder = {
        get_KS_LINE_TEMPLATE() {
            KsRecorder.set('max_line_id', KsRecorder.get('max_line_id') + 1);
            let line_id = KsRecorder.get('max_line_id');
            return `
            <div class="ks-line">
                <div class="ks-line-addon ks-voice-control">
                    <select id="ks-line-${line_id}_actor" class="ks-line-actor ks-select" title="Select Actor">
                        ${built_actor_list}
                    </select>
                    <div class="ks-button ks-square ks-blue" title="Select A Voice file"><span>VOICE</span>
                    <input type="file" name="ks-line-${line_id}_voice" class="ks-line-voice_file" accept="audio/*" /></div>
                    <select id="ks-line-${line_id}_voiceaction" class="ks-select ks-line-voice_action" title="Select Action for Voice">
                        <option >play</option>
                        <option>pause</option>
                        <option>resume</option>
                        <option>stop</option>
                    </select>
                    <input id="ks-line-${line_id}_voicevolume" class="no-border-input ks-line-voice_volume" type="number" value=""
                        placeholder="Volume" title="Set Volume of Voice[0~100]">
                    <label class="ks-input-label" for="ks-line-${line_id}_voiceloop"  title="Check If Voice Should Loop">loop</label>
                    <input id="ks-line-${line_id}_voiceloop" class="ks-line-voice_loop" type="checkbox">
                </div>
                <input id="ks-line-${line_id}" type="text" class="ks-input ks-line-text" placeholder="Line Text" title="Input Actor Line"><span class="ks-remove" title="Remove">×</span>
            </div>
            `;
        },
        get_BG_ACCORDION_NODE() {
            return `
            <div class="ks-action-cont ks-action-grid">
                <div class="ks-action-grid-item ks-bg-control">
                    <div class="ks-button ks-square ks-blue"><span>BG</span><input type="file" name="ks-action-[[action_id]]_bg" accept="image/*" /></div>
                    <select id="ks-action-[[action_id]]-select_bglayer" class="ks-select">
                        <option>Foreground3</option>
                        <option>Foreground2</option>
                        <option>Foreground1</option>
                        <option>Ground</option>
                        <option>Background1</option>
                        <option>Background2</option>
                        <option>Background3</option>
                    </select>
                </div>
                <span class="ks-remove" title="Remove">×</span>
            </div>
            `;
        },
        get_JUDGE_EVENT_ITEM_NODE() {
            KsRecorder.set('max_common_event_id', KsRecorder.get('max_common_event_id') + 1);
            let common_event_id = KsRecorder.get('max_common_event_id');
            KsRecorder.set('max_adjuster_id', KsRecorder.get('max_adjuster_id') + 1);
            let adjuster_id = KsRecorder.get('max_adjuster_id');
            return `
            <div class="ks-action-cont ks-action-grid ks-action-event">
                <div class="ks-action-grid-item ks-action-event-item">
                    <select class="ks-select ks-event ks-judge-event" id="ks-action-[[action_id]]-common-event-${common_event_id}">
                        ${built_options_common_event}
                    </select>
                    <div class="ks-adjuster" id="ks-adjuster-${adjuster_id}">
                        <div class="ks-button ks-blue ks-square open-ks-aujuster-config" title="Change Adjuster Config">config</div>
                        <div class="ks-adjuster-editor-values">
                        ${built_adjuster_values_list}
                        </div>
                    </div>
                </div>
                <span class="ks-remove" title="Remove">×</span>
            </div>
            `;
        },
        get_EVENT_ITEM_NODE() {
            KsRecorder.set('max_common_event_id', KsRecorder.get('max_common_event_id') + 1);
            let common_event_id = KsRecorder.get('max_common_event_id');
            KsRecorder.set('max_adjuster_id', KsRecorder.get('max_adjuster_id') + 1);
            let adjuster_id = KsRecorder.get('max_adjuster_id');
            return `
            <div class="ks-action-cont ks-action-grid ks-action-event">
                <div class="ks-action-grid-item ks-action-event-item">
                    <select class="ks-select ks-event" id="ks-action-[[action_id]]-common-event-${common_event_id}">
                        ${built_options_common_event}
                    </select>
                    <div class="ks-adjuster" id="ks-adjuster-${adjuster_id}">
                        <div class="ks-button ks-blue ks-square open-ks-aujuster-config" title="Change Adjuster Config">config</div>
                        <div class="ks-adjuster-editor-values">
                        ${built_adjuster_values_list}
                        </div>
                    </div>
                </div>
                <span class="ks-remove" title="Remove">×</span>
            </div>
            `;
        },
        get_BGM_ACCORDION_NODE() {
            return `
            <div class="ks-action-cont ks-action-grid">
                <div class="ks-action-grid-item ks-bgm-control">
                    <div class="ks-button ks-square ks-blue"><span>BGM</span><input type="file" name="ks-action-[[action_id]]_bgm" accept="audio/*" /></div>
                    <select id="ks-action-[[action_id]]-select_bgmaction" class="ks-select">
                        <option >play</option>
                        <option>pause</option>
                        <option>resume</option>
                        <option>stop</option>
                    </select>
                    <input id="ks-action-[[action_id]]_bgmvolume" class="no-border-input" min="0" max="100" type="number" value="" placeholder="Volume">
                    <label class="ks-input-label" for="ks-action-[[action_id]]_bgmloop">loop</label>
                    <input id="ks-action-[[action_id]]_bgmloop" type="checkbox">
                </div>
                <span class="ks-remove" title="Remove">×</span>
            </div>
            `;
        },
        get_VOICE_ACCORDION_NODE() {
            return `
            <div class="ks-action-cont ks-action-grid">
                <div class="ks-action-grid-item ks-voice-control">
                    <div class="ks-button ks-square ks-blue"><span>VOICE</span><input type="file" name="ks-action-[[action_id]]_voice" accept="audio/*" /></div>
                    <select id="ks-action-[[action_id]]-select_voiceaction" class="ks-select">
                        <option >play</option>
                        <option>pause</option>
                        <option>resume</option>
                        <option>stop</option>
                    </select>
                    <input id="ks-action-[[action_id]]_voicevolume" class="no-border-input" min="0" max="100" type="number" value="" placeholder="Volume">
                    <label class="ks-input-label" for="ks-action-[[action_id]]_voiceloop">loop</label>
                    <input id="ks-action-[[action_id]]_voiceloop" type="checkbox">
                </div>
                <span class="ks-remove" title="Remove">×</span>
            </div>
            `;
        },
        get_ADJUSTER_NODE() {
            KsRecorder.set('max_adjuster_id', KsRecorder.get('max_adjuster_id') + 1);
            let adjuster_id = KsRecorder.get('max_adjuster_id');
            return `
            <div class="ks-action-cont ks-action-grid">
                <div class="ks-action-grid-item ks-adjuster" id="ks-adjuster-${adjuster_id}">
                    <div class="ks-action-grid-item">
                        <div class="ks-button ks-blue ks-square open-ks-aujuster-config" title="Change Adjuster Config">config</div>
                    </div>
                    <div class="ks-adjuster-editor-values">
                    ${built_adjuster_values_list}
                    </div>
                </div>
                <span class="ks-remove" title="Remove">×</span>
            </div>
            `;
        },
        get_AND_JUDGE_ITEM_NODE() {
            return `
            <div class="ks-accordion-andjudge-item">
                <select class="ks-select" title="Select Condition" id="ks-accordion-action-[[action_id]]-add-andjudge-[[judge_id]]-item-[[judge_item_id_num]]" value="${KsPlayerStatus.keys().next().value}">
                    ${built_options_player_status}
                </select>
                =
                <input type="number" class="ks-input" placeholder="VALUE" title="Add Expected Value">
                <span class="ks-remove" title="Remove">×</span>
            </div>
            `;
        },
        get_OR_JUDGE_ITEM_NODE() {
            return `
            <h3>[[item_id]]
                <span class="ks-accordion-op ks-accordion-remove" title="Remove">×</span>
                <span class="ks-accordion-op ks-accordion-add-andjudge" title="Add a logical 'AND' judgment">+</span>
            </h3>
            <div class="ks-accordion-item ks-accordion-andjudge-list" id="ks-group-[[group_id]]">
                <div class="ks-accordion-andjudge-item">
                    <select class="ks-select" title="Select Condition" id="ks-accordion-action-[[action_id]]-add-andjudge-[[item_id]]-item-1" value="${KsPlayerStatus.keys().next().value}">
                        ${built_options_player_status}
                    </select>
                    =
                    <input type="number" class="ks-input" placeholder="VALUE" title="Add Expected Value">
                    <span class="ks-remove" title="Remove">×</span>
                </div>
            </div>
            `;
        },
        get_LINE_ACTOR_LIST_NODE() {
            return `
            <select id="ks-select_action-[[action_id]]-line-[[line_id]]_actor" class="ks-select ks-green" title="Select Actor">
                ${built_actor_list}
            </select>
            `;
        },
        get_ACTION_ACTOR_LIST_NODE() {
            return `
            <select id="ks-select_action-[[action_id]]_actor" class="ks-select ks-green" title="Select Actor">
                ${built_actor_list}
            </select>
            `;
        },
        get_SELECTOR_ITEM_NODE() {
            return `
            <h3>[[item_id]]
            <span class="ks-accordion-op ks-accordion-remove" title="Remove">×</span>
            <span class="ks-accordion-op ks-accordion-addline" title="Add A Sub-Line">+</span>
            <span class="ks-accordion-op ks-accordion-addbgm" title="Add BGM">bgm</span>
            <!--<span class="ks-accordion-op ks-accordion-addvoice" title="Add Voice">voice</span>-->
            <span class="ks-accordion-op ks-accordion-addbg" title="Add A Background Image">bg</span>
            <span class="ks-accordion-op ks-accordion-addadjuster" title="Add Adjuster">adjuster</span>
            </h3>
            <div class="ks-accordion-item">
                <input type="text" id="ks-action-[[action_id]]-selector-item-[[item_id]]_text" class="ks-input ks-accordion-item_text" placeholder="Item Text" title="Input Display Text">
            </div>
            `;
        },
        get_ADJUSTER_GROUP_LIST_NODE() {
            return `
            <div class="ks-adjuster-editor">
                <h3></h3> <label for="ks-adjuster-isactived">ACTIVED</label> <input id="ks-adjuster-isactived" type="checkbox" /> <font color="red" size="0.8rem">Notice: Values here are relative variation but not absolute variation!</font>
                ${built_adjuster_group_list}
            </div>
            `;
        },
        get_ADJUSTER_VALUES_LIST_NODE() {
            return built_adjuster_values_list;
        },
        get_COMMON_EVENT_LIST_NODE() {
            KsRecorder.set('max_common_event_id', KsRecorder.get('max_common_event_id') + 1);
            let common_event_id = KsRecorder.get('max_common_event_id');
            KsRecorder.set('max_adjuster_id', KsRecorder.get('max_adjuster_id') + 1);
            let adjuster_id = KsRecorder.get('max_adjuster_id');
            return `
            <select class="ks-select ks-event ks-judge-event" title="Select Common Event" id="ks-action-[[action_id]]-common-event-${common_event_id}">
                ${built_options_common_event}
            </select>
            <div class="ks-adjuster" id="ks-adjuster-${adjuster_id}">
                <div class="ks-button ks-blue ks-square open-ks-aujuster-config" title="Change Adjuster Config">config</div>
                <div class="ks-adjuster-editor-values">
                ${built_adjuster_values_list}
                </div>
            </div>
            `;
        },
        get_COMMON_ACTION_NODE() {
            KsRecorder.set('max_action_id', KsRecorder.get('max_action_id') + 1);
            let action_id = KsRecorder.get('max_action_id');
            KsRecorder.set('max_adjuster_id', KsRecorder.get('max_adjuster_id') + 1);
            let adjuster_id = KsRecorder.get('max_adjuster_id');
            return `
            <div id="ks-action-${action_id}" class="ks-widget ks-action" style="position: absolute; left: 300px; top: 255px;">
                <span class="ks-remove" title="Remove">×</span>
                <div class="ks-action-cont ks-action-info">
                    <span>ID: ${action_id}</span>/<span>Commond Action</span>
                </div>
                <div class="ks-action-cont ks-action-grid">
                    <div class="ks-action-grid-item">
                        <div class="ks-button ks-color-picker" title="Select Text Color">
                            <div class="color-picker"></div>
                        </div>
                        <div>
                            <input id="ks-select_action-${action_id}_color" class="no-border-input color-hex" type="text"
                                value="#xxxxxx">
                        </div>
                    </div>
                    <div class="ks-action-grid-item">
                        <div class="ks-button add-ks-line" title="Add A Line">+</div>
                    </div>
                    <div class="ks-action-grid-item ks-adjuster" id="ks-adjuster-${adjuster_id}">
                        <div class="ks-button ks-blue ks-square open-ks-aujuster-config" title="Change Adjuster Config">config</div>
                        <div class="ks-adjuster-editor-values">
                            ${built_adjuster_values_list}
                        </div>
                    </div>
                </div>
                <div class="ks-action-cont ks-action-grid">
                    <div class="ks-action-grid-item ks-bgm-control">
                        <div class="ks-button ks-square ks-blue" title="Select A BGM file"><span>BGM</span>
                        <input type="file" name="ks-action-${action_id}_bgm" accept="audio/*" /></div>
                        <select id="ks-select_action-${action_id}_bgmaction" class="ks-select" title="Select Action for BGM">
                            <option >play</option>
                            <option>pause</option>
                            <option>resume</option>
                            <option>stop</option>
                        </select>
                        <input id="ks-action-${action_id}_bgmvolume" class="no-border-input" type="number" value=""
                            placeholder="Volume" title="Set Volume of BGM[0~100]">
                        <label class="ks-input-label" for="ks-action-${action_id}_bgmloop" title="Check If BGM Should Loop">loop</label>
                        <input id="ks-action-${action_id}_bgmloop" type="checkbox">
                    </div>
                </div>
                <div class="ks-action-cont ks-action-grid">
                    <div class="ks-action-grid-item">
                        <select id="ks-select_action-${action_id}_actor" class="ks-action-actor ks-select ks-green" title="Select Actor" style="display: none;">
                        ${built_actor_list}
                        </select>
                    </div>
                    <div class="ks-action-grid-item">
                        <select id="ks-select_action-${action_id}_align" class="ks-select ks-action-textalign" title="Select Text Align Style">
                            <option >LT</option>
                            <option>LM</option>
                            <option>LB</option>
                            <option>MT</option>
                            <option>MM</option>
                            <option>MB</option>
                            <option>RT</option>
                            <option>RM</option>
                            <option>RB</option>
                        </select>
                    </div>
                    <div class="ks-action-grid-item">
                        <select id="ks-select_action-${action_id}_style" class="ks-select ks-action-textstyle" title="Select Text Weight Style">
                            <option >Normal</option>
                            <option>Bold</option>
                            <option>Italic</option>
                            <option>BoldAndItalic</option>
                        </select>
                    </div>
                </div>
                <div class="ks-action-cont ks-action-grid">
                    <div class="ks-action-grid-item ks-action-bg">
                        <div class="ks-button ks-square ks-blue" title="Select A Background Image"><span>BG</span><input type="file"
                                name="ks-action-${action_id}_bg" accept="image/*" /></div>
                        <select id="ks-select_action-${action_id}_bglayer" class="ks-select" title="Select Layer of Background Image">
                            <option>Foreground3</option>
                            <option>Foreground2</option>
                            <option>Foreground1</option>
                            <option>Ground</option>
                            <option >Background1</option>
                            <option>Background2</option>
                            <option>Background3</option>
                        </select>
                    </div>
                    <div class="ks-action-grid-item ks-action-fg">
                        <div class="ks-button ks-square ks-blue" title="Select A Foreground Image"><span>FG</span>
                        <input type="file" name="ks-action-${action_id}_fg" accept="image/*" /></div>
                        <select id="ks-select_action-${action_id}_fglayer" class="ks-select" title="Select Layer of Foreground Image">
                            <option>Foreground3</option>
                            <option>Foreground2</option>
                            <option >Foreground1</option>
                            <option>Ground</option>
                            <option>Background1</option>
                            <option>Background2</option>
                            <option>Background3</option>
                        </select>
                    </div>
                </div>
                <div class="ks-action-cont ks-action-grid">
                    <div class="ks-action-grid-item">
                        <div>
                            <input id="ks-select_action-${action_id}_fontsize" class="no-border-input ks-action-textsize" type="number" value=""
                                placeholder="Size(px)" title="Set Text Size">
                        </div>
                    </div>
                    <div class="ks-action-grid-item">
                        <div>
                            <input id="ks-select_action-${action_id}_linespacing" class="no-border-input ks-action-textlinespacing" type="number"
                                value="" placeholder="Space(px)" title="Set Line Spacing">
                        </div>
                    </div>
                </div>
            </div>
            `;
        },
        get_SELECTOR_ACTION_NODE() {
            KsRecorder.set('max_action_id', KsRecorder.get('max_action_id') + 1);
            let action_id = KsRecorder.get('max_action_id');
            KsRecorder.set('max_selector_id', KsRecorder.get('max_selector_id') + 1);
            let selector_id = KsRecorder.get('max_selector_id');
            return `
            <div id="ks-action-${action_id}" class="ks-widget ks-action" style="position: absolute; left: 300px; top: 255px;">
                <span class="ks-remove" title="Remove">×</span>
                <div class="ks-action-cont ks-action-info">
                    <span>ID: ${action_id}</span>/<span>Selector Action</span>
                </div>
                <div class="ks-selector" id="ks-selector-${selector_id}">
                    <div class="ks-action-cont ks-action-grid">
                        <div class="ks-action-grid-item">
                            <div class="ks-button add-ks-selector-item" title="Add A Selector Item">+</div>
                        </div>
                    </div>
                    <div class="ks-accordion">
                        <h3>1
                            <span class="ks-accordion-op ks-accordion-remove" title="Remove">×</span>
                            <span class="ks-accordion-op ks-accordion-addline" title="Add A Sub-Line">+</span>
                            <span class="ks-accordion-op ks-accordion-addbgm" title="Add BGM">bgm</span>
                            <!--<span class="ks-accordion-op ks-accordion-addvoice" title="Add Voice">voice</span>-->
                            <span class="ks-accordion-op ks-accordion-addbg" title="Add A Background Image">bg</span>
                            <span class="ks-accordion-op ks-accordion-addadjuster" title="Add Adjuster">adjuster</span>
                        </h3>
                        <div class="ks-accordion-item">
                        <input type="text" id="ks-action-${action_id}-selector-item-1_text" class="ks-input ks-accordion-item_text" placeholder="Item Text" title="Input Display Text">
                        </div>
                    </div>
                </div>
            </div>
            `;
        },
        get_EVENTS_ACTION_NODE() {
            KsRecorder.set('max_action_id', KsRecorder.get('max_action_id') + 1);
            KsRecorder.set('max_common_event_id', KsRecorder.get('max_common_event_id') + 1);
            KsRecorder.set('max_adjuster_id', KsRecorder.get('max_adjuster_id') + 1);
            KsRecorder.set('max_events_id', KsRecorder.get('max_events_id') + 1);
            let adjuster_id = KsRecorder.get('max_adjuster_id');
            let common_event_id = KsRecorder.get('max_common_event_id');
            let action_id = KsRecorder.get('max_action_id');
            let events_id = KsRecorder.get('max_events_id');
            return `
            <div id="ks-action-${action_id}" class="ks-widget ks-action"  style="position: absolute; left: 300px; top: 255px;">
                <span class="ks-remove" title="Remove">×</span>
                <div class="ks-action-cont ks-action-info">
                    <span>ID: ${action_id}</span>/<span>Events Action</span>
                </div>
                <div class="ks-events" id="ks-events-${events_id}">
                    <div class="ks-action-cont ks-action-grid">
                        <div class="ks-action-grid-item">
                            <div class="ks-button ks-blue ks-square add-ks-events-item" title="Add Trigger Event">+event</div>
                        </div>
                    </div>
                    <div class="ks-action-cont ks-action-grid ks-action-event">
                        <div class="ks-action-grid-item ks-action-event-item">
                            <select class="ks-select ks-event" title="Select Common Event" id="ks-action-${action_id}-common-event-${common_event_id}">
                                ${built_options_common_event}
                            </select>
                            <div class="ks-adjuster" id="ks-adjuster-${adjuster_id}">
                                <div class="ks-button ks-blue ks-square open-ks-aujuster-config" title="Change Adjuster Config">config</div>
                                <div class="ks-adjuster-editor-values">
                                ${built_adjuster_values_list}
                                </div>
                            </div>
                        </div>
                        <span class="ks-remove" title="Remove">×</span>
                    </div>
                </div>
            </div>
            `;
        },
        get_JUDGE_ACTION_NODE() {
            KsRecorder.set('max_action_id', KsRecorder.get('max_action_id') + 1);
            KsRecorder.set('max_common_event_id', KsRecorder.get('max_common_event_id') + 1);
            let common_event_id = KsRecorder.get('max_common_event_id');
            let action_id = KsRecorder.get('max_action_id');
            KsRecorder.set('max_judge_id', KsRecorder.get('max_judge_id') + 1);
            let judge_id = KsRecorder.get('max_judge_id');
            KsRecorder.set('max_group_id', KsRecorder.get('max_group_id') + 1);
            let group_id = KsRecorder.get('max_group_id');
            KsRecorder.set('max_adjuster_id', KsRecorder.get('max_adjuster_id') + 1);
            let adjuster_id = KsRecorder.get('max_adjuster_id');
            return `
            <div id="ks-action-${action_id}" class="ks-widget ks-action"  style="position: absolute; left: 300px; top: 255px;">
                <span class="ks-remove" title="Remove">×</span>
                <div class="ks-action-cont ks-action-info">
                    <span>ID: ${action_id}</span>/<span>Judge Action</span>
                </div>
                <div class="ks-judge" id="ks-judge-${judge_id}">
                    <div class="ks-action-cont ks-action-grid">
                        <div class="ks-action-grid-item">
                            <div class="ks-button add-ks-orjudge-item" title="Add a logical 'OR' judgment">+</div>
                            <div class="ks-button ks-blue ks-square add-ks-judge-event" title="Add Trigger Event">+event</div>
                        </div>
                    </div>
                    <div class="ks-action-cont ks-action-grid ks-action-event">
                        <div class="ks-action-grid-item ks-action-event-item">
                            <select class="ks-select ks-event ks-judge-event" title="Select Common Event" id="ks-action-${action_id}-common-event-${common_event_id}">
                                ${built_options_common_event}
                            </select>
                            <div class="ks-adjuster" id="ks-adjuster-${adjuster_id}">
                                <div class="ks-button ks-blue ks-square open-ks-aujuster-config" title="Change Adjuster Config">config</div>
                                <div class="ks-adjuster-editor-values">
                                ${built_adjuster_values_list}
                                </div>
                            </div>
                        </div>
                        <span class="ks-remove" title="Remove">×</span>
                    </div>
                    <div class="ks-accordion">
                        <h3>1
                            <span class="ks-accordion-op ks-accordion-remove" title="Remove">×</span>
                            <span class="ks-accordion-op ks-accordion-add-andjudge" title="Add a logical 'AND' judgment">+</span>
                        </h3>
                        <div class="ks-accordion-item ks-accordion-andjudge-list" id="ks-group-${group_id}">
                            <div class="ks-accordion-andjudge-item">
                                <select class="ks-select" title="Select Condition" id="ks-accordion-action-${action_id}-add-andjudge-1-item-1" value="${KsPlayerStatus.keys().next().value}">
                                    ${built_options_player_status}
                                </select>
                                =
                                <input type="number" class="ks-input" placeholder="VALUE" title="Add Expected Value">
                                <span class="ks-remove" title="Remove">×</span>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            `;
        },
        get_ADJUSTER_ACTION_NODE() {
            KsRecorder.set('max_action_id', KsRecorder.get('max_action_id') + 1);
            let action_id = KsRecorder.get('max_action_id');
            KsRecorder.set('max_adjuster_id', KsRecorder.get('max_adjuster_id') + 1);
            let adjuster_id = KsRecorder.get('max_adjuster_id');
            return `
            <div id="ks-action-${action_id}" class="ks-widget ks-action" style="position: absolute; left: 300px; top: 255px;">
                <span class="ks-remove" title="Remove">×</span>
                <div class="ks-action-cont ks-action-info">
                    <span>ID: ${action_id}</span>/<span>Adjuster Action</span>
                </div>
                <div class="ks-adjuster" id="ks-adjuster-${adjuster_id}">
                    <div class="ks-action-cont ks-action-grid">
                        <div class="ks-action-grid-item">
                            <div class="ks-button ks-blue ks-square open-ks-aujuster-config" title="Change Adjuster Config">config</div>
                        </div>
                    </div>
                    <div class="ks-adjuster-editor-values">
                    ${built_adjuster_values_list}
                    </div>
                </div>
            </div>
            `;
        },
        get_BGM_ACTION_NODE() {
            KsRecorder.set('max_action_id', KsRecorder.get('max_action_id') + 1);
            let action_id = KsRecorder.get('max_action_id');
            return `
            <div id="ks-action-${action_id}" class="ks-widget ks-action" style="position: absolute; left: 300px; top: 255px;">
                <span class="ks-remove" title="Remove">×</span>
                <div class="ks-action-cont ks-action-info">
                    <span>ID: ${action_id}</span>/<span>BGM Action</span>
                </div>
                <div class="ks-action-cont ks-action-grid">
                    <div class="ks-action-grid-item ks-bgm-control">
                        <div class="ks-button ks-square ks-blue" title="Select A BGM file"><span>BGM</span>
                        <input type="file" name="ks-action-${action_id}_bgm" accept="audio/*" /></div>
                        <select id="ks-select_action-${action_id}_bgmaction" class="ks-select" title="Select Action for BGM">
                            <option >play</option>
                            <option>pause</option>
                            <option>resume</option>
                            <option>stop</option>
                        </select>
                        <input id="ks-action-${action_id}_bgmvolume" class="no-border-input" type="number" value=""
                            placeholder="Volume" title="Set Volume of BGM[0~100]">
                        <label class="ks-input-label" for="ks-action-${action_id}_bgmloop" title="Check If BGM Should Loop">loop</label>
                        <input id="ks-action-${action_id}_bgmloop" type="checkbox">
                    </div>
                </div>
            </div>
            `;
        },
        get_VIDEO_ACTION_NODE() {
            KsRecorder.set('max_action_id', KsRecorder.get('max_action_id') + 1);
            let action_id = KsRecorder.get('max_action_id');
            return `
            <div id="ks-action-${action_id}" class="ks-widget ks-action" style="position: absolute; left: 300px; top: 255px;">
                <span class="ks-remove" title="Remove">×</span>
                <div class="ks-action-cont ks-action-info">
                    <span>ID: ${action_id}</span>/<span>Video Action</span>
                </div>
                <div class="ks-action-cont ks-action-grid">
                    <div class="ks-action-grid-item ks-video-control">
                        <div class="ks-button ks-square ks-blue" title="Select A Video file"><span>Video</span>
                        <input type="file" name="ks-action-${action_id}_video" accept="video/*" /></div>
                        <select id="ks-select_action-${action_id}_videoaction" class="ks-select" title="Select Action for Video">
                            <option >play</option>
                            <option>pause</option>
                            <option>resume</option>
                            <option>stop</option>
                        </select>
                        <input id="ks-action-${action_id}_videovolume" class="no-border-input" type="number" value=""
                            placeholder="Volume" title="Set Volume of Video[0~100]">
                        <label class="ks-input-label" for="ks-action-${action_id}_videoloop" title="Check If Video Should Loop">loop</label>
                        <input id="ks-action-${action_id}_videoloop" type="checkbox">
                    </div>
                </div>
            </div>
            `;
        },
        get_BFG_ACTION_NODE() {
            KsRecorder.set('max_action_id', KsRecorder.get('max_action_id') + 1);
            let action_id = KsRecorder.get('max_action_id');
            return `
            <div id="ks-action-${action_id}" class="ks-widget ks-action" style="position: absolute; left: 300px; top: 255px;">
                <span class="ks-remove" title="Remove">×</span>
                <div class="ks-action-cont ks-action-info">
                    <span>ID: ${action_id}</span>/<span>BG&FG Action</span>
                </div>
                <div class="ks-action-cont ks-action-grid">
                    <div class="ks-action-grid-item ks-action-bg ks-bg-control">
                        <div class="ks-button ks-square ks-blue" title="Select A Background Image"><span>BG</span><input type="file"
                                name="ks-action-${action_id}_bg" accept="image/*" /></div>
                        <select id="ks-select_action-${action_id}_bglayer" class="ks-select" title="Select Layer of Background Image">
                            <option>Foreground3</option>
                            <option>Foreground2</option>
                            <option>Foreground1</option>
                            <option>Ground</option>
                            <option >Background1</option>
                            <option>Background2</option>
                            <option>Background3</option>
                        </select>
                    </div>
                    <div class="ks-action-grid-item ks-action-fg ks-fg-control">
                        <div class="ks-button ks-square ks-blue" title="Select A Foreground Image"><span>FG</span><input type="file"
                                name="ks-action-${action_id}_fg" accept="image/*" /></div>
                        <select id="ks-select_action-${action_id}_fglayer" class="ks-select" title="Select Layer of Foreground Image">
                            <option>Foreground3</option>
                            <option>Foreground2</option>
                            <option >Foreground1</option>
                            <option>Ground</option>
                            <option>Background1</option>
                            <option>Background2</option>
                            <option>Background3</option>
                        </select>
                    </div>
                </div>
            </div>
            `;
        },
        get_LINE_ACTION_NODE() {
            KsRecorder.set('max_action_id', KsRecorder.get('max_action_id') + 1);
            let action_id = KsRecorder.get('max_action_id');
            return `
            <div id="ks-action-${action_id}" class="ks-widget ks-action" style="position: absolute; left: 300px; top: 255px;">
                <span class="ks-remove" title="Remove">×</span>
                <div class="ks-action-cont ks-action-info">
                    <span>ID: ${action_id}</span>/<span>Line Action</span>
                </div>
                <div class="ks-action-cont ks-action-grid">
                    <div class="ks-action-grid-item">
                        <div class="ks-button ks-color-picker" title="Select Text Color">
                            <div class="color-picker"></div>
                        </div>
                        <div>
                            <input id="ks-select_action-${action_id}_color" class="no-border-input color-hex" type="text"
                                value="#xxxxxx">
                        </div>
                    </div>
                    <div class="ks-action-grid-item">
                        <div class="ks-button add-ks-line" title="Add A Line">+</div>
                    </div>
                </div>
                <div class="ks-action-cont ks-action-grid">
                    <div class="ks-action-grid-item">
                        <div class="ks-button ks-square ks-blue" title="Select A Voice file"><span>VOICE</span>
                        <input type="file" name="ks-action-${action_id}_voice" accept="audio/*" /></div>
                        <select id="ks-select_action-${action_id}_voiceaction" class="ks-select"  title="Select Action for Voice">
                            <option >play</option>
                            <option>pause</option>
                            <option>resume</option>
                            <option>stop</option>
                        </select>
                        <input id="ks-action-${action_id}_voicevolume" class="no-border-input" type="number" value=""
                            placeholder="Volume" title="Set Volume of Voice[0~100]">
                        <label class="ks-input-label" for="ks-action-${action_id}_voiceloop"  title="Check If Voice Should Loop">loop</label>
                        <input id="ks-action-${action_id}_voiceloop" type="checkbox">
                    </div>
                </div>
                <div class="ks-action-cont ks-action-grid">
                    <div class="ks-action-grid-item">
                        <div>
                            <input id="ks-select_action-${action_id}_fontsize" class="no-border-input ks-action-textsize" type="number" value=""
                                placeholder="Size(px)" title="Set Text Size">
                        </div>
                    </div>
                    <div class="ks-action-grid-item">
                        <div>
                            <input id="ks-select_action-${action_id}_linespacing" class="no-border-input ks-action-textlinespacing" type="number"
                                value="" placeholder="Space(px)" title="Set Line Spacing">
                        </div>
                    </div>
                </div>
            </div>
            `;
        },
    };

    Object.defineProperties(KsConstant, {
        builder: {
            value: builder
        },
        // KS_LINE_TEMPLATE: {
        //     value: builder.get_KS_LINE_TEMPLATE(),
        //     writable: false
        // },
        BG_ACCORDION_NODE: {
            value: builder.get_BG_ACCORDION_NODE(),
            writable: false
        },
        BGM_ACCORDION_NODE: {
            value: builder.get_BGM_ACCORDION_NODE(),
            writable: false
        },
        VOICE_ACCORDION_NODE: {
            value: builder.get_VOICE_ACCORDION_NODE(),
            writable: false
        },
        OR_JUDGE_ITEM_NODE: {
            value: builder.get_OR_JUDGE_ITEM_NODE(),
            writable: false
        },
        AND_JUDGE_ITEM_NODE: {
            value: builder.get_AND_JUDGE_ITEM_NODE(),
            writable: false
        },
        SELECTOR_ITEM_NODE: {
            value: builder.get_SELECTOR_ITEM_NODE(),
            writable: false
        },
        // JUDGE_EVENT_ITEM_NODE: {
        //     value: builder.get_JUDGE_EVENT_ITEM_NODE(),
        //     writable: false
        // },
        ADJUSTER_GROUP_LIST_NODE: {
            value: builder.get_ADJUSTER_GROUP_LIST_NODE(),
            writable: false
        },
        ADJUSTER_VALUES_LIST_NODE: {
            value: builder.get_ADJUSTER_VALUES_LIST_NODE(),
            writable: false
        },
        // COMMON_EVENT_LIST_NODE: {
        //     value: builder.get_COMMON_EVENT_LIST_NODE(),
        //     writable: false
        // },
        ACTION_ACTOR_LIST_NODE: {
            value: builder.get_ACTION_ACTOR_LIST_NODE(),
            writable: false
        }
    });

})();
