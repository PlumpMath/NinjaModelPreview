using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class Tasks : MonoBehaviour {

    public EventHandler<EventArgs> OnThrow;

    public Image bloodScreen;
    public Image staminaBar;
    public PlayerController[] players = new PlayerController[2];
    public GameObject[] obstructions = new GameObject[0];

    public Button uiFPSButton;
    public Text uiFPSLabel;

    public Text uiProgressLabel;
    public GameObject dummy;

    private float[] uiPositions = new float[3];
    private int task = 0;
    public int hits = 0;

    private float lastFPS = 0.0f;
    private float bloodCooldown = 0.0f;

    // Use this for initialization
    void Start () {

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
                    if (dx > 20.0f)
                    {
                        obj.transform.position -= Vector3.right * 40.0f;
                    }
                    if (dx < -20.0f)
                    {
                        obj.transform.position += Vector3.right * 40.0f;
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
            }
            bloodScreen.color = new Color(1.0f, 1.0f, 1.0f, (1.0f - Mathf.Abs(bloodCooldown - 0.125f) * 8.0f));
        }

        staminaBar.rectTransform.sizeDelta = new Vector2(players[0].stamina * Screen.width, staminaBar.rectTransform.sizeDelta.y);

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
    }

    public void HitMe()
    {
        bloodCooldown = 0.25f;
        bloodScreen.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        bloodScreen.enabled = true;
    }

}
