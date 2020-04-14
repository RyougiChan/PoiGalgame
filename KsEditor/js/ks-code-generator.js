(function () {
    'use strict';

    if (!window.KsCode) KsCode = window.KsCode = {};

    Object.defineProperties(KsCode, {
        addAction: {
            value: function (action_id) {
                let $action = $(action_id);
                if (!$action.length) return;

                let lines = {}, bgm = {}, adjuster, actor, bg, fg;
    
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
                    $global_actor = $action.find('.ks-action-actor'),
                    $bg_files = $action.find('.ks-action-bg input[type=file]'),
                    $bg_layer = $action.find('.ks-action-bg select'),
                    $fg_files = $action.find('.ks-action-fg input[type=file]'),
                    $fg_layer = $action.find('.ks-action-fg select'),
                    $lines = $action.find('.ks-line')
                    ;
                
                ///// Text-color,size,linespacing,align,style
                if ($text_color.length && $text_color.val() !== '#xxxxxx') {
                    lines.color = $text_color.val().replace('#', '0x') + 'ff';
                }
    
                if($text_size.val()) {
                    lines.size = $text_size.val();
                }
                
                if($text_linespacing.val()) {
                    lines.linespacing = $text_linespacing.val();
                }
    
                lines.align = $text_align.val();
                lines.style = $text_style.val();
                
                ///// Adjuster
                if ($adjuster_values.length) {
                    adjuster = new Map();
                    for (let ai = 0; ai < $adjuster_values.length; ai++) {
                        let $input = $($adjuster_values[ai]);
                        adjuster.set($input.attr('name'), $input.val());
                    }
                }
    
                ///// BGM
                bgm.action = $bgm_action.val();
                bgm.loop = $bgm_loop.val() === 'on';
    
                if ($bgm_files.length && $bgm_files[0].files.length) {
                    bgm.name = $bgm_files[0].files[0].name;
                }
    
                if ($bgm_volume.val() !== undefined) {
                    bgm.volume = $bgm_volume.val();
                }
    
                ///// Actor-action
                actor = $global_actor.val();
    
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
                if($lines.length) {
                    lines.list = [];
                    for(let li = 0; li < $lines.length; li++) {
                        let $t_line = $($lines[li]),
                        $t_actor = $t_line.find('.ks-line-actor'),
                        $t_voice_file = $t_line.find('.ks-line-voice_file'),
                        $t_voice_action = $t_line.find('.ks-line-voice_action'),
                        $t_voice_volume = $t_line.find('.ks-line-voice_volume'),
                        $t_voice_loop = $t_line.find('.ks-line-voice_loop'),
                        $t_text = $t_line.find('.ks-line-text')
                        ;

                        if($t_text.val()) {
    
                            let item = {};
                            item.actor = $t_actor.val();
                            if($t_text.val()) {
                                item.text = $t_text.val();
                            }
                            if($t_voice_file[0] && $t_voice_file[0].files.length) {
                                item.voice_file = $t_voice_file[0].files[0].name;
                                item.voice_volume = $t_voice_volume.val(); 
                                item.voice_action = $t_voice_action.val(); 
                                item.voice_loop = $t_voice_loop.val() === 'on'; 
                            }
                            
                            lines.list.push(item);
                        }
                    }
                }

                let ks_code_bgm_tag = '',ks_code_bg_tag = '',ks_code_fg_tag = '';
                if(bgm) {
                    ks_code_bgm_tag = `[bgm src="${bgm.name}" ${bgm.volume ? 'volume="' + bgm.volume + '"' : '' } ${bgm.loop ? 'loop' : ''} action="${bgm.action}"]`;
                }
                if(bg) {
                    ks_code_bg_tag = `[bg src="${bg.name}" layer="${bg.layer}"]`;
                }
                if(fg) {
                    ks_code_fg_tag = `[fg src="${fg.name}" layer="${fg.layer}"]`;
                }
                if(lines.list.length) {
                    for(let i = 0; i < lines.list.length; i++) {}
                }

                $('#y-area-codetext').append(`
                <div class="ks-code-action">
                    [action]
                    <ul>
                    <li>
                    ${ks_code_bg_tag}
                    </li>
                    <li>
                    ${ks_code_fg_tag}
                    </li>
                    <li>
                    ${ks_code_bgm_tag}
                    </li>
                    </ul>
                    [action]
                </div>
                `);
    
                console.log('lines', lines)
                console.log('bg', bg)
                console.log('fg', fg)
                console.log('bgm', bgm)
                console.log('adjuster', adjuster)
            }
        }
    });

})();