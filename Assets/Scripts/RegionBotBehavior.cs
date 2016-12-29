﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RegionBotBehavior : MonoBehaviour {

    public GameObject player;

    public GameObject playerIcon;
    public SpriteRenderer playerIconRenderer;
    public SpriteRenderer playerFaceRenderer;
    public SpriteRenderer playerRankCircleRenderer;
    public Image offscreenPointer;

    public RegionMap map = new RegionMap();
    public RegionMapNode mapNode = null;

    public RegionHook hook = null;

    public int coverageType = 0;
    public int lastCoverageType = 0;

    public float rankModifier = 0.0f;

    public float speed = 1.0f;
    public float visibleDistance = 7.0f;
    public bool isVisible = true;
    public bool isGoodVisible = true;

    public Vector3 direction = Vector3.zero;
    private float cooldown = 0.1f;
    private float cooldown2 = 0.1f;

    // Use this for initialization
    void Start () {

        transform.position = new Vector3(Random.Range(-9.0f, 9.0f), 0.0f, Random.Range(-9.0f, 9.0f));
        rankModifier = Random.Range(-1.0f, 1.0f);
        playerRankCircleRenderer.color = new Color(rankModifier * 0.5f + 0.5f, 0.75f - rankModifier * 0.25f, rankModifier * 0.5f + 0.5f, 1.0f);
        hook.transform.parent = null;
        hook.Hide();

    }
	
	// Update is called once per frame
	void Update () {

        if(cooldown2 > 0.0f)
        {
            cooldown2 -= Time.deltaTime;
            if(cooldown2 <= 0.0f)
            {
                cooldown2 = 0.1f;
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
            }
        }
        if(cooldown > 0.0f)
        {
            cooldown -= Time.deltaTime;
            if(cooldown <= 0.0f)
            {
                cooldown = 0.7f;
                direction.x = Random.Range(-1.0f, 1.0f);
                direction.z = Random.Range(-1.0f, 1.0f);
                direction.Normalize();
                if((player.transform.position - transform.position).magnitude < 2.7f)
                {
                    if(rankModifier > 0.0f)
                    {
                        direction = player.transform.position - transform.position;
                    }
                    else
                    {
                        direction = transform.position - player.transform.position;
                    }
                    direction.y = 0.0f;
                    direction.Normalize();
                    direction.x += Random.Range(-0.1f, 0.1f);
                    direction.z += Random.Range(-0.1f, 0.1f);
                    direction.Normalize();
                }
            }
        }
        transform.position += direction * speed * Time.deltaTime;
        playerIcon.transform.localRotation = Quaternion.LookRotation(Vector3.right * direction.x + Vector3.up * direction.z, Vector3.forward);
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
                speed = 1.2f; // 0.8f
                newColor.a = 0.5f;
                visibleDistance = 3.0f;
            }
            else if (coverageType == 1)
            {
                speed = 1.2f;
                newColor.a = 0.75f;
                visibleDistance = 5.0f;
            }
            else
            {
                speed = 1.6f;
                visibleDistance = 7.0f;
            }
            playerIconRenderer.color = newColor;
            playerFaceRenderer.color = newColor;
            newColor = new Color(rankModifier * 0.5f + 0.5f, 1.0f - rankModifier * 0.5f, 0.5f - rankModifier * 0.25f, newColor.a);
            playerRankCircleRenderer.color = newColor;
        }

        if (!hook.enabled && rankModifier > 0.0f)
        {
            hook.transform.position = transform.position;
            hook.velocity = playerIcon.transform.forward * 3.0f;
            hook.Show();
        }

    }
}
