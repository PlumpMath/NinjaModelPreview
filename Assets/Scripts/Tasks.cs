using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class Tasks : MonoBehaviour {

    public EventHandler<EventArgs> OnThrow;

    public Text uiFPSLabel;

    public Text uiProgressLabel;
    public GameObject dummy;

    private float[] uiPositions = new float[3];
    private int task = 0;
    private int hits = 0;

	// Use this for initialization
	void Start () {

        uiPositions[0] = -35.0f;
        uiPositions[1] = -9.0f;
        uiPositions[2] = -86.0f;

        uiProgressLabel.text = hits + " / 3";
        uiProgressLabel.rectTransform.localPosition = new Vector3(uiProgressLabel.rectTransform.localPosition.x, uiPositions[task], 0.0f);

    }

    // Update is called once per frame
    void Update () {


        uiFPSLabel.text = Mathf.Round(1.0f / Time.deltaTime)  + " FPS";


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
            dummy.transform.position += Vector3.right * (Mathf.Round(UnityEngine.Random.Range(-1.4f, 1.4f)) * 5.0f - dummy.transform.position.x);
        }
        uiProgressLabel.text = hits + " / 3";
        uiProgressLabel.rectTransform.localPosition = new Vector3(uiProgressLabel.rectTransform.localPosition.x, uiPositions[task], 0.0f);
    }

}
