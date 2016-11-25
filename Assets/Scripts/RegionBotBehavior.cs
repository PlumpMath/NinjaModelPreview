using UnityEngine;
using System.Collections;

public class RegionBotBehavior : MonoBehaviour {

    public float speed = 2.0f;

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

    }
}
