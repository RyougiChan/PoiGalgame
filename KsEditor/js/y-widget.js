(function () {
    'use strict';

    $(".y-widget").parent().draggable({ opacity: 0.7, helper: "clone" });

    $('.color-picker').farbtastic(function (c) {
        console.log(c);
        let $target_con = $(this.wheel).parent().parent().parent().parent();
        let $hex_color = $($($target_con.children().get(1)).children().get(0));
        $hex_color.val(c);
        $hex_color.css('color', c);
    });

    $('.ks-color-picker').on('click', function (evt) {
        let color_picker = $(evt.target).children().get(0);
        if (!color_picker) return;
        if ($(color_picker).css('display') === 'block') {
            $(color_picker).fadeOut(100);
        } else {
            $(color_picker).fadeIn(100);
        }
    });

    $('.ks-action').delegate('.ks-remove', 'click', function (evt) {
        $(this).parent().remove();
    });

    $('#main-container').delegate('.ks-button', 'click', function (evt) {
        // select a file from system
        let $select_file = $(this).find('input[type=file]');
        if($select_file.length) {
            $select_file[0].click();
            $select_file.on('change', function(file_evt) {
                let files = file_evt.target.files;
                if(files.length) {
                    $(this).siblings('span').text(files[0].name);
                }
            });
        }
    });

    $('.ks-action').delegate('.ks-accordion-remove', 'click', function (evt) {
        $($(this).parent().next('div')[0]).remove();
        $(this).parent().remove();
    });

    $('.ks-action').delegate('.ks-accordion-addline', 'click', function (evt) {
        let $cont = $(this).parent().next('div');
        if($cont && $cont[0]) {
            $($cont[0]).append(`
            <div class="ks-line">
                <input type="text" class="ks-input" placeholder="Line Text">
                <span class="ks-remove">×</span>
            </div>
            `);
        }
    });

    $('.ks-action').delegate('.ks-accordion-addbgm', 'click', function (evt) {
        let $cont = $(this).parent().next('div');
        if ($cont && !$($cont[0]).find('.ks-bgm-control').length) {
            $($cont[0]).append(`
            <div class="ks-action-cont ks-action-grid">
                <div class="ks-action-grid-item ks-bgm-control">
                    <div class="ks-button ks-square ks-blue"><span>BGM</span><input type="file" name="ks-action-1_bgm" accept="audio/*" /></div>
                    <select id="ks-select_action-1_bgmaction" class="ks-select">
                        <option selected="selected">play</option>
                        <option>pause</option>
                        <option>resume</option>
                        <option>stop</option>
                    </select>
                    <input id="ks-action-1_bgmvolume" class="no-border-input" type="text" value="" placeholder="Volume">
                    <label class="ks-input-label" for="ks-action-1_bgmloop">loop</label>
                    <input id="ks-action-1_bgmloop" type="checkbox">
                </div>
                <span class="ks-remove">×</span>
            </div>
            `);
            $($cont[0]).find('.ks-select').selectmenu();
        }
    });

    $('.ks-action').delegate('.ks-accordion-addvoice', 'click', function (evt) {
        let $cont = $(this).parent().next('div');
        if ($cont && !$($cont[0]).find('.ks-voice-control').length) {
            $($cont[0]).append(`
            <div class="ks-action-cont ks-action-grid">
                <div class="ks-action-grid-item ks-voice-control">
                    <div class="ks-button ks-square ks-blue"><span>VOICE</span><input type="file" name="ks-action-1_voice" accept="audio/*" /></div>
                    <select id="ks-select_action-1_voiceaction" class="ks-select">
                        <option selected="selected">play</option>
                        <option>pause</option>
                        <option>resume</option>
                        <option>stop</option>
                    </select>
                    <input id="ks-action-1_voicevolume" class="no-border-input" type="text" value="" placeholder="Volume">
                    <label class="ks-input-label" for="ks-action-1_voiceloop">loop</label>
                    <input id="ks-action-1_voiceloop" type="checkbox">
                </div>
                <span class="ks-remove">×</span>
            </div>
            `);
            $($cont[0]).find('.ks-select').selectmenu();
        }
    });

    $('.ks-action').delegate('.ks-accordion-addbg', 'click', function (evt) {
        let $cont = $(this).parent().next('div');
        if ($cont && !$($cont[0]).find('.ks-bg-control').length) {
            $($cont[0]).append(`
            <div class="ks-action-cont ks-action-grid">
                <div class="ks-action-grid-item ks-bg-control">
                    <div class="ks-button ks-square ks-blue"><span>BG</span><input type="file" name="ks-action-1_bg" accept="image/*" /></div>
                    <select id="ks-select_action-1_bglayer" class="ks-select">
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
            `);
            $($cont[0]).find('.ks-select').selectmenu();
        }
    });
    
    $('.ks-action').delegate('.add-ks-orjudge-item', 'click', function (evt) {
        let $cont = $(this).parents('.ks-judge');
        let item_id = $cont.find('h3').length ? parseInt($cont.find('h3').last().text()) + 1 : 1;
        if ($cont && $cont.find('.ks-accordion')) {
            $cont.find('.ks-accordion').append(`
            <h3>${item_id}
                <span class="ks-accordion-op ks-accordion-remove">×</span>
                <span class="ks-accordion-op ks-accordion-add-andjudge">+</span>
            </h3>
            <div class="ks-accordion-item ks-accordion-andjudge-list">
                <div class="ks-accordion-andjudge-item" id="ks-accordion-add-andjudge-${item_id}-item-1">
                    <select class="ks-select">
                        <option selected="selected">Health Point</option>
                        <option>Mana Point</option>
                    </select>
                    =
                    <input type="number" class="ks-input" placeholder="VALUE">
                    <span class="ks-remove">×</span>
                </div>
            </div>
            `);
            $cont.find('.ks-accordion').accordion('refresh');
            $cont.find('.ks-select').selectmenu();
        }
    });

    $('.ks-action').delegate('.ks-accordion-add-andjudge', 'click', function (evt) {
        let $cont = $(this).parent().next('div');
        let $judge_cont = $(this).parents('.ks-judge');
        let judge_id = $judge_cont.find('h3').length ? parseInt($judge_cont.find('h3').last().text()) : 1;
        if ($cont) {
            let $judge_item_id = $($cont[0]).find('.ks-accordion-andjudge-item').last().attr('id'),
            judge_item_id_num = $judge_item_id ? parseInt($judge_item_id.slice($judge_item_id.lastIndexOf('-') + 1)) + 1 : 1;
            $($cont[0]).append(`
            <div class="ks-accordion-andjudge-item" id="ks-accordion-add-andjudge-${judge_id}-item-${judge_item_id_num}">
                <select class="ks-select">
                    <option selected="selected">Health Point</option>
                    <option>Mana Point</option>
                </select>
                =
                <input type="number" class="ks-input" placeholder="VALUE">
                <span class="ks-remove">×</span>
            </div>
            `);
            $($cont[0]).find('.ks-select').selectmenu();
        }
    });

    $('.ks-action').delegate('.add-ks-selector-item', 'click', function (evt) {
        let $cont = $(this).parents('.ks-selector');
        let item_id = $cont.find('h3').length ? parseInt($cont.find('h3').last().text()) + 1 : 1;
        if ($cont && $cont.find('.ks-accordion')) {
            $cont.find('.ks-accordion').append(`
            <h3>${item_id}
                <span class="ks-accordion-op ks-accordion-remove">×</span>
                <span class="ks-accordion-op ks-accordion-addline">+</span>
                <span class="ks-accordion-op ks-accordion-addbgm">bgm</span>
                <span class="ks-accordion-op ks-accordion-addvoice">voice</span>
                <span class="ks-accordion-op ks-accordion-addbg">bg</span>
            </h3>
            <div class="ks-accordion-item">
                <input type="text" class="ks-input ks-accordion-item_text" placeholder="Item Text">
            </div>
            `);
            $cont.find('.ks-accordion').accordion('refresh');
        }
    });

    $(".ks-select").selectmenu();

    $(".ks-accordion").accordion();
})();