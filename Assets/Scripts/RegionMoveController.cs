using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RegionMoveController : MonoBehaviour {


    public float speed = 2.0f;
    public Camera camera;
    public GameObject playerIcon;
    public SpriteRenderer battleIcon;
    public Image joystickKey;
    public Image joystickFrame;
    public Button[] inputModeButtons = new Button[4];

    public int inputMode = 0;
    public float inputCooldown = 0.0f;

    private Vector3 direction = Vector3.zero;
    private float battleCooldown = 0.0f;

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
        GameObject bot;
        for(i = 0; i < 10; i++)
        {
            bot = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Bot"));
        }
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


    }

    // Update is called once per frame
    void Update () {

        bool stop = true;
        bool touched = false;
        float posX = 0.0f;
        float posY = 0.0f;

        if (battleCooldown > 0.0f)
        {
            battleCooldown -= Time.deltaTime;
            if (battleCooldown <= 0.0f)
            {
                Application.LoadLevel("battle");
            }
            return;
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
                    inputCooldown = direction.magnitude * 5.3f;
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
        transform.position += direction * speed * Time.deltaTime;
        playerIcon.transform.localRotation = Quaternion.LookRotation(Vector3.right * direction.x + Vector3.up * direction.z, Vector3.forward);
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
