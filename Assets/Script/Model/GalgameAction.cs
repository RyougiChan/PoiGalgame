﻿using System;
using UnityEngine;

namespace Assets.Scripts.Models
{
    /// <summary>
    /// A scene in a script | 剧本中的某一幕
    /// </summary>
    [Serializable]
    public class GalgameAction
    {
        public string Line;                     // Actor's lines | 台词
        public AudioClip Bgm;                   // Background music in current scene | 当前幕的背景音乐
        public AudioClip Voice;                 // Actor's voice | 台词语音
        public Sprite Background;               // Background in current scene | 当前幕的背景/环境
        public Actor Actor;                     // Actor's name | 演员名称
        public string ActorAnimation;           // Actor's animation: blink, speck, smile and etc. | 演员动作: 眨眼，说话，笑等等
    }
}
