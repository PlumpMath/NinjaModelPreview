using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DailyBonusView : MonoBehaviour {

    public Canvas canvas;
    public Image rewardImage;
    public Text rewardLabel;
    public Text rewardSubLabel;
    public RectTransform rewardListPanel;
    public Button closeButton;

    public float rewardListStep = 128.0f;

    public Sprite[] rewardImageList;
    public string[] rewardLabelList;
    public string[] rewardSubLabelList;

    public float currentReward = 0.0f;
    public float currentShift = 0.0f;

    // Use this for initialization
    void Start () {

        closeButton.onClick.AddListener(delegate() {
            Close();
        });

        //Open(4);

	}
	
	// Update is called once per frame
	void Update () {
	
        if(currentShift < rewardListStep * currentReward)
        {
            Debug.Log("currentShift: " + currentShift);
            currentShift += Mathf.Min(0.02f, Time.deltaTime) * (rewardListStep * rewardListStep * currentReward / (currentShift + 3.0f));
            Debug.Log("newShift: " + currentShift + " ; dt: " + Mathf.Min(0.02f, Time.deltaTime) + " ; d: " + ((rewardListStep * rewardListStep * currentReward / (currentShift + 3.0f))));
            if (currentShift > rewardListStep * currentReward)
            {
                currentShift = rewardListStep * currentReward;
            }
            rewardListPanel.anchoredPosition = new Vector2(-currentShift, 0.0f);
        }

	}

    public void Open (int day)
    {
        Open();
        currentReward = (float)(day - 1);
        if (day > -1 && day < rewardImageList.Length)
        {
            rewardImage.sprite = rewardImageList[day];
            rewardLabel.text = rewardLabelList[day];
            rewardSubLabel.text = rewardSubLabelList[day];
        }
    }

    public void Open()
    {
        enabled = true;
        canvas.enabled = true;
        currentReward = 0.0f;
        currentShift = 0.0f;
    }

    public void Close ()
    {
        canvas.enabled = false;
        enabled = false;
    }

}
