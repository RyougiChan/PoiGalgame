using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace Assets.Script.Model
{
    /// <summary>
    /// A basic scene in a script(No other components) | 剧本中的基本的某一幕(无其他组件)
    /// </summary>
    [Serializable]
    public class GalgamePlainAction
    {
        public string Id;                                                       // 当前场景ID
        public string PreviousActionId;                                         // 前一个场景ID
        public string NextActionId;                                             // 下一个场景ID
        public string RoundId;                                                  // 轮次ID
        public GalgameScriptLine Line;                                          // Actor's line(Single) | 台词(单条)
        public List<GalgameScriptLine> Lines = new List<GalgameScriptLine>();   // Actor's lines | 台词
        public VideoClip Video;                                                 // Video in this scene | 剧中视频
        public AudioClip Bgm;                                                   // Background music in current scene | 当前幕的背景音乐
        public AudioClip Voice;                                                 // Actor's voice | 台词语音
        public Sprite Background;                                               // Background in current scene | 当前幕的背景/环境
        public Layer BgLayer = Layer.BACKGROUND1;                               // Which Layer does the Background on | 当前背景/环境所在的展示层
        public Sprite Foreground;                                               // Foreground in current scene | 当前幕的前景/演员立绘
        public Layer FgLayer = Layer.FOREGROUND1;                               // Which Layer does the Foreground on | 当前前景/演员立绘所在的展示层
        public Actor Actor;                                                     // Actor's name | 演员名称
        public string ActorAnimation;                                           // Actor's animation: blink, speck, smile and etc. | 演员动作: 眨眼，说话，笑等等
        public string Input;                                                    // Input scene | 玩家输入场景
    }
}
