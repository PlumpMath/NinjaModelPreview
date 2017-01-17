using UnityEngine;
using System.Collections;

public class MissileController : MonoBehaviour {

    public Tasks taskObject;

    public GameObject meshObject;
    public TrailRenderer[] trails = new TrailRenderer[4];

    public Vector3 velocity = Vector3.zero;
    public Vector3 acceleration = Vector3.zero;
    public Vector3 torsion = Vector3.zero;
    public float passiveRotation = 360.0f;

    public GameObject damagePrefab;
    public MissileObject obj;
    public GameNetwork gameNetwork;

    private Rigidbody rigidbody;
    private bool collided = false;

    void Start () {

        name = "MissileObject";
        rigidbody = GetComponent<Rigidbody>();
        GameObject.Destroy(gameObject, 20.0f);

    }

    void Update () {

        if (false && collided)
        {
        }
        else
        {
            velocity += acceleration * Time.deltaTime;
            //transform.position += velocity * Time.deltaTime;
            if (collided)
            {
                velocity *= 1.0f - Time.deltaTime * 20.0f;
            }
            else
            {
                if(float.IsNaN(torsion.x))
                {
                    torsion.x = 0.0f;
                }
                if (float.IsNaN(torsion.y))
                {
                    torsion.y = 0.0f;
                }
                if (float.IsNaN(torsion.z))
                {
                    torsion.z = 0.0f;
                }
                transform.Rotate(torsion.x * Time.deltaTime, torsion.z * Time.deltaTime /* torsion.y * Time.deltaTime */, -2.0f * torsion.y * Time.deltaTime);
                meshObject.transform.Rotate(0.0f, 0.0f, -(passiveRotation + 50.0f * torsion.y) * Time.deltaTime);
            }
            if (rigidbody != null)
            {
                rigidbody.velocity = velocity;
            }
        }

    }

    public void DestroyDelayed(Vector3 passiveVelocity)
    {
        velocity = passiveVelocity;
        //Destroy(gameObject, 1.0f);
    }

    public void DestroyImmediate()
    {

        //Destroy(gameObject, 0.2f);
    }

    void OnCollisionEnter(Collision collision)
    {
        int i;
        if(collided)
        {
            return;
        }
        if(collision.collider.name == "MissileObject")
        {
            return;
        }
        if(collision.collider.tag == "Player" && ((transform.position.z < 0.0f && velocity.z > 0.0f) || (transform.position.z > 0.0f && velocity.z < 0.0f)))
        {
            return;
        }
        collided = true;
        //transform.position = (transform.position + collision.contacts[0].point) * 0.5f;
        acceleration *= 0.0f;
        velocity *= 0.0f;
        transform.parent = collision.collider.transform;
        transform.position = collision.contacts[0].point;
        Rigidbody.Destroy(rigidbody);
        rigidbody = null;
        Collider collider = GetComponent<SphereCollider>();
        Rigidbody.Destroy(collider);
        if (collision.collider.tag == "BonusHolder")
        {
            if(collision.collider.name != "Striked")
            {
                collision.collider.GetComponent<Animation>().Play("Strike1");
                collision.collider.name = "Striked";
            }
            else
            {
                collision.collider.GetComponent<Animation>().Play("Strike2");
                GameObject.Destroy(collision.collider);
            }
        }
        if (collision.collider.tag == "Player")
        {
            if (collision.collider.transform.position.z > 0.0f)
            {
                velocity = ((transform.position + collision.contacts[0].point) * 0.5f - transform.position) * 25.0f;
                GameObject projector = (GameObject)GameObject.Instantiate(damagePrefab);
                projector.transform.position = collision.contacts[0].point + collision.contacts[0].normal * 0.2f;
                projector.transform.forward = transform.forward;
                projector.transform.parent = collision.collider.transform;
                GameObject.Destroy(projector, 1.0f);
                taskObject.Hit(transform.position.y);
            }
            else
            {
                taskObject.HitMe();
                GameObject brokenGlass = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/BrokenGlass"));
                brokenGlass.transform.position = transform.position;
                brokenGlass.transform.parent = transform;
                brokenGlass.transform.rotation = Quaternion.identity;
            }
            for (i = 0; i < trails.Length; i++)
            {
                GameObject.Destroy(trails[i].gameObject);
            }
        }
        GameObject.Destroy(gameObject, 1.0f);
    }

}
