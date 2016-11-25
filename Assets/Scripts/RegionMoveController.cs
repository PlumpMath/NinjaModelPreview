using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RegionMoveController : MonoBehaviour {


    public float speed = 2.0f;
    public Camera camera;
    public SpriteRenderer battleIcon;

    private Vector3 direction = Vector3.zero;
    private float battleCooldown = 0.0f;

    // Use this for initialization
    void Start () {

        int i;
        GameObject bot;
        for(i = 0; i < 10; i++)
        {
            bot = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Bot"));
        }

	}
	
	// Update is called once per frame
	void Update () {

        if (battleCooldown > 0.0f)
        {
            battleCooldown -= Time.deltaTime;
            if (battleCooldown <= 0.0f)
            {
                Application.LoadLevel("battle");
            }
            return;
        }

        if (Input.GetMouseButton(0))
        {
            direction.x = -Input.mousePosition.x / (float)Screen.width + 0.5f;
            direction.z = -Input.mousePosition.y / (float)Screen.height + 0.5f;
            direction.Normalize();
        }
        else if (Input.touchCount > 0)
        {
            direction.x = -Input.touches[0].position.x / (float)Screen.width + 0.5f;
            direction.z = -0.5f + Input.touches[0].position.y / (float)Screen.height;
            direction.Normalize();
        }
        else
        {
            direction *= 1.0f - Time.deltaTime * 5.0f;
        }
        transform.position += direction * speed * Time.deltaTime;
        if(transform.position.x < -10.0f)
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
        camera.transform.position += new Vector3(transform.position.x - camera.transform.position.x, 0.0f, transform.position.z - camera.transform.position.z) * Time.deltaTime * 5.0f;
        if (camera.transform.position.x < -7.0f)
        {
            camera.transform.position += Vector3.right * (-7.0f - camera.transform.position.x);
        }
        if (camera.transform.position.x > 7.0f)
        {
            camera.transform.position += Vector3.right * (7.0f - camera.transform.position.x);
        }
        if (camera.transform.position.z < -5.0f)
        {
            camera.transform.position += Vector3.forward * (-5.0f - camera.transform.position.z);
        }
        if (camera.transform.position.z > 5.0f)
        {
            camera.transform.position += Vector3.forward * (5.0f - camera.transform.position.z);
        }

        RaycastHit hit;
        if (Physics.SphereCast(new Ray(transform.position, transform.forward), 0.2f, out hit, 0.2f))
        {
            battleCooldown = 1.0f;
            battleIcon.enabled = true;
            GameObject.Destroy(hit.collider.gameObject.GetComponent<RegionBotBehavior>());
        }

    }
}
