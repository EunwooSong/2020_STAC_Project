using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UIAnimation
{
    public class SwipeAnimation : MonoBehaviour, IDragHandler, IEndDragHandler
    {
        [Header("Transform")]
        [SerializeField] private RectTransform targetTr;
        [SerializeField] private Vector2 baseLocation;
        [SerializeField] Vector2 targetLocation;
        [SerializeField] private float swipeWidth = Screen.width;
        [Header("Contorl Value")]
        [SerializeField] private float thresholdPercent = 0.2f;
        [SerializeField] private RectTransform[] objects;
        [SerializeField] private int currentIndex = 0;
        [SerializeField] private int maxIndex;
        [SerializeField] private bool isOnDrag = false;
        [SerializeField] private bool stopSwipe = true;


        // Start is called before the first frame update
        void Start()
        {
            if (!targetTr)
            {
                targetTr = GetComponent<RectTransform>();
                //if (!targetTr)
                //    targetTr = GetComponent<Transform>();

                targetLocation = targetTr.rect.center;
                baseLocation = targetTr.rect.center;
            }

            if(objects[0] && objects[1])
            {
                maxIndex = objects.Length - 1;

                //swipeWidth = objects[0].rect.width;
            }

            if(swipeWidth == 0)
            {
                swipeWidth = Screen.width;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if(!isOnDrag && stopSwipe)
                targetTr.position = Vector2.Lerp(targetTr.position, targetLocation, 5.0f * Time.deltaTime);
        }

        //Do Swipe Animation
        public void OnDrag(PointerEventData d)
        {
            float difference = d.pressPosition.x - d.position.x;
            targetTr.position = targetLocation - new Vector2(difference, 0);
            isOnDrag = true;
        }

        public void OnEndDrag(PointerEventData d)
        {
            float percentage = (d.pressPosition.x - d.position.x) / swipeWidth;
            if (Mathf.Abs(percentage) >= thresholdPercent)
            {
                if(percentage > 0)
                {
                    currentIndex++;
                }
                
                else if(percentage < 0) {
                    currentIndex--;
                }

                currentIndex = Mathf.Clamp(currentIndex, 0, maxIndex);

                targetLocation = baseLocation - new Vector2(swipeWidth * currentIndex, 0);
            }

            isOnDrag = false;
        }

        //------------------------------------------------
        // Set / Get 
        //------------------------------------------------
        public void SetCurrentIndex(int index)
        {
            currentIndex = index;
            targetLocation = baseLocation - new Vector2(swipeWidth * currentIndex, 0);
        }

        public int GetCurrentIndex()
        {
            return currentIndex;
        }

        public void SetStopSwipe(bool state)
        {
            stopSwipe = state;
        }
        public bool GetStopSwipe()
        {
            return stopSwipe;
        }
    }
}