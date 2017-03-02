using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class RegionMoveController : MonoBehaviour {

    public GameMatchMaker matchMaker = null;

    public float speed = 1.0f;
    public Camera camera;
    public Canvas mainCanvas;
    public GameObject playerIcon;
    public SpriteRenderer playerIconRenderer;
    public SpriteRenderer playerFaceRenderer;
    public SpriteRenderer battleIcon;
    public SpriteRenderer smileyBackground;
    public SpriteRenderer smileyIcon;
    public Image discoveredFrame1;
    public Image discoveredFrame2;
    public Image discoveredIcon;
    public Image taskPointer;
    public TaskTarget taskTarget;
    public TaskTarget[] taskTargets = new TaskTarget[0];
    public TaskTarget[] enteringPoints = new TaskTarget[0];
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
    public Button exitButton;
    public Button leaveScreen;
    public Text statusBar;
    public Text goldLabel;
    public MeshRenderer[] mapQuads = new MeshRenderer[3];
    public GameObject timedIconPrefab = null;

    public RegionMap map = new RegionMap();
    public RegionMapNode mapNode = null;

    public int coverageType = 0;
    public int lastCoverageType = 0;
    public bool hidden = false;
    public int gold = 0;

    public int inputMode = 3;
    public float inputCooldown = 0.0f;
    public float inputTargetingCooldown = 0.0f;
    public float applyInputCooldown = 0.0f;
    public float inputSendCooldown = 0.0f;
    public float blockInput = 0.0f;
    public int ignoreFinger = -1;

    public RegionHook hook = null;

    public RegionPreset region = null;

    public int traceType = 1;

    public ParticleSystem stepsPS1;
    public ParticleSystem stepsPS2;
    public ParticleSystem stepsPS3;
    public ParticleSystem stepsPS4;

    public Button traceTypeButton;

    public Vector3 direction = Vector3.zero;
    public Vector3 normalDirection = Vector3.forward;
    private Vector3 smoothDirection = Vector3.zero;
    public Vector3 inputDirection = Vector3.zero;
    public Vector3 lastInputDirection = Vector3.zero;
    private bool inputTouched = false;
    public float battleCooldown = 0.0f;
    private float botActionCooldown = 0.0f;
    private float bushDistanceTraveled = 0.0f;
    private float discoveredTimer = 0.0f;
    private float smileyCooldown = 0.0f;
    private float leaveCooldown = 0.0f;
    private float releaseProgress = 0.0f;

    private LinkedList<RegionBotBehavior> bots = new LinkedList<RegionBotBehavior>();
    private LinkedList<RoutePoint> route = new LinkedList<RoutePoint>();

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

    // Use this for initialization
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
        hook.Hide();

        exitButton.onClick.AddListener(delegate() {
            //leaveCooldown = 5.0f;
            //leaveScreen.image.rectTransform.anchoredPosition = new Vector2(0.0f, 0.0f);
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
            smileyCooldown = 2.0f;
            smileyIcon.sprite = smileyButtons[0].icon.sprite;
            smileyIcon.transform.localScale = Vector3.one * 1.5f;
            smileyBackground.enabled = true;
            smileyIcon.enabled = true;
            smileyButton.OnPointerClick(new PointerEventData(EventSystem.current));

            RegionChatMessage regionChatMessage = new RegionChatMessage();
            regionChatMessage.iconId = 0;
            PhotonNetwork.networkingPeer.OpCustom((byte)4, new Dictionary<byte, object> { { 245, regionChatMessage.Pack() } }, true);
        });

        smileyButtons[1].button.onClick.AddListener(delegate () {
            smileyCooldown = 2.0f;
            smileyIcon.sprite = smileyButtons[1].icon.sprite;
            smileyIcon.transform.localScale = Vector3.one * 1.5f;
            smileyBackground.enabled = true;
            smileyIcon.enabled = true;
            smileyButton.OnPointerClick(new PointerEventData(EventSystem.current));

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

        if (PlayerPrefs.GetInt("MapObjectState_" + currentRegionId + "_2", 0) == 1)
        {
            taskTargets[0].active = false;
        }
        if (PlayerPrefs.GetInt("MapObjectState_" + currentRegionId + "_3", 0) == 1)
        {
            taskTargets[2].active = false;
        }
        if (PlayerPrefs.GetInt("MapObjectState_" + currentRegionId + "_4", 0) == 1)
        {
            taskTargets[4].active = false;
        }

        string currentPointId = PlayerPrefs.GetString("CurrentPoint");
        for(i = 0; i < enteringPoints.Length; i++)
        {
            if(enteringPoints[i].unlockPoint == currentPointId)
            {
                transform.position = enteringPoints[i].transform.position;
            }
        }
        /*
        switch(currentPointId)
        {
            case "1":
                transform.position = new Vector3(1.33f, 0.0f, -16.0f);
                break;
            case "2":
                transform.position = taskTargets[0].transform.position;
                break;
            case "3":
                transform.position = taskTargets[2].transform.position;
                break;
            case "4":
                transform.position = taskTargets[4].transform.position;
                break;
            default:
                transform.position = new Vector3(1.33f, 0.0f, -16.0f);
                break;
        }
        */

        if(PlayerPrefs.GetInt("WinBattle", 0) == 1)
        {
            PlayerPrefs.SetInt("WinBattle", 0);
            transform.position = new Vector3(PlayerPrefs.GetFloat("RegionLastX", 0.0f), 0.0f, PlayerPrefs.GetFloat("RegionLastY", 0.0f));
        }

        /*
        GameObject bot;
        bots = new RegionBotBehavior[1];
        for (i = 0; i < bots.Length; i++)
        {
            bot = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Bot"));
            bots[i] = bot.GetComponent<RegionBotBehavior>();
            bots[i].player = this.gameObject;
            bots[i].map = map;
            bots[i].mapNode = map.FindNode(bots[i].transform.position.x, bots[i].transform.position.z);
            bots[i].offscreenPointer = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/BotOffscreenPointer")).GetComponent<Image>();
            bots[i].offscreenPointer.rectTransform.parent = mainCanvas.transform;
            bots[i].offscreenPointer.rectTransform.anchoredPosition = new Vector2(-1000.0f, 0.0f);
        }
        */

        if(currentRegionId == "01")
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

        ParticleSystem.EmissionModule emission1 = stepsPS1.emission;
        ParticleSystem.EmissionModule emission2 = stepsPS2.emission;
        ParticleSystem.EmissionModule emission3 = stepsPS3.emission;
        ParticleSystem.EmissionModule emission4 = stepsPS4.emission;

        emission1.enabled = false;
        emission2.enabled = false;
        emission3.enabled = false;
        emission4.enabled = false;

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

        GameObject gameNetworkGO = GameObject.Find("GameNetwork");
        if (gameNetworkGO != null)
        {
            matchMaker = gameNetworkGO.GetComponent<GameMatchMaker>();
            matchMaker.regionMoveController = this;
        }

        /*
        RegionPreset preset = GameObject.FindObjectOfType<RegionPreset>();
        if(preset != null)
        {
            Light light = GameObject.Find("Directional Light").GetComponent<Light>();
            light.color = preset.ambientColor;
            Shader.SetGlobalColor("_AmbientLight", preset.ambientColor);
        }
        */

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

    // Update is called once per frame
    void Update () {

        int i;
        bool stop = true;
        bool touched = false;
        float f;
        float posX = 0.0f;
        float posY = 0.0f;
        RoutePoint routePoint;
        RegionBotBehavior bot;
        LinkedListNode<RegionBotBehavior> botNode;
        Vector3 v3;

        if (battleCooldown > 0.0f)
        {
            battleCooldown -= Time.deltaTime;
            if (battleCooldown <= 0.0f)
            {
                PlayerPrefs.SetFloat("RegionLastX", transform.position.x);
                PlayerPrefs.SetFloat("RegionLastY", transform.position.z);
                PlayerPrefs.SetFloat("EnemyAdvantage", hook.targetRank * 0.5f + 0.5f);
                SceneManager.LoadScene("battle");
            }
            return;
        }

        /*
        if (leaveCooldown > 0.0f)
        {
            leaveCooldown -= Time.deltaTime;
            if(leaveCooldown <= 0.0f)
            {
                if(transform.position.x < -8.0f)
                {
                    PlayerPrefs.SetString("CurrentPoint", "E");
                }
                else if(transform.position.x > 8.0f)
                {
                    PlayerPrefs.SetString("CurrentPoint", "W");
                }
                else if (transform.position.y < -26.0f)
                {
                    PlayerPrefs.SetString("CurrentPoint", "S");
                }
                else if (transform.position.y > 27.0f)
                {
                    PlayerPrefs.SetString("CurrentPoint", "N");
                }
                matchMaker.targetRoom = "";
                matchMaker.LeaveRoom();
                //matchMaker.Disconnect();
                SceneManager.LoadScene("map");
            }
        }
        */

        if(inputSendCooldown > 0.0f)
        {
            inputSendCooldown -= Time.deltaTime;
            if(inputSendCooldown < 0.0f)
            {
                inputSendCooldown = 0.0f;
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
                /*
                if ((bot.transform.position - transform.position).magnitude > 9.0f)
                {
                    bot.transform.position = transform.position + (new Vector3(Random.Range(-1.0f, 1.0f), 0.0f, Random.Range(-1.0f, 1.0f))).normalized * 5.0f;
                    bot.direction = transform.position - bot.transform.position;
                    bot.direction.y = 0.0f;
                    bot.direction.Normalize();
                }
                */
                v3 = bot.transform.position - transform.position;
                if(v3.magnitude < bot.visibleDistance)
                {
                    if (v3.magnitude < bot.visibleDistance * 0.75f)
                    {
                        if (!bot.isGoodVisible)
                        {
                            bot.isGoodVisible = true;
                            bot.isVisible = true;
                            bot.playerIconRenderer.enabled = true;
                            bot.playerFaceRenderer.enabled = true;
                            bot.playerIconRenderer.color = new Color(1.0f, 0.5f, 0.5f, 1.0f);
                            bot.playerFaceRenderer.color = new Color(1.0f, 0.5f, 0.5f, 1.0f);
                        }
                    }
                    else
                    {
                        if(!bot.isVisible || bot.isGoodVisible)
                        {
                            bot.isVisible = true;
                            bot.isGoodVisible = false;
                            bot.playerIconRenderer.enabled = true;
                            bot.playerFaceRenderer.enabled = true;
                            bot.playerIconRenderer.color = new Color(1.0f, 0.5f, 0.5f, 0.25f);
                            bot.playerFaceRenderer.color = new Color(1.0f, 0.5f, 0.5f, 0.25f);
                        }
                    }
                }
                else
                {
                    if (bot.isVisible)
                    {
                        bot.isVisible = false;
                        bot.isGoodVisible = false;
                        bot.playerIconRenderer.enabled = false;
                        bot.playerFaceRenderer.enabled = false;
                    }
                }
                if((v3.x > 2.7f || v3.x < -2.7f || v3.y > 5.0f || v3.y < -5.0f) && !bot.offscreenPointer.enabled && bot.isVisible)
                {
                    bot.offscreenPointer.enabled = true;
                }
                if(bot.offscreenPointer.enabled)
                {
                    bot.offscreenPointer.color = new Color(1.0f, 0.5f, 0.5f, Mathf.Max(0.0f, Mathf.Min(1.0f, 1.0f - (v3.magnitude - 2.7f) / (bot.visibleDistance - 2.7f))));
                }
                if (((v3.x <= 2.7f && v3.x >= -2.7f && v3.y <= 5.0f && v3.y >= -5.0f) && bot.offscreenPointer.enabled) || !bot.isVisible)
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
                v3.x /= 2.7f;
                v3.z /= 5.0f;
                if(Mathf.Abs(v3.x) > Mathf.Abs(v3.z))
                {
                    bot.offscreenPointer.rectTransform.anchoredPosition = new Vector2(v3.x / Mathf.Abs(v3.x) * (((float)Screen.width) / mainCanvas.scaleFactor - 24.0f) / 2.0f, v3.z * ((float)Screen.height) / mainCanvas.scaleFactor / 2.0f);
                }
                else
                {
                    bot.offscreenPointer.rectTransform.anchoredPosition = new Vector2(v3.x * (((float)Screen.width) / mainCanvas.scaleFactor - 24.0f) / 2.0f, v3.z / Mathf.Abs(v3.z) * ((float)Screen.height) / mainCanvas.scaleFactor / 2.0f);
                }
            }
            botNode = botNode.Next;
        }

        if(smileyCooldown > 0.0f)
        {
            smileyCooldown -= Time.deltaTime;
            if(smileyCooldown <= 0.0f)
            {
                smileyCooldown = 0.0f;
                smileyIcon.enabled = false;
                smileyBackground.enabled = false;
            }
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

        if (touched && blockInput <= 0.0f)
        {
            switch (inputMode)
            {
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
                case 3:
                    inputDirection.x = posX / (float)Screen.width - 0.5f;
                    inputDirection.z = Mathf.Max(-0.5f, Mathf.Min(0.5f, (posY - (float)Screen.height * 0.5f) / (float)Screen.width));
                    inputDirection.z *= 1.6f;
                    Vector3 cameraShift = camera.transform.position - transform.position + Vector3.forward * 10.0f;
                    cameraShift.y = 0.0f;
                    cameraShift.x /= 2.7f * 2.0f;
                    cameraShift.z /= 2.7f * 2.0f;
                    inputDirection += cameraShift;
                    inputDirection *= 5.3f;
                    inputTouched = true;
                    break;
            }
        }

        if (blockInput > 0.0f)
        {
            if(touched)
            {
                Vector3 previousInputDirection = new Vector3(inputDirection.x, inputDirection.y, inputDirection.z);
                inputDirection.x = posX / (float)Screen.width - 0.5f;
                inputDirection.z = Mathf.Max(-0.5f, Mathf.Min(0.5f, (posY - (float)Screen.height * 0.5f) / (float)Screen.width));
                inputDirection.z *= 1.6f;
                Vector3 cameraShift = camera.transform.position - transform.position + Vector3.forward * 10.0f;
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
                releaseProgress += f;
                if(releaseProgress > 720.0f)
                {
                    releaseProgress = 0.0f;
                    BaseObjectMessage baseMessage = new BaseObjectMessage();
                    baseMessage.id = 1;
                    if (PhotonNetwork.networkingPeer.PeerState == ExitGames.Client.Photon.PeerStateValue.Connected)
                    {
                        PhotonNetwork.networkingPeer.OpCustom((byte)5, new Dictionary<byte, object> { { 245, baseMessage.Pack() } }, true);
                    }
                }
                lastInputDirection.x = previousInputDirection.x;
                lastInputDirection.y = previousInputDirection.y;
                lastInputDirection.z = previousInputDirection.z;
            }
            else
            {
                releaseProgress = 0.0f;
            }
            blockInput -= Time.deltaTime;
            if (blockInput < 0.0f)
            {
                blockInput = 0.0f;
            }
        }
        else if (releaseProgress > 0.0f)
        {
            releaseProgress = 0.0f;
        }

        if (stop)
        {
            if (inputCooldown > 0.0f && inputMode == 3)
            {
                inputCooldown -= Time.deltaTime;
            }
            else
            {
                if (route.Count > 0)
                {
                    route.RemoveFirst();
                    if (route.Count > 0)
                    {
                        routePoint = route.First.Value;
                        direction = (routePoint.destination - transform.position).normalized;
                        inputCooldown = (routePoint.destination - transform.position).magnitude / speed; // routePoint.timestamp - Time.time;
                    }
                }
                else
                {
                    direction *= Mathf.Max(0.01f, 1.0f - Time.deltaTime * 10.0f);
                }
            }
        }

        /*
        if (!inputTouched)
        {
            inputTargetingCooldown = 0.0f;
        }
        */
        applyInputCooldown -= Time.deltaTime;
        if(applyInputCooldown <= 0.0f)
        {
            applyInputCooldown += 0.1f;
            if (inputTouched)
            {
                inputTouched = false;
                inputTargetingCooldown += Time.deltaTime;
                direction.x = inputDirection.x;
                direction.z = inputDirection.z;
                //if (inputTargetingCooldown > 0.1f)
                //{
                    //inputCooldown = direction.magnitude * 5.3f / speed;
                    speed = 1.6f;
                    route = region.GetRoute(transform.position, transform.position + direction, speed, 0.0f);
                    //direction.Normalize();
                    inputCooldown = 0.0f;
                    direction *= 0.0f;
                    if (route.Count > 0)
                    {
                        routePoint = route.First.Value;
                        direction = (routePoint.destination - transform.position).normalized;
                        inputCooldown = (routePoint.destination - transform.position).magnitude / speed; // routePoint.timestamp - Time.time;
                    }
                //}
                //else
                //{
                //    direction = direction.normalized * 0.01f;
                //}
            }
            else
            {
                if (inputTargetingCooldown >= 0.2f)
                {
                    inputTargetingCooldown = 0.0f;
                }
                else if(inputTargetingCooldown > 0.0f)
                {
                    inputTargetingCooldown = 0.0f;
                    route.Clear();
                    routePoint = null;
                    inputCooldown = 0.0f;
                    //direction *= 0.0f;
                }
            }
        }

        Vector2 position2D = new Vector2(transform.position.x, transform.position.z);
        Vector2 direction2D = new Vector2(direction.x, direction.z);
        Vector2 newPosition2D = position2D + direction2D * speed * Time.deltaTime;

        if (inputSendCooldown <= 0.0f)
        {
            inputSendCooldown = 0.1f;
            RegionMoveMessage regionMoveMessage = new RegionMoveMessage();
            regionMoveMessage.destination = position2D + direction2D * speed * (inputCooldown + Time.deltaTime);
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

        /*
        if (mapNode != null && mapNode.coverageType == 2)
        {
            bushDistanceTraveled += (new Vector3(newPosition2D.x, transform.position.y, newPosition2D.y) - transform.position).magnitude * Random.Range(0.0f, 1.0f);
            if (bushDistanceTraveled > 5.0f)
            {
                bushDistanceTraveled = 0.0f;
                ShowDiscovered(0);
            }
        }
        */

        transform.position = new Vector3(newPosition2D.x, transform.position.y, newPosition2D.y);

        if (direction.magnitude > 0.001f)
        {
            normalDirection.x = direction.x;
            normalDirection.z = direction.z;
            normalDirection.Normalize();
            v3 = new Vector3(normalDirection.x, 0.0f, normalDirection.z);
            smoothDirection.Normalize();
            f = Mathf.Min(0.33f, Time.deltaTime * 5.0f);
            smoothDirection = smoothDirection * (1.0f - f) + new Vector3(v3.x, v3.z, 0.0f) * f;
            playerIcon.transform.localRotation = Quaternion.LookRotation(smoothDirection, -Vector3.forward);
        }

        ParticleSystem.EmissionModule emission1 = stepsPS1.emission;
        ParticleSystem.EmissionModule emission2 = stepsPS2.emission;
        ParticleSystem.EmissionModule emission3 = stepsPS3.emission;
        ParticleSystem.EmissionModule emission4 = stepsPS4.emission;

        if (traceType == 1)
        {

            if(emission3.enabled)
            {
                emission3.enabled = false;
                emission4.enabled = false;
            }

            if (direction.magnitude <= 0.5f && emission1.enabled)
            {
                emission1.enabled = false;
                emission2.enabled = false;
            }
            else if (direction.magnitude > 0.5f && !emission1.enabled)
            {
                emission1.enabled = true;
                emission2.enabled = true;
            }
            transform.position += Vector3.up * (0.0f - transform.position.y);

        }
        else if(traceType == 2)
        {

            if (emission1.enabled)
            {
                emission1.enabled = false;
                emission2.enabled = false;
            }

            if (direction.magnitude <= 0.5f && emission3.enabled)
            {
                emission3.enabled = false;
                emission4.enabled = false;
            }
            else if (direction.magnitude > 0.5f && !emission3.enabled)
            {
                emission3.enabled = true;
                emission4.enabled = true;
            }
            transform.position += Vector3.up * (0.0f - transform.position.y);

        }
        else if(traceType == 3)
        {

            if (emission3.enabled)
            {
                emission3.enabled = false;
                emission4.enabled = false;
            }
            if (emission1.enabled)
            {
                emission1.enabled = false;
            }
            if (direction.magnitude <= 0.5f && emission2.enabled)
            {
                emission2.enabled = false;
            }
            else if (direction.magnitude > 0.5f && !emission2.enabled)
            {
                emission2.enabled = true;
            }
            transform.position += Vector3.up * ((direction.magnitude * 0.3f - transform.position.y) * Mathf.Min(1.0f, Time.deltaTime * 10.0f));

        }

        /*
        if (transform.position.x < -9.0f)
        {
            transform.position += Vector3.right * (-9.0f - transform.position.x);
        }
        if (transform.position.x > 9.0f)
        {
            transform.position += Vector3.right * (9.0f - transform.position.x);
        }
        if (transform.position.z < -27.0f)
        {
            transform.position += Vector3.forward * (-27.0f - transform.position.z);
        }
        if (transform.position.z > 30.0f)
        {
            transform.position += Vector3.forward * (30.0f - transform.position.z);
        }
        */

        mapNode = map.FindNode(transform.position.x, transform.position.z);
        if(mapNode != null)
        {
            coverageType = mapNode.coverageType;
        }
        else
        {
            coverageType = 0;
        }
        /*
        if(coverageType != lastCoverageType || (!hidden && direction.magnitude < 0.1f) || (hidden && direction.magnitude >= 0.1f))
        {
            lastCoverageType = coverageType;
            Color newColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            if(coverageType == 2)
            {
                inputCooldown *= speed / 1.2f; // 0.8f
                //speed = 1.2f; // 0.8f
                newColor.a = 0.5f;
            }
            else if (coverageType == 1)
            {
                inputCooldown *= speed / 1.2f;
                //speed = 1.2f;
                newColor.a = 0.75f;
            }
            else
            {
                inputCooldown *= speed / 1.6f;
                //speed = 1.6f;
            }
            hidden = direction.magnitude < 0.1f;
            if (hidden)
            {
                newColor.a -= 0.25f;
            }
            //playerIconRenderer.color = newColor;
            //playerFaceRenderer.color = newColor;
        }
        */

        camera.transform.position += new Vector3(transform.position.x - camera.transform.position.x, 0.0f, transform.position.z - camera.transform.position.z - 10.0f) * Time.deltaTime * 5.0f;
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

        /*
        if (!exitButton.enabled)
        {
            if (transform.position.x < -8.0f || transform.position.x > 8.0f || transform.position.z < -26.0f || transform.position.z > 27.0f)
            {
                leaveCooldown = 5.0f;
                statusBar.text = "Переход через 5 секунд";
                exitButton.enabled = true;
                exitButton.image.enabled = true;
            }
        }
        else
        {
            if (!(transform.position.x < -8.0f || transform.position.x > 8.0f || transform.position.z < -26.0f || transform.position.z > 27.0f))
            {
                leaveCooldown = 0.0f;
                statusBar.text = "";
                exitButton.enabled = false;
                exitButton.image.enabled = false;
            }
            else if(leaveCooldown > 0.0f)
            {
                if (leaveCooldown + Time.deltaTime > Mathf.Ceil(leaveCooldown))
                {
                    statusBar.text = "Переход через " + Mathf.Ceil(leaveCooldown) + " секунд";
                }
            }
        }
        */
        if (leaveCooldown > 0.0f)
        {
            leaveCooldown -= Time.deltaTime;
            if (leaveCooldown + Time.deltaTime > Mathf.Ceil(leaveCooldown) && Mathf.Ceil(leaveCooldown) >= 0.0f)
            {
                statusBar.text = "Переход через " + Mathf.Ceil(leaveCooldown) + " секунд";
            }
        }

        /*
        Vector2 v1 = direction2D.normalized * speed;
        float x01 = transform.position.x;
        float y01 = transform.position.y;
        float vx1 = v1.x;
        float vy1 = v1.y;

        Vector2 v2 = (new Vector2(bots[0].direction.x, bots[0].direction.z)).normalized * speed;
        float x02 = bots[0].transform.position.x;
        float y02 = bots[0].transform.position.y;
        float vx2 = v2.x;
        float vy2 = v2.y;

        float tx = (x02 - x01) / (vx1 - vx2);
        float ty = (y02 - y01) / (vy1 - vy2);
        if (Mathf.Abs(tx - ty) < 0.5f)
        {
            if ((tx + ty) / 2.0f <= inputCooldown)
            {
                //Debug.LogWarning("!!!");
            }
        }
        //Debug.Log("D: " + Mathf.Abs(tx - ty) + " ; TX: " + tx + " ; TY: " + ty);
        */

        /*
        RaycastHit hit;
        if (Physics.SphereCast(hook.hook.transform.position - Vector3.up, 0.3f, Vector3.up, out hit, 2.0f, 255))
        {
            if (hit.collider.tag == "Enemy")
            {
                battleCooldown = 1.0f;
                hook.targetRank = hit.collider.gameObject.GetComponent<RegionBotBehavior>().rankModifier;
                GameObject.Destroy(hit.collider.gameObject.GetComponent<RegionBotBehavior>());
                hook.hook.transform.position = hit.collider.transform.position;
                battleIcon.transform.position = transform.position + (hit.collider.transform.position - transform.position).normalized * 0.5f + Vector3.up * 0.1f;
                battleIcon.enabled = true;
                hit.collider.transform.parent = hook.hook.transform;
                hook.Rollback();
            }
        }

        if (Physics.SphereCast(transform.position - Vector3.up, 0.3f, Vector3.up, out hit, 2.0f, 255))
        {
            if (hit.collider.tag == "Enemy")
            {
                battleCooldown = 1.0f;
                hook.targetRank = hit.collider.gameObject.GetComponent<RegionBotBehavior>().rankModifier;
                GameObject.Destroy(hit.collider.gameObject.GetComponent<RegionBotBehavior>());
                hook.hook.transform.position = hit.collider.transform.position;
                battleIcon.transform.position = transform.position + (hit.collider.transform.position - transform.position).normalized * 0.5f + Vector3.up * 0.1f;
                battleIcon.enabled = true;
                hit.collider.transform.parent = hook.hook.transform;
                hook.Rollback();
            }
        }
        */

        if (discoveredTimer > 0.0f)
        {
            discoveredTimer -= Time.deltaTime;
            discoveredFrame1.rectTransform.anchoredPosition = new Vector2(0.0f, Mathf.Max(0.0f, Mathf.Abs(discoveredTimer * 2.0f - 2.0f) - 1.0f) * 144.0f);
            discoveredFrame2.rectTransform.anchoredPosition = new Vector2(0.0f, Mathf.Max(0.0f, Mathf.Abs(discoveredTimer * 3.0f - 3.0f) - 1.0f) * 144.0f);
            if (!discoveredFrame1.enabled)
            {
                discoveredFrame1.enabled = true;
                discoveredFrame2.enabled = true;
                discoveredIcon.enabled = true;
            }
            else if (discoveredTimer <= 0.0f)
            {
                discoveredFrame1.enabled = false;
                discoveredFrame2.enabled = false;
                discoveredIcon.enabled = false;
            }
        }

        if (direction.magnitude > 0.75f)
        {
            for (i = 0; i < taskTargets.Length; i++)
            {
                taskTargets[i].Process(this);
            }
        }

        if (taskPointer.enabled)
        {
            v3 = taskTarget.transform.position - camera.transform.position;
            v3.y = 0.0f;
            /*
            if ((taskTarget.transform.position - transform.position).magnitude < 0.5f)
            {
                ShowDiscovered(1);
                taskPointer.enabled = false;
                taskTarget.enabled = false;
            }
            */
            v3.x /= 2.7f;
            v3.z /= 5.0f;
            if (Mathf.Abs(v3.x) >= 1.0f || Mathf.Abs(v3.z) >= 1.0f)
            {
                if (Mathf.Abs(v3.x) > Mathf.Abs(v3.z))
                {
                    taskPointer.rectTransform.anchoredPosition = new Vector2(v3.x / Mathf.Abs(v3.x) * (((float)Screen.width) / mainCanvas.scaleFactor - 24.0f) / 2.0f, v3.z * ((float)Screen.height) / mainCanvas.scaleFactor / 2.0f);
                }
                else
                {
                    taskPointer.rectTransform.anchoredPosition = new Vector2(v3.x * (((float)Screen.width) / mainCanvas.scaleFactor - 24.0f) / 2.0f, v3.z / Mathf.Abs(v3.z) * ((float)Screen.height) / mainCanvas.scaleFactor / 2.0f);
                }
            }
            else
            {
                if (Mathf.Abs(v3.x) > Mathf.Abs(v3.z))
                {
                    taskPointer.rectTransform.anchoredPosition = new Vector2(v3.x * (((float)Screen.width) / mainCanvas.scaleFactor - 24.0f) / 2.0f, v3.z * ((float)Screen.height) / mainCanvas.scaleFactor / 2.0f);
                }
                else
                {
                    taskPointer.rectTransform.anchoredPosition = new Vector2(v3.x * (((float)Screen.width) / mainCanvas.scaleFactor - 24.0f) / 2.0f, v3.z * ((float)Screen.height) / mainCanvas.scaleFactor / 2.0f);
                }
            }
        }

    }

    public void SetState(Vector2 destination, float moveTime)
    {
        if (moveTime == 0.0f)
        {
            transform.position = new Vector3(destination.x, transform.position.y, destination.y);
        }
        else
        {
            direction = new Vector3(destination.x - transform.position.x, 0.0f, destination.y - transform.position.z);
            inputCooldown = moveTime;
            speed = direction.magnitude / inputCooldown;
            direction.Normalize();

            blockInput = inputCooldown;
            applyInputCooldown = inputCooldown;
        }
        inputTargetingCooldown = 0.0f;
        route.Clear();
    }

    public void SetOpponentState(string id, Vector2 destination, float moveTime)
    {
        int i;
        RegionBotBehavior bot;
        LinkedListNode<RegionBotBehavior> botNode = bots.First;
        while(botNode != null)
        {
            bot = botNode.Value;
            if(bot.playerId == id)
            {
                bot.SetState(destination, moveTime);
                return;
            }
            botNode = botNode.Next;
        }
        bot = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Bot")).GetComponent<RegionBotBehavior>();
        bot.player = this.gameObject;
        bot.playerId = id;
        bot.map = map;
        bot.mapNode = map.FindNode(bot.transform.position.x, bot.transform.position.z);
        bot.offscreenPointer = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/BotOffscreenPointer")).GetComponent<Image>();
        bot.offscreenPointer.rectTransform.parent = mainCanvas.transform;
        bot.offscreenPointer.rectTransform.anchoredPosition = new Vector2(-1000.0f, 0.0f);
        bots.AddLast(bot);
        bot.SetState(destination, moveTime);
    }

    public void RemoveOpponent(string id)
    {
        int i;
        RegionBotBehavior bot;
        LinkedListNode<RegionBotBehavior> botNode = bots.First;
        while (botNode != null)
        {
            bot = botNode.Value;
            if (bot.playerId == id)
            {
                GameObject.Destroy(bot.gameObject);
                bots.Remove(botNode);
                return;
            }
            botNode = botNode.Next;
        }
    }

    public void ThrowOpponentHook(string id, Vector2 destination, float moveTime)
    {
        int i;
        RegionBotBehavior bot;
        LinkedListNode<RegionBotBehavior> botNode = bots.First;
        while (botNode != null)
        {
            bot = botNode.Value;
            if (bot.playerId == id)
            {
                bot.ThrowHook(destination, moveTime);
                return;
            }
            botNode = botNode.Next;
        }
    }

    public void ShowOpponentChat(string id, int iconId)
    {
        int i;
        RegionBotBehavior bot;
        LinkedListNode<RegionBotBehavior> botNode = bots.First;
        while (botNode != null)
        {
            bot = botNode.Value;
            if (bot.playerId == id)
            {
                bot.ShowChat(iconId);
                return;
            }
            botNode = botNode.Next;
        }
    }

    public void ThrowHook(Vector2 destination, float time)
    {
        Vector3 v3;
        if(!hook.enabled)
        {
            hook.transform.position = transform.position;
            v3 = (new Vector3(destination.x, hook.transform.position.y, destination.y) - hook.transform.position);
            hook.velocity = v3.normalized * (v3.magnitude / time);
            //hook.destinationTimemark = time;
            hook.cooldown = 8.0f;
            //hook.rollbackTimemark = 2.5f;
            hook.Show(time);
        }
        else if(destination.magnitude != 0.0f)
        {
            v3 = new Vector3(destination.x, 0.0f, destination.y) - hook.transform.position;
            v3.y = 0.0f;
            hook.velocity = v3.normalized * (v3.magnitude / time);
            hook.Move(time);
        }
        else
        {
            hook.Rollback();
        }
    }

    public void ThrowHook()
    {
        if (blockInput > 0.0f)
        {
            return;
        }
        Vector3 v = normalDirection * 6.0f; // playerIcon.transform.forward * 6.0f;
        RegionThrowMessage regionThrowMessage = new RegionThrowMessage();
        Vector2 position2D = new Vector2(transform.position.x, transform.position.z);
        Vector2 direction2D = new Vector2(v.x, v.z);
        regionThrowMessage.throwTimemark = 1.5f;
        regionThrowMessage.destination = position2D + direction2D * regionThrowMessage.throwTimemark;
        Debug.Log("HOOK DIRECTION: " + v + " ; " + position2D + " ; " + direction2D + " ; " + regionThrowMessage.destination + " ; " + regionThrowMessage.throwTimemark);
        PhotonNetwork.networkingPeer.OpCustom((byte)3, new Dictionary<byte, object> { { 245, regionThrowMessage.Pack() } }, true);
    }

    public void ShowDiscovered(int iconId)
    {
        switch (iconId)
        {
            case 0:
                smileyIcon.sprite = someFoundSprite;
                break;
            case 1:
                smileyIcon.sprite = taskCompleteSprite;
                break;
            case 2:
                smileyIcon.sprite = itemFoundSprite;
                break;
        }
        smileyIcon.transform.localScale = Vector3.one * 0.5f;
        smileyCooldown = 2.0f;
        smileyBackground.enabled = true;
        smileyIcon.enabled = true;
    }

    public void ShowOpponentDiscovered(string id, int iconId)
    {
        int i;
        RegionBotBehavior bot;
        LinkedListNode<RegionBotBehavior> botNode = bots.First;
        while (botNode != null)
        {
            bot = botNode.Value;
            if (bot.playerId == id)
            {
                bot.ShowDiscovered(iconId);
                return;
            }
            botNode = botNode.Next;
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
        RegionTimedIcon icon = (RegionTimedIcon)((GameObject)GameObject.Instantiate(timedIconPrefab)).GetComponent<RegionTimedIcon>();
        switch(iconId)
        {
            case 1:
                icon.transform.position = new Vector3(position.x, 0.8f, position.y - 0.4f);
                icon.sprite.transform.localScale = Vector3.one * 0.5f;
                icon.sprite.sprite = startBattleSprite;
                icon.cooldown = 4.0f;
                break;
            case 2:
                icon.transform.position = new Vector3(position.x, 0.3f, position.y - 0.1f);
                icon.sprite.transform.localScale = Vector3.one * 1.0f;
                icon.sprite.sprite = hookBounceSprite;
                icon.cooldown = 0.5f;
                break;
        }
    }

    public void StartLeaving(float time)
    {
        leaveCooldown = time;
        statusBar.text = "Переход через 5 секунд";
        exitButton.enabled = true;
        exitButton.image.enabled = true;
    }

    public void StopLeaving()
    {
        leaveCooldown = 0.0f;
        statusBar.text = "";
        exitButton.enabled = false;
        exitButton.image.enabled = false;
    }

    public void SetGold(int newGold)
    {
        // ... show new gold and gold change
        goldLabel.text = "+ " + newGold;
        gold = newGold;
    }

}
