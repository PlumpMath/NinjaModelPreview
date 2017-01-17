using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;
using UnityEngine.Networking.Match;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using ExitGames;
using ExitGames.Client;
using ExitGames.Client.Photon;

public class GameMatchMaker : Photon.PunBehaviour
{

    public GameObject gameNetworkPrefab;
    public LoginController loginController;
    public RegionMoveController regionMoveController;
    public PlayerController playerController;
    [NonSerialized]
    public GameNetwork gameNetwork;
    public Camera camera;
    public Canvas canvasConnect;
    public Canvas canvasPlay;
    public Canvas canvasSettings;
    public Canvas canvasGameover;
    public Image lobbyPanel;
    public Button startLocalButton;
    public Button settingsButton;
    public InputField roomIdField;
    public Button missileSelectorPrevButton;
    public Button missileSelectorNextButton;
    public Button gameOverButton;
    public Text battleTitleLabel;
    public Text battleTimeLabel;
    public Text battleDamageLabel;
    public Text battleDPSLabel;
    public Text battleWoundLabel;
    public ArmedMissileController armedMissile;
    //public RavenController visualEffectRaven;
    public string targetRoom = "";

    public string enteringToken = "";

    public int joinAttempts = 0;

    public static bool created = false;

    private Dictionary<int, string> langNotices = new Dictionary<int, string>();
    private LinkedList<BaseObjectMessage> delayedMessages = new LinkedList<BaseObjectMessage>();
    private RoomInfo selectedRoom = null;
    private TypedLobby lobby;
    private float remoteTimestamp = 0.0f;
    private int abilityFirst = 1;
    private int abilitySecond = 2;

    public int gameMode = 1;

    public float GetRemoteTimestamp()
    {
        return remoteTimestamp;
    }

    public void AddDelayedMessage(BaseObjectMessage message)
    {
        delayedMessages.AddLast(message);
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("CONNECTED!");
        lobby = new TypedLobby();
        lobby.Type = LobbyType.Default;
        lobby.Name = "main";
        //PhotonNetwork.JoinLobby(lobby);

        if (targetRoom != "")
        {
            RoomOptions room = new RoomOptions();
            Debug.Log("JoinOrCreateRoom: " + targetRoom);
            PhotonNetwork.JoinOrCreateRoom(targetRoom, room, TypedLobby.Default);
            targetRoom = "";
        }
        else
        {
            PhotonNetwork.Disconnect();
        }

    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        int i;
        Debug.Log("OnJoinedLobby: " + PhotonNetwork.networkingPeer.lobby.Name + " [" + PhotonNetwork.networkingPeer.lobby.Type + "] (" + PhotonNetwork.networkingPeer.insideLobby + ")");

    }

    public override void OnReceivedRoomListUpdate()
    {
        base.OnReceivedRoomListUpdate();
        int i;
        int j = 0;
        RoomInfo[] rooms = PhotonNetwork.GetRoomList();
        Debug.Log("Rooms: " + rooms.Length);
        for(i = 0; i < rooms.Length; i++)
        {
            Debug.Log("Room: " + rooms[i].name);
            if(rooms[i].name == "root")
            {
                selectedRoom = rooms[i];
            }
        }
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        int i;
        if(PhotonNetwork.networkingPeer.CurrentRoom.name == "entrance")
        {
            PhotonNetwork.networkingPeer.OpCustom((byte)1, new Dictionary<byte, object> { { 245, Encoding.ASCII.GetBytes(enteringToken) } }, true);
        }
        else if (PhotonNetwork.networkingPeer.CurrentRoom.name.Length == 8 && PhotonNetwork.networkingPeer.CurrentRoom.name.Substring(0, 6) == "region")
        {
            //PhotonNetwork.networkingPeer.OpCustom((byte)1, new Dictionary<byte, object> { { 245, Encoding.ASCII.GetBytes(enteringToken) } }, true);
            PlayerPrefs.SetString("CurrentRegion", PhotonNetwork.networkingPeer.CurrentRoom.name.Substring(6, 2));
            PlayerPrefs.SetString("CurrentPoint", "S");
            SceneManager.LoadScene("region");
        }
        else if (PhotonNetwork.networkingPeer.CurrentRoom.name.Length == 68 && PhotonNetwork.networkingPeer.CurrentRoom.name.Substring(0, 4) == "duel")
        {
            SceneManager.LoadScene("battle");
            //PhotonNetwork.networkingPeer.OpCustom((byte)1, new Dictionary<byte, object> { { 245, Encoding.ASCII.GetBytes(enteringToken) } }, true);
        }
        Debug.Log("OnJoinedRoom: " + PhotonNetwork.networkingPeer.CurrentRoom.name + " (" + PhotonNetwork.networkingPeer.CurrentRoom.playerCount + ")");
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        Debug.Log("OnCreatedRoom: " + PhotonNetwork.networkingPeer.CurrentRoom.name + " (" + PhotonNetwork.networkingPeer.CurrentRoom.playerCount + ")");
    }

    public override void OnPhotonCreateRoomFailed(object[] codeAndMsg)
    {
        base.OnPhotonCreateRoomFailed(codeAndMsg);
        Debug.LogError("Create room failed");
    }

    public override void OnPhotonJoinRoomFailed(object[] codeAndMsg)
    {
        base.OnPhotonCreateRoomFailed(codeAndMsg);
        Debug.LogError("Join room failed");
    }

    public void TryConnect()
    {
        PhotonNetwork.logLevel = PhotonLogLevel.ErrorsOnly;
        //PhotonNetwork.logLevel = PhotonLogLevel.Full;
        PhotonNetwork.OnEventCall += OnEvent;
        Debug.Log("Connecting to Photon Server start #1");
        //if (PhotonNetwork.ConnectUsingSettings("1.0"))
        if (PhotonNetwork.ConnectUsingSettings("1.0"))
        {
            Debug.Log("Connecting to Photon Server process... #1");
            UpdatePreferences();
        }
        else
        {
            Debug.LogError("Connection to Photon Server failed");
        }
    }

    void Start()
    {
        int i;

        Application.runInBackground = true;

        langNotices.Clear();
        langNotices.Add(0, "");
        langNotices.Add(1, "К");
        langNotices.Add(2, "ГОЛОВА");
        langNotices.Add(3, "НОГА");
        langNotices.Add(4, "РУКА");
        langNotices.Add(5, "(ЛЕГ.)");
        langNotices.Add(6, "(СРЕД.)");
        langNotices.Add(7, "(ТЯЖ.)");
        langNotices.Add(8, "ЯД");
        langNotices.Add(9, "ЩИТ");
        langNotices.Add(10, "КРИТИЧЕСКИЙ УРОН");
        langNotices.Add(11, "УКЛОНЕНИЕ");
        langNotices.Add(12, "ОГЛУШЕН");
        langNotices.Add(13, "ОГЛУШЕНИЕ");

        SceneManager.activeSceneChanged += SceneChanged;

    }

    public void SceneChanged(Scene lastScene, Scene currentScene)
    {
        switch(currentScene.name)
        {
            case "map":
                Debug.Log("MAP LOADED!!!");
                gameMode = 1;
                loginController.statusCanvas.enabled = false;
                loginController.map = GameObject.Find("Main Camera").GetComponent<MapController>();
                break;
            case "region":
                Debug.Log("REGION LOADED!!!");
                gameMode = 2;
                loginController.statusCanvas.enabled = false;
                break;
            case "battle":
                Debug.Log("DUEL LOADED!!!");
                gameMode = 3;
                loginController.statusCanvas.enabled = false;
                break;
        }
    }

    public void ConnectWithToken(string token)
    {
        enteringToken = token;
        targetRoom = "entrance";
        TryConnect();
    }

    void Awake ()
    {
        if (!created)
        {
            DontDestroyOnLoad(this.gameObject);
            created = true;
        }

        else
        {
            //Destroy(this.gameObject);
        }
    }

    public void Disconnect()
    {
        PhotonNetwork.Disconnect();
    }

    void OnLevelWasLoaded(int level)
    {
        if (level == 0 && this.gameObject.scene.name != "DontDestroyOnLoad")
        {
            Destroy(this.gameObject);
        }
    }

    public void ClickJoinButton(int index)
    {
        int i;
        RoomInfo roomInfo;
        RoomOptions roomOptions;
        if (selectedRoom != null)
        {
            roomInfo = selectedRoom;
            if (PhotonNetwork.JoinRoom(roomInfo.name))
            {
                Debug.Log("Room joining!");
            }
            else
            {
                Debug.LogError("Can't join room");
            }
        }
        else
        {
            roomOptions = new RoomOptions();
            roomOptions.IsOpen = true;
            roomOptions.IsVisible = true;
            if (index < 3)
            {
                roomOptions.MaxPlayers = 2;
            }
            else
            {
                roomOptions.MaxPlayers = 1;
            }
            if (PhotonNetwork.CreateRoom(roomIdField.text, roomOptions, lobby))
            {
                Debug.Log("Room creating!");
            }
            else
            {
                Debug.LogError("Can't create room");
            }
        }
    }

    void OnEvent(byte eventCode, object content, int senderId)
    {
        switch(gameMode)
        {
            case 1:
                OnLoginEvent(eventCode, content, senderId);
                break;
            case 2:
                OnRegionEvent(eventCode, content, senderId);
                break;
            case 3:
                OnDuelEvent(eventCode, content, senderId);
                break;
        }
    }

    void OnLoginEvent(byte eventCode, object content, int senderId)
    {
        int i;
        BaseObjectMessage baseObjectMessage;
        PlayerObject playerObject = null;
        //PlayerController playerController = null;
        //Debug.Log("RECEIVE EVENT[" + eventCode + "] from [" + senderId + "]");
        switch (eventCode)
        {
            case 1:
                RedirectMessage redirectMessage = new RedirectMessage();
                redirectMessage.Unpack((byte[])content);
                Debug.Log("CATCH REDIRECT MESSAGE: " + redirectMessage.roomName);
                targetRoom = redirectMessage.roomName;
                PhotonNetwork.LeaveRoom();
                break;
        }
    }

    void OnRegionEvent(byte eventCode, object content, int senderId)
    {
        int i;
        BaseObjectMessage baseObjectMessage;
        PlayerObject playerObject = null;
        //PlayerController playerController = null;
        Debug.Log("RECEIVE EVENT[" + eventCode + "] from [" + senderId + "]");
        switch (eventCode)
        {
            case 1:
                RedirectMessage redirectMessage = new RedirectMessage();
                redirectMessage.Unpack((byte[])content);
                Debug.Log("CATCH REDIRECT MESSAGE: " + redirectMessage.roomName);
                targetRoom = redirectMessage.roomName;
                PhotonNetwork.LeaveRoom();
                regionMoveController.enabled = false;
                gameMode = 0;
                loginController.statusCanvas.enabled = true;
                break;
            case 2:
                RegionMoveMessage moveMessage = new RegionMoveMessage();
                moveMessage.Unpack((byte[])content);
                Debug.Log("OBJECT [" + moveMessage.userId + "] MOVE TO: " + moveMessage.destination + " IN " + moveMessage.moveTimemark + " SEC");
                regionMoveController.SetOpponentState(moveMessage.userId, moveMessage.destination, moveMessage.moveTimemark);
                break;
            case 3:
                RegionThrowMessage throwMessage = new RegionThrowMessage();
                throwMessage.Unpack((byte[])content);
                regionMoveController.ThrowOpponentHook(throwMessage.userId, throwMessage.destination, throwMessage.throwTimemark);
                break;
            case 4:
                RegionChatMessage chatMessage = new RegionChatMessage();
                chatMessage.Unpack((byte[])content);
                regionMoveController.ShowOpponentChat(chatMessage.userId, chatMessage.iconId);
                break;
        }
    }

    void OnDuelEvent(byte eventCode, object content, int senderId)
    {
        int i;
        BaseObjectMessage baseObjectMessage;
        PlayerObject playerObject = null;
        //PlayerController playerController = null;
        //Debug.Log("RECEIVE EVENT[" + eventCode + "] from [" + senderId + "]");
        switch (eventCode)
        {
            case 1:
                RedirectMessage redirectMessage = new RedirectMessage();
                redirectMessage.Unpack((byte[])content);
                Debug.Log("CATCH REDIRECT MESSAGE: " + redirectMessage.roomName);
                targetRoom = redirectMessage.roomName;
                PhotonNetwork.LeaveRoom();
                playerController.enabled = false;
                gameMode = 0;
                loginController.statusCanvas.enabled = true;
                break;
            case 91:
                baseObjectMessage = new BaseObjectMessage();
                baseObjectMessage.Unpack((byte[])content);
                remoteTimestamp = baseObjectMessage.timemark;
                gameNetwork.ClientInit();
                gameNetwork.playerId = baseObjectMessage.id;
                Debug.Log("INITIALIZE PLAYER ID: " + gameNetwork.playerId);
                /* duplicate for GameNetwork RpcSpawnObject case PLAYER */
                playerObject = (PlayerObject)gameNetwork.location.GetObject(gameNetwork.playerId);
                if (playerObject != null)
                {
                    camera.transform.position = playerObject.position * 100.0f + Vector3.up * 20.0f;
                    if (gameNetwork.playerId == 1)
                    {
                        camera.transform.eulerAngles = new Vector3(camera.transform.eulerAngles.x, 180.0f, camera.transform.eulerAngles.z);
                    }
                }
                /*
                playerObject = (PlayerObject)gameNetwork.location.GetObject(gameNetwork.playerId == 1 ? 0 : 1);
                if (playerObject != null && playerObject.visualObject == null)
                {
                    playerController = (Instantiate(gameNetwork.bodyPrefabs[0])).GetComponent<PlayerController>();
                    playerController.gameNetwork = gameNetwork;
                    playerController.obj = playerObject;
                    playerObject.visualObject = playerController;
                    playerController.transform.position = playerObject.position * 100.0f;
                    //playerController.transform.localScale *= 10.0f;
                    if(playerObject.position.z < 0.0f)
                    {
                        playerObject.visualObject.transform.Rotate(0.0f, 180.0f, 0.0f);
                    }
                }
                */
                /* */
                canvasPlay.enabled = true;
                InitializeMessage initializeMessage = new InitializeMessage();
                /*
                for (i = 1; i < AbilityButtons.Length; i++)
                {
                    if (AbilityButtons[i].image.color == Color.green)
                    {
                        if (initializeMessage.abilityFirstId <= -1)
                        {
                            initializeMessage.abilityFirstId = i;
                        }
                        else
                        {
                            initializeMessage.abilitySecondId = i;
                        }
                    }
                }
                gameNetwork.myMissileId = armedMissile.GetCurrentMissile();
                initializeMessage.missileId = gameNetwork.myMissileId;
                for (i = 1; i < VenomButtons.Length; i++)
                {
                    if (VenomButtons[i].image.color == Color.green)
                    {
                        initializeMessage.venomId = i;
                    }
                }
                */
                PhotonNetwork.networkingPeer.OpCustom((byte)1, new Dictionary<byte, object> { { 245, initializeMessage.Pack() } }, true);

                break;
            case 2:
                SpawnObjectMessage spawnObjectMessage = new SpawnObjectMessage();
                spawnObjectMessage.Unpack((byte[])content);
                //Debug.Log(Time.fixedTime + " Spawn." + spawnObjectMessage.objectType + " [" + spawnObjectMessage.id + "]");
                spawnObjectMessage.eventCode = eventCode;
                delayedMessages.AddLast(spawnObjectMessage);
                //gameNetwork.RpcSpawnObject(spawnObjectMessage.id, spawnObjectMessage.objectType, spawnObjectMessage.newPosition, spawnObjectMessage.newFloat, spawnObjectMessage.visualId);
                break;
            case 3:
                DestroyObjectMessage destroyObjectMessage = new DestroyObjectMessage();
                destroyObjectMessage.Unpack((byte[])content);
                //Debug.Log(Time.fixedTime + " Destroy [" + destroyObjectMessage.id + "]: " + destroyObjectMessage.objectId);
                destroyObjectMessage.eventCode = eventCode;
                delayedMessages.AddLast(destroyObjectMessage);
                //gameNetwork.RpcDestroyObject(destroyObjectMessage.id);
                break;
            case 4:
                MoveObjectMessage moveObjectMessage = new MoveObjectMessage();
                moveObjectMessage.Unpack((byte[])content);
                //Debug.Log(Time.fixedTime + " Move [" + moveObjectMessage.id + "]");
                moveObjectMessage.eventCode = eventCode;
                delayedMessages.AddLast(moveObjectMessage);
                //gameNetwork.RpcMoveObject(moveObjectMessage.id, moveObjectMessage.newPosition, moveObjectMessage.newFloat, moveObjectMessage.timestamp);
                break;
            case 5:
                UpdatePlayerMessage updatePlayerMessage = new UpdatePlayerMessage();
                updatePlayerMessage.Unpack((byte[])content);
                //Debug.Log("Player[" + updatePlayerMessage.id + "] health: " + updatePlayerMessage.newHealth + " ; stamina: " + updatePlayerMessage.newStamina);
                gameNetwork.RpcUpdatePlayer(updatePlayerMessage.id, updatePlayerMessage.newHealth, updatePlayerMessage.newStamina, updatePlayerMessage.newStaminaConsumption);
                break;
            case 6:
                gameNetwork.RpcRearmMissile();
                break;
            case 7:
                baseObjectMessage = new BaseObjectMessage();
                baseObjectMessage.Unpack((byte[])content);
                gameNetwork.RpcFlashPlayer(baseObjectMessage.id);
                break;
            case 8:
                GameOverMessage gameOverMessage = new GameOverMessage();
                gameOverMessage.Unpack((byte[])content);
                gameNetwork.RpcGameOver(gameOverMessage.winner, gameOverMessage.time, gameOverMessage.damage, gameOverMessage.wound);

                /*
                gameNetwork = GameObject.Instantiate(gameNetworkPrefab).GetComponent<GameNetwork>();
                gameNetwork.camera = camera;
                gameNetwork.gameMatchMaker = this;
                gameNetwork.isServer = false;
                gameNetwork.isLocal = false;
                */

                break;
            case 9:
                SetAbilityMessage setAbilityMessage = new SetAbilityMessage();
                setAbilityMessage.Unpack((byte[])content);
                gameNetwork.RpcSetAbility(setAbilityMessage.id, setAbilityMessage.value);
                break;
            case 10:
                NoticeMessage noticeMessage = new NoticeMessage();
                noticeMessage.Unpack((byte[])content);
                //Debug.Log("GET NOTICE MESSAGE. timemark: " + noticeMessage.timemark + " ; numericValue: " + noticeMessage.numericValue);
                noticeMessage.eventCode = eventCode;
                delayedMessages.AddLast(noticeMessage);
                break;
            case 11:
                baseObjectMessage = new BaseObjectMessage();
                baseObjectMessage.Unpack((byte[])content);
                Debug.Log("RECEIVE FLASH PASSIVE ABILITY. timemark: " + baseObjectMessage.timemark);
                baseObjectMessage.eventCode = eventCode;
                delayedMessages.AddLast(baseObjectMessage);
                break;
            case 12:
                baseObjectMessage = new BaseObjectMessage();
                baseObjectMessage.Unpack((byte[])content);
                //Debug.Log("FLASH OBSTRUCTION[" + baseObjectMessage.id + "]. timemark: " + baseObjectMessage.timemark);
                gameNetwork.RpcFlashObstruction(baseObjectMessage.id);
                break;
            case 13:
                VisualEffectMessage visualEffectMessage = new VisualEffectMessage();
                visualEffectMessage.Unpack((byte[])content);
                Debug.Log("VISUAL EFFECT [" + visualEffectMessage.id + "]. targetId: " + visualEffectMessage.targetId);
                visualEffectMessage.eventCode = eventCode;
                delayedMessages.AddLast(visualEffectMessage);
                break;
            case 14:
                PingMessage pingMessage = new PingMessage();
                PingMessage newPingMessage;
                pingMessage.Unpack((byte[])content);
                if(pingMessage.time == 0.0f)
                {
                    newPingMessage = new PingMessage(remoteTimestamp, pingMessage.timemark);
                    PhotonNetwork.networkingPeer.OpCustom((byte)4, new Dictionary<byte, object> { { 245, newPingMessage.Pack() } }, true);
                }
                else
                {
                    remoteTimestamp = pingMessage.timemark + pingMessage.time / 2.0f;
                }
                break;
            case 15:
                ThrowMessage throwMessage = new ThrowMessage();
                throwMessage.Unpack((byte[])content);
                if(playerController.opponent.stamina > 0.33f)
                {
                    playerController.swipe.Throw2(playerController, new Vector2(throwMessage.angleX, throwMessage.angleY), throwMessage.torsion, throwMessage.speed);
                }
                break;
        }
    }

    void CheckEvents()
    {
        LinkedListNode<BaseObjectMessage> objMessageNode;
        LinkedListNode<BaseObjectMessage> objMessageNodeNext;
        BaseObjectMessage baseObjectMessage;
        PlayerObject playerObject = null;
        //PlayerController playerController = null;
        objMessageNode = delayedMessages.First;
        while(objMessageNode != null)
        {
            objMessageNodeNext = objMessageNode.Next;
            if(objMessageNode.Value.timemark <= remoteTimestamp)
            {
                switch(objMessageNode.Value.eventCode)
                {
                    case 2:
                        SpawnObjectMessage spawnObjectMessage = (SpawnObjectMessage)objMessageNode.Value;
                        gameNetwork.RpcSpawnObject(spawnObjectMessage.objectId, spawnObjectMessage.objectType, spawnObjectMessage.newPosition, spawnObjectMessage.newVelocity, spawnObjectMessage.newAcceleration, spawnObjectMessage.newTorsion, spawnObjectMessage.newFloat, spawnObjectMessage.visualId);
                        break;
                    case 3:
                        DestroyObjectMessage destroyObjectMessage = (DestroyObjectMessage)objMessageNode.Value;
                        gameNetwork.RpcDestroyObject(destroyObjectMessage.objectId);
                        break;
                    case 4:
                        MoveObjectMessage moveObjectMessage = (MoveObjectMessage)objMessageNode.Value;
                        gameNetwork.RpcMoveObject(moveObjectMessage.objectId, moveObjectMessage.newPosition, moveObjectMessage.newVelocity, moveObjectMessage.newAcceleration, moveObjectMessage.newTorsion, moveObjectMessage.newFloat, moveObjectMessage.timestamp);
                        break;
                    case 10:
                        NoticeMessage noticeMessage = (NoticeMessage)objMessageNode.Value;
                        //Debug.Log("NOTICE MESSAGE: " + noticeMessage.numericValue + " ; " + noticeMessage.color + " ; " + noticeMessage.floating + " ; " + noticeMessage.offset);
                        string noticeText = "";
                        if (noticeMessage.color == 0)
                        {
                            noticeText += "+";
                        }
                        else
                        {
                            noticeText += "-";
                        }
                        if (noticeMessage.prefixMessage != -1)
                        {
                            noticeText += " " + langNotices[noticeMessage.prefixMessage];
                        }
                        if (noticeMessage.numericValue != 0)
                        {
                            noticeText += " " + noticeMessage.numericValue;
                        }
                        if (noticeMessage.suffixMessage != -1)
                        {
                            noticeText += " " + langNotices[noticeMessage.suffixMessage];
                        }
                        gameNetwork.RpcShowNotice(noticeMessage.id, noticeText, noticeMessage.offset, noticeMessage.color, noticeMessage.floating);
                        break;
                    case 11:
                        Debug.Log("@ INVOKE FLASH PASSIVE ABILITY[" + objMessageNode.Value.id + "]");
                        gameNetwork.RpcFlashPassiveAbility(objMessageNode.Value.id);
                        break;
                    case 13:
                        VisualEffectMessage visualEffectMessage = (VisualEffectMessage)objMessageNode.Value;
                        gameNetwork.RpcVisualEffect(visualEffectMessage.id, visualEffectMessage.invokerId, visualEffectMessage.targetId, visualEffectMessage.targetPosition, visualEffectMessage.direction, visualEffectMessage.duration);
                        break;
                }
                delayedMessages.Remove(objMessageNode);
            }
            objMessageNode = objMessageNodeNext;
        }
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    void Update()
    {
        remoteTimestamp += Time.deltaTime;
        CheckEvents();
    }

    void OnGUI()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    /*
    private void OnServerConnect(object sender, NetManager.NetworkConnectionEventArgs e)
    {
        if (e.conn.address != "localServer" && e.conn.address != "localClient")
        {
            gameNetwork = GameObject.Instantiate(gameNetworkPrefab).GetComponent<GameNetwork>();
            gameNetwork.camera = camera;
            gameNetwork.gameMatchMaker = this;
            NetworkServer.Spawn(gameNetwork.gameObject);
        }
        //Debug.Log("Client connected: " + e.conn.address);
    }
    */

    public void CreateInternetMatch(string matchName)
    {
        UpdatePreferences();
        //Debug.Log("Create internet match");
        //NetManager.singleton.matchMaker.CreateMatch(matchName, 4, true, "", "", "", 0, 0, OnInternetMatchCreate);
    }

    /*
    private void OnInternetMatchCreate(bool success, string extendedInfo, MatchInfo hostInfo)
    {
        if (hostInfo != null)
        {
            //Debug.Log("Create match succeeded");
            if(gameNetwork != null)
            {
                NetManager.singleton.StopHost();
                NetManager.singleton.StopServer();
                NetManager.singleton.StopMatchMaker();
                NetManager.singleton.StartMatchMaker();
            }
            //NetworkServer.Listen(hostInfo, NetManager.singleton.networkPort);
            //NetManager.singleton.StartHost(hostInfo);
            canvasPlay.enabled = true;
        }
        else
        {
            //Debug.LogError("Create match failed");
        }
    }
    */

    public void FindInternetMatch(string matchName)
    {
        if (gameNetwork != null)
        {
            //NetManager.singleton.StopMatchMaker();
            //NetManager.singleton.StartMatchMaker();
        }
        //storedMatchName = matchName;
        //NetManager.singleton.matchMaker.ListMatches(0, 20, matchName, true, 0, 0, OnInternetMatchList);
    }

    private void OnInternetMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matchList)
    {
        if (matchList != null)
        {
            if (matchList.Count != 0)
            {
                //NetManager.singleton.matchMaker.JoinMatch(matchList[matchList.Count - 1].networkId, "", "", "", 0, 0, OnJoinInternetMatch);
            }
            else
            {
                joinAttempts++;
                //Debug.Log("No matches in requested room! Attempt: " + joinAttempts);
                if (joinAttempts < 10)
                {
                    //FindInternetMatch(storedMatchName);
                }
                else
                {
                    joinAttempts = 0;
                    //Debug.Log("Failed 10 join attempts");
                }
            }
        }
        else
        {
            //Debug.LogError("Couldn't connect to match maker");
        }
    }

    //this method is called when your request to join a match is returned
    private void OnJoinInternetMatch(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        //NetManager.singleton.OnServerConnect()
        if (matchInfo != null)
        {
            //Debug.Log("Able to join a match");
            //NetManager.singleton.StartClient(matchInfo);
            canvasPlay.enabled = true;
        }
        else
        {
            //Debug.LogError("Join match failed");
        }
    }

    public void UpdatePreferences()
    {
        /*
        preferenceHealth = float.Parse(preferenceFieldHealth.text);
        preferenceStamina = float.Parse(preferenceFieldStamina.text);
        preferenceStaminaConsume = float.Parse(preferenceFieldStaminaConsume.text);
        preferenceStaminaRegeneration = float.Parse(preferenceFieldStaminaRegeneration.text);
        preferenceMinDamage = float.Parse(preferenceFieldMinDamage.text);
        preferenceMaxDamage = float.Parse(preferenceFieldMaxDamage.text);
        preferenceCritChance = float.Parse(preferenceFieldCritChance.text) * 0.01f;
        preferenceCritMultiplier = float.Parse(preferenceFieldCritMultiplier.text);
        preferenceInjureChance = float.Parse(preferenceFieldInjureChance.text) * 0.01f;
        preferenceAbilityEvadeChance = float.Parse(preferenceFieldAbilityEvadeChance.text) * 0.01f;
        preferenceAbilityCritChance = float.Parse(preferenceFieldAbilityCritChance.text) * 0.01f;
        preferenceAbilityStunDuration = float.Parse(preferenceFieldAbilityStunDuration.text);
        preferenceAbilityShieldDuration = float.Parse(preferenceFieldAbilityShieldDuration.text);
        preferenceAbilityShieldMultiplier = float.Parse(preferenceFieldAbilityShieldMultiplier.text) * 0.01f;
        preferenceInjureArmEffect = float.Parse(preferenceFieldInjureArmEffect.text) * 0.01f;
        preferenceInjureLegEffect = float.Parse(preferenceFieldInjureLegEffect.text) * 0.01f;
        preferenceStrafeSpeed = float.Parse(preferenceFieldStrafeSpeed.text);
        */
    }

}