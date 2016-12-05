using UnityEngine;
using System.Collections;

public class RegionBotBehavior : MonoBehaviour {

    public GameObject playerIcon;
    public SpriteRenderer playerIconRenderer;
    public SpriteRenderer playerFaceRenderer;

    public RegionMap map = new RegionMap();
    public RegionMapNode mapNode = null;

    public int coverageType = 0;
    public int lastCoverageType = 0;

    public float speed = 1.0f;
    public float visibleDistance = 2.7f;

    private Vector3 direction = Vector3.zero;
    private float cooldown = 0.1f;

    // Use this for initialization
    void Start () {

        transform.position = new Vector3(Random.Range(-9.0f, 9.0f), 0.0f, Random.Range(-9.0f, 9.0f));

	}
	
	// Update is called once per frame
	void Update () {

        if(cooldown > 0.0f)
        {
            cooldown -= Time.deltaTime;
            if(cooldown <= 0.0f)
            {
                cooldown = 3.0f;
                direction.x = Random.Range(-1.0f, 1.0f);
                direction.z = Random.Range(-1.0f, 1.0f);
                direction.Normalize();
            }
        }
        transform.position += direction * speed * Time.deltaTime;
        playerIcon.transform.localRotation = Quaternion.LookRotation(Vector3.right * direction.x + Vector3.up * direction.z, Vector3.forward);
        if (transform.position.x < -10.0f)
        {
            transform.position += Vector3.right * (-10.0f - transform.position.x);
        }
        if (transform.position.x > 10.0f)
        {
            transform.position += Vector3.right * (10.0f - transform.position.x);
        }
        if (transform.position.z < -10.0f)
        {
            transform.position += Vector3.forward * (-10.0f - transform.position.z);
        }
        if (transform.position.z > 10.0f)
        {
            transform.position += Vector3.forward * (10.0f - transform.position.z);
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
                speed = 0.8f;
                newColor.a = 0.5f;
                visibleDistance = 1.8f;
            }
            else if (coverageType == 1)
            {
                speed = 1.2f;
                newColor.a = 0.75f;
                visibleDistance = 2.7f;
            }
            else
            {
                speed = 1.6f;
                visibleDistance = 5.0f;
            }
            playerIconRenderer.color = newColor;
            playerFaceRenderer.color = newColor;
        }

    }
}
