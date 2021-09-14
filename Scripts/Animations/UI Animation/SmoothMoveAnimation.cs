using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIAnimation
{
    public class SmoothMoveAnimation : MonoBehaviour
    {
        RectTransform r_tr;
        Transform tr;

        [Header("Target (RectTransform)")]
        public List<RectTransform> rectTransform;
        public List<Transform> transforms;

        [Header("Value")]
        [SerializeField] bool isDone = false;
        [Tooltip("0 : Start, 1 : EndPoint, ~ : Eles Points . . .")]
        [SerializeField] private int goIndex;
        [SerializeField] private float moveSpeed = 5.0f;
        [SerializeField] private float endLength = 0.001f;

        // Start is called before the first frame update
        void Start()
        {
            if (GetComponent<RectTransform>())
                r_tr = GetComponent<RectTransform>();

            else if (GetComponent<Transform>())
                tr = GetComponent<Transform>();
        }

        // Update is called once per frame
        void Update()
        {
            //Movement
            if(r_tr)
            {
                if (!isDone)
                {
                    Vector3 lastPos = r_tr.position;
                    Vector3 toMovePos
                         = Vector3.Lerp(r_tr.position, rectTransform[Mathf.Clamp(goIndex, 0, rectTransform.Count - 1)].position, moveSpeed * Time.deltaTime);

                    Vector3 diff = toMovePos - lastPos;

                    if (Mathf.Abs(diff.x) < endLength && Mathf.Abs(diff.y) < endLength && Mathf.Abs(diff.z) < endLength)
                    {
                        r_tr.position = rectTransform[Mathf.Clamp(goIndex, 0, rectTransform.Count - 1)].position;
                        isDone = true;
                    }
                    else
                    {
                        r_tr.position = toMovePos;
                    }
                }
            }
            else if(tr)
            {
                if (!isDone)
                {
                    Vector3 lastPos = tr.position;
                    Vector3 toMovePos
                         = Vector3.Lerp(tr.position, transforms[Mathf.Clamp(goIndex, 0, transforms.Count - 1)].position, moveSpeed * Time.deltaTime);

                    Vector3 diff = toMovePos - lastPos;

                    if (Mathf.Abs(diff.x) < endLength && Mathf.Abs(diff.y) < endLength && Mathf.Abs(diff.z) < endLength)
                    {
                        tr.position = transforms[Mathf.Clamp(goIndex, 0, transforms.Count - 1)].position;
                        isDone = true;
                    }
                    else
                    {
                        tr.position = toMovePos;
                    }
                }
            }
        }

        public void ChangeIndex(int index)
        {
            goIndex = index;
            isDone = false;
        }
    }
}