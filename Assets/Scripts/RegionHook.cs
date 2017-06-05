using UnityEngine;
using System.Collections;

public class RegionHook : MonoBehaviour {

    public GameObject hook;
    public MeshRenderer hookMesh;
    public MeshRenderer hookInHandMesh;
    public LineRenderer chain;
    public RegionBodyBehavior player;
    public Transform hookHandBone;
    public float targetRank = 0.0f;

    public bool throwing = false;
    public bool rollback = false;
    public Vector3 velocity = new Vector3();

    public float throwingTime = 0.0f;
    public float destinationTimemark = 0.0f;
    public float throwTimemark = 0.0f;
    public float rollbackTimemark = 0.0f;
    public float cooldown = 0.0f;
    public float wrappingCooldown = 0.0f;

    private RegionBodyController body = null;
    private float startRollbackDistance = 0.0f;

    private Vector3[] points = new Vector3[60];

    private RegionBodyBehavior delayedClingTarget = null;
    private float delayedClingTimeout = 0.0f;
    private float delayedClingTime = 0.0f;

    public float ropeScale = 12.0f;

    private Vector3 lastVelocity = Vector3.zero;

    // Use this for initialization
    void Start () {

        RegionBodyBehavior target = player.gameObject.GetComponent<RegionBodyBehavior>();
        if (target != null)
        {
            body = target.body;
        }

    }
	
	// Update is called once per frame
	void Update () {

        int i;
        float l;
        float f;
        float f2;
        float f3;
        float c = 0.0f;
        float a;
        Vector3 direction;
        Vector3 v3;

        if(player == null)
        {
            Destroy(gameObject);
            return;
        }

        direction = hookHandBone.position - hook.transform.position;

        throwingTime += Time.deltaTime;

        if (throwingTime > 0.311f && !hookMesh.enabled)
        {
            transform.position = hookHandBone.position;
            hook.transform.position = hookHandBone.transform.position;
            hookMesh.enabled = true;
            hookInHandMesh.enabled = false;
            chain.enabled = true;
        }

        if (destinationTimemark > 0.0f)
        {
            c = Mathf.Min(1.0f, destinationTimemark / Mathf.Max(0.1f, throwTimemark));
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


        if ((direction.magnitude < 0.2f || rollbackTimemark <= 1.0f) && rollback)
        {
            Hide();
            return;
        }

        if(delayedClingTimeout > 0.0f)
        {
            delayedClingTimeout -= Time.deltaTime;
            if(delayedClingTimeout <= 0.0f)
            {
                delayedClingTime += delayedClingTimeout;
                RegionBodyController targetBody = null;
                RegionHook targetHook = null;
                if (delayedClingTarget != null)
                {
                    targetBody = delayedClingTarget.body;
                    targetHook = delayedClingTarget.hook;
                    //delayedClingTarget.blockInput = delayedClingTime;
                    delayedClingTarget.pullingTime = delayedClingTime;
                    if (targetBody != null)
                    {
                        Debug.Log("HOOK VICTIM: " + targetBody.name + " ; " + targetBody.transform.parent.parent.gameObject.name);
                        transform.parent = targetBody.locomotionBones[4].transform;
                        transform.localPosition = new Vector3(-0.09f, 0.25f, 0.35f);
                        transform.localRotation = Quaternion.Euler(-30.0f, 0.0f, 90.0f);
                        if (targetHook != null)
                        {
                            if (targetHook.hookInHandMesh.enabled)
                            {
                                targetHook.hookInHandMesh.enabled = false;
                            }
                        }
                    }
                    player.pullingTime = delayedClingTime;
                }
            }
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
            transform.position = transform.position + direction.normalized * (direction.magnitude - startRollbackDistance + startRollbackDistance * Mathf.Max(0.0f, Mathf.Min(1.0f, 2.2f - rollbackTimemark)));
            velocity = Vector3.zero; // direction.normalized * velocity.magnitude;
            c = 1.0f - Mathf.Abs(rollbackTimemark - 1.5f) * 2.0f;
            if (rollbackTimemark < 1.35f && hook.transform.position.y < hookHandBone.transform.position.y)
            {
                hook.transform.position += Vector3.up * Time.deltaTime * (hookHandBone.transform.position.y - hook.transform.position.y) * 20.0f;
            }
            else if (hook.transform.position.y > 0.1f)
            {
                hook.transform.position -= Vector3.up * Time.deltaTime * 3.0f;
                if(hook.transform.position.y < 0.1f)
                {
                    hook.transform.position += Vector3.up * (0.1f - hook.transform.position.y);
                }
            }
        }

        if (velocity.magnitude > 0.0f && throwingTime > 0.311f && transform.parent == null && destinationTimemark > 0.0f)
        {
            transform.position += velocity * Time.deltaTime;
        }

        l = 0.0f;

        if (transform.parent != null)
        {
            c = 0.0f;
        }

        for (i = 0; i < points.Length; i++)
        {
            f = (float)i / (float)points.Length;
            v3 = hookHandBone.transform.position - hook.transform.position;// (-Vector3.up * 0.05f - velocity.normalized * 0.2f * direction.magnitude);
            v3.Normalize();
            if (!throwing)
            {
                if (rollback)
                {
                    v3 = new Vector3(v3.z, (Mathf.Abs(v3.x) + Mathf.Abs(v3.z)) * 0.1f, -v3.x) * Mathf.Sin(f * l * 2.0f + Time.time * 15.0f) * 0.5f;
                    v3 = v3 * SmoothNormalizedRange(f) * c;
                    points[i] = (hook.transform.position + direction.normalized * 0.0f) * (1.0f - f) + (hookHandBone.transform.position + Vector3.up * 0.0f - direction.normalized * 0.0f) * f + v3;
                }
                else
                {
                    if (i < 0) //(i < points.Length / 2)
                    {
                        f = (float)i / (float)points.Length * 2.0f * (1.0f - wrappingCooldown);
                        v3 = new Vector3(Mathf.Sin(f * 12.0f) * 0.4f, f, Mathf.Cos(f * 12.0f) * 0.4f);
                        f2 = Mathf.Min(1.0f, (1.0f - f) * 8.0f);
                        f3 = 0.0f;
                        v3 = v3 * SmoothNormalizedRange(f3) * c;
                        points[i] = (hook.transform.position + v3) * f2 + ((hook.transform.position + direction.normalized * 0.0f) * (1.0f - f3) + (hookHandBone.transform.position + Vector3.up * 0.0f - direction.normalized * 0.0f) * f3 + v3) * (1.0f - f2);
                    }
                    else
                    {
                        f = (float)i / (float)(points.Length); //(float)(i - points.Length / 2) / (float)(points.Length / 2);
                        v3 = new Vector3(v3.z, (Mathf.Abs(v3.x) + Mathf.Abs(v3.z)) * 0.1f, -v3.x) * Mathf.Sin(f * l * 2.0f + Time.time * 5.0f) * 0.5f;
                        v3 = v3 * SmoothNormalizedRange(f) * c;
                        points[i] = (hook.transform.position + direction.normalized * 0.0f) * (1.0f - f) + (hookHandBone.transform.position + Vector3.up * 0.0f - direction.normalized * 0.0f) * f + v3;
                    }
                }
            }
            else
            {
                v3 = new Vector3(Mathf.Sin(f * 3.0f + Time.time * 5.0f), Mathf.Sin(f * 2.0f + Time.time * 5.0f - 0.5f) + 1.0f, Mathf.Cos(f * 3.0f + Time.time * 5.0f)) * (0.2f + direction.magnitude * 0.5f);
                v3 = v3 * SmoothNormalizedRange(f) * c;
                points[i] = (hook.transform.position + direction.normalized * 0.0f) * (1.0f - f) + (hookHandBone.transform.position + Vector3.up * 0.0f - direction.normalized * 0.0f) * f + v3;
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
        chain.material.SetTextureScale("_MainTex", new Vector2((l - 0.5f) * ropeScale, 1.0f));
        chain.material.SetTextureScale("_BumpMap", new Vector2((l - 0.5f) * ropeScale, 1.0f));
        v3 = points[points.Length - 1] - points[points.Length - 2];
        v3.Normalize();
        a = Vector3.Angle(Vector3.forward, v3);
        if(Vector3.Angle(Vector3.forward, v3) < Vector3.Angle(Vector3.forward + Vector3.right * 0.01f, v3))
        {
            a *= -1.0f;
        }
        //a += Mathf.Sin(Time.time * 5.0f) * 20.0f * c;
        if (transform.parent == null)
        {
            chain.transform.rotation = Quaternion.Euler(0.0f, a, 0.0f);
        }

    }

    private float SmoothNormalizedRange(float value)
    {
        float ramp = Mathf.Abs(Mathf.Min(1.0f, Mathf.Max(0.0f, value)) - 0.5f) * 2.0f;
        float quadraticRamp = Mathf.Pow(ramp, 2.0f);
        float quadraticConvex = 1.0f - quadraticRamp;
        return Mathf.Pow(quadraticConvex, 2.0f);
    }

    public void Hide()
    {
        DelayedCling(null, 0.0f, 0.0f);
        Debug.Log("HOOK HIDE");
        enabled = false;
        hookMesh.enabled = false;
        hookInHandMesh.enabled = true;
        hook.transform.position = new Vector3(hook.transform.position.x, -10.0f, hook.transform.position.z);
        chain.enabled = false;
        rollback = false;
        throwing = false;
        throwingTime = 0.0f;
    }

    public void Show(float throwTime)
    {
        Debug.Log("HOOK SHOW (time: " + throwTime + ")");
        DelayedCling(null, 0.0f, 0.0f);
        hook.transform.position = new Vector3(hook.transform.position.x, 0.1f, hook.transform.position.z);
        destinationTimemark = throwTime + 0.311f;
        throwTimemark = throwTime;
        //Update();
        enabled = true;
        throwing = true;
        throwingTime = 0.0f;
        lastVelocity = new Vector3(velocity.x, velocity.y, velocity.z);
    }

    public void Move(float throwTime)
    {
        Debug.Log("HOOK MOVE (time: " + throwTime + ")");
        int i;

        /* Cling to nearest body for pulling */
        RaycastHit[] hits = Physics.SphereCastAll(transform.position + lastVelocity * 0.1f, 1.5f, Vector3.up);
        RaycastHit nearestHit = new RaycastHit();
        nearestHit.distance = 1000.0f;
        for (i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];
            if ((hit.collider.tag == "Enemy" || hit.collider.tag == "Player") && hit.collider.gameObject != player.gameObject)
            {
                if (hit.distance < nearestHit.distance)
                {
                    nearestHit = hit;
                }
            }
        }
        if (nearestHit.distance < 1000.0f)
        {
            RegionBodyBehavior target = nearestHit.collider.gameObject.GetComponent<RegionBodyBehavior>();
            DelayedCling(target, throwTime, 0.15f);
            destinationTimemark += 0.15f;
            velocity = target.transform.position - transform.position;
            velocity.y = 0.0f;
            velocity = velocity.normalized * lastVelocity.magnitude;
            lastVelocity = new Vector3(velocity.x, velocity.y, velocity.z);
        }
        else
        {
            destinationTimemark = throwTime;
            throwTimemark = destinationTimemark;
            //Update();
            throwing = false;
            //wrappingCooldown = 1.0f;
            lastVelocity = new Vector3(velocity.x, velocity.y, velocity.z);
        }
        enabled = true;
    }

    public void Rollback()
    {
        Debug.Log("HOOK ROLLBACK");
        DelayedCling(null, 0.0f, 0.0f);
        Vector3 direction3D = hookHandBone.position - hook.transform.position;
        Vector2 direction = new Vector2(direction3D.x, direction3D.z);
        //velocity = direction.normalized * direction.magnitude / 1.5f;
        startRollbackDistance = direction.magnitude;
        rollbackTimemark = 2.0f;
        rollback = true;
        enabled = true;
        throwing = false;
        transform.parent = null;
    }

    public void DelayedCling(RegionBodyBehavior target, float time, float timeout)
    {
        Debug.Log("SETUP DELAYED CLING: " + (target != null));
        transform.parent = null;
        delayedClingTarget = target;
        delayedClingTime = time - timeout;
        delayedClingTimeout = timeout;
    }

    public RegionBodyBehavior GetDelayedClingTarget()
    {
        return delayedClingTarget;
    }

}
