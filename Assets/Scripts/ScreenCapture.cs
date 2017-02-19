using UnityEngine;
using System.Collections;

public class ScreenCapture : MonoBehaviour {

    public LayerMask fullLayers;
    public LayerMask solidLayers;
    public LayerMask normalLayers;
    public Camera camera;
    public RenderTexture fullCapture;
    public RenderTexture solidCapture;
    public RenderTexture normalCapture;
    public Shader normalShader;

    public Material[] materials = new Material[0];

    private Camera subCamera;

    // Use this for initialization
    void Start () {

        try
        {

            GameObject subCameraObj = new GameObject();
            subCameraObj.hideFlags = HideFlags.DontSave;
            subCameraObj.transform.parent = transform;
            subCameraObj.transform.localPosition = Vector3.zero;
            subCameraObj.transform.localRotation = Quaternion.identity;
            subCamera = subCameraObj.AddComponent<Camera>();

            solidCapture = new RenderTexture(Screen.width / 2, Screen.height / 2, 16, RenderTextureFormat.Default);
            solidCapture.wrapMode = TextureWrapMode.Repeat;
            solidCapture.Create();
        }
        catch(System.Exception ex)
        {

        }

    }

    void OnDispose()
    {
        OnDisable();
    }

    void OnDisable()
    {
        solidCapture.Release();
    }

    /*
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {

        

        Graphics.Blit(source, destination);

    }
    */

    // Update is called once per frame
    void Update () {

        try
        {

            int i;

            subCamera.CopyFrom(camera);
            subCamera.clearFlags = CameraClearFlags.Color;
            subCamera.backgroundColor = Color.black;

            /*
            fullCapture = new RenderTexture(source.width, source.height, 16, RenderTextureFormat.Default);
            fullCapture.wrapMode = TextureWrapMode.Repeat;
            fullCapture.Create();

            subCamera.cullingMask = fullLayers.value;
            subCamera.targetTexture = fullCapture;
            subCamera.Render();
            */

            /*
            solidCapture = new RenderTexture(source.width, source.height, 16, RenderTextureFormat.Default);
            solidCapture.wrapMode = TextureWrapMode.Repeat;
            solidCapture.Create();
            */

            subCamera.cullingMask = solidLayers.value;
            subCamera.targetTexture = solidCapture;
            subCamera.Render();

            /*
            normalCapture = new RenderTexture(source.width, source.height, 16, RenderTextureFormat.Default);
            normalCapture.wrapMode = TextureWrapMode.Repeat;
            normalCapture.Create();

            subCamera.cullingMask = normalLayers.value;
            subCamera.targetTexture = normalCapture;
            subCamera.RenderWithShader(normalShader, "");
            */

            for (i = 0; i < materials.Length; i++)
            {
                //materials[i].SetTexture("_FullScreen", fullCapture);
                materials[i].SetTexture("_SolidScreen", solidCapture);
                //materials[i].SetTexture("_NormalScreen", normalCapture);
            }

            //fullCapture.Release();
            //solidCapture.Release();
            //normalCapture.Release();

        }
        catch (System.Exception ex)
        {

        }

    }
}
