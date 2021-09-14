using Core;
using NotiSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Appdata.Option
{
    public class OptionManager : MonoBehaviour
    {
        [Header("Values")]
        [SerializeField] private Slider sli_graphicQuality;
        [SerializeField] private Slider sli_frameRate;
        //[SerializeField] private Slider sli_sfxVolume;
        //[SerializeField] private Slider sli_bgVolume;

        [Header("Buttons")]
        [SerializeField] private Button btn_ApplyData;
        [SerializeField] private Button[] btn_CancleData;

        [Header("Animation")]
        [SerializeField] private NotiMessageCard cardAnimation;

        // Start is called before the first frame update
        void Start()
        {
            btn_ApplyData.onClick.AddListener(ApplyAppData);
            btn_ApplyData.onClick.AddListener(CancleAppData);

            foreach(Button cancelButton in btn_CancleData)
            {
                cancelButton.onClick.AddListener(() => { CancleAppData(); });
            }
            CancleAppData();

            cardAnimation = GetComponent<NotiMessageCard>();
        }

        public void StartOptionField()
        {
            StartCoroutine(UI_Animation(true));
        }

        private void ApplyAppData()
        {
            ApplicationData.optionData.graphicQuality = (int)sli_graphicQuality.value;
            ApplicationData.optionData.frameRate = (int)sli_frameRate.value;
            ApplicationData.optionData.sfx_volume = 100.0f;
            ApplicationData.optionData.bg_volume = 100.0f;

            ApplicationData.SaveApplicationData(ApplicationData.optionData);
            GameCore.instance.UpdateGraphicQuailty();
        }

        private void CancleAppData()
        {
            sli_graphicQuality.value = (int)ApplicationData.optionData.graphicQuality;
            sli_frameRate.value = (int)ApplicationData.optionData.frameRate;

            StartCoroutine(UI_Animation(false));
        }

        IEnumerator UI_Animation(bool isFadein)
        {
            if(isFadein) {
                //transform.GetChild(0).gameObject.SetActive(true);
                //transform.GetChild(1).gameObject.SetActive(true);

                transform.GetChild(0).GetComponent<Image>().raycastTarget = true;
                transform.GetChild(1).GetComponent<Image>().raycastTarget = true;

                cardAnimation.animtionFadeIn = true;

                yield return null;
            }
            else
            {
                transform.GetChild(0).GetComponent<Image>().raycastTarget = false;
                transform.GetChild(1).GetComponent<Image>().raycastTarget = false;

                cardAnimation.animtionFadeIn = false;

                yield return new WaitForSeconds(0.6f);

                //transform.GetChild(0).gameObject.SetActive(false);
                //transform.GetChild(1).gameObject.SetActive(false);
            }
        }
    }
}