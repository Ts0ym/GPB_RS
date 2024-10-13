using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NativeWebSocket;
using System.Net;
using System.Net.Sockets;
using UnityEngine.Events;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Text;
using System.Threading;
using AwakeComponents.DebugUI;

namespace AwakeComponents.WebSockets
{
    [ComponentInfo("1.0.2", "27.08.2024")]
    public class WebSocketClient : MonoBehaviour, IDebuggableComponent
    {
        public int port = 80;

        public UnityEvent onOpen = new UnityEvent();
        public UnityEvent<string> onMessage = new UnityEvent<string>();
        public UnityEvent<string> onError = new UnityEvent<string>();
        public UnityEvent<WebSocketCloseCode> onClose = new UnityEvent<WebSocketCloseCode>();

        public UnityEvent<string> onLocalIpFound = new UnityEvent<string>();

        private List<string> foundIPs = new List<string>();

        private WebSocket websocket;

        public static WebSocketClient instance;

        string serverIp = "127.0.0.1";
        private int pingsCountNotFinished;
        
        float lastReconnectTime = 0f;
        
        public bool debugInEditor = false;
        public bool debugInBuild = true;

        public void RenderDebugUI()
        {
            if (websocket == null)
                ConnectForm();
            else if (websocket != null) {
                if (websocket.State != WebSocketState.Open)
                    ConnectForm();
                else {
                    if (GUILayout.Button("Disconnect")) {
                        DisconnectWebSocket();
                    }
                }
            }
        }

        public async void DisconnectWebSocket()
        {
            if (websocket != null && websocket.State == WebSocketState.Open)
            {
                await websocket.Close();
                websocket = null;
                Debug.Log("WebSocket disconnected.");
            }
            else
            {
                websocket = null;
                Debug.Log("WebSocket was not connected.");
            }
        }

        private void ConnectForm()
        {
            serverIp = GUILayout.TextField(serverIp);

            if (GUILayout.Button("Connect!"))
                Connect(serverIp);

            if (websocket != null)
                GUILayout.Label("Status: " + websocket.State.ToString());
            else
                GUILayout.Label("No connections was established");
        }

        void Start()
        {
            DebugLog("Starting WebSocket client");
            
            instance = this;
            
            // Load IP from PlayerPrefs
            if (PlayerPrefs.HasKey("serverIp")) {
                serverIp = PlayerPrefs.GetString("serverIp");
                
                Connect(serverIp);
            }

            //Autoconnect(); // Автоматический поиск сервера в подсети
        }
        
        void DebugLog(string message)
        {
            #if UNITY_EDITOR
                if (debugInEditor)
                    Debug.Log("[WebSocketClient] " + message);
            #else
                if (debugInBuild)
                    Debug.Log("[WebSocketClient] " + message);
            #endif
        }

        async void Connect(string ip)
        {
            DebugLog("Connecting to " + ip);

            if (websocket != null)
            {
                DebugLog("Closing existing WebSocket");
                await websocket.Close();
            }

            if (ip == null || ip == "")
            {
                DebugLog("No WebSocket server IP specified!");
                return;
            }

            serverIp = ip;
            
            // Save IP to PlayerPrefs
            PlayerPrefs.SetString("serverIp", serverIp);

            websocket = new WebSocket("ws://" + serverIp + ":" + port);

            BindEvents(websocket);

            await websocket.Connect();

            DebugLog("Connecting status: " + websocket.State);

            if (websocket.State != WebSocketState.Open && foundIPs.Contains(serverIp))
            {
                websocket = null;

                foundIPs.Remove(serverIp);

                if (foundIPs.Count > 0)
                    Connect(foundIPs[0]);
                else
                {
                    DebugLog("No WebSocket server found!");

                    Autoconnect();
                }
            }
        }

        // Автоматический поиск сервера в подсети
        private async void Autoconnect()
        {
            string host = Dns.GetHostName();
            IPHostEntry IPs = Dns.GetHostEntry(host); // Чекаем СВОИ айпишники
            string currentIp = "192.168.0.0"; // Сейчас заменится, но лучше указать дефолтный

            foreach (IPAddress ip in IPs.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    currentIp = ip.ToString(); // Получили свой внутренний IP // TODO: А уверен что IPv4?
            }

            // Получили "XXX.XXX.XXX." - а в конце будем перебирать пингом
            string currentIpZone = currentIp.Split('.')[0] + "." + currentIp.Split('.')[1] + "." + currentIp.Split('.')[2] + ".";

            for (int i = 0; i < 255; i++)
                await PingHost(currentIpZone + i); // We are NOT awaiting this, cause it causes long time waiting on app start

            // Все Пинги выполняются асинхронно! Мы не ждём предыдущий перед следующим.
        }

        public async Task<bool> PingHost(string ip)
        {
            bool isSuccess = false;

            pingsCountNotFinished++; // Чтоб трекать, когда закончим пинговать всю подсеть

            System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();
            //ping.PingCompleted += new PingCompletedEventHandler(OnPingComplete); не нужно

            try
            {
                var reply = await ping.SendPingAsync(ip, 5); // 5 ms таймаут, вроде

                if (reply.Status == IPStatus.Success)
                {
                    DebugLog("Ping success: " + ip);

                    // Добавляем в найденные айпишники, чтоб потом, когда все закончат пинговаться, начать тестить подключения именно к WS server
                    foundIPs.Add(ip);

                    onLocalIpFound?.Invoke(ip);

                    isSuccess = true;
                }
            }
            catch (SocketException)
            {
            }

            // Незаконченных пингов стало меньше
            pingsCountNotFinished--;

            // Если все пинги закончились TODO: а если мы пинговали один айпишник в рандомном другом месте?
            if (pingsCountNotFinished == 0)
                Connect(foundIPs[0]); // Подключаемся начиная с первого (обычно это роутер 192.168.0.1)

            return isSuccess;
        }

        private void BindEvents(WebSocket websocket)
        {
            websocket.OnOpen += () =>
            {
                DebugLog("WebSocket connection opened");
                
                onOpen?.Invoke();
            };

            websocket.OnError += async (e) =>
            {
                DebugLog("WebSocket connection error: " + e);

                WebSocket _ws = websocket;

                websocket = null;

                onError?.Invoke(e);

                await _ws.Close();
            };

            websocket.OnClose += (e) =>
            {
                DebugLog("WebSocket connection closed!");

                websocket = null;

                onClose?.Invoke(e);
            };

            websocket.OnMessage += (bytes) =>
            {
                var message = System.Text.Encoding.UTF8.GetString(bytes);
                
                DebugLog("Received WebSocket message at " + Time.time + " (" + bytes.Length + " bytes): " + message);
                
                onMessage?.Invoke(message);
            };
        }

        void Update()
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            if (websocket != null)
                websocket.DispatchMessageQueue();
#endif
            
            // Check if we have to reconnect once per 5 seconds
            if ((websocket == null || websocket.State != WebSocketState.Open) && Time.time - lastReconnectTime > 5f)
            {
                lastReconnectTime = Time.time;

                Connect(serverIp);
            }
        }
        
        public async void Send(string message)
        {
            DebugLog("Sending message: " + message);

            if (websocket == null) {
                DebugLog("Соединение еще не установлено. Скорее всего, WebScoketClient не успел подключиться к серверу. Подожди чуть-чуть))");
            }

            if (websocket.State == WebSocketState.Open)
            {
                await websocket.SendText(message);
            }
            else
            {
                DebugLog("WebSocket connection is not open!");
            }
        }

        private async void OnApplicationQuit()
        {
            if (websocket != null)
                await websocket.Close();
        }
    }
}
