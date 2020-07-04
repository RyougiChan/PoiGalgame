#if UNITY_EDITOR
using Assets.Script.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Assets.Script.Utility
{
    /// <summary>
    /// Galgame script translator
    /// </summary>
    public class GalgameScriptTranslator : MonoBehaviour
    {
        public static void KsToAsset(string ksFileFullPath)
        {
            KsToAsset(ksFileFullPath, null);
        }
        /// <summary>
        /// Translator to translate .ks file to Unity asset
        /// </summary>
        /// <param name="ksFileFullPath"></param>
        /// <param name="assetFileFullPath"></param>
        /// <returns></returns>
        public static void KsToAsset(string ksFileFullPath, string assetFileFullPath)
        {
            if (File.Exists(ksFileFullPath))
            {
                using (FileStream kfs = new FileStream(ksFileFullPath, FileMode.Open, FileAccess.Read))
                {
                    using (StreamReader reader = new StreamReader(kfs))
                    {
                        // All lines in ksFileFullPath
                        List<KsScriptLine> ksScriptLines = new List<KsScriptLine>();
                        Regex tagRegex = new Regex(@"\[(\w+)", RegexOptions.IgnoreCase);
                        Regex propRegex = new Regex(@"(\w+?)=""(.*?)""", RegexOptions.IgnorePatternWhitespace);
                        string currentLine = string.Empty;
                        string tagType = string.Empty;
                        string propName = string.Empty;
                        string propValue = string.Empty;
                        KsScriptLine line = null;
                        List<KsScriptLineProperty> propPairs;
                        // Analysis | 解析
                        while (null != (currentLine = reader.ReadLine()))
                        {
                            // Skip Empty line
                            if (string.IsNullOrEmpty(currentLine.Trim())) continue;
                            line = new KsScriptLine();
                            propPairs = new List<KsScriptLineProperty>();
                            // Tag | 标签类型
                            if (tagRegex.IsMatch(currentLine))
                            {
                                tagType = tagRegex.Match(currentLine).Groups[1].Value.ToUpper();
                            }
                            // Properties | 属性
                            if (propRegex.IsMatch(currentLine))
                            {
                                foreach (Match propMatch in propRegex.Matches(currentLine))
                                {
                                    propName = propMatch.Groups[1].Value;
                                    propValue = propMatch.Groups[2].Value;
                                    propPairs.Add(new KsScriptLineProperty() { name = propName, value = propValue });
                                }
                            }
                            line.tag = (GalgameKsScriptTag)Enum.Parse(typeof(GalgameKsScriptTag), tagType);
                            line.props = propPairs;
                            ksScriptLines.Add(line);
                        }
                        // Convert to GalgameScript
                        GalgameScript gs = GalgameScriptUtil.KsScriptToGalgameScript(ksScriptLines);
                        string fileName = new FileInfo(ksFileFullPath).Name;
                        fileName = fileName.Substring(0, fileName.IndexOf(".")) + ".asset";
                        GalgameScriptUtil.CreateGalgameScriptAsset(gs, fileName, assetFileFullPath);
                    }
                }
            }
        }
    }
}
#endif