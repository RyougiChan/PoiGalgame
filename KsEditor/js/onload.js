$(function () {
    'use strict';

    // Append Elements
    $('#ks-adjuster-editor-container').html(KsConstant.ADJUSTER_GROUP_LIST_NODE);
    $('.ks-judge .ks-action-event .ks-action-event-item').html(KsConstant.COMMON_EVENT_LIST_NODE);
    $('.ks-judge .ks-accordion .ks-accordion-andjudge-list .ks-accordion-andjudge-item').html(
        KsConstant.AND_JUDGE_ITEM_NODE
        .replace(/\[\[judge_id\]\]/g, 1)
        .replace(/\[\[judge_item_id_num\]\]/g, 1)
    );
    $('.ks-adjuster-editor-values').html(KsConstant.ADJUSTER_VALUES_LIST_NODE);
    $('.ks-action-actor').html(KsConstant.ACTION_ACTOR_LIST_NODE);

    // Enable Slider
    $(".ks-adjuster-slider").slider({
        orientation: "horizontal",
        range: "min",
        min: 0,
        max: 100,
        value: 0,
        animate: true,
        slide: function (event, ui) {
            let $input = $(this).parent().find('.ks-input');
            $input.val(ui.value);

            $('#' + $('#ks-adjuster-editor-container').attr('data-from-action')).
                find(`input[name=${$input.attr('name')}]`).val(ui.value);
        }
    });

    $('.ks-select').selectmenu();
});