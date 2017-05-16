using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class TeahouseView : MonoBehaviour {

    public TeahouseNearbyPlayerItem nearbyPlayerItemPrefab;
    public MapController map;
    public Canvas canvas;
    public Button closeButton;
    public RectTransform listContainer;

    public LinkedList<TeahouseNearbyPlayerItem> nearbyPlayerList = new LinkedList<TeahouseNearbyPlayerItem>();

    /***/

    public void OnPlayerDataLoaded(object sender, PlayerDataEventArgs e)
    {
        //if(!canvas.enabled)
        //{
        //    return;
        //}
        Debug.Log("OnPlayerDataLoaded");
        LinkedListNode<TeahouseNearbyPlayerItem> node = nearbyPlayerList.First;
        while(node != null)
        {
            Debug.Log("OnPlayerDataLoaded check ( " + node.Value.data.playerId + " == " + e.playerView.playerId + " )");
            if (node.Value.data.playerId == (long)e.playerView.playerId)
            {
                node.Value.viewData = e.playerView;
                node.Value.Setup();
            }
            node = node.Next;
        }
    }

    public void OnRequestDuel(object sender, PlayerDataEventArgs e)
    {
        int index;
        BaseObjectMessage message = new BaseObjectMessage();
        byte[] buf = new byte[2 + 8];
        Buffer.BlockCopy(BitConverter.GetBytes((short)1103), 0, buf, 0, 2);
        index = 2;
        message.PutULong(buf, e.playerView.playerId, ref index);
        map.loginController.SendGameMessage(buf);
    }

    /***/


	// Use this for initialization
	void Start () {

        closeButton.onClick.AddListener(delegate() {
            Close();
        });

	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Open()
    {
        enabled = true;
        canvas.enabled = true;
        map.mapCanvas.enabled = false;
        map.OnPlayerViewLoaded += OnPlayerDataLoaded;
    }

    public void Close()
    {
        map.mapCanvas.enabled = true;
        canvas.enabled = false;
        enabled = false;
        map.OnPlayerViewLoaded -= OnPlayerDataLoaded;
    }

}
