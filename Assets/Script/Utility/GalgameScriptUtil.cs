#if UNITY_EDITOR
using Assets.Script.Model;
using Assets.Script.Model.Components;
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

            // Is now in a [select] element
            PSelector nowSelector = null;
            PSelectorOption nowSelectorOption = null;

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
                    if ("SELECT".Equals(tagName)) nowSelector = null;
                    if ("OPTION".Equals(tagName)) {
                        nowSelector.Options.Add(nowSelectorOption);
                        nowSelectorOption = null;
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
                                    if ("BG".Equals(tagName)) ksTagProperty.bgsrc = propValue;
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
                                    if ("SELECT".Equals(tagName)) ksTagProperty.selector_type = propValue;
                                    break;
                                case "text":
                                    if ("SELECT".Equals(tagName)) ksTagProperty.selector_text = propValue;
                                    if ("OPTION".Equals(tagName)) ksTagProperty.option_text = propValue;
                                    break;
                                case "bg":
                                    if ("SELECT".Equals(tagName)) ksTagProperty.selector_bg = propValue;
                                    if ("OPTION".Equals(tagName)) ksTagProperty.option_bg = propValue;
                                    if ("LINE".Equals(tagName)) ksTagProperty.line_bg = propValue;
                                    break;
                                case "bgm":
                                    if ("SELECT".Equals(tagName)) ksTagProperty.selector_bgm = propValue;
                                    if ("OPTION".Equals(tagName)) ksTagProperty.option_bgm = propValue;
                                    if ("LINE".Equals(tagName)) ksTagProperty.line_bgm = propValue;
                                    break;
                            }
                        }

                        if (tagType == 1)
                        {
                            if ("CHS".Equals(tagName))
                            {
                                if (null != ksTagProperty.bgmsrc) galgameScript.Bgm = (AudioClip)Resources.Load("Audio/" + ksTagProperty.bgmsrc, typeof(AudioClip));
                                if (null != ksTagProperty.bgsrc) galgameScript.Bg = (Sprite)Resources.Load("Sprite/" + ksTagProperty.bgsrc, typeof(Sprite));
                                if (null != ksTagProperty.name) galgameScript.ChapterName = ksTagProperty.name;
                                if (null != ksTagProperty.value) galgameScript.ChapterAbstract = ksTagProperty.value;
                            }
                            continue;
                        }

                        galgameAction = new GalgameAction();
                        if (null != ksTagProperty.line)
                        {
                            galgameAction.Line = new GalgameScriptLine()
                            {
                                text = ksTagProperty.line,
                                ffamily = ksTagProperty.ffamily,
                                fcolor = ksTagProperty.fcolor,
                                fsize = ksTagProperty.fsize,
                                linespacing = ksTagProperty.linespacing,
                                align = ksTagProperty.align,
                                fstyle = ksTagProperty.fstyle
                            };
                        }
                        if ("SELECT".Equals(tagName))
                        {
                            nowSelector = new PSelector()
                            {
                                Type = (SelectorType)Enum.Parse(typeof(SelectorType), ksTagProperty.selector_type.ToUpper())
                            };

                            nowSelector.IsSelected = false;
                            nowSelector.SelectedItem = -1;
                            nowSelector.Options = new List<PSelectorOption>();

                            galgameAction.Selector = nowSelector;
                        }
                        if (null != nowSelector && "OPTION".Equals(tagName))
                        {
                            nowSelectorOption = new PSelectorOption();
                            nowSelectorOption.Actions = new List<GalgamePlainAction>();
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
                            if(galgameAction.Actor != Actor.NULL)
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
                        else
                        {
                            galgameActions.Add(galgameAction);
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
    }
}
#endif
