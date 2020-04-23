(function () {
    'use strict';

    if (!window.KsCode) KsCode = window.KsCode = {};

    Object.defineProperties(KsCode, {
        updateAction: {
            value: function (action_id) {
                // console.log(action_id)
                let $action = $(action_id);
                if (!$action.length) return;

                let lines = {}, bgm, video, selector, adjuster, judge, actor, bg, fg;

                let $text_color = $action.children('.ks-action-cont').find('.color-hex'),
                    $text_align = $action.children('.ks-action-cont').find('.ks-action-textalign'),
                    $text_style = $action.children('.ks-action-cont').find('.ks-action-textstyle'),
                    $text_size = $action.children('.ks-action-cont').find('.ks-action-textsize'),
                    $text_linespacing = $action.children('.ks-action-cont').find('.ks-action-textlinespacing'),
                    $adjuster_values = $action.find('.ks-adjuster-editor-values input[type=hidden]'),
                    $bgm_files = $action.children('.ks-action-cont').find('.ks-bgm-control input[type=file]'),
                    $bgm_volume = $action.children('.ks-action-cont').find('.ks-bgm-control input[type=number]'),
                    $bgm_action = $action.children('.ks-action-cont').find('.ks-bgm-control select'),
                    $bgm_loop = $action.children('.ks-action-cont').find('.ks-bgm-control input[type=checkbox]'),
                    $video_files = $action.children('.ks-action-cont').find('.ks-video-control input[type=file]'),
                    $video_volume = $action.children('.ks-action-cont').find('.ks-video-control input[type=number]'),
                    $video_action = $action.children('.ks-action-cont').find('.ks-video-control select'),
                    $video_loop = $action.children('.ks-action-cont').find('.ks-video-control input[type=checkbox]'),
                    $global_actor = $action.children('.ks-action-cont').find('.ks-action-actor'),
                    $bg_files = $action.children('.ks-action-cont').find('.ks-action-bg input[type=file]'),
                    $bg_layer = $action.children('.ks-action-cont').find('.ks-action-bg select'),
                    $fg_files = $action.children('.ks-action-cont').find('.ks-action-fg input[type=file]'),
                    $fg_layer = $action.children('.ks-action-cont').find('.ks-action-fg select'),
                    $lines = $action.children('.ks-line'),
                    $judge_comp = $action.find('.ks-judge'),
                    $selector_comp = $action.find('.ks-selector')
                    ;

                ///// Text-color,size,linespacing,align,style
                if ($text_align.length) {
                    lines.align = $text_align.attr('value');
                }
                if ($text_style.length) {
                    lines.style = $text_style.attr('value');
                }
                if ($text_color.length && $text_color.attr('value') !== '#xxxxxx') {
                    lines.color = $text_color.attr('value').replace('#', '0x') + 'ff';
                }

                if ($text_size.attr('value')) {
                    lines.size = $text_size.attr('value');
                }

                if ($text_linespacing.attr('value')) {
                    lines.linespacing = $text_linespacing.attr('value');
                }

                ///// Adjuster
                if ($adjuster_values.length) {
                    adjuster = new Map();
                    for (let ai = 0; ai < $adjuster_values.length; ai++) {
                        let $input = $($adjuster_values[ai]);
                        adjuster.set($input.attr('name'), $input.attr('value'));
                    }
                }

                ///// BGM
                if ($bgm_action.length) {
                    bgm = {};
                    bgm.action = $bgm_action.attr('value') || $bgm_action.val();
                }

                if ($bgm_loop.length) {
                    bgm.loop = $bgm_loop.is(":checked");
                }

                if ($bgm_files.length && $($bgm_files[0]).attr('value')) {
                    // bgm.name = $bgm_files[0].value;
                    bgm.name = KsUtil.getFileName($($bgm_files[0]).attr('value'));
                }

                if ($bgm_volume.attr('value') !== undefined) {
                    bgm.volume = $bgm_volume.attr('value');
                }

                ///// Video
                if ($video_action.length) {
                    video = {};
                    video.action = $video_action.attr('value') || $video_action.val();
                }

                if ($video_loop.length) {
                    video.loop = $video_loop.is(":checked");
                }

                if ($video_files.length && $($video_files[0]).attr('value')) {
                    // video.name = $video_files[0].value;
                    video.name = KsUtil.getFileName($($video_files[0]).attr('value'));
                }

                if ($video_volume.attr('value') !== undefined) {
                    video.volume = $video_volume.attr('value');
                }

                ///// Actor-action
                if ($global_actor.length) {
                    actor = $global_actor.attr('value') || $global_actor.val();
                }

                ///// BG
                if ($bg_files.length && $($bg_files[0]).attr('value')) {
                    bg = {};
                    bg.name = KsUtil.getFileName($($bg_files[0]).attr('value'));
                    bg.layer = $bg_layer.attr('value') || $bg_layer.val();
                }

                ///// FG
                if ($fg_files.length && $($fg_files[0]).attr('value')) {
                    fg = {};
                    fg.name = KsUtil.getFileName($($fg_files[0]).attr('value'));
                    fg.layer = $fg_layer.attr('value') || $fg_layer.val();
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

                        if ($t_text.attr('value')) {

                            let item = {};
                            item.id = `ks-code-${$t_line.attr('id')}`;
                            item.actor = $t_actor.attr('value') || $t_actor.val();
                            if ($t_text.attr('value')) {
                                item.text = $t_text.attr('value');
                            }
                            if ($t_voice_file[0] && $($t_voice_file[0]).attr('value')) {
                                // item.voice_file = $t_voice_file[0].value;
                                item.voice_file = KsUtil.getFileName($($t_voice_file[0]).attr('value'));
                                item.voice_volume = $t_voice_volume.attr('value');
                                item.voice_action = $t_voice_action.attr('value') || $t_voice_action.val();
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
                        let v = $evt.attr('value') || $evt.val();
                        if(!judge.events.includes(v)) {
                            judge.events.push(v);
                        }
                    }

                    for(let gi = 0; gi < $judge_groups.length; gi++) {
                        let $g = $($judge_groups[gi]);
                        let $g_items = $g.find('.ks-accordion-andjudge-item');
                        let group_item_map = new Map();

                        // console.log('$judge_groups: ', $judge_groups.length)
                        // console.log('$g_items: ', $g_items.length)

                        for(let i = 0; i < $g_items.length; i++) {
                            let $g_item  = $($g_items[i]);
                            let name = $g_item.find('.ks-select').attr('value') || $g_item.find('.ks-select').val();
                            let value = $g_item.find('input[type=number]').attr('value');
                            
                            if(name && value) {
                                group_item_map.set(name, value);
                            }
                        }

                        judge.groups.push(group_item_map);
                    }

                    // console.log(judge);
                }
                console.log('judge: ', judge);

                ///// Selector
                if($selector_comp.length) {
                    let $selector_groups = $selector_comp.find('.ks-accordion > .ks-accordion-item');
                    selector = [];

                    for(let si = 0; si < $selector_groups.length; si++) {
                        let $g = $($selector_groups[si]),
                            $g_item_text = $g.children('.ks-accordion-item_text'),
                            $g_item_bgm_files = $g.find('.ks-bgm-control input[type=file]'),
                            $g_item_bgm_volume = $g.find('.ks-bgm-control input[type=number]'),
                            $g_item_bgm_action = $g.find('.ks-bgm-control select'),
                            $g_item_bgm_loop = $g.find('.ks-bgm-control input[type=checkbox]'),
                            $g_item_voice_files = $g.children('.ks-action-cont').find('.ks-voice-control input[type=file]'),
                            $g_item_voice_volume = $g.children('.ks-action-cont').find('.ks-voice-control input[type=number]'),
                            $g_item_voice_action = $g.children('.ks-action-cont').find('.ks-voice-control select'),
                            $g_item_voice_loop = $g.children('.ks-action-cont').find('.ks-voice-control input[type=checkbox]'),
                            $g_item_bg_files = $g.find('.ks-bg-control input[type=file]'),
                            $g_item_bg_layer = $g.find('.ks-bg-control select'),
                            $g_item_lines = $g.find('.ks-line')
                            ;
                        let item = {};

                        if($g_item_text.length && $g_item_text.attr('value')) {
                            item.text = $g_item_text.attr('value');
                        }

                        if ($g_item_bgm_action.length) {
                            item.bgm = {};
                            item.bgm.action = $g_item_bgm_action.attr('value') || $g_item_bgm_action.val();
                        }
        
                        if (item.bgm && $g_item_bgm_loop.length) {
                            item.bgm.loop = $g_item_bgm_loop.is(":checked");
                        }
        
                        if (item.bgm && $g_item_bgm_files.length && $($g_item_bgm_files[0]).attr('value')) {
                            // item.bgm.name = $g_item_bgm_files[0].value;
                            item.bgm.name = KsUtil.getFileName($($g_item_bgm_files[0]).attr('value'));
                        }
        
                        if (item.bgm && $g_item_bgm_volume.attr('value') !== undefined) {
                            item.bgm.volume = $g_item_bgm_volume.attr('value');
                        }

                        if ($g_item_voice_action.length) {
                            item.voice = {};
                            item.voice.action = $g_item_voice_action.attr('value') || $g_item_voice_action.val();
                        }
        
                        if (item.voice && $g_item_voice_loop.length) {
                            item.voice.loop = $g_item_voice_loop.is(":checked");
                        }
        
                        if (item.voice && $g_item_voice_files.length && $($g_item_voice_files[0]).attr('value')) {
                            // item.voice.name = $g_item_voice_files[0].value;
                            item.voice.name = KsUtil.getFileName($($g_item_voice_files[0]).attr('value'));
                        }
        
                        if (item.voice && $g_item_voice_volume.attr('value') !== undefined) {
                            item.voice.volume = $g_item_voice_volume.attr('value');
                        }

                        if ($g_item_bg_files.length && $($g_item_bg_files[0]).attr('value')) {
                            item.bg = {};
                            item.bg.name = KsUtil.getFileName($($g_item_bg_files[0]).attr('value'));
                            item.bg.layer = $g_item_bg_layer.attr('value') || $g_item_bg_layer.val();
                        }

                        if ($g_item_lines.length) {
                            item.lines = []
                            for (let li = 0; li < $g_item_lines.length; li++) {
                                let $t_line = $($g_item_lines[li]),
                                    $t_actor = $t_line.find('.ks-line-actor'),
                                    $t_voice_file = $t_line.find('.ks-line-voice_file'),
                                    $t_voice_action = $t_line.find('.ks-line-voice_action'),
                                    $t_voice_volume = $t_line.find('.ks-line-voice_volume'),
                                    $t_voice_loop = $t_line.find('.ks-line-voice_loop'),
                                    $t_text = $t_line.find('.ks-line-text')
                                    ;
        
                                if ($t_text.attr('value')) {
                                    let line_item = {};
                                    line_item.id = `ks-code-${$t_text.attr('id')}`;
                                    line_item.actor = $t_actor.attr('value') || $t_actor.val();
                                    if ($t_text.attr('value')) {
                                        line_item.text = $t_text.attr('value');
                                    }
                                    if ($t_voice_file[0] && $($t_voice_file[0]).attr('value')) {
                                        // line_item.voice_file = $t_voice_file[0].value;
                                        line_item.voice_file = KsUtil.getFileName($($t_voice_file[0]).attr('value'));
                                        line_item.voice_volume = $t_voice_volume.attr('value');
                                        line_item.voice_action = $t_voice_action.attr('value') || $t_voice_action.val();
                                        line_item.voice_loop = $t_voice_loop.is(":checked");
                                    }
        
                                    item.lines.push(line_item);
                                }
                            }
                        }

                        if(Object.getOwnPropertyNames(item).length) {
                            selector.push(item);
                        }
                    }
                    
                }

                let ks_code_bgm_tag = '',
                    ks_code_video_tag = '',
                    ks_code_bg_tag = '',
                    ks_code_fg_tag = '',
                    ks_code_selector_tag = '',
                    ks_code_adjuster_tag = '',
                    ks_code_judge_tag = '',
                    ks_code_line_tags = '';
                if (bgm) {
                    ks_code_bgm_tag = `<li class="ks-code-indent-1">[<font class="color-teal">bgm</font> ${bgm.name ? 'src=<font class="color-orange">"' + bgm.name + '"</font>' : ''} ${bgm.volume ? 'volume=<font class="color-orange">"' + bgm.volume + '"</font>' : ''} ${bgm.loop ? 'loop' : ''} action=<font class="color-orange">"${bgm.action}"</font>]</li>`;
                }
                if (video) {
                    ks_code_video_tag = `<li class="ks-code-indent-1">[<font class="color-teal">video</font> ${video.name ? 'src=<font class="color-orange">"' + video.name + '"</font>' : ''} ${video.volume ? 'volume=<font class="color-orange">"' + video.volume + '"</font>' : ''} ${video.loop ? 'loop' : ''} action=<font class="color-orange">"${video.action}"</font>]</li>`;
                }
                if (bg) {
                    ks_code_bg_tag = `<li class="ks-code-indent-1">[<font class="color-teal">bg</font> src=<font class="color-orange">"${bg.name}"</font> layer=<font class="color-orange">"${bg.layer}"</font>]</li>`;
                }
                if (fg) {
                    ks_code_fg_tag = `<li class="ks-code-indent-1">[<font class="color-teal">fg</font> src=<font class="color-orange">"${fg.name}"</font> layer=<font class="color-orange">"${fg.layer}"</font>]</li>`;
                }
                if (lines && lines.list && lines.list.length) {
                    // console.log(lines.list)
                    for (let i = 0; i < lines.list.length; i++) {
                        let line = lines.list[i];
                        ks_code_line_tags += `
                        <li id="${line.id}" class="ks-code-indent-1">
                        [<font class="color-teal">line</font> actor=<font class="color-orange">"${line.actor}"</font> line=<font class="color-orange">"${line.text}"</font>${line.voice_file ? ' voice=<font class="color-orange">"' + line.voice_file + '"</font>' : ''}${lines.size ? ' fsize=<font class="color-orange">"' + lines.size + '"</font>' : ''}${lines.linespacing ? ' linespacing=<font class="color-orange">"' + lines.linespacing + '"</font>' : ''}${lines.color ? ' fcolor=<font class="color-orange">"' + lines.color + '"</font>' : ''}${lines.style ? ' fstyle=<font class="color-orange">"' + lines.style + '"</font>' : ''}] 
                        </li>
                        `;
                    }
                }

                if (adjuster) {
                    if (adjuster.get('is-adjuster-actived') === 'true') {
                        adjuster.delete('is-adjuster-actived');
                        ks_code_adjuster_tag = `<li class="ks-code-indent-1">[<font class="color-teal">adjuster</font> id=<font class="color-orange">"adjuster-${++GV.adjuster_id}"</font>]</li>`;
                        adjuster.forEach(function (v, k) {
                            ks_code_adjuster_tag += `<li class="ks-code-indent-2"> [<font class="color-teal">pair</font> name=<font class="color-orange">"${k.split('_')[1]}"</font> value=<font class="color-orange">"${v}"</font>]</li>`;
                        });
                        ks_code_adjuster_tag += '<li class="ks-code-indent-1">[<font class="color-teal">adjuster</font>]</li>';
                    }
                }

                if(judge) {
                    console.log('111111');
                    ks_code_judge_tag = `<li class="ks-code-indent-1">[<font class="color-teal">judge</font> id=<font class="color-orange">"judge-${++GV.judge_id}"</font> events=<font class="color-orange">"${judge.events.join(',')}"</font>]</li>`;
                    judge.groups.forEach(function(g) {
                        if(g.size) {
                            ks_code_judge_tag += `<li class="ks-code-indent-2">[<font class="color-teal">group</font> id=<font class="color-orange">"group-${++GV.group_id}"</font>]</li>`;
                            g.forEach(function(v, k) {
                                ks_code_judge_tag += `<li class="ks-code-indent-3">[<font class="color-teal">pair</font> name=<font class="color-orange">"${k}"</font> value=<font class="color-orange">"${v}"</font>]</li>`;
                            });
                            ks_code_judge_tag += `<li class="ks-code-indent-2">[<font class="color-teal">group</font>]</li>`;
                        }
                    });
                    ks_code_judge_tag += `<li class="ks-code-indent-1">[<font class="color-teal">judge</font>]</li>`;
                }

                // console.log(adjuster)
                // console.log(selector)

                if(selector && selector.length) {
                    ks_code_selector_tag = '<li class="ks-code-indent-1">[<font class="color-teal">select</font> type=<font class="color-orange">"horizontal"</font>]</li>';
                    selector.forEach(function(si) {
                        ks_code_selector_tag += `<li class="ks-code-indent-2">[<font class="color-teal">option</font>{{text-anchor}}{{bg-anchor}}{{bgm-anchor}}]</li>`;
                        ks_code_selector_tag= ks_code_selector_tag.replace('{{text-anchor}}', si.text ? ` text=<font class="color-orange">"${si.text}"</font>` : '');
                        ks_code_selector_tag=ks_code_selector_tag.replace('{{bg-anchor}}', si.bg ? ` bg=<font class="color-orange">"${si.bg.name}"</font>` : '');
                        ks_code_selector_tag=ks_code_selector_tag.replace('{{bgm-anchor}}', si.bgm ? ` bgm=<font class="color-orange">"${si.bgm.name}"</font>` : '');
                        // if(si.bgm) {
                        //     ks_code_selector_tag += `<li class="ks-code-indent-3">[bgm src="${si.bgm.name}" action=<font class="color-orange">"${si.bgm.action}"</font>${si.bgm.volume?' volume=<font class="color-orange">"'+si.bgm.volume+'"</font>':''}${si.bgm.loop ? ' loop': ''}]</li>`;
                        // }
                        // if(si.voice) {
                        //     ks_code_selector_tag += `<li class="ks-code-indent-3">[voice src="${si.voice.name}" action=<font class="color-orange">"${si.voice.action}"</font>${si.voice.volume?' volume=<font class="color-orange">"'+si.voice.volume+'"</font>':''}${si.voice.loop ? ' loop': ''}]</li>`;
                        // }
                        if (si.lines && si.lines.length) {
                            // console.log(lines.list)
                            for (let i = 0; i < si.lines.length; i++) {
                                let line = si.lines[i];
                                ks_code_selector_tag += `
                                <li id="${line.id}" class="ks-code-indent-3">
                                [<font class="color-teal">line</font> actor=<font class="color-orange">"${line.actor}"</font> line=<font class="color-orange">"${line.text}"</font> ${line.voice_file ? 'voice=<font class="color-orange">"' + line.voice_file + '"</font>' : ''}] 
                                </li>
                                `;
                            }
                        }
                        ks_code_selector_tag += `<li class="ks-code-indent-2">[<font class="color-teal">option</font>]</li>`;
                    });
                    ks_code_selector_tag += '<li class="ks-code-indent-1">[<font class="color-teal">select</font>]</li>';
                }

                let ks_code_action_id = 'ks-code-' + $action.attr('id');
                let id_num = $action.attr('id').slice($action.attr('id').lastIndexOf('-') + 1);
                let next_action_id = $action.attr('data-next-action-id');

                if ($(`#${ks_code_action_id}`).length) {
                    $(`#${ks_code_action_id}`).html(`
                    [<font class="color-teal">action</font> id=<font class="color-orange">"${id_num}"</font>${next_action_id ? ' nextActionId=<font class="color-orange">"' +next_action_id.slice(next_action_id.lastIndexOf('-')+1)+ '"</font>' : ''}]
                        <ul>
                        ${ks_code_bg_tag}
                        ${ks_code_fg_tag}
                        ${ks_code_bgm_tag}
                        ${ks_code_video_tag}
                        ${ks_code_adjuster_tag}
                        ${ks_code_judge_tag}
                        ${ks_code_selector_tag}
                        ${ks_code_line_tags}
                        </ul>
                    [<font class="color-teal">action</font>]
                    `);
                } else {
                    $('#y-area-codetext').append(`
                    <div class="ks-code-action" id="${ks_code_action_id}">
                        [<font class="color-teal">action</font> id=<font class="color-orange">"${id_num}"</font>${next_action_id ? ' nextActionId=<font class="color-orange">"' +next_action_id.slice(next_action_id.lastIndexOf('-')+1)+ '"</font>' : ''}]
                        <ul>
                        ${ks_code_bg_tag}
                        ${ks_code_fg_tag}
                        ${ks_code_bgm_tag}
                        ${ks_code_video_tag}
                        ${ks_code_adjuster_tag}
                        ${ks_code_judge_tag}
                        ${ks_code_line_tags}
                        </ul>
                        [<font class="color-teal">action</font>]
                    </div>
                    `);
                }
                // console.log($('#y-area-codetext').text().replace(/\s{2,}/g, ' ').replace(/\s+\[/g, '[').replace(/\]/g, ']\n'))
            }
        },
        removeAction: {
            value: function (action_node) {
                if ($(action_node).length) {
                    $(`#ks-code-${$(action_node).attr('id')}`).remove()
                }
                // console.log($('#y-area-codetext').text().replace(/\s{2,}/g, ' ').replace(/\s+\[/g, '[').replace(/\]/g, ']\n'))
            }
        }
    });

})();