using System;
using UnityEngine;

namespace Assets.Scripts.Model
{
    [Serializable]
    public class GalgameScriptProps
    {
        public string Line;
        public AudioClip Bgm;
        public AudioClip Voice;
        public Sprite Background;
        public CharacterSelect Character;

        public enum CharacterSelect
        {
            曉,
            七海,
            羽月,
        }
    }
}
