using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommunicationFormat;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using Account;
using Core;
using NotiSystem;

namespace UIGroup
{
    public class UIGroup_ShowCharacterInfos : UIGroupBase
    {
        [Header("Values")]
        private CharacterData charData;
        public int teamIndex = 0;
        public bool canShow = true;

        [Header("Status Field")]
        [SerializeField] private TMP_Text charRank;
        [SerializeField] private TMP_Text charName;
        [SerializeField] private TMP_Text status_HP;
        [SerializeField] private TMP_Text status_Speed;
        
        [Header("Skill Field")]
        [SerializeField] private TMP_Text skill_name;
        [SerializeField] private TMP_Text skill_damage;
        [SerializeField] private TMP_Text skill_info;
        [SerializeField] private Button btn_Skill_info_1;
        [SerializeField] private Button btn_Skill_info_2;
        [SerializeField] private Button btn_Skill_info_3;
        
        [Header("Level Field")]
        [SerializeField] private TMP_Text charLevel;
        [SerializeField] private TMP_Text charXp;
        [SerializeField] private Slider sli_charLevel;
        [SerializeField] private Button btn_Upgrade_Char;

        [Header("Contorl Field")]
        [SerializeField] private Button btn_Change_Char;
        [SerializeField] private Button btn_OpenCharacterInfo;
        [SerializeField] private Button btn_MoveStateDefault;

        [Header("CharInfo Card")]
        [SerializeField] private NotiMessageCard noti_Animation;
        [SerializeField] private Button btn_CloseCharacterInfo;
        [SerializeField] private TMP_Text text_CharacterInfo;

        [Header("Sprite Size Up")]
        [SerializeField] private SpriteRenderer characterRenderer;

        private void Awake()
        {
            noti_Animation.animtionFadeIn = false;
        }

        private void Start()
        {
            if(GameCore.instance != null)
                UpdateEachData();

            btn_Skill_info_1.onClick.AddListener(() => { Btn_ChangeSkill_Info(0); });
            btn_Skill_info_2.onClick.AddListener(() => { Btn_ChangeSkill_Info(1); });
            btn_Skill_info_3.onClick.AddListener(() => { Btn_ChangeSkill_Info(2); });

            btn_OpenCharacterInfo.onClick.AddListener(() => { Btn_OpenCharacterInfoField(); });
            btn_CloseCharacterInfo.onClick.AddListener(() => { Btn_CloseCharacterInfoField(); });
        }

        private void Update()
        {
            if (groupState == UIGroupState.DISABLED)
                characterRenderer.transform.localScale = Vector3.Lerp(characterRenderer.transform.localScale, Vector3.one * 2, 5.0f * Time.deltaTime);
            else
                characterRenderer.transform.localScale = Vector3.Lerp(characterRenderer.transform.localScale, Vector3.one * 2.3f, 5.0f * Time.deltaTime);
        }

        //------------------------------------------------
        // Add Button Actions
        //------------------------------------------------
        public void Btn_UpgradeCharacter(UnityAction upgradeFunc)
        {
            btn_Upgrade_Char.onClick.AddListener(upgradeFunc);
        }

        public void Btn_ChangeCharacter(UnityAction changeFunc)
        {
            btn_Change_Char.onClick.AddListener(changeFunc);
        }

        public void Btn_MoveStateDefault(UnityAction action)
        {
            btn_MoveStateDefault.onClick.AddListener(action);
        }


        private void Btn_ChangeSkill_Info(int index)
        {
            // Change Skill Info (index)
            // ex) skill_info = GetCharacter().skill info[index]
            skill_name.text = charData.GetSkill_Info()[index + 1].GetName();
            skill_damage.text = charData.GetSkill_Info()[index + 1].GetDamage() + " / " + charData.GetSkill_Info()[1].GetMana();
            skill_info.text = charData.GetSkill_Info()[index + 1].GetAction();
        }

        private void Btn_CloseCharacterInfoField()
        {
            noti_Animation.animtionFadeIn = false;
        }

        private void Btn_OpenCharacterInfoField()
        {
            noti_Animation.animtionFadeIn = true;
        }

        //------------------------------------------------
        // Value Manager
        //------------------------------------------------
        override public void UpdateEachData()
        {
            foreach (CharacterData data in UserData.GetPlayerData().GetCharactersData())
            {
                if (data.GetSelected() == teamIndex)
                {
                    charData = data;
                    break;
                }
                else
                    charData = null;
            }
            if (charData == null)
            {
                canShow = false;
                return;
            }
            else
                canShow = true;

            charRank.text = charData.GetRank().ToString();
            charName.text = charData.GetCharacterType().ToString();
            status_HP.text = charData.GetMaximumHp().ToString();
            status_Speed.text = charData.GetCurrentSpeed().ToString();
            
            skill_name.text = charData.GetSkill_Info()[1].GetName();
            skill_damage.text = charData.GetSkill_Info()[1].GetDamage() + " / " + charData.GetSkill_Info()[1].GetMana();
            skill_info.text = charData.GetSkill_Info()[1].GetAction();

            charXp.text = charData.GetCurrentXp().ToString() + " / " + (charData.GetLevel() * 10);
            sli_charLevel.value = charData.GetCurrentXp();
            sli_charLevel.maxValue = 10 * charData.GetLevel();

            //Set Skill Icon
            Sprite[] skill_icon = Resources.LoadAll<Sprite>("Skill_Icon/" + charData.GetCharacterType().ToString());

            btn_Skill_info_1.gameObject.GetComponent<Image>().sprite = skill_icon[0];
            btn_Skill_info_2.gameObject.GetComponent<Image>().sprite = skill_icon[1];
            btn_Skill_info_3.gameObject.GetComponent<Image>().sprite = skill_icon[2];


            text_CharacterInfo.text = charData.GetCharacterInfo() + "\n아무 곳이나 클릭 . . .";

            charLevel.text = "LV." + charData.GetLevel().ToString();
        }
    }
}