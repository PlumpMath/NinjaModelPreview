using UnityEngine;
using System.Collections;

public class RegionTimedIcon : MonoBehaviour {

    public SpriteRenderer sprite;
    public float cooldown = 1.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        cooldown -= Time.deltaTime;
        if(cooldown <= 0.0f)
        {
            Destroy(gameObject);
        }
	
	}
}
