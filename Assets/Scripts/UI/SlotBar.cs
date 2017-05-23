using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SlotBar : MonoBehaviour {

    public RectTransform bar;
    public Image highlight;

    public bool rolling = false;

    private float randomShift = 0.0f;

	// Use this for initialization
	void Start () {
        highlight.enabled = false;
    }
	
	// Update is called once per frame
	void Update () {
	
        if(rolling)
        {
            randomShift = Mathf.Max(-500.0f, Mathf.Min(500.0f, randomShift + Random.Range(-500.0f, 500.0f) * Time.deltaTime));

            bar.anchoredPosition += Vector2.up * (-750.0f + randomShift) * Time.deltaTime;
            if(bar.anchoredPosition.y < 0.0f)
            {
                bar.anchoredPosition += Vector2.up * 512.0f;
            }
        }

	}
}
