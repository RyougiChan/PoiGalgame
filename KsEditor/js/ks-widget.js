(function () {
    'use strict';

    $(".y-widget").parent().draggable({ opacity: 0.7, helper: "clone" });

    if ($('.color-picker').length) {
        $('.color-picker').farbtastic(function (c) {
            let $target_con = $(this.wheel).parent().parent().parent().parent();
            let $hex_color = $($($target_con.children().get(1)).children().get(0));
            $hex_color.val(c);
            $hex_color.css('color', c);
        });
    }

    // GV.Observer = new MutationObserver(function (mutationRecords) {
    //     mutationRecords.forEach (function (mutation) {
    //         KsUtil.refreshAction($(mutation.target).parents('.ks-action'));
    //     } );
    // });

    // $('#main-container .ks-action').each (function () {
    //     GV.Observer.observe (this, GV.obsConfig);
    // } );

    $('#main-container').on('change', '.ks-action', function (evt) {
        // Update action code
        KsUtil.refreshAction(evt.currentTarget);
    });

    $('#main-container').on('change', 'input', function(evt) {
        $(this).attr('value', $(this).val());
        if($(this).attr('type') === 'checkbox') {
            if($(this).attr('checked')) {
                $(this).removeAttr('checked');
            } else {
                $(this).attr('checked','checked');
            }
        }
    });

    $(document).on('click', '.ui-selectmenu-menu', function (evt) {
        let $t = $(evt.currentTarget);
        let labelby = $t.find('ul').attr('id');
        let action_re = /^.*(action-\d+).*$/.exec(labelby);
        let line_re = /^.*(line-\d+).*$/.exec(labelby);
        let action_id;
        if (action_re) {
            action_id = `ks-${action_re[1]}`;
        } else if (line_re) {
            action_id = $(`#ks-${line_re[1]}`).parents('.ks-action').attr('id');
        }

        let selected_content = $(evt.target).text();
        let $selected_option = $(`#${labelby.replace('-menu', '')} option:contains('${selected_content}')`);
        $(`#${labelby.replace('-menu', '')}`).attr('value', $selected_option.val() || selected_content);
        KsUtil.refreshAction(`#${action_id}`);
    });

    $('#main-container').on('click', '.ks-color-picker', function (evt) {
        let color_picker = $(evt.target).children().get(0);
        if (!color_picker) return;
        if ($(color_picker).css('display') === 'block') {
            $(color_picker).fadeOut(100);
        } else {
            $(color_picker).fadeIn(100);
        }
    });

    $('#main-container').on('click', '.ks-remove', function (evt) {
        let action_id = $(this).parents('.ks-action').attr('id');
        if($(this).parent()[0] === $(this).parents('.ks-action')[0]) {
            GV.is_removing_action = true;
            jsPlumb.removeAllEndpoints(action_id);
            KsCode.removeAction(`#${action_id}`);
        }
        $(this).parent().remove();
        if($(`#${action_id}`).length) {
            KsUtil.refreshAction(`#${action_id}`);
        }
        jsPlumb.repaintEverything();
        GV.is_removing_action = false;
    });

    $('#main-container').on('click', '.ks-button', function (evt) {
        // select a file from system
        let $select_file = $(this).find('input[type=file]');
        if ($select_file.length) {
            $select_file[0].click();
            $select_file.on('change', function (file_evt) {
                let files = file_evt.target.files;
                if ($(this).parent().attr('id') !== 'y-button-import') {
                    $(this).siblings('span').text(files.length ? files[0].name : 'File');
                }
            });
        }
        if ($(this).parents('.ks-action').length) {
            setTimeout(function () {
                KsUtil.refreshAction(`#${$(this).parents('.ks-action').attr('id')}`);
            }, 100);
        }
    });

    $('#main-container').on('click', '.ks-accordion-remove', function (evt) {
        $($(this).parent().next('div')[0]).remove();
        let action_id = $(this).parents('.ks-action').attr('id');
        $(this).parent().remove();
        KsUtil.refreshAction(`#${action_id}`);
        jsPlumb.repaintEverything();
    });

    $('#main-container').on('click', '.ks-accordion-addline', function (evt) {
        let $cont = $(this).parent().next('div');
        if ($cont && $cont[0]) {
            $($cont[0]).append(KsConstant.builder.get_KS_LINE_TEMPLATE());
        }
        $cont.find('.ks-select').selectmenu();
        jsPlumb.repaintEverything();
    });

    $('#main-container').on('click', '.ks-accordion-addbgm', function (evt) {
        let $cont = $(this).parent().next('div');
        let action_id = $(this).parents('.ks-action').attr('id');
        let action_id_num = action_id.slice(action_id.lastIndexOf('-') + 1);
        if ($cont && !$($cont[0]).find('.ks-bgm-control').length) {
            $($cont[0]).append(KsConstant.BGM_ACCORDION_NODE
                .replace(/\[\[action_id\]\]/g, action_id_num));
            $($cont[0]).find('.ks-select').selectmenu();
            jsPlumb.repaintEverything();
        }
    });

    $('#main-container').on('click', '.ks-accordion-addvoice', function (evt) {
        let $cont = $(this).parent().next('div');
        let action_id = $(this).parents('.ks-action').attr('id');
        let action_id_num = action_id.slice(action_id.lastIndexOf('-') + 1);

        if ($cont && !$($cont[0]).children('.ks-action-cont').children('.ks-voice-control').length) {
            $($cont[0]).append(KsConstant.VOICE_ACCORDION_NODE
                .replace(/\[\[action_id\]\]/g, action_id_num));
            $($cont[0]).find('.ks-select').selectmenu();
            jsPlumb.repaintEverything();
        }
    });

    $('#main-container').on('click', '.ks-accordion-addvoice', function (evt) {
        let $cont = $(this).parent().next('div');
        let action_id = $(this).parents('.ks-action').attr('id');
        let action_id_num = action_id.slice(action_id.lastIndexOf('-') + 1);

        if ($cont && !$($cont[0]).children('.ks-action-cont').children('.ks-voice-control').length) {
            $($cont[0]).append(KsConstant.VOICE_ACCORDION_NODE
                .replace(/\[\[action_id\]\]/g, action_id_num));
            $($cont[0]).find('.ks-select').selectmenu();
            jsPlumb.repaintEverything();
        }
    });

    $('#main-container').on('click', '.ks-accordion-addadjuster', function (evt) {
        let $cont = $(this).parent().next('div');

        if ($cont && !$($cont[0]).children('.ks-action-cont').children('.ks-adjuster').length) {
            $($cont[0]).append(KsConstant.builder.get_ADJUSTER_NODE());
            jsPlumb.repaintEverything();
        }
    });

    $('#main-container').on('click', '.ks-accordion-addbg', function (evt) {
        let $cont = $(this).parent().next('div');
        let action_id = $(this).parents('.ks-action').attr('id');
        let action_id_num = action_id.slice(action_id.lastIndexOf('-') + 1);
        if ($cont && !$($cont[0]).find('.ks-bg-control').length) {
            $($cont[0]).append(KsConstant.BG_ACCORDION_NODE
                .replace(/\[\[action_id\]\]/g, action_id_num));
            $($cont[0]).find('.ks-select').selectmenu();
            jsPlumb.repaintEverything();
        }
    });

    $('#main-container').on('click', '.add-ks-orjudge-item', function (evt) {
        let $cont = $(this).parents('.ks-judge');
        let action_id = $(this).parents('.ks-action').attr('id');
        let action_id_num = action_id.slice(action_id.lastIndexOf('-') + 1);
        let item_id = $cont.find('h3').length ? parseInt($cont.find('h3').last().text()) + 1 : 1;
        KsRecorder.set('max_group_id', KsRecorder.get('max_group_id') + 1);
        if ($cont && $cont.find('.ks-accordion')) {
            $cont.find('.ks-accordion').append(
                KsConstant.OR_JUDGE_ITEM_NODE
                    .replace(/\[\[action_id\]\]/g, action_id_num)
                    .replace(/\[\[group_id\]\]/g, KsRecorder.get('max_group_id'))
                    .replace(/\[\[item_id\]\]/g, item_id)
            );
            $cont.find('.ks-accordion').accordion('refresh');
            $cont.find('.ks-select').selectmenu();
            jsPlumb.repaintEverything();
        }
    });

    $('#main-container').on('click', '.add-ks-judge-event', function (evt) {
        let $cont = $(this).parents('.ks-judge');
        let action_id = $(this).parents('.ks-action').attr('id');
        let action_id_num = action_id.slice(action_id.lastIndexOf('-') + 1);
        $(KsConstant.builder.get_JUDGE_EVENT_ITEM_NODE()
            .replace(/\[\[action_id\]\]/g, action_id_num))
            .insertBefore($cont.find('.ks-accordion'));
        $cont.find('.ks-select').selectmenu();
        jsPlumb.repaintEverything();
    });

    $('#main-container').on('click', '.add-ks-events-item', function (evt) {
        let $cont = $(this).parents('.ks-events');
        let action_id = $(this).parents('.ks-action').attr('id');
        let action_id_num = action_id.slice(action_id.lastIndexOf('-') + 1);
        $(KsConstant.builder.get_EVENT_ITEM_NODE()
            .replace(/\[\[action_id\]\]/g, action_id_num))
            .insertAfter($cont.find('.ks-action-event').last());
        $cont.find('.ks-select').selectmenu();
        jsPlumb.repaintEverything();
    });

    $('#main-container').on('click', '.ks-accordion-add-andjudge', function (evt) {
        let $cont = $(this).parent().next('div');
        let action_id = $(this).parents('.ks-action').attr('id');
        let action_id_num = action_id.slice(action_id.lastIndexOf('-') + 1);
        let $judge_cont = $(this).parents('.ks-judge');
        let judge_id = $judge_cont.find('h3').length ? parseInt($judge_cont.find('h3').last().text()) : 1;

        if ($cont) {
            let judge_item_id = $($cont[0]).find('.ks-accordion-andjudge-item').last().children('select').attr('id'),
                judge_item_id_num = judge_item_id ? parseInt(judge_item_id.slice(judge_item_id.lastIndexOf('-') + 1)) + 1 : 1;
        
        $($cont[0]).append(KsConstant.AND_JUDGE_ITEM_NODE
                .replace(/\[\[action_id\]\]/g, action_id_num)
                .replace(/\[\[judge_id\]\]/g, judge_id)
                .replace(/\[\[judge_item_id_num\]\]/g, judge_item_id_num));
            $($cont[0]).find('.ks-select').selectmenu();
            jsPlumb.repaintEverything();
        }
    });

    $('#main-container').on('click', '.add-ks-selector-item', function (evt) {
        let $cont = $(this).parents('.ks-selector');
        let action_id = $(this).parents('.ks-action').attr('id');
        let action_id_num = action_id.slice(action_id.lastIndexOf('-') + 1);
        let item_id = $cont.find('h3').length ? parseInt($cont.find('h3').last().text()) + 1 : 1;
        if ($cont && $cont.find('.ks-accordion')) {
            $cont.find('.ks-accordion').append(KsConstant.SELECTOR_ITEM_NODE
                .replace(/\[\[action_id\]\]/g, action_id_num)
                .replace(/\[\[item_id\]\]/g, item_id));
            $cont.find('.ks-accordion').accordion('refresh');
            jsPlumb.repaintEverything();
        }
    });

    $('#main-container').on('click', '.open-ks-aujuster-config', function (evt) {
        let $action = $(this).parents('.ks-action'),
            $adjuster = $(this).parents('.ks-adjuster');
        $('#ks-adjuster-editor-container').attr('data-from-action', $action.attr('id'));
        $('#ks-adjuster-editor-container').attr('data-from-adjuster', $adjuster.attr('id'));
        $('#ks-adjuster-editor-container').find('h3').text(`Adjuster Config: ${$adjuster.attr('id')}`);
        let $existing_values = $adjuster.find('.ks-adjuster-editor-values > input[type=hidden]');

        for (let i = 0; i < $existing_values.length; i++) {
            let name = $existing_values[i].name;
            let value = $existing_values[i].value;
            if (name === 'is-adjuster-actived') {
                $('#ks-adjuster-isactived').prop('checked', value === 'true');
            } else {
                $('#ks-adjuster-editor-container').find(`#${name}>.ks-input`).val(value);
                $('#ks-adjuster-editor-container').find(`#${name}>.ks-adjuster-slider`).slider('value', value);
            }
        }

        $('#ks-adjuster-editor-container').fadeIn(300);
    });

    $('#main-container').on('click', '#ks-adjuster-editor-container', function (evt) {
        if (this == evt.target) {
            $(this).fadeOut(300);
            KsUtil.refreshAction(`#${$('#ks-adjuster-editor-container').attr('data-from-action')}`);
        }
    });

    $('#main-container').on('click', '.ks-accordion-op', function (evt) {
        if ($(this).parents('.ks-action').length) {
            KsUtil.refreshAction(`#${$(this).parents('.ks-action').attr('id')}`);
        }
    });

    $('#main-container').on('change', '.ks-adjuster-item > .ks-input', function (evt) {
        $(this).parent().find('.ks-adjuster-slider').slider('value', $(this).val());

        $('#' + $('#ks-adjuster-editor-container').attr('data-from-adjuster')).
            find(`input[name=${$(this).attr('name')}]`).val($(this).val());
    });

    $('.ks-select').selectmenu();

    $('.ks-accordion').accordion();
})();