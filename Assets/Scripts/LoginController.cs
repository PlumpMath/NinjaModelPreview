using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

public class LoginController : MonoBehaviour {

    public string serverHost = "localhost";
    public int serverPort = 8081;

    public Canvas statusCanvas;
    public Image loginPanel;
    public Text statusText;
    public InputField loginField;
    public InputField passwordField;
    public Button submitButton;
    public MapController map;

    private TcpClient loginSocket = null;
    private TcpClient gameSocket = null;
    private byte[] socketBuffer;
    private LinkedList<ByteArrayContainer> messageQueue = new LinkedList<ByteArrayContainer>();
    private string storedKey = "";
    private string storedSecret = "";
    private string authToken = "";
    private string storedHost = "";
    private int storedPort = 0;

    private Vector2 baseLoginFieldPosition;
    private Vector2 basePasswordFieldPosition;
    private Vector2 baseSubmitButtonPosition;

    private int updates = 0;

    private bool waitingForInternet = false;
    private int waitingConnectType = -1;

    public PlayerViewMessage playerView = null;

    // Use this for initialization
    void Start () {

        Debug.Log("LoginController.Start()");

        baseLoginFieldPosition = loginField.image.rectTransform.anchoredPosition;
        basePasswordFieldPosition = passwordField.image.rectTransform.anchoredPosition;
        baseSubmitButtonPosition = submitButton.image.rectTransform.anchoredPosition;

        statusText.text = "Enter your credentials";

        submitButton.onClick.AddListener(delegate() {
            if(loginField.text == "")
            {
                statusText.text = "Please, enter your login";
                return;
            }
            if (passwordField.text == "")
            {
                statusText.text = "Please, enter your password";
                return;
            }
            Connect();
        });

        //
        //serverHost = "localhost";
        //

        if (PlayerPrefs.GetInt("LastVersion", 5) < 6)
        {
            storedKey = "";
            storedSecret = "";
            PlayerPrefs.SetInt("LastVersion", 6);
            PlayerPrefs.SetString("CredentialsKey", "");
            PlayerPrefs.SetString("CredentialsSecret", "");
        }

        storedKey = PlayerPrefs.GetString("CredentialsKey", "");
        storedSecret = PlayerPrefs.GetString("CredentialsSecret", "");

        if (storedKey != "" && storedSecret != "")
        {
            submitButton.image.rectTransform.anchoredPosition = new Vector2(0.0f, -2000.0f);
            loginField.image.rectTransform.anchoredPosition = new Vector2(0.0f, -2000.0f);
            passwordField.image.rectTransform.anchoredPosition = new Vector2(0.0f, -2000.0f);
            statusText.text = "Connecting...";
            //Connect();
        }

        statusText.text = "Connecting...";
        Connect();

        statusCanvas.enabled = true;

    }

    public void CheckConnection()
    {
        if(gameSocket != null && !gameSocket.Client.Connected)
        {
            Connect(storedHost, storedPort, authToken);
        }
    }

    void Connect()
    {
        waitingConnectType = -1;
        waitingForInternet = false;
        statusCanvas.enabled = true;
        messageQueue.Clear();
        statusText.text = "Connecting...";
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            statusText.text = "Internet not reachable";
            waitingConnectType = 1;
            waitingForInternet = true;
            return;
        }
        loginSocket = new TcpClient();
        loginSocket.BeginConnect(serverHost, serverPort, new AsyncCallback(LoginConnectCallback), loginSocket);
    }

    public void Connect(string host, int port, string token)
    {
        Debug.Log("Connecting to map server...");
        waitingConnectType = -1;
        waitingForInternet = false;
        statusCanvas.enabled = true;
        storedHost = host;
        storedPort = port;
        authToken = token;
        messageQueue.Clear();
        statusText.text = "Connecting...";
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            statusText.text = "Internet not reachable";
            waitingConnectType = 2;
            waitingForInternet = true;
            return;
        }
        gameSocket = new TcpClient();
        gameSocket.BeginConnect(storedHost, storedPort, new AsyncCallback(GameConnectCallback), gameSocket);
    }

    void LoginConnectCallback(IAsyncResult result)
    {
        Debug.Log("Connected!");
        int i;
        int dataLength = 0;
        loginSocket.Client.EndConnect(result);
        Debug.Log("End connect");
        socketBuffer = new byte[4096];
        if (storedKey != "" && storedSecret != "")
        {
            Debug.Log("Authenticate with key [" + storedKey + "]");
            byte[] keyData = Encoding.UTF8.GetBytes(storedKey);
            dataLength = 2 + 2 + keyData.Length;
            i = 0;
            Buffer.BlockCopy(BitConverter.GetBytes((short)1002), 0, socketBuffer, i, 2); // Auth method: key + secret
            i += 2;
            Buffer.BlockCopy(BitConverter.GetBytes((short)keyData.Length), 0, socketBuffer, i, 2); // Login length
            i += 2;
            Buffer.BlockCopy(keyData, 0, socketBuffer, i, keyData.Length); // Login data
        }
        else
        {
            if (loginField.text != "")
            {
                Debug.Log("Authenticate with login");
                byte[] loginData = Encoding.UTF8.GetBytes(loginField.text);
                byte[] passwordData = Encoding.UTF8.GetBytes(passwordField.text);
                //byte[] socketBuffer = new byte[2 + 2 + loginData.Length + 2 + passwordData.Length];
                dataLength = 2 + 2 + loginData.Length + 2 + passwordData.Length;
                i = 0;
                Buffer.BlockCopy(BitConverter.GetBytes((short)1001), 0, socketBuffer, i, 2); // Auth method: login / password
                i += 2;
                Buffer.BlockCopy(BitConverter.GetBytes((short)loginData.Length), 0, socketBuffer, i, 2); // Login length
                i += 2;
                Buffer.BlockCopy(loginData, 0, socketBuffer, i, loginData.Length); // Login data
                i += loginData.Length;
                Buffer.BlockCopy(BitConverter.GetBytes((short)passwordData.Length), 0, socketBuffer, i, 2); // Password length
                i += 2;
                Buffer.BlockCopy(passwordData, 0, socketBuffer, i, passwordData.Length); // Password data
            }
            else
            {
                Debug.Log("Open registration");
                dataLength = 2;
                i = 0;
                Buffer.BlockCopy(BitConverter.GetBytes((short)1004), 0, socketBuffer, i, 2); // Auth method: free
                i += 2;
            }
        }
        Debug.Log("Prepare to send: " + dataLength);
        Debug.Log("Sending...");
        loginSocket.Client.BeginSend(socketBuffer, 0, dataLength, SocketFlags.None, new AsyncCallback(LoginSendCallback), null);
    }

    void LoginSendCallback(IAsyncResult result)
    {
        loginSocket.Client.EndSend(result);
        loginSocket.Client.BeginReceive(socketBuffer, 2048, 2048, SocketFlags.None, new AsyncCallback(LoginReceiveCallback), socketBuffer);
    }

    void LoginReceiveCallback(IAsyncResult result)
    {
        byte[] rawData;
        loginSocket.Client.EndReceive(result);
        rawData = new byte[2048];
        Buffer.BlockCopy(socketBuffer, 2048, rawData, 0, 2048);
        messageQueue.AddLast(new ByteArrayContainer(rawData));
    }

    void GameConnectCallback(IAsyncResult result)
    {
        Debug.Log("Connected!");
        int i;
        int dataLength = 0;
        gameSocket.Client.EndConnect(result);
        Debug.Log("End connect");
        socketBuffer = new byte[4096];

        ResetReceive();

        Debug.Log("Authenticate with token");
        byte[] tokenData = Encoding.UTF8.GetBytes(authToken);
        dataLength = 2 + 2 + tokenData.Length;
        i = 0;
        Buffer.BlockCopy(BitConverter.GetBytes((short)1001), 0, socketBuffer, i, 2); // Auth method: token
        i += 2;
        Buffer.BlockCopy(BitConverter.GetBytes((short)tokenData.Length), 0, socketBuffer, i, 2); // Token length
        i += 2;
        Buffer.BlockCopy(tokenData, 0, socketBuffer, i, tokenData.Length); // Token data
        Debug.Log("Prepare to send: " + dataLength);
        gameSocket.Client.BeginSend(socketBuffer, 0, dataLength, SocketFlags.None, new AsyncCallback(GameSendCallback), null);
        Debug.Log("Sending...");
    }

    public bool IsConnected()
    {
        if(gameSocket == null)
        {
            return false;
        }
        return gameSocket.Connected;
    }

    public void SendGameMessage(byte[] buffer)
    {
        SendGameMessage(buffer, 0, buffer.Length, new AsyncCallback(GameSendCallback));
    }

    public void SendGameMessage(byte[] buffer, int offset, int length, AsyncCallback callback)
    {
        try
        {
            gameSocket.Client.BeginSend(buffer, offset, length, SocketFlags.None, callback, null);
        }
        catch(Exception ex)
        {
            Connect(storedHost, storedPort, authToken);
        }
    }

    void GameSendCallback(IAsyncResult result)
    {
        gameSocket.Client.EndSend(result);
    }

    void GameReceiveCallback(IAsyncResult result)
    {
        Debug.Log("GameReceiveCallback() game:" + (gameSocket != null) + " ; updates:" + updates);
        byte[] rawData;
        gameSocket.Client.EndReceive(result);
        rawData = new byte[2048];
        Buffer.BlockCopy(socketBuffer, 2048, rawData, 0, 2048);
        ResetReceive();
        messageQueue.AddLast(new ByteArrayContainer(rawData));
    }

    public void ResetReceive()
    {
        gameSocket.Client.BeginReceive(socketBuffer, 2048, 2048, SocketFlags.None, new AsyncCallback(GameReceiveCallback), socketBuffer);
    }

    void SignPhrase(ref byte[] data, byte[] secret)
    {
        int i;
        int j = 0;
        int c;
        for (i = 0; i < data.Length; i++)
        {
            c = data[i] + secret[j];
            if (c > 255)
            {
                c -= 255;
            }
            if (c >= 127)
            {
                c -= 127;
            }
            if (c < 0)
            {
                c += 64;
            }
            if (c < 48)
            {
                c += 48;
            }
            data[i] = (byte)c;
            j++;
            if (j > secret.Length)
            {
                j -= secret.Length;
            }
        }
    }

    // Update is called once per frame
    void Update() {
        updates++;
        if(waitingForInternet)
        {
            if(waitingConnectType == 1)
            {
                Connect();
            }
            else if(waitingConnectType == 2)
            {
                Connect(storedHost, storedPort, authToken);
            }
            return;
        }
        if (gameSocket != null)
        {
            ProcessGameMessageQueue();
        }
        else
        {
            ProcessLoginMessageQueue();
        }
    }


    public void ProcessLoginMessageQueue()
    {
        int i;
        short l;
        int dataLength;
        byte[] data;
        LinkedListNode<ByteArrayContainer> node;

        node = messageQueue.First;
        while (node != null)
        {
            data = node.Value.value;

            i = 0;
            short messageCode = BitConverter.ToInt16(data, i);
            i += 2;
            Debug.Log("Response code: " + messageCode);

            switch (messageCode)
            {
                case 900:
                    dataLength = 2;
                    i = 0;
                    Buffer.BlockCopy(BitConverter.GetBytes((short)900), 0, socketBuffer, i, 2); // Ping answer
                    i += 2;
                    loginSocket.Client.BeginSend(socketBuffer, 0, dataLength, SocketFlags.None, new AsyncCallback(LoginSendCallback), null);
                    break;
                case 1001:
                case 1002:
                case 1004:
                    l = BitConverter.ToInt16(data, i);
                    i += 2;
                    string token = Encoding.ASCII.GetString(data, i, l);
                    i += l;
                    l = BitConverter.ToInt16(data, i);
                    i += 2;
                    string host = Encoding.ASCII.GetString(data, i, l);
                    i += l;
                    int port = (int)BitConverter.ToInt16(data, i);
                    i += 2;
                    Debug.Log("Host: " + host + " ; port: " + port + " ; token: " + token);
                    if (messageCode == 1001 || messageCode == 1002)
                    {
                        l = BitConverter.ToInt16(data, i);
                        i += 2;
                        string key = Encoding.ASCII.GetString(data, i, l);
                        i += l;
                        l = BitConverter.ToInt16(data, i);
                        i += 2;
                        string secret = Encoding.ASCII.GetString(data, i, l);
                        Debug.Log("Key: " + key + " ; Secret: " + secret);
                        PlayerPrefs.SetString("CredentialsKey", key);
                        PlayerPrefs.SetString("CredentialsSecret", secret);
                    }
                    loginSocket.Close();
                    loginSocket = null;
                    Connect(host, port, token);
                    /*
                    submitButton.image.rectTransform.anchoredPosition = new Vector2(0.0f, -2000.0f);
                    loginField.image.rectTransform.anchoredPosition = new Vector2(0.0f, -2000.0f);
                    passwordField.image.rectTransform.anchoredPosition = new Vector2(0.0f, -2000.0f);
                    submitButton.enabled = false;
                    submitButton.image.enabled = false;
                    loginField.enabled = false;
                    loginField.image.enabled = false;
                    passwordField.enabled = false;
                    passwordField.image.enabled = false;
                    loginPanel.enabled = false;
                    statusText.enabled = false;
                    */
                    break;
                case 1003:
                    //readBuffer = new byte[4096];
                    //loginSocket.Client.BeginReceive(socketBuffer, 2048, 2048, SocketFlags.None, new AsyncCallback(ReceiveCallback), socketBuffer);
                    l = BitConverter.ToInt16(data, i);
                    i += 2;
                    byte[] phrase = new byte[l];
                    byte[] keyData = Encoding.UTF8.GetBytes(storedKey);
                    byte[] secretData = Encoding.UTF8.GetBytes(storedSecret);
                    Buffer.BlockCopy(data, i, phrase, 0, l);
                    Debug.Log("Phrase: " + Encoding.UTF8.GetString(phrase));
                    //byte[] writeBuffer = new byte[4 + phrase.Length];
                    SignPhrase(ref phrase, secretData);
                    dataLength = 4 + keyData.Length + 2 + phrase.Length;
                    i = 0;
                    Buffer.BlockCopy(BitConverter.GetBytes((short)1003), 0, socketBuffer, i, 2); // Auth method: pass phrase
                    i += 2;
                    Buffer.BlockCopy(BitConverter.GetBytes((short)keyData.Length), 0, socketBuffer, i, 2); // Auth method: pass phrase
                    i += 2;
                    Buffer.BlockCopy(keyData, 0, socketBuffer, i, keyData.Length);
                    i += keyData.Length;
                    Buffer.BlockCopy(BitConverter.GetBytes((short)phrase.Length), 0, socketBuffer, i, 2); // Auth method: pass phrase
                    i += 2;
                    Buffer.BlockCopy(phrase, 0, socketBuffer, i, phrase.Length);
                    loginSocket.Client.BeginSend(socketBuffer, 0, dataLength, SocketFlags.None, new AsyncCallback(LoginSendCallback), null);
                    break;
                case 1101:
                case 1102:
                case 1103:
                    Debug.LogError("Server throw error: " + messageCode);
                    statusText.text = "Error code: " + messageCode;
                    loginSocket.Close();
                    if (messageCode == 1103)
                    {
                        storedKey = "";
                        storedSecret = "";
                        PlayerPrefs.SetString("CredentialsKey", storedKey);
                        PlayerPrefs.SetString("CredentialsSecret", storedSecret);
                        Connect();
                    }
                    else
                    {
                        loginField.image.rectTransform.anchoredPosition = baseLoginFieldPosition;
                        passwordField.image.rectTransform.anchoredPosition = basePasswordFieldPosition;
                        submitButton.image.rectTransform.anchoredPosition = baseSubmitButtonPosition;
                    }
                    break;
            }

            node = node.Next;
            messageQueue.RemoveFirst();
        }

    }

    public void ProcessGameMessageQueue()
    {
        int i;
        int dataLength;
        LinkedListNode<ByteArrayContainer> byteArrayNode;
        LinkedListNode<ByteArrayContainer> byteArrayNodeNext;

        if(map == null)
        {
            return;
        }

        byteArrayNode = messageQueue.First;
        while (byteArrayNode != null)
        {
            byteArrayNodeNext = byteArrayNode.Next;
            map.AddMessage(byteArrayNode.Value);
            short messageCode = BitConverter.ToInt16(byteArrayNode.Value.value, 0);
            Debug.Log("ProcessGameMessage[" + messageCode + "]");
            switch (messageCode)
            {
                case 900:
                    dataLength = 2;
                    i = 0;
                    Buffer.BlockCopy(BitConverter.GetBytes((short)900), 0, socketBuffer, i, 2); // Ping answer
                    i += 2;
                    gameSocket.Client.BeginSend(socketBuffer, 0, dataLength, SocketFlags.None, new AsyncCallback(GameSendCallback), null);
                    break;
                case 1001:
                    statusCanvas.enabled = false;
                    break;
            }
            messageQueue.Remove(byteArrayNode);
            byteArrayNode = byteArrayNodeNext;
        }
    }

    public void TapToRegion(string regionId)
    {
        int i;
        int dataLength = 4;
        byte[] regionIdData = Encoding.UTF8.GetBytes(regionId);
        i = 0;
        Buffer.BlockCopy(BitConverter.GetBytes((short)1101), 0, socketBuffer, i, 2);
        i += 2;
        Buffer.BlockCopy(regionIdData, 0, socketBuffer, i, regionId.Length); // Region ID
        SendGameMessage(socketBuffer, 0, dataLength, new AsyncCallback(GameSendCallback));
    }

    public void ChangeCloth(int clothId)
    {
        uint value = (uint)clothId;
        int i;
        int dataLength = 6;
        i = 0;
        Buffer.BlockCopy(BitConverter.GetBytes((short)1301), 0, socketBuffer, i, 2);
        i += 2;
        Buffer.BlockCopy(BitConverter.GetBytes(value), 0, socketBuffer, i, 4);
        SendGameMessage(socketBuffer, 0, dataLength, new AsyncCallback(GameSendCallback));
    }

    public void ChangeWeapon(int weaponId)
    {
        uint value = (uint)weaponId;
        int i;
        int dataLength = 6;
        i = 0;
        Buffer.BlockCopy(BitConverter.GetBytes((short)1302), 0, socketBuffer, i, 2);
        i += 2;
        Buffer.BlockCopy(BitConverter.GetBytes(value), 0, socketBuffer, i, 4);
        SendGameMessage(socketBuffer, 0, dataLength, new AsyncCallback(GameSendCallback));
    }

}

public class ByteArrayContainer
{

    public byte[] value;

    public ByteArrayContainer(byte[] initialValue)
    {
        value = initialValue;
    }

}


