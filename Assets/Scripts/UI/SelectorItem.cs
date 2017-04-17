using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SelectorItem : MonoBehaviour {

    public RectTransform rectTransform;

    public int position = -1;
    public string value = "";

    void Start () {
	
	}
	
	void Update () {
	
	}

    public virtual void Setup(Transform presentContainer, float presentShift, RectTransform descriptionContainer, float descriptionShift)
    {
        rectTransform.parent = descriptionContainer;
        rectTransform.anchoredPosition = Vector2.right * descriptionShift;
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, 0.0f);
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, 0.0f);
        rectTransform.localScale = Vector3.one;
    }

    public virtual void Destroy()
    {
        GameObject.Destroy(gameObject);
    }

}
