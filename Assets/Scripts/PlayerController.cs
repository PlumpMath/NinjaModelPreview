using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    public GameObject opponent;

    public float position = 0.0f;

    public float direction = 1.0f;
    public float reverseCooldownMin = 0.5f;
    public float reverseCooldownMax = 1.5f;
    public float reverseTimeout = 0.0f;
    public float speed = 0.0f;

    private AnimationState animationWalk;
    private float animationTime = 0.0f;

    public float globalSpeed = 1.0f;

    // Use this for initialization
    void Start () {

        Animation animation = GetComponent<Animation>();
        if (animation != null)
        {
            animationWalk = animation["walk"];
            animationWalk.speed = 0.0f;
        }

    }

    // Update is called once per frame
    void Update () {

        reverseTimeout -= Time.deltaTime;
        if(reverseTimeout <= 0.0f)
        {
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
            if(Random.Range(0.0f, 1.0f) > 0.5f)
            {
                speed = 1.5f * globalSpeed;
            }
            else
            {
                speed = 1.0f * globalSpeed;
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
        transform.position += Vector3.right * direction * speed * Time.deltaTime;

    }
}
