using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenEffectsController : MonoBehaviour {

    public Image image;

    private float redFlashCooldown = 0.0f;
    private float greenFlashCooldown = 0.0f;

    // Use this for initialization
    void Start () {
	
	}

    // Update is called once per frame
    void Update()
    {

        float f;
        if (redFlashCooldown > 0.0f)
        {
            redFlashCooldown -= Time.deltaTime;
            f = 0.5f * (1.0f - Mathf.Abs(redFlashCooldown * 4.0f - 0.5f));
            image.color = new Color(0.95f, 0.4f, 0.4f, f);
            if (redFlashCooldown <= 0.0f)
            {
                image.color = new Color(0.95f, 0.4f, 0.4f, 0.0f);
                redFlashCooldown = 0.0f;
            }
        }
        else if(greenFlashCooldown > 0.0f)
        {
            greenFlashCooldown -= Time.deltaTime;
            f = 0.5f * (1.0f - Mathf.Abs(greenFlashCooldown * 4.0f - 0.5f));
            image.color = new Color(0.3f, 1.0f, 0.3f, f);
            if (greenFlashCooldown <= 0.0f)
            {
                image.color = new Color(0.3f, 1.0f, 0.3f, 0.0f);
                greenFlashCooldown = 0.0f;
            }
        }

    }

    public void RedFlash()
    {
        redFlashCooldown = 0.25f;
    }

    public void GreenFlash()
    {
        greenFlashCooldown = 0.25f;
    }

}
