using UnityEngine;
using System.Collections;

public class HookController : MonoBehaviour {

    public LineRenderer rope;
    public GameObject anchor;

    public GameObject targetObject;

    private bool hooked = false;
    private Vector3[] ropePositions = new Vector3[16];
    private Vector3 velocity = Vector3.zero;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        int i;
        float f;
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
            transform.position += Vector3.up * (anchor.transform.position.y + (12.0f - Mathf.Abs(distance - 12.0f)) / 12.0f * 2.0f - transform.position.y);
            if (distance < 1.0f || (targetObject.transform.position - anchor.transform.position).magnitude < (transform.position - anchor.transform.position).magnitude)
            {
                targetObject.transform.parent = transform;
                hooked = true;
            }
        }
        for (i = 0; i < ropePositions.Length; i++)
        {
            f = (float)i / 6.0f;
            ropePositions[i] = transform.position * (1.0f - f) + anchor.transform.position * f + Vector3.right * Mathf.Sin(f * Mathf.PI + Time.time * 15.0f) * (1.0f - Mathf.Abs(f - 0.5f) * 2.0f) * (12.0f - Mathf.Abs(distance - 12.0f)) / 12.0f * 2.0f;
        }
        rope.SetPositions(ropePositions);
        rope.material.SetTextureScale("_MainTex", new Vector2(((transform.position - anchor.transform.position).magnitude - 0.5f) * 5.0f, 1.0f));

    }

    public void Throw (GameObject target)
    {
        if(enabled)
        {
            return;
        }
        Vector3 direction;
        targetObject = target;
        transform.position = anchor.transform.position;
        direction = targetObject.transform.position - transform.position;
        velocity = direction.normalized * 50.0f;
        hooked = false;
        enabled = true;
    }

}
