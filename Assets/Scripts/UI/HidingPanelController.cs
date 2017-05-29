using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HidingPanelController : MonoBehaviour {

    public RectTransform rectTransform;
    public Button selfButton;

    public float hiddenAnchoredX = 0.0f;
    public float hiddenAnchoredY = 0.0f;

    private float baseAnchoredX = 0.0f;
    private float baseAnchoredY = 0.0f;
    private float showTimeout = 0.0f;

    // Use this for initialization
    void Start () {

        baseAnchoredX = rectTransform.anchoredPosition.x;
        baseAnchoredY = rectTransform.anchoredPosition.y;

        if (selfButton != null)
        {
            selfButton.onClick.AddListener(delegate() {
                Show();
            });
        }

	}
	
	// Update is called once per frame
	void Update () {

        float f;
        if(showTimeout > 0.0f)
        {
            showTimeout -= Time.deltaTime;
            f = Mathf.Max(0.0f, Mathf.Min(1.0f, showTimeout));
            rectTransform.anchoredPosition = (new Vector2(baseAnchoredX, baseAnchoredY)) * f + (new Vector2(hiddenAnchoredX, hiddenAnchoredY)) * (1.0f - f);
        }

	}

    public void Show()
    {
        Show(5.0f);
    }

    public void Show(float timeout)
    {
        rectTransform.anchoredPosition = new Vector2(baseAnchoredX, baseAnchoredY);
        showTimeout = timeout;
    }

    public void Hide()
    {
        float f;
        showTimeout = 0.0f;
        f = showTimeout;
        rectTransform.anchoredPosition = (new Vector2(baseAnchoredX, baseAnchoredY)) * f + (new Vector2(hiddenAnchoredX, hiddenAnchoredY)) * (1.0f - f);
    }

}
