using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Video;

namespace Assets.Script.Chapter
{
    public class GameController : MonoBehaviour
    {
        public GameObject background;
        public GameObject displayCanvas;
        public GameObject videoPlayer;
        public GameObject audioSource;

        private SpriteRenderer bg;
        private GameObject titleContainer;
        private GameObject lineContainer;
        private GameObject historyField;
        private GameObject savedDataField;
        private GameObject settingField;
        private VideoPlayer _video;
        private AudioSource _audio;
        private AudioClip bgmMusic;

        void Awake()
        {
            bg = background.transform.Find("Bg").gameObject.GetComponent<SpriteRenderer>();
            titleContainer = displayCanvas.transform.Find("TitleContainer").gameObject;
            lineContainer = displayCanvas.transform.Find("LineContainer").gameObject;
            historyField = displayCanvas.transform.Find("HistoryField").gameObject;
            savedDataField = displayCanvas.transform.Find("SavedDataField").gameObject;
            settingField = displayCanvas.transform.Find("SettingField").gameObject;
            _video = videoPlayer.GetComponent<VideoPlayer>();
            _audio = audioSource.GetComponent<AudioSource>();

            bgmMusic = (AudioClip)Resources.Load("Audio/BGM30", typeof(AudioClip));
            _audio.clip = bgmMusic;
            _audio.loop = true;
            _audio.Play();
        }

        /// <summary>
        /// Play game from the very start
        /// </summary>
        public void NewGame()
        {
            bg.sprite = null;
            _audio.Stop();
            Resources.UnloadUnusedAssets();
            titleContainer.SetActive(false);
        }

        /// <summary>
        /// Shut down game immediately
        /// </summary>
        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
