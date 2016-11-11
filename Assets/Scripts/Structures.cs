using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.Networking.Match;
using UnityEngine.Serialization;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class Structures : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}


public class SwipeEventArgs : EventArgs
{
    public Vector2 angle;
    public float torsion;
    public float speed;
    public bool throwing = false;
}

public class SwipePoint
{
    public Vector2 point;
    public float duration;

    public SwipePoint(Vector2 newPoint, float newDuration)
    {
        point = newPoint;
        duration = newDuration;
    }

}

public class SwipeController
{

    public float screenScale = 1.0f;
    public float screenAspect = 1.0f;
    public float minLength = 0.025f;
    public float minY = 0.22f; // 0.27f
    public float maxY = 0.5f;
    public int swipeType = 2;

    public bool active = true;
    public bool started = false;
    public bool locked = false;
    public bool correct = false;
    public LinkedList<SwipePoint> pointsList = new LinkedList<SwipePoint>();
    public LinkedList<SwipePoint> correctPointsList = new LinkedList<SwipePoint>();

    private Vector2 direction = Vector2.zero;

    public SwipeController()
    {
    }

    public void AddPoint(Vector2 newPoint, float newDuration, bool touched)
    {
        if (active)
        {
            int i = 0;
            float length = 0.0f;
            float duration = 0.0f;
            float duration2 = 0.0f;
            int startPoint = 0;
            int endPoint = 0;
            /*
            float beginX = 0.0f;
            float beginY = 0.0f;
            */
            float f;
            float endX = 0.0f;
            float endY = 0.0f;
            float fullX = 0.0f;
            float fullY = 0.0f;
            int startPointCount = 0;
            float startPointX = 0.0f;
            float startPointY = 0.0f;
            int endPointCount = 0;
            float endPointX = 0.0f;
            float endPointY = 0.0f;
            float firstHalfLength = 0.0f;
            float firstHalfDuration = 0.0f;
            float firstHalfSpeed = 0.0f;
            float secondHalfLength = 0.0f;
            float secondHalfDuration = 0.0f;
            float secondHalfSpeed = 0.0f;
            Vector2 v2Delta = Vector2.zero;
            Vector2 v2Delta2 = Vector2.zero;
            Vector2 v2Average = Vector2.zero;
            Vector2 v2StartPoint = Vector2.zero;
            LinkedListNode<SwipePoint> prevPointNode = null;
            LinkedListNode<SwipePoint> pointNode = null;
            LinkedListNode<SwipePoint> pointNodePrev = null;
            LinkedListNode<SwipePoint> pointNodeNext = null;
            LinkedListNode<SwipePoint> pointNodeNextNext = null;
            SwipePoint firstQuarterCorrectPoint = null;
            SwipePoint middleCorrectPoint = null;
            SwipePoint thirdQuarterCorrectPoint = null;
            SwipeEventArgs eventArgs;
            if (!started && !touched)
            {
                locked = false;
                return;
            }
            v2Delta.x = (newPoint.x - 0.5f) * screenScale;
            v2Delta.y = Mathf.Max(0.0f, (1.0f - (newPoint.y + 0.1f)) / screenAspect) * screenScale;
            if (started || (touched /* && newPoint.x > 0.5f - 0.25f * screenScale && newPoint.x < 0.5f + 0.25f * screenScale && newPoint.y > 1.0f - minY * screenScale */ && v2Delta.magnitude < 0.5f))
            {
                if (!started)
                {
                    started = true;
                }
                pointsList.AddLast(new SwipePoint(newPoint, newDuration));
                if (pointsList.Last.Previous != null && pointsList.Last.Previous.Previous != null)
                {
                    if (pointsList.Last.Value.point.y < pointsList.Last.Previous.Value.point.y)
                    {
                        pointsList.Last.Previous.Value.point = (pointsList.Last.Value.point + pointsList.Last.Previous.Previous.Value.point) * 0.5f;
                    }
                }
            }
            /*
            pointNode = pointsList.First;
            while (pointNode != null)
            {
                pointNodeNext = pointNode.Next;
                if (prevPointNode != null)
                {
                    v2Delta = pointNode.Value.point - prevPointNode.Value.point;
                }
                else
                {
                    v2Delta = Vector2.zero;
                }
                v2Delta.y *= -1.0f;
                if (v2Delta.y > 0.0f)
                {
                    length += v2Delta.magnitude;
                    duration += pointNode.Value.duration;
                    // Конец траектории считаем по последней четверти свайпа
                    if (i >= pointsList.Count * 3 / 4)
                    {
                        endX += v2Delta.x;
                        endY += v2Delta.y;
                    }
                    // Исключаем последнюю точку из общей траектории свайпа
                    if (i < pointsList.Count - 1)
                    {
                        fullX += v2Delta.x;
                        fullY += v2Delta.y;
                        if (swipeType == 1 || swipeType == 3)
                        {
                            if (i <= pointsList.Count / 3)
                            {
                                startPointCount++;
                                startPointX += pointNode.Value.point.x;
                                startPointY += pointNode.Value.point.y;
                            }
                            else if (i >= pointsList.Count * 2 / 3)
                            {
                                endPointCount++;
                                endPointX += pointNode.Value.point.x;
                                endPointY += pointNode.Value.point.y;
                            }
                        }
                        if (swipeType == 3)
                        {
                            if (i < pointsList.Count / 2)
                            {
                                firstHalfLength += v2Delta.magnitude;
                                firstHalfDuration += pointNode.Value.duration;
                            }
                            else
                            {
                                secondHalfLength += v2Delta.magnitude;
                                secondHalfDuration += pointNode.Value.duration;
                            }
                        }
                    }
                }
                if (v2Delta.y < 0.0f && locked)
                {
                    pointsList.Clear();
                    locked = false;
                    started = false;
                    pointNodeNext = null;
                }
                i++;
                prevPointNode = pointNode;
                pointNode = pointNodeNext;
            }
            if(swipeType == 3)
            {
                firstHalfSpeed = firstHalfLength / firstHalfDuration;
                secondHalfSpeed = secondHalfLength / secondHalfDuration;
                if(firstHalfSpeed <= 0.0f)
                {
                    firstHalfSpeed = 0.0001f;
                }
                if (secondHalfSpeed <= 0.0f)
                {
                    secondHalfSpeed = 0.0001f;
                }
            }
            if (swipeType == 1 || swipeType == 3)
            {
                if (startPointCount > 0 && endPointCount > 0)
                {
                    fullX = endPointX / (float)endPointCount - startPointX / (float)startPointCount;
                    fullY = (endPointY / (float)endPointCount - startPointY / (float)startPointCount) * -1.0f;
                }
                else
                {
                    fullX = 0.0f;
                    fullY = 0.0f;
                }
            }
            */



            if (started && !locked && pointsList.Count > 2)
            {
                /*
                if (Mathf.Abs(fullX) > Mathf.Abs(fullY))
                {
                    fullX = fullX / Mathf.Abs(fullX) * Mathf.Abs(fullY);
                }
                v2Delta.x = fullX;
                v2Delta.y = fullY;
                v2Delta.Normalize();
                eventArgs = new SwipeEventArgs();
                if (Mathf.Abs(v2Delta.x) * 4.0f > Mathf.Abs(v2Delta.y))
                {
                    eventArgs.torsion = -v2Delta.x / Mathf.Abs(v2Delta.x) * 90.0f * (Mathf.Abs(v2Delta.x) * 4.0f - Mathf.Abs(v2Delta.y)) / 2.0f; // 2.0f - Высчитать коефициент в зависимости от времени полета (обратно пропорционально скорости) и угла между 30 и 45 градусами отклонения
                }
                float dxSign = v2Delta.x / Mathf.Abs(v2Delta.x);
                eventArgs.angle.x = Mathf.Atan(v2Delta.x / v2Delta.y);
                if (Mathf.Abs(eventArgs.angle.x) > 0.001f)
                {
                    eventArgs.angle.x = Mathf.Pow(Mathf.Abs(v2Delta.x), 1.75f) * dxSign;
                }
                eventArgs.angle.x *= 180.0f / Mathf.PI;
                eventArgs.angle.y = length / maxLength;
                if(swipeType == 3)
                {
                    eventArgs.angle.y = Mathf.Min(1.0f, Mathf.Max(0.0f, (firstHalfSpeed / (firstHalfSpeed + secondHalfSpeed) - 0.5f) * 2.0f + 1.0f));
                }
                eventArgs.speed = Mathf.Sqrt(0.2f / duration);
                */

                /******/

                eventArgs = new SwipeEventArgs();
                correctPointsList.Clear();
                pointNode = pointsList.First;
                while (pointNode != null)
                {
                    pointNodeNext = pointNode.Next;
                    if (prevPointNode != null)
                    {
                        v2Delta = pointNode.Value.point - prevPointNode.Value.point;
                    }
                    else
                    {
                        v2Delta = Vector2.zero;
                    }
                    v2Delta.y *= -1.0f;
                    if (v2Delta.y > 0.0f)
                    {
                        length += v2Delta.magnitude;
                        duration += pointNode.Value.duration;
                    }
                    v2Average += v2Delta * pointNode.Value.duration;
                    prevPointNode = pointNode;
                    pointNode = pointNodeNext;
                }
                v2Average.Normalize();
                startPoint = 0;
                endPoint = pointsList.Count;
                i = 0;
                pointNode = pointsList.First;
                while (pointNode != null)
                {
                    pointNodeNext = pointNode.Next;
                    if (prevPointNode != null)
                    {
                        v2Delta = pointNode.Value.point - prevPointNode.Value.point;
                    }
                    else
                    {
                        v2Delta = Vector2.zero;
                    }
                    v2Delta.y *= -1.0f;
                    if (v2Delta.y > 0.0f)
                    {
                        duration2 += pointNode.Value.duration;
                    }
                    if(Vector2.Angle(v2Delta, v2Average) > 90)
                    {
                        if(duration2 < duration * 0.5f)
                        {
                            startPoint = i;
                        }
                        else if(endPoint == pointsList.Count)
                        {
                            endPoint = i;
                        }
                    }
                    i++;
                    prevPointNode = pointNode;
                    pointNode = pointNodeNext;
                }
                i = 0;
                pointNode = pointsList.First;
                while (pointNode != null)
                {
                    pointNodeNext = pointNode.Next;
                    if (i >= startPoint && i < endPoint)
                    {
                        correctPointsList.AddLast(pointNode.Value);
                        //if (swipeType == 3 || swipeType == 4)
                        //{
                        if (pointNodeNext != null)
                        {
                            pointNodePrev = pointNode.Previous;
                            pointNodeNextNext = pointNodeNext.Next;
                            pointNode = correctPointsList.Last;
                            if (pointNodePrev != null && pointNodeNextNext != null)
                            {
                                f = Mathf.Max(0.0f, Mathf.Min(1.0f, Vector2.Angle(pointNode.Value.point - pointNodePrev.Value.point, pointNodeNextNext.Value.point - pointNodeNext.Value.point) / 90.0f));
                                pointNode.Value.point = pointNode.Value.point * f + (pointNodePrev.Value.point + pointNodeNext.Value.point) * 0.5f * (1.0f - f);
                            }
                        }
                        //}
                    }
                    i++;
                    pointNode = pointNodeNext;
                }
                if (correctPointsList.Count > 1)
                {
                    //if (swipeType == 2 || swipeType == 4)
                    //{
                    v2Average = correctPointsList.Last.Value.point - correctPointsList.First.Value.point;
                    v2Delta = v2Average;
                    v2Delta.Normalize();
                    v2Delta *= 0.01f;
                    f = 1.0f;
                    i = 0;
                    pointNode = pointsList.Last;
                    for (i = 0; i < pointsList.Count - endPoint; i++)
                    {
                        pointNodePrev = pointNode.Previous;
                        if (pointNodePrev != null)
                        {
                            v2Delta += pointNode.Value.point - pointNodePrev.Value.point;
                        }
                        pointNode = pointNodePrev;
                        if (pointNode == null)
                        {
                            i = pointsList.Count;
                        }
                    }
                    v2Average.Normalize();
                    v2Delta.Normalize();
                    v2StartPoint = correctPointsList.First.Value.point;
                    f = 1.0f - Mathf.Max(-100.0f, Mathf.Min(180.0f, Vector2.Angle(v2Average, v2Delta) / 90.0f));
                    /*
                    Debug.Log("Parabola: " + f);
                    if(swipeType > 1)
                    {
                        if(f > 0.6f)
                        {
                            f = 1.0f;
                        }
                    }
                    */
                    pointNode = correctPointsList.First;
                    while (pointNode != null)
                    {
                        pointNodeNext = pointNode.Next;
                        pointNode.Value.point = pointNode.Value.point * f + (v2StartPoint + v2Average.normalized * (pointNode.Value.point - v2StartPoint).magnitude) * (1.0f - f);
                        pointNode = pointNodeNext;
                    }
                    //}
                    i = 0;
                    v2Average *= 0.0f;
                    duration2 = 0.0f;
                    pointNode = correctPointsList.First;
                    while (pointNode != null)
                    {
                        pointNodeNext = pointNode.Next;
                        if (prevPointNode != null)
                        {
                            v2Delta = pointNode.Value.point - prevPointNode.Value.point;
                        }
                        else
                        {
                            v2Delta = Vector2.zero;
                        }
                        v2Delta.y *= -1.0f;
                        v2Average += v2Delta * pointNode.Value.duration;
                        duration2 += pointNode.Value.duration;
                        if (i <= correctPointsList.Count / 2 && i + 1 > correctPointsList.Count / 2)
                        {
                            middleCorrectPoint = pointNode.Value;
                        }
                        if (i <= correctPointsList.Count / 4 && i + 1 > correctPointsList.Count / 4)
                        {
                            firstQuarterCorrectPoint = pointNode.Value;
                        }
                        if (i <= correctPointsList.Count * 3 / 4 && i + 1 > correctPointsList.Count * 3 / 4)
                        {
                            thirdQuarterCorrectPoint = pointNode.Value;
                        }
                        i++;
                        prevPointNode = pointNode;
                        pointNode = pointNodeNext;
                    }
                    v2Average.Normalize();

                    correct = true;
                    /*
                    if(correctPointsList.Last.Value.point.y > 1.0f - minY)
                    {
                        correct = false;
                    }
                    */
                    /*
                    if (correctPointsList.First.Value.point.y < 1.0f - minY)
                    {
                        correct = false;
                    }
                    */
                    if (duration2 > 0.5f)
                    {
                        correct = false;
                    }
                    v2Delta = correctPointsList.Last.Value.point - correctPointsList.First.Value.point;
                    float firstHalfCurvature = Vector3.Angle(firstQuarterCorrectPoint.point - correctPointsList.First.Value.point, middleCorrectPoint.point - firstQuarterCorrectPoint.point);
                    float secondHalfCurvature = Vector3.Angle(correctPointsList.Last.Value.point - thirdQuarterCorrectPoint.point, thirdQuarterCorrectPoint.point - middleCorrectPoint.point);
                    float curvature = Mathf.Max(0.0f, Mathf.Min(1.0f, (firstHalfCurvature - secondHalfCurvature) / firstHalfCurvature));
                    v2Delta = v2Delta * (1.0f - curvature) + (middleCorrectPoint.point - correctPointsList.First.Value.point + (thirdQuarterCorrectPoint.point - middleCorrectPoint.point).normalized * (correctPointsList.Last.Value.point - middleCorrectPoint.point).magnitude) * curvature;
                    v2Delta.Normalize();
                    //if (Mathf.Abs(v2Delta.x) > Mathf.Abs(v2Delta.y))
                    //{
                    //    v2Delta.x = v2Delta.x / Mathf.Abs(v2Delta.x) * Mathf.Abs(v2Delta.x);
                    //}
                    float dxSign = v2Delta.x / Mathf.Abs(v2Delta.x);
                    eventArgs.angle.x = -Mathf.Atan(v2Delta.x / (v2Delta.y * 4.0f)) / Mathf.PI;
                    if (Mathf.Abs(eventArgs.angle.x) > 0.0001f)
                    {
                        //eventArgs.angle.x = Mathf.Pow(Mathf.Abs(eventArgs.angle.x * 1.0f), 1.8f) * dxSign * 1.0f;
                    }
                    eventArgs.angle.x *= 180.0f;
                    eventArgs.angle.x += (correctPointsList.First.Value.point.x - 0.5f) * 18.0f;
                    eventArgs.angle.y = Mathf.Min(1.0f, Mathf.Max(0.0f, (1.0f - correctPointsList.Last.Value.point.y) - minY * screenScale) / (maxY - minY) * screenScale);
                    if(eventArgs.angle.y < 0.45f)
                    {
                        if (eventArgs.angle.y < 0.1f)
                        {
                            eventArgs.angle.y = 0.1f;
                        }
                        else if (eventArgs.angle.y < 0.25f)
                        {
                            eventArgs.angle.y = 0.25f;
                        }
                        else
                        {
                            eventArgs.angle.y = 0.45f;
                        }
                    }
                    else if (eventArgs.angle.y > 0.99f)
                    {
                        eventArgs.angle.y = 1.0f;
                    }
                    else
                    {
                        if (eventArgs.angle.y < 0.75f)
                        {
                            eventArgs.angle.y = 0.65f;
                        }
                        else
                        {
                            eventArgs.angle.y = 0.8f;
                        }
                    }
                    if (!correct)
                    {
                        eventArgs.angle.y = -1.0f;
                    }
                    //eventArgs.angle.y = 0.01f;
                    v2Delta2 = middleCorrectPoint.point - correctPointsList.First.Value.point;
                    v2Delta.Normalize();
                    v2Delta2.Normalize();
                    if (Vector2.Angle(v2Delta, v2Delta2) > 15.0f)
                    {
                        eventArgs.torsion = Vector2.Angle(v2Delta, v2Delta2) / 180.0f * (v2Delta.x - v2Delta2.x) / Mathf.Abs(v2Delta.x - v2Delta2.x);
                    }
                    else
                    {
                        eventArgs.torsion = 0.0f;
                    }
                    eventArgs.speed = Mathf.Sqrt(0.2f / duration);
                    if (length > minLength * screenScale)
                    {
                        if (!touched || pointsList.Last.Value.point.y < 1.0f - maxY /* || pointsList.Last.Previous.Value.point.y - newPoint.y < 0.0f*/)
                        {
                            eventArgs.throwing = true;
                            InvokeAction(eventArgs);
                            touched = false;
                            started = true;
                            locked = true;
                        }
                        else if (touched)
                        {
                            eventArgs.throwing = false;
                            InvokeAction(eventArgs);
                        }
                    }
                    if(correctPointsList.Count < pointsList.Count * 0.75f && newPoint.y > 1.0f - minY * screenScale)
                    {
                        pointsList.Clear();
                        started = false;
                    }
                }
            }
            if ((!touched && started) || (pointsList.Count > 1 && (pointsList.Last.Previous.Value.point - newPoint).y < 0.0f))
            {
                pointsList.Clear();
                started = false;
            }
        }
    }

    public event EventHandler<SwipeEventArgs> OnInvokeAction;

    public void InvokeAction(SwipeEventArgs e)
    {
        EventHandler<SwipeEventArgs> handler = OnInvokeAction;
        if (handler != null)
        {
            handler(this, e);
        }
    }

}
