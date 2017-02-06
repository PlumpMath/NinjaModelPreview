using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CanvasFloatingNotifyController : MonoBehaviour {

    public Text text;
    public Text textBack;

    private Color baseColor;
    private float cooldown = 0.0f;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        float f;
        if (cooldown > 0.0f)
        {
            cooldown -= Time.deltaTime;
            f = cooldown / 2.0f;
            text.color = new Color(baseColor.r, baseColor.g, baseColor.b, f);
            textBack.color = new Color(baseColor.r * 0.5f, baseColor.g * 0.5f, baseColor.b * 0.5f, f);
            text.rectTransform.anchoredPosition += Vector2.up * 5.0f * Time.deltaTime;
            if (cooldown <= 0.0f)
            {
                Destroy(gameObject);
            }
        }
    }

    public void Show(Color color, string message)
    {
        baseColor = color;
        text.text = message;
        text.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        textBack.text = message;
        textBack.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        cooldown = 2.0f;
    }

    public void Show(string message, int color)
    {
        Color _color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        switch (color)
        {
            case 0:
                _color = new Color(0.2f, 0.9f, 0.2f, 1.0f);
                break;
            case 1:
                _color = new Color(1.0f, 0.2f, 0.2f, 1.0f);
                break;
            case 2:
                _color = new Color(0.0f, 0.7f, 0.5f, 1.0f);
                break;
            case 3:
                _color = new Color(0.2f, 0.2f, 0.9f, 1.0f);
                break;
        }
        Show(_color, message);
    }

}
