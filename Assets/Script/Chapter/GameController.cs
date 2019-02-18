using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
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
        private GameObject popupWindow;
        private VideoPlayer _video;
        private AudioSource _audio;
        private AudioClip bgmMusic;

        private Text popupCallbackText;

        void Awake()
        {
            bg = background.transform.Find("Bg").gameObject.GetComponent<SpriteRenderer>();
            titleContainer = displayCanvas.transform.Find("TitleContainer").gameObject;
            lineContainer = displayCanvas.transform.Find("LineContainer").gameObject;
            historyField = displayCanvas.transform.Find("HistoryField").gameObject;
            savedDataField = displayCanvas.transform.Find("SavedDataField").gameObject;
            settingField = displayCanvas.transform.Find("SettingField").gameObject;
            popupWindow = displayCanvas.transform.Find("PopupWindow").gameObject;
            popupCallbackText = popupWindow.transform.Find("CallbackMethod").gameObject.GetComponent<Text>();
            _video = videoPlayer.GetComponent<VideoPlayer>();
            _audio = audioSource.GetComponent<AudioSource>();

            bgmMusic = (AudioClip)Resources.Load("Audio/BGM30", typeof(AudioClip));
            _audio.clip = bgmMusic;
            _audio.loop = true;
            _audio.Play();
        }


        /// <summary>
        /// Clear all display items in screen and show CG picture
        /// </summary>
        public void ShowCG()
        {
            DeactiveGameObject(titleContainer);
            DeactiveGameObject(lineContainer);
            DeactiveGameObject(historyField);
            DeactiveGameObject(savedDataField);
            DeactiveGameObject(settingField);
            DeactiveGameObject(popupWindow);
        }

        #region Title Screen
        /// <summary>
        /// Play game from the very start
        /// </summary>
        public void NewGame()
        {
            bg.sprite = null;
            _audio.Stop();
            ReleaseMemory();
            titleContainer.SetActive(false);
        }

        /// <summary>
        /// Shut down game immediately
        /// </summary>
        public void QuitGame()
        {
            Application.Quit();
        }
        #endregion

        /// <summary>
        /// Open popup window with comfirm callback
        /// </summary>
        /// <param name="callback">The comfirm callback</param>
        public void OpenPopup(string callback)
        {
            popupCallbackText.text = callback;
            ActiveGameObject(popupWindow);
        }

        /// <summary>
        /// Close popup window
        /// </summary>
        public void ClosePopup()
        {
            DeactiveGameObject(popupWindow);
        }

        /// <summary>
        /// Return to the title screen
        /// </summary>
        public void BackToTitle()
        {
            bg.sprite = Resources.Load<Sprite>("Sprite/STT_BG00");
            // Release menory
            ReleaseMemory();
            // Deactive display objects
            DeactiveGameObject(lineContainer);
            DeactiveGameObject(historyField);
            DeactiveGameObject(savedDataField);
            DeactiveGameObject(settingField);
            DeactiveGameObject(popupWindow);
            // Active title menu
            ActiveGameObject(titleContainer);
        }

        /// <summary>
        /// To active a gameobject
        /// </summary>
        /// <param name="gameObject">The target object</param>
        public void ActiveGameObject(GameObject gameObject)
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// To deactive a gameobject
        /// </summary>
        /// <param name="gameObject">The target object</param>
        public void DeactiveGameObject(GameObject gameObject)
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Release RAM
        /// </summary>
        public void ReleaseMemory()
        {
            Resources.UnloadUnusedAssets();
            GC.Collect();
        }
    }
}
