using Assets.Script.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
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
        public GameObject bgmAudioSource;
        public GameObject voiceAudioSource;

        private SpriteRenderer bg;
        private GameObject titleContainer;
        private GameObject lineContainer;
        private GameObject historyField;
        private GameObject savedDataField;
        private GameObject settingField;
        private GameObject popupWindow;
        private VideoPlayer _video;
        private AudioSource _bgmAudio;
        private AudioSource _voiceAudio;
        private AudioClip bgmMusic;

        private Text popupCallbackText;

        public static string rootPath;
        public static string savedDataPath;
        public static string savedDataFile;
        public static List<SavedDataModel> savedDatas;
        public static int savdDataPageCount;
        public static int lastLoadedSavedDataPage;

        // Setting: Duration of text showing speed, in seconds
        public static float textShowDuration = 0.1f;
        // Setting: Duration of line auto switch speed, in seconds
        public static float lineSwitchDuration = 3.0f;
        // Setting: Duration of line switch speed under skip mode, in seconds
        public static float skipModeLineSwitchDuration = 0.1f;

        void Awake()
        {
            bg = background.transform.Find("Bg").gameObject.GetComponent<SpriteRenderer>();
            titleContainer = displayCanvas.transform.Find("TitleContainer").gameObject;
            lineContainer = displayCanvas.transform.Find("LineContainer").gameObject;
            historyField = displayCanvas.transform.Find("HistoryField").gameObject;
            savedDataField = displayCanvas.transform.Find("SavedDataField").gameObject;
            settingField = displayCanvas.transform.Find("SettingField").gameObject;
            popupWindow = displayCanvas.transform.Find("PopupWindow").gameObject;
            popupCallbackText = popupWindow.transform.Find("Popup").Find("CallbackMethod").gameObject.GetComponent<Text>();
            _video = videoPlayer.GetComponent<VideoPlayer>();
            _bgmAudio = bgmAudioSource.GetComponent<AudioSource>();
            _voiceAudio = voiceAudioSource.GetComponent<AudioSource>();

            bgmMusic = (AudioClip)Resources.Load("Audio/BGM30", typeof(AudioClip));

            rootPath = Application.dataPath;
            savedDataPath = rootPath + "/Resources/SavedData/";
            savedDataFile = "savedata.dat";
            savdDataPageCount = 10;
            lastLoadedSavedDataPage = 0;
            savedDatas = LoadSavedDatas();

            _bgmAudio.clip = bgmMusic;
            _bgmAudio.loop = true;
            _bgmAudio.Play();
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

        #region Title Menu Callback
        /// <summary>
        /// Play game from the very start
        /// </summary>
        public void NewGame()
        {
            bg.sprite = null;
            _bgmAudio.Stop();
            DeactiveGameObject(titleContainer);
            ReleaseMemory();
        }

        /// <summary>
        /// Shut down game immediately
        /// </summary>
        public void QuitGame()
        {
            Application.Quit();
        }
        #endregion

        #region Menu Callback
        /// <summary>
        /// Return to the title screen
        /// </summary>
        public void BackToTitle()
        {
            bg.sprite = Resources.Load<Sprite>("Sprite/STT_BG00");
            // Deactive display objects
            DeactiveGameObject(lineContainer);
            DeactiveGameObject(historyField);
            DeactiveGameObject(savedDataField);
            DeactiveGameObject(settingField);
            DeactiveGameObject(popupWindow);
            // Active title menu
            ActiveGameObject(titleContainer);
            // Release menory
            ReleaseMemory();
        }
        #endregion

        #region Saved data
        /// <summary>
        /// Save a saveddata at index of `index` and write to file [savedata.dat]
        /// </summary>
        /// <param name="savedData">This data model to be saved</param>
        /// <returns></returns>
        public bool PersistSavedDatas()
        {
            try
            {
                if (!Directory.Exists(savedDataPath))
                {
                    Directory.CreateDirectory(savedDataPath);
                }
                using (StreamWriter w = new StreamWriter(savedDataPath + savedDataFile, false, Encoding.UTF8))
                {
                    // TODO: This convertion will fail as the SavedDataModel contains
                    string savedDataJson = JsonConvert.SerializeObject(savedDatas);
                    Debug.Log(savedDataJson);
                    w.Write(savedDataJson);
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Load saved game data from saveddata file
        /// </summary>
        /// <returns>Return saved datas list, including null value</returns>
        public List<SavedDataModel> LoadSavedDatas()
        {
            if (!Directory.Exists(savedDataPath))
            {
                Directory.CreateDirectory(savedDataPath);
            }
            using (FileStream fs = new FileStream(savedDataPath + savedDataFile, FileMode.OpenOrCreate))
            {
                using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
                {
                    string savedDataJson = sr.ReadToEnd();
                    if (null == savedDatas) savedDatas = InitList<SavedDataModel>(savdDataPageCount * 12);
                    if (!string.IsNullOrEmpty(savedDataJson))
                    {
                        JArray savedDataJArray = JArray.Parse(savedDataJson);
                        foreach (JToken jSavedData in savedDataJArray)
                        {
                            if (null != jSavedData && jSavedData.Type != JTokenType.Null)
                            {
                                SavedDataModel thisModel = JsonConvert.DeserializeObject<SavedDataModel>(jSavedData.ToString());
                                savedDatas[thisModel.savedDataIndex] = thisModel;
                            }
                        }
                        return savedDatas;
                    }
                }
            }
            return InitList<SavedDataModel>(savdDataPageCount * 12);
        }

        /// <summary>
        /// Load a saved data from the specific `index` for all saved datas
        /// </summary>
        /// <param name="index">The specific index</param>
        /// <returns>Return null if data in the `index` is null</returns>
        public SavedDataModel LoadSavedData(int index)
        {
            return savedDatas[index];
        }
        #endregion

        #region Popup Window
        /// <summary>
        /// Open popup window with comfirm callback
        /// </summary>
        /// <param name="callback">The comfirm callback</param>
        public void OpenPopup(string callback)
        {
            popupCallbackText.text = callback.Trim();
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
        /// popup window confirm button click listener
        /// </summary>
        public void PopupCallback()
        {
            switch(popupCallbackText.text)
            {
                case "QuitGame":
                    QuitGame();
                    break;
                default:
                    break;
            }
        }
        #endregion

        /// <summary>
        /// Request full screen mode
        /// </summary>
        public void FullScreen()
        {
            Screen.fullScreen = true;
        }

        /// <summary>
        /// Set volume of BGM to mute or not
        /// </summary>
        /// <param name="mute"></param>
        public void SetBgmMute(bool mute)
        {
            _bgmAudio.mute = mute;
        }

        /// <summary>
        /// Set volume of Voices to mute or not
        /// </summary>
        /// <param name="mute"></param>
        public void SetVoicesMute(bool mute)
        {
            _voiceAudio.mute = mute;
        }

        /// <summary>
        /// Set volume of BGM
        /// </summary>
        /// <param name="volume">The volume value in float</param>
        public void SetBgmVolume(float volume)
        {
            _bgmAudio.volume = volume;
        }

        /// <summary>
        /// Set volume of Voices
        /// </summary>
        /// <param name="volume">The volume value in float</param>
        public void SetVoicesVolume(float volume)
        {
            _voiceAudio.volume = volume;
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

        /// <summary>
        /// Init a empty <see cref="List"/> with size
        /// </summary>
        /// <typeparam name="T">Type of list</typeparam>
        /// <param name="size">length of list</param>
        /// <returns></returns>
        public List<T> InitList<T>(int size)
        {
            List<T> newList = new List<T>();
            if (size > 0)
            {
                for (int i = 0; i < size; i++)
                {
                    newList.Add(default(T));
                }
            }
            return newList;
        }
    }
}
