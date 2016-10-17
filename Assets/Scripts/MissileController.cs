using UnityEngine;
using System.Collections;

public class MissileController : MonoBehaviour {

    public GameObject meshObject;

    public Vector3 velocity = Vector3.zero;
    public Vector3 acceleration = Vector3.zero;
    public Vector3 torsion = Vector3.zero;
    public float passiveRotation = 900.0f;

    void Start () {

        name = "MissileObject";

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

}
