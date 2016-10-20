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
            screenScale = Mathf.Min(1.0f, Mathf.Max(0.75f, 4.0f / (Screen.height / Screen.dpi)));
            swipeController.screenScale = screenScale;
        }

        swipeType2.onClick.AddListener(delegate() {
            if (swipeType2.image.color == Color.green)
            {
                swipeType2.image.color = Color.red;
            }
            else
            {
                swipeType2.image.color = Color.green;
            }
            OnSwipeTypeChanged();
        });
        swipeType3.onClick.AddListener(delegate () {
            if (swipeType3.image.color == Color.green)
            {
                swipeType3.image.color = Color.red;
            }
            else
            {
                swipeType3.image.color = Color.green;
            }
            OnSwipeTypeChanged();
        });
        OnSwipeTypeChanged();

    }

    public void OnSwipeTypeChanged()
    {
        int swipeType = 1;
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
        swipeController.swipeType = swipeType;
    }

    private Vector3 normalCameraPosition = new Vector3(0.0f, 5.0f, -25.0f);

    void Update ()
    {

        if(!touchedOnce && Input.touchCount > 0)
        {
            armedMissile.Rearm();
            touchedOnce = true;
        }
        if (touchedOnce && (transform.position - normalCameraPosition).magnitude > 0.01f)
        {
            transform.position += (normalCameraPosition - transform.position) * Time.deltaTime * 5.0f;
            if ((normalCameraPosition - transform.position).magnitude < 1.0f)
            {
                transform.position = normalCameraPosition;
            }
        }

        float angle = 0.0f;
        float power = 0.0f;
        float mouseX = Input.mousePosition.x / (float)Screen.width;
        float mouseY = 1.0f - Input.mousePosition.y / (float)Screen.height;
        float touchX = 0.0f;
        float touchY = 0.0f;
        Vector3 position = Vector3.zero;
#if !UNITY_STANDALONE
        Touch touch;
        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);
            touchX = touch.position.x / (float)Screen.width;
            touchY = 1.0f - touch.position.y / (float)Screen.height;
            position = (camera.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, 0.5f)) - camera.transform.position);
            if (throwState != ThrowState.TOUCHED && touchX > 0.5f - 0.25f * screenScale && touchX < 0.5f + 0.25f * screenScale && touchY > 1.0f - 0.22f * screenScale && touchY < 1.0f)
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
            if (throwState != ThrowState.TOUCHED)
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
                //if (mouseX > 0.25f && mouseX < 0.75f && mouseY > 0.8f && mouseY < 1.0f)
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
            armedMissile.transform.localRotation = Quaternion.AngleAxis(Mathf.Min(60.0f, transform.localRotation.eulerAngles.x + touchTime * 180.0f), Vector3.right);
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
        Vector3 position;
        Vector3 acceleration;
        Vector3 velocity;
        Vector3 _torsion;

        float gravity = -0.98f;

        MissileController missileController;
        float trimmedSpeed = Mathf.Min(1.0f, Mathf.Max(1.0f, speed)) * 50.0f;
        float horizontalAngle = Mathf.Min(10.0f, Mathf.Max(-10.0f, angle.x));
        float t = 1.0f / trimmedSpeed;
        missileController = (Instantiate(missilePrefab)).GetComponent<MissileController>();
        missileController.taskObject = taskObject;
        if (angle.y < 0.0f)
        {
            angle.y = 1.0f;
            t *= 0.5f;
            position = armedMissile.transform.position;
            acceleration = new Vector3(0.0f, (angle.y - 0.5f) * 2.0f * gravity * 2.0f, 0.0f);
            velocity = new Vector3(0.0f, Mathf.Min(0.44f, 0.44f), trimmedSpeed * 0.05f + Mathf.Abs(horizontalAngle) / 22.0f * 0.1f); // !!! not trigonometrical coeficient
            velocity = Quaternion.Euler(0.0f, horizontalAngle + (1.0f + position.z / Mathf.Abs(position.z)) * 90.0f, 0.0f) * velocity;
            _torsion = new Vector3(360.0f, Mathf.Min(90.0f, Mathf.Max(-90.0f, torsion)) - (angle.x - horizontalAngle) * 5.0f, 0.0f);
        }
        else
        {
            position = armedMissile.transform.position;
            acceleration = new Vector3(torsion * trimmedSpeed, (angle.y - 0.5f) * 2.0f * gravity, 0.0f);
            velocity = new Vector3(-acceleration.x / 2, Mathf.Min(0.044f * 30.0f, Mathf.Max(-0.176f * 30.0f, (angle.y - 0.8f) * 0.22f * 30.0f)) - acceleration.y / 2, trimmedSpeed * (1.0f + Mathf.Abs(horizontalAngle) / 10.0f * 0.1f)); // !!! not trigonometrical coeficient
            velocity = Quaternion.Euler(0.0f, horizontalAngle + (1.0f + position.z / Mathf.Abs(position.z)) * 90.0f, 0.0f) * velocity;
            _torsion = new Vector3(0.0f, Mathf.Min(90.0f, Mathf.Max(-90.0f, torsion)) - (angle.x - horizontalAngle) * 5.0f, 0.0f);
        }
        armedMissile.Rearm();
        missileController.transform.Rotate(70.0f, 0.0f, 0.0f);
        missileController.transform.position = position;
        missileController.acceleration = acceleration;
        missileController.velocity = velocity;
        missileController.torsion = _torsion;
        GameObject.Destroy(missileController.gameObject, 2.0f);
        return true;
    }
    

}
