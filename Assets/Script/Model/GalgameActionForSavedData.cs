using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Model
{
    /// <summary>
    /// A scene in a script | 剧本中的某一幕
    /// </summary>
    [Serializable]
    public class GalgameActionForSavedData
    {
        public GalgameScriptLine Line;          // Actor's lines | 台词
        public string videoSrc;                 // Video in this scene | 剧中视频
        public string bgmSrc;                   // Background music in current scene | 当前幕的背景音乐
        public string voiceSrc;                 // Actor's voice | 台词语音
        public string bgSrc;                    // Background in current scene | 当前幕的背景/环境
        public Actor actor;                     // Actor's name | 演员名称
        public string ActorAnimation;           // Actor's animation: blink, speck, smile and etc. | 演员动作: 眨眼，说话，笑等等
    }
}
