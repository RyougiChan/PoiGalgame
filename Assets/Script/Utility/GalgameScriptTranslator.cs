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

        /// <summary>
        /// Translator to translate .ks file to Unity asset
        /// </summary>
        /// <param name="ksFileFullPath"></param>
        /// <param name="assetFileFullPath"></param>
        /// <returns></returns>
        public void KsToAsset(string ksFileFullPath)
        {
            if(File.Exists(ksFileFullPath))
            {
                using (FileStream kfs = new FileStream(ksFileFullPath, FileMode.Open, FileAccess.Read))
                {
                    using (StreamReader reader = new StreamReader(kfs))
                    {
                        // All lines in ksFileFullPath
                        Dictionary<string, Dictionary<string, string>> lines = new Dictionary<string, Dictionary<string, string>>();
                        Regex tagRegex = new Regex(@"\[(\w+)", RegexOptions.IgnoreCase);
                        Regex propRegex = new Regex(@"(\w+?)=""(.*?)""", RegexOptions.IgnorePatternWhitespace);
                        string currentLine = string.Empty;
                        string tagType = string.Empty;
                        string propName = string.Empty;
                        string propValue = string.Empty;
                        Dictionary<string, string> propPairs = new Dictionary<string, string>();
                        // Analysis | 解析
                        while (null != (currentLine = reader.ReadLine()))
                        {
                            // Tag | 标签类型
                            if(tagRegex.IsMatch(currentLine))
                            {
                                tagType = tagRegex.Match(currentLine).Groups[1].Value;
                            }
                            // Properties | 属性
                            if(propRegex.IsMatch(currentLine))
                            {
                                foreach(Match propMatch in propRegex.Matches(currentLine))
                                {
                                    propName = propMatch.Groups[1].Value;
                                    propValue = propMatch.Groups[2].Value;
                                    propPairs.Add(propName, propValue);
                                }
                            }

                            if(propPairs.Count != 0)
                            {
                                propPairs.Clear();
                            }
                        }
                        if(!string.IsNullOrEmpty(tagType))
                        {
                            lines.Add(tagType, propPairs);
                        }
                    }
                }
            }
        }
    }
}
