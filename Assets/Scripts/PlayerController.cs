using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    public GameObject opponent;

    public Tasks tasks;

    public WayPoint[] waypoints = new WayPoint[0];

    public WayPoint waypoint;
    public WayPoint lastWaypoint;

    public float position = 0.0f;

    public float direction = 1.0f;
    public float reverseCooldownMin = 0.5f;
    public float reverseCooldownMax = 1.5f;
    public float reverseTimeout = 0.0f;
    public float speed = 0.0f;

    public Rigidbody rigidbody;

    private AnimationState animationWalk;
    private float animationTime = 0.0f;

    public float globalSpeed = 1.0f;

    public int behaviorType = 1;

    // Use this for initialization
    void Start () {

        rigidbody = GetComponent<Rigidbody>();
        Animation animation = GetComponent<Animation>();
        if (animation != null)
        {
            animationWalk = animation["walk"];
            animationWalk.speed = 0.0f;
        }

    }

    // Update is called once per frame
    void Update () {

        int i;
        float dx;
        PlayerController me;
        WayPoint newWaypoint = null;
        WayPoint obj;

        reverseTimeout -= Time.deltaTime;
        if (reverseTimeout <= 0.0f)
        {


            me = tasks.players[0];
            for (i = 0; i < waypoints.Length; i++)
            {
                obj = waypoints[i];
                dx = (obj.transform.position - me.transform.position).x;
                if (dx > 20.0f)
                {
                    obj.transform.position -= Vector3.right * 40.0f;
                }
                if (dx < -20.0f)
                {
                    obj.transform.position += Vector3.right * 40.0f;
                }
            }


            /*
            reverseTimeout = Random.Range(reverseCooldownMin, reverseCooldownMax);
            direction = Random.Range(-1.0f, 1.0f);
            if(Mathf.Abs(direction) > 0.6f - globalSpeed * 0.2f)
            {
                direction /= Mathf.Abs(direction);
            }
            else
            {
                direction = 0.0f;
                animationTime = 0.0f;
            }
            if((opponent.transform.position - transform.position).x * direction < 0.0f)
            {
                direction *= -1.0f;
            }
            */
            bool runaway = (tasks.players[0] == this && tasks.hits <= 1) || (tasks.players[1] == this && tasks.hits > 1);
            if (Random.Range(0.0f, 1.0f) > 0.5f || (!runaway && behaviorType == 3))
            {
                speed = 1.5f * globalSpeed;
            }
            else
            {
                speed = 1.0f * globalSpeed;
            }
            if (opponent.transform.position.x - transform.position.x > 0.0f && !runaway)
            {
                direction = 1.0f;
            }
            else
            {
                direction = -1.0f;
            }
            if (behaviorType != 1)
            {
                if ((runaway && Random.Range(0.0f, 1.0f) > 0.5f) || (lastWaypoint != waypoint && (waypoint.waypointType == WayPoint.WaypointType.COVER_FULL || waypoint.waypointType == WayPoint.WaypointType.COVER_DOWN)))
                {
                    direction = 0.0f;
                }
            }
            if(runaway && (Random.Range(0.0f, 1.0f) > 0.4f && (waypoint.waypointType == WayPoint.WaypointType.COVER_FULL || waypoint.waypointType == WayPoint.WaypointType.COVER_DOWN) || Random.Range(0.0f, 1.0f) > 0.8f))
            {
                direction *= -1.0f;
            }
            if(direction > 0.0f)
            {
                newWaypoint = waypoint.rightNode;
            }
            else if(direction < 0.0f)
            {
                newWaypoint = waypoint.leftNode;
            }
            if (newWaypoint != null)
            {
                waypoint = newWaypoint;
                dx = waypoint.transform.position.x - transform.position.x;
                reverseTimeout = Mathf.Abs(dx) / speed;
            }
            else
            {
                reverseTimeout = Random.Range(0.5f, 0.5f);
                lastWaypoint = waypoint;
            }
        }
        float d = 0.0f;
        if (speed == 2.5f)
        {
            d = 0.6f * globalSpeed;
        }
        else
        {
            d = 0.4f * globalSpeed;
        }
        if (Mathf.Abs(direction) > 0.0f)
        {
            animationTime += d * -direction * Time.deltaTime;
            if (animationTime > 10.0f)
            {
                animationTime -= 10.0f;
            }
            if (animationTime < -10.0f)
            {
                animationTime += 10.0f;
            }
        }
        if (animationWalk != null)
        {
            animationWalk.time = animationTime;
        }
        rigidbody.velocity = Vector3.right * direction * speed;
        transform.position += Vector3.right * direction * speed * Time.deltaTime;

    }
}
