using UnityEngine;
using System.Collections;

public class RegionHook : MonoBehaviour {

    public MeshRenderer hook;
    public LineRenderer chain;
    public GameObject player;
    public float targetRank = 0.0f;

    public bool rollback = false;
    public Vector3 velocity = new Vector3();

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        float a;
        Vector3 direction;
        Vector3[] points = new Vector3[2];

        direction = player.transform.position - hook.transform.position;

        if (direction.magnitude > 3.0f && !rollback)
        {
            Rollback();
        }

        if (direction.magnitude < 0.5f && rollback)
        {
            Hide();
            return;
        }

        if(rollback)
        {
            velocity = direction.normalized * 6.0f;
        }

        transform.position += velocity * Time.deltaTime;

        points[0] = hook.transform.position - Vector3.up * 0.1f + direction.normalized * 0.2f;
        points[1] = player.transform.position - Vector3.up * 0.1f - direction.normalized * 0.3f;
        chain.SetPositions(points);
        chain.material.SetTextureScale("_MainTex", new Vector2((direction.magnitude - 0.5f) * 5.0f, 1.0f));
        a = Vector3.Angle(Vector3.forward, direction);
        if(Vector3.Angle(Vector3.forward, direction) < Vector3.Angle(Vector3.forward + Vector3.right * 0.01f, direction))
        {
            a *= -1.0f;
        }
        chain.transform.rotation = Quaternion.Euler(0.0f, a, 0.0f);

    }

    public void Hide()
    {
        enabled = false;
        hook.enabled = false;
        hook.transform.position = new Vector3(hook.transform.position.x, 10.0f, hook.transform.position.z);
        chain.enabled = false;
        rollback = false;
    }

    public void Show()
    {
        hook.transform.position = new Vector3(hook.transform.position.x, 0.0f, hook.transform.position.z);
        Update();
        enabled = true;
        hook.enabled = true;
        chain.enabled = true;
    }

    public void Rollback()
    {
        rollback = true;
    }

}
