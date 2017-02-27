using UnityEngine;
using System.Collections;

public class RegionHook : MonoBehaviour {

    public GameObject hook;
    public MeshRenderer hookMesh;
    public LineRenderer chain;
    public GameObject player;
    public float targetRank = 0.0f;

    public bool throwing = false;
    public bool rollback = false;
    public Vector3 velocity = new Vector3();

    public float destinationTimemark = 0.0f;
    public float throwTimemark = 0.0f;
    public float rollbackTimemark = 0.0f;
    public float cooldown = 0.0f;
    public float wrappingCooldown = 0.0f;

    private float startRollbackDistance = 0.0f;

    private Vector3[] points = new Vector3[60];

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        int i;
        float l;
        float f;
        float c = 0.0f;
        float a;
        Vector3 direction;
        Vector3 v3;

        direction = player.transform.position - hook.transform.position;

        if(destinationTimemark > 0.0f)
        {
            c = destinationTimemark / Mathf.Max(0.1f, throwTimemark);
            destinationTimemark -= Time.deltaTime;
            if (destinationTimemark <= 0.0f)
            {
                destinationTimemark = 0.0f;
                if (!rollback)
                {
                    //Rollback();
                }
            }
        }


        if (direction.magnitude < 0.5f && rollback)
        {
            Hide();
            return;
        }

        if(wrappingCooldown > 0.0f)
        {
            wrappingCooldown -= Time.deltaTime;
            if(wrappingCooldown < 0.0f)
            {
                wrappingCooldown = 0.0f;
            }
        }

        if(rollback)
        {
            rollbackTimemark -= Time.deltaTime;
            if(rollbackTimemark <= 0.0f)
            {
                rollbackTimemark = 0.0f;
            }
            direction.y = 0.0f;
            transform.position = transform.position + direction.normalized * (direction.magnitude - startRollbackDistance + startRollbackDistance * (1.0f - rollbackTimemark));
            velocity = Vector3.zero; // direction.normalized * velocity.magnitude;
            c = 1.0f - Mathf.Abs(rollbackTimemark - 0.5f) * 2.0f;
        }

        if (velocity.magnitude > 0.0f)
        {
            transform.position += velocity * Time.deltaTime;
        }

        l = 0.0f;

        for (i = 0; i < points.Length; i++)
        {
            f = (float)i / (float)points.Length;
            v3 = player.transform.position - hook.transform.position;// (-Vector3.up * 0.05f - velocity.normalized * 0.2f * direction.magnitude);
            v3.Normalize();
            if (!throwing)
            {
                if (rollback)
                {
                    v3 = new Vector3(v3.z, (Mathf.Abs(v3.x) + Mathf.Abs(v3.z)) * 0.1f, -v3.x) * Mathf.Sin(f * l * 2.0f + Time.time * 5.0f) * 0.5f;
                    points[i] = (hook.transform.position - Vector3.up * 0.05f + direction.normalized * 0.0f) * (1.0f - f) + (player.transform.position + Vector3.up * 0.2f - direction.normalized * 0.3f) * f + v3 * (1.0f - Mathf.Abs(f - 0.5f) * 2.0f) * c;
                }
                else
                {
                    if (i < points.Length / 2)
                    {
                        f = (float)i / (float)points.Length * 2.0f * (1.0f - wrappingCooldown);
                        v3 = new Vector3(Mathf.Sin(f * 8.0f) * 0.3f, f, Mathf.Cos(f * 8.0f) * 0.3f);
                        points[i] = hook.transform.position + v3;
                    }
                    else
                    {
                        f = (float)(i - points.Length / 2) / (float)(points.Length / 2);
                        v3 = new Vector3(v3.z, (Mathf.Abs(v3.x) + Mathf.Abs(v3.z)) * 0.1f, -v3.x) * Mathf.Sin(f * l * 2.0f + Time.time * 5.0f) * 0.5f;
                        points[i] = (hook.transform.position - Vector3.up * 0.05f + direction.normalized * 0.0f) * (1.0f - f) + (player.transform.position + Vector3.up * 0.2f - direction.normalized * 0.3f) * f + v3 * (1.0f - Mathf.Abs(f - 0.5f) * 2.0f) * c;
                    }
                }
            }
            else
            {
                v3 = new Vector3(Mathf.Sin(f * 4.0f + Time.time * 5.0f), Mathf.Sin(f * 3.0f + Time.time * 5.0f - 0.5f) + 1.0f, Mathf.Cos(f * 4.0f + Time.time * 5.0f)) * (0.2f + direction.magnitude * 0.5f);
                points[i] = (hook.transform.position - Vector3.up * 0.05f + direction.normalized * 0.0f) * (1.0f - f) + (player.transform.position + Vector3.up * 0.2f - direction.normalized * 0.3f) * f + v3 * (1.0f - Mathf.Pow(Mathf.Abs(f - 0.5f) * 2.0f, 4.0f)) * c;
            }
            if (i > 0)
            {
                l += (points[i] - points[i - 1]).magnitude;
            }
        }
        //points[0] = hook.transform.position - Vector3.up * 0.05f + direction.normalized * 0.2f;
        //points[1] = (hook.transform.position + player.transform.position - direction.normalized * (0.2f * direction.magnitude + 0.1f)) * 0.5f - Vector3.up * 0.05f - velocity.normalized * 0.2f * direction.magnitude;
        //points[2] = player.transform.position - Vector3.up * 0.05f - direction.normalized * 0.3f;
        chain.SetPositions(points);
        chain.material.SetTextureScale("_MainTex", new Vector2((l - 0.5f) * 5.0f, 1.0f));
        v3 = points[points.Length - 1] - points[points.Length - 2];
        v3.Normalize();
        a = Vector3.Angle(Vector3.forward, v3);
        if(Vector3.Angle(Vector3.forward, v3) < Vector3.Angle(Vector3.forward + Vector3.right * 0.01f, v3))
        {
            a *= -1.0f;
        }
        //a += Mathf.Sin(Time.time * 5.0f) * 20.0f * c;
        chain.transform.rotation = Quaternion.Euler(0.0f, a, 0.0f);

    }

    public void Hide()
    {
        enabled = false;
        hookMesh.enabled = false;
        hook.transform.position = new Vector3(hook.transform.position.x, -10.0f, hook.transform.position.z);
        chain.enabled = false;
        rollback = false;
    }

    public void Show(float throwTime)
    {
        hook.transform.position = new Vector3(hook.transform.position.x, 0.1f, hook.transform.position.z);
        destinationTimemark = throwTime;
        throwTimemark = destinationTimemark;
        Update();
        enabled = true;
        hookMesh.enabled = true;
        chain.enabled = true;
        throwing = true;
    }

    public void Move(float throwTime)
    {
        //hook.transform.position = new Vector3(hook.transform.position.x, 0.1f, hook.transform.position.z);
        destinationTimemark = throwTime;
        throwTimemark = destinationTimemark;
        Update();
        enabled = true;
        hookMesh.enabled = true;
        chain.enabled = true;
        throwing = false;
        wrappingCooldown = 1.0f;
    }

    public void Rollback()
    {
        Vector3 direction3D = player.transform.position - hook.transform.position;
        Vector2 direction = new Vector2(direction3D.x, direction3D.z);
        //velocity = direction.normalized * direction.magnitude / 1.5f;
        startRollbackDistance = direction.magnitude;
        rollbackTimemark = 1.0f;
        rollback = true;
        throwing = false;
    }

}
