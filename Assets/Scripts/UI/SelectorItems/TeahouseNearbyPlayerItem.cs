using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class TeahouseNearbyPlayerItem : MonoBehaviour {

    public Image maskIcon;
    public Text nicknameLabel;
    public Text distanceLabel;
    public Button duelButton;
    public RectTransform rectTransform;

    public NearbyPlayerNode data;
    public PlayerViewMessage viewData;

    public EventHandler<PlayerDataEventArgs> OnRequestDuel;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Setup()
    {
        rectTransform.localScale = new Vector2(1.0f, 1.0f);
        rectTransform.sizeDelta = new Vector2(0.0f, 60.0f);
        string distance = data.distance + " m.";
        if(data.distance > 500)
        {
            distance = (data.distance / 1000) + " km.";
        }
        nicknameLabel.text = viewData.nickname;
        distanceLabel.text = distance.ToString();
        duelButton.onClick.AddListener(delegate() {
            EventHandler<PlayerDataEventArgs> handler = OnRequestDuel;
            if(handler != null)
            {
                PlayerDataEventArgs e = new PlayerDataEventArgs();
                e.playerView = viewData;
                handler(this, e);
            }
        });
    }

}
