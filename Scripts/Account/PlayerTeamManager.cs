using CommunicationFormat;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Appdata;
using Core;
using UIGroup;
using UIAnimation;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using UnityEngine.SceneManagement;

namespace Account.Team
{
    public enum TeamManagerShowState
    {
        DEFAULT, CHANGE_CHARACTER, SHOW_CHARACTER_INFO_1, SHOW_CHARACTER_INFO_2, MOVE_MAIN
    }

    public class PlayerTeamManager : MonoBehaviour
    {
        [Header("Guid Blink")]
        public BlickLoopAnimation[] blinks;

        [Header("ShowCharacterInfo")]
        public Button btn_ShowTeam_1;
        public Button btn_ShowTeam_2;
        public Button btn_MoveMainScene;

        [Header("CurrentState")]
        public TeamManagerShowState showState;
        public TeamManagerShowState beforeState { get; private set; }

        [Header("Group")]
        public UIGroup_Main uiGroup_Main;
        public UIGroup_ChangeCharacter uiGroup_ChangeCharacter;
        public UIGroup_ShowCharacterInfos uiGroup_ShowCharacterInfo1;
        public UIGroup_ShowCharacterInfos uiGroup_ShowCharacterInfo2;

        [Header("Values")]
        //public GameObject swipe_Characters;
        //public GameObject swipe_Items;
        private bool readyUpdateUserData;

        [Header("Transition")]
        public Animator anim_SceneTransition;

        // Start is called before the first frame update
        void Start()
        {
            //Group Set
            ChangeCurrentShowState(TeamManagerShowState.DEFAULT);
            UpdateTeamData();

            //Buttons Set
            uiGroup_ShowCharacterInfo1.Btn_UpgradeCharacter(() => { Btn_CharacterLevelUp(); });
            uiGroup_ShowCharacterInfo2.Btn_UpgradeCharacter(() => { Btn_CharacterLevelUp(); });
            uiGroup_ShowCharacterInfo1.Btn_ChangeCharacter(() => { Btn_ChangeShowState(TeamManagerShowState.CHANGE_CHARACTER); });
            uiGroup_ShowCharacterInfo2.Btn_ChangeCharacter(() => { Btn_ChangeShowState(TeamManagerShowState.CHANGE_CHARACTER); });
            uiGroup_ShowCharacterInfo1.Btn_MoveStateDefault(() => { ChangeCurrentShowState(TeamManagerShowState.DEFAULT); });
            uiGroup_ShowCharacterInfo2.Btn_MoveStateDefault(() => { ChangeCurrentShowState(TeamManagerShowState.DEFAULT); });
            btn_ShowTeam_1.onClick.AddListener(() => { Btn_ChangeShowState(TeamManagerShowState.SHOW_CHARACTER_INFO_1); });
            btn_ShowTeam_2.onClick.AddListener(() => { Btn_ChangeShowState(TeamManagerShowState.SHOW_CHARACTER_INFO_2); });
            uiGroup_ChangeCharacter.Btn_SetCharacterApply(() => { Btn_ChangeCharacter(); });
            uiGroup_ChangeCharacter.Btn_BackBeforeState(() => { BackBeforeState(); });
            btn_MoveMainScene.onClick.AddListener(() => { Btn_ChangeShowState(TeamManagerShowState.MOVE_MAIN); });

            foreach (BlickLoopAnimation blink in blinks)
            {
                blink.StopBlick();
            }
        }

        // Update is called once per frame
        void Update()
        {
            //Update ValueManager
            if (readyUpdateUserData && GameCore.instance.dataState == DataState.DONE)
            {
                ChangeCurrentShowState(TeamManagerShowState.DEFAULT);
                readyUpdateUserData = false;   
                UpdateTeamData();
            }
        }

        //------------------------------------------------
        // Value Manager
        //------------------------------------------------
        private void UpdateTeamData()
        {
            uiGroup_Main.UpdateEachData();
            uiGroup_ChangeCharacter.UpdateEachData();
            uiGroup_ShowCharacterInfo1.UpdateEachData();
            uiGroup_ShowCharacterInfo2.UpdateEachData();

            if (!uiGroup_ShowCharacterInfo1.canShow && !uiGroup_ShowCharacterInfo2.canShow)
            {
                foreach (BlickLoopAnimation blink in blinks)
                {
                    blink.StartBlick();
                }
            }
            else
            {
                foreach (BlickLoopAnimation blink in blinks)
                {
                    blink.StopBlick();
                }
            }
        }

        private void ChangeCurrentShowState(TeamManagerShowState state)
        {
            //if (beforeState == showState)
            //{
            //    return;
            //}

            beforeState = showState;
            showState = state;

            btn_ShowTeam_1.interactable = false;
            btn_ShowTeam_2.interactable = false;

            switch (state)
            {
                case TeamManagerShowState.DEFAULT:
                    uiGroup_ChangeCharacter.ChangeGroupState(UIGroupState.DISABLED);
                    uiGroup_ShowCharacterInfo1.ChangeGroupState(UIGroupState.DISABLED);
                    uiGroup_ShowCharacterInfo2.ChangeGroupState(UIGroupState.DISABLED);
                    uiGroup_Main.ChangeGroupState(UIGroupState.ENABLED);

                    btn_ShowTeam_1.interactable = true;
                    btn_ShowTeam_2.interactable = true;
                    break;

                case TeamManagerShowState.CHANGE_CHARACTER:
                    uiGroup_Main.ChangeGroupState(UIGroupState.DISABLED);
                    uiGroup_ShowCharacterInfo1.ChangeGroupState(UIGroupState.DISABLED);
                    uiGroup_ShowCharacterInfo2.ChangeGroupState(UIGroupState.DISABLED);
                    uiGroup_ChangeCharacter.ChangeGroupState(UIGroupState.ENABLED);
                    break;

                case TeamManagerShowState.SHOW_CHARACTER_INFO_1:
                    if (!uiGroup_ShowCharacterInfo1.canShow)
                    {
                        showState = TeamManagerShowState.DEFAULT;
                        ChangeCurrentShowState(TeamManagerShowState.CHANGE_CHARACTER);
                        return;
                    }
                    uiGroup_Main.ChangeGroupState(UIGroupState.ENABLED);
                    uiGroup_ChangeCharacter.ChangeGroupState(UIGroupState.DISABLED);
                    uiGroup_ShowCharacterInfo2.ChangeGroupState(UIGroupState.DISABLED);
                    uiGroup_ShowCharacterInfo1.ChangeGroupState(UIGroupState.ENABLED);
                    break;

                case TeamManagerShowState.SHOW_CHARACTER_INFO_2:
                    if(!uiGroup_ShowCharacterInfo2.canShow)
                    {
                        showState = TeamManagerShowState.DEFAULT;
                        ChangeCurrentShowState(TeamManagerShowState.CHANGE_CHARACTER);
                        return;
                    }
                    uiGroup_Main.ChangeGroupState(UIGroupState.ENABLED);
                    uiGroup_ChangeCharacter.ChangeGroupState(UIGroupState.DISABLED);
                    uiGroup_ShowCharacterInfo1.ChangeGroupState(UIGroupState.DISABLED);
                    uiGroup_ShowCharacterInfo2.ChangeGroupState(UIGroupState.ENABLED);
                    break;

                case TeamManagerShowState.MOVE_MAIN:
                    //Start Move Scene - Main
                    StartCoroutine(MoveMainScene());
                    break;
                default:
                    break;
            }
        }

        private void BackBeforeState()
        {
            Debug.Log(beforeState.ToString());
            ChangeCurrentShowState(beforeState);
        }

        //------------------------------------------------
        // Button Actions
        //------------------------------------------------
        private void Btn_ChangeShowState(TeamManagerShowState state)
        {
            ChangeCurrentShowState(state);
        } 

        //팀 변경 코드가 눌릴시 호출되는 함수
        private void Btn_ChangeCharacter()
        {
            CharacterType type;

            //바꿀 캐릭터의 타입
            type = (CharacterType)uiGroup_ChangeCharacter.GetCurrentIndex();


            if (beforeState == TeamManagerShowState.SHOW_CHARACTER_INFO_1)
            {
                //첫번째 캐릭터를 변경한다는 정보를 전송함
                StartCoroutine(Send_ChangeCharacter(type, 0));
            }
            else if (beforeState == TeamManagerShowState.SHOW_CHARACTER_INFO_2)
            {
                //두번째 캐릭터 변경한다는 정보를 전송함
                StartCoroutine(Send_ChangeCharacter(type, 1));
            }
            else
            {
                //서버와의 정보 교환이 성공적으로 이루어지면...
                if (!uiGroup_ShowCharacterInfo1.canShow)
                {
                    beforeState = TeamManagerShowState.SHOW_CHARACTER_INFO_1; //이전 상태의 정보를 변경
                    Btn_ChangeCharacter(); //다시 호출시켜 원상태(캐릭터가 보이는)로 이동
                }
                else if(!uiGroup_ShowCharacterInfo2.canShow)
                {
                    beforeState = TeamManagerShowState.SHOW_CHARACTER_INFO_2;
                    Btn_ChangeCharacter();
                }
            }
        }

        private void Btn_CharacterLevelUp()
        {
            if(showState == TeamManagerShowState.SHOW_CHARACTER_INFO_1)
            {
                //Team의 첫 번째 캐릭터의 레벨업 진행
                foreach(CharacterData data in UserData.GetPlayerData().GetCharactersData())
                {
                    if(data.GetSelected() == 0)
                    {
                        StartCoroutine(Send_CharacterLevelUp(data.GetCharacterType()));
                        break;
                    }
                }
            }
            else if(showState == TeamManagerShowState.SHOW_CHARACTER_INFO_2)
            {
                //Team의 두 번째 캐릭터의 레벨업 진행
                foreach (CharacterData data in UserData.GetPlayerData().GetCharactersData())
                {
                    if (data.GetSelected() == 1)
                    {
                        StartCoroutine(Send_CharacterLevelUp(data.GetCharacterType()));
                        break;
                    }
                }
            }
            else
            {
                Debug.LogError("Current Show State Error");
                return;
            }
        }

        IEnumerator MoveMainScene()
        {
            anim_SceneTransition.SetTrigger("Transition_In");
            AsyncOperation op = SceneManager.LoadSceneAsync("MainScene");
            op.allowSceneActivation = false;

            yield return new WaitForSeconds(1.0f);

            op.allowSceneActivation = true;
        }


        //------------------------------------------------
        //  Connect Server
        //------------------------------------------------
        IEnumerator Send_ChangeCharacter(CharacterType type, int index)
        {
            yield return new WaitForEndOfFrame();

            WWWForm form = new WWWForm();

            form.AddField("name", type.ToString().ToLower());
            form.AddField("index", index);

            using(var w = UnityWebRequest.Post(ApplicationData.GetServerURL() + "character/raise", form))
            {
                w.SetRequestHeader("Authorization", "Bearer " + UserData.GetAccessToken());
                yield return w.SendWebRequest();

                if (w.isNetworkError || w.isHttpError)
                    Debug.LogError(w.error);
                else
                {
                    JObject d = JObject.Parse(w.downloadHandler.text);
                    if (d["success"].Value<bool>())
                    {
                        //CharacterData[] datas = UserData.GetPlayerData().GetCharactersData();
                        //foreach (CharacterData charData in datas)
                        //{
                        //    if (charData.GetCharacterType() == type)
                        //        charData.SetSelected(index);
                        //    else
                        //        charData.SetSelected(-1);
                        //}
                        //Debug.Log(form.ToString());
                        readyUpdateUserData = true;
                        GameCore.instance.UpdatePlayerData();
                    }
                    else
                    {
                        Debug.LogError(d.GetValue("mes").ToString()); 
                    }
                }
            }
        }

        IEnumerator Send_CharacterLevelUp(CharacterType type)
        {
            yield return new WaitForEndOfFrame();

            WWWForm form = new WWWForm();

            form.AddField("name", type.ToString());

            using (var w = UnityWebRequest.Post(ApplicationData.GetServerURL() + "character/levelUp", form))
            {
                yield return w.SendWebRequest();

                if (w.isNetworkError || w.isHttpError)
                    Debug.LogError(w.error);
                else
                {
                    JObject d = JObject.Parse(w.downloadHandler.text);

                    if(d["success"].Value<bool>())
                    {
                        readyUpdateUserData = true;
                        GameCore.instance.UpdatePlayerData();
                    }
                    else
                    {
                        Debug.LogError(d.GetValue("mes").ToString());
                    }
                }
            }
        }
    }
}