using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIAnimation;
using TMPro;
using CommunicationFormat;
using UnityEngine.UI;
using Account;
using Core;
using UnityEngine.Events;

namespace UIGroup
{
    public class UIGroup_ChangeCharacter : UIGroupBase
    {
        [Header("Value")]
        [SerializeField] private SwipeAnimation swipe_Character;
        [SerializeField] private TMP_Text characterInfo;
        [SerializeField] private Button btn_ApplyBtn;
        [SerializeField] private GameObject[] buttons;
        [SerializeField] private Image[] showCharacter;
        [SerializeField] int beforeIndex = 0;
        [SerializeField] private bool stopSwipe;
        [SerializeField] private Button btn_backBeforeState;

        Vector2 defaultBtnScale;

        private void Start()
        {
            //Add Buttons Action
            defaultBtnScale = buttons[0].GetComponent<RectTransform>().sizeDelta;
            buttons[0].GetComponent<Button>().onClick.AddListener(() => { Btn_QuickChangeCharacter(CharacterType.MARK); });
            buttons[1].GetComponent<Button>().onClick.AddListener(() => { Btn_QuickChangeCharacter(CharacterType.LEE_DR); });
            buttons[2].GetComponent<Button>().onClick.AddListener(() => { Btn_QuickChangeCharacter(CharacterType.WOLF); });
            buttons[3].GetComponent<Button>().onClick.AddListener(() => { Btn_QuickChangeCharacter(CharacterType.VAMPIRE); });
            buttons[4].GetComponent<Button>().onClick.AddListener(() => { Btn_QuickChangeCharacter(CharacterType.SM_124); });
            buttons[5].GetComponent<Button>().onClick.AddListener(() => { Btn_QuickChangeCharacter(CharacterType.MAROS); });
            buttons[6].GetComponent<Button>().onClick.AddListener(() => { Btn_QuickChangeCharacter(CharacterType.MIYA); });
            buttons[7].GetComponent<Button>().onClick.AddListener(() => { Btn_QuickChangeCharacter(CharacterType.MR_MA); });

            if (GameCore.instance != null)
            {
                UpdateEachData();
            }

            UpdateCharacterInfoText();
        }

        private void Update()
        {
            if (beforeIndex != swipe_Character.GetCurrentIndex())
                UpdateCharacterInfoText();


            //Animation
            for(int i = 0; i < buttons.Length; i ++)
            {
                RectTransform rTr = buttons[i].GetComponent<RectTransform>();

                if (i == beforeIndex)
                {
                    rTr.sizeDelta = Vector2.Lerp(rTr.sizeDelta, defaultBtnScale * 1.25f, Time.deltaTime * 5.0f);
                }
                else
                {
                    rTr.sizeDelta = Vector2.Lerp(rTr.sizeDelta, defaultBtnScale, Time.deltaTime * 5.0f);
                }
            }
        }

        public override void ChangeGroupState(UIGroupState state)
        {
            base.ChangeGroupState(state);

            swipe_Character.SetStopSwipe(state == UIGroupState.ENABLED);
        }

        //------------------------------------------------
        // Value Manager
        //------------------------------------------------

        void UpdateCharacterInfoText()
        {
            beforeIndex = swipe_Character.GetCurrentIndex();

            characterInfo.text = ((CharacterType)beforeIndex).ToString() + " 적용하기";

            if (showCharacter[beforeIndex].color == Color.black)
            {
                characterInfo.text = "????????";
                btn_ApplyBtn.interactable = false;
            }
            else
                btn_ApplyBtn.interactable = true;
        }

        public override void UpdateEachData()
        {
            CharacterData[] datas = UserData.GetPlayerData().GetCharactersData();

            foreach (Image image in showCharacter)
                image.color = Color.black;

            foreach (GameObject button in buttons)
                button.GetComponent<Image>().color = Color.black;

            foreach (CharacterData data in datas)
            {
                showCharacter[(int)data.GetCharacterType()].color = Color.white;
                buttons[(int)data.GetCharacterType()].GetComponent<Image>().color = Color.white;
            }
        }

        public int GetCurrentIndex()
        {
            return swipe_Character.GetCurrentIndex();
        }

        //------------------------------------------------
        // Button Action
        //------------------------------------------------
        private void Btn_QuickChangeCharacter(CharacterType quickType)
        {
            swipe_Character.SetCurrentIndex((int)quickType);
        }

        public void Btn_SetCharacterApply(UnityAction action)
        {
            btn_ApplyBtn.onClick.AddListener(action);
        }

        public void Btn_BackBeforeState(UnityAction action)
        {
            btn_backBeforeState.onClick.AddListener(action);
        }
    }
}