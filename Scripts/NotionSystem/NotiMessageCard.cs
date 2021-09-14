using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using System.Runtime.InteropServices;

namespace NotiSystem
{
    public class NotiMessageCard : MonoBehaviour
    {
        private RectTransform tr;

        [Header("Info")]
        [SerializeField] private NotiType type;
        [SerializeField] private Sprite[] icons;
        [SerializeField] private TMP_Text messageIcon;
        [SerializeField] private TMP_Text messageField;

        [Header("Actions")]
        [SerializeField] private GameObject btn_Confirm;
        [SerializeField] private GameObject btn_Cancle;
        [SerializeField] private GameObject btn_BackGround;

        [Header("Valuse")]
        [SerializeField] private float animationSpeed = 5.0f;
        [SerializeField] public bool animtionFadeIn = true;
        [SerializeField] public bool onlyUseAnimation = false;
        //[SerializeField] private bool useAnimation;

        private void Awake()
        {
            tr = transform.GetChild(1).GetComponent<RectTransform>();

            tr.transform.localScale = Vector3.zero;

            Color color = btn_BackGround.GetComponent<Image>().color;
            color.a = 0.0f;
            btn_BackGround.GetComponent<Image>().color = color;

            animtionFadeIn = false;
        }

        public void Set(NotiType type, string message = "", bool useAnimation = true,
            UnityAction confirmAction = null, UnityAction cancleAction = null,
            string confirmInfo = "", string cancleInfo = "", float animationSpeed = 5.0f)
        {
            //------------------------------------------------
            // Initialize NotiMessageCard
            //------------------------------------------------
            if (onlyUseAnimation) return;

            this.type = type;
            this.animationSpeed = animationSpeed;

            //if(messageIcon)
            //    messageIcon.sprite = icons[(int)type];
            messageIcon.text = type.ToString();

            if (messageField)
                messageField.text = message;

            //btn_BackGround = Instantiate(Resources.Load("NotiSystem/NotiCardBackGround") as GameObject, tr.transform.parent);

            switch (type)
            {
                case NotiType.MESSAGE:
                    Destroy(btn_Cancle);
                    btn_Confirm.transform.GetChild(0).GetComponent<TMP_Text>().text = "확인";
                    btn_BackGround.GetComponent<Button>().onClick.AddListener(RemoveFunction);
                    break;

                case NotiType.CAUTION:
                case NotiType.ERROR:
                    btn_Confirm.transform.GetChild(0).GetComponent<TMP_Text>().text = confirmInfo;
                    btn_Confirm.GetComponent<Button>().onClick.AddListener(confirmAction);
                    
                    if (cancleAction != null)
                    {
                        btn_Cancle.transform.GetChild(0).GetComponent<TMP_Text>().text = cancleInfo;
                        btn_Cancle.GetComponent<Button>().onClick.AddListener(cancleAction);

                        btn_BackGround.GetComponent<Button>().onClick.AddListener(cancleAction);
                    }
                    else
                        Destroy(btn_Cancle);
                    break;
            }
            
            btn_Confirm.GetComponent<Button>().onClick.AddListener(RemoveFunction);

            if (useAnimation)
            {
                tr.transform.localScale = Vector3.zero;
                
                Color color = btn_BackGround.GetComponent<Image>().color;
                color.a = 0.0f;
                btn_BackGround.GetComponent<Image>().color = color;

                animtionFadeIn = true;
            }
            else
            {

            }
        }

        // Update is called once per frame
        void Update()
        {
            //------------------------------------------------
            // NotiMessageCard Aniamtion
            //------------------------------------------------
            if (animtionFadeIn)
            {
                tr.transform.localScale = Vector3.Lerp(tr.transform.localScale, new Vector3(1,1,1), animationSpeed * Time.deltaTime);

                Color color = btn_BackGround.GetComponent<Image>().color;
                color.a = Mathf.Lerp(color.a, 0.8f, animationSpeed * Time.deltaTime);
                btn_BackGround.GetComponent<Image>().color = color;

                if (tr.transform.localScale.x > 0.1f)
                    btn_BackGround.GetComponent<Image>().raycastTarget = true;
            }
            else
            {
                tr.transform.localScale = Vector3.Lerp(tr.transform.localScale, Vector3.zero, animationSpeed * Time.deltaTime);

                Color color = btn_BackGround.GetComponent<Image>().color;
                color.a = Mathf.Lerp(color.a, 0.0f, animationSpeed * Time.deltaTime);
                btn_BackGround.GetComponent<Image>().color = color;

                if (tr.transform.localScale.x < 0.1f)
                    btn_BackGround.GetComponent<Image>().raycastTarget = false;
            }
        }

        void RemoveFunction()
        {
            animtionFadeIn = false;
            Destroy(this.gameObject, 1.0f);
        }
    }
}