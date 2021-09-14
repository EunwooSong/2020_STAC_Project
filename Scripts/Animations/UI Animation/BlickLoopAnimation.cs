using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIAnimation
{
    public class BlickLoopAnimation : MonoBehaviour
    {
        [Header("Contorl Values")]
        [SerializeField] private TMP_Text targetText;
        [SerializeField] private Image targetImage;
        [SerializeField] private float blickSpeed = 5.0f;
        [SerializeField] private bool blickLoop = true;
        [SerializeField] private bool blickPlayOnce = false;
        [SerializeField] private bool playOnAwake = true;

        // Start is called before the first frame update
        void Awake()
        {
            if (!targetText)
                if (GetComponent<TMP_Text>())
                    targetText = GetComponent<TMP_Text>();

            if (playOnAwake)
                StartBlick();
        }

        public void StartBlick()
        {
            blickLoop = true;
            StartCoroutine(StartBlickAnimation());
        }

        public void StartBlickOnce()
        {
            blickPlayOnce = true;

            StartBlick();
        }

        public void StopBlick()
        {
            blickLoop = false;
        }


        IEnumerator StartBlickAnimation()
        {
            float time = 0.0f;

            bool trigger = false;

            if(targetText)
            {
                Color tmp = targetText.color;
                tmp.a = 0.0f;
                targetText.color = tmp;
            }
            
            if (targetImage)
            {
                Color tmp = targetImage.color;
                tmp.a = 0.0f;
                targetImage.color = tmp;
            }

            while (blickLoop)
            {
                time += Time.deltaTime;

                if(targetText)
                {
                    //Text
                    Color tmpColor = targetText.color;
                    tmpColor.a = Mathf.Abs(Mathf.Sin(time * blickSpeed));

                    targetText.color = tmpColor;

                    if (blickPlayOnce)
                    {
                        if (tmpColor.a > 0.9f)
                            trigger = true;

                        else if (trigger && tmpColor.a < 0.01f)
                            blickLoop = false;
                    }
                }
                if(targetImage)
                {
                    Color tmpImageColor = targetImage.color;
                    tmpImageColor.a = Mathf.Abs(Mathf.Sin(time * blickSpeed));

                    targetImage.color = tmpImageColor;

                    if (blickPlayOnce)
                    {
                        if (tmpImageColor.a > 0.9f)
                            trigger = true;

                        else if (trigger && tmpImageColor.a < 0.01f)
                            blickLoop = false;
                    }
                }

                yield return null;
            }

            if (targetText)
            {
                Color tmp = targetText.color;
                tmp.a = 0.0f;
                targetText.color = tmp;
            }

            if (targetImage)
            {
                Color tmp = targetImage.color;
                tmp.a = 0.0f;
                targetImage.color = tmp;
            }
        }
    }
}