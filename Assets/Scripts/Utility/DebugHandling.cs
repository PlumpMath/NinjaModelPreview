using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DebugHandling : MonoBehaviour {

    public Canvas canvas;
    public Button button;
    public Image panel;
    public Text log;

    public static bool created = false;
    private bool initialized = false;

    // Use this for initialization
    void Start () {

        Debug.Log("Debug Handling Enabled");

        panel.enabled = false;
        log.enabled = false;

        button.onClick.AddListener(delegate() {
            if(log.enabled)
            {
                panel.enabled = false;
                log.enabled = false;
            }
            else
            {
                panel.enabled = true;
                log.enabled = true;
            }
        });

	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void Awake()
    {
        if (!created)
        {
            DontDestroyOnLoad(this.gameObject);
            created = true;
            initialized = true;
        }

        else
        {
            if (!initialized)
            {
                Destroy(this.gameObject);
            }
        }
    }

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        log.text += "\r\n" + type.ToString() + ": " + logString;
    }

}
