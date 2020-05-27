using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace Assets.Script.Model
{
    /// <summary>
    /// Script model of a story | 剧本实体模型
    /// </summary>
    [Serializable]
    public class GalgameScript : ScriptableObject
    {
        public string ScriptName;                           // 剧本名称
        public string ChapterName;                          // 章节名称
        public string ChapterAbstract;                      // 章节简介
        public string StartActionId;                        // 第一幕ID
        public Sprite Bg;                                   // 初始背景图片
        public AudioClip Bgm;                               // 初始背景音乐
        public VideoClip Video;                             // 开场视频
                                                            // 剧本幕集合
        public List<GalgameAction> GalgameActions = new List<GalgameAction>();
    }
}
