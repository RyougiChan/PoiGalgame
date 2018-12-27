using Assets.Scripts.Model;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Models
{
    /// <summary>
    /// Script model of a story | 剧本实体模型
    /// </summary>
    public class GalgameScript : ScriptableObject
    {
        public string ScriptName;                           // 剧本名称
        public string ChapterName;                          // 章节名称
        public string ChapterAbstract;                      // 章节简介
        public Sprite Bg;                                   // 初始背景图片
        public AudioClip Bgm;                               // 初始背景音乐
                                                            // 剧本幕集合
        public List<GalgameAction> GalgameScenes = new List<GalgameAction>();
    }
}
