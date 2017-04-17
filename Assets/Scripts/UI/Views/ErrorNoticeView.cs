using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ErrorNoticeView : MonoBehaviour {

    public Canvas canvas;
    public Text title;
    public Text description;
    public Button closeButton;

	// Use this for initialization
	void Start () {

        closeButton.onClick.AddListener(delegate() {
            Close();
        });

        canvas.enabled = false;

	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Open(string titleText, string descriptionText)
    {
        title.text = titleText;
        description.text = descriptionText;
        canvas.enabled = true;
    }

    public void Close()
    {
        canvas.enabled = false;
    }

}
