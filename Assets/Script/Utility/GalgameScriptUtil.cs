#if UNITY_EDITOR
using Assets.Script.Model;
using Assets.Script.Model.Components;
using Assets.Script.Model.Datas;
using Assets.Script.Model.Enum;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Video;

namespace Assets.Script.Utility
{
    public class GalgameScriptUtil : ScriptableObject
    {
        public static List<GalgameKsScriptTag> BLOCK_TYPE_TAGS = new List<GalgameKsScriptTag> { GalgameKsScriptTag.LINE, GalgameKsScriptTag.ADJUSTER, GalgameKsScriptTag.BATTLE, GalgameKsScriptTag.SELECT, GalgameKsScriptTag.JUDGE, GalgameKsScriptTag.GROUP };
        public static List<GalgameKsScriptTag> NOT_ACTION_TAGS = new List<GalgameKsScriptTag>() { GalgameKsScriptTag.PAIR, GalgameKsScriptTag.OPTION, GalgameKsScriptTag.GROUP };
        //public static string[] GalgameScriptTags;
        public static void CreateGalgameScriptAsset()
        {
            GalgameScript ggs = CreateInstance<GalgameScript>();
            CreateGalgameScriptAsset(ggs);
        }

        public static void CreateGalgameScriptAsset(string assetFileFullPath)
        {
            GalgameScript ggs = CreateInstance<GalgameScript>();
            CreateGalgameScriptAsset(ggs, assetFileFullPath);
        }

        public static void CreateGalgameScriptAsset(GalgameScript ggs, string assetFileFullPath = null)
        {
            if(null == assetFileFullPath)
            {
                string path = AssetDatabase.GetAssetPath(Selection.activeObject);
                if (path == "")
                {
                    path = "Assets";
                }
                else if (Path.GetExtension(path) != "")
                {
                    path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
                }
                assetFileFullPath = AssetDatabase.GenerateUniqueAssetPath(path + "/NewGalgameScript.gs.asset");
            }
            AssetDatabase.CreateAsset(ggs, assetFileFullPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = ggs;
        }

        public static GalgameScript KsScriptToGalgameScript(List<KsScriptLine> ksScript)
        {
            if (null == ksScript || ksScript.Count == 0) return null;
            //if (null == GalgameScriptTags) GalgameScriptTags = Enum.GetNames(typeof(GalgameKsScriptTag));
            GalgameScript galgameScript = new GalgameScript();
            List<GalgameAction> galgameActions = new List<GalgameAction>();

            GalgameAction galgameAction = null;
            PSelector nowSelector = null;
            PSelectorOption nowSelectorOption = null;
            PGameValuesAdjuster nowAdjuster = null;
            PJudge nowJudge = null;
            PBattle nowBattle = null;
            List<GameValues> groupStack = new List<GameValues>();
            bool isWrappedByActionTag = false;

            foreach (KsScriptLine ksScriptLine in ksScript)
            {
                /**
                 * TODO:
                 * 2. 调研转换效果 transform
                 */
                GalgameKsScriptTag tag = ksScriptLine.tag;
                // if no such tag, just skip
                if (!Enum.IsDefined(typeof(GalgameKsScriptTag), tag)) continue;
                string tagNameT = Enum.GetName(typeof(GalgameKsScriptTag), GalgameKsScriptTag.BG);
                // value of tag
                string tagName = Enum.GetName(typeof(GalgameKsScriptTag), tag);
                int tagValue = (int)tag;
                int tagType = Mathf.FloorToInt(tagValue / 100);

                // properties on tag
                List<KsScriptLineProperty> props = ksScriptLine.props;
                GalgameKsScriptTagProperty ksTagProperty = new GalgameKsScriptTagProperty();

                // if is a empty tag
                if (props.Count == 0)
                {
                    switch (tag)
                    {
                        case GalgameKsScriptTag.SELECT:
                            nowSelector = null;
                            break;
                        case GalgameKsScriptTag.OPTION:
                            nowSelector.Options.Add(nowSelectorOption);
                            nowSelectorOption = null;
                            break;
                        case GalgameKsScriptTag.ACTION:
                            galgameAction = null;
                            isWrappedByActionTag = false;
                            break;
                        case GalgameKsScriptTag.ADJUSTER:
                            nowAdjuster = null;
                            break;
                        case GalgameKsScriptTag.JUDGE:
                            nowJudge = null;
                            break;
                        case GalgameKsScriptTag.BATTLE:
                            nowBattle = null;
                            break;
                        case GalgameKsScriptTag.GROUP:
                            if(groupStack.Count > 0)
                            {
                                if(null != nowJudge)
                                {
                                    nowJudge.MeetGameValues.Add(groupStack.Last());
                                }
                                groupStack.Remove(groupStack.Last());
                            }
                            break;
                    }
                    continue;
                };

                // Global property tags
                switch (tagType)
                {
                    case 0:
                        switch (tag)
                        {
                            case GalgameKsScriptTag.STYLE:

                                foreach (KsScriptLineProperty prop in props)
                                {
                                    string propName = prop.name.ToLower();
                                    string propValue = prop.value.Trim();
                                    // Set properties
                                    switch (propName)
                                    {
                                        case "width":
                                            DefaultScriptProperty.width = Convert.ToSingle(propValue);
                                            break;
                                        case "height":
                                            DefaultScriptProperty.height = Convert.ToSingle(propValue);
                                            break;
                                        case "top":
                                            DefaultScriptProperty.top = Convert.ToSingle(propValue);
                                            break;
                                        case "left":
                                            DefaultScriptProperty.left = Convert.ToSingle(propValue);
                                            break;
                                        case "visible":
                                            DefaultScriptProperty.visible = Convert.ToBoolean(propValue);
                                            break;
                                        case "layer":
                                            DefaultScriptProperty.layer = (Layer)Enum.Parse(typeof(Layer), propValue.ToUpper());
                                            break;
                                        case "method":
                                            DefaultScriptProperty.method = propValue;
                                            break;
                                        case "canskip":
                                            DefaultScriptProperty.canskip = Convert.ToBoolean(propValue);
                                            break;
                                        case "time":
                                            DefaultScriptProperty.time = Convert.ToInt32(propValue);
                                            break;
                                        case "linespacing":
                                            DefaultScriptProperty.linespacing = Convert.ToSingle(propValue);
                                            break;
                                        case "align":
                                            DefaultScriptProperty.align = (Align)Enum.Parse(typeof(Align), propValue.ToUpper());
                                            break;
                                        case "fcolor":
                                            DefaultScriptProperty.fcolor = propValue;
                                            break;
                                        case "fsize":
                                            DefaultScriptProperty.fsize = Convert.ToSingle(propValue);
                                            break;
                                        case "fstyle":
                                            DefaultScriptProperty.fstyle = (FontStyle)Enum.Parse(typeof(FontStyle), propValue);
                                            break;
                                        case "ffamily":
                                            DefaultScriptProperty.ffamily = propValue;
                                            break;
                                    }
                                }
                                break;
                            case GalgameKsScriptTag.TRAN:
                                break;
                            case GalgameKsScriptTag.POS:
                                break;
                            case GalgameKsScriptTag.EFFECT:
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        foreach (KsScriptLineProperty prop in props)
                        {
                            string propName = prop.name.ToLower();
                            string propValue = prop.value.Trim();
                            // Set properties
                            switch (propName)
                            {
                                case "id":
                                    ksTagProperty.id = propValue;
                                    break;
                                case "nextactionid":
                                    ksTagProperty.next_action_id = propValue;
                                    break;
                                case "previousactionid":
                                    ksTagProperty.previous_action_id = propValue;
                                    break;
                                case "tag":
                                    ksTagProperty.tag = propValue;
                                    break;
                                case "name":
                                    ksTagProperty.name = propValue;
                                    break;
                                case "value":
                                    ksTagProperty.value = propValue;
                                    break;
                                case "width":
                                    ksTagProperty.width = Convert.ToSingle(propValue);
                                    break;
                                case "height":
                                    ksTagProperty.height = Convert.ToSingle(propValue);
                                    break;
                                case "top":
                                    ksTagProperty.top = Convert.ToSingle(propValue);
                                    break;
                                case "left":
                                    ksTagProperty.left = Convert.ToSingle(propValue);
                                    break;
                                case "visible":
                                    ksTagProperty.visible = Convert.ToBoolean(propValue);
                                    break;
                                case "layer":
                                    ksTagProperty.layer = (Layer)Enum.Parse(typeof(Layer), propValue.ToUpper());
                                    break;
                                case "method":
                                    ksTagProperty.method = propValue;
                                    break;
                                case "canskip":
                                    ksTagProperty.canskip = Convert.ToBoolean(propValue);
                                    break;
                                case "time":
                                    ksTagProperty.time = Convert.ToInt32(propValue);
                                    break;
                                case "linespacing":
                                    ksTagProperty.linespacing = Convert.ToSingle(propValue);
                                    break;
                                case "align":
                                    ksTagProperty.align = (Align)Enum.Parse(typeof(Align), propValue.ToUpper());
                                    break;
                                case "fcolor":
                                    ksTagProperty.fcolor = propValue;
                                    break;
                                case "fsize":
                                    ksTagProperty.fsize = Convert.ToSingle(propValue);
                                    break;
                                case "fstyle":
                                    ksTagProperty.fstyle = (FontStyle)Enum.Parse(typeof(FontStyle), propValue);
                                    break;
                                case "ffamily":
                                    ksTagProperty.ffamily = propValue;
                                    break;
                                case "src":
                                    if ("BG".Equals(tag)) ksTagProperty.bgsrc = propValue;
                                    if ("FG".Equals(tagName)) ksTagProperty.fgsrc = propValue;
                                    else if (tagName.Contains("BGM")) ksTagProperty.bgmsrc = propValue;
                                    else if (tagName.Contains("VIDEO")) ksTagProperty.videosrc = propValue;
                                    // tagName = null;
                                    break;
                                case "volume":
                                    ksTagProperty.volume = Convert.ToSingle(propValue);
                                    break;
                                case "loop":
                                    ksTagProperty.loop = Convert.ToBoolean(propValue);
                                    break;
                                case "action":
                                    ksTagProperty.action = propValue;
                                    break;
                                case "actor":
                                    ksTagProperty.actor = propValue;
                                    break;
                                case "voice":
                                    ksTagProperty.voice = propValue;
                                    break;
                                case "line":
                                    ksTagProperty.line = propValue;
                                    break;
                                case "anim":
                                    ksTagProperty.anim = propValue;
                                    break;
                                case "type":
                                    if (GalgameKsScriptTag.SELECT == tag) ksTagProperty.selector_type = propValue;
                                    // if ("SELECT".Equals(tagName)) ksTagProperty.selector_type = propValue;
                                    break;
                                case "text":
                                    if (GalgameKsScriptTag.SELECT == tag) ksTagProperty.selector_text = propValue;
                                    if (GalgameKsScriptTag.OPTION == tag) ksTagProperty.option_text = propValue;
                                    // if ("SELECT".Equals(tagName)) ksTagProperty.selector_text = propValue;
                                    // if ("OPTION".Equals(tagName)) ksTagProperty.option_text = propValue;
                                    break;
                                case "bg":
                                    if (GalgameKsScriptTag.SELECT == tag) ksTagProperty.selector_bg = propValue;
                                    if (GalgameKsScriptTag.OPTION == tag) ksTagProperty.option_bg = propValue;
                                    if (GalgameKsScriptTag.LINE == tag) ksTagProperty.line_bg = propValue;
                                    // if ("SELECT".Equals(tagName)) ksTagProperty.selector_bg = propValue;
                                    // if ("OPTION".Equals(tagName)) ksTagProperty.option_bg = propValue;
                                    // if ("LINE".Equals(tagName)) ksTagProperty.line_bg = propValue;
                                    break;
                                case "bgm":
                                    if (GalgameKsScriptTag.SELECT == tag) ksTagProperty.selector_bgm = propValue;
                                    if (GalgameKsScriptTag.OPTION == tag) ksTagProperty.option_bgm = propValue;
                                    if (GalgameKsScriptTag.LINE == tag) ksTagProperty.line_bgm = propValue;
                                    //if ("SELECT".Equals(tagName)) ksTagProperty.selector_bgm = propValue;
                                    //if ("OPTION".Equals(tagName)) ksTagProperty.option_bgm = propValue;
                                    //if ("LINE".Equals(tagName)) ksTagProperty.line_bgm = propValue;
                                    break;
                            }
                        }

                        if (tagType == 1)
                        {
                            if (GalgameKsScriptTag.CHS.Equals(tag))
                            {
                                if (null != ksTagProperty.bgmsrc) galgameScript.Bgm = (AudioClip)Resources.Load("Audio/" + ksTagProperty.bgmsrc, typeof(AudioClip));
                                if (null != ksTagProperty.bgsrc) galgameScript.Bg = (Sprite)Resources.Load("Sprite/" + ksTagProperty.bgsrc, typeof(Sprite));
                                if (null != ksTagProperty.name) galgameScript.ChapterName = ksTagProperty.name;
                                if (null != ksTagProperty.value) galgameScript.ChapterAbstract = ksTagProperty.value;
                            }
                            continue;
                        }

                        // if (null == galgameAction) galgameAction = new GalgameAction();

                        if(BLOCK_TYPE_TAGS.Contains(tag))
                        {
                            ForceCloseAllBlockTag(nowSelector, nowAdjuster, nowJudge, nowBattle);
                            // If these block-type elements are not wrapped by [action] tag, generate a new action autoly
                            if (!isWrappedByActionTag)
                            {
                                galgameAction = new GalgameAction();
                            }
                        }
                        
                        switch(tag)
                        {
                            case GalgameKsScriptTag.ACTION:
                                isWrappedByActionTag = true;
                                galgameAction = new GalgameAction();
                                break;
                            case GalgameKsScriptTag.SELECT:
                                if(!string.IsNullOrEmpty(ksTagProperty.selector_type))
                                {
                                    nowSelector = new PSelector()
                                    {
                                        Id = ksTagProperty.id,
                                        Type = (SelectorType)Enum.Parse(typeof(SelectorType), ksTagProperty.selector_type.ToUpper())
                                    };

                                    nowSelector.IsSelected = false;
                                    nowSelector.SelectedItem = -1;
                                    nowSelector.Options = new List<PSelectorOption>();

                                    galgameAction.Selector = nowSelector;
                                }
                                break;
                            case GalgameKsScriptTag.ADJUSTER:
                                nowAdjuster = new PGameValuesAdjuster()
                                {
                                    Id = ksTagProperty.id
                                };
                                nowAdjuster.DeltaGameValues = new GameValues()
                                {
                                    RoleAbility = new RoleAbility(),
                                    RoleStatus = new RoleStatus()
                                };
                                galgameAction.GameValuesAdjuster = nowAdjuster;
                                break;
                            case GalgameKsScriptTag.JUDGE:
                                if (!string.IsNullOrEmpty(ksTagProperty.next_action_id))
                                {
                                    nowJudge = new PJudge()
                                    {
                                        Id = ksTagProperty.id,
                                        NextActionId = ksTagProperty.next_action_id
                                    };
                                    nowJudge.MeetGameValues = new List<GameValues>();
                                    galgameAction.Judge = nowJudge;
                                }
                                break;
                            case GalgameKsScriptTag.BATTLE:
                                nowBattle = new PBattle()
                                {
                                    Id = ksTagProperty.id
                                };
                                galgameAction.Battle = nowBattle;
                                break;
                            case GalgameKsScriptTag.GROUP:
                                groupStack.Add(new GameValues()
                                {
                                    RoleAbility = new RoleAbility(),
                                    RoleStatus = new RoleStatus()
                                });
                                break;
                        }
                        if (null != nowSelector && GalgameKsScriptTag.OPTION.Equals(tag))
                        {
                            nowSelectorOption = new PSelectorOption();
                            nowSelectorOption.Actions = new List<GalgamePlainAction>();
                            nowSelectorOption.DeltaGameValues = new GameValues();
                            nowSelectorOption.Bg = (Sprite)Resources.Load("Sprite/" + ksTagProperty.option_bg, typeof(Sprite));
                            nowSelectorOption.Bgm = (AudioClip)Resources.Load("Audio/" + ksTagProperty.option_bgm, typeof(AudioClip));
                            nowSelectorOption.Text = new PText()
                            {
                                text = ksTagProperty.option_text.Trim(),
                                ffamily = ksTagProperty.ffamily,
                                fcolor = ksTagProperty.fcolor,
                                fsize = ksTagProperty.fsize,
                                linespacing = ksTagProperty.linespacing,
                                align = ksTagProperty.align,
                                fstyle = ksTagProperty.fstyle
                            };
                        }
                        if (GalgameKsScriptTag.PAIR == tag)
                        {
                            if (null != nowSelectorOption && !string.IsNullOrEmpty(ksTagProperty.name) && !string.IsNullOrEmpty(ksTagProperty.value))
                            {
                                EntityUtil.SetDeepValue(nowSelectorOption.DeltaGameValues, ksTagProperty.name, ksTagProperty.value);
                            }
                            if (null != nowAdjuster && !string.IsNullOrEmpty(ksTagProperty.name) && !string.IsNullOrEmpty(ksTagProperty.value))
                            {
                                EntityUtil.SetDeepValue(nowAdjuster.DeltaGameValues, ksTagProperty.name, ksTagProperty.value);
                            }
                            if(null != nowJudge && !string.IsNullOrEmpty(ksTagProperty.name) && !string.IsNullOrEmpty(ksTagProperty.value))
                            {
                                EntityUtil.SetDeepValue(groupStack.Last(), ksTagProperty.name, ksTagProperty.value);
                            }
                        }
                        if(null != galgameAction)
                        {
                            if (null != ksTagProperty.line)
                            {
                                galgameAction.Lines.Add(new GalgameScriptLine()
                                {
                                    text = ksTagProperty.line,
                                    ffamily = ksTagProperty.ffamily,
                                    fcolor = ksTagProperty.fcolor,
                                    fsize = ksTagProperty.fsize,
                                    linespacing = ksTagProperty.linespacing,
                                    align = ksTagProperty.align,
                                    fstyle = ksTagProperty.fstyle
                                });

                                if(null == galgameAction.Line)
                                {
                                    galgameAction.Line = galgameAction.Lines[0];
                                }
                            }
                            if (null != ksTagProperty.videosrc) galgameAction.Video = (VideoClip)Resources.Load("Video/" + ksTagProperty.videosrc, typeof(VideoClip));
                            if (null != ksTagProperty.bgmsrc) galgameAction.Bgm = (AudioClip)Resources.Load("Audio/" + ksTagProperty.bgmsrc, typeof(AudioClip));
                            if (null != ksTagProperty.voice) galgameAction.Voice = (AudioClip)Resources.Load("Audio/" + ksTagProperty.voice, typeof(AudioClip));
                            if (null != ksTagProperty.bgsrc) galgameAction.Background = (Sprite)Resources.Load("Sprite/" + ksTagProperty.bgsrc, typeof(Sprite));
                            // A BG and BGM set int line property have a higher priority than set in tag [bg] and [bgm]
                            if (null != ksTagProperty.line_bg) galgameAction.Background = (Sprite)Resources.Load("Sprite/" + ksTagProperty.line_bg, typeof(Sprite));
                            if (null != ksTagProperty.line_bgm) galgameAction.Bgm = (AudioClip)Resources.Load("Audio/" + ksTagProperty.line_bgm, typeof(AudioClip));
                            if (null != ksTagProperty.actor) galgameAction.Actor = (Actor)Enum.Parse(typeof(Actor), ksTagProperty.actor);
                            if (null != ksTagProperty.anim) galgameAction.ActorAnimation = ksTagProperty.anim;
                            if (null != nowSelector && null != nowSelectorOption)
                            {
                                if (galgameAction.Actor != Actor.NULL)
                                {
                                    nowSelectorOption.Actions.Add(new GalgamePlainAction()
                                    {
                                        Actor = galgameAction.Actor,
                                        ActorAnimation = galgameAction.ActorAnimation,
                                        Background = galgameAction.Background,
                                        Bgm = galgameAction.Bgm,
                                        Line = galgameAction.Line,
                                        Video = galgameAction.Video,
                                        Voice = galgameAction.Voice,
                                        Input = galgameAction.Input
                                    });
                                }
                            }
                            else if(null != galgameAction && !NOT_ACTION_TAGS.Contains(tag))
                            {
                                galgameActions.Add(galgameAction);
                            }
                        }
                        
                        break;
                }
            }
            if(null != galgameActions)
            {
                galgameScript.GalgameActions = galgameActions;
            }
            return galgameScript;
        }

        /// <summary>
        /// Force to close all block-type tag
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="adjuster"></param>
        /// <param name="judge"></param>
        /// <param name="pBattle"></param>
        private static void ForceCloseAllBlockTag(PSelector selector, PGameValuesAdjuster adjuster, PJudge judge, PBattle battle)
        {
            if (null != selector)
            {
                selector = null;
            }
            if (null != adjuster)
            {
                adjuster = null;
            }
            if (null != judge)
            {
                judge = null;
            }
            if (null != battle)
            {
                battle = null;
            }
        }
    }
}
#endif
