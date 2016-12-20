using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class FogEffect : MonoBehaviour
{

    public LayerMask layers;
    public Camera camera;
    public Camera subCamera;
    public Material material;
    public Shader depthShader;
    public RenderTexture depthTex;

    void Start()
    {
        //material = new Material(Shader.Find("Fog/FogEffect"));
        camera = GetComponent<Camera>();
        if(subCamera != null)
        {
            GameObject.DestroyImmediate(subCamera.gameObject);
        }
        subCamera = new GameObject().AddComponent<Camera>();
        subCamera.name = "Depth Camera";
        subCamera.enabled = false;
        depthShader = Shader.Find("Fog/FogDepth");
#if UNITY_EDITOR
        Shader.EnableKeyword("INVERT_Y");
#else
        Shader.DisableKeyword("INVERT_Y");
#endif
    }

    void Awake()
    {
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {

        subCamera.CopyFrom(camera);
        subCamera.clearFlags = CameraClearFlags.Color;
        subCamera.cullingMask = layers.value;
        subCamera.backgroundColor = Color.black;
        subCamera.targetTexture = depthTex;

        depthTex = new RenderTexture(source.width, source.height, 16, RenderTextureFormat.Default);
        depthTex.wrapMode = TextureWrapMode.Repeat;
        depthTex.Create();

        subCamera.targetTexture = depthTex;
        subCamera.RenderWithShader(depthShader, "");

        //material.SetFloat("_Blend", intensity);
        //material.SetTexture("_FogTex", fogTex);
        //material.SetColor("_FogColor", fogColor);
        material.SetTexture("_Depth", depthTex);
        Graphics.Blit(source, destination, material);

        depthTex.Release();


    }

}