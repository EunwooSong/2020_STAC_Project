using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace NotiSystem
{
    public enum NotiType
    {
        MESSAGE, CAUTION, ERROR
    }

    public class NotiSystemManager : MonoBehaviour
    {
        [Header("NotiSystem")]
        public static NotiSystemManager instacne;
        [SerializeField] private RectTransform parent;

        [Header("NotiCard")]
        [SerializeField] private GameObject notiPrefabs;

        // Start is called before the first frame update

        void Start()
        {
            if (!instacne)
            {
                instacne = this;
                DontDestroyOnLoad(this);
            }   
            else
                Destroy(this);

            notiPrefabs = Resources.Load("NotiSystem/NotiMessageCard") as GameObject;
        }

        // Update is called once per frame
        void Update()
        {
            //------------------------------------------------
            // Auto Get Canvas
            //------------------------------------------------
            if(!parent)
            {
                parent = GameObject.FindGameObjectWithTag("GameUI").GetComponent<RectTransform>();
            }
        }

        public void ShowMessage(string mes, string confirm = "확인")
        {
            if(parent)
            {
                GameObject tmp = Instantiate(notiPrefabs, parent);
                tmp.GetComponent<NotiMessageCard>().Set(NotiType.MESSAGE, mes);
            }
        }

        public void ShowError(string errorCode, string mes, UnityAction action, string confirm = "확인")
        {
            if(parent)
            {
                GameObject tmp = Instantiate(notiPrefabs, parent);
                tmp.GetComponent<NotiMessageCard>().Set(NotiType.ERROR, mes + "\n <color=\"red\">" + errorCode, true, action, null, confirm);
            }
        }

        public void ShowCaution(string mes, UnityAction confirmAction, UnityAction cancleAction, string confirm = "확인", string cancle = "취소") 
        {
            if (parent)
            {
                GameObject tmp = Instantiate(notiPrefabs, parent);
                tmp.GetComponent<NotiMessageCard>().Set(NotiType.CAUTION, mes, true, confirmAction, cancleAction, confirm, cancle);
            }
        }
    }
}

