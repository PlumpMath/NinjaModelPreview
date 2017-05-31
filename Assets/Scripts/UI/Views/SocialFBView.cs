using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using Facebook;
using Facebook.Unity;
using Facebook.Unity.Mobile.Android;
using Facebook.Unity.Mobile.IOS;

public class SocialFBView : MonoBehaviour {

    public Canvas canvas;
    public Button closeButton;
    public Button loginButton;
    public Text statusLabel;
    public MapController map;

    public List<string> permissions;

    public string userId = "";

    // Use this for initialization
    void Start () {

        permissions = new List<string>();
        permissions.Add("public_profile");
        permissions.Add("user_friends");

        closeButton.onClick.AddListener(delegate() {
            Close();
        });

        loginButton.onClick.AddListener(delegate () {
            FB.Init("1683948398576587", null, true, true, true, false, true, null, "en_US", null, delegate () {
                FB.LogInWithReadPermissions(permissions, delegate (ILoginResult result) {
                    if (result.Error != null)
                    {
                        statusLabel.text = result.Error;
                    }
                    else
                    {
                        userId = result.ResultDictionary["user_id"].ToString();
                        Debug.Log("UserID: " + result.ResultDictionary["user_id"]);
                        statusLabel.text = result.AccessToken.Permissions.ToCommaSeparateList();
                        FB.API("/me", HttpMethod.POST, delegate (IGraphResult gResult)
                        {
                            Debug.Log(gResult.RawResult);
                            map.changeNicknameView.Open();
                            map.changeNicknameView.inputField.text = ((string)gResult.ResultDictionary["name"]).Trim();

                            int friendsActive = ((List<object>)((Dictionary<string, object>)gResult.ResultDictionary["friends"])["data"]).Count;
                            statusLabel.text = "Friends " + friendsActive + " of " + ((Dictionary<string, object>)((Dictionary<string, object>)gResult.ResultDictionary["friends"])["summary"])["total_count"].ToString();

                        }, (new Dictionary<string, string>() { { "fields", "id,name,friends.limit(5).fields(first_name,id)" } }));
                    }
                });
            });
        });

        Close();

	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Open()
    {
        enabled = true;
        canvas.enabled = true;
    }

    public void Close()
    {
        enabled = false;
        canvas.enabled = false;
    }

}
