using UnityEngine;
using System.Collections;

public class ArmedMissileController : MonoBehaviour {

    public MeshRenderer[] meshes;
    public Vector3 anchor = new Vector3(0.0f, -1.5f, 3.0f);

    private Vector3 baseAnchor = new Vector3();
    private float margin = -1.0f;

	// Use this for initialization
	void Start () {
        baseAnchor = anchor;
        Rearm();
    }
	
	// Update is called once per frame
	void Update () {
        if(margin < 0.0f)
        {
            margin += Time.deltaTime * 4.0f;
            if(margin > 0.0f)
            {
                margin = 0.0f;
            }
        }
        transform.localPosition += (anchor + Vector3.up * margin - transform.localPosition) * Mathf.Min(1.0f, Time.deltaTime * 30.0f);
    }

    public void Rearm() {
        margin = -1.0f;
        ResetAnchor();
        transform.localPosition += (anchor + Vector3.up * margin - transform.localPosition);
        transform.localRotation = Quaternion.identity;
        transform.Rotate(40.0f, 0.0f, 0.0f);
    }

    public void SetAnchor(Vector2 position, float time)
    {
        anchor.x = baseAnchor.x + (position.x - 0.5f) * 0.67f * (1.0f + Mathf.Pow(position.y, 2.0f) * 5.5f);
        anchor.y = baseAnchor.y + 0.07f + position.y * 1.0f;
        anchor.z = baseAnchor.z - 0.5f + Mathf.Pow(position.y, 2.0f) * 12.0f;
    }

    public void ResetAnchor()
    {
        anchor.x = baseAnchor.x;
        anchor.y = baseAnchor.y;
        anchor.z = baseAnchor.z;
    }

    public void SetMissile(int id)
    {
        int i;
        for (i = 1; i < meshes.Length; i++)
        {
            if (i == id)
            {
                meshes[i].enabled = true;
            }
            else
            {
                meshes[i].enabled = false;
            }
        }
    }

    public int GetCurrentMissile()
    {
        int i;
        for(i = 1; i < meshes.Length; i++)
        {
            if(meshes[i].enabled)
            {
                return i;
            }
        }
        return 1;
    }

    public void SetNextMissile()
    {
        int id = GetCurrentMissile();
        id++;
        if(id >= meshes.Length)
        {
            id = 1;
        }
        SetMissile(id);
    }

    public void SetPreviousMissile()
    {
        int id = GetCurrentMissile();
        id--;
        if (id <= 0)
        {
            id = meshes.Length - 1;
        }
        SetMissile(id);
    }

}
