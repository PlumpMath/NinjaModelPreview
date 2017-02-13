using UnityEngine;
using System.Collections;

public class RegionColliderController : MonoBehaviour {

	void Start () {

        RegionPreset region = GameObject.FindObjectOfType<RegionPreset>();
        if(region != null)
        {
            region.RegisterCollider(this);
            Destroy(this.gameObject);
        }

	}
	
	void Update () {
	
	}
}
