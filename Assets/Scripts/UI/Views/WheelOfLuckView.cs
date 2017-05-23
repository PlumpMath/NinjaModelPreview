using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WheelOfLuckView : MonoBehaviour {

    public MapController map;
    public Canvas canvas;
    public Button closeButton;

    public SlotBar[] baseBars = new SlotBar[9];
    public SlotBar[] resultBars = new SlotBar[3];
    public SlotBar finalBar;

    public Button[] rollButtons = new Button[3];

    private int state = 0;
    private float cooldown = 0.0f;

    // Use this for initialization
    void Start () {

        closeButton.onClick.AddListener(delegate () {
            Close();
        });

        rollButtons[0].onClick.AddListener(delegate() {
            PushRow(0);
        });
        rollButtons[1].onClick.AddListener(delegate () {
            PushRow(1);
        });
        rollButtons[2].onClick.AddListener(delegate () {
            PushRow(2);
        });


    }

    public void PushRow(int row)
    {
        if (state == 0)
        {
            if (!finalBar.rolling)
            {
                Flush();
            }
            baseBars[row * 3].rolling = true;
            baseBars[row * 3 + 1].rolling = true;
            baseBars[row * 3 + 2].rolling = true;
            CheckState();
        }
    }

    public void Flush()
    {
        int i;
        for (i = 0; i < 9; i++)
        {
            baseBars[i].rolling = false;
            baseBars[i].highlight.enabled = false;
            baseBars[i].bar.anchoredPosition *= 0.0f;
        }
        for (i = 0; i < 3; i++)
        {
            resultBars[i].rolling = true;
            resultBars[i].highlight.enabled = false;
            resultBars[i].bar.anchoredPosition *= 0.0f;
        }
        finalBar.rolling = true;
        finalBar.highlight.enabled = false;
        finalBar.bar.anchoredPosition *= 0.0f;
    }

    public void CheckState()
    {
        bool b;
        int i;
        switch(state)
        {
            case 0:
                b = true;
                for (i = 0; i < 9; i++)
                {
                    if (!baseBars[i].rolling)
                    {
                        b = false;
                    }
                }
                if (b)
                {
                    state = 1;
                    cooldown = 0.2f;
                }
                break;
        }
    }

    // Update is called once per frame
    void Update () {

        bool b;
        int i;
        if(state == 1)
        {
            if(cooldown > 0.0f)
            {
                cooldown -= Time.deltaTime;
            }
            else
            {
                cooldown = 0.2f;
                b = false;
                for(i = 0; i < 9; i++)
                {
                    if(baseBars[i].rolling)
                    {
                        baseBars[i].rolling = false;
                        baseBars[i].bar.anchoredPosition += Vector2.up * (Mathf.Floor(baseBars[i].bar.anchoredPosition.y / 130.0f) * 130.0f - baseBars[i].bar.anchoredPosition.y);
                        b = true;
                        i = 9;
                    }
                }
                if(b == false)
                {
                    state = 2;
                }
            }
        }
        if (state == 2)
        {
            for (i = 0; i < 3; i++)
            {
                resultBars[i].rolling = false;
                resultBars[i].bar.anchoredPosition = Vector2.up * baseBars[6 + i].bar.anchoredPosition.y;
                baseBars[6 + i].highlight.enabled = true;
                if (Mathf.Abs(resultBars[i].bar.anchoredPosition.y - baseBars[3 + i].bar.anchoredPosition.y) < 50.0f)
                {
                    resultBars[i].bar.anchoredPosition += Vector2.right * (-130.0f - resultBars[i].bar.anchoredPosition.x);
                    baseBars[3 + i].highlight.enabled = true;
                    if (Mathf.Abs(resultBars[i].bar.anchoredPosition.y - baseBars[i].bar.anchoredPosition.y) < 50.0f)
                    {
                        resultBars[i].bar.anchoredPosition += Vector2.right * (-260.0f - resultBars[i].bar.anchoredPosition.x);
                        baseBars[i].highlight.enabled = true;
                    }
                }
            }
            state = 3;
        }
        if(state == 3)
        {
            finalBar.rolling = false;
            finalBar.bar.anchoredPosition = Vector2.up * resultBars[2].bar.anchoredPosition.y;
            resultBars[2].highlight.enabled = true;
            if (Mathf.Abs(finalBar.bar.anchoredPosition.y - resultBars[1].bar.anchoredPosition.y) < 50.0f)
            {
                resultBars[1].highlight.enabled = true;
                if (Mathf.Abs(finalBar.bar.anchoredPosition.y - resultBars[0].bar.anchoredPosition.y) < 50.0f)
                {
                    resultBars[0].highlight.enabled = true;
                    if (resultBars[0].bar.anchoredPosition.x <= -130.0f && resultBars[1].bar.anchoredPosition.x <= -130.0f && resultBars[2].bar.anchoredPosition.x <= -130.0f)
                    {
                        finalBar.bar.anchoredPosition += Vector2.right * (-130.0f - finalBar.bar.anchoredPosition.x);
                        if (resultBars[0].bar.anchoredPosition.x <= -260.0f && resultBars[1].bar.anchoredPosition.x <= -260.0f && resultBars[2].bar.anchoredPosition.x <= -260.0f)
                        {
                            finalBar.bar.anchoredPosition += Vector2.right * (-260.0f - finalBar.bar.anchoredPosition.x);
                        }
                    }
                }
            }
            state = 0;
        }

	}

    public void Open()
    {
        enabled = true;
        canvas.enabled = true;
        map.mapCanvas.enabled = false;
    }

    public void Close()
    {
        map.mapCanvas.enabled = true;
        canvas.enabled = false;
        enabled = false;
    }

}
