using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class Tasks : MonoBehaviour {

    public EventHandler<EventArgs> OnThrow;

    public Image bloodScreen;
    public Image bloodScreenVeins;
    public Image staminaBar;
    public Image staminaIcon;
    public Text staminaLabel;
    public Image healthBar;
    public Text healthBarLabel;
    public Image opponentHealthBar;
    public Text opponentHealthBarLabel;
    public PlayerController[] players = new PlayerController[2];
    public GameObject[] obstructions = new GameObject[0];

    public Image staminaBeacon1;
    public Image staminaBeacon2;

    public Button uiFPSButton;
    public Text uiFPSLabel;

    public Text uiProgressLabel;
    public GameObject dummy;

    private float[] uiPositions = new float[3];
    private int task = 0;
    public int hits = 0;

    private float lastFPS = 0.0f;
    private float bloodCooldown = 0.0f;
    private bool staminaInsufficient = false;

    // Use this for initialization
    void Start () {

        staminaBeacon1.rectTransform.anchoredPosition = new Vector2(Screen.width / 3.0f, -2.0f);
        staminaBeacon2.rectTransform.anchoredPosition = new Vector2(Screen.width / 3.0f * 2.0f, -2.0f);

        uiPositions[0] = -35.0f * 10.0f;
        uiPositions[1] = -9.0f * 10.0f;
        uiPositions[2] = -86.0f * 10.0f;

        if (uiProgressLabel != null)
        {

            uiProgressLabel.text = hits + " / 3";
            uiProgressLabel.rectTransform.localPosition = new Vector3(uiProgressLabel.rectTransform.localPosition.x, uiPositions[task], 0.0f);

            uiFPSButton.onClick.AddListener(delegate ()
            {
                if (uiFPSLabel.color == Color.white)
                {
                    uiFPSLabel.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
                }
                else
                {
                    uiFPSLabel.color = Color.white;
                }
            });

        }

    }

    // Update is called once per frame
    void Update () {

        int i;
        float dx;
        float f;
        float f2;
        PlayerController me;
        GameObject obj;

        string screenSize = "Size: " + (Mathf.Round(Screen.height / Screen.dpi * 10.0f) * 0.1f) + " inch";
        string screenScale = "Scale: " + Mathf.Round(Mathf.Min(1.0f, Mathf.Max(0.6f, 3.0f / (Screen.height / Screen.dpi))) * 100.0f) * 0.01f;

        lastFPS += (Mathf.Round(1.0f / Time.deltaTime) - lastFPS) * Time.deltaTime;
        uiFPSLabel.text = Mathf.Round(lastFPS)  + " FPS\n" + screenSize + "\n" + screenScale;

        me = players[0];
        for(i = 0; i < obstructions.Length; i++)
        {
            obj = obstructions[i];
            if (obj != null)
            {
                if (obj.name == "Ground")
                {
                    dx = (obj.transform.position - me.transform.position).x;
                    if (dx > 100.0f)
                    {
                        obj.transform.position -= Vector3.right * 200.0f;
                    }
                    if (dx < -100.0f)
                    {
                        obj.transform.position += Vector3.right * 200.0f;
                    }
                }
                else
                {
                    dx = (obj.transform.position - me.transform.position).x;
                    if (dx > 40.0f)
                    {
                        obj.transform.position -= Vector3.right * 80.0f;
                    }
                    if (dx < -40.0f)
                    {
                        obj.transform.position += Vector3.right * 80.0f;
                    }
                }
            }
        }


        if (bloodCooldown > 0.0f)
        {
            bloodCooldown -= Time.deltaTime;
            if (bloodCooldown < 0.0f)
            {
                bloodCooldown = 0.0f;
                bloodScreen.enabled = false;
                bloodScreenVeins.enabled = false;
            }
            f2 = Mathf.Pow(Mathf.Max(0.0f, Mathf.Sin((Time.time - Mathf.Floor(Time.time)) * Mathf.PI)), 16.0f) + Mathf.Pow(Mathf.Max(0.0f, Mathf.Sin((Time.time - Mathf.Floor(Time.time) + 0.25f) * Mathf.PI)), 16.0f);
            f = Mathf.Min(1.0f, (Mathf.Max(0.0f, Mathf.Min(1.0f, bloodCooldown * 2.0f - 3.5f)) + f2));
            bloodScreen.color = new Color(f, f, f, Mathf.Pow(Mathf.Min(1.0f, Mathf.Max(0.0f, Mathf.Min(1.0f, bloodCooldown - 3.0f)) + (1.0f - bloodCooldown * 0.25f) * f2 * 0.3f), 2.0f));
            if(!bloodScreen.enabled)
            {
                bloodScreen.enabled = true;
            }
            bloodScreenVeins.color = new Color(0.7f + f2 * 0.3f, f2 * 0.1f, f2 * 0.1f, Mathf.Max(0.0f, Mathf.Min(1.0f, (bloodCooldown * 0.5f - 1.0f) + f2 * 0.2f)));
            if (!bloodScreenVeins.enabled)
            {
                bloodScreenVeins.enabled = true;
            }
        }

        staminaBar.rectTransform.sizeDelta = new Vector2(players[0].stamina * Screen.width, staminaBar.rectTransform.sizeDelta.y);
        if(!staminaInsufficient && players[0].stamina < 0.33f)
        {
            staminaInsufficient = true;
            staminaBar.color = Color.red;
            staminaIcon.color = new Color(0.5f, 0.05f, 0.05f, 1.0f);
            staminaLabel.color = new Color(0.5f, 0.05f, 0.05f, 1.0f);
        }
        else if(staminaInsufficient && players[0].stamina >= 0.33f)
        {
            staminaInsufficient = false;
            staminaBar.color = Color.white;
            staminaIcon.color = Color.white;
            staminaLabel.color = Color.white;
        }

        healthBar.fillAmount = Mathf.Max(0.0f, Mathf.Min(1.0f, players[0].health));
        healthBarLabel.text = Mathf.Ceil(healthBar.fillAmount * 100.0f) + "";
        opponentHealthBar.fillAmount = Mathf.Max(0.0f, Mathf.Min(1.0f, players[1].health));
        opponentHealthBarLabel.text = Mathf.Ceil(opponentHealthBar.fillAmount * 100.0f) + "";
        staminaLabel.text = "x " + Mathf.Floor(players[0].stamina / 0.33f);

    }

    public void Hit (float y)
    {
        if (y < 2.8f && task == 2)
        {
            hits++;
        }
        else if (y > 4.8f && task == 1)
        {
            hits++;
        }
        else if(y < 4.8f && y > 2.8f && task == 0)
        {
            hits++;
        }
        if(hits >= 3)
        {
            hits = 0;
            task = Math.Max(0, Math.Min(2, UnityEngine.Random.Range(0, 3)));
            //dummy.transform.position += Vector3.right * (Mathf.Round(UnityEngine.Random.Range(-1.4f, 1.4f)) * 5.0f - dummy.transform.position.x);
        }
        uiProgressLabel.text = hits + " / 3";
        uiProgressLabel.rectTransform.localPosition = new Vector3(uiProgressLabel.rectTransform.localPosition.x, uiPositions[task], 0.0f);
        players[1].health -= UnityEngine.Random.Range(0.05f, 0.08f);
        if(players[1].health <= 0.0f)
        {
            Application.LoadLevel("region");
        }
    }

    public void HitMe()
    {
        bloodCooldown = 4.0f;
        players[0].health -= UnityEngine.Random.Range(0.05f, 0.08f);
        if (players[0].health <= 0.0f)
        {
            Application.LoadLevel("region");
        }
    }

}
