using UnityEngine;
using System.Collections;

public class SwipeTrailController : MonoBehaviour {

    public LineRenderer lineRenderer;
    public Color startColor = Color.white;
    public Color endColor = Color.white;

    public int pointsCount = 1;
    public Vector3[] points = new Vector3[1];
    public float cooldown = 2.0f;

    public Vector3 lastParentPosition = Vector3.zero;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        int i;
        Vector3 v3Delta;
        if (cooldown > 0.0f)
        {
            cooldown -= Time.deltaTime;
            startColor.a = Mathf.Max(0.0f, Mathf.Min(1.0f, cooldown * 2.0f));
            endColor.a = Mathf.Max(0.0f, Mathf.Min(1.0f, cooldown * 2.0f - 0.5f));
            lineRenderer.SetColors(startColor, endColor);
            if(cooldown < 0.0f)
            {
                Destroy(gameObject);
            }
        }
        v3Delta = transform.parent.position - lastParentPosition;
        for (i = 0; i < points.Length; i++)
        {
            points[i] += v3Delta;
        }
        lineRenderer.SetPositions(points);
        lastParentPosition = transform.parent.position;

    }
}
