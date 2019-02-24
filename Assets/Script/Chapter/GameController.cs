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
        public bool inGame;

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
        private GameObject menuField;
        private GameObject popupWindow;
        private VideoPlayer _video;
        private AudioSource _bgmAudio;
        private AudioSource _voiceAudio;
        private AudioClip bgmMusic;

        private float startTime;
        private bool isMenuShow;
        private Vector3 defaultMenuLocalPos;
        private Vector3 shownMenuLocalPos;

        private const string DEFAULT_POPUP_NOTICE_TEXT = "本気ですか？";
        private Text popupNoticeText;
        private Text popupCallbackText;

        public static string rootPath;
        public static string savedDataPath;
        public static string savedDataFile;
        public static List<SavedDataModel> savedDatas;
        public static int savdDataPageCount;
        public static int lastLoadedSavedDataPage;
        public static GameObject[] historyQuene;

        void Awake()
        {
            bg = background.transform.Find("Bg").gameObject.GetComponent<SpriteRenderer>();
            titleContainer = displayCanvas.transform.Find("TitleContainer").gameObject;
            lineContainer = displayCanvas.transform.Find("LineContainer").gameObject;
            historyField = displayCanvas.transform.Find("HistoryField").gameObject;
            savedDataField = displayCanvas.transform.Find("SavedDataField").gameObject;
            settingField = displayCanvas.transform.Find("SettingField").gameObject;
            menuField = displayCanvas.transform.Find("MenuField").gameObject;
            popupWindow = displayCanvas.transform.Find("PopupWindow").gameObject;
            popupNoticeText = popupWindow.transform.Find("Popup").Find("NoticeText").gameObject.GetComponent<Text>();
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
            historyQuene = new GameObject[] { null, titleContainer };
            savedDatas = LoadSavedDatas();

            defaultMenuLocalPos = menuField.transform.localPosition;
            shownMenuLocalPos = new Vector3(defaultMenuLocalPos.x, defaultMenuLocalPos.y - menuField.GetComponent<RectTransform>().sizeDelta.y, defaultMenuLocalPos.z);

            _bgmAudio.clip = bgmMusic;
            _bgmAudio.loop = true;
            _bgmAudio.Play();
        }

        void Start()
        {
            startTime = Time.time;
        }

        void FixedUpdate()
        {
            if(inGame)
            {
                if (Input.mousePosition.y > Screen.height - menuField.GetComponent<RectTransform>().sizeDelta.y)
                {
                    MoveMenu(defaultMenuLocalPos, shownMenuLocalPos, (Time.time - startTime) * 5.0f);
                }
                else
                {
                    MoveMenu(shownMenuLocalPos, defaultMenuLocalPos, (Time.time - startTime) * 5.0f);
                }
            }
        }

        /// <summary>
        /// Clear all display items in screen and show CG picture
        /// </summary>
        public void ShowCG()
        {
            titleContainer.SetActive(false);
            lineContainer.SetActive(false);
            historyField.SetActive(false);
            savedDataField.SetActive(false);
            settingField.SetActive(false);
            popupWindow.SetActive(false);
        }

        public void MoveMenu(Vector3 start, Vector3 end, float speed)
        {
            if (menuField.transform.localPosition != end)
            {
                menuField.transform.localPosition = Vector3.Lerp(start, end, speed);
            }
            else
            {
                startTime = Time.time;
            }
        }

        #region Title Menu Callback
        /// <summary>
        /// Play game from the very start
        /// </summary>
        public void NewGame()
        {
            inGame = true;
            bg.sprite = null;
            _bgmAudio.Stop();
            // DeactiveGameObject(titleContainer);
            historyQuene[1].SetActive(false);
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
            _bgmAudio.clip = (AudioClip)Resources.Load("Audio/BGM30", typeof(AudioClip));
            _bgmAudio.Play();
            inGame = false;

            // Deactive display objects
            ShowCG();
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
            OpenPopup(callback, null);
        }

        /// <summary>
        /// Open popup window with comfirm callback
        /// </summary>
        /// <param name="callback">The comfirm callback</param>
        /// <param name="notice">The notice text</param>
        public void OpenPopup(string callback, string notice)
        {
            if(!string.IsNullOrEmpty(notice)) popupNoticeText.text = notice;
            popupCallbackText.text = callback.Trim();
            // ActiveGameObject(popupWindow);
            popupWindow.SetActive(true);
        }

        /// <summary>
        /// Close popup window
        /// </summary>
        public void ClosePopup()
        {
            // DeactiveGameObject(popupWindow);
            popupNoticeText.text = DEFAULT_POPUP_NOTICE_TEXT;
            popupWindow.SetActive(false);
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
                case "BackToTitle":
                    BackToTitle();
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Setting
        /// <summary>
        /// Set <see cref="SettingModel.textShowDuration"/>
        /// </summary>
        /// <param name="duration">the value</param>
        public void SetTextShowDuration(float ratio)
        {
            SettingModel.textShowDuration = SettingModel.MAX_TEXT_SHOW_DURATION * ratio;
        }

        /// <summary>
        /// Set <see cref="SettingModel.lineSwitchDuration"/>
        /// </summary>
        /// <param name="duration">the value</param>
        public void SetLineSwitchDuration(float ratio)
        {
            SettingModel.lineSwitchDuration = SettingModel.MAX_LINE_SWITCH_DURATION * ratio;
        }

        /// <summary>
        /// Set <see cref="SettingModel.skipModeLineSwitchDuration"/>
        /// </summary>
        /// <param name="duration">the value</param>
        public void SetSkipModeLineSwitchDuration(float ratio)
        {
            SettingModel.skipModeLineSwitchDuration = SettingModel.MAX_SKIP_MODE_LINE_SWITCH_DURATION * ratio;
        }

        /// <summary>
        /// Set <see cref="SettingModel.appActiveInBackground"/>
        /// </summary>
        /// <param name="active"></param>
        public void SetAppActiveInBackground(bool active)
        {
            SettingModel.appActiveInBackground = active;
        }

        /// <summary>
        /// Set <see cref="SettingModel.showCGInSkipMode"/>
        /// </summary>
        /// <param name="show"></param>
        public void SetShowCGInSkipMode(bool show)
        {
            SettingModel.showCGInSkipMode = show;
        }

        /// <summary>
        /// Set <see cref="SettingModel.showSpecialEffects"/>
        /// </summary>
        /// <param name="show"></param>
        public void SetShowSpecialEffects(bool show)
        {
            SettingModel.showSpecialEffects = show;
        }

        /// <summary>
        /// Set <see cref="SettingModel.showTextShadow"/>
        /// </summary>
        /// <param name="active"></param>
        public void SetShowTextShadow(bool show)
        {
            SettingModel.showTextShadow = show;
        }

        /// <summary>
        /// Set <see cref="SettingModel.showAnimation"/>
        /// </summary>
        /// <param name="active"></param>
        public void SetShowAnimation(bool show)
        {
            SettingModel.showAnimation = show;
        }

        /// <summary>
        /// Set <see cref="SettingModel.appLanguage"/> with value in <see cref="SettingModel.languages"/> via <paramref name="index"/>
        /// </summary>
        /// <param name="index"></param>
        public void SetAppLanguage(int index)
        {
            SettingModel.appLanguage = SettingModel.languages[index];
            // TODO: Work after rebooting or work imediately?
        }

        /// <summary>
        /// Request full screen mode
        /// </summary>
        public void SetFullScreenMode(bool fullScreen)
        {
            Screen.fullScreen = fullScreen;
            SettingModel.isFullScreenModeOn = fullScreen;
        }

        /// <summary>
        /// Set application run with windowed mode
        /// </summary>
        /// <param name="windowed"></param>
        public void SetWindowedMode(bool windowed)
        {
            SetFullScreenMode(!windowed);
        }

        /// <summary>
        /// Set resoulution
        /// </summary>
        /// <param name="index">The index in <see cref="Screen.resolutions"/></param>
        public void SetWindowedResoulution(int index)
        {
            Resolution selected = Screen.resolutions[index]; 
            Screen.SetResolution(selected.width, selected.height, SettingModel.isFullScreenModeOn);
            SettingModel.resolution = string.Format("{0}x{1}", selected.width, selected.height);
        }

        /// <summary>
        /// Set volume of BGM to mute or not
        /// </summary>
        /// <param name="mute"></param>
        public void SetBgmMute(bool mute)
        {
            _bgmAudio.mute = mute;
            SettingModel.isBgmMute = mute;
        }

        /// <summary>
        /// Set volume of Voices to mute or not
        /// </summary>
        /// <param name="mute"></param>
        public void SetVoicesMute(bool mute)
        {
            _voiceAudio.mute = mute;
            SettingModel.isVoicesMute = mute;
        }

        /// <summary>
        /// Set volume of BGM
        /// </summary>
        /// <param name="volume">The volume value in float</param>
        public void SetBgmVolume(float volume)
        {
            _bgmAudio.volume = volume;
            SettingModel.bgmVolume = volume;
        }

        /// <summary>
        /// Set volume of Voices
        /// </summary>
        /// <param name="volume">The volume value in float</param>
        public void SetVoicesVolume(float volume)
        {
            _voiceAudio.volume = volume;
            SettingModel.voicesVolume = volume;
        }
        #endregion

        /// <summary>
        /// To active a gameobject
        /// </summary>
        /// <param name="gameObject">The target object</param>
        public void ActiveGameObject(GameObject gameObject)
        {
            if (null != historyQuene[1]) historyQuene[1].SetActive(false);
            gameObject.SetActive(true);
            historyQuene[0] = historyQuene[1];
            historyQuene[1] = gameObject;
            Debug.Log(DateTime.Now.ToString() + ": " + (null == historyQuene[0] ? "" : historyQuene[0].name) + " - " + (null == historyQuene[1] ? "" : historyQuene[1].name));
            // gameObject.SetActive(true);
        }

        /// <summary>
        /// To deactive a gameobject
        /// </summary>
        /// <param name="gameObject">The target object</param>
        public void DeactiveGameObject(GameObject gameObject)
        {
            if (null != historyQuene[0]) historyQuene[0].SetActive(true);
            gameObject.SetActive(false);
            historyQuene[1] = historyQuene[0];
            historyQuene[0] = gameObject;
            Debug.Log( DateTime.Now.ToString() + ": " + (null == historyQuene[0] ? "" : historyQuene[0].name) + " - " + (null == historyQuene[1] ? "" : historyQuene[1].name));
            //gameObject.SetActive(false);
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
