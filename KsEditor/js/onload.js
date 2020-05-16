$(function () {
    'use strict';

    // Generate round selection table
    let $round_selection_table = $('<table><tbody></tbody></table>');
    for(let i = 0; i < 10; i++) {
        let $row = $('<tr></tr>');
        for(let j = (i) * 10; j < (i+1) * 10; j++) {
            $row.append(`
            <td data-round-id="${j+1}">${j+1}
                <input type="hidden" name="data-jsplumb-connect-uuids">
                <input type="hidden" name="data-scale-pos" value="[0,0]">
                <input type="hidden" name="data-scale-value" value="100">
                <input type="hidden" name="data-draggable-data" value="">
            </td>`);
        }
        if(i == 0) $row.children('td:first-child').addClass('active');
        $round_selection_table.append($row);
    }
    $('#ks-round-selection-container > .ks-round-selection').append($round_selection_table);
    $('#y-area-draggable').on('click', '#ks-round-selection-container > .ks-round-selection table td', function(evt) {
        if(!$(evt.target).hasClass('active') && confirm('Load this round?')) {

            $('#ks-round-selection-container').fadeOut(100);
            $('#y-area-scaleable').attr('data-round-id', $(evt.target).attr('data-round-id'));
            
            let $cur_active = $('#ks-round-selection-container > .ks-round-selection table td.active');
            $cur_active.attr('data-round-first-action-id', $('#y-area-scaleable').attr('data-round-first-action-id'));
            $cur_active.attr('data-round-last-action-id', $('#y-area-scaleable').attr('data-round-last-action-id'));
            
            KsUtil.saveRoundAction($cur_active);
            
            $cur_active.removeClass('active');
            $(evt.target).addClass('active');
            
            KsUtil.loadRoundAction($(evt.target));
        }
    });

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
        min: -100,
        max: 100,
        value: 0,
        animate: true,
        slide: function (event, ui) {
            let $input = $(this).parent().find('.ks-input');
            $input.val(ui.value);

            $('#' + $('#ks-adjuster-editor-container').attr('data-from-adjuster')).
                find(`input[name=${$input.attr('name')}]`).val(ui.value);
        }
    });

    $('#ks-adjuster-isactived').on('change', function() {
        $('#' + $('#ks-adjuster-editor-container').attr('data-from-adjuster')).
            find('input[name=is-adjuster-actived]').val(
                $('#ks-adjuster-isactived').prop('checked')
            );
    });

    $('.ks-select').selectmenu();
});