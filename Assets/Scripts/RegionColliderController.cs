using UnityEngine;
using System.Collections;

public class RegionColliderController : MonoBehaviour {

	void Start () {

        if (gameObject.tag == "RegionObstacle")
        {
            Destroy(GetComponent<MeshRenderer>());
            Destroy(GetComponent<MeshFilter>());
            SphereCollider collider = gameObject.AddComponent<SphereCollider>();
            collider.radius = transform.lossyScale.x / 2.0f;
        }
        else
        {
            RegionPreset region = GameObject.FindObjectOfType<RegionPreset>();
            if (region != null)
            {
                region.RegisterCollider(this);
                Destroy(this.gameObject);
            }
        }

	}
	
	void Update () {
	
	}
}
