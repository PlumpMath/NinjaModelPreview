using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RegionBotBehavior : MonoBehaviour {

    public GameObject player;

    public RegionBodyController body;
    //public Animation anim;
    public GameObject playerIcon;
    public GameObject playerIconOuter;
    public GameObject playerIconInner;
    public Transform[] locomotionBones;
    public Rigidbody[] turbulencedBones = new Rigidbody[0];
    public GameObject directionPointer;
    public SpriteRenderer playerIconRenderer;
    public SpriteRenderer playerFaceRenderer;
    public SpriteRenderer playerRankCircleRenderer;
    public Image offscreenPointer;
    public SpriteRenderer smileyBackground;
    public SpriteRenderer smileyIcon;

    public ParticleSystem stepsPS1;
    public ParticleSystem stepsPS2;

    public RegionMap map = new RegionMap();
    public RegionMapNode mapNode = null;

    public RegionHook hook = null;

    public string playerId = "";

    public int coverageType = 0;
    public int lastCoverageType = 0;

    public float rankModifier = 0.0f;

    public float speed = 1.0f;
    public float visibleDistance = 15.0f;
    public bool isVisible = true;
    public bool isGoodVisible = true;

    public Vector3 direction = Vector3.zero;
    public Vector3 normalDirection = Vector3.forward;
    private Vector3 smoothDirection = Vector3.zero;
    private Vector3 adaptiveSmoothDirection = Vector3.zero;
    public Vector3 inputDirection = Vector3.zero;
    public Vector3 lastInputDirection = Vector3.zero;
    private bool inputTouched = false;
    public float blockInput = 0.0f;
    public float smoothLean = 0.0f;
    public float battleCooldown = 0.0f;
    private float botActionCooldown = 0.0f;
    private float bushDistanceTraveled = 0.0f;
    private float discoveredTimer = 0.0f;
    private float smileyCooldown = 0.0f;
    private float leaveCooldown = 0.0f;
    private float releaseProgress = 0.0f;
    private float searchingCooldown = 0.0f;
    private float animBlockCooldown = 0.0f;
    private float cooldown = 0.0f;
    private float cooldown2 = 0.0f;
    private float sign = 0.0f;
    private float animMoveWeight = 0.0f;

    public float xEdge = 0.0f;
    public float yEdge = 0.0f;
    public float xx1 = 0.0f;
    public float yy1 = 0.0f;

    // Use this for initialization
    void Start () {

        //transform.position = new Vector3(Random.Range(-9.0f, 9.0f), 0.0f, Random.Range(-9.0f, 9.0f));
        rankModifier = Random.Range(-1.0f, 1.0f);
        //playerRankCircleRenderer.color = new Color(rankModifier * 0.5f + 0.5f, 0.75f - rankModifier * 0.25f, rankModifier * 0.5f + 0.5f, 1.0f);
        hook.transform.parent = null;
        hook.Hide();

        body.SetCloth("10005");

    }
	
	// Update is called once per frame
	void Update () {

        int i;
        float t;
        float f;
        Vector3 v3;
        Quaternion q;

        if(cooldown2 > 0.0f)
        {
            cooldown2 -= Time.deltaTime;
            if(cooldown2 <= 0.0f)
            {
                cooldown2 = 0.1f;
                /*
                RaycastHit hit;
                if (Physics.SphereCast(hook.hook.transform.position - Vector3.up, 0.3f, Vector3.up, out hit, 2.0f, 255))
                {
                    if (hit.collider.tag == "Player")
                    {
                        RegionMoveController playerController = player.GetComponent<RegionMoveController>();
                        playerController.battleCooldown = 1.0f;
                        hook.targetRank = gameObject.GetComponent<RegionBotBehavior>().rankModifier;
                        GameObject.Destroy(gameObject.GetComponent<RegionBotBehavior>());
                        hook.hook.transform.position = hit.collider.transform.position;
                        playerController.battleIcon.transform.position = transform.position + (hit.collider.transform.position - transform.position).normalized * 0.5f + Vector3.up * 0.1f;
                        playerController.battleIcon.enabled = true;
                        playerController.battleIcon.transform.parent = null;
                        hit.collider.transform.parent = hook.hook.transform;
                        hook.Rollback();
                    }
                }
                */
            }
        }
        if(cooldown > 0.0f)
        {
            cooldown -= Time.deltaTime;
            t = Time.deltaTime;
            if(cooldown <= 0.0f)
            {
                t = cooldown + Time.deltaTime;
                cooldown = 0.0f;
                /*
                cooldown = 0.7f;
                direction.x = Random.Range(-1.0f, 1.0f);
                direction.z = Random.Range(-1.0f, 1.0f);
                direction.Normalize();
                if((player.transform.position - transform.position).magnitude < 2.7f)
                {
                    *//*
                    if(rankModifier > 0.0f)
                    {
                        direction = player.transform.position - transform.position;
                    }
                    else
                    {
                        direction = transform.position - player.transform.position;
                    }
                    *//*
                    direction.y = 0.0f;
                    direction.Normalize();
                    direction.x += Random.Range(-0.1f, 0.1f);
                    direction.z += Random.Range(-0.1f, 0.1f);
                    direction.Normalize();
                }
                */
            }
            transform.position += direction * speed * t;
        }
        else
        {
            direction *= Mathf.Max(0.01f, 1.0f - Time.deltaTime * 10.0f);
        }

        if (smileyCooldown > 0.0f)
        {
            smileyCooldown -= Time.deltaTime;
            if (smileyCooldown <= 0.0f)
            {
                smileyCooldown = 0.0f;
                smileyIcon.enabled = false;
                smileyBackground.enabled = false;
            }
        }
        //transform.position += direction * speed * Time.deltaTime;

        /*
        if (direction.magnitude > 0.0f)
        {
            playerIcon.transform.localRotation = Quaternion.LookRotation(Vector3.right * direction.x + Vector3.up * direction.z, -Vector3.forward);
        }
        */

        if (blockInput > 0.0f)
        {
            blockInput -= Time.deltaTime;
            if (blockInput < 0.0f)
            {
                blockInput = 0.0f;
            }
            body.pulling = true;
        }
        else
        {
            if (body.pulling)
            {
                body.pulling = false;
            }
        }

        if (direction.magnitude > 0.001f)
        {
            normalDirection.x = direction.x;
            normalDirection.z = direction.z;
            normalDirection.Normalize();
            v3 = new Vector3(normalDirection.x, 0.0f, normalDirection.z);
            smoothDirection.Normalize();
            //f = Mathf.Min(0.1f, Time.deltaTime * 6.0f);
            //smoothDirection = smoothDirection * (1.0f - f) + new Vector3(v3.x, v3.z, 0.0f) * f;
            f = Mathf.Min(1.0f, Time.deltaTime * 1.0f);
            smoothDirection = Vector3.RotateTowards(smoothDirection, v3, f * Mathf.PI, 1.0f);
            adaptiveSmoothDirection = new Vector3(smoothDirection.x, 0.0f, smoothDirection.z);

            directionPointer.transform.localRotation = Quaternion.LookRotation(smoothDirection, Vector3.up);

            /*
            v3 = new Vector3(v3.x, v3.z, 0.0f);
            q = Quaternion.LookRotation(smoothDirection, Vector3.up);

            float eulerYn = q.eulerAngles.y;
            float eulerYc = playerIcon.transform.localRotation.eulerAngles.y;

            if (eulerYc - eulerYn > 180.0f)
            {
                eulerYc -= 360.0f;
            }
            else if (eulerYn - eulerYc > 180.0f)
            {
                eulerYc += 360.0f;
            }

            //Debug.Log("Q2: " + eulerXc + " -> " + eulerXn + " (d: " + (eulerXc - eulerXn) + ")");

            sign = 1.0f;
            if (eulerYc > eulerYn)
            {
                sign = -1.0f;
            }
            f = Mathf.Min(0.1f, Time.deltaTime * 5.0f);
            smoothLean = smoothLean * (1.0f - f) + Mathf.Min(0.33f, Mathf.Max(-0.33f, Vector3.Angle(adaptiveSmoothDirection, normalDirection) * 0.01f * sign)) * f;

            playerIcon.transform.localRotation = q;
            playerIconInner.transform.localRotation = Quaternion.LookRotation(Vector3.forward, Vector3.up + Vector3.right * smoothLean);
            //playerIconOuter.transform.localPosition = playerIconOuter.transform.localPosition * Mathf.Max(0.0f, 1.0f - Time.deltaTime * 1.5f);
            //playerIconOuter.transform.position += adaptiveSmoothDirection * 1.6f * 3.0f * Vector3.Angle(adaptiveSmoothDirection, normalDirection) / 180.0f * Time.deltaTime;


            for (i = 0; i < turbulencedBones.Length; i++)
            {
                turbulencedBones[i].AddForce((Vector3.up * (Mathf.Sin(Time.time * 15.0f + turbulencedBones[i].transform.position.x * 5.0f) + 0.7f) * 10.0f) * 50.0f * Time.deltaTime);
            }
            */

        }
        else
        {
            /*
            if (Mathf.Abs(smoothLean) > 0.05f)
            {
                smoothLean *= (1.0f - Time.deltaTime * 2.0f);
                playerIconInner.transform.localRotation = Quaternion.LookRotation(Vector3.forward, Vector3.up + Vector3.right * smoothLean);
            }
            */
        }

        ParticleSystem.EmissionModule emission1 = stepsPS1.emission;
        ParticleSystem.EmissionModule emission2 = stepsPS2.emission;

        if (direction.magnitude <= 0.5f && emission1.enabled)
        {
            emission1.enabled = false;
            emission2.enabled = false;
        }
        else if (direction.magnitude > 0.5f && !emission1.enabled && blockInput > 0.0f)
        {
            emission1.enabled = true;
            emission2.enabled = true;
        }
        else if (direction.magnitude > 0.5f && !emission2.enabled)
        {
            emission1.enabled = false;
            emission2.enabled = true;
        }




        /*
        if (transform.position.x < -9.0f)
        {
            transform.position += Vector3.right * (-9.0f - transform.position.x);
        }
        if (transform.position.x > 9.0f)
        {
            transform.position += Vector3.right * (9.0f - transform.position.x);
        }
        if (transform.position.z < -27.0f)
        {
            transform.position += Vector3.forward * (-27.0f - transform.position.z);
        }
        if (transform.position.z > 29.0f)
        {
            transform.position += Vector3.forward * (29.0f - transform.position.z);
        }
        */

        mapNode = map.FindNode(transform.position.x, transform.position.z);
        if (mapNode != null)
        {
            coverageType = mapNode.coverageType;
        }
        else
        {
            coverageType = 0;
        }
        if (coverageType != lastCoverageType)
        {
            lastCoverageType = coverageType;
            Color newColor = new Color(1.0f, 0.5f, 0.5f, 1.0f);
            if (coverageType == 2)
            {
                //speed = 1.2f; // 0.8f
                newColor.a = 0.5f;
                //visibleDistance = 3.0f;
            }
            else if (coverageType == 1)
            {
                //speed = 1.2f;
                newColor.a = 0.75f;
                //visibleDistance = 5.0f;
            }
            else
            {
                //speed = 1.6f;
                visibleDistance = 15.0f;
            }
            //playerIconRenderer.color = newColor;
            //playerFaceRenderer.color = newColor;
            newColor = new Color(rankModifier * 0.5f + 0.5f, 1.0f - rankModifier * 0.5f, 0.5f - rankModifier * 0.25f, newColor.a);
            //playerRankCircleRenderer.color = newColor;
        }

        /*
        if (!hook.enabled && rankModifier > 0.0f)
        {
            hook.transform.position = transform.position;
            hook.velocity = playerIcon.transform.forward * 3.0f;
            hook.Show();
        }
        */



        if (animBlockCooldown > 0.0f)
        {
            animBlockCooldown -= Time.deltaTime;
        }

        body.direction = smoothDirection;
        body.desiredDirection = normalDirection;
        body.speed = direction.magnitude;
        body.searching = searchingCooldown > 0.0f;
        body.block = animBlockCooldown > 0.0f;
        body.hookThrowing = hook.enabled && !hook.rollback; //hook.throwing;
        body.hookRollback = hook.rollback;
        body.hookDirection = (hook.transform.position + smoothDirection * 2.0f - transform.position).normalized;

    }

    public void SetState(Vector2 destination, float moveTime)
    {
        direction = new Vector3(destination.x, transform.position.y, destination.y) - transform.position;
        cooldown = moveTime;
        if (moveTime < 0.05f)
        {
            if ((transform.position - new Vector3(destination.x, transform.position.y, destination.y)).magnitude > 0.1f)
            {
                transform.position = new Vector3(destination.x, transform.position.y, destination.y);
            }
            speed = 0.0f;
        }
        else
        {
            speed = direction.magnitude / cooldown;
            if ((destination - new Vector2(player.transform.position.x, player.transform.position.z)).magnitude < 0.5f)
            {
                blockInput = moveTime;
            }
            if ((new Vector2(hook.transform.position.x, hook.transform.position.z) - new Vector2(player.transform.position.x, player.transform.position.z)).magnitude < 0.5f)
            {
                blockInput = moveTime;
            }
            direction.Normalize();
        }
    }

    public void ThrowHook(Vector2 destination, float time)
    {
        Vector3 v3;
        if (!hook.enabled)
        {
            hook.transform.position = transform.position;
            v3 = (new Vector3(destination.x, transform.position.y, destination.y) - transform.position);
            hook.velocity = v3.normalized * (v3.magnitude / time);
            hook.cooldown = 8.0f;
            hook.Show(time);
        }
        else if (destination.magnitude != 0.0f)
        {
            v3 = new Vector3(destination.x, hook.transform.position.y, destination.y) - hook.transform.position;
            v3.y = 0.0f;
            hook.velocity = v3.normalized * (v3.magnitude / time);
            hook.Move(time);
        }
        else
        {
            hook.Rollback();
        }
    }

    public void ShowChat(int iconId)
    {
        smileyCooldown = 2.0f;
        smileyIcon.sprite = player.GetComponent<RegionMoveController>().smileyButtons[iconId].icon.sprite;
        smileyIcon.transform.localScale = Vector3.one * 1.5f;
        smileyBackground.enabled = true;
        smileyIcon.enabled = true;
    }

    public void ShowDiscovered(int iconId)
    {
        smileyCooldown = 2.0f;
        switch (iconId)
        {
            case 0:
                smileyIcon.sprite = player.GetComponent<RegionMoveController>().someFoundSprite;
                break;
            default:
                smileyIcon.sprite = player.GetComponent<RegionMoveController>().someFoundSprite;
                break;
        }
        smileyIcon.transform.localScale = Vector3.one * 1.5f;
        smileyBackground.enabled = true;
        smileyIcon.enabled = true;
    }

    public void SetCloth(string id)
    {
        body.SetCloth(id);
    }

    public void AnimateBlock()
    {
        animBlockCooldown = 0.8f;
    }

}
