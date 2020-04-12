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
        if ($select_file.length) {
            $select_file[0].click();
            $select_file.on('change', function (file_evt) {
                let files = file_evt.target.files;
                if (files.length) {
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
        let action_id = $(this).parents('.ks-action').attr('id');
        let action_id_num = action_id.slice(action_id.lastIndexOf('-')+1);
        let item_id = parseInt($(this).parent().text());
        let $line_items = $cont.find('.ks-line');
        let line_id = $line_items.length ? parseInt($line_items.last().attr('id').slice($line_items.last().attr('id').lastIndexOf('-') + 1)) + 1 : 1;
        if ($cont && $cont[0]) {
            $($cont[0]).append(KsConstant.LINE_ACCORDION_NODE
                .replace('[[action_id]]', action_id_num)
                .replace('[[item_id]]', item_id)
                .replace('[[line_id]]', line_id));
        }
    });

    $('.ks-action').delegate('.ks-accordion-addbgm', 'click', function (evt) {
        let $cont = $(this).parent().next('div');
        let action_id = $(this).parents('.ks-action').attr('id');
        let action_id_num = action_id.slice(action_id.lastIndexOf('-')+1);
        if ($cont && !$($cont[0]).find('.ks-bgm-control').length) {
            $($cont[0]).append(KsConstant.BGM_ACCORDION_NODE.replace('[[action_id]]', action_id_num));
            $($cont[0]).find('.ks-select').selectmenu();
        }
    });

    $('.ks-action').delegate('.ks-accordion-addvoice', 'click', function (evt) {
        let $cont = $(this).parent().next('div');
        if ($cont && !$($cont[0]).find('.ks-voice-control').length) {
            $($cont[0]).append(KsConstant.VOICE_ACCORDION_NODE.replace('[[action_id]]', action_id_num));
            $($cont[0]).find('.ks-select').selectmenu();
        }
    });

    $('.ks-action').delegate('.ks-accordion-addbg', 'click', function (evt) {
        let $cont = $(this).parent().next('div');
        if ($cont && !$($cont[0]).find('.ks-bg-control').length) {
            $($cont[0]).append(KsConstant.BG_ACCORDION_NODE.replace('[[action_id]]', action_id_num));
            $($cont[0]).find('.ks-select').selectmenu();
        }
    });

    $('.ks-action').delegate('.add-ks-orjudge-item', 'click', function (evt) {
        let $cont = $(this).parents('.ks-judge');
        let item_id = $cont.find('h3').length ? parseInt($cont.find('h3').last().text()) + 1 : 1;
        if ($cont && $cont.find('.ks-accordion')) {
            $cont.find('.ks-accordion').append(KsConstant.OR_JUDGE_ITEM_NODE.replace('[[item_id]]', item_id));
            $cont.find('.ks-accordion').accordion('refresh');
            $cont.find('.ks-select').selectmenu();
        }
    });

    $('.ks-action').delegate('.add-ks-judge-event', 'click', function (evt) {
        let $cont = $(this).parents('.ks-judge');
        let $event_items = $cont.find('.ks-action-event-item');
        let action_id = $(this).parents('.ks-action').attr('id');
        let action_id_num = action_id.slice(action_id.lastIndexOf('-')+1);
        // console.log($(this).parents('.ks-action').attr('id'))
        let event_item_id = $event_items.length ? parseInt($event_items.last().attr('id').slice($event_items.last().attr('id').lastIndexOf('-') + 1)) + 1 : 1;
        $(KsConstant.JUDGE_EVENT_ITEM_NODE.replace('[[item_id]]', event_item_id).replace('[[action_id]]', action_id_num))
            .insertBefore($cont.find('.ks-accordion'));
        $cont.find('.ks-select').selectmenu();
    });

    $('.ks-action').delegate('.ks-accordion-add-andjudge', 'click', function (evt) {
        let $cont = $(this).parent().next('div');
        let $judge_cont = $(this).parents('.ks-judge');
        let judge_id = $judge_cont.find('h3').length ? parseInt($judge_cont.find('h3').last().text()) : 1;
        if ($cont) {
            let $judge_item_id = $($cont[0]).find('.ks-accordion-andjudge-item').last().attr('id'),
                judge_item_id_num = $judge_item_id ? parseInt($judge_item_id.slice($judge_item_id.lastIndexOf('-') + 1)) + 1 : 1;
            $($cont[0]).append(KsConstant.AND_JUDGE_ITEM_NODE.replace('[[judge_id]]', judge_id).replace('[[judge_item_id_num]]', judge_item_id_num));
            $($cont[0]).find('.ks-select').selectmenu();
        }
    });

    $('.ks-action').delegate('.add-ks-selector-item', 'click', function (evt) {
        let $cont = $(this).parents('.ks-selector');
        let item_id = $cont.find('h3').length ? parseInt($cont.find('h3').last().text()) + 1 : 1;
        if ($cont && $cont.find('.ks-accordion')) {
            $cont.find('.ks-accordion').append(KsConstant.SELECTOR_ITEM_NODE.replace('[[item_id]]', item_id));
            $cont.find('.ks-accordion').accordion('refresh');
        }
    });

    $('.ks-action').delegate('.open-ks-aujuster-config', 'click', function (evt) {
        let $action = $(this).parents('.ks-action');
        $('#ks-adjuster-editor-container').attr('data-from-action', $action.attr('id'));
        $('#ks-adjuster-editor-container').find('h3').text(`Adjuster Config: ${$action.attr('id')}`);
        let $existing_values = $action.find('.ks-adjuster-editor-values > input[type=hidden]');
        
        for(let i = 0; i < $existing_values.length; i++) {
            let name = $existing_values[i].name;
            let value = $existing_values[i].value;
            $('#ks-adjuster-editor-container').find(`#${name}>.ks-input`).val(value);
            $('#ks-adjuster-editor-container').find(`#${name}>.ks-adjuster-slider`).slider('value', value);
        }
        
        $('#ks-adjuster-editor-container').fadeIn(300);
    });

    $('#ks-adjuster-editor-container').on('click', function(evt) {
        if(this == evt.target) $(this).fadeOut(300);
    });

    $('.ks-adjuster-item > .ks-input').on('change', function(evt) {
        $(this).parent().find('.ks-adjuster-slider').slider('value', $(this).val());
        $('#'+$('#ks-adjuster-editor-container').attr('data-from-action')).
            find(`input[name=${$(this).attr('name')}]`).val($(this).val());
    });

    $(".ks-select").selectmenu();

    $(".ks-accordion").accordion();

    $( ".ks-adjuster-slider" ).slider({
        orientation: "horizontal",
        range: "min",
        min: 0,
        max: 100,
        value: 0,
        animate: true,
        slide: function( event, ui ) {
            let $input = $(this).parent().find('.ks-input');
            $input.val( ui.value );
            $('#'+$('#ks-adjuster-editor-container').attr('data-from-action')).
            find(`input[name=${$input.attr('name')}]`).val(ui.value);
        }
      });
})();