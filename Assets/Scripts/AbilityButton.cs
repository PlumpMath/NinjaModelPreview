using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class AbilityButton : MonoBehaviour {

    public Button button;
    public Image icon;

    public float progress = 0.0f;

    public EventHandler OnActivate;

	// Use this for initialization
	void Start () {

	}

    // Update is called once per frame
    void Update()
    {

        int i;

        if (progress < 0.0f)
        {
            progress += Time.deltaTime * 5.0f;
            if(progress > 0.0f)
            {
                progress = 0.0f;
                button.image.color = Color.white;
            }
            if(progress > -25.0f && progress - Time.deltaTime * 5.0f <= -25.0f)
            {
                button.image.color = Color.red;
            }
            button.image.fillAmount = Mathf.Max(0.0f, Mathf.Min(1.0f, -progress * 0.02f));
        }
        if (progress > 0.0f)
        {
            progress -= Time.deltaTime * 0.25f;
            if (progress < 0.0f)
            {
                progress = 0.0f;
            }
            button.image.fillAmount = Mathf.Max(0.0f, Mathf.Min(1.0f, progress));
        }

#if UNITY_EDITOR
        if (Input.GetMouseButton(0))
        {
            if (button.image.rectTransform.rect.Contains(Input.mousePosition - button.image.rectTransform.position))
            {
                Press();
            }
        }
#else
        for (i = 0; i < Input.touchCount; i++)
        {
            if(button.image.rectTransform.rect.Contains(Input.touches[i].position - new Vector2(button.image.rectTransform.position.x, button.image.rectTransform.position.y)))
            {
                Press();
            }
        }
#endif

    }

    public void Press()
    {
        progress += Time.deltaTime * 1.75f;
        if(progress > 1.0f)
        {
            progress = -50.0f;
            button.image.color = Color.green;
            button.image.fillAmount = Mathf.Max(0.0f, Mathf.Min(1.0f, -progress * 0.02f));
            Activate();
        }
    }

    public void Activate() {
        if(OnActivate != null)
        {
            EventArgs e = new EventArgs();
            OnActivate(this, e);
        }
    }

}
