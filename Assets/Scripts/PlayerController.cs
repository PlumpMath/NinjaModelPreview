using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    public PlayerController opponent;

    public Tasks tasks;
    public Swipe swipe;

    public WayPoint[] waypoints = new WayPoint[0];

    public WayPoint waypoint;
    public WayPoint lastWaypoint;

    public float position = 0.0f;

    public float direction = 1.0f;
    public float reverseCooldownMin = 0.5f;
    public float reverseCooldownMax = 1.5f;
    public float reverseTimeout = 0.0f;
    public float speed = 0.0f;
    public float stamina = 0.0f;
    public float health = 1.0f;
    public bool runaway = false;
    public bool takeCover = false;

    public Rigidbody rigidbody;
    public Vector3 velocity = Vector3.zero;

    public float basePositionY = 0.0f;
    public float basePositionZ = 0.0f;
    public float marginY = 0.0f;
    public float marginZ = 0.0f;

    private AnimationState animationWalk;
    private float animationTime = 0.0f;

    public int behaviorType = 1;

    private float walkSpeed = 2.0f;
    private float runSpeed = 4.0f;

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
        float dx1;
        float dx2;
        float dx3;
        PlayerController me;
        WayPoint newWaypoint = null;
        WayPoint obj;

        reverseTimeout -= Time.deltaTime;
        if (reverseTimeout <= 0.0f)
        {


            me = this;
            for (i = 0; i < waypoints.Length; i++)
            {
                obj = waypoints[i];
                dx = (obj.transform.position - me.transform.position).x;
                if (dx > 40.0f)
                {
                    obj.transform.position -= Vector3.right * 80.0f;
                }
                if (dx < -40.0f)
                {
                    obj.transform.position += Vector3.right * 80.0f;
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

            newWaypoint = waypoint;
            runaway = (health < opponent.health);
            if (runaway)
            {
                if(takeCover)
                {
                    if(waypoint.waypointType == WayPoint.WaypointType.OPEN)
                    {
                        if(lastWaypoint == waypoint)
                        {
                            takeCover = false;
                            if(waypoint.leftNode.waypointType == WayPoint.WaypointType.COVER_FULL /* || waypoint.leftNode.waypointType == WayPoint.WaypointType.COVER_DOWN */)
                            {
                                newWaypoint = waypoint.leftNode;
                                speed = runSpeed;
                            }
                            else if(waypoint.rightNode.waypointType == WayPoint.WaypointType.COVER_FULL /* || waypoint.rightNode.waypointType == WayPoint.WaypointType.COVER_DOWN */)
                            {
                                newWaypoint = waypoint.rightNode;
                                speed = runSpeed;
                            }
                        }
                        else
                        {
                            speed = 0.0f;
                            reverseTimeout = 1.0f;
                        }
                    }
                    else
                    {
                        if (lastWaypoint == waypoint)
                        {
                            speed = walkSpeed;
                            if (Random.Range(0.0f, 1.0f) > 0.5f)
                            {
                                newWaypoint = waypoint.leftNode;
                            }
                            else
                            {
                                newWaypoint = waypoint.rightNode;
                            }
                        }
                        else
                        {
                            direction = 0.0f;
                            speed = 0.0f;
                            reverseTimeout = 0.5f;
                        }
                    }
                    if (newWaypoint.waypointType != WayPoint.WaypointType.COVER_DOWN && newWaypoint.waypointType != WayPoint.WaypointType.COVER_FULL)
                    {
                        marginY = 0.0f;
                        marginZ = 0.0f;
                    }
                }
                else
                {
                    if(waypoint.waypointType == WayPoint.WaypointType.COVER_FULL /* || waypoint.waypointType == WayPoint.WaypointType.COVER_DOWN */)
                    {
                        if (Random.Range(0.0f, 1.0f) > 0.3f)
                        {
                            takeCover = true;
                            marginZ = -5.0f;
                        }
                    }
                    if (waypoint.waypointType == WayPoint.WaypointType.COVER_DOWN)
                    {
                        if (Random.Range(0.0f, 1.0f) > 0.3f)
                        {
                            takeCover = true;
                            marginY = -2.0f;
                            marginZ = -5.0f;
                        }
                    }
                    if (takeCover)
                    {
                        if (lastWaypoint == waypoint)
                        {
                            speed = runSpeed;
                            if (opponent.transform.position.x - transform.position.x > 0.0f)
                            {
                                newWaypoint = waypoint.leftNode;
                            }
                            else
                            {
                                newWaypoint = waypoint.rightNode;
                            }
                        }
                        else
                        {
                            direction = 0.0f;
                            speed = 0.0f;
                            if (waypoint.waypointType == WayPoint.WaypointType.COVER_DOWN)
                            {
                                reverseTimeout = 1.5f;
                            }
                            else
                            {
                                reverseTimeout = 1.5f;
                            }
                        }
                    }
                    else
                    {
                        speed = walkSpeed;
                        if (opponent.transform.position.x - transform.position.x > 0.0f)
                        {
                            newWaypoint = waypoint.leftNode;
                        }
                        else
                        {
                            newWaypoint = waypoint.rightNode;
                        }
                    }
                }
            }
            else
            {
                speed = runSpeed;
                if (opponent.transform.position.x - transform.position.x > 0.0f)
                {
                    newWaypoint = waypoint.rightNode;
                }
                else
                {
                    newWaypoint = waypoint.leftNode;
                }
                dx1 = opponent.transform.position.x - transform.position.x;
                dx2 = newWaypoint.transform.position.x - waypoint.transform.position.x;
                dx3 = waypoint.transform.position.x - lastWaypoint.transform.position.x;
                if (Mathf.Abs(dx1) < 0.001f)
                {
                    dx1 = 0.001f;
                }
                if (Mathf.Abs(dx2) < 0.001f)
                {
                    dx2 = 0.001f;
                }
                if (Mathf.Abs(dx3) < 0.001f)
                {
                    dx3 = 0.001f;
                }
                if (Mathf.Abs(dx1) < Mathf.Abs(dx2) * 0.5f && Mathf.Abs(dx2 / Mathf.Abs(dx2) - dx3 / Mathf.Abs(dx3)) > 0.5f)
                {
                    if (Random.Range(0.0f, 1.0f) > 0.5f)
                    {
                        newWaypoint = waypoint;
                        speed = 0.0f;
                        reverseTimeout = 0.5f;
                    }
                    else
                    {
                        speed = walkSpeed;
                        /*
                        if (waypoint.leftNode == newWaypoint)
                        {
                            newWaypoint = waypoint.rightNode;
                        }
                        else
                        {
                            newWaypoint = waypoint.leftNode;
                        }
                        */
                    }
                }
            }
            if (waypoint != newWaypoint)
            {
                if (newWaypoint.transform.position.x - waypoint.transform.position.x > 0.0f)
                {
                    direction = 1.0f;
                }
                else
                {
                    direction = -1.0f;
                }
                dx = newWaypoint.transform.position.x - transform.position.x;
                reverseTimeout = Mathf.Abs(dx) / speed;
            }
            else
            {
                direction = 0.0f;
            }



            if(behaviorType == 1)
            {
                newWaypoint = waypoint;
                reverseTimeout = 1.0f;
                speed = 0.0f;
                direction = 0.0f;
            }


            lastWaypoint = waypoint;
            waypoint = newWaypoint;

            if(lastWaypoint.waypointType == WayPoint.WaypointType.OPEN && tasks.players[1] == this)
            {
                float throwAngle = Vector3.Angle(opponent.transform.position + opponent.velocity * 0.4f - transform.position, new Vector3(0.0f, opponent.transform.position.y, -50.0f));
                if(opponent.transform.position.x > transform.position.x)
                {
                    throwAngle *= -1.0f;
                }
                if (stamina > 0.33f && Mathf.Abs(opponent.transform.position.x - transform.position.x) < 10.0f)
                {
                    stamina -= 0.33f;
                    swipe.Throw2(this, new Vector2(throwAngle + Random.Range(-0.75f, 0.75f), Random.Range(0.0f, 1.0f)), 0.0f, 1.0f);
                }
            }

            /*
            if (Random.Range(0.0f, 1.0f) > 0.5f || (!runaway && behaviorType == 3))
            {
                speed = 1.5f * globalSpeed;
            }
            else
            {
                speed = 0.7f * globalSpeed;
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
            */
        }
        float d = 0.0f;
        if (speed == runSpeed)
        {
            d = 1.2f;
        }
        else
        {
            d = 0.6f;
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
        velocity = Vector3.right * direction * speed;
        //rigidbody.velocity = velocity;
        transform.position += velocity * Time.deltaTime;
        transform.position += Vector3.up * (basePositionY + marginY - transform.position.y) * Mathf.Min(1.0f, Time.deltaTime * 5.0f);
        transform.position += Vector3.forward * (basePositionZ + (basePositionZ / Mathf.Abs(basePositionZ)) * marginZ - transform.position.z) * Mathf.Min(1.0f, Time.deltaTime * 5.0f);

        stamina += Time.deltaTime * 0.2f;
        if(stamina > 1.0f)
        {
            stamina = 1.0f;
        }

    }
}
