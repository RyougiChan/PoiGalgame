using Assets.Script.Model;
using Assets.Scripts.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

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
            foreach (KsScriptLine ksScriptLine in ksScript)
            {
                GalgameKsScriptTag tag = ksScriptLine.tag;
                // if no such tag, just skip
                if (!Enum.IsDefined(typeof(GalgameKsScriptTag), tag)) continue;
                // value of tag
                int tagValue = (int)tag;
                int tagType = Mathf.FloorToInt(tagValue / 100);
                // properties on tag
                List<KsScriptLineProperty> props = ksScriptLine.props;
                GalgameKsScriptTagProperty ksTagProperty = new GalgameKsScriptTagProperty();
                foreach (KsScriptLineProperty prop in props)
                {
                    string propName = prop.name.ToLower();
                    string propValue = prop.value;
                    // Set properties
                    switch(propName)
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
                            ksTagProperty.layer = (Layer)Enum.Parse(typeof(Layer), propValue);
                            break;
                        case "method":
                            ksTagProperty.method = propValue;
                            break;
                        case "canskip":
                            ksTagProperty.canskip = Convert.ToBoolean(propValue);
                            break;
                        case "linespacing":
                            ksTagProperty.linespacing = Convert.ToSingle(propValue);
                            break;
                        case "align":
                            ksTagProperty.align = (Align)Enum.Parse(typeof(Align), propValue);
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
                            string tagName = Enum.GetName(typeof(GalgameKsScriptTag), tag);
                            if (tagName.Equals("bg")) ksTagProperty.bgsrc = propValue;
                            if (tagName.Equals("fg")) ksTagProperty.fgsrc = propValue;
                            else if(tagName.Contains("bgm")) ksTagProperty.bgmsrc = propValue;
                            else if(tagName.Contains("video")) ksTagProperty.videosrc = propValue;
                            tagName = null;
                            break;
                        case "volume":
                            ksTagProperty.volume = Convert.ToSingle(propValue);
                            break;
                        case "loop":
                            ksTagProperty.loop = Convert.ToBoolean(propValue);
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
                    }
                }
                galgameAction = new GalgameAction();

                if (null != ksTagProperty.line) galgameAction.Line = ksTagProperty.line;
                if (null != ksTagProperty.bgmsrc) galgameAction.Bgm = (AudioClip)Resources.Load("Audio/" + ksTagProperty.bgmsrc, typeof(AudioClip));
                if (null != ksTagProperty.voice) galgameAction.Voice = (AudioClip)Resources.Load("Audio/" + ksTagProperty.voice, typeof(AudioClip));
                if (null != ksTagProperty.bgsrc) galgameAction.Background = (Sprite)Resources.Load("Sprite/" + ksTagProperty.bgsrc, typeof(Sprite));
                if (null != ksTagProperty.actor) galgameAction.Actor = (Actor)Enum.Parse(typeof(Actor), ksTagProperty.actor);
                if (null != ksTagProperty.anim) galgameAction.ActorAnimation = ksTagProperty.anim;

                galgameActions.Add(galgameAction);
            }
            if(null != galgameActions)
            {
                galgameScript.GalgameActions = galgameActions;
            }
            return galgameScript;
        }
    }
}
