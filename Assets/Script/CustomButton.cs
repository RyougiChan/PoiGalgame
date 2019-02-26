using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Script
{
    public class CustomButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        Button button;
        RawImage buttonBg;
        Text buttonText;

        string textureResolution;
        Texture normalTexture;
        Texture enterTexture;
        Texture pressTexture;

        public void OnPointerEnter(PointerEventData eventData)
        {
            SetEnterStyle();
            Debug.Log(string.Format("Pointer enter button named {0}", button.name));
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            SetNormalStyle();
            Debug.Log(string.Format("Pointer click button named {0}", button.name));
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            SetPressStyle();
            Debug.Log(string.Format("Pointer click down button named {0}", button.name));
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (button.name.ToLower().Equals("closebtn") || button.name.ToLower().Equals("closebutton") || button.name.ToLower().Equals("close")) SetNormalStyle();
            else SetEnterStyle();
            Debug.Log(string.Format("Pointer click up button named {0}", button.name));
        }

        void Start()
        {
            button = GetComponent<Button>();
            buttonBg = GetComponent<RawImage>();
            buttonText = transform.GetChild(0).GetComponent<Text>();
            textureResolution = buttonBg.texture.name.Split('_')[2];

            normalTexture = Resources.Load<Texture>("Sprite/button_normal_" + textureResolution);
            enterTexture = Resources.Load<Texture>("Sprite/button_enter_" + textureResolution);
            pressTexture = Resources.Load<Texture>("Sprite/button_press_" + textureResolution);
        }

        void SetNormalStyle()
        {
            buttonBg.texture = normalTexture;
            buttonText.color = new Color(0, 51f/255, 17f/255);
        }
        void SetEnterStyle()
        {
            buttonBg.texture = enterTexture;
            buttonText.color = Color.white;
        }
        void SetPressStyle()
        {
            buttonBg.texture = pressTexture;
            buttonText.color = Color.white;
        }

    }
}
