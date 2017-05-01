using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class ChangeNicknameView : MonoBehaviour {

    public Canvas canvas;
    public Button closeButton;
    public Button confirmButton;
    public InputField inputField;

    public MapController map;

    // Use this for initialization
    void Start () {

        Close();

        closeButton.onClick.AddListener(delegate() {
            Close();
        });

        confirmButton.onClick.AddListener(delegate() {
            int index;
            byte[] data;
            byte[] nicknameBytes;
            BaseObjectMessage message;
            string newNickname = inputField.text;
            if(newNickname.Length < 3)
            {
                inputField.image.color = Color.red;
                return;
            }
            else if(newNickname.Length > 24)
            {
                inputField.image.color = Color.red;
                return;
            }
            message = new BaseObjectMessage();
            nicknameBytes = Encoding.UTF8.GetBytes(newNickname);
            data = new byte[nicknameBytes.Length + 4];
            index = 0;
            message.PutShort(data, (short)1002, ref index);
            message.PutSString(data, newNickname, ref index);
            map.loginController.SendGameMessage(data);
            Close();
        });

        inputField.onValueChanged.AddListener(delegate (string value) {
            if (value.Length >= 3 && value.Length <= 24)
            {
                inputField.image.color = Color.white;
            }
        });

	}
	
	// Update is called once per frame
	void Update () {
	
	}


    public void Open()
    {
        enabled = true;
        canvas.enabled = true;
        inputField.text = "";

    }

    public void Close()
    {
        enabled = false;
        canvas.enabled = false;
    }

}
