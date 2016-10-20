using UnityEngine;
using System.Collections;

public class MissileController : MonoBehaviour {

    public Tasks taskObject;

    public GameObject meshObject;

    public Vector3 velocity = Vector3.zero;
    public Vector3 acceleration = Vector3.zero;
    public Vector3 torsion = Vector3.zero;
    public float passiveRotation = 900.0f;

    public GameObject damagePrefab;

    private SphereCollider collider;
    private bool collided = false;

    void Start () {

        name = "MissileObject";
        GameObject.Destroy(gameObject, 10.0f);

    }

    void Update () {

        transform.Rotate(torsion.x * Time.deltaTime, torsion.y * Time.deltaTime, -2.0f * torsion.z * Time.deltaTime);
        meshObject.transform.Rotate(0.0f, 0.0f, -(passiveRotation + 50.0f * torsion.y) * Time.deltaTime);
        velocity += acceleration * Time.deltaTime;
        transform.position += velocity * Time.deltaTime;

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
        collided = true;
        if (collision.collider.name != "Ground")
        {
            GameObject projector = (GameObject)GameObject.Instantiate(damagePrefab);
            projector.transform.position = collision.contacts[0].point + collision.contacts[0].normal * 0.2f;
            //projector.transform.forward = -collision.contacts[0].normal;
            projector.transform.forward = transform.forward;
            GameObject.Destroy(projector, 1.0f);
            taskObject.Hit(transform.position.y);
        }
        GameObject.Destroy(gameObject);
    }

}
