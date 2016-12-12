using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class RegionMoveController : MonoBehaviour {


    public float speed = 1.0f;
    public Camera camera;
    public Canvas mainCanvas;
    public GameObject playerIcon;
    public SpriteRenderer playerIconRenderer;
    public SpriteRenderer playerFaceRenderer;
    public SpriteRenderer battleIcon;
    public Image discoveredFrame1;
    public Image discoveredFrame2;
    public Image discoveredIcon;
    public Image taskPointer;
    public SpriteRenderer taskTarget;
    public Sprite someFoundSprite;
    public Sprite taskCompleteSprite;
    public Image joystickKey;
    public Image joystickFrame;
    public Button[] inputModeButtons = new Button[4];
    public Button hookButton;

    public RegionMap map = new RegionMap();
    public RegionMapNode mapNode = null;

    public int coverageType = 0;
    public int lastCoverageType = 0;
    public bool hidden = false;

    public int inputMode = 3;
    public float inputCooldown = 0.0f;
    public int ignoreFinger = -1;

    public RegionHook hook = null;

    private Vector3 direction = Vector3.zero;
    private float battleCooldown = 0.0f;
    private float botActionCooldown = 0.0f;
    private float bushDistanceTraveled = 0.0f;
    private float discoveredTimer = 0.0f;

    private RegionBotBehavior[] bots = new RegionBotBehavior[0];

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

        int i;

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

        hookButton.onClick.AddListener(delegate() {
            ThrowHook();
        });
        hook.Hide();

        map.Load("map_01_areas");
        mapNode = map.FindNode(transform.position.x, transform.position.z);

        GameObject bot;
        bots = new RegionBotBehavior[2];
        for (i = 0; i < bots.Length; i++)
        {
            bot = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Bot"));
            bots[i] = bot.GetComponent<RegionBotBehavior>();
            bots[i].map = map;
            bots[i].mapNode = map.FindNode(bots[i].transform.position.x, bots[i].transform.position.z);
            bots[i].offscreenPointer = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/BotOffscreenPointer")).GetComponent<Image>();
            bots[i].offscreenPointer.rectTransform.parent = mainCanvas.transform;
            bots[i].offscreenPointer.rectTransform.anchoredPosition = new Vector2(-1000.0f, 0.0f);
        }

    }

    // Update is called once per frame
    void Update () {

        int i;
        bool stop = true;
        bool touched = false;
        float posX = 0.0f;
        float posY = 0.0f;
        RegionBotBehavior bot;
        Vector3 v3;

        if (battleCooldown > 0.0f)
        {
            battleCooldown -= Time.deltaTime;
            if (battleCooldown <= 0.0f)
            {
                Application.LoadLevel("battle");
            }
            return;
        }

        botActionCooldown -= Time.deltaTime;
        if (botActionCooldown < 0.0f)
        {
            botActionCooldown = 0.1f;
            for(i = 0; i < bots.Length; i++)
            {
                bot = bots[i];
                if((bot.transform.position - transform.position).magnitude > 9.0f)
                {
                    bot.transform.position = transform.position + (new Vector3(Random.Range(-1.0f, 1.0f), 0.0f, Random.Range(-1.0f, 1.0f))).normalized * 5.0f;
                }
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
                if (((v3.x <= 2.7f && v3.x >= -2.7f && v3.y <= 5.0f && v3.y >= -5.0f) && bot.offscreenPointer.enabled) || !bot.isVisible)
                {
                    bot.offscreenPointer.enabled = false;
                }
            }
        }

        for (i = 0; i < bots.Length; i++)
        {
            bot = bots[i];
            if(bot.offscreenPointer.enabled)
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

        if (touched)
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
                    direction.x = posX / (float)Screen.width - 0.5f;
                    direction.z = Mathf.Max(-0.5f, Mathf.Min(0.5f, (posY - (float)Screen.height * 0.5f) / (float)Screen.width));
                    Vector3 cameraShift = camera.transform.position - transform.position;
                    cameraShift.y = 0.0f;
                    cameraShift.x /= 2.7f * 2.0f;
                    cameraShift.z /= 2.7f * 2.0f;
                    direction += cameraShift;
                    inputCooldown = direction.magnitude * 5.3f / speed;
                    direction.Normalize();
                    break;
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
                direction *= 1.0f - Time.deltaTime * 5.0f;
            }
        }

        Vector2 position2D = new Vector2(transform.position.x, transform.position.z);
        Vector2 direction2D = new Vector2(direction.x, direction.z);
        Vector2 newPosition2D = position2D + direction2D * speed * Time.deltaTime;

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

        if (mapNode.coverageType == 2)
        {
            bushDistanceTraveled += (new Vector3(newPosition2D.x, transform.position.y, newPosition2D.y) - transform.position).magnitude * Random.Range(0.0f, 1.0f);
            if (bushDistanceTraveled > 2.0f)
            {
                bushDistanceTraveled = 0.0f;
                ShowDiscovered(0);
            }
        }

        transform.position = new Vector3(newPosition2D.x, transform.position.y, newPosition2D.y);

        if (direction.magnitude > 0.1f)
        {
            playerIcon.transform.localRotation = Quaternion.LookRotation(Vector3.right * direction.x + Vector3.up * direction.z, Vector3.forward);
        }
        if (transform.position.x < -10.0f)
        {
            transform.position += Vector3.right * (-10.0f - transform.position.x);
        }
        if (transform.position.x > 10.0f)
        {
            transform.position += Vector3.right * (10.0f - transform.position.x);
        }
        if (transform.position.z < -10.0f)
        {
            transform.position += Vector3.forward * (-10.0f - transform.position.z);
        }
        if (transform.position.z > 10.0f)
        {
            transform.position += Vector3.forward * (10.0f - transform.position.z);
        }

        mapNode = map.FindNode(transform.position.x, transform.position.z);
        if(mapNode != null)
        {
            coverageType = mapNode.coverageType;
        }
        else
        {
            coverageType = 0;
        }
        if(coverageType != lastCoverageType || (!hidden && direction.magnitude < 0.1f) || (hidden && direction.magnitude >= 0.1f))
        {
            lastCoverageType = coverageType;
            Color newColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            if(coverageType == 2)
            {
                inputCooldown *= speed / 0.8f;
                speed = 0.8f;
                newColor.a = 0.5f;
            }
            else if (coverageType == 1)
            {
                inputCooldown *= speed / 1.2f;
                speed = 1.2f;
                newColor.a = 0.75f;
            }
            else
            {
                inputCooldown *= speed / 1.6f;
                speed = 1.6f;
            }
            hidden = direction.magnitude < 0.1f;
            if (hidden)
            {
                newColor.a -= 0.25f;
            }
            playerIconRenderer.color = newColor;
            playerFaceRenderer.color = newColor;
        }

        camera.transform.position += new Vector3(transform.position.x - camera.transform.position.x, 0.0f, transform.position.z - camera.transform.position.z) * Time.deltaTime * 5.0f;
        if (camera.transform.position.x < -7.0f)
        {
            camera.transform.position += Vector3.right * (-7.0f - camera.transform.position.x);
        }
        if (camera.transform.position.x > 7.0f)
        {
            camera.transform.position += Vector3.right * (7.0f - camera.transform.position.x);
        }
        if (camera.transform.position.z < -5.0f)
        {
            camera.transform.position += Vector3.forward * (-5.0f - camera.transform.position.z);
        }
        if (camera.transform.position.z > 5.0f)
        {
            camera.transform.position += Vector3.forward * (5.0f - camera.transform.position.z);
        }

        RaycastHit hit;
        if (Physics.SphereCast(hook.hook.transform.position - Vector3.up, 0.3f, Vector3.up, out hit, 2.0f, 255))
        {
            if (hit.collider.tag == "Player")
            {
                battleCooldown = 1.0f;
                GameObject.Destroy(hit.collider.gameObject.GetComponent<RegionBotBehavior>());
                hook.hook.transform.position = hit.collider.transform.position;
                battleIcon.transform.position = transform.position + (hit.collider.transform.position - transform.position).normalized * 0.5f + Vector3.up * 0.1f;
                battleIcon.enabled = true;
                hit.collider.transform.parent = hook.hook.transform;
                hook.Rollback();
            }
        }

        if(discoveredTimer > 0.0f)
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

        if (taskPointer.enabled)
        {
            v3 = taskTarget.transform.position - camera.transform.position;
            v3.y = 0.0f;
            if ((taskTarget.transform.position - transform.position).magnitude < 0.5f)
            {
                ShowDiscovered(1);
                taskPointer.enabled = false;
                taskTarget.enabled = false;
            }
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

    public void ThrowHook()
    {
        if(!hook.enabled)
        {
            hook.transform.position = transform.position;
            hook.velocity = playerIcon.transform.forward * 3.0f;
            hook.Show();
        }
    }

    public void ShowDiscovered(int iconId)
    {
        switch(iconId)
        {
            case 0:
                discoveredIcon.sprite = someFoundSprite;
                break;
            case 1:
                discoveredIcon.sprite = taskCompleteSprite;
                break;
        }
        discoveredTimer = 2.0f;
    }

}
