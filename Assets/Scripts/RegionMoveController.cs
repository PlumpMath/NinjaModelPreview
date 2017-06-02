using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class RegionMoveController : MonoBehaviour {

    public GameMatchMaker matchMaker = null;

    public HidingPanelController topPanel;
    public HidingPanelController bottomLabel;

    public Camera camera;
    public Canvas mainCanvas;
    public RegionBodyBehavior body;
    public MeshRenderer overlayMesh;
    public Image leavingImage;
    public Image hookCooldownProgress;
    public Image discoveredFrame1;
    public Image discoveredFrame2;
    public Image discoveredIcon;

    public Sprite someFoundSprite;
    public Sprite itemFoundSprite;
    public Sprite taskCompleteSprite;
    public Sprite startBattleSprite;
    public Sprite hookBounceSprite;

    public Image joystickKey;
    public Image joystickFrame;
    public Button[] inputModeButtons = new Button[4];
    public Button hookButton;
    public Button smileyButton;
    public SmileyButton[] smileyButtons = new SmileyButton[0];
    public Button leaveScreen;
    public Text statusBar;
    public Text goldLabel;
    public MeshRenderer[] mapQuads = new MeshRenderer[3];
    public GameObject timedIconPrefab = null;

    public int gold = 0;

    public int inputMode = 3;
    public float inputCooldown = 0.0f;
    public float inputTargetingCooldown = 0.0f;
    public float applyInputCooldown = 0.0f;
    public float inputSendCooldown = 0.0f;
    public int ignoreFinger = -1;
    private float xEdge = 5.0f;
    private float yEdge = 5.0f;

    public RegionPreset region = null;

    public RegionMap map = new RegionMap();
    public RegionMapNode mapNode = null;

    public int traceType = 1;

    public Button traceTypeButton;

    public Vector3 inputDirection = Vector3.zero;
    public Vector3 lastInputDirection = Vector3.zero;
    private bool inputTouched = false;
    private float leaveCooldown = 0.0f;
    public Vector3 lastSearchingPosition = Vector3.up * 1000.0f;
    public float searchingCooldown = 0.0f;
    private float cameraShakeCooldown = 0.0f;

    private float botActionCooldown = 0.0f;

    private LinkedList<RegionBodyBehavior> bots = new LinkedList<RegionBodyBehavior>();
    
    public void SwitchInputMode(int mode)
    {
        int i;
        inputMode = mode;
        for (i = 0; i < inputModeButtons.Length; i++)
        {
            if (i == mode)
            {
                inputModeButtons[i].image.color = new Color(0.0f, 1.0f, 0.0f, 0.15f);
            }
            else
            {
                inputModeButtons[i].image.color = new Color(1.0f, 0.0f, 0.0f, 0.15f);
            }
        }
        if(inputMode == 2)
        {
            joystickFrame.enabled = true;
            joystickKey.enabled = true;
        }
        else
        {
            joystickFrame.enabled = false;
            joystickKey.enabled = false;
        }
    }

    void Start () {

        bool b;
        int i;

        string currentRegionId = PlayerPrefs.GetString("CurrentRegion", "01");
        //mapQuads[0].material = Resources.Load<Material>("Materials/RegionMap" + currentRegionId + "_1");
        //mapQuads[1].material = Resources.Load<Material>("Materials/RegionMap" + currentRegionId + "_2");
        //mapQuads[2].material = Resources.Load<Material>("Materials/RegionMap" + currentRegionId + "_3");

        /*
        inputModeButtons[0].onClick.AddListener(delegate() {
            SwitchInputMode(0);
        });
        inputModeButtons[1].onClick.AddListener(delegate () {
            SwitchInputMode(1);
        });
        inputModeButtons[2].onClick.AddListener(delegate () {
            SwitchInputMode(2);
        });
        inputModeButtons[3].onClick.AddListener(delegate () {
            SwitchInputMode(3);
        });
        */

        hookButton.onClick.AddListener(delegate() {
            ThrowHook();
        });

        leaveScreen.image.rectTransform.anchoredPosition = new Vector2(-1000.0f, 0.0f);

        //leaveScreen.onClick.AddListener(delegate() {
        //    leaveCooldown = 0.01f;
        //});

        smileyButton.onClick.AddListener(delegate () {
            b = false;
            if(!smileyButtons[0].button.enabled)
            {
                b = true;
            }
            for (i = 0; i < smileyButtons.Length; i++)
            {
                smileyButtons[i].button.enabled = b;
                smileyButtons[i].button.image.enabled = b;
                smileyButtons[i].icon.enabled = b;
            }
        });

        smileyButtons[0].button.onClick.AddListener(delegate() {
            RegionChatMessage regionChatMessage = new RegionChatMessage();
            regionChatMessage.iconId = 0;
            PhotonNetwork.networkingPeer.OpCustom((byte)4, new Dictionary<byte, object> { { 245, regionChatMessage.Pack() } }, true);
        });

        smileyButtons[1].button.onClick.AddListener(delegate () {
            RegionChatMessage regionChatMessage = new RegionChatMessage();
            regionChatMessage.iconId = 1;
            PhotonNetwork.networkingPeer.OpCustom((byte)4, new Dictionary<byte, object> { { 245, regionChatMessage.Pack() } }, true);
        });

        b = false;
        for (i = 0; i < smileyButtons.Length; i++)
        {
            smileyButtons[i].button.enabled = b;
            smileyButtons[i].button.image.enabled = b;
            smileyButtons[i].icon.enabled = b;
        }

        map.size = new Vector2(60.0f, 178.0f);
        map.Load("map_01_areas");
        mapNode = map.FindNode(transform.position.x, transform.position.z);

        if (currentRegionId == "01")
        {
            statusBar.text = "Долина змеиной головы";
        }
        if (currentRegionId == "02")
        {
            statusBar.text = "Ущелье холодных вершин";
        }
        if (currentRegionId == "03")
        {
            statusBar.text = "Пустошь дырявых штанов";
        }

        traceTypeButton.onClick.AddListener(delegate () {
            if (traceType == 1)
            {
                traceType = 2;
                traceTypeButton.image.color = new Color(1.0f, 1.0f, 0.0f, 1.0f);
            }
            else if (traceType == 2)
            {
                traceType = 3;
                traceTypeButton.image.color = new Color(0.0f, 1.0f, 0.0f, 1.0f);
            }
            else if (traceType == 3)
            {
                traceType = 1;
                traceTypeButton.image.color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
            }
        });

        region = ((GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Regions/Region" + currentRegionId))).GetComponent<RegionPreset>();
        region.transform.position = new Vector3(0.0f, -0.015f, 0.0f);

        if(currentRegionId == "01")
        {
            SetStatusText("Долина змеиной головы");
        }

        GameObject gameNetworkGO = GameObject.Find("GameNetwork");
        if (gameNetworkGO != null)
        {
            matchMaker = gameNetworkGO.GetComponent<GameMatchMaker>();
            matchMaker.regionMoveController = this;
        }

        xEdge = 5.0f * (float)Screen.width / (float)Screen.height;
        yEdge = 5.0f;

        overlayMesh.transform.localScale = new Vector3(overlayMesh.transform.localScale.y * ((float)Screen.width / (float)Screen.height), overlayMesh.transform.localScale.y, 1.0f);

    }

    /*
    #if UNITY_EDITOR

        private bool inEditorUpdated = false;

        RegionMoveController()
        {
            inEditorUpdated = false;
            EditorApplication.update += InEditorUpdate;
        }

        void InEditorUpdate()
        {
            if(!inEditorUpdated)
            {
                inEditorUpdated = true;
                RegionPreset preset = GameObject.FindObjectOfType<RegionPreset>();
                if (preset != null)
                {
                    Light light = GameObject.Find("Directional Light").GetComponent<Light>();
                    light.color = preset.ambientColor;
                    Shader.SetGlobalColor("_AmbientLight", preset.ambientColor);
                }
            }
        }

    #endif
    */

    void OnGUI() {

        int i, j;
#if !UNITY_EDITOR
        if(Input.touchCount > 0)
        {
            for(i = 0; i < Input.touchCount; i++)
            {
                if(Input.touches[i].phase == TouchPhase.Began && !EventSystem.current.IsPointerOverGameObject(Input.touches[i].fingerId) && smileyButtons[0].button.enabled)
                {
                    /*
                    for(j = 0; j < smileyButtons.Length; j++)
                    {
                        smileyButtons[j].button.enabled = false;
                        smileyButtons[j].button.image.enabled = false;
                        smileyButtons[j].icon.enabled = false;
                    }
                    */
                }
            }
        }
#endif

    }

    void Update () {

        int i;
        bool stop = true;
        bool touched = false;
        float f;
        float sign;
        float posX = 0.0f;
        float posY = 0.0f;
        Quaternion q;
        RoutePoint routePoint;
        RegionBodyBehavior bot;
        LinkedListNode<RegionBodyBehavior> botNode;
        Vector3 v3;

        if(inputSendCooldown > 0.0f)
        {
            inputSendCooldown -= Time.deltaTime;
            if(inputSendCooldown < 0.0f)
            {
                inputSendCooldown = 0.0f;
            }
        }

        if(cameraShakeCooldown > 0.0f)
        {
            cameraShakeCooldown -= Time.deltaTime;
            if (cameraShakeCooldown < 2.5f)
            {
                if (cameraShakeCooldown + Time.deltaTime >= 2.5f)
                {
                    Handheld.Vibrate();
                }
                f = Mathf.Max(0.0f, cameraShakeCooldown - 2.0f) * 2.0f;
                camera.transform.position += Vector3.forward * Mathf.Sin(f * 20.0f) * f * 7.0f * Time.deltaTime;
            }
        }

        if(body.hook.cooldown > 0.0f)
        {
            body.hook.cooldown -= Time.deltaTime;
            hookCooldownProgress.fillAmount = Mathf.Min(1.0f, Mathf.Max(0.0f, 1.0f - body.hook.cooldown / 5.0f));
        }
        else
        {
            if(hookCooldownProgress.fillAmount > 0.0f)
            {
                hookCooldownProgress.fillAmount = 0.0f;
            }
        }

        botActionCooldown -= Time.deltaTime;
        if (botActionCooldown < 0.0f)
        {
            botActionCooldown = 0.1f;
            botNode = bots.First;
            while (botNode != null)
            {
                bot = botNode.Value;

                v3 = bot.transform.position - transform.position;
                if(v3.magnitude < bot.visibleDistance)
                {
                    if (v3.magnitude < bot.visibleDistance * 0.75f)
                    {
                        if (!bot.isGoodVisible)
                        {
                            bot.isGoodVisible = true;
                            bot.isVisible = true;
                        }
                    }
                    else
                    {
                        if(!bot.isVisible || bot.isGoodVisible)
                        {
                            bot.isVisible = true;
                            bot.isGoodVisible = false;
                        }
                    }
                }
                else
                {
                    if (bot.isVisible)
                    {
                        bot.isVisible = false;
                        bot.isGoodVisible = false;
                    }
                }

                if ((v3.x > -xEdge || v3.x < xEdge || v3.z > -yEdge || v3.z < yEdge) && !bot.offscreenPointer.enabled && bot.isVisible)
                {
                    bot.offscreenPointer.enabled = true;
                }
                if(bot.offscreenPointer.enabled)
                {
                    bot.offscreenPointer.color = new Color(1.0f, 0.5f, 0.5f, Mathf.Max(0.0f, Mathf.Min(1.0f, 1.0f - (v3.magnitude - xEdge) / (bot.visibleDistance - xEdge))));
                }
                if (((v3.x <= xEdge && v3.x >= -xEdge && v3.z <= yEdge && v3.z >= -yEdge) && bot.offscreenPointer.enabled) /*|| !bot.isVisible*/)
                {
                    bot.offscreenPointer.enabled = false;
                }
                botNode = botNode.Next;
            }
        }

        botNode = bots.First;
        while (botNode != null)
        {
            bot = botNode.Value;
            if (bot.offscreenPointer.enabled)
            {
                v3 = bot.transform.position - transform.position;
                float halfWidth = (((float)Screen.width) / mainCanvas.scaleFactor - 24.0f) / 2.0f;
                float halfHeight = (((float)Screen.height) / mainCanvas.scaleFactor - 24.0f) / 2.0f;
                v3 = v3.normalized * halfHeight;
                if (Mathf.Abs(v3.x) / Mathf.Abs(v3.z) > halfWidth / halfHeight)
                {
                    v3.z = v3.z * (halfWidth * v3.x / Mathf.Abs(v3.x)) / v3.x;
                    v3.x = halfWidth * v3.x / Mathf.Abs(v3.x);
                }
                else
                {
                    v3.x = v3.x * (halfHeight * v3.z / Mathf.Abs(v3.z)) / v3.z;
                    v3.z = halfHeight * v3.z / Mathf.Abs(v3.z);
                }
                bot.offscreenPointer.rectTransform.anchoredPosition = new Vector2(v3.x, v3.z);
            }
            botNode = botNode.Next;
        }

#if UNITY_EDITOR
        if (Input.GetMouseButton(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                posX = Input.mousePosition.x;
                posY = Input.mousePosition.y;
                touched = true;
            }
        }
#else
        for (i = 0; i < Input.touchCount; i++)
        {
            if (EventSystem.current.IsPointerOverGameObject(Input.touches[i].fingerId) && Input.touches[i].phase == TouchPhase.Began)
            {
                ignoreFinger = Input.touches[i].fingerId;
            }
            if (ignoreFinger != Input.touches[i].fingerId)
            {
                posX = Input.touches[i].position.x;
                //posY = (float)Screen.height - Input.touches[0].position.y;
                posY = Input.touches[i].position.y;
                touched = true;
            }
            if (ignoreFinger == Input.touches[i].fingerId && (Input.touches[i].phase == TouchPhase.Ended || Input.touches[i].phase == TouchPhase.Canceled))
            {
                ignoreFinger = -1;
            }
        }
#endif

        if (touched && body.blockInput <= 0.0f)
        {
            switch (inputMode)
            {
                /*
                case 0:
                    direction.x = posX / (float)Screen.width - 0.5f;
                    direction.z = posY / (float)Screen.height - 0.5f;
                    direction.Normalize();
                    break;
                case 1:
                    direction.x = -posX / (float)Screen.width + 0.5f;
                    direction.z = -posY / (float)Screen.height + 0.5f;
                    direction.Normalize();
                    break;
                case 2:
                    if(posX > (float)Screen.width / 2.0f - 60.0f && posX < (float)Screen.width / 2.0f + 60.0f && posY < 120.0f)
                    {
                        posX = (posX - (float)Screen.width / 2.0f) / 60.0f;
                        posY = (posY - 60.0f) / 60.0f;
                        direction.x = posX;
                        direction.z = posY;
                        direction.Normalize();
                        joystickKey.rectTransform.anchoredPosition = (new Vector2(posX, posY)) * 60.0f;
                    }
                    break;
                */
                case 3:
                    inputDirection.x = posX / (float)Screen.width - 0.5f;
                    inputDirection.z = Mathf.Max(-0.5f, Mathf.Min(0.5f, (posY - (float)Screen.height * 0.5f) / (float)Screen.width));
                    inputDirection.z *= 1.6f;
                    Vector3 cameraShift = camera.transform.position - transform.position + Vector3.forward * 5.0f;
                    cameraShift.y = 0.0f;
                    cameraShift.x /= 2.7f * 2.0f;
                    cameraShift.z /= 2.7f * 2.0f;
                    inputDirection += cameraShift;
                    inputDirection *= 5.3f;
                    inputTouched = true;
                    break;
            }
        }

        if (body.blockInput > 0.0f)
        {
            if(touched)
            {
                Vector3 previousInputDirection = new Vector3(inputDirection.x, inputDirection.y, inputDirection.z);
                inputDirection.x = posX / (float)Screen.width - 0.5f;
                inputDirection.z = Mathf.Max(-0.5f, Mathf.Min(0.5f, (posY - (float)Screen.height * 0.5f) / (float)Screen.width));
                inputDirection.z *= 1.6f;
                Vector3 cameraShift = camera.transform.position - transform.position + Vector3.forward * 5.0f;
                cameraShift.y = 0.0f;
                cameraShift.x /= 2.7f * 2.0f;
                cameraShift.z /= 2.7f * 2.0f;
                inputDirection += cameraShift;
                inputDirection *= 0.001f;
                f = Vector3.Angle(previousInputDirection.normalized, inputDirection.normalized);
                if(Vector3.Angle((previousInputDirection.normalized + (previousInputDirection.normalized + lastInputDirection.normalized) * 0.1f).normalized, inputDirection.normalized) < Vector3.Angle(previousInputDirection.normalized, inputDirection.normalized))
                {
                    f *= -1.0f;
                }
                lastInputDirection.x = previousInputDirection.x;
                lastInputDirection.y = previousInputDirection.y;
                lastInputDirection.z = previousInputDirection.z;
            }
        }


        if (stop)
        {
            if (inputCooldown > 0.0f && inputMode == 3)
            {
                inputCooldown -= Time.deltaTime;
            }
            else
            {
                if (body.route.Count > 0)
                {
                    body.route.RemoveFirst();
                    if (body.route.Count > 0)
                    {
                        routePoint = body.route.First.Value;
                        body.direction = (routePoint.destination - transform.position).normalized;
                        inputCooldown = (routePoint.destination - transform.position).magnitude / body.speed; // routePoint.timestamp - Time.time;
                        body.moveTimeout = inputCooldown;
                    }
                }
            }
        }

        applyInputCooldown -= Time.deltaTime;
        if(applyInputCooldown <= 0.0f)
        {
            applyInputCooldown += 0.1f;
            if (inputTouched)
            {
                Debug.Log("SET DIRECTION");
                inputTouched = false;
                inputTargetingCooldown += 0.1f;
                body.direction.x = inputDirection.x;
                body.direction.z = inputDirection.z;
                body.speed = 2.1f; //5.33f; // 1.6f;
                body.route = region.GetRoute(transform.position, transform.position + body.direction, body.speed, 0.0f);
                inputCooldown = 0.0f;
                body.direction *= 0.0f;
                if (body.route.Count > 0)
                {
                    routePoint = body.route.First.Value;
                    body.direction = (routePoint.destination - transform.position).normalized;
                    inputCooldown = (routePoint.destination - transform.position).magnitude / body.speed; // routePoint.timestamp - Time.time;
                    body.moveTimeout = inputCooldown;
                }
            }
            else
            {
                if (inputTargetingCooldown >= 0.5f)
                {
                    inputTargetingCooldown = 0.0f;
                }
                else if(inputTargetingCooldown > 0.0f)
                {
                    inputTargetingCooldown = 0.0f;
                    body.route.Clear();
                    routePoint = null;
                    inputCooldown = 0.0f;
                    body.moveTimeout = inputCooldown;
                }
            }
        }

        Vector2 position2D = new Vector2(transform.position.x, transform.position.z);
        Vector2 direction2D = new Vector2(body.body.desiredDirection.x, body.body.desiredDirection.z) * body.direction.magnitude;
        Vector2 newPosition2D = position2D + direction2D * body.speed * Time.deltaTime;

        if (inputSendCooldown <= 0.0f)
        {
            inputSendCooldown = 0.1f;
            RegionMoveMessage regionMoveMessage = new RegionMoveMessage();
            regionMoveMessage.destination = position2D + direction2D * body.speed * (inputCooldown + Time.deltaTime);
            regionMoveMessage.moveTimemark = inputCooldown;
            if (PhotonNetwork.networkingPeer.PeerState == ExitGames.Client.Photon.PeerStateValue.Connected)
            {
                PhotonNetwork.networkingPeer.OpCustom((byte)2, new Dictionary<byte, object> { { 245, regionMoveMessage.Pack() } }, true);
            }
        }

        RegionBarrier barrier;
        LinkedListNode<RegionBarrier> barrierNode = map.barriers.First;
        while(barrierNode != null)
        {
            barrier = barrierNode.Value;
            if ((barrier.start.x <= newPosition2D.x && barrier.end.x >= newPosition2D.x) || (barrier.start.x >= newPosition2D.x && barrier.end.x <= newPosition2D.x))
            {
                if ((barrier.start.y <= newPosition2D.y && barrier.end.y >= newPosition2D.y) || (barrier.start.y >= newPosition2D.y && barrier.end.y <= newPosition2D.y))
                {
                    //if (Vector2.Angle(barrier.end - barrier.start + newPosition2D - position2D, newPosition2D - barrier.start) >= Vector2.Angle(barrier.end - barrier.start, newPosition2D - barrier.start))
                    //{
                        if (Vector2.Angle(barrier.end - barrier.start, newPosition2D - barrier.start) <= Vector2.Angle(position2D - barrier.start, newPosition2D - barrier.start))
                        {
                            newPosition2D = position2D;
                            break;
                        }
                    //}
                }
            }
            barrierNode = barrierNode.Next;
        }
        

        if (searchingCooldown > 0.0f)
        {
            searchingCooldown -= Time.deltaTime;
            if (searchingCooldown <= 0.0f)
            {
                lastSearchingPosition = Vector3.up * 1000.0f;
            }
        }
        

        v3 = transform.position + body.smoothDirection * body.direction.magnitude;

        camera.transform.position += new Vector3(v3.x - camera.transform.position.x, 0.0f, v3.z - camera.transform.position.z - 5.0f) * Time.deltaTime * 3.0f;
        if (camera.transform.position.x < -27.0f)
        {
            camera.transform.position += Vector3.right * (-27.0f - camera.transform.position.x);
        }
        if (camera.transform.position.x > 27.0f)
        {
            camera.transform.position += Vector3.right * (27.0f - camera.transform.position.x);
        }
        if (camera.transform.position.z < -20.0f - 10.0f)
        {
            camera.transform.position += Vector3.forward * (-20.0f - 10.0f - camera.transform.position.z);
        }
        if (camera.transform.position.z > 20.0f - 10.0f)
        {
            camera.transform.position += Vector3.forward * (20.0f - 10.0f - camera.transform.position.z);
        }

        if (leaveCooldown > 0.0f)
        {
            leaveCooldown -= Time.deltaTime;
            if (leaveCooldown + Time.deltaTime > Mathf.Ceil(leaveCooldown) && Mathf.Ceil(leaveCooldown) >= 0.0f)
            {
                statusBar.text = "Переход через " + Mathf.Ceil(leaveCooldown) + " секунд";
                bottomLabel.Show();
            }
        }

    }

    public void SetState(string id, Vector2 destination, float moveTime)
    {
        RegionBodyBehavior bot = FindBodyById(id);
        if (bot == null)
        {
            bot = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Bot")).GetComponent<RegionBodyBehavior>();
            bot.playerId = id;
            bot.map = map;
            bot.mapNode = map.FindNode(bot.transform.position.x, bot.transform.position.z);
            bot.offscreenPointer = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/BotOffscreenPointer")).GetComponent<Image>();
            bot.offscreenPointer.rectTransform.parent = mainCanvas.transform;
            bot.offscreenPointer.rectTransform.anchoredPosition = new Vector2(-1000.0f, 0.0f);
            bots.AddLast(bot);
        }
        bot.SetState(destination, moveTime);
        if (id == "")
        {
            if (moveTime != 0.0f)
            {
                inputCooldown = moveTime;
                applyInputCooldown = inputCooldown;
            }
            inputTargetingCooldown = 0.0f;
        }
    }

    public void SetProgress(string id, float progress)
    {
        RegionBodyBehavior bot = FindBodyById(id);
        if (bot != null)
        {
            bot.SetProgress(progress);
        }
    }

    public void RemoveOpponent(string id)
    {
        RegionBodyBehavior bot = FindBodyById(id);
        if (bot != null)
        {
            bots.Remove(bot);
            GameObject.Destroy(bot.gameObject);
        }
    }

    public void ThrowHook(string id, Vector2 destination, float moveTime)
    {
        RegionBodyBehavior bot = FindBodyById(id);
        if (bot != null)
        {
            bot.ThrowHook(destination, moveTime);
        }
        if(id != "")
        {
            bot = bot.hook.GetDelayedClingTarget();
            if(bot != null && bot.tag == "Player")
            {
                AnimateCameraShake();
            }
        }
    }

    public void ShowChat(string id, int iconId)
    {
        RegionBodyBehavior bot = FindBodyById(id);
        if (bot != null)
        {
            bot.ShowChat(iconId);
        }
    }

    public void ThrowHook()
    {
        if (body.blockInput > 0.0f)
        {
            return;
        }
        Vector3 v = body.smoothDirection.normalized * 6.0f; // playerIcon.transform.forward * 6.0f;
        RegionThrowMessage regionThrowMessage = new RegionThrowMessage();
        Vector2 position2D = new Vector2(transform.position.x, transform.position.z);
        Vector2 direction2D = new Vector2(v.x, v.z);
        regionThrowMessage.throwTimemark = 1.5f;
        regionThrowMessage.destination = position2D + direction2D * regionThrowMessage.throwTimemark;
        Debug.Log("HOOK DIRECTION: " + v + " ; " + position2D + " ; " + direction2D + " ; " + regionThrowMessage.destination + " ; " + regionThrowMessage.throwTimemark);
        PhotonNetwork.networkingPeer.OpCustom((byte)3, new Dictionary<byte, object> { { 245, regionThrowMessage.Pack() } }, true);
    }

    public void ShowDiscovered(string id, int iconId)
    {
        RegionBodyBehavior bot = FindBodyById(id);
        if (bot != null)
        {
            bot.ShowDiscovered(iconId);
        }
        if(id == "")
        {
            statusBar.text = "Найден предмет";
            bottomLabel.Show();
        }
    }

    public void ShowTimedIcon(Vector2 position, int iconId)
    {
        RegionTimedIcon icon = (RegionTimedIcon)((GameObject)GameObject.Instantiate(timedIconPrefab)).GetComponent<RegionTimedIcon>();
        /*
        switch (iconId)
        {
            case 0:
                icon.sprite.sprite = someFoundSprite;
                break;
            case 1:
                icon.sprite.sprite = taskCompleteSprite;
                break;
            case 2:
                icon.sprite.sprite = itemFoundSprite;
                break;
        }
        */
        icon.transform.position = new Vector3(position.x, 0.0f, position.y);
    }

    public void ShowEffect(Vector2 position, int iconId)
    {
        int i;
        RegionTimedIcon icon;
        GameObject obj;
        ParticleSystem ps;
        switch (iconId)
        {
            case 1:
                /*
                icon = (RegionTimedIcon)((GameObject)GameObject.Instantiate(timedIconPrefab)).GetComponent<RegionTimedIcon>();
                icon.transform.position = new Vector3(position.x, 0.8f, position.y - 0.8f);
                icon.sprite.transform.localScale = Vector3.one * 0.3f;
                icon.sprite.sprite = startBattleSprite;
                icon.cooldown = 4.0f;
                */
                SetStatusText("Поединок начался!");
                break;
            case 2:
                obj = (GameObject)GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Effects/SparksEffect"));
                obj.transform.position = new Vector3(position.x, 0.3f, position.y);
                GameObject.Destroy(obj, 2.0f);
                ps = obj.GetComponentInChildren<ParticleSystem>();
                ps.Emit(30);


                RaycastHit[] hits = Physics.SphereCastAll(new Vector3(position.x, 0.0f, position.y), 0.5f, Vector3.up);
                for (i = 0; i < hits.Length; i++)
                {
                    RaycastHit hit = hits[i];
                    if (hit.collider.tag == "Enemy" || hit.collider.tag == "Player")
                    {
                        RegionBodyBehavior target = hit.collider.gameObject.GetComponent<RegionBodyBehavior>();
                        if (target != null)
                        {
                            target.AnimateBlock();
                            if (target.tag == "Player")
                            {
                                AnimateCameraShake();
                            }
                        }
                    }
                }

                break;
            case 3:
                obj = (GameObject)GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Effects/BlinkEffect"));
                obj.transform.position = new Vector3(position.x, 0.0f, position.y);
                GameObject.Destroy(obj, 3.5f);
                ps = obj.GetComponentInChildren<ParticleSystem>();
                lastSearchingPosition = obj.transform.position;
                if ((lastSearchingPosition - transform.position).magnitude <= 1.5f)
                {
                    body.AnimateSearching();
                }
                //ps.Emit(30);
                break;
        }
    }

    public void StartLeaving(float time)
    {
        leaveCooldown = time;
        statusBar.text = "Переход через 5 секунд";
        bottomLabel.Show();
        leavingImage.enabled = true;
    }

    public void StopLeaving()
    {
        leaveCooldown = 0.0f;
        statusBar.text = "";
        bottomLabel.Hide();
        leavingImage.enabled = false;
    }

    public void SetGold(int newGold)
    {
        goldLabel.text = "+ " + newGold;
        gold = newGold;
        topPanel.Show();
    }

    public void SetStatusText(string value)
    {
        statusBar.text = value;
        bottomLabel.Show();
    }

    public void SetCloth(string id, string value)
    {
        RegionBodyBehavior bot = FindBodyById(id);
        if (bot != null)
        {
            bot.SetCloth(value);
        }
    }

    public void AnimateCameraShake()
    {
        if(cameraShakeCooldown > 0.0f)
        {
            return;
        }
        Debug.Log("CAMERA SHAKE");
        cameraShakeCooldown = 3.0f;
    }

    public RegionBodyBehavior FindBodyById(string id)
    {
        if (id == "")
        {
            return body;
        }
        int i;
        RegionBodyBehavior bot;
        LinkedListNode<RegionBodyBehavior> botNode = bots.First;
        while (botNode != null)
        {
            bot = botNode.Value;
            if (bot.playerId == id)
            {
                return bot;
            }
            botNode = botNode.Next;
        }
        return null;
    }

}
