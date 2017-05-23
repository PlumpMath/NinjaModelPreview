#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public class RegionPreset : MonoBehaviour {

    public Color ambientColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    public Texture2D ambientPalette;
    public Texture2D fogTexture;
    public Color fogColor;

    public LinkedList<RegionCollider> colliders = new LinkedList<RegionCollider>();

#if UNITY_EDITOR

    private bool inEditorUpdated = false;

    RegionPreset()
    {
        inEditorUpdated = false;
        EditorApplication.update += InEditorUpdate;
    }

    void InEditorUpdate()
    {
        if (!inEditorUpdated)
        {
            inEditorUpdated = true;
            UpdatePreset();
        }
    }

#endif

    void Start() {
        UpdatePreset();
    }

    void UpdatePreset() {
        Light light = GameObject.Find("Directional Light").GetComponent<Light>();
        light.color = ambientColor;
        Shader.SetGlobalColor("_AmbientLight", ambientColor);
        Shader.SetGlobalTexture("_AmbientPalette", ambientPalette);
        Shader.SetGlobalFloat("_ScreenRatio", (float)Screen.height / (float)Screen.width);
        Shader.SetGlobalTexture("_FogTex", fogTexture);
        Shader.SetGlobalColor("_FogColor", fogColor);
    }

    void Update () {
	
	}

    public void RegisterCollider(RegionColliderController obj)
    {
        RegionCollider collider = new RegionCollider();
        collider.position = obj.transform.position;
        collider.position.y = 0.0f;
        collider.radius = obj.transform.localScale.x;
        colliders.AddLast(collider);
    }

    public LinkedList<RoutePoint> GetRoute(Vector3 origin, Vector3 destination, float speed, float timeOffset)
    {
        float totalTimeOffset = timeOffset;
        RoutePoint nextPoint;
        RoutePoint lastPoint = null;
        RegionCollider collider;
        LinkedList<RoutePoint> route = new LinkedList<RoutePoint>();
        LinkedList<RegionCollider> nearestColliders = new LinkedList<RegionCollider>();
        LinkedListNode<RegionCollider> colliderNode = colliders.First;
        Vector3 middlePoint = (origin + destination) * 0.5f;
        Vector3 newOrigin;
        Vector3 v3;
        float nearestRadius = (middlePoint - origin).magnitude + 6.0f;
        while(colliderNode != null)
        {
            if((colliderNode.Value.position - middlePoint).magnitude <= nearestRadius)
            {
                nearestColliders.AddLast(colliderNode.Value);
            }
            colliderNode = colliderNode.Next;
        }
        nextPoint = GetNextRouteStep(origin, destination, nearestColliders, speed, totalTimeOffset, false);
        if(nextPoint == null)
        {
            nextPoint = GetNextRouteStep(origin, destination, nearestColliders, speed, totalTimeOffset, true);
        }
        lastPoint = nextPoint;
        while(nextPoint != null)
        {
            totalTimeOffset += nextPoint.timestamp - (Time.time + totalTimeOffset);
            if(route.Count > 1)
            {
                if(Vector3.Angle(route.Last.Value.destination - route.Last.Previous.Value.destination, nextPoint.destination - route.Last.Value.destination) > 90.0f)
                {
                    return route;
                }
            }
            if ((collider = CheckCollision(nextPoint.destination, nearestColliders)) != null)
            {
                v3 = nextPoint.destination - lastPoint.destination;
                if(v3.magnitude == 0.0f)
                {
                    v3.z += 0.001f;
                }
                nextPoint.destination = lastPoint.destination + v3.normalized * ((collider.position - lastPoint.destination).magnitude - (collider.radius + 0.01f));
                route.AddLast(nextPoint);
                return route;
            }
            route.AddLast(nextPoint);
            newOrigin = nextPoint.destination;
            lastPoint = nextPoint;
            nextPoint = GetNextRouteStep(newOrigin, destination, nearestColliders, speed, totalTimeOffset, false);
            if(nextPoint == null)
            {
                nextPoint = GetNextRouteStep(newOrigin, destination, nearestColliders, speed, totalTimeOffset, true);
            }
            if (route.Count > 30)
            {
                //Debug.Log("Route.Count > 30");
                return route;
            }
        }
        return route;
    }

    public RoutePoint GetNextRouteStep(Vector3 origin, Vector3 destination, LinkedList<RegionCollider> nearestColliders, float speed, float timeoffset, bool alternateWay)
    {
        if((origin - destination).magnitude <= 0.01f)
        {
            return null;
        }
        float f;
        float colliderDistance = 10000.0f;
        Vector3 v3;
        Vector3 v3g;
        Vector3 v3m;
        Vector3 baseDirection = destination - origin;
        Vector3 baseDirectionNormalized = baseDirection.normalized;
        float baseDirectionMagnitude = baseDirection.magnitude;
        RoutePoint point = new RoutePoint();
        RegionCollider nearestCollider = null;
        LinkedListNode<RegionCollider> colliderNode = nearestColliders.First;

        /*
        if((nearestCollider = CheckCollision(origin, nearestColliders)) != null)
        {
            point.destination = nearestCollider.position + (origin - nearestCollider.position).normalized * (nearestCollider.radius + 0.01f);
            point.timestamp = Time.time + timeoffset + (point.destination - origin).magnitude / speed;
            return point;
        }
        */

        while (colliderNode != null)
        {
            v3 = baseDirectionNormalized * (colliderNode.Value.position - origin).magnitude;
            f = (origin + v3 - colliderNode.Value.position).magnitude;
            v3g = new Vector3(v3.z, v3.y, -v3.x);
            if(Vector3.Angle(colliderNode.Value.position + v3g.normalized * 0.01f - origin, baseDirection) > Vector3.Angle(colliderNode.Value.position - origin, baseDirection))
            {
                v3g *= -1.0f;
            }
            if (alternateWay)
            {
                v3 = colliderNode.Value.position - v3g.normalized * (colliderNode.Value.radius + 0.01f); // (v3 - colliderNode.Value.position).normalized * (colliderNode.Value.radius + 0.2f);
            }
            else
            {
                v3 = colliderNode.Value.position + v3g.normalized * (colliderNode.Value.radius + 0.01f); // (v3 - colliderNode.Value.position).normalized * (colliderNode.Value.radius + 0.2f);
            }
            if (f <= colliderNode.Value.radius && (v3 - origin).magnitude < baseDirectionMagnitude * 2.0f && (v3 - origin).magnitude <= colliderDistance)
            {
                /*
                if (alternateWay)
                {
                    v3 = colliderNode.Value.position - (v3 - colliderNode.Value.position).normalized * (colliderNode.Value.radius + 0.2f);
                }
                else
                {
                    v3 = colliderNode.Value.position + (v3 - colliderNode.Value.position).normalized * (colliderNode.Value.radius + 0.2f);
                }
                */
                //if (CheckCollision(v3, nearestColliders) == null)
                //{
                    nearestCollider = colliderNode.Value;
                    colliderDistance = (v3 - origin).magnitude;
                //}
            }
            colliderNode = colliderNode.Next;
        }
        if(nearestCollider != null)
        {
            v3 = baseDirectionNormalized * (nearestCollider.position - origin).magnitude;
            v3g = new Vector3(v3.z, v3.y, -v3.x);
            if (Vector3.Angle(nearestCollider.position + v3g.normalized * 0.1f - origin, baseDirection) > Vector3.Angle(nearestCollider.position - origin, baseDirection))
            {
                v3g *= -1.0f;
            }
            if (alternateWay)
            {
                v3 = nearestCollider.position - v3g.normalized * (nearestCollider.radius + 0.01f); // (v3 - nearestCollider.position).normalized * (nearestCollider.radius + 0.2f);
            }
            else
            {
                v3 = nearestCollider.position + v3g.normalized * (nearestCollider.radius + 0.01f); // (v3 - nearestCollider.position).normalized * (nearestCollider.radius + 0.2f);
            }
            v3m = origin + (v3 - origin).normalized * nearestCollider.radius * 0.7f;
            if ((nearestCollider.position - v3m).magnitude < nearestCollider.radius)
            {
                v3 = nearestCollider.position + (v3m - nearestCollider.position).normalized * (nearestCollider.radius + 0.01f);
            }
            point.destination = v3;

            if ((nearestCollider = CheckCollision(origin, nearestColliders)) != null)
            {
                v3 = nearestCollider.position + (origin - nearestCollider.position).normalized * (nearestCollider.radius + 0.01f);
                f = Mathf.Min(1.0f, Mathf.Max(0.0f, (nearestCollider.radius - (origin - nearestCollider.position).magnitude) * 5.0f));
                point.destination = v3 * f + point.destination * (1.0f - f);
            }

            point.timestamp = Time.time + timeoffset + (point.destination - origin).magnitude / speed;
            return point;
        }
        point.destination = destination;
        point.timestamp = Time.time + timeoffset + baseDirectionMagnitude / speed;
        return point;
    }

    public RegionCollider CheckCollision(Vector3 point, LinkedList<RegionCollider> list)
    {
        LinkedListNode<RegionCollider> node = list.First;
        while(node != null)
        {
            if((point - node.Value.position).magnitude < node.Value.radius)
            {
                return node.Value;
            }
            node = node.Next;
        }
        return null;
    }

}

public class RegionCollider
{
    public Vector3 position = Vector3.zero;
    public float radius = 1.0f;
}

public class RoutePoint
{
    public Vector3 destination = Vector3.zero;
    public float timestamp = 0.0f;
}

