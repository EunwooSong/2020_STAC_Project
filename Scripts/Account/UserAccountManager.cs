//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Networking;
//using GooglePlayGames;
//using GooglePlayGames.BasicApi;
//using UnityEngine.UI;
//using TMPro;
//using CommunicationFormat;
//using Appdata;
//using Newtonsoft.Json.Linq;
//using Newtonsoft.Json;
//using NotiSystem;
//using Core;
//using UnityEngine.SceneManagement;

//namespace Account
//{
//    public enum ServerState
//    {
//        READY, CONNECT_GOOGLE, CONNECT_SIGNIN, CONNECT_SIGNUP, CONNECT_SERVER, CONNECT_DONE, ERROR
//    }

//    public class UserAccountManager : MonoBehaviour
//    {
//        [Header("Action Buttons")]
//        [SerializeField] private Button btn_SignIn;
//        [SerializeField] private Button btn_SignUp;
//        [SerializeField] private GameObject ui_animation;

//        [Header("Input Field")] //전송에 필요한 변수들
//        [SerializeField] private TMP_InputField input_userName;
//        [SerializeField] private TMP_InputField input_googleToken;

//        [Header("Values")]
//        [SerializeField]bool isUnSignUp = false;
//        [SerializeField]bool isUserDataReady = false;
//        [SerializeField] public ServerState currentState { get;  private set; }
//        [SerializeField] public DataState beforeDataState { get;  private set; }
//        private ServerState beforeState;


//        [Header("Show State")]
//        public TMP_Text text_currentState;
//        public TMP_Text text_currentGuid;

//        // Start is called before the first frame update
//        private void Awake()
//        {
//            //------------------------------------------------
//            // Initialize GPGS
//            //------------------------------------------------
//            PlayGamesClientConfiguration config = new PlayGamesClientConfiguration
//                .Builder()
//                .RequestServerAuthCode(false)
//                .RequestEmail()
//                .RequestIdToken()
//                .Build();

//            PlayGamesPlatform.InitializeInstance(config);
//            PlayGamesPlatform.DebugLogEnabled = false;

//            PlayGamesPlatform.Activate();

//            currentState = ServerState.READY;
//            beforeState = ServerState.ERROR;

//            btn_SignIn.onClick.AddListener(StartSignIn);
//            btn_SignUp.onClick.AddListener(StartSignUp);

//            text_currentState.text = "Current Version : " + Application.version + "\n";
//            text_currentState.text += beforeDataState.ToString() + " . . .";
//        }

//        private void Start()
//        {
//            AccessGoogleAuth();
//            isUnSignUp = false;
//        }

//        private void Update()
//        {

//            if (beforeDataState != GameCore.instance.dataState)
//            {
//                beforeDataState = GameCore.instance.dataState;

//                text_currentState.text = "Current Version : " + Application.version + "\n";
//                text_currentState.text += beforeDataState.ToString() + " . . .";

//                switch (GameCore.instance.dataState)
//                {

//                    case DataState.READY:
//                        text_currentGuid.text = "서버 연결 중 . . .";
//                        break;
//                    case DataState.PROCESS:
//                        text_currentGuid.text = "데이터 불러오는 중 . . .";
//                        break;
//                    case DataState.DONE:
//                        text_currentGuid.text = "완료 . . .";
//                        break;
//                }
//            }
//            else
//            {
//                if (beforeState != currentState)
//                {
//                    beforeState = currentState;


//                    //if(currentState == )


//                    text_currentState.text = "Current Version : " + Application.version + "\n";
//                    text_currentState.text += currentState.ToString() + " . . .";
//                    switch (currentState)
//                    {
//                        case ServerState.READY:
//                            text_currentGuid.text = "터치하여 계속 . . .";
//                            break;
//                        case ServerState.CONNECT_GOOGLE:
//                            text_currentGuid.text = "구글 접속 중 . . .";
//                            break;
//                        case ServerState.CONNECT_SIGNIN:
//                            text_currentGuid.text = "서버 로그인 중 . . .";
//                            break;
//                        case ServerState.CONNECT_SIGNUP:
//                            text_currentGuid.text = "서버 회원가입 중 . . .";
//                            break;
//                        case ServerState.CONNECT_SERVER:
//                            text_currentGuid.text = "서버 연결 중 . . .";
//                            break;
//                        case ServerState.CONNECT_DONE:
//                            text_currentGuid.text = "완료 . . .";
//                            btn_SignIn.onClick.RemoveAllListeners();
//                            btn_SignIn.onClick.AddListener(() => { SceneManager.LoadScene("MainScene"); });
//                            break;
//                        case ServerState.ERROR:
//                            text_currentGuid.text = "터치하여 계속 . . .";
//                            break;
//                    }
//                }

//            }

//            if (isUnSignUp)
//            {
//                ui_animation.GetComponent<NotiMessageCard>().animtionFadeIn = true;
//                ui_animation.SetActive(true);
//            }
//            else
//            {
//                ui_animation.GetComponent<NotiMessageCard>().animtionFadeIn = false;

//                if (ui_animation.transform.GetChild(1).GetComponent<RectTransform>().localScale.x < 0.001f)
//                    ui_animation.SetActive(false);
//            }
//        }

//        private void AccessGoogleAuth()
//        {
//            if (!PlayGamesPlatform.Instance.localUser.authenticated)
//            {
//                Social.localUser.Authenticate(result =>
//                {
//                    if (!result)
//                    {
//                        currentState = ServerState.ERROR;
//                        Debug.LogError("Google SignIn Failed");
//                        return;
//                    }
//                    else
//                    {
//                        Debug.Log("Google Sign In Successed");
//                        currentState = ServerState.CONNECT_GOOGLE;
//                    }
//                });
//            }
//        }

//        private string GetGoogleIDToken()
//        {
//            if (PlayGamesPlatform.Instance.localUser.authenticated)
//            {
//                string _IDtoken = PlayGamesPlatform.Instance.GetIdToken();
//                return _IDtoken;
//            }
//            else
//            {
//                Debug.Log("Connecting GPGS. . . Please try again later");
//                AccessGoogleAuth();
//                return null;
//            }
//        }

//        private void StartSignIn()
//        {
//            currentState = ServerState.CONNECT_SIGNIN;
            
//            StartCoroutine(SendSignIn());
//        }

//        private void StartSignUp()
//        {
//            currentState = ServerState.CONNECT_SIGNUP;

//            StartCoroutine(SenSignUp());
//        }

//        //SignIn에 필요한 값들 추가 예정
//        IEnumerator SendSignIn()
//        {
//            yield return new WaitForEndOfFrame();

//            WWWForm form = new WWWForm();

//            if (input_googleToken.text != "")
//                form.AddField("google_uuid", input_googleToken.text);
//            else
//                form.AddField("google_uuid", GetGoogleIDToken());

//            using (var w = UnityWebRequest.Post(ApplicationData.GetServerURL() + "auth/signin", form))
//            {
//                currentState = ServerState.CONNECT_SERVER;

//                yield return w.SendWebRequest();

//                if (w.isNetworkError || w.isHttpError)
//                {
//                    Debug.LogError(w.error);
//                    currentState = ServerState.ERROR;
//                    //NotiSystemManager.instacne.ShowError(w.responseCode.ToString(), "서버와 연결할 수 없습니다..!", () => { StartSignIn(); }, "다시 시도");
//                    if(w.responseCode == 400)
//                    {
//                        isUnSignUp = true;
//                    }
//                    else
//                    {
//                        NotiSystemManager.instacne.ShowError(w.responseCode.ToString(), "서버와 연결할 수 없습니다..!", () => { StartSignIn(); }, "다시 시도");
//                    }
//                }

//                else
//                {
//                    JObject res = JObject.Parse(w.downloadHandler.text);

//                    if(res["success"].Value<bool>())
//                    {
//                        UserData.SetAccessToken(res["accessToken"].Value<string>());

//                        //Scene 이동
//                        currentState = ServerState.CONNECT_DONE;
//                        isUserDataReady = true;
//                        GameCore.instance.UpdatePlayerData();
//                    }
//                    else
//                    {
//                        isUnSignUp = true;
//                    }
//                }
//            }
//        }

//        //SignUp에 필요한 값들 추가 예정
//        IEnumerator SenSignUp()
//        {
//            yield return new WaitForEndOfFrame();

//            WWWForm form = new WWWForm();

//            if (input_googleToken.text != "")
//                form.AddField("google_uuid", input_googleToken.text);
//            else
//                form.AddField("google_uuid", GetGoogleIDToken());

//            form.AddField("username", input_userName.text);

//            using (var w = UnityWebRequest.Post(ApplicationData.GetServerURL() + "auth/signup", form))
//            {
//                currentState = ServerState.CONNECT_SERVER;

//                yield return w.SendWebRequest();

//                if (w.isNetworkError || w.isHttpError)
//                {
//                    Debug.LogError(w.error);
//                    currentState = ServerState.ERROR;
//                }

//                else
//                {
//                    JObject res = JObject.Parse(w.downloadHandler.text  );

//                    if (res["success"].Value<bool>())
//                    {
//                        Debug.Log("Start Sign In");
//                        isUnSignUp = false;
//                        StartSignIn();
//                    }
//                }
//            }
//        }
//    }
//}

