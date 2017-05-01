using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.Networking.Match;
using UnityEngine.Serialization;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

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


public class RegionMap
{
    public float scale = 2.0f / 6.4f;
    public Vector2 size = new Vector2(64.0f, 64.0f);
    public RegionMapNode nwNode = null;
    public LinkedList<RegionBarrier> barriers = new LinkedList<RegionBarrier>();

    public void Load(string name)
    {
        int i, j;
        int sizeX, sizeY;
        RegionMapNode currentNode;
        RegionMapNode newNode;
        RegionMapNode northNode;
        sizeX = (int)size.x;
        sizeY = (int)size.y;
        TextAsset asset = Resources.Load<TextAsset>("Maps/" + name);
        byte[] data = asset.bytes;
        i = 0;
        j = 0;
        nwNode = new RegionMapNode();
        RegionMapNode firstRowNode = nwNode;
        currentNode = firstRowNode;
        while (j < sizeY)
        {
            currentNode.position.x = ((float)i - ((float)sizeX) / 2.0f) * scale + scale * 0.5f;
            currentNode.position.y = -((float)j - ((float)sizeY) / 2.0f) * scale - scale * 0.5f;
            currentNode.coverageType = (int)data[j * sizeX + i];
            i++;
            while (i < sizeX)
            {
                newNode = new RegionMapNode();
                newNode.west = currentNode;
                currentNode.east = newNode;
                if (currentNode.north != null && currentNode.north.east != null)
                {
                    northNode = currentNode.north.east;
                    northNode.south = newNode;
                    newNode.north = northNode;
                }
                currentNode = newNode;
                currentNode.position.x = ((float)i - ((float)sizeX) / 2.0f) * scale + scale * 0.5f;
                currentNode.position.y = -((float)j - ((float)sizeY) / 2.0f) * scale - scale * 0.5f;
                currentNode.coverageType = (int)data[j * sizeX + i];
                i++;
            }
            if (j < sizeY - 1)
            {
                firstRowNode.south = new RegionMapNode();
                firstRowNode.south.north = firstRowNode;
                firstRowNode = firstRowNode.south;
                currentNode = firstRowNode;

            }
            i = 0;
            j++;
        }


        /*
        RegionBarrier barrier;

        barrier = new RegionBarrier();
        barrier.start = new Vector2(-0.9f, 2.58f);
        barrier.end = new Vector2(-1.1f, 4.83f);
        barriers.AddLast(barrier);

        barrier = new RegionBarrier();
        barrier.start = new Vector2(0.55f, 5.14f);
        barrier.end = new Vector2(0.71f, 2.7f);
        barriers.AddLast(barrier);
        */

    }

    public RegionMapNode FindNode(float x, float y)
    {
        RegionMapNode node = nwNode;
        while(node != null && x > node.position.x)
        {
            node = node.east;
        }
        while (node != null && y < node.position.y)
        {
            node = node.south;
        }
        return node;
    }
}

public class RegionMapNode
{
    public int coverageType = 0;
    public Vector2 position = new Vector2();
    public RegionMapNode west = null;
    public RegionMapNode north = null;
    public RegionMapNode east = null;
    public RegionMapNode south = null;
    

}

public class RegionBarrier
{
    public bool twoSided = false;
    public Vector2 start = new Vector2();
    public Vector2 end = new Vector2();
}


















public class Location
{

    public enum ObjectType
    {
        NONE = 0,
        OBSTRUCTION = 1,
        PLAYER = 2,
        MISSILE = 3
    };

    public enum VisualEffects
    {
        NONE = 0,
        HIT = 1,
        SPARKS = 2,
        RED_SCREEN = 3,
        GREEN_SCREEN = 4,
        RAVEN = 1001
    };

    public static float gravity = -0.25f; //-0.098f;

    private DuelController network;
    private LinkedList<LocationObject> objects = new LinkedList<LocationObject>();

    public void SetNetworkBehavior(DuelController pointer)
    {
        network = pointer;
    }

    public void AddObject(ObstructionObject obj)
    {
        objects.AddLast(obj);
        if (network.isServer && !network.isLocal)
        {

            network.RpcSpawnObject(obj.id, obj.objectType, obj.position, obj.velocity, obj.acceleration, obj.torsion, obj.scale, obj.visualId);
        }
    }

    public void AddObject(PlayerObject obj)
    {
        objects.AddLast(obj);
        if (network.isServer && !network.isLocal)
        {
            network.RpcSpawnObject(obj.id, obj.objectType, obj.position, obj.velocity, obj.acceleration, obj.torsion, obj.scale, obj.visualId);
        }
        else
        {
        }
    }

    public void AddObject(MissileObject obj)
    {
        objects.AddLast(obj);
        if (network.isServer && !network.isLocal)
        {
            network.RpcSpawnObject(obj.id, obj.objectType, obj.position, obj.velocity, obj.acceleration, obj.torsion, obj.torsion.y, obj.visualId);
        }
    }

    public void RemoveObject(LocationObject obj)
    {
        if (network.isServer && !network.isLocal)
        {
            network.RpcDestroyObject(obj.id);
        }
        objects.Remove(obj);
    }

    public void UpdateCycle(float deltaTime)
    {
        LinkedListNode<LocationObject> objNode;
        LinkedListNode<LocationObject> objNodeNext;
        LinkedListNode<LocationObject> objNode2;
        LinkedListNode<LocationObject> objNodeNext2;
        LocationObject obj;
        ObstructionObject obstructionObject;
        PlayerObject playerObject;
        MissileObject missileObject;
        Vector3 v3Delta;
        float timestamp = Time.time;
        float scale = 0.0f;
        objNode = objects.First;
        while (objNode != null)
        {
            objNodeNext = objNode.Next;
            if (objNode.Value != null)
            {
                obj = objNode.Value;
                obj.velocity += obj.acceleration * deltaTime;
                obj.position += obj.velocity * deltaTime;
                switch (objNode.Value.objectType)
                {
                    case ObjectType.PLAYER:
                        playerObject = (PlayerObject)objNode.Value;
                        //Debug.Log("[" + playerObject.id + "] position: " + playerObject.position + " ; velocity: " + playerObject.velocity);
                        if (playerObject.visualObject != null)
                        {
                            /*
                            if (Mathf.Abs(network.camera.transform.position.x - playerObject.position.x * 10.0f) > 30.0f)
                            {
                                if (network.camera.transform.position.x - playerObject.position.x * 10.0f > 0.0f)
                                {
                                    playerObject.position += Vector3.right * 6.0f;
                                }
                                else
                                {
                                    playerObject.position += Vector3.right * -6.0f;
                                }
                            }
                            v3Delta = Vector3.right * (playerObject.position.x * 10.0f - playerObject.visualObject.transform.position.x);
                            if (Mathf.Abs(v3Delta.x) > 10.0f)
                            {
                                if (v3Delta.x > 0.0f)
                                {
                                    playerObject.visualObject.transform.position += Vector3.right * 6.0f * 10.0f;
                                }
                                else
                                {
                                    playerObject.visualObject.transform.position += Vector3.right * -6.0f * 10.0f;
                                }
                                v3Delta = Vector3.right * (playerObject.position.x * 10.0f - playerObject.visualObject.transform.position.x);
                            }
                            playerObject.visualObject.transform.position += v3Delta * Mathf.Min(1.0f, deltaTime * 15.0f);
                            */
                            playerObject.visualObject.transform.position = playerObject.position * 50.0f;
                            playerObject.visualObject.direction = playerObject.velocity.normalized.x;
                            /*
                            if ((playerObject.velocity.x > 0.0f && playerObject.position.z < 0.0f) || (playerObject.velocity.x < 0.0f && playerObject.position.z > 0.0f))
                            {
                                playerObject.visualObject.Animate(1);
                            }
                            else
                            {
                                playerObject.visualObject.Animate(0);
                            }
                            */
                        }
                        else
                        {
                            /*
                            v3Delta = Vector3.right * (playerObject.position.x * 10.0f - network.camera.transform.position.x);
                            if (Mathf.Abs(v3Delta.x) > 10.0f)
                            {
                                if (v3Delta.x > 0.0f)
                                {
                                    network.camera.transform.position += Vector3.right * 4.0f * 10.0f;
                                }
                                else
                                {
                                    network.camera.transform.position += Vector3.right * -4.0f * 10.0f;
                                }
                                v3Delta = Vector3.right * (playerObject.position.x * 10.0f - network.camera.transform.position.x);
                            }
                            network.camera.transform.position += v3Delta * Mathf.Min(1.0f, deltaTime * 15.0f);
                            */
                            network.camera.transform.position = playerObject.position * 50.0f + Vector3.up * 5.4f;
                        }
                        if (playerObject.id == network.playerId)
                        {
                            network.duelUI.SetSelfHealth(playerObject.health);
                            //playerObject.stamina = Mathf.Max(0.0f, Mathf.Min(100.0f, playerObject.stamina + playerObject.staminaRegeneration * deltaTime));
                            network.duelUI.SetStamina(Mathf.Max(0.0f, Mathf.Min(100.0f, playerObject.stamina + Mathf.Max(0.0f, playerObject.staminaRegeneration) * (Time.time - playerObject.lastTimestamp))));
                            //network.healthBarSelf.text = Mathf.Floor(playerObject.health) + "";
                            //network.staminaBar.rectTransform.sizeDelta = new Vector2(network.staminaBar.rectTransform.sizeDelta.x + (network.gameMatchMaker.canvasPlay.pixelRect.width * playerObject.stamina / 100.0f - network.staminaBar.rectTransform.sizeDelta.x) * Mathf.Min(1.0f, Time.deltaTime * 15.0f), network.staminaBar.rectTransform.sizeDelta.y);
                        }
                        else
                        {
                            network.duelUI.SetOpponentHealth(playerObject.health);
                            //network.healthBarEnemy.text = Mathf.Floor(playerObject.health) + "";
                        }
                        break;
                    case ObjectType.OBSTRUCTION:
                        obstructionObject = (ObstructionObject)objNode.Value;
                        if (Mathf.Abs(network.camera.transform.position.x - obstructionObject.visualObject.transform.position.x) > 3.0f * 50.0f)
                        {
                            if (network.camera.transform.position.x - obstructionObject.visualObject.transform.position.x > 0.0f)
                            {
                                obstructionObject.visualObject.transform.position += Vector3.right * 6.0f * 50.0f;
                            }
                            else
                            {
                                obstructionObject.visualObject.transform.position += Vector3.right * -6.0f * 50.0f;
                            }
                        }
                        break;
                    case ObjectType.MISSILE:
                        missileObject = (MissileObject)objNode.Value;
                        //Debug.Log("[" + Time.time + "] position: " + missileObject.position + " ; velocity: " + missileObject.velocity);
                        if (missileObject.visualObject != null)
                        {
                            //v3Delta = missileObject.position * 10.0f - missileObject.visualObject.transform.position;
                            //missileObject.visualObject.transform.position += v3Delta * Mathf.Min(1.0f, deltaTime * 15.0f);
                            missileObject.visualObject.transform.position = missileObject.position * 50.0f;
                            //scale = 1.0f - (missileObject.position.y + 1.0f) * 0.3f;
                            //missileObject.visualObject.transform.localScale = new Vector3(scale, Mathf.Pow(scale, 1.5f), 1.0f);
                        }
                        break;
                }
                if(objNode.Value.floatingNotifyOffset > 0.0f)
                {
                    objNode.Value.floatingNotifyOffset -= Time.deltaTime;
                    if(objNode.Value.floatingNotifyOffset < 0.0f)
                    {
                        objNode.Value.floatingNotifyOffset = 0.0f;
                    }
                }
            }
            objNode = objNodeNext;
        }
        //network.armedMissile.transform.position += Vector3.right * (network.camera.transform.position.x - network.armedMissile.transform.position.x);
    }

    public void PhysicCycle(float deltaTime)
    {
        network.updateTimeout -= deltaTime;
        if (network.updateTimeout <= 0.0f)
        {
            network.updating = true;
            network.updateTimeout = network.updateCooldown;
        }
        LinkedListNode<LocationObject> objNode;
        LinkedListNode<LocationObject> objNodeNext;
        LinkedListNode<LocationObject> objNode2;
        LinkedListNode<LocationObject> objNodeNext2;
        ObstructionObject obstructionObject;
        PlayerObject playerObject;
        PlayerObject attackerObject;
        MissileObject missileObject;
        string notifyMessage = "";
        float moveSpeed = 0.0f;
        float damage = 0.0f;
        float critChance = 0.0f;
        float noticeOffset = 0.0f;
        bool hit = false;
        objNode = objects.First;
        while (objNode != null)
        {
            objNodeNext = objNode.Next;
            if (objNode.Value != null)
            {
                switch (objNode.Value.objectType)
                {
                    case ObjectType.PLAYER:
                        playerObject = (PlayerObject)objNode.Value;
                        playerObject.position.x += playerObject.MoveSpeed() * deltaTime;
                        if (playerObject.position.x < -3.0f)
                        {
                            playerObject.position += Vector3.right * 6.0f;
                        }
                        if (playerObject.position.x > 3.0f)
                        {
                            playerObject.position += Vector3.right * -6.0f;
                        }
                        playerObject.strafeTimeout -= deltaTime;
                        if (playerObject.strafeTimeout <= 0.0f)
                        {
                            float playerX = playerObject.position.x;
                            float enemyX = 0.0f;
                            float strafeMin = 0.0f;
                            float strafeMax = 0.0f;
                            float deltaX;
                            if (playerObject.id == 0)
                            {
                                enemyX = objects.First.Next.Value.position.x;
                            }
                            else
                            {
                                enemyX = objects.First.Value.position.x;
                            }
                            deltaX = playerX - enemyX;
                            if (deltaX > 3.0f)
                            {
                                deltaX += -6.0f;
                            }
                            if (deltaX < -3.0f)
                            {
                                deltaX += 6.0f;
                            }
                            if ((deltaX > 0.0f && playerObject.direction > 0.0f) || (deltaX < 0.0f && playerObject.direction < 0.0f))
                            {
                                strafeMin = (playerObject.strafeMinTimeout + playerObject.strafeMaxTimeout) * 0.5f;
                                strafeMax = playerObject.strafeMaxTimeout;
                            }
                            else
                            {
                                strafeMin = playerObject.strafeMinTimeout;
                                strafeMax = (playerObject.strafeMinTimeout + playerObject.strafeMaxTimeout) * 0.5f;
                            }
                            playerObject.strafeTimeout += UnityEngine.Random.Range(strafeMin, strafeMax);
                            playerObject.direction *= -1.0f;
                        }
                        if (playerObject.stamina < playerObject.maxStamina)
                        {
                            playerObject.stamina += playerObject.staminaRegeneration * Time.deltaTime;
                            if (playerObject.stamina > playerObject.maxStamina)
                            {
                                playerObject.stamina = playerObject.maxStamina;
                            }
                        }
                        if (playerObject.legInjury > 0.0f)
                        {
                            playerObject.legInjury -= deltaTime;
                            if (playerObject.legInjury < 0.0f)
                            {
                                playerObject.legInjury = 0.0f;
                            }
                        }
                        if (playerObject.armInjury > 0.0f)
                        {
                            playerObject.armInjury -= deltaTime;
                            if (playerObject.armInjury < 0.0f)
                            {
                                playerObject.armInjury = 0.0f;
                            }
                        }
                        if (playerObject.stun > 0.0f)
                        {
                            playerObject.stun -= deltaTime;
                            if (playerObject.stun < 0.0f)
                            {
                                playerObject.stun = 0.0f;
                            }
                        }
                        if (playerObject.abilityCrit > 0.0f)
                        {
                            playerObject.abilityCrit -= deltaTime;
                            if (playerObject.abilityCrit < 0.0f)
                            {
                                playerObject.abilityCrit = 0.0f;
                            }
                        }
                        if (playerObject.abilityStun > 0.0f)
                        {
                            playerObject.abilityStun -= deltaTime;
                            if (playerObject.abilityStun < 0.0f)
                            {
                                playerObject.abilityStun = 0.0f;
                            }
                        }
                        if (playerObject.abilityEvade > 0.0f)
                        {
                            playerObject.abilityEvade -= deltaTime;
                            if (playerObject.abilityEvade < 0.0f)
                            {
                                playerObject.abilityEvade = 0.0f;
                            }
                        }
                        if (playerObject.abilityShield > 0.0f)
                        {
                            playerObject.abilityShield -= deltaTime;
                            if (playerObject.abilityShield < 0.0f)
                            {
                                playerObject.abilityShield = 0.0f;
                            }
                        }
                        if (network.updating && !network.isLocal)
                        {
                            network.RpcMoveObject(objNode.Value.id, objNode.Value.position, objNode.Value.velocity, objNode.Value.acceleration, objNode.Value.torsion, objNode.Value.scale, 0.0f);
                            network.RpcUpdatePlayer(playerObject.id, playerObject.health, playerObject.stamina, playerObject.staminaConsumption);
                        }
                        break;
                    case ObjectType.OBSTRUCTION:
                        //obstructionObject = (ObstructionObject) objNode.Value;
                        break;
                    case ObjectType.MISSILE:
                        missileObject = (MissileObject)objNode.Value;
                        missileObject.direction = Quaternion.Euler(missileObject.torsion.x * deltaTime, missileObject.torsion.y * deltaTime, missileObject.torsion.z * deltaTime) * missileObject.direction;
                        missileObject.direction.y += -0.98f * 0.45f * deltaTime;
                        missileObject.direction.Normalize();
                        missileObject.velocity.x += missileObject.acceleration.x * deltaTime;
                        missileObject.velocity.y += missileObject.acceleration.y * deltaTime;
                        missileObject.velocity.z += missileObject.acceleration.z * deltaTime;
                        missileObject.position.x += missileObject.velocity.x * deltaTime;
                        missileObject.position.y += missileObject.velocity.y * deltaTime;
                        missileObject.position.z += missileObject.velocity.z * deltaTime;
                        //Debug.Log("missileObject position: " + missileObject.position);
                        if (missileObject.position.y <= 0.0f)
                        {
                            Debug.Log("missileObject DESTROY by position.z");
                            if (missileObject.visualObject != null)
                            {
                                GameObject.Destroy(missileObject.visualObject.gameObject);
                            }
                            RemoveObject(objNode.Value);
                        }
                        else if (missileObject.position.z > 2.5f && missileObject.position.z - missileObject.direction.z * missileObject.velocity.z * deltaTime <= 2.5f)
                        {
                            objNode2 = objects.First;
                            while (objNode2 != null)
                            {
                                objNodeNext2 = objNode2.Next;
                                if (objNode2.Value != null)
                                {
                                    switch (objNode2.Value.objectType)
                                    {
                                        case ObjectType.OBSTRUCTION:
                                            obstructionObject = (ObstructionObject)objNode2.Value;
                                            if (Mathf.Abs(missileObject.position.x - obstructionObject.position.x) < obstructionObject.scale)
                                            {
                                                if (missileObject.direction.y > 0.0f)
                                                {
                                                    attackerObject = (PlayerObject)objects.First.Value;
                                                }
                                                else
                                                {
                                                    attackerObject = (PlayerObject)objects.First.Next.Value;
                                                }
                                                obstructionObject.durability -= UnityEngine.Random.Range(attackerObject.minDamage, attackerObject.maxDamage);
                                                if (obstructionObject.durability <= 0.0f)
                                                {
                                                    if (obstructionObject.visualObject != null)
                                                    {
                                                        GameObject.Destroy(obstructionObject.visualObject.gameObject);
                                                    }
                                                    RemoveObject(objNode2.Value);
                                                    if (missileObject.visualObject != null)
                                                    {
                                                        GameObject.Destroy(missileObject.visualObject.gameObject);
                                                    }
                                                    RemoveObject(objNode.Value);
                                                    objNodeNext2 = null;
                                                    float hpBonus = 0.0f;
                                                    if (UnityEngine.Random.Range(0.0f, 1.0f) < 0.5f)
                                                    {
                                                        hpBonus = 15.0f;
                                                    }
                                                    else
                                                    {
                                                        if (UnityEngine.Random.Range(0.0f, 1.0f) < 0.5f)
                                                        {
                                                            hpBonus = 25.0f;
                                                        }
                                                        else
                                                        {
                                                            if (UnityEngine.Random.Range(0.0f, 1.0f) < 0.66f)
                                                            {
                                                                hpBonus = -15.0f;
                                                            }
                                                        }
                                                    }
                                                    if (hpBonus != 0.0f)
                                                    {
                                                        if (missileObject.direction.y > 0.0f)
                                                        {
                                                            playerObject = (PlayerObject)objects.First.Value;
                                                        }
                                                        else
                                                        {
                                                            playerObject = (PlayerObject)objects.First.Next.Value;
                                                        }
                                                        if (playerObject != null)
                                                        {
                                                            playerObject.health += hpBonus;
                                                            if (hpBonus > 0.0f)
                                                            {
                                                                if (!network.isLocal)
                                                                {
                                                                    network.RpcShowNotice(playerObject.id, "+" + hpBonus, noticeOffset, 0, true);
                                                                }
                                                                network.ShowNotice(playerObject.id, "+" + hpBonus, noticeOffset, 0, true);
                                                            }
                                                            else
                                                            {
                                                                if (!network.isLocal)
                                                                {
                                                                    network.RpcShowNotice(playerObject.id, "" + hpBonus, noticeOffset, 1, true);
                                                                }
                                                                network.ShowNotice(playerObject.id, "" + hpBonus, noticeOffset, 1, true);
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    network.FlashObstruction(obstructionObject.id);
                                                    if (!network.isLocal)
                                                    {
                                                        network.RpcFlashObstruction(obstructionObject.id);
                                                    }
                                                }
                                            }
                                            break;
                                    }
                                }
                                objNode2 = objNodeNext2;
                            }
                        }
                        else if ((missileObject.direction.z > 0.0f && missileObject.position.z >= 5.0f) || (missileObject.direction.z < 0.0f && missileObject.position.z <= -5.0f))
                        {
                            objNode2 = objects.First;
                            while (objNode2 != null)
                            {
                                objNodeNext2 = objNode2.Next;
                                if (objNode2.Value != null)
                                {
                                    switch (objNode2.Value.objectType)
                                    {
                                        case ObjectType.PLAYER:
                                            playerObject = (PlayerObject)objNode2.Value;
                                            attackerObject = null;
                                            hit = false;
                                            if (missileObject.direction.z > 0.0f && playerObject.id == 1 && Mathf.Abs(missileObject.position.x - playerObject.position.x) < 0.3f)
                                            {
                                                hit = true;
                                                attackerObject = (PlayerObject)objects.First.Value;
                                            }
                                            else if (missileObject.direction.z < 0.0f && playerObject.id == 0 && Mathf.Abs(missileObject.position.x - playerObject.position.x) < 0.3f)
                                            {
                                                hit = true;
                                                attackerObject = (PlayerObject)objects.First.Next.Value;
                                            }
                                            if (hit && playerObject.abilityEvade == 0.0f && UnityEngine.Random.Range(0.0f, 1.0f) < playerObject.abilityEvadeChance)
                                            {
                                                hit = false;
                                                playerObject.abilityEvade = 5.0f;
                                                if (!network.isLocal)
                                                {
                                                    network.RpcShowNotice(playerObject.id, "+ УКЛОНЕНИЕ", noticeOffset, 0, true);
                                                }
                                                network.ShowNotice(playerObject.id, "+ УКЛОНЕНИЕ", noticeOffset, 0, true);
                                                noticeOffset += 1.0f;
                                                if (playerObject.id == 0)
                                                {
                                                    network.FlashPassiveAbility(playerObject.id);
                                                }
                                                else
                                                {
                                                    if (!network.isLocal)
                                                    {
                                                        network.RpcFlashPassiveAbility(playerObject.id);
                                                    }
                                                }
                                                // Ability Evade
                                            }
                                            if (hit)
                                            {
                                                notifyMessage = "";
                                                network.FlashPlayer(playerObject.id);
                                                if (!network.isLocal)
                                                {
                                                    network.RpcFlashPlayer(playerObject.id);
                                                }
                                                damage = UnityEngine.Random.Range(attackerObject.minDamage, attackerObject.maxDamage);
                                                critChance = attackerObject.critChance;
                                                if (attackerObject.abilityCrit == 0.0f)
                                                {
                                                    attackerObject.abilityCrit = 5.0f;
                                                    critChance += attackerObject.abilityCritChance;
                                                    if (attackerObject.id == 0)
                                                    {
                                                        network.FlashPassiveAbility(attackerObject.id);
                                                    }
                                                    else
                                                    {
                                                        if (!network.isLocal)
                                                        {
                                                            network.RpcFlashPassiveAbility(attackerObject.id);
                                                        }
                                                    }
                                                    // Ability Crit
                                                }
                                                if (UnityEngine.Random.Range(0.0f, 1.0f) < critChance)
                                                {
                                                    notifyMessage += "K";
                                                    damage *= attackerObject.critMultiplier;
                                                }
                                                if (playerObject.abilityShield >= 5.0f)
                                                {
                                                    notifyMessage += "Щ";
                                                    damage *= playerObject.abilityShieldMultiplier;
                                                    if (!network.isLocal)
                                                    {
                                                        network.RpcShowNotice(playerObject.id, "+ ЩИТ", noticeOffset, 0, true);
                                                    }
                                                    network.ShowNotice(playerObject.id, "+ ЩИТ", noticeOffset, 0, true);
                                                    noticeOffset += 1.0f;
                                                }
                                                notifyMessage += " -" + Mathf.Floor(damage);
                                                if (playerObject.id == 0)
                                                {
                                                    if (!network.isLocal)
                                                    {
                                                        network.RpcShowNotice(playerObject.id, notifyMessage, noticeOffset, 1, true);
                                                    }
                                                    network.ShowNotice(playerObject.id, notifyMessage, 1.0f, 1, false);
                                                }
                                                else
                                                {
                                                    if (!network.isLocal)
                                                    {
                                                        network.RpcShowNotice(playerObject.id, notifyMessage, 1.0f, 1, false);
                                                    }
                                                    network.ShowNotice(playerObject.id, notifyMessage, noticeOffset, 1, true);
                                                }
                                                noticeOffset += 1.0f;
                                                playerObject.health -= damage;
                                                if (attackerObject.stunMove > 0.0f)
                                                {
                                                    attackerObject.stunMove = 0.0f;
                                                    playerObject.stun += attackerObject.abilityStunDuration;
                                                    if (!network.isLocal)
                                                    {
                                                        network.RpcShowNotice(playerObject.id, "- ОГЛУШЕН", noticeOffset, 1, true);
                                                    }
                                                    network.ShowNotice(playerObject.id, "- ОГЛУШЕН", noticeOffset, 1, true);
                                                    if (!network.isLocal)
                                                    {
                                                        network.RpcShowNotice(playerObject.id, "- ОГЛУШЕН", 5.0f, 1, false);
                                                    }
                                                    network.ShowNotice(playerObject.id, "- ОГЛУШЕН", 5.0f, 1, false);
                                                    noticeOffset += 1.0f;
                                                }
                                                if (UnityEngine.Random.Range(0.0f, 1.0f) < attackerObject.injuryChance)
                                                {
                                                    if (UnityEngine.Random.Range(0.0f, 1.0f) < 0.5f)
                                                    {
                                                        // Injury Arm
                                                        playerObject.armInjury = 8.0f;
                                                        if (!network.isLocal)
                                                        {
                                                            network.RpcShowNotice(playerObject.id, "- РУКА", noticeOffset, 1, true);
                                                        }
                                                        network.ShowNotice(playerObject.id, "- РУКА", noticeOffset, 1, true);
                                                        if (!network.isLocal)
                                                        {
                                                            network.RpcShowNotice(playerObject.id, "- РУКА", 8.0f, 1, false);
                                                        }
                                                        network.ShowNotice(playerObject.id, "- РУКА", 8.0f, 1, false);
                                                        noticeOffset += 1.0f;
                                                    }
                                                    else
                                                    {
                                                        // Injury Leg
                                                        playerObject.legInjury = 8.0f;
                                                        if (!network.isLocal)
                                                        {
                                                            network.RpcShowNotice(playerObject.id, "- НОГА", noticeOffset, 1, true);
                                                        }
                                                        network.ShowNotice(playerObject.id, "- НОГА", noticeOffset, 1, true);
                                                        if (!network.isLocal)
                                                        {
                                                            network.RpcShowNotice(playerObject.id, "- НОГА", 8.0f, 1, false);
                                                        }
                                                        network.ShowNotice(playerObject.id, "- НОГА", 8.0f, 1, false);
                                                        noticeOffset += 1.0f;
                                                    }
                                                }
                                            }
                                            if (playerObject.health <= 0.0f)
                                            {
                                                if (playerObject.id == 0)
                                                {
                                                    GameOver(1);
                                                    return;
                                                }
                                                else
                                                {
                                                    GameOver(0);
                                                    return;
                                                }
                                            }
                                            break;
                                    }
                                }
                                objNode2 = objNodeNext2;
                            }
                            if (missileObject.visualObject != null)
                            {
                                GameObject.Destroy(missileObject.visualObject.gameObject);
                            }
                            RemoveObject(objNode.Value);
                        }
                        if (network.updating && !network.isLocal)
                        {
                            network.RpcMoveObject(objNode.Value.id, objNode.Value.position, objNode.Value.velocity, objNode.Value.acceleration, objNode.Value.torsion, objNode.Value.scale, 0.0f);
                        }
                        break;
                }
            }
            objNode = objNodeNext;
        }
        if (network.updating)
        {
            network.updating = false;
        }
    }

    public LocationObject GetObject(int id)
    {
        LinkedListNode<LocationObject> objNode;
        objNode = objects.First;
        while (objNode != null)
        {
            if (objNode.Value.id == id)
            {
                return objNode.Value;
            }
            objNode = objNode.Next;
        }
        return null;
    }

    public void GameOver(int winner)
    {
        if (!network.isLocal)
        {
            network.RpcGameOver(winner, 0.0f, 0.0f, 0.0f);
        }
        network.GameOver(winner, 0.0f, 0.0f, 0.0f);
        network = null;
    }

    public void Cleanup()
    {
        LinkedListNode<LocationObject> objNode;
        LinkedListNode<LocationObject> objNodeNext;
        objNode = objects.First;
        while (objNode != null)
        {
            objNodeNext = objNode.Next;
            switch (objNode.Value.objectType)
            {
                case ObjectType.PLAYER:
                    PlayerObject playerObject = (PlayerObject)objNode.Value;
                    if (playerObject.visualObject != null)
                    {
                        GameObject.Destroy(playerObject.visualObject.gameObject);
                    }
                    break;
                case ObjectType.OBSTRUCTION:
                    ObstructionObject obstructionObject = (ObstructionObject)objNode.Value;
                    if (obstructionObject.visualObject != null)
                    {
                        GameObject.Destroy(obstructionObject.visualObject.gameObject);
                    }
                    break;
                case ObjectType.MISSILE:
                    MissileObject missileObject = (MissileObject)objNode.Value;
                    if (missileObject.visualObject != null)
                    {
                        GameObject.Destroy(missileObject.visualObject.gameObject);
                    }
                    break;
            }
            objects.Remove(objNode);
            objNode = objNodeNext;
        }
        network.location = null;
    }

}

public class LocationObject
{

    public static int lastObjectId = 0;

    public float lastTimestamp = 0.0f;
    public float lastRemoteTimestamp = 0.0f;
    public Vector3 lastPosition = new Vector3();

    public int id = -1;
    public Location.ObjectType objectType = Location.ObjectType.NONE;
    public int visualId = -1;
    public Vector3 position = new Vector3(0.0f, 0.0f, 0.0f);
    public Vector3 velocity = new Vector3(0.0f, 0.0f, 0.0f);
    public Vector3 acceleration = new Vector3(0.0f, 0.0f, 0.0f);
    public Vector3 torsion = new Vector3(0.0f, 0.0f, 0.0f);
    //public Vector3 localVelocity = new Vector3(0.0f, 0.0f, 0.0f);
    //public Vector3 passiveVelocity = new Vector3(0.0f, 0.0f, 0.0f);
    public float scale = 0.0f;

    public float floatingNotifyOffset = 0.0f;

    public LocationObject()
    {
        id = lastObjectId++;
    }

}

public class ObstructionObject : LocationObject
{

    public ObstructionObject() : base()
    {
        objectType = Location.ObjectType.OBSTRUCTION;
    }

    public float durability = 0.0f;
    public ObstructionController visualObject;

}

public class PlayerObject : LocationObject
{

    public PlayerObject() : base()
    {
        objectType = Location.ObjectType.PLAYER;
    }

    public float strafeMinTimeout = 0.0f;
    public float strafeMaxTimeout = 0.0f;

    public float direction = 0.0f;
    public float stunMove = 0.0f;
    public float stun = 0.0f;
    public float strafeTimeout = 0.0f;
    public float armInjury = 0.0f;
    public float legInjury = 0.0f;
    public float abilityShield = -1.0f;
    public float abilityStun = -1.0f;
    public float abilityCrit = -1.0f;
    public float abilityEvade = -1.0f;

    public float health = 0.0f;
    public float stamina = 0.0f;
    public float maxStamina = 0.0f;
    public float staminaConsumption = 0.0f;
    public float staminaRegeneration = 0.0f;
    public float minDamage = 0.0f;
    public float maxDamage = 0.0f;
    public float critChance = 0.15f;
    public float critMultiplier = 1.5f;
    public float injuryChance = 0.1f;
    public float armInjuryEffect = 0.0f;
    public float legInjuryEffect = 0.0f;
    public float abilityEvadeChance = 0.0f;
    public float abilityCritChance = 0.0f;
    public float abilityStunDuration = 0.0f;
    public float abilityShieldDuration = 0.0f;
    public float abilityShieldMultiplier = 0.0f;
    public float strafeSpeed = 0.0f;

    public PlayerController visualObject;

    public float MoveSpeed()
    {
        float moveSpeed = direction * strafeSpeed;
        if (stun > 0.0f)
        {
            moveSpeed = 0.0f;
        }
        else if (legInjury > 0.0f)
        {
            moveSpeed /= 1.0f + legInjuryEffect;
        }
        return moveSpeed;
    }

}

public class MissileObject : LocationObject
{

    public MissileObject() : base()
    {
        objectType = Location.ObjectType.MISSILE;
    }

    public Vector3 direction = new Vector3(0.0f, 0.0f, 0.0f);
    //public float velocity = 0.0f;
    public MissileController visualObject;

}

public class BaseObjectMessage
{

    public int id;
    public float timestamp = 0.0f;
    public float timemark = 0.0f;
    public byte eventCode = 0;

    public BaseObjectMessage()
    {

    }

    public BaseObjectMessage(float currentTimestamp, float targetTimemark)
    {
        timestamp = currentTimestamp;
        timemark = targetTimemark;
    }

    public virtual byte[] Pack()
    {
        int index = 0;
        byte[] data = new byte[4 * 3];
        PackBase(ref data, ref index);
        return data;
    }

    public virtual void Unpack(byte[] data)
    {
        int index = 0;
        UnpackBase(ref data, ref index);
    }

    public void PackBase(ref byte[] data, ref int index)
    {
        PutInt(data, id, ref index);
        PutFloat(data, timestamp, ref index);
        PutFloat(data, timemark, ref index);
    }

    public void UnpackBase(ref byte[] data, ref int index)
    {
        id = GetInt(data, ref index);
        timestamp = GetFloat(data, ref index);
        timemark = GetFloat(data, ref index);
    }

    public bool GetBool(byte[] data, ref int index)
    {
        bool value = BitConverter.ToBoolean(data, index);
        index += 1;
        return value;
    }

    public short GetShort(byte[] data, ref int index)
    {
        short value = BitConverter.ToInt16(data, index);
        index += 2;
        return value;
    }

    public int GetInt(byte[] data, ref int index)
    {
        int value = BitConverter.ToInt32(data, index);
        index += 4;
        return value;
    }

    public uint GetUInt(byte[] data, ref int index)
    {
        uint value = BitConverter.ToUInt32(data, index);
        index += 4;
        return value;
    }

    public long GetLong(byte[] data, ref int index)
    {
        long value = BitConverter.ToInt64(data, index);
        index += 8;
        return value;
    }

    public ulong GetULong(byte[] data, ref int index)
    {
        byte[] b8 = new byte[8];
        Buffer.BlockCopy(data, index, b8, 0, 8);
        Array.Reverse(b8, 0, 8);
        ulong value = BitConverter.ToUInt64(b8, 0);
        index += 8;
        return value;
    }

    public float GetFloat(byte[] data, ref int index)
    {
        float value = BitConverter.ToSingle(data, index);
        index += 4;
        return value;
    }

    public string GetString(byte[] data, ref int index)
    {
        string value;
        int length = BitConverter.ToInt32(data, index);
        index += 4;
        value = Encoding.UTF8.GetString(data, index, length);
        index += length;
        return value;
    }

    public string GetSString(byte[] data, ref int index)
    {
        string value;
        int length = BitConverter.ToInt16(data, index);
        index += 2;
        value = Encoding.UTF8.GetString(data, index, length);
        index += length;
        return value;
    }

    public void PutBool(byte[] data, bool value, ref int index)
    {
        byte[] b1 = BitConverter.GetBytes(value);
        Buffer.BlockCopy(b1, 0, data, index, 1);
        index += 1;
    }

    public void PutShort(byte[] data, short value, ref int index)
    {
        byte[] b2 = BitConverter.GetBytes(value);
        Buffer.BlockCopy(b2, 0, data, index, 2);
        index += 2;
    }

    public void PutInt(byte[] data, int value, ref int index)
    {
        byte[] b4 = BitConverter.GetBytes(value);
        Buffer.BlockCopy(b4, 0, data, index, 4);
        index += 4;
    }

    public void PutUInt(byte[] data, uint value, ref int index)
    {
        byte[] b4 = BitConverter.GetBytes(value);
        Buffer.BlockCopy(b4, 0, data, index, 4);
        index += 4;
    }

    public void PutULong(byte[] data, ulong value, ref int index)
    {
        byte[] b8 = BitConverter.GetBytes(value);
        Array.Reverse(b8, 0, 8);
        Buffer.BlockCopy(b8, 0, data, index, 8);
        index += 8;
    }

    public void PutDouble(byte[] data, double value, ref int index)
    {
        byte[] b8 = BitConverter.GetBytes(value);
        Array.Reverse(b8, 0, 8);
        Buffer.BlockCopy(b8, 0, data, index, 8);
        index += 8;
    }

    public void PutFloat(byte[] data, float value, ref int index)
    {
        byte[] b4 = BitConverter.GetBytes(value);
        Buffer.BlockCopy(b4, 0, data, index, 4);
        index += 4;
    }

    public void PutString(byte[] data, string value, ref int index)
    {
        byte[] b = Encoding.UTF8.GetBytes(value);
        byte[] b4 = BitConverter.GetBytes(b.Length);
        Buffer.BlockCopy(b4, 0, data, index, 4);
        index += 4;
        Buffer.BlockCopy(b, 0, data, index, b.Length);
        index += b.Length;
    }

    public void PutSString(byte[] data, string value, ref int index)
    {
        byte[] b = Encoding.UTF8.GetBytes(value);
        byte[] b2 = BitConverter.GetBytes(b.Length);
        Buffer.BlockCopy(b2, 0, data, index, 2);
        index += 2;
        Buffer.BlockCopy(b, 0, data, index, b.Length);
        index += b.Length;
    }

}

public class SpawnObjectMessage : BaseObjectMessage
{

    public Location.ObjectType objectType;
    public int objectId = -1;
    public Vector3 newPosition;
    public Vector3 newVelocity;
    public Vector3 newAcceleration;
    public Vector3 newTorsion;
    public float newFloat;
    public int visualId;

    public SpawnObjectMessage() : base()
    {
    }

    public SpawnObjectMessage(float currentTimestamp, float targetTimemark) : base(currentTimestamp, targetTimemark)
    {
    }

    public override byte[] Pack()
    {
        int index = 0;
        byte[] data = new byte[4 * 19];
        PackBase(ref data, ref index);
        PutInt(data, objectId, ref index);
        PutInt(data, (int)objectType, ref index);
        PutFloat(data, newPosition.x, ref index);
        PutFloat(data, newPosition.y, ref index);
        PutFloat(data, newPosition.z, ref index);
        PutFloat(data, newVelocity.x, ref index);
        PutFloat(data, newVelocity.y, ref index);
        PutFloat(data, newVelocity.z, ref index);
        PutFloat(data, newAcceleration.x, ref index);
        PutFloat(data, newAcceleration.y, ref index);
        PutFloat(data, newAcceleration.z, ref index);
        PutFloat(data, newTorsion.x, ref index);
        PutFloat(data, newTorsion.y, ref index);
        PutFloat(data, newTorsion.z, ref index);
        PutFloat(data, newFloat, ref index);
        PutInt(data, visualId, ref index);
        return data;
    }

    public override void Unpack(byte[] data)
    {
        int index = 0;
        UnpackBase(ref data, ref index);
        objectId = GetInt(data, ref index);
        objectType = (Location.ObjectType)GetInt(data, ref index);
        newPosition = new Vector3();
        newPosition.x = GetFloat(data, ref index);
        newPosition.y = GetFloat(data, ref index);
        newPosition.z = GetFloat(data, ref index);
        newVelocity.x = GetFloat(data, ref index);
        newVelocity.y = GetFloat(data, ref index);
        newVelocity.z = GetFloat(data, ref index);
        newAcceleration.x = GetFloat(data, ref index);
        newAcceleration.y = GetFloat(data, ref index);
        newAcceleration.z = GetFloat(data, ref index);
        newTorsion.x = GetFloat(data, ref index);
        newTorsion.y = GetFloat(data, ref index);
        newTorsion.z = GetFloat(data, ref index);
        newFloat = GetFloat(data, ref index);
        visualId = GetInt(data, ref index);
    }

}

public class DestroyObjectMessage : BaseObjectMessage
{

    public int objectId = -1;

    public DestroyObjectMessage() : base()
    {
    }

    public DestroyObjectMessage(float currentTimestamp, float targetTimemark) : base(currentTimestamp, targetTimemark)
    {
    }

    public override byte[] Pack()
    {
        int index = 0;
        byte[] data = new byte[4 * 4];
        PackBase(ref data, ref index);
        PutInt(data, objectId, ref index);
        return data;
    }

    public override void Unpack(byte[] data)
    {
        int index = 0;
        UnpackBase(ref data, ref index);
        objectId = GetInt(data, ref index);
    }

}

public class MoveObjectMessage : BaseObjectMessage
{

    public int objectId = -1;
    public Vector3 newPosition;
    public Vector3 newVelocity;
    public Vector3 newAcceleration;
    public Vector3 newTorsion;
    public float newFloat;

    public MoveObjectMessage() : base()
    {
    }

    public MoveObjectMessage(float currentTimestamp, float targetTimemark) : base(currentTimestamp, targetTimemark)
    {
    }

    public override byte[] Pack()
    {
        int index = 0;
        byte[] data = new byte[4 * 17];
        PackBase(ref data, ref index);
        PutInt(data, objectId, ref index);
        PutFloat(data, newPosition.x, ref index);
        PutFloat(data, newPosition.y, ref index);
        PutFloat(data, newPosition.z, ref index);
        PutFloat(data, newVelocity.x, ref index);
        PutFloat(data, newVelocity.y, ref index);
        PutFloat(data, newVelocity.z, ref index);
        PutFloat(data, newAcceleration.x, ref index);
        PutFloat(data, newAcceleration.y, ref index);
        PutFloat(data, newAcceleration.z, ref index);
        PutFloat(data, newTorsion.x, ref index);
        PutFloat(data, newTorsion.y, ref index);
        PutFloat(data, newTorsion.z, ref index);
        PutFloat(data, newFloat, ref index);
        return data;
    }

    public override void Unpack(byte[] data)
    {
        int index = 0;
        UnpackBase(ref data, ref index);
        objectId = GetInt(data, ref index);
        newPosition = new Vector3();
        newPosition.x = GetFloat(data, ref index);
        newPosition.y = GetFloat(data, ref index);
        newPosition.z = GetFloat(data, ref index);
        newVelocity.x = GetFloat(data, ref index);
        newVelocity.y = GetFloat(data, ref index);
        newVelocity.z = GetFloat(data, ref index);
        newAcceleration.x = GetFloat(data, ref index);
        newAcceleration.y = GetFloat(data, ref index);
        newAcceleration.z = GetFloat(data, ref index);
        newTorsion.x = GetFloat(data, ref index);
        newTorsion.y = GetFloat(data, ref index);
        newTorsion.z = GetFloat(data, ref index);
        newFloat = GetFloat(data, ref index);
        index += 4;
    }

}

public class UpdatePlayerMessage : BaseObjectMessage
{

    public float newHealth;
    public float newStamina;
    public float newStaminaConsumption;

    public UpdatePlayerMessage() : base()
    {
    }

    public UpdatePlayerMessage(float currentTimestamp, float targetTimemark) : base(currentTimestamp, targetTimemark)
    {
    }

    public override byte[] Pack()
    {
        int index = 0;
        byte[] data = new byte[4 * 6];
        PackBase(ref data, ref index);
        PutFloat(data, newHealth, ref index);
        PutFloat(data, newStamina, ref index);
        PutFloat(data, newStaminaConsumption, ref index);
        return data;
    }

    public override void Unpack(byte[] data)
    {
        int index = 0;
        UnpackBase(ref data, ref index);
        newHealth = GetFloat(data, ref index);
        newStamina = GetFloat(data, ref index);
        newStaminaConsumption = GetFloat(data, ref index);
    }

}

public class SetAbilityMessage : BaseObjectMessage
{

    public int place;
    public int value;

    public SetAbilityMessage() : base()
    {
    }

    public SetAbilityMessage(float currentTimestamp, float targetTimemark) : base(currentTimestamp, targetTimemark)
    {
    }

    public override byte[] Pack()
    {
        int index = 0;
        byte[] data = new byte[4 * 5];
        PackBase(ref data, ref index);
        PutInt(data, place, ref index);
        PutInt(data, value, ref index);
        return data;
    }

    public override void Unpack(byte[] data)
    {
        int index = 0;
        UnpackBase(ref data, ref index);
        place = GetInt(data, ref index);
        value = GetInt(data, ref index);
    }

}

public class NoticeMessage : BaseObjectMessage
{

    public int numericValue;
    public int prefixMessage;
    public int suffixMessage;
    public float offset;
    public int color;
    public bool floating;

    public NoticeMessage() : base()
    {
    }

    public NoticeMessage(float currentTimestamp, float targetTimemark) : base(currentTimestamp, targetTimemark)
    {
    }

    public override byte[] Pack()
    {
        int index = 0;
        byte[] data = new byte[4 * 8 + 1];
        PackBase(ref data, ref index);
        PutInt(data, numericValue, ref index);
        PutInt(data, prefixMessage, ref index);
        PutInt(data, suffixMessage, ref index);
        PutFloat(data, offset, ref index);
        PutInt(data, color, ref index);
        PutBool(data, floating, ref index);
        return data;
    }

    public override void Unpack(byte[] data)
    {
        int index = 0;
        UnpackBase(ref data, ref index);
        numericValue = GetInt(data, ref index);
        prefixMessage = GetInt(data, ref index);
        suffixMessage = GetInt(data, ref index);
        offset = GetFloat(data, ref index);
        color = GetInt(data, ref index);
        floating = GetBool(data, ref index);
    }

}

public class ThrowMessage : BaseObjectMessage
{

    public float angleX;
    public float angleY;
    public float torsion;
    public float speed;

    public ThrowMessage() : base()
    {
    }

    public ThrowMessage(float currentTimestamp) : base(currentTimestamp, 0.0f)
    {
    }

    public override byte[] Pack()
    {
        int index = 0;
        byte[] data = new byte[4 * 7];
        PackBase(ref data, ref index);
        PutFloat(data, angleX, ref index);
        PutFloat(data, angleY, ref index);
        PutFloat(data, torsion, ref index);
        PutFloat(data, speed, ref index);
        return data;
    }

    public override void Unpack(byte[] data)
    {
        int index = 0;
        UnpackBase(ref data, ref index);
        angleX = GetFloat(data, ref index);
        angleY = GetFloat(data, ref index);
        torsion = GetFloat(data, ref index);
        speed = GetFloat(data, ref index);
    }

}

public class InitializeMessage : BaseObjectMessage
{

    public int locationId = -1;
    public int skinId = -1;
    public int abilityFirstId = -1;
    public int abilitySecondId = -1;
    public int missileId = -1;
    public int missileSkinId = -1;
    public int venomId = -1;

    public InitializeMessage() : base()
    {
    }

    public InitializeMessage(float currentTimestamp, float targetTimemark) : base(currentTimestamp, targetTimemark)
    {
    }

    public override byte[] Pack()
    {
        int index = 0;
        byte[] data = new byte[4 * 10];
        PackBase(ref data, ref index);
        PutInt(data, locationId, ref index);
        PutInt(data, skinId, ref index);
        PutInt(data, abilityFirstId, ref index);
        PutInt(data, abilitySecondId, ref index);
        PutInt(data, missileId, ref index);
        PutInt(data, missileSkinId, ref index);
        PutInt(data, venomId, ref index);
        return data;
    }

    public override void Unpack(byte[] data)
    {
        int index = 0;
        UnpackBase(ref data, ref index);
        locationId = GetInt(data, ref index);
        skinId = GetInt(data, ref index);
        abilityFirstId = GetInt(data, ref index);
        abilitySecondId = GetInt(data, ref index);
        missileId = GetInt(data, ref index);
        missileSkinId = GetInt(data, ref index);
        venomId = GetInt(data, ref index);
    }

}

public class HelloRegionMessage : BaseObjectMessage
{

    public string token = "";

    public HelloRegionMessage() : base()
    {
    }

    public HelloRegionMessage(float currentTimestamp, float targetTimemark) : base(currentTimestamp, targetTimemark)
    {
    }

    public override byte[] Pack()
    {
        int index = 0;
        byte[] data = new byte[4 * 4 + Encoding.UTF8.GetBytes(token).Length];
        PackBase(ref data, ref index);
        PutString(data, token, ref index);
        return data;
    }

    public override void Unpack(byte[] data)
    {
        int index = 0;
        UnpackBase(ref data, ref index);
        token = GetString(data, ref index);
    }

}

public class HelloDuelMessage : BaseObjectMessage
{

    public string token = "";

    public HelloDuelMessage() : base()
    {
    }

    public HelloDuelMessage(float currentTimestamp, float targetTimemark) : base(currentTimestamp, targetTimemark)
    {
    }

    public override byte[] Pack()
    {
        int index = 0;
        byte[] data = new byte[4 * 4 + Encoding.UTF8.GetBytes(token).Length];
        PackBase(ref data, ref index);
        PutString(data, token, ref index);
        return data;
    }

    public override void Unpack(byte[] data)
    {
        int index = 0;
        UnpackBase(ref data, ref index);
        token = GetString(data, ref index);
    }

}

public class RedirectMessage : BaseObjectMessage
{

    public string roomName = "";

    public RedirectMessage() : base()
    {
    }

    public RedirectMessage(float currentTimestamp, float targetTimemark) : base(currentTimestamp, targetTimemark)
    {
    }

    public override byte[] Pack()
    {
        int index = 0;
        byte[] data = new byte[4 * 4 + Encoding.UTF8.GetBytes(roomName).Length];
        PackBase(ref data, ref index);
        PutString(data, roomName, ref index);
        return data;
    }

    public override void Unpack(byte[] data)
    {
        int index = 0;
        UnpackBase(ref data, ref index);
        roomName = GetString(data, ref index);
    }

}

public class VisualEffectMessage : BaseObjectMessage
{

    public int invokerId = -1;
    public int targetId = -1;
    public Vector3 targetPosition;
    public Vector3 direction;
    public float duration = 0.0f;

    public VisualEffectMessage() : base()
    {
    }

    public VisualEffectMessage(float currentTimestamp, float targetTimemark) : base(currentTimestamp, targetTimemark)
    {
    }

    public override byte[] Pack()
    {
        int index = 0;
        byte[] data = new byte[4 * 12];
        PackBase(ref data, ref index);
        PutInt(data, invokerId, ref index);
        PutInt(data, targetId, ref index);
        PutFloat(data, targetPosition.x, ref index);
        PutFloat(data, targetPosition.y, ref index);
        PutFloat(data, targetPosition.z, ref index);
        PutFloat(data, direction.x, ref index);
        PutFloat(data, direction.y, ref index);
        PutFloat(data, direction.z, ref index);
        PutFloat(data, duration, ref index);
        return data;
    }

    public override void Unpack(byte[] data)
    {
        int index = 0;
        UnpackBase(ref data, ref index);
        invokerId = GetInt(data, ref index);
        targetId = GetInt(data, ref index);
        targetPosition.x = GetFloat(data, ref index);
        targetPosition.y = GetFloat(data, ref index);
        targetPosition.z = GetFloat(data, ref index);
        direction.x = GetFloat(data, ref index);
        direction.y = GetFloat(data, ref index);
        direction.z = GetFloat(data, ref index);
        duration = GetFloat(data, ref index);
    }

}

public class GameOverMessage : BaseObjectMessage
{

    public int winner = -1;
    public float time = 0.0f;
    public float damage = 0.0f;
    public float wound = 0.0f;
    public int rank = -1;
    public int rankChange = -1;
    public int rankPoints = -1;
    public int rankPointsChange = -1;
    public int regionUnlocked = -1;

    public GameOverMessage() : base()
    {
    }

    public GameOverMessage(float currentTimestamp, float targetTimemark) : base(currentTimestamp, targetTimemark)
    {
    }

    public override byte[] Pack()
    {
        int index = 0;
        byte[] data = new byte[4 * 12];
        PackBase(ref data, ref index);
        PutInt(data, winner, ref index);
        PutFloat(data, time, ref index);
        PutFloat(data, damage, ref index);
        PutFloat(data, wound, ref index);
        PutInt(data, rank, ref index);
        PutInt(data, rankChange, ref index);
        PutInt(data, rankPointsChange, ref index);
        PutInt(data, rankPointsChange, ref index);
        PutInt(data, regionUnlocked, ref index);
        return data;
    }

    public override void Unpack(byte[] data)
    {
        int index = 0;
        UnpackBase(ref data, ref index);
        winner = GetInt(data, ref index);
        time = GetFloat(data, ref index);
        damage = GetFloat(data, ref index);
        wound = GetFloat(data, ref index);
        rank = GetInt(data, ref index);
        rankChange = GetInt(data, ref index);
        rankPoints = GetInt(data, ref index);
        rankPointsChange = GetInt(data, ref index);
        regionUnlocked = GetInt(data, ref index);
    }

}

public class PingMessage : BaseObjectMessage
{

    public float time = 0.0f;

    public PingMessage() : base()
    {
    }

    public PingMessage(float currentTimestamp, float targetTimemark) : base(currentTimestamp, targetTimemark)
    {
    }

    public override byte[] Pack()
    {
        int index = 0;
        byte[] data = new byte[4 * 4];
        PackBase(ref data, ref index);
        PutFloat(data, time, ref index);
        return data;
    }

    public override void Unpack(byte[] data)
    {
        int index = 0;
        UnpackBase(ref data, ref index);
        time = GetFloat(data, ref index);
    }

}

public class RegionMoveMessage : BaseObjectMessage
{

    public string userId = "";
    public Vector2 destination = new Vector2();
    public float moveTimemark = 0.0f;

    public RegionMoveMessage() : base()
    {
    }

    public RegionMoveMessage(float currentTimestamp, float targetTimemark) : base(currentTimestamp, targetTimemark)
    {
    }

    public override byte[] Pack()
    {
        int index = 0;
        byte[] data = new byte[7 * 4 + Encoding.UTF8.GetBytes(userId).Length];
        PackBase(ref data, ref index);
        PutString(data, userId, ref index);
        PutFloat(data, destination.x, ref index);
        PutFloat(data, destination.y, ref index);
        PutFloat(data, moveTimemark, ref index);
        return data;
    }

    public override void Unpack(byte[] data)
    {
        int index = 0;
        UnpackBase(ref data, ref index);
        userId = GetString(data, ref index);
        destination.x = GetFloat(data, ref index);
        destination.y = GetFloat(data, ref index);
        moveTimemark = GetFloat(data, ref index);
    }

}

public class RegionThrowMessage : BaseObjectMessage
{

    public string userId = "";
    public Vector2 destination = new Vector2();
    public float throwTimemark = 0.0f;

    public RegionThrowMessage() : base()
    {
    }

    public RegionThrowMessage(float currentTimestamp, float targetTimemark) : base(currentTimestamp, targetTimemark)
    {
    }

    public override byte[] Pack()
    {
        int index = 0;
        byte[] data = new byte[7 * 4 + Encoding.UTF8.GetBytes(userId).Length];
        PackBase(ref data, ref index);
        PutString(data, userId, ref index);
        PutFloat(data, destination.x, ref index);
        PutFloat(data, destination.y, ref index);
        PutFloat(data, throwTimemark, ref index);
        return data;
    }

    public override void Unpack(byte[] data)
    {
        int index = 0;
        UnpackBase(ref data, ref index);
        userId = GetString(data, ref index);
        destination.x = GetFloat(data, ref index);
        destination.y = GetFloat(data, ref index);
        throwTimemark = GetFloat(data, ref index);
    }

}

public class RegionChatMessage : BaseObjectMessage
{

    public string userId = "";
    public int iconId = -1;

    public RegionChatMessage() : base()
    {
    }

    public RegionChatMessage(float currentTimestamp, float targetTimemark) : base(currentTimestamp, targetTimemark)
    {
    }

    public override byte[] Pack()
    {
        int index = 0;
        byte[] data = new byte[5 * 4 + Encoding.UTF8.GetBytes(userId).Length];
        PackBase(ref data, ref index);
        PutString(data, userId, ref index);
        PutInt(data, iconId, ref index);
        return data;
    }

    public override void Unpack(byte[] data)
    {
        int index = 0;
        UnpackBase(ref data, ref index);
        userId = GetString(data, ref index);
        iconId = GetInt(data, ref index);
    }

}

public class RegionDiscoverMessage : BaseObjectMessage
{

    public string userId = "";
    public int iconId = -1;
    public int playerGold = -1;

    public RegionDiscoverMessage() : base()
    {
    }

    public RegionDiscoverMessage(float currentTimestamp, float targetTimemark) : base(currentTimestamp, targetTimemark)
    {
    }

    public override byte[] Pack()
    {
        int index = 0;
        byte[] data = new byte[6 * 4 + Encoding.UTF8.GetBytes(userId).Length];
        PackBase(ref data, ref index);
        PutString(data, userId, ref index);
        PutInt(data, iconId, ref index);
        PutInt(data, playerGold, ref index);
        return data;
    }

    public override void Unpack(byte[] data)
    {
        int index = 0;
        UnpackBase(ref data, ref index);
        userId = GetString(data, ref index);
        iconId = GetInt(data, ref index);
        playerGold = GetInt(data, ref index);
    }

}

public class RegionIconMessage : BaseObjectMessage
{

    public Vector2 position = Vector2.zero;
    public int iconId = -1;

    public RegionIconMessage() : base()
    {
    }

    public RegionIconMessage(float currentTimestamp, float targetTimemark) : base(currentTimestamp, targetTimemark)
    {
    }

    public override byte[] Pack()
    {
        int index = 0;
        byte[] data = new byte[6 * 4];
        PackBase(ref data, ref index);
        PutFloat(data, position.x, ref index);
        PutFloat(data, position.y, ref index);
        PutInt(data, iconId, ref index);
        return data;
    }

    public override void Unpack(byte[] data)
    {
        int index = 0;
        UnpackBase(ref data, ref index);
        position.x = GetFloat(data, ref index);
        position.y = GetFloat(data, ref index);
        iconId = GetInt(data, ref index);
    }

}

public class RegionEffectMessage : BaseObjectMessage
{

    public Vector2 position = Vector2.zero;
    public int iconId = -1;

    public RegionEffectMessage() : base()
    {
    }

    public RegionEffectMessage(float currentTimestamp, float targetTimemark) : base(currentTimestamp, targetTimemark)
    {
    }

    public override byte[] Pack()
    {
        int index = 0;
        byte[] data = new byte[6 * 4];
        PackBase(ref data, ref index);
        PutFloat(data, position.x, ref index);
        PutFloat(data, position.y, ref index);
        PutInt(data, iconId, ref index);
        return data;
    }

    public override void Unpack(byte[] data)
    {
        int index = 0;
        UnpackBase(ref data, ref index);
        position.x = GetFloat(data, ref index);
        position.y = GetFloat(data, ref index);
        iconId = GetInt(data, ref index);
    }

}

public class RegionPlayerDataMessage : BaseObjectMessage
{

    public ulong playerId = 0;
    public uint cloth = 0;
    public uint gold = 0;
    public uint sessionGold = 0;
    public uint emerald = 0;
    public uint ratingPoints = 0;
    public uint loot = 0;

    public RegionPlayerDataMessage() : base()
    {
    }

    public RegionPlayerDataMessage(float currentTimestamp, float targetTimemark) : base(currentTimestamp, targetTimemark)
    {
    }

    public override byte[] Pack()
    {
        int index = 0;
        byte[] data = new byte[6 * 4];
        PackBase(ref data, ref index);
        PutULong(data, playerId, ref index);
        PutUInt(data, cloth, ref index);
        PutUInt(data, gold, ref index);
        PutUInt(data, sessionGold, ref index);
        PutUInt(data, emerald, ref index);
        PutUInt(data, ratingPoints, ref index);
        PutUInt(data, loot, ref index);
        return data;
    }

    public override void Unpack(byte[] data)
    {
        int index = 0;
        UnpackBase(ref data, ref index);
        playerId = GetULong(data, ref index);
        cloth = GetUInt(data, ref index);
        gold = GetUInt(data, ref index);
        sessionGold = GetUInt(data, ref index);
        emerald = GetUInt(data, ref index);
        ratingPoints = GetUInt(data, ref index);
        loot = GetUInt(data, ref index);
    }

}

public class PlayerViewMessage : BaseObjectMessage
{

    public ulong playerId = 0;
    public string nickname = "";
    public string country = "";
    public uint gold = 0;
    public uint emerald = 0;
    public uint ratingPoints = 0;
    public uint cloth = 0;
    public uint weapon = 0;
    public uint weaponSkin = 0;

    public PlayerViewMessage() : base()
    {
    }

    public PlayerViewMessage(float currentTimestamp, float targetTimemark) : base(currentTimestamp, targetTimemark)
    {
    }

    public override byte[] Pack()
    {
        int index = 0;
        byte[] data = new byte[0];
        return data;
    }

    public override void Unpack(byte[] data)
    {
        int index = 0;
        //UnpackBase(ref data, ref index);
        playerId = GetULong(data, ref index);
        nickname = GetSString(data, ref index);
        country = GetSString(data, ref index);
        gold = GetUInt(data, ref index);
        emerald = GetUInt(data, ref index);
        ratingPoints = GetUInt(data, ref index);
        cloth = GetUInt(data, ref index);
        weapon = GetUInt(data, ref index);
        weaponSkin = GetUInt(data, ref index);
    }

}

public class NearbyPlayerNode
{
    public long playerId = 0;
    public long distance = 0;
}

public class NearbyPlayersListMessage : BaseObjectMessage
{

    public LinkedList<NearbyPlayerNode> list;

    public NearbyPlayersListMessage() : base()
    {
        list = new LinkedList<NearbyPlayerNode>();
    }

    public NearbyPlayersListMessage(float currentTimestamp, float targetTimemark) : base(currentTimestamp, targetTimemark)
    {
    }

    public override byte[] Pack()
    {
        int index = 0;
        byte[] data = new byte[0];
        return data;
    }

    public override void Unpack(byte[] data)
    {
        Debug.Log(data);
        int i;
        NearbyPlayerNode node;
        int index = 2;
        int amount = (int)GetUInt(data, ref index);
        for(i = 0; i < amount; i++)
        {
            node = new NearbyPlayerNode();
            node.playerId = (long)GetULong(data, ref index);
            node.distance = (long)GetULong(data, ref index);
            list.AddLast(node);
        }
    }

}

