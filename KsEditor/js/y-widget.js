(() => {
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

    $('.ks-action').delegate('.ks-accordion-remove', 'click', function (evt) {
        $($(this).parent().siblings('div')[0]).remove();
        $(this).parent().remove();
    });

    $('.ks-action').delegate('.ks-accordion-addline', 'click', function (evt) {
        $(this).parent().siblings('div').append(`
        <div class="ks-line">
            <input type="text" class="ks-input" placeholder="Line Text">
            <span class="ks-remove">×</span>
        </div>
        `);
    });

    $('.ks-action').delegate('.ks-accordion-addbgm', 'click', function (evt) {
        let $cont = $(this).parent().siblings('div');
        if (!$cont.find('.ks-bgm-control').length) {
            $cont.append(`
            <div class="ks-action-cont ks-action-grid">
                <div class="ks-action-grid-item ks-bgm-control">
                    <div class="ks-button ks-square ks-blue">BGM</div>
                    <select id="ks-select_action-1_bgmaction" class="ks-select" style="display: none;">
                        <option selected="selected">play</option>
                        <option>pause</option>
                        <option>resume</option>
                        <option>stop</option>
                    </select><span tabindex="0" id="ks-select_action-1_bgmaction-button" role="combobox" aria-expanded="false" aria-autocomplete="list" aria-owns="ks-select_action-1_bgmaction-menu" aria-haspopup="true" class="ui-selectmenu-button ui-selectmenu-button-closed ui-corner-all ui-button ui-widget"><span class="ui-selectmenu-icon ui-icon ui-icon-triangle-1-s"></span><span class="ui-selectmenu-text">play</span></span>
                    <input id="ks-action-1_bgmvolume" class="no-border-input" type="text" value="" placeholder="Volume">
                    <label class="ks-input-label" for="ks-action-1_bgmloop">loop</label>
                    <input id="ks-action-1_bgmloop" type="checkbox">
                </div>
                <span class="ks-remove">×</span>
            </div>
            `);
        }
    });

    $('.ks-action').delegate('.ks-accordion-addvoice', 'click', function (evt) {
        let $cont = $(this).parent().siblings('div');
        if (!$cont.find('.ks-voice-control').length) {
            $cont.append(`
            <div class="ks-action-cont ks-action-grid">
                <div class="ks-action-grid-item ks-voice-control">
                    <div class="ks-button ks-square ks-blue">VOICE</div>
                    <select id="ks-select_action-1_voiceaction" class="ks-select" style="display: none;">
                        <option selected="selected">play</option>
                        <option>pause</option>
                        <option>resume</option>
                        <option>stop</option>
                    </select><span tabindex="0" id="ks-select_action-1_voiceaction-button" role="combobox" aria-expanded="false" aria-autocomplete="list" aria-owns="ks-select_action-1_voiceaction-menu" aria-haspopup="true" class="ui-selectmenu-button ui-selectmenu-button-closed ui-corner-all ui-button ui-widget"><span class="ui-selectmenu-icon ui-icon ui-icon-triangle-1-s"></span><span class="ui-selectmenu-text">play</span></span>
                    <input id="ks-action-1_voicevolume" class="no-border-input" type="text" value="" placeholder="Volume">
                    <label class="ks-input-label" for="ks-action-1_voiceloop">loop</label>
                    <input id="ks-action-1_voiceloop" type="checkbox">
                </div>
                <span class="ks-remove">×</span>
            </div>
            `);
        }
    });

    $(".ks-select").selectmenu();

    $(".ks-accordion").accordion();
})();