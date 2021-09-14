using CommunicationFormat;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UIAnimation;
using UnityEngine;
using UnityEngine.UI;
using CharacterCore;
using Account;
using TMPro;
using NotiSystem;
using UnityEngine.SceneManagement;
using System.Runtime.ExceptionServices;
using UnityEngine.Events;

namespace InGame
{
    enum InGameState
    {
        READY, CHOOSE_ACTION, GAME_PROCESSING, DONE
    }

    public class InGameManager : MonoBehaviour
    {
        [Header("In Game Manager")]
        [SerializeField] private string _id;
        public static InGameManager instance;

        [Header("-= Smooth Move Animation =-")]
        public SmoothMoveAnimation[] main_InputField;
        public SmoothMoveAnimation[] skill_InputField;

        [Header("-= Select Button =-")]
        public Button btn_SendPlayerAction;

        [Header("-= Players Data =-")]
        [SerializeField] TeamData myTeamData;
        [SerializeField] TeamData enemyTeamData;

        [Header("-= My Character =-")]
        [SerializeField] List<GameObject> characterPrefabs;
        [SerializeField] CharacterBase myCharacter;
        [SerializeField] CharacterBase enemyCharacter;

        [Header("-= My State =-")]
        [SerializeField] TMP_Text txt_CurrentState;
        [SerializeField] InGameState currentState;
        [SerializeField] InGameState beforeState;

        [Header("-= Choose Action State =-")]
        [SerializeField] List<CharacterState> chooseActionState;

        [Header("-= Test Button =-")]
        [SerializeField] Button btn_Test;
        [SerializeField] Button btn_CreateRandomTest;


        private void Awake()
        {
            //------------------------------------------------
            // Init Game Data
            //------------------------------------------------
            InitData();

            if (instance == null)
            {
                instance = this;
            }
            else
                Destroy(this.gameObject);

            //Button Actions
            btn_Test.onClick.AddListener(() => Send_ActionState());
            btn_CreateRandomTest.onClick.AddListener(() => Create_RandomActionState());

            Send_ReadyState();
            currentState = InGameState.DONE;
        }

        private void Update()
        {
            CheckSocketMessage();

            txt_CurrentState.text = currentState.ToString();
        }

        void InitData()
        {
            foreach(JObject obj in SocketCore.instance.MatchingReadyBuffer)
            {
                JObject data = JObject.Parse(obj["data"].ToString());
                _id = data["_id"].ToString();

                foreach(JObject player in JArray.Parse(data["player"].ToString()))
                {
                    TeamData tmpData = new TeamData();
                    tmpData.player1 = new CharacterData();
                    tmpData.player2 = new CharacterData();

                    JArray characters = JArray.Parse(player["characters"].ToString());
                    JObject character1_Data = JObject.Parse(characters[0].ToString());

                    tmpData.player1.SetCharacterType((CharacterType)System.Enum.Parse(typeof(CharacterType), character1_Data["name"].ToString().ToUpper()));
                    tmpData.player1.SetName(character1_Data["name"].ToString());
                    tmpData.player1.SetMaximumHp(character1_Data["maxHp"].Value<int>());
                    tmpData.player1.SetCurrentSpeed(character1_Data["speed"].Value<int>());

                    SkillData[] skillDatas = new SkillData[4];

                    //Default Info
                    skillDatas[0] = new SkillData();
                    skillDatas[0].SetName(character1_Data["defaultName"].ToString());
                    skillDatas[0].SetDamage(character1_Data["defaultDamage"].Value<int>());
                    skillDatas[0].SetMana(character1_Data["defaultMana"].Value<int>());
                    skillDatas[0].SetAction(skillDatas[0].GetName());

                    //Skills Info
                    for (int i = 1; i < 4; i++)
                    {
                        skillDatas[i] = new SkillData();
                        skillDatas[i].SetName(character1_Data["skill" + i + "Name"].ToString());
                        skillDatas[i].SetDamage(character1_Data["skill" + i + "Damage"].Value<int>());
                        skillDatas[i].SetMana(character1_Data["skill" + i + "Mana"].Value<int>());
                        skillDatas[i].SetAction(character1_Data["skill" + i + "Action"].ToString());
                    }
                    tmpData.player1.SetSkill_Info(skillDatas);

                    if(characters[1].ToString() != "")
                    {
                        JObject character2_Data = JObject.Parse(characters[1].ToString());

                        tmpData.player2.SetCharacterType((CharacterType)System.Enum.Parse(typeof(CharacterType), character2_Data["name"].ToString().ToUpper()));
                        tmpData.player2.SetName(character2_Data["name"].ToString());
                        tmpData.player2.SetMaximumHp(character2_Data["maxHp"].Value<int>());
                        tmpData.player2.SetCurrentSpeed(character2_Data["speed"].Value<int>());

                        skillDatas = new SkillData[4];

                        //Default Info
                        skillDatas[0] = new SkillData();
                        skillDatas[0].SetName(character2_Data["defaultName"].ToString());
                        skillDatas[0].SetDamage(character2_Data["defaultDamage"].Value<int>());
                        skillDatas[0].SetMana(character2_Data["defaultMana"].Value<int>());
                        skillDatas[0].SetAction(skillDatas[0].GetName());

                        //Skills Info
                        for (int i = 1; i < 4; i++)
                        {
                            skillDatas[i] = new SkillData();
                            skillDatas[i].SetName(character2_Data["skill" + i + "Name"].ToString());
                            skillDatas[i].SetDamage(character2_Data["skill" + i + "Damage"].Value<int>());
                            skillDatas[i].SetMana(character2_Data["skill" + i + "Mana"].Value<int>());
                            skillDatas[i].SetAction(character2_Data["skill" + i + "Action"].ToString());
                        }
                        tmpData.player2.SetSkill_Info(skillDatas);
                    }


                    if (player["username"].ToString().Equals(UserData.GetPlayerData().GetNickname()))
                    {
                        myTeamData = tmpData;
                    }
                    else
                        enemyTeamData = tmpData;
                }
            }

            SocketCore.instance.ReadyCompleteBuffer.Clear();
        }

        //------------------------------------------------
        // Socket Manager
        //------------------------------------------------
        void CheckSocketMessage()
        {
            switch (currentState)
            {
                case InGameState.READY:
                    CheckGameStart();
                    break;
                case InGameState.CHOOSE_ACTION:
                    CheckActionComplete();
                    break;
                case InGameState.GAME_PROCESSING:
                    break;
                case InGameState.DONE:
                    CheckReady();
                    break;
            }

            if(currentState != beforeState)
            {
                beforeState = currentState;

                switch (beforeState)
                {
                    case InGameState.READY:
                        break;
                    case InGameState.CHOOSE_ACTION:
                        break;
                    case InGameState.GAME_PROCESSING:
                        StartCoroutine(ProcessingLoop());
                        break;
                    case InGameState.DONE:
                        Send_ReadyState();
                        break;
                }
            }
        }

        //------------------------------------------------
        // Main Manager
        //------------------------------------------------
        IEnumerator ProcessingLoop()
        {
            int index = 0;
            while(myCharacter.GetCharacterStateLength() > index + 1)
            {
                myCharacter.SetCurrentActionIndex(index);
                enemyCharacter.SetCurrentActionIndex(index);

                if(myCharacter.ActionIsAttack())
                {
                    if(enemyCharacter.ActionIsAttack())
                    {
                        CharacterBase fir;
                        CharacterBase sec;

                        if (myCharacter.currentSpeed > enemyCharacter.currentSpeed)
                        {
                            fir = myCharacter;
                            sec = enemyCharacter;
                        }
                        else
                        {
                            fir = enemyCharacter;
                            sec = myCharacter ;
                        }

                        fir.ChangeEnemyAction(sec);
                        sec.ChangeEnemyAction(fir);

                        if (fir.ActionIsAttack())
                        {
                            fir.PlayCurrentAction();
                            sec.Hit();
                        }

                        yield return new WaitForSeconds(2.0f);

                        if (sec.ActionIsAttack())
                        {
                            sec.PlayCurrentAction();
                            fir.Hit();
                        }

                        Debug.Log(fir.GetCharacterState());
                        Debug.Log(sec.GetCharacterState());
                        yield return new WaitForSeconds(2.0f);
                    }
                }
                else
                {
                    myCharacter.PlayCurrentAction();
                    enemyCharacter.PlayCurrentAction();
                    yield return new WaitForSeconds(2.0f);
                }

                index++;
            }

            Debug.Log("Processing");
            currentState = InGameState.DONE;
        }

        //------------------------------------------------
        // Check Socket
        //------------------------------------------------
        void CheckReady() {
            foreach(JObject obj in SocketCore.instance.ReadyCompleteBuffer)
            {
                if (!obj["success"].Value<bool>())
                    NotiSystemManager.instacne.ShowError("E","Server ERROR",()=> { SceneManager.LoadScene("MainScene"); });

                currentState = InGameState.READY;
                Debug.Log("Client State is Ready");
            }

            SocketCore.instance.ReadyCompleteBuffer.Clear();
        }

        void CheckGameStart()
        {
            foreach (JObject obj in SocketCore.instance.GameStartBuffer)
            {
                currentState = InGameState.CHOOSE_ACTION;
                Debug.Log("Client State is Choose Action");
            }

            SocketCore.instance.GameStartBuffer.Clear();
        }

        void CheckActionComplete()
        {
            if (SocketCore.instance.ActionBuffer.Count < 2)
                return;

            List<CharacterState>[] states = new List<CharacterState>[2];
            states[0] = new List<CharacterState>();
            states[1] = new List<CharacterState>();
            
            int index = 0;
            int myIndex = 0;
            int enemyIndex = 0;

            foreach (JObject obj in SocketCore.instance.ActionBuffer)
            {
                foreach (string state in JArray.Parse(obj["action_data"].ToString()))
                {
                    Debug.Log(state);
                    states[index].Add((CharacterState)System.Enum.Parse(typeof(CharacterState), state));
                }

                if (obj["username"].ToString().Equals(UserData.GetPlayerData().GetNickname()))
                {
                    myIndex = index;
                }
                else
                {
                    enemyIndex = index;
                }

                index++;
            }

            myCharacter.UpdateStatesArray(states[myIndex].ToArray(), states[enemyIndex].Count);
            enemyCharacter.UpdateStatesArray(states[enemyIndex].ToArray(), states[myIndex].Count);

            SocketCore.instance.ActionBuffer.Clear();

            currentState = InGameState.GAME_PROCESSING;
            Debug.Log("Client State is PROCESSING");
        }

        void Send_ReadyState()
        {
            JObject obj = new JObject();
            obj.Add("room_id", _id);

            SocketCore.instance.socket.Emit("Ready", obj.ToString());
        }

        void Send_ActionState()
        {
            if (currentState != InGameState.CHOOSE_ACTION) return;

            JObject obj = new JObject();

            obj.Add("room_id", _id);
            obj.Add("username", UserData.GetPlayerData().GetNickname());
            obj.Add("action_data", "[\"" + System.String.Join("\", \"", chooseActionState.ToArray()) + "\"]");

            //foreach (string tmp in JArray.Parse(obj["action_data"].ToString()))
            //{
            //    Debug.Log(tmp.ToString());
            //}

            chooseActionState.Clear();
            SocketCore.instance.socket.Emit("Action", obj.ToString());
        }

        //------------------------------------------------
        // Button Actions
        //------------------------------------------------
        void Create_RandomActionState()
        {
            chooseActionState = new List<CharacterState>();

            chooseActionState.Add((CharacterState)(Random.Range((int)CharacterState.ATTACK, (int)CharacterState.AVOID)));
            chooseActionState.Add((CharacterState)(Random.Range((int)CharacterState.ATTACK, (int)CharacterState.AVOID)));
            chooseActionState.Add((CharacterState)(Random.Range((int)CharacterState.ATTACK, (int)CharacterState.AVOID)));
            chooseActionState.Add((CharacterState)(Random.Range((int)CharacterState.ATTACK, (int)CharacterState.AVOID)));
            chooseActionState.Add((CharacterState)(Random.Range((int)CharacterState.ATTACK, (int)CharacterState.AVOID)));
        }
    }
}

