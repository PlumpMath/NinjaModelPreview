using UnityEngine;
using System.Collections;

public class MissileController : MonoBehaviour {

    public Tasks taskObject;

    public GameObject meshObject;

    public Vector3 velocity = Vector3.zero;
    public Vector3 acceleration = Vector3.zero;
    public Vector3 torsion = Vector3.zero;
    public float passiveRotation = 360.0f;

    public GameObject damagePrefab;

    private Rigidbody rigidbody;
    private bool collided = false;

    void Start () {

        name = "MissileObject";
        rigidbody = GetComponent<Rigidbody>();
        GameObject.Destroy(gameObject, 10.0f);

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
        Destroy(gameObject, 1.0f);
    }

    public void DestroyImmediate()
    {

        Destroy(gameObject, 0.2f);
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collided)
        {
            return;
        }
        if(collision.collider.name == "MissileObject")
        {
            return;
        }
        collided = true;
        //transform.position = (transform.position + collision.contacts[0].point) * 0.5f;
        acceleration *= 0.0f;
        velocity *= 0.0f;
        transform.parent = collision.collider.transform;
        Rigidbody.Destroy(rigidbody);
        rigidbody = null;
        if (collision.collider.name != "Ground")
        {
            velocity = ((transform.position + collision.contacts[0].point) * 0.5f - transform.position) * 25.0f;
            GameObject projector = (GameObject)GameObject.Instantiate(damagePrefab);
            projector.transform.position = collision.contacts[0].point + collision.contacts[0].normal * 0.2f;
            projector.transform.forward = transform.forward;
            GameObject.Destroy(projector, 1.0f);
            taskObject.Hit(transform.position.y);
        }
        GameObject.Destroy(gameObject, 1.0f);
    }

}
