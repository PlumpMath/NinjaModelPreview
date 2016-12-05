using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class RegionMoveController : MonoBehaviour {


    public float speed = 1.0f;
    public Camera camera;
    public GameObject playerIcon;
    public SpriteRenderer playerIconRenderer;
    public SpriteRenderer playerFaceRenderer;
    public SpriteRenderer battleIcon;
    public Image joystickKey;
    public Image joystickFrame;
    public Button[] inputModeButtons = new Button[4];

    public RegionMap map = new RegionMap();
    public RegionMapNode mapNode = null;

    public int coverageType = 0;
    public int lastCoverageType = 0;
    public bool hidden = false;

    public int inputMode = 3;
    public float inputCooldown = 0.0f;

    private Vector3 direction = Vector3.zero;
    private float battleCooldown = 0.0f;
    private float botActionCooldown = 0.0f;

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
                if((bot.transform.position - transform.position).magnitude > 7.5f)
                {
                    bot.transform.position = transform.position + (new Vector3(Random.Range(-1.0f, 1.0f), 0.0f, Random.Range(-1.0f, 1.0f))).normalized * 5.0f;
                }
                v3 = bot.transform.position - transform.position;
                if(v3.magnitude < bot.visibleDistance)
                {
                    bot.playerIconRenderer.enabled = true;
                    bot.playerFaceRenderer.enabled = true;
                }
                else
                {
                    bot.playerIconRenderer.enabled = false;
                    bot.playerFaceRenderer.enabled = false;
                }
            }
        }

        if (Input.GetMouseButton(0))
        {
            posX = Input.mousePosition.x;
            posY = Input.mousePosition.y;
            touched = true;
        }
        if (Input.touchCount > 0)
        {
            posX = Input.touches[0].position.x;
            //posY = (float)Screen.height - Input.touches[0].position.y;
            posY = Input.touches[0].position.y;
            touched = true;
        }

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
            Debug.Log("dir.mag: " + direction.magnitude);
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
        if (Physics.SphereCast(transform.position - Vector3.up, 0.5f, Vector3.up, out hit, 2.0f, 255))
        {
            if (hit.collider.tag == "Player")
            {
                battleCooldown = 1.0f;
                battleIcon.enabled = true;
                GameObject.Destroy(hit.collider.gameObject.GetComponent<RegionBotBehavior>());
            }
        }

    }
}
