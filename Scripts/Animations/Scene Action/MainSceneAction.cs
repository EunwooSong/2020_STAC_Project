using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Account.Team;
using Appdata.Option;
using TMPro;
using Core;
using Account;
using CommunicationFormat;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Xml;
using UIAnimation;

public class MainSceneAction : MonoBehaviour
{
    [Header("-= Move Buttons =-")]
    [SerializeField] private Button btn_ActiveShop;
    [SerializeField] private Button btn_ActiveTeam;
    [SerializeField] private Button btn_ActiveFriendsField;
    [SerializeField] private Button btn_ActiveOption;
    [SerializeField] private Button btn_ActiveMatching;
    [SerializeField] private Button btn_CancleMatching;

    [Header("-= Main Scene Info =-")]
    [SerializeField] private SpriteRenderer sprite_Team_1;
    [SerializeField] private SpriteRenderer sprite_Team_2;
    [SerializeField] private TMP_Text txt_username;
    [SerializeField] private TMP_Text txt_cash;
    [SerializeField] private TMP_Text txt_coin;
    [SerializeField] private Slider sli_xp;
    [SerializeField] private TMP_Text txt_matchingTimer;

    //[Header("-= Scene Transition =-")]
    //[SerializeField] private Image transition_Effect_Black;

    [Header("-= Each Manager =-")]
    [SerializeField] private OptionManager option_Manager;
    [SerializeField] private ShopManager shop_Manager;
    [SerializeField] private FriendsFieldManager friends_Manager;

    [Header("-= Characters Sprites =-")]
    public Dictionary<CharacterType, Sprite> charSpriteData;

    [Header("-= Scene Transition =-")]
    public Animator anim_SceneTransition;
    public SmoothMoveAnimation ui_MatchingField;

    bool isUpdateData = false;
    bool isMatching = false;

    private void Awake()
    {
        charSpriteData = new Dictionary<CharacterType, Sprite>();

        charSpriteData.Add(CharacterType.MAROS,     Resources.Load<Sprite>("Character_Info/MAROS"));
        charSpriteData.Add(CharacterType.MIYA,      Resources.Load<Sprite>("Character_Info/MIYA"));
        charSpriteData.Add(CharacterType.MR_MA,     Resources.Load<Sprite>("Character_Info/MR_MA"));
        charSpriteData.Add(CharacterType.SM_124,    Resources.Load<Sprite>("Character_Info/SM_124"));

        ui_MatchingField.ChangeIndex(0);
    }

    void Start()
    {
        UpdateShowUserData();

        btn_ActiveTeam.onClick.AddListener(() => { Btn_ActiveTeam(); });
        btn_ActiveShop.onClick.AddListener(() => { Btn_ActiveShop(); });
        btn_ActiveOption.onClick.AddListener(() => { Btn_ActiveOption(); });
        btn_ActiveFriendsField.onClick.AddListener(() => { Btn_ActiveFriendsField(); });
        btn_ActiveMatching.onClick.AddListener(() => { Btn_ActiveMatching(); });
        btn_CancleMatching.onClick.AddListener(() => { Btn_CancleMatching(); });
    }

    //------------------------------------------------
    // Main Loop
    //------------------------------------------------
    void Update()
    {
        if (GameCore.instance.dataState != DataState.DONE)
            isUpdateData = true;

        if(isUpdateData)
        {
            if(GameCore.instance.dataState == DataState.DONE)
            {
                UpdateShowUserData();
            }
        }

        ui_MatchingField.ChangeIndex((isMatching) ? 1 : 0);

        SocketController();
    }

    void UpdateShowUserData()
    {
        isUpdateData = false;

        // Reset
        sprite_Team_1.sprite = null;
        sprite_Team_2.sprite = null;

        // Set Team Info
        foreach (CharacterData d in UserData.GetPlayerData().GetCharactersData())
        {
            if (d.GetSelected() == 0)
            {
                sprite_Team_1.sprite = charSpriteData[d.GetCharacterType()];

                // Set Main Character Xp
                sli_xp.value = d.GetCurrentXp();
                sli_xp.maxValue = 4;
            }
            else if (d.GetSelected() == 1)
            {
                sprite_Team_2.sprite = charSpriteData[d.GetCharacterType()];
            }
            else
                continue;
        }

        // Set Cash, Coin Data
        txt_username.text = UserData.GetPlayerData().GetNickname();
        txt_cash.text = UserData.GetPlayerData().GetMoneyData().GetCurrentGem().ToString();
        txt_coin.text = UserData.GetPlayerData().GetMoneyData().GetCurrentCoin().ToString();
    }

    //------------------------------------------------
    // Socket Contorl
    //------------------------------------------------
    void SocketController()
    {
        if(SocketCore.instance)
        {
            CheckMatching();
            CheckMatchingCancle();
            CheckMatchingReady();
        }

        btn_ActiveMatching.interactable = !isMatching;
    }

    void CheckMatching()
    {
        if(SocketCore.instance.MatchingBuffer.Count != 0)
        {
            //NotiSystem.NotiSystemManager.instacne.ShowMessage("매칭이 시작되었습니다!");
            
            SocketCore.instance.MatchingBuffer.Clear();
            
            isMatching = true;

            StartCoroutine(Socket_ReadyMatching());
        }
    }

    void CheckMatchingCancle()
    {
        if (SocketCore.instance.MatchingCancleBuffer.Count != 0)
        {
            NotiSystem.NotiSystemManager.instacne.ShowMessage("매칭이 취소되었습니다!");

            SocketCore.instance.MatchingCancleBuffer.Clear();

            isMatching = false;
        }
    }

    void CheckMatchingReady()
    {
        if (SocketCore.instance.MatchingReadyBuffer.Count != 0)
        {
            //NotiSystem.NotiSystemManager.instacne.ShowMessage("매칭이 성사되었습니다!");

            SceneManager.LoadScene("GameScene");

            isMatching = false;
        }
    }

    IEnumerator Socket_ReadyMatching()
    {
        float timer = 0.0f;

        while(isMatching)
        {
            timer += Time.deltaTime;

            txt_matchingTimer.text = (int)(timer / 60) + " : " + (int)(timer % 60);

            yield return null;
        }

        txt_matchingTimer.text = " : ";
    }

    //------------------------------------------------
    // Add Button Actions
    //------------------------------------------------
    void Btn_ActiveTeam()
    {
        StartCoroutine(ActiveTeam());
    }

    void Btn_ActiveShop()
    {

    }

    void Btn_ActiveFriendsField()
    {

    }

    void Btn_ActiveOption()
    {
        option_Manager.StartOptionField();
    }

    void Btn_ActiveMatching()
    {
        JObject obj = new JObject();
        obj.Add("username", UserData.GetPlayerData().GetNickname());
        obj.Add("rankPoint", UserData.GetPlayerData().GetRankData().GetRankScoreData());
        
        int teamLength = 0;
        foreach(CharacterData data in UserData.GetPlayerData().GetCharactersData())
        {
            if (data.GetSelected() != -1)
                teamLength++;
        }

        if(teamLength <= 0)
        {
            NotiSystem.NotiSystemManager.instacne.ShowError("error_01", "팀원이 없습니다! (니팀버려?)", () => { });
            return;
        }

        obj.Add("characterNumber", teamLength);


        SocketCore.instance.socket.Emit("Matching", obj.ToString());

        Debug.Log(obj.ToString());
    }

    void Btn_CancleMatching()
    {
        JObject obj = new JObject();
        obj.Add("username", UserData.GetPlayerData().GetNickname());

        SocketCore.instance.socket.Emit("Matching__Cancel", obj.ToString());
    }

    IEnumerator ActiveTeam()
    {
        anim_SceneTransition.SetTrigger("Transition_In");
        AsyncOperation op = SceneManager.LoadSceneAsync("TeamManagerScene");
        op.allowSceneActivation = false;

        yield return new WaitForSeconds(1.0f);

        op.allowSceneActivation = true;
    }

    IEnumerator ActiveShop()
    {
        yield return null;
    }

    IEnumerator ActiveFriendsField()
    {
        yield return null;
    }
}
