using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Quobject.SocketIoClientDotNet.Client;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Appdata;
using Newtonsoft.Json.Linq;
using System.Data;

public class SocketCore : MonoBehaviour
{
    public static SocketCore instance = null;
    public Socket socket;

    [Header("Main Scene Buffer")]
    public List<JObject> MatchingBuffer;
    public List<JObject> MatchingReadyBuffer;
    public List<JObject> MatchingCancleBuffer;

    [Header("In Game Buffer")]
    public List<JObject> ReadyCompleteBuffer;
    public List<JObject> GameStartBuffer;
    public List<JObject> ActionBuffer;
    
    public List<JObject> ErrorBuffer;


    // Start is called before the first frame update
    void Awake()
    {
        Application.runInBackground = true;
        if (instance == null)
        {
            instance = this;

            socket = IO.Socket("https://hellfightsocket.run.goorm.io");

            MatchingBuffer = new List<JObject>();
            MatchingReadyBuffer = new List<JObject>();
            MatchingCancleBuffer = new List<JObject>();
            ReadyCompleteBuffer = new List<JObject>();
            GameStartBuffer = new List<JObject>();
            ActionBuffer = new List<JObject>();
            ErrorBuffer = new List<JObject>();

            socket.On(Socket.EVENT_CONNECT, () =>
            {
                socket.On("Matching__Complete", (data) => {
                    MatchingBuffer.Add(JObject.Parse(data.ToString()));
                });

                socket.On("Ready__Start", (data) => {
                    Debug.Log(data.ToString());
                    MatchingReadyBuffer.Add(JObject.Parse(data.ToString()));
                });

                socket.On("Matching__Cancel__Complete", (data) => {
                    MatchingCancleBuffer.Add(JObject.Parse(data.ToString()));
                });

                socket.On("Ready__Complete", (data) => {
                    ReadyCompleteBuffer.Add(JObject.Parse(data.ToString()));
                });

                socket.On("Game__Start", (data) => {
                    GameStartBuffer.Add(JObject.Parse(data.ToString()));
                });

                socket.On("Action__Complete", (data) => {
                    Debug.Log(data.ToString());
                    ActionBuffer.Add(JObject.Parse(data.ToString()));
                });

                //Error Socket
                socket.On("Socket__Fail", (data) => {
                    ErrorBuffer.Add(JObject.Parse(data.ToString()));
                });

                Debug.Log("Socket Connected - " + System.DateTime.Now.ToString());
            });
        }
        else if (instance != null)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }
}
