﻿using UnityEngine;
using System.Collections;

public class HookController : MonoBehaviour {

    public LineRenderer rope;
    public GameObject anchor;

    public GameObject targetObject;

    private bool hooked = false;
    private Vector3[] ropePositions = new Vector3[2];
    private Vector3 velocity = Vector3.zero;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        float distance;
        if (hooked)
        {
            transform.position += (anchor.transform.position - transform.position) * Time.deltaTime * 5.0f;
            distance = (anchor.transform.position - transform.position).magnitude;
            if (distance < 1.0f)
            {
                targetObject.transform.parent = null;
                Destroy(targetObject);
                enabled = false;
            }
        }
        else
        {
            transform.position += velocity * Time.deltaTime;
            distance = (targetObject.transform.position - transform.position).magnitude;
            if (distance < 1.0f || (targetObject.transform.position - anchor.transform.position).magnitude < (transform.position - anchor.transform.position).magnitude)
            {
                targetObject.transform.parent = transform;
                hooked = true;
            }
        }
        ropePositions[0] = transform.position;
        ropePositions[1] = anchor.transform.position;
        rope.SetPositions(ropePositions);

	}

    public void Throw (GameObject target)
    {
        Vector3 direction;
        targetObject = target;
        transform.position = anchor.transform.position;
        direction = targetObject.transform.position - transform.position;
        velocity = direction.normalized * 50.0f;
        hooked = false;
        enabled = true;
    }

}