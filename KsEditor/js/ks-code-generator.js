(function () {
    'use strict';

    if (!window.KsCode) KsCode = window.KsCode = {};

    Object.defineProperties(KsCode, {
        updateAction: {
            value: function (action_id) {
                let $action = $(action_id);
                if (!$action.length) return;

                let lines = {}, bgm, video, adjuster, judge, actor, bg, fg;

                let $text_color = $action.find('.color-hex'),
                    $text_align = $action.find('.ks-action-textalign'),
                    $text_style = $action.find('.ks-action-textstyle'),
                    $text_size = $action.find('.ks-action-textsize'),
                    $text_linespacing = $action.find('.ks-action-textlinespacing'),
                    $adjuster_values = $action.find('.ks-adjuster-editor-values input[type=hidden]'),
                    $bgm_files = $action.find('.ks-bgm-control input[type=file]'),
                    $bgm_volume = $action.find('.ks-bgm-control input[type=number]'),
                    $bgm_action = $action.find('.ks-bgm-control select'),
                    $bgm_loop = $action.find('.ks-bgm-control input[type=checkbox]'),
                    $video_files = $action.find('.ks-video-control input[type=file]'),
                    $video_volume = $action.find('.ks-video-control input[type=number]'),
                    $video_action = $action.find('.ks-video-control select'),
                    $video_loop = $action.find('.ks-video-control input[type=checkbox]'),
                    $global_actor = $action.find('.ks-action-actor'),
                    $bg_files = $action.find('.ks-action-bg input[type=file]'),
                    $bg_layer = $action.find('.ks-action-bg select'),
                    $fg_files = $action.find('.ks-action-fg input[type=file]'),
                    $fg_layer = $action.find('.ks-action-fg select'),
                    $lines = $action.find('.ks-line'),
                    $judge_comp = $action.find('.ks-judge')
                    ;

                ///// Text-color,size,linespacing,align,style
                if ($text_align.length) {
                    lines.align = $text_align.val();
                }
                if ($text_style.length) {
                    lines.style = $text_style.val();
                }
                if ($text_color.length && $text_color.val() !== '#xxxxxx') {
                    lines.color = $text_color.val().replace('#', '0x') + 'ff';
                }

                if ($text_size.val()) {
                    lines.size = $text_size.val();
                }

                if ($text_linespacing.val()) {
                    lines.linespacing = $text_linespacing.val();
                }

                ///// Adjuster
                if ($adjuster_values.length) {
                    adjuster = new Map();
                    for (let ai = 0; ai < $adjuster_values.length; ai++) {
                        let $input = $($adjuster_values[ai]);
                        adjuster.set($input.attr('name'), $input.val());
                    }
                }

                ///// BGM
                if ($bgm_action.length) {
                    bgm = {};
                    bgm.action = $bgm_action.val();
                }

                if ($bgm_loop.length) {
                    bgm.loop = $bgm_loop.is(":checked");
                }

                if ($bgm_files.length && $bgm_files[0].files.length) {
                    bgm.name = $bgm_files[0].files[0].name;
                    bgm.name = bgm.name.substring(0, bgm.name.lastIndexOf('.'));
                }

                if ($bgm_volume.val() !== undefined) {
                    bgm.volume = $bgm_volume.val();
                }

                ///// Video
                if ($video_action.length) {
                    video = {};
                    video.action = $video_action.val();
                }

                if ($video_loop.length) {
                    video.loop = $video_loop.is(":checked");
                }

                if ($video_files.length && $video_files[0].files.length) {
                    video.name = $video_files[0].files[0].name;
                    video.name = video.name.substring(0, video.name.lastIndexOf('.'));
                }

                if ($video_volume.val() !== undefined) {
                    video.volume = $video_volume.val();
                }

                ///// Actor-action
                if ($global_actor.length) {
                    actor = $global_actor.val();
                }

                ///// BG
                if ($bg_files.length && $bg_files[0].files.length) {
                    bg = {};
                    bg.name = $bg_files[0].files[0].name;
                    bg.layer = $bg_layer.val();
                }

                ///// FG
                if ($fg_files.length && $fg_files[0].files.length) {
                    fg = {};
                    fg.name = $fg_files[0].files[0].name;
                    fg.layer = $fg_layer.val();
                }

                ///// Lines
                if ($lines.length) {
                    lines.list = [];
                    for (let li = 0; li < $lines.length; li++) {
                        let $t_line = $($lines[li]),
                            $t_actor = $t_line.find('.ks-line-actor'),
                            $t_voice_file = $t_line.find('.ks-line-voice_file'),
                            $t_voice_action = $t_line.find('.ks-line-voice_action'),
                            $t_voice_volume = $t_line.find('.ks-line-voice_volume'),
                            $t_voice_loop = $t_line.find('.ks-line-voice_loop'),
                            $t_text = $t_line.find('.ks-line-text')
                            ;

                        if ($t_text.val()) {

                            let item = {};
                            item.id = `ks-code-${$t_line.attr('id')}`;
                            item.actor = $t_actor.val();
                            if ($t_text.val()) {
                                item.text = $t_text.val();
                            }
                            if ($t_voice_file[0] && $t_voice_file[0].files.length) {
                                item.voice_file = $t_voice_file[0].files[0].name;
                                item.voice_file = item.voice_file.substring(0, item.voice_file.lastIndexOf('.'));
                                item.voice_volume = $t_voice_volume.val();
                                item.voice_action = $t_voice_action.val();
                                item.voice_loop = $t_voice_loop.is(":checked");
                            }

                            lines.list.push(item);
                        }
                    }
                }

                ///// Judge
                if ($judge_comp.length) {
                    let $judge_events = $action.find('.ks-judge-event'),
                        $judge_groups = $action.find('.ks-accordion-andjudge-list');

                    judge = {};
                    judge.events = [];
                    judge.groups = [];
                    
                    for(let ji = 0; ji < $judge_events.length; ji++) {
                        let $evt = $($judge_events[ji]);
                        if(!judge.events.includes($evt.val())) {
                            judge.events.push($evt.val());
                        }
                    }

                    for(let gi = 0; gi < $judge_groups.length; gi++) {
                        let $g = $($judge_groups[gi]);
                        let $g_items = $g.find('.ks-accordion-andjudge-item');
                        let group_item_map = new Map();

                        for(let i = 0; i < $g_items.length; i++) {
                            let $g_item  = $($g_items[i]);
                            let name = $g_item.find('.ks-select').val();
                            let value = $g_item.find('input[type=number]').val();
                            
                            if(name && value) {
                                group_item_map.set(name, value);
                            }
                        }

                        judge.groups.push(group_item_map);
                    }

                    // console.log(judge);
                }

                let ks_code_bgm_tag = '',
                    ks_code_video_tag = '',
                    ks_code_bg_tag = '',
                    ks_code_fg_tag = '',
                    ks_code_adjuster_tag = '',
                    ks_code_judge_tag = '',
                    ks_code_line_tags = '';
                if (bgm) {
                    ks_code_bgm_tag = `[bgm ${bgm.name ? 'src="' + bgm.name + '"' : ''} ${bgm.volume ? 'volume="' + bgm.volume + '"' : ''} ${bgm.loop ? 'loop' : ''} action="${bgm.action}"]`;
                }
                if (video) {
                    ks_code_video_tag = `[video ${video.name ? 'src="' + video.name + '"' : ''} ${video.volume ? 'volume="' + video.volume + '"' : ''} ${video.loop ? 'loop' : ''} action="${video.action}"]`;
                }
                if (bg) {
                    ks_code_bg_tag = `[bg src="${bg.name}" layer="${bg.layer}"]`;
                }
                if (fg) {
                    ks_code_fg_tag = `[fg src="${fg.name}" layer="${fg.layer}"]`;
                }
                if (lines && lines.list && lines.list.length) {
                    // console.log(lines.list)
                    for (let i = 0; i < lines.list.length; i++) {
                        let line = lines.list[i];
                        ks_code_line_tags += `
                        <li id="${line.id}" class="ks-code-indent-1">
                        [line actor="${line.actor}" line="${line.text}" ${line.voice_file ? 'voice="' + line.voice_file + '"' : ''}] 
                        </li>
                        `;
                    }
                }
                if (adjuster) {
                    if (adjuster.get('is-adjuster-actived') === 'true') {
                        adjuster.delete('is-adjuster-actived');
                        ks_code_adjuster_tag = '<li class="ks-code-indent-1">[adjuster id=""]</li>';
                        adjuster.forEach(function (v, k) {
                            ks_code_adjuster_tag += `<li class="ks-code-indent-2"> [pair name="${k.split('_')[1]}" value="${v}"]</li>`;
                        });
                        ks_code_adjuster_tag += '<li class="ks-code-indent-1">[adjuster]</li>';
                    }
                }
                if(judge) {
                    ks_code_judge_tag = `<li class="ks-code-indent-1">[judge id="" events=""]</li>`;

                }

                let ks_code_action_id = 'ks-code-' + $action.attr('id');
                if ($(`#${ks_code_action_id}`).length) {
                    $(`#${ks_code_action_id}`).html(`
                    [action]
                        <ul>
                        <li class="ks-code-indent-1">
                        ${ks_code_bg_tag}
                        </li>
                        <li class="ks-code-indent-1">
                        ${ks_code_fg_tag}
                        </li>
                        <li class="ks-code-indent-1">
                        ${ks_code_bgm_tag}
                        </li>
                        <li class="ks-code-indent-1">
                        ${ks_code_video_tag}
                        </li>
                        ${ks_code_adjuster_tag}
                        ${ks_code_line_tags}
                        </ul>
                    [action]
                    `);
                } else {
                    $('#y-area-codetext').append(`
                    <div class="ks-code-action" id="${ks_code_action_id}">
                        [action]
                        <ul>
                        <li class="ks-code-indent-1">
                        ${ks_code_bg_tag}
                        </li>
                        <li class="ks-code-indent-1">
                        ${ks_code_fg_tag}
                        </li>
                        <li class="ks-code-indent-1">
                        ${ks_code_bgm_tag}
                        </li>
                        <li class="ks-code-indent-1">
                        ${ks_code_video_tag}
                        </li>
                        ${ks_code_adjuster_tag}
                        ${ks_code_line_tags}
                        </ul>
                        [action]
                    </div>
                    `);
                }
                console.log($('#y-area-codetext').text().replace(/\s{2,}/g, ' ').replace(/\s+\[/g, '[').replace(/\]/g, ']\n'))
            }
        },
        removeAction: {
            value: function (action_id) {
                if ($(action_id).length) {
                    $(`#ks-code-${$(action_id).attr('id')}`).remove()
                }
                console.log($('#y-area-codetext').text().replace(/\s{2,}/g, ' ').replace(/\s+\[/g, '[').replace(/\]/g, ']\n'))
            }
        }
    });

})();