using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MapController : MonoBehaviour {

    public Button settingsButton;
    public Button playerObject;
    public Button[] mapObjects = new Button[0];

	// Use this for initialization
	void Start () {

        int i;
        int playerCheckpoint = PlayerPrefs.GetInt("MapPlayerCheckpoint", -1);

        PlayerPrefs.SetInt("MapObjectState_01_1", 1);

        settingsButton.onClick.AddListener(delegate () {
            PlayerPrefs.SetInt("MapObjectState_01_1", 1);
            for (i = 1; i < mapObjects.Length; i++)
            {
                if (i < 4)
                {
                    PlayerPrefs.SetInt("MapObjectState_01_" + (i + 1), 0);
                }
                else
                {
                    PlayerPrefs.SetInt("MapObjectState_02_" + (i - 3), 0);
                }
            }
            for (i = 0; i < mapObjects.Length; i++)
            {
                string[] strArr = mapObjects[i].name.Split('_');
                if (PlayerPrefs.GetInt("MapObjectState_" + strArr[1] + "_" + strArr[2], 0) == 1)
                {
                    mapObjects[i].enabled = true;
                    mapObjects[i].image.enabled = true;
                    if (playerCheckpoint == i)
                    {
                        playerObject.image.rectTransform.anchoredPosition = mapObjects[i].image.rectTransform.anchoredPosition + new Vector2(20.0f, 0.0f);
                    }
                }
                else
                {
                    mapObjects[i].enabled = false;
                    mapObjects[i].image.enabled = false;
                }
            }
        });

        for (i = 0; i < mapObjects.Length; i++)
        {
            string[] strArr = mapObjects[i].name.Split('_');
            if(PlayerPrefs.GetInt("MapObjectState_" + strArr[1] + "_" + strArr[2], 0) == 1)
            {
                mapObjects[i].enabled = true;
                mapObjects[i].image.enabled = true;
                if (playerCheckpoint == i)
                {
                    playerObject.image.rectTransform.anchoredPosition = mapObjects[i].image.rectTransform.anchoredPosition + new Vector2(20.0f, 0.0f);
                }
            }
            else
            {
                mapObjects[i].enabled = false;
                mapObjects[i].image.enabled = false;
            }
        }

        mapObjects[0].onClick.AddListener(delegate () {
            PlayerPrefs.SetString("CurrentRegion", "01");
            PlayerPrefs.SetString("CurrentPoint", "1");
            SceneManager.LoadScene("region");
        });
        mapObjects[1].onClick.AddListener(delegate () {
            PlayerPrefs.SetString("CurrentRegion", "01");
            PlayerPrefs.SetString("CurrentPoint", "2");
            SceneManager.LoadScene("region");
        });
        mapObjects[2].onClick.AddListener(delegate () {
            PlayerPrefs.SetString("CurrentRegion", "01");
            PlayerPrefs.SetString("CurrentPoint", "3");
            SceneManager.LoadScene("region");
        });
        mapObjects[3].onClick.AddListener(delegate () {
            PlayerPrefs.SetString("CurrentRegion", "01");
            PlayerPrefs.SetString("CurrentPoint", "4");
            SceneManager.LoadScene("region");
        });
        mapObjects[4].onClick.AddListener(delegate () {
            PlayerPrefs.SetString("CurrentRegion", "02");
            PlayerPrefs.SetString("CurrentPoint", "1");
            SceneManager.LoadScene("region");
        });
        mapObjects[5].onClick.AddListener(delegate () {
            PlayerPrefs.SetString("CurrentRegion", "02");
            PlayerPrefs.SetString("CurrentPoint", "2");
            SceneManager.LoadScene("region");
        });
        mapObjects[6].onClick.AddListener(delegate () {
            PlayerPrefs.SetString("CurrentRegion", "02");
            PlayerPrefs.SetString("CurrentPoint", "3");
            SceneManager.LoadScene("region");
        });
        mapObjects[7].onClick.AddListener(delegate () {
            PlayerPrefs.SetString("CurrentRegion", "02");
            PlayerPrefs.SetString("CurrentPoint", "4");
            SceneManager.LoadScene("region");
        });

        playerObject.onClick.AddListener(delegate() {
            PlayerPrefs.SetString("CurrentPoint", "1");
            SceneManager.LoadScene("region");
        });
        
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
