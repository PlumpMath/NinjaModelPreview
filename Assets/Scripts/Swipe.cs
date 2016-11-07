using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.Networking.Match;
using UnityEngine.Serialization;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class Swipe : MonoBehaviour {

    public enum ThrowState {
        NONE = 0,
        TOUCHED = 1
    }


    public Tasks taskObject;

    public GameObject swipeTrailPrefab;
    public GameObject missilePrefab;

    public Button swipeType2;
    public Button swipeType3;
    public Button swipeType4;
    public ArmedMissileController armedMissile;
    public SwipeTrailController swipeTrail = null;
    public SwipeController swipeController = new SwipeController();
    public Camera camera;
    public float lastTouchX = 0.0f;
    public float lastTouchY = 0.0f;
    public ThrowState throwState = ThrowState.NONE;

    private float screenScale = 1.0f;
    private float touchTime = 0.0f;

    private bool touchedOnce = false;

    void Start ()
    {
        swipeController.OnInvokeAction += OnThrow;
        if (Screen.dpi > 0.0f)
        {
            screenScale = Mathf.Min(1.0f, Mathf.Max(0.6f, 4.0f / (Screen.height / Screen.dpi)));
            swipeController.screenScale = screenScale;
            swipeController.screenAspect = (float)Screen.width / (float)Screen.height;
        }

        swipeType2.onClick.AddListener(delegate() {
            if (swipeType2.image.color == Color.red)
            {
                swipeType2.image.color = Color.green;
                swipeType3.image.color = Color.red;
                swipeType4.image.color = Color.red;
            }
            OnSwipeTypeChanged();
        });
        swipeType3.onClick.AddListener(delegate () {
            if (swipeType3.image.color == Color.red)
            {
                swipeType2.image.color = Color.red;
                swipeType3.image.color = Color.green;
                swipeType4.image.color = Color.red;
            }
            OnSwipeTypeChanged();
        });
        swipeType4.onClick.AddListener(delegate () {
            if (swipeType4.image.color == Color.red)
            {
                swipeType2.image.color = Color.red;
                swipeType3.image.color = Color.red;
                swipeType4.image.color = Color.green;
            }
            OnSwipeTypeChanged();
        });
        OnSwipeTypeChanged();

    }

    public void OnSwipeTypeChanged()
    {
        int swipeType = 2;
        /*
        if(swipeType2.image.color == Color.green)
        {
            if(swipeType3.image.color == Color.green)
            {
                swipeType = 4;
            }
            else
            {
                swipeType = 2;
            }
        }
        else
        {
            if (swipeType3.image.color == Color.green)
            {
                swipeType = 3;
            }
            else
            {
                swipeType = 1;
            }
        }
        */
        if(swipeType2.image.color == Color.green)
        {
            swipeType = 1;
        }
        if (swipeType4.image.color == Color.green)
        {
            swipeType = 3;
        }
        if(swipeType == 1)
        {
            taskObject.players[0].globalSpeed = 0.0f;
            taskObject.players[1].globalSpeed = 0.0f;
            taskObject.players[0].transform.position -= Vector3.right * taskObject.players[0].transform.position.x;
            taskObject.players[1].transform.position -= Vector3.right * taskObject.players[1].transform.position.x;
        }
        else if(swipeType == 2)
        {
            taskObject.players[0].globalSpeed = 1.0f;
            taskObject.players[1].globalSpeed = 1.0f;
        }
        else
        {
            taskObject.players[0].globalSpeed = 2.0f;
            taskObject.players[1].globalSpeed = 2.0f;
        }
        //swipeController.swipeType = swipeType;
    }

    private Vector3 startCameraPosition = new Vector3();
    private Vector3 normalCameraPosition = new Vector3(0.0f, 5.0f, -25.0f);
    private Quaternion startCameraRotation = new Quaternion();
    private Quaternion normalCameraRotation = Quaternion.Euler(10.0f, 0.0f, 0.0f);
    private float startCameraFov = 60;
    private float normalCameraFov = 30;
    private float normalCameraCooldown = 2.0f;

    void Update ()
    {
        float f;

        if (!touchedOnce)
        {
            armedMissile.Rearm();
        }
        if(!touchedOnce && (Input.touchCount > 0 || Input.GetMouseButtonDown(0)))
        {
            touchedOnce = true;
            startCameraPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            startCameraRotation = new Quaternion(transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w);
            startCameraFov = camera.fov;
        }
        if (touchedOnce && normalCameraCooldown > 0.0f)
        {
            normalCameraCooldown -= Time.deltaTime;
            f = normalCameraCooldown - 1.0f;
            f = ((Mathf.Pow(Mathf.Abs(f), 0.5f) * f / Mathf.Abs(f)) + 1.0f) * 0.5f;
            if(f < 0.0f)
            {
                f = 0.0f;
            }
            transform.position = normalCameraPosition * (1.0f - f) + startCameraPosition * f;
            transform.rotation = Quaternion.Lerp(normalCameraRotation, startCameraRotation, f);
            camera.fov = normalCameraFov * (1.0f - f) + startCameraFov * f;
            if (normalCameraCooldown <= 0.0f)
            {
                normalCameraCooldown = 0.0f;
                transform.position = normalCameraPosition;
                transform.rotation = normalCameraRotation;
                camera.fov = normalCameraFov;
                taskObject.players[0].enabled = true;
                taskObject.players[1].enabled = true;
                taskObject.players[1].GetComponent<Animation>().Play();
            }
        }

        float angle = 0.0f;
        float power = 0.0f;
        float mouseX = Input.mousePosition.x / (float)Screen.width;
        float mouseY = 1.0f - Input.mousePosition.y / (float)Screen.height;
        float touchX = 0.0f;
        float touchY = 0.0f;
        Vector2 v2 = Vector2.zero;
        Vector3 position = Vector3.zero;
#if !UNITY_STANDALONE
        Touch touch;
        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);
            touchX = touch.position.x / (float)Screen.width;
            touchY = 1.0f - touch.position.y / (float)Screen.height;
            position = (camera.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, 0.5f)) - camera.transform.position);
            v2.x = (touchX - 0.5f) / swipeController.screenScale;
            v2.y = Mathf.Max(0.0f, (1.0f - (touchY + 0.1f)) / swipeController.screenAspect) / swipeController.screenScale;
            if (throwState != ThrowState.TOUCHED /* && touchX > 0.5f - 0.25f * screenScale && touchX < 0.5f + 0.25f * screenScale && touchY > 1.0f - 0.45f * screenScale && touchY < 1.0f */ && v2.magnitude < 0.5f)
            {
                throwState = ThrowState.TOUCHED;
                /*
                swipeTrail = ((GameObject)GameObject.Instantiate(swipeTrailPrefab, position, Quaternion.identity)).GetComponent<SwipeTrailController>();
                swipeTrail.transform.parent = camera.transform;
                swipeTrail.transform.localPosition = Vector3.zero;
                */
            }
            /*
            swipeTrail.lineRenderer.SetVertexCount(swipeTrail.pointsCount);
            swipeTrail.lineRenderer.SetPosition(swipeTrail.pointsCount - 1, position);
            swipeTrail.pointsCount++;
            */
            lastTouchX = touchX;
            lastTouchY = touchY;
            if (throwState == ThrowState.TOUCHED)
            {
                armedMissile.SetAnchor(new Vector2(lastTouchX, 1.0f - lastTouchY), touchTime);
            }
        }
        else
        {
            if (throwState == ThrowState.TOUCHED)
            {
                throwState = ThrowState.NONE;
                armedMissile.ResetAnchor();
            }
        }
#else
        if (Input.GetMouseButtonDown(0))
        {
            lastTouchX = mouseX;
            lastTouchY = mouseY;
            position = (camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.1f)) - camera.transform.position * 0.0001f);
            v2.x = (lastTouchX - 0.5f) / swipeController.screenScale;
            v2.y = Mathf.Max(0.0f, (1.0f - (lastTouchY + 0.1f)) / swipeController.screenAspect) / swipeController.screenScale;
            if (throwState != ThrowState.TOUCHED && v2.magnitude < 0.5f)
            {
                /*
                swipeTrail = ((GameObject)GameObject.Instantiate(swipeTrailPrefab, position, Quaternion.identity)).GetComponent<SwipeTrailController>();
                swipeTrail.startColor = new Color(0.5f, 0.5f, 0.5f);
                swipeTrail.endColor = new Color(0.5f, 0.5f, 0.5f);
                swipeTrail.transform.parent = camera.transform;
                swipeTrail.transform.localPosition = Vector3.zero;
                swipeTrail.pointsCount = 1;
                swipeTrail.lineRenderer.SetVertexCount(swipeTrail.pointsCount);
                swipeTrail.lineRenderer.SetPosition(swipeTrail.pointsCount - 1, position);
                */
                //if (mouseX > 0.25f && mouseX < 0.75f && mouseY > 1.0f - 0.27f && mouseY < 1.0f)
                //{
                    throwState = ThrowState.TOUCHED;
                //}
            }
        }
        if( Input.GetMouseButton(0))
        {
            lastTouchX = mouseX;
            lastTouchY = mouseY;
            if (swipeTrail != null)
            {
                /*
                position = (camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.1f)) - camera.transform.position * 0.0001f);
                swipeTrail.pointsCount++;
                swipeTrail.lineRenderer.SetVertexCount(swipeTrail.pointsCount);
                swipeTrail.lineRenderer.SetPosition(swipeTrail.pointsCount - 1, position);
                */
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (throwState == ThrowState.TOUCHED)
            {
                throwState = ThrowState.NONE;
                //if (mouseY < 0.75f)
                //{
                //}
            }
        }
        if (throwState == ThrowState.TOUCHED)
        {
            armedMissile.SetAnchor(new Vector2(lastTouchX, 1.0f - lastTouchY), touchTime);
        }
#endif
        swipeController.AddPoint(new Vector2(lastTouchX, lastTouchY), Time.deltaTime, throwState == ThrowState.TOUCHED);
        if (throwState == ThrowState.TOUCHED)
        {
            touchTime += Time.deltaTime;
            //armedMissile.transform.localRotation = Quaternion.AngleAxis(Mathf.Min(60.0f, transform.localRotation.eulerAngles.x + touchTime * 180.0f), Vector3.right);
            armedMissile.transform.localRotation = Quaternion.AngleAxis(Mathf.Min(15.0f, transform.localRotation.eulerAngles.x + touchTime * 180.0f), Vector3.right);
        }
        else
        {
            if (touchTime > 0.0f)
            {
                touchTime = 0.0f;
                armedMissile.Rearm();
            }
        }
    }

    public void OnThrow(object sender, SwipeEventArgs e)
    {
        if (e.throwing)
        {
            DrawSwipe();
            Throw(e.angle, e.torsion, e.speed);
            throwState = ThrowState.NONE;
        }
    }

    public void DrawSwipe()
    {
        int i;
        Vector3 position;
        LinkedListNode<SwipePoint> pointNode;
        SwipeTrailController swipeTrail;
        swipeTrail = ((GameObject)GameObject.Instantiate(swipeTrailPrefab, camera.transform.position, Quaternion.identity)).GetComponent<SwipeTrailController>();
        if (swipeController.correct)
        {
            swipeTrail.startColor = new Color(0.2f, 1.0f, 0.2f);
            swipeTrail.endColor = new Color(0.2f, 1.0f, 0.2f);
        }
        else
        {
            swipeTrail.startColor = new Color(1.0f, 0.0f, 0.0f);
            swipeTrail.endColor = new Color(1.0f, 0.0f, 0.0f);
        }
        swipeTrail.transform.parent = camera.transform;
        swipeTrail.transform.localPosition = Vector3.zero;
        swipeTrail.pointsCount = swipeController.correctPointsList.Count;
        swipeTrail.lineRenderer.SetVertexCount(swipeTrail.pointsCount);
        i = 0;
        pointNode = swipeController.correctPointsList.First;
        while(pointNode != null)
        {
            position = camera.ViewportToWorldPoint(new Vector3(pointNode.Value.point.x, 1.0f - pointNode.Value.point.y, 0.11f));
            swipeTrail.lineRenderer.SetPosition(i, position);
            i++;
            pointNode = pointNode.Next;
        }

    }

    public bool Throw(Vector2 angle, float torsion, float speed)
    {
        int i;
        Vector3 position;
        Vector3 acceleration;
        Vector3 velocity;
        Vector3 _torsion;
        float zTorsionMin = -60.0f;
        float zTorsionMax = 60.0f;

        float distance = 50.0f;

        if (float.IsNaN(torsion) || Mathf.Abs(torsion) < 0.001f)
        {
            torsion = 0.0f;
        }

        float gravity = -0.98f * 3.0f;

        MissileController missileController;
        float trimmedSpeed = Mathf.Min(1.0f, Mathf.Max(1.0f, speed)) * distance * 1.8f;
        float realLength = 1.0f - Mathf.Abs(camera.transform.position.z - armedMissile.transform.position.z) / distance;
        float horizontalAngle = angle.x; //Mathf.Min(10.0f, Mathf.Max(-10.0f, angle.x));
        if(float.IsNaN(horizontalAngle))
        {
            horizontalAngle = 0.0f;
        }
        float t = distance * realLength / trimmedSpeed;

        if(swipeController.swipeType == 1)
        {
            trimmedSpeed = distance * 2.0f;
            gravity = -0.98f * 2.0f;
        }
        else if(swipeController.swipeType == 3)
        {
            trimmedSpeed = distance * 1.6f;
            gravity = -0.98f * 4.0f;
        }

        missileController = (Instantiate(missilePrefab)).GetComponent<MissileController>();
        missileController.name = missilePrefab.name;
        missileController.taskObject = taskObject;
        position = armedMissile.transform.position;
        if (angle.y < 0.0f)
        {
            angle.y = 1.0f;
            t *= 0.5f;
            acceleration = new Vector3(0.0f, (angle.y - 0.5f) * 2.0f * gravity * 2.0f, 0.0f);
            velocity = new Vector3(0.0f, 0.44f, trimmedSpeed * 0.05f + Mathf.Abs(horizontalAngle) / 22.0f * 0.1f); // !!! not trigonometrical coeficient
            velocity = Quaternion.Euler(0.0f, horizontalAngle + (1.0f + position.z / Mathf.Abs(position.z)) * 90.0f, 0.0f) * velocity;
            _torsion = new Vector3(360.0f, Mathf.Min(90.0f, Mathf.Max(-90.0f, torsion)) - (angle.x - horizontalAngle) * 5.0f, UnityEngine.Random.Range(-180.0f, 180.0f));
        }
        else
        {
            acceleration = new Vector3(torsion * trimmedSpeed, (angle.y - 0.4f) * 4.0f * gravity, 0.0f);
            //velocity = new Vector3(-acceleration.x / 2, Mathf.Min(/*0.044f*/ 0.12f * 50.0f, Mathf.Max(/*-0.176f*/ -0.13fs * 50.0f, (angle.y - 0.63f) * /* 0.22f */ 0.26f * 50.0f)) - acceleration.y / 2 / t, trimmedSpeed * (1.0f + Mathf.Abs(horizontalAngle) * 0.01f)); // !!! not trigonometrical coeficient
            velocity = new Vector3(-acceleration.x / 2, Mathf.Min(0.044f * 50.0f, Mathf.Max(-0.174f * 50.0f, (angle.y - 0.63f) * 0.22f * 50.0f)) - acceleration.y / 2 * t, trimmedSpeed); // !!! not trigonometrical coeficient
            velocity = Quaternion.Euler(0.0f, horizontalAngle + (1.0f + position.z / Mathf.Abs(position.z)) * 90.0f, 0.0f) * velocity;
            if (torsion > 0.0f)
            {
                zTorsionMax = 0.0f;
            }
            else if(torsion < 0.0f)
            {
                zTorsionMin = 0.0f;
            }
            _torsion = new Vector3(0.0f, Mathf.Min(90.0f, Mathf.Max(-90.0f, torsion)) - (angle.x - horizontalAngle) * 5.0f, UnityEngine.Random.Range(zTorsionMin, zTorsionMax));
            _torsion.z /= Mathf.Max(1.0f, torsion);
        }
        Vector3 intersectionPosition = position + velocity * t + acceleration * Mathf.Pow(t, 2.0f) / 2.0f;
        //RaycastHit[] hits = Physics.SphereCastAll(new Ray(intersectionPosition - new Vector3(0.0f, 0.0f, 5.0f), new Vector3(0.0f, 0.0f, 1.0f)), 1.7f, 10.0f, 255, QueryTriggerInteraction.Collide);
        RaycastHit[] hits = Physics.CapsuleCastAll(intersectionPosition - new Vector3(0.6f, 0.0f, 0.0f) - new Vector3(0.0f, 0.0f, 5.0f), intersectionPosition + new Vector3(0.6f, 0.0f, 0.0f) - new Vector3(0.0f, 0.0f, 5.0f), 0.5f, new Vector3(0.0f, 0.0f, 1.0f), 10.0f, 255, QueryTriggerInteraction.Collide);
        RaycastHit hit;
        Vector3 deltaVelocity = Vector3.zero;
        float lastDistance = 1000.0f;
        for(i = 0; i < hits.Length; i++)
        {
            hit = hits[i];
            if (hit.collider.name != "Ground" && hit.collider.name != "MissileObject" && hit.point.magnitude > 0.01f && (intersectionPosition - hit.point).magnitude < lastDistance)
            {
                Vector3 delta = hit.point - intersectionPosition;
                lastDistance = delta.magnitude;
                deltaVelocity.x = delta.x / t;
                deltaVelocity.y = delta.y / t;
            }
        }
        velocity.x += deltaVelocity.x;
        velocity.y += deltaVelocity.y;


        position += velocity * Time.deltaTime;
        armedMissile.Rearm();
        missileController.transform.Rotate(70.0f, 0.0f, 0.0f);
        missileController.transform.position = position;
        missileController.acceleration = acceleration;
        missileController.velocity = velocity;
        missileController.torsion = _torsion;
        //GameObject.Destroy(missileController.gameObject, 2.0f);
        return true;
    }
    

}
