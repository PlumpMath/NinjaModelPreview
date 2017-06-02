using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class RegionBodyBehavior : MonoBehaviour {

    public RegionBodyController body;
    public GameObject directionPointer;
    public Image progressCircle;
    public Image offscreenPointer;
    public SpriteRenderer smileyBackground;
    public SpriteRenderer smileyIcon;

    public RegionMap map = new RegionMap();
    public RegionMapNode mapNode = null;

    public RegionHook hook = null;

    public string playerId = "";

    public int coverageType = 0;
    public int lastCoverageType = 0;
    public float rankModifier = 0.0f;
    public float speed = 2.1f;
    public float blockInput = 0.0f;
    public float moveTimeout = 0.0f;
    public float pullingTime = 0.0f;
    public float smoothLean = 0.0f;
    public float battleCooldown = 0.0f;
    public float visibleDistance = 15.0f;
    public bool isVisible = true;
    public bool isGoodVisible = true;

    public Vector3 direction = Vector3.zero;
    public Vector3 inputDirection = Vector3.zero;
    public Vector3 smoothDirection = Vector3.forward;

    public LinkedList<RoutePoint> route = new LinkedList<RoutePoint>();

    private Vector3 normalDirection = Vector3.forward;
    private float smileyCooldown = 0.0f;
    private float searchingCooldown = 0.0f;
    private float animBlockCooldown = 0.0f;
    private float animPickupCooldown = 0.0f;
    private float taskProgress = 0.0f;

    // Use this for initialization
    void Start () {

        //rankModifier = Random.Range(-1.0f, 1.0f);
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

        speed = Mathf.Min(2.1f * 1.5f, Mathf.Max(0.0f, speed));

        transform.position += smoothDirection.normalized * speed * Time.deltaTime;

        if (moveTimeout > 0.0f)
        {
            moveTimeout -= Time.deltaTime;
        }
        if (moveTimeout <= 0.0f)
        {
            direction *= Mathf.Max(0.01f, 1.0f - Time.deltaTime * 10.0f);
        }
        if (direction.magnitude < 0.01f)
        {
            speed = 0.0f;
        }

        if (blockInput > 0.0f)
        {
            blockInput -= Time.deltaTime;
            if (blockInput < 0.0f)
            {
                blockInput = 0.0f;
            }
            if (pullingTime > 0.0f)
            {
                pullingTime -= Time.deltaTime;
            }
            if (!body.pulling && pullingTime > 0.0f)
            {
                body.pulling = true;
            }
            else if(body.pulling && pullingTime <= 0.0f)
            {
                body.pulling = false;
            }
        }
        else
        {
            if (body.pulling)
            {
                pullingTime = 0.0f;
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
            f = Mathf.Min(1.0f, Time.deltaTime * 5.0f);
            //if (blockInput > 0.0f)
            //{
            //    f = Mathf.Min(1.0f, Time.deltaTime * 10.0f);
            //}
            smoothDirection = Vector3.RotateTowards(smoothDirection, v3, f * Mathf.PI * (0.2f + Vector3.Angle(v3, smoothDirection) / 180.0f), 1.0f);
            directionPointer.transform.localRotation = Quaternion.LookRotation(smoothDirection, Vector3.up);
        }

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
            switch(coverageType)
            {
                case 2:
                    //speed = 1.2f; // 0.8f
                    //newColor.a = 0.5f;
                    //visibleDistance = 3.0f;
                    break;
                case 1:
                    //speed = 1.2f;
                    //newColor.a = 0.75f;
                    //visibleDistance = 5.0f;
                    break;
                default:
                    //speed = 1.6f;
                    //visibleDistance = 15.0f;
                    break;
            }
        }

        if (animBlockCooldown > 0.0f)
        {
            animBlockCooldown -= Time.deltaTime;
        }

        if (animPickupCooldown > 0.0f)
        {
            animPickupCooldown -= Time.deltaTime;
            if(animPickupCooldown <= 0.0f)
            {
                taskProgress = 0.0f;
            }
        }

        if (searchingCooldown > 0.0f)
        {
            searchingCooldown -= Time.deltaTime;
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

        if (progressCircle != null)
        {
            f = (taskProgress - progressCircle.fillAmount) * Time.deltaTime * 5.0f;
            if (taskProgress > 0.0f)
            {
                progressCircle.fillAmount = Mathf.Min(1.0f, Mathf.Max(0.0f, progressCircle.fillAmount * (1.0f - f) + taskProgress * f));
            }
            else if (progressCircle.fillAmount > 0.0f)
            {
                progressCircle.fillAmount = 0.0f;
            }
        }

        body.direction = smoothDirection;
        body.desiredDirection = normalDirection;
        body.speed = speed; // direction.magnitude / cooldown;
        body.searching = searchingCooldown > 0.0f;
        body.block = animBlockCooldown > 0.0f;
        body.pickingup = animPickupCooldown > 0.0f;
        body.hookThrowing = hook.enabled && !hook.rollback; //hook.throwing;
        body.hookRollback = hook.rollback;
        body.hookVisible = hook.hookMesh.enabled;
        body.hookDirection = (hook.transform.position + smoothDirection * 0.5f - transform.position).normalized;
        body.hookDirection.y = 0.0f;
        body.hookDirection.Normalize();

    }

    public void SetState(Vector2 destination, float moveTime)
    {
        direction = new Vector3(destination.x, transform.position.y, destination.y) - transform.position;
        if (moveTime == 0.0f)
        {
            transform.position = new Vector3(destination.x, transform.position.y, destination.y);
            speed = 0.0f;
            moveTimeout = 0.0f;
        }
        else
        {
            speed = direction.magnitude / moveTime;
            blockInput = moveTime;
            moveTimeout = moveTime;
            /*
            if (hook.transform.parent != null || body.locomotionBones[4].transform.FindChild("Hook") != null)
            {
                blockInput = moveTime;
                pullingTime = moveTime;
            }
            */
        }
        direction.Normalize();
        route.Clear();
    }

    public void SetProgress(float progress)
    {
        taskProgress = progress;
    }

    public void ThrowHook(Vector2 destination, float time)
    {
        Vector3 v3;
        if (!hook.enabled)
        {
            hook.transform.position = transform.position;
            v3 = (new Vector3(destination.x, hook.transform.position.y, destination.y) - hook.transform.position);
            hook.velocity = v3.normalized * Mathf.Min(15.0f, v3.magnitude / time);
            hook.cooldown = 5.0f;
            hook.Show(time);
        }
        else if (destination.magnitude != 0.0f)
        {
            v3 = new Vector3(destination.x, hook.transform.position.y, destination.y) - hook.transform.position;
            hook.velocity = v3.normalized * Mathf.Min(15.0f, v3.magnitude / time);
            hook.Move(time);
        }
        else
        {
            pullingTime = 0.0f;
            hook.Rollback();
        }
    }

    public void ShowChat(int iconId)
    {
        smileyCooldown = 2.0f;
        //smileyIcon.sprite = player.GetComponent<RegionMoveController>().smileyButtons[iconId].icon.sprite;
        smileyIcon.transform.localScale = Vector3.one * 1.5f;
        smileyBackground.enabled = true;
        smileyIcon.enabled = true;
    }

    public void ShowDiscovered(int iconId)
    {
        AnimatePickup();
        return;
        smileyCooldown = 2.0f;
        switch (iconId)
        {
            case 0:
                //smileyIcon.sprite = player.GetComponent<RegionMoveController>().someFoundSprite;
                break;
            default:
                //smileyIcon.sprite = player.GetComponent<RegionMoveController>().someFoundSprite;
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

    public void AnimateSearching()
    {
        searchingCooldown = 3.5f;
    }

    public void AnimateBlock()
    {
        animBlockCooldown = 0.8f;
    }

    public void AnimatePickup()
    {
        animPickupCooldown = 3.0f; // 4.667f
        taskProgress = 1.0f;
    }

}
