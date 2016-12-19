using UnityEngine;
using System.Collections;

public class TaskTarget : MonoBehaviour {

    public TaskTarget next;
    public SpriteRenderer icon;

    public bool active = true;
    public int type = 0;
    public float activeRadius = 0.0f;
    public string unlockPoint = "";

    public float progress = 0.0f;

    private float iconCooldown = 0.0f;

	// Use this for initialization
	void Start () {

        icon.enabled = false;

	}
	
	// Update is called once per frame
	void Update () {

        if (iconCooldown > 0.0f)
        {
            iconCooldown -= Time.deltaTime;
            if (iconCooldown <= 0.0f)
            {
                iconCooldown = 0.0f;
                icon.enabled = false;
            }
        }

	}

    public void Process(RegionMoveController invoker)
    {
        if(!active)
        {
            return;
        }
        float f;
        Vector3 v3;
        v3 = invoker.transform.position - transform.position;
        v3.y = 0.0f;
        if (v3.magnitude < activeRadius)
        {
            f = Time.deltaTime * Random.Range(0.0f, 30.0f);
            progress += f;
            if(progress > Mathf.Round(progress / 20.0f) * 20.0f && progress - f <= Mathf.Round(progress / 20.0f) * 20.0f)
            {
                iconCooldown = 1.0f;
                icon.transform.position = transform.position + Vector3.right * Random.Range(-activeRadius * 0.5f, activeRadius * 0.5f) + Vector3.forward * Random.Range(-activeRadius * 0.5f, activeRadius * 0.5f);
                icon.enabled = true;
            }
            if(progress >= 100.0f && type == 0)
            {
                next.active = true;
                invoker.taskTarget = next;
                invoker.taskPointer.enabled = true;
                invoker.ShowDiscovered(2);
                active = false;
            }
            if(progress >= 0.0f && type == 1)
            {
                invoker.taskTarget = null;
                invoker.taskPointer.enabled = false;
                invoker.ShowDiscovered(1);
                active = false;
                if(unlockPoint != "")
                {
                    PlayerPrefs.SetInt("MapObjectState_" + PlayerPrefs.GetString("CurrentRegion") + "_" + unlockPoint, 1);
                    if (PlayerPrefs.GetString("CurrentRegion") == "01")
                    {
                        if (PlayerPrefs.GetInt("MapObjectState_01_2", 0) == 1 && PlayerPrefs.GetInt("MapObjectState_01_3", 0) == 1 && PlayerPrefs.GetInt("MapObjectState_01_4", 0) == 1)
                        {
                            PlayerPrefs.SetInt("MapObjectState_02_1", 1);
                        }
                    }
                }
            }
        }
        else
        {
            progress -= Time.deltaTime;
            if(progress < 0.0f)
            {
                progress = 0.0f;
            }
        }
    }

}
