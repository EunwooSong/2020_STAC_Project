using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Appdata;
using UnityEngine.Rendering.Universal;
using Account;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using System.Linq;
using CommunicationFormat;
using NotiSystem;
using Microsoft.Win32.SafeHandles;
using System.Diagnostics.PerformanceData;

namespace Core
{
    public enum DataState
    {
        READY, PROCESS, DONE, ERROR
    }

    public class GameCore : MonoBehaviour
    {
        public static GameCore instance;

        [SerializeField] private UniversalRenderPipelineAsset[] rpAssets;

        [Header("State")]
        public DataState dataState = DataState.READY;

        // Start is called before the first frame update
        void Start()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this.gameObject);

                InitGameOptions();
            }
            else
                Destroy(this);
        }

        //------------------------------------------------
        // Initialize Game Graphic(Render Pipeline Asset)
        //------------------------------------------------

        private void InitGameOptions()
        {
            Application.runInBackground = true;

            ApplicationData.LoadApplicationData();

            UpdateGraphicQuailty();


            Debug.Log("Update Graphic Quailty Done!");
        }

        public void UpdateGraphicQuailty()
        {
            Application.targetFrameRate = ApplicationData.optionData.frameRate;
            switch ((GraphicQuality)ApplicationData.optionData.graphicQuality)
            {
                case GraphicQuality.LOW:
                    QualitySettings.SetQualityLevel(0, true);
                    //GraphicsSettings.renderPipelineAsset = rpAssets[0];
                    break;
                case GraphicQuality.MEDIUM:
                    QualitySettings.SetQualityLevel(1, true);
                    //GraphicsSettings.renderPipelineAsset = rpAssets[1];
                    break;
                case GraphicQuality.HIGH:
                    QualitySettings.SetQualityLevel(2, true);
                    //GraphicsSettings.renderPipelineAsset = rpAssets[2];
                    break;
                default:
                    QualitySettings.SetQualityLevel(1, true);
                    //GraphicsSettings.renderPipelineAsset = rpAssets[1];
                    break;
            }
        }

        public void UpdatePlayerData()
        {
            dataState = DataState.READY;
            StartCoroutine(Get_PlayerData());
        }

        //------------------------------------------------
        // Google Version Check
        //------------------------------------------------
        private void GooglePlayVersionCheck()
        {

        }


        //------------------------------------------------
        //  Connect Server (Get Player Data)
        //------------------------------------------------
        IEnumerator Get_PlayerData()
        {
            yield return new WaitForEndOfFrame();

            using (var w = UnityWebRequest.Get(ApplicationData.GetServerURL() + "auth/info"))
            {
                dataState = DataState.PROCESS;
                w.SetRequestHeader("Authorization", "Bearer " + UserData.GetAccessToken());
                yield return w.SendWebRequest();

                if (w.isNetworkError || w.isHttpError)
                {
                    Debug.LogError(w.error);
                    dataState = DataState.ERROR;
                    NotiSystemManager.instacne.ShowError(w.responseCode.ToString(), "데이터를 불러올 수 없습니다...!", () => { UpdatePlayerData(); }, "다시 시도");
                }
                else
                {
                    //Debug.Log(w.downloadHandler.text);
                    JObject res = JObject.Parse(w.downloadHandler.text);
                    JObject data = JObject.Parse(res["data"].ToString());

                    UserData.SetPlayerData(new PlayerData());
                    UserData.GetPlayerData().SetNickname(data["username"].ToString());
                    UserData.GetPlayerData().GetMoneyData().SetCurrentCoin(data["money"].Value<int>());
                    UserData.GetPlayerData().GetMoneyData().SetCurrentGem(data["cash"].Value<int>());
                    UserData.GetPlayerData().SetRankData(new RankData(0, data["rankPoint"].Value<int>()));

                    CharacterData[] charList = new CharacterData[JArray.Parse(data["characters"].ToString()).Count];
                    int index = 0;

                    //Get Selected
                    JArray selected = JArray.Parse(data["onCharacters"].ToString());
                    CharacterType[] selectType = new CharacterType[2];

                    for (int i = 0; i < selectType.Length; i++)
                        if (selected[i].ToString() != "")
                            selectType[i] = (CharacterType)System.Enum.Parse(typeof(CharacterType), selected[i].ToString().ToUpper());
                        else
                            selectType[i] = CharacterType.NONE;
                    
                    //Debug.Log(selectType[0].ToString() + selectType[1].ToString());


                    foreach (JObject obj in JArray.Parse(data["characters"].ToString()))
                    {
                        CharacterData tmp = new CharacterData();

                        tmp.SetRank(obj["rating"].ToString());
                        tmp.SetCharacterType((CharacterType)System.Enum.Parse(typeof(CharacterType), obj["name"].ToString().ToUpper()));
                        tmp.SetCurrentXp(obj["amount"].Value<int>());
                        tmp.SetName(obj["name"].Value<string>().ToUpper());
                        tmp.SetLevel(obj["level"].Value<int>());
                        tmp.SetMaximumHp(obj["currentHp"].Value<int>());
                        tmp.SetCurrentSpeed(obj["speed"].Value<int>());
                        tmp.SetCharacterInfo(obj["history"].ToString());

                        //Set Character Selected
                        for (int i = 0; i < selectType.Length; i++)
                            if (tmp.GetCharacterType().Equals(selectType[i]))
                            {
                                tmp.SetSelected(i);
                                break;
                            }
                            else
                                tmp.SetSelected(-1);


                        SkillData[] skillDatas = new SkillData[4];

                        //Default Info
                        skillDatas[0] = new SkillData();
                        skillDatas[0].SetName(obj["defaultName"].ToString());
                        skillDatas[0].SetDamage(obj["defaultDamage"].Value<int>());
                        skillDatas[0].SetMana(obj["defaultMana"].Value<int>());
                        skillDatas[0].SetAction("기본 공격 입니다 " + skillDatas[0].GetName());

                        //Skills Info
                        for (int i = 1; i < 4; i++)
                        {
                            skillDatas[i] = new SkillData();
                            skillDatas[i].SetName(obj["skill" + i + "Name"].ToString());
                            skillDatas[i].SetDamage(obj["skill" + i + "Damage"].Value<int>());
                            skillDatas[i].SetMana(obj["skill" + i + "Mana"].Value<int>());
                            skillDatas[i].SetAction(obj["skill" + i + "Action"].ToString());
                        }

                        tmp.SetSkill_Info(skillDatas);
                        
                        charList[index++] = tmp;
                    }

                    //Debug.Log(charList[0].GetSelected().ToString());

                    UserData.GetPlayerData().SetCharactersData(charList);

                    dataState = DataState.DONE;
                }
            }
        }
    }
}
