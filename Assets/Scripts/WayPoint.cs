using UnityEngine;
using System.Collections;

public class WayPoint : MonoBehaviour {

    public enum WaypointType {
        OPEN,
        COVER_FULL,
        COVER_DOWN,
        NEAR_COVER_LEFT,
        NEAR_COVER_RIGHT
    };

    public WaypointType waypointType;

    public WayPoint leftNode;
    public WayPoint rightNode;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
