//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
namespace Xaz
{
    #if USE_LUA
    [SLua.CustomLuaClass]
    #endif
    [AddComponentMenu("UI/Effects/Underline")]
    [RequireComponent(typeof(Text))]
    [ExecuteInEditMode]
    public class UIUnderline :UIBehaviour
    {
        public Color color = Color.black;

        private Text _uiText; 
        private Text uiText 
        {
            get{
                if(!_uiText){
                    _uiText = transform.GetComponent<Text>();
                }
                return _uiText;
            }
           
        }
        private Image _uiUnderLineImage;
        private Image uiUnderLineImage 
        {
            get{
                if(!_uiUnderLineImage){
                    Transform underline = uiText.transform.Find("underline");
                    if(!underline){
                        Image  image = XazHelper.AddChild<Image>(transform.gameObject);
                        image.name = "underline";
                        image.transform.SetParent(transform);
                        underline = image.transform;
                        underline.gameObject.hideFlags = HideFlags.NotEditable;
                    }
                    _uiUnderLineImage = underline.GetComponent<Image>();
                }
                _uiUnderLineImage.color =color;
                return _uiUnderLineImage;
            }
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            CreateUnderline();
        }

        public  void CreateUnderline()
        {
            uiUnderLineImage.rectTransform.anchoredPosition =new Vector2(0f,- (float)uiText.fontSize *0.5f);
             uiUnderLineImage.rectTransform.sizeDelta = new Vector2(uiText.rectTransform.sizeDelta.x,2f);
        }


        private string lastContent = string.Empty;
        void Update()
        {
            if(uiText.text != lastContent){
                CreateUnderline();
                lastContent = uiText.text;
            }

        }
    }
}