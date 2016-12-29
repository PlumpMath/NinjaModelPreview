using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI.Extensions;
using System.Collections;
using System.Collections.Generic;

public class MapController : MonoBehaviour {

    public Button settingsButton;
    public Button playerObject;
    public Image[] mapObjects = new Image[0];
    public MapPoint[] mapPoints = new MapPoint[0];
    public Button[] mapRegions = new Button[0];
    public UILineTextureRenderer pathRenderer;

    public MapPoint movingBasePoint;
    public MapPoint movingLastPoint;
    public MapPoint movingDestinationPoint;
    public MapRoute movingRoute;
    public float movingCooldown = 0.0f;

    // Use this for initialization
    void Start () {

        int i;
        string currentRegion = PlayerPrefs.GetString("CurrentRegion", "01");
        string enteringPoint = PlayerPrefs.GetString("CurrentPoint", "S");
        string[] playerPoint;// = PlayerPrefs.GetString("MapPlayerPoint", "01_03").Split('_');
        string[] mapPointName;

        PlayerPrefs.SetInt("MapObjectState_01_1", 1);

        settingsButton.onClick.AddListener(delegate () {
            PlayerPrefs.SetString("CurrentRegion", "01");
            PlayerPrefs.SetString("CurrentPoint", "S");
            PlayerPrefs.SetInt("MapObjectState_01_1", 1);
            for (i = 1; i < mapObjects.Length; i++)
            {
                if (i < 4)
                {
                    PlayerPrefs.SetInt("MapObjectState_01_" + (i + 1), 0);
                }
                else
                {
                    PlayerPrefs.SetInt("MapObjectState_02_" + (i - 3), 0);
                }
            }
            for (i = 0; i < mapPoints.Length; i++)
            {
                mapPointName = mapPoints[i].name.Split('_');
                if ((mapPointName[1] == currentRegion && mapPointName[3] == enteringPoint) || (mapPointName[2] == currentRegion && mapPointName[4] == enteringPoint))
                {
                    playerObject.image.rectTransform.anchoredPosition = mapPoints[i].rectTransform.anchoredPosition;
                }
            }
            for (i = 0; i < mapObjects.Length; i++)
            {
                string[] strArr = mapObjects[i].name.Split('_');
                if (PlayerPrefs.GetInt("MapObjectState_" + strArr[1] + "_" + strArr[2], 0) == 1)
                {
                    mapObjects[i].enabled = true;
                }
                else
                {
                    mapObjects[i].enabled = false;
                }
            }
        });

        for (i = 0; i < mapPoints.Length; i++)
        {
            mapPointName = mapPoints[i].name.Split('_');
            if ((mapPointName[1] == currentRegion && mapPointName[3] == enteringPoint) || (mapPointName[2] == currentRegion && mapPointName[4] == enteringPoint))
            {
                playerObject.image.rectTransform.anchoredPosition = mapPoints[i].rectTransform.anchoredPosition;
            }
        }
        for (i = 0; i < mapObjects.Length; i++)
        {
            string[] strArr = mapObjects[i].name.Split('_');
            if(PlayerPrefs.GetInt("MapObjectState_" + strArr[1] + "_" + strArr[2], 0) == 1)
            {
                mapObjects[i].enabled = true;
            }
            else
            {
                mapObjects[i].enabled = false;
            }
        }

        mapRegions[0].onClick.AddListener(delegate () {
            MoveToRegion("01");
            //PlayerPrefs.SetString("CurrentRegion", "01");
            //PlayerPrefs.SetString("CurrentPoint", "1");
            //SceneManager.LoadScene("region");
        });
        mapRegions[1].onClick.AddListener(delegate () {
            MoveToRegion("02");
        });
        mapRegions[2].onClick.AddListener(delegate () {
            MoveToRegion("03");
        });


        playerObject.onClick.AddListener(delegate() {
            SceneManager.LoadScene("region");
        });
        
    }
	
	// Update is called once per frame
	void Update () {

        int i;
        int l;
        Vector2[] points;
        MapPoint point;
        LinkedListNode<MapPoint> pointNode;
        if (movingCooldown > 0.0f)
        {
            movingCooldown -= Time.deltaTime * 0.2f;
            if(movingCooldown <= 0.0f)
            {
                movingRoute.route.RemoveFirst();
                if(movingRoute.route.Count < 2)
                {
                    string[] mapPointName = movingRoute.route.Last.Value.name.Split('_');
                    PlayerPrefs.SetString("CurrentRegion", movingRoute.targetRegion);
                    if(mapPointName[1] == movingRoute.targetRegion)
                    {
                        PlayerPrefs.SetString("CurrentPoint", mapPointName[3]);
                    }
                    else if(mapPointName[2] == movingRoute.targetRegion)
                    {
                        PlayerPrefs.SetString("CurrentPoint", mapPointName[4]);
                    }
                    else
                    {
                        PlayerPrefs.SetString("CurrentPoint", "S");
                    }
                    SceneManager.LoadScene("region");
                }
                else
                {
                    movingLastPoint = movingRoute.route.First.Value;
                    movingCooldown = 1.0f;
                }
            }
            else
            {
                l = movingRoute.route.Count;
                points = new Vector2[l];
                i = 0;
                pointNode = movingRoute.route.First;
                while (pointNode != null)
                {
                    point = pointNode.Value;
                    points[points.Length - 1 - i] = new Vector2(point.rectTransform.anchoredPosition.x, point.rectTransform.anchoredPosition.y);
                    i++;
                    pointNode = pointNode.Next;
                }
                playerObject.image.rectTransform.anchoredPosition = movingRoute.route.First.Value.rectTransform.anchoredPosition + (movingRoute.route.First.Next.Value.rectTransform.anchoredPosition - movingRoute.route.First.Value.rectTransform.anchoredPosition) * (1.0f - movingCooldown);
                points[points.Length - 1] = playerObject.image.rectTransform.anchoredPosition;
                pathRenderer.Points = points;

                if (movingCooldown < 0.5f && movingCooldown + Time.deltaTime * 0.2f >= 0.5f)
                {
                    if(Random.Range(0.0f, 1.0f) > 0.5f)
                    {
                        PlayerPrefs.SetFloat("RegionLastX", -32000.0f);
                        PlayerPrefs.SetFloat("RegionLastY", 0.0f);
                        PlayerPrefs.SetFloat("EnemyAdvantage", Random.Range(0.0f, 1.0f));
                        SceneManager.LoadScene("battle");
                    }
                }

            }
        }

	}

    public void MoveToRegion (string regionId)
    {
        if(movingCooldown > 0.0f)
        {
            return;
        }
        string[] mapPointName;
        LinkedList<MapPoint> points = new LinkedList<MapPoint>();
        MapPoint currentPoint = GetCurrentMapPoint();
        movingRoute = GetShortestRoute(ref points, currentPoint, regionId);
        if (movingRoute.route.Count < 2)
        {
            PlayerPrefs.SetString("CurrentRegion", movingRoute.targetRegion);
            mapPointName = movingRoute.route.Last.Value.name.Split('_');
            if (mapPointName[1] == movingRoute.targetRegion)
            {
                PlayerPrefs.SetString("CurrentPoint", mapPointName[3]);
            }
            else if (mapPointName[2] == movingRoute.targetRegion)
            {
                PlayerPrefs.SetString("CurrentPoint", mapPointName[4]);
            }
            else
            {
                PlayerPrefs.SetString("CurrentPoint", "S");
            }
            SceneManager.LoadScene("region");
            return;
        }
        movingBasePoint = currentPoint;
        movingLastPoint = movingBasePoint;
        movingDestinationPoint = movingRoute.route.Last.Value;
        movingCooldown = 1.0f;
    }

    public MapPoint GetCurrentMapPoint()
    {
        int i;
        string regionId = PlayerPrefs.GetString("CurrentRegion", "01");
        string enteringPoint = PlayerPrefs.GetString("CurrentPoint", "01");
        string[] mapPointsName;
        for (i = 0; i < mapPoints.Length; i++)
        {
            mapPointsName = mapPoints[i].name.Split('_');
            if ((mapPointsName[1] == regionId && mapPointsName[3] == enteringPoint) || (mapPointsName[2] == regionId && mapPointsName[4] == enteringPoint))
            {
                return mapPoints[i];
            }
        }
        return null;
    }

    public MapRoute GetShortestRoute(ref LinkedList<MapPoint> points, MapPoint currentPoint, string regionId)
    {
        MapRoute returnRoute = null;
        LinkedListNode<MapPoint> pointNode;
        if (currentPoint.name.IndexOf(regionId) > -1)
        {
            returnRoute = new MapRoute();
            pointNode = points.First;
            while(pointNode != null)
            {
                returnRoute.route.AddLast(pointNode.Value);
                pointNode = pointNode.Next;
            }
            returnRoute.route.AddLast(currentPoint);
            returnRoute.targetRegion = regionId;
            return returnRoute;
        }
        LinkedList<MapPoint> routes = new LinkedList<MapPoint>();
        MapRoute route = null;
        if(currentPoint.north != null)
        {
            routes.AddLast(currentPoint.north);
        }
        if (currentPoint.northEast != null)
        {
            routes.AddLast(currentPoint.northEast);
        }
        if (currentPoint.northWest != null)
        {
            routes.AddLast(currentPoint.northWest);
        }
        if (currentPoint.east != null)
        {
            routes.AddLast(currentPoint.east);
        }
        if (currentPoint.west != null)
        {
            routes.AddLast(currentPoint.west);
        }
        if (currentPoint.southEast != null)
        {
            routes.AddLast(currentPoint.southEast);
        }
        if (currentPoint.southWest != null)
        {
            routes.AddLast(currentPoint.southWest);
        }
        if (currentPoint.south != null)
        {
            routes.AddLast(currentPoint.south);
        }
        points.AddLast(currentPoint);
        pointNode = routes.First;
        while(pointNode != null)
        {
            if (!points.Contains(pointNode.Value))
            {
                route = GetShortestRoute(ref points, pointNode.Value, regionId);
                if (route != null)
                {
                    if (returnRoute == null || returnRoute.GetLength() > route.GetLength())
                    {
                        returnRoute = route;
                    }
                }
            }
            pointNode = pointNode.Next;
        }
        points.RemoveLast();
        return returnRoute;
    }


}

public class MapRoute
{
    public string targetRegion = "";
    public LinkedList<MapPoint> route = new LinkedList<MapPoint>();
    public int GetLength()
    {
        return route.Count;
    }
}



