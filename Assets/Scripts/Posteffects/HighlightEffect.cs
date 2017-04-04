using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class HighlightEffect : MonoBehaviour
{

    public LayerMask layers;
    public Vector2 bloomSpread = new Vector2(0.001f, 0.001f);
    public Camera camera;
    public Camera subCamera;
    public Material material;
    public Material material2;
    public Shader highlightShader;
    public RenderTexture smallTex;
    public RenderTexture fullTex;
    public RenderTexture fullBloomTex;

    void Start()
    {
        if (subCamera != null)
        {
            GameObject.DestroyImmediate(subCamera.gameObject);
        }
        subCamera = new GameObject().AddComponent<Camera>();
        subCamera.name = "Highlight Camera";
        subCamera.enabled = false;
        //diffuseShader = Shader.Find("Mobile/Diffuse");
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

        fullTex = new RenderTexture(source.width, source.height, 16, RenderTextureFormat.Default);
        fullTex.wrapMode = TextureWrapMode.Clamp;
        fullTex.filterMode = FilterMode.Trilinear;
        fullTex.useMipMap = true;
        //fullTex.generateMips = true;
        fullTex.Create();

        subCamera.targetTexture = fullTex;
        subCamera.RenderWithShader(highlightShader, "");
        subCamera.targetTexture = null;

        material2.SetFloat("_SpreadX", bloomSpread.x);
        material2.SetFloat("_SpreadY", bloomSpread.y);

        fullBloomTex = new RenderTexture(source.width / 4, source.height / 4, 0, RenderTextureFormat.Default);
        fullBloomTex.wrapMode = TextureWrapMode.Clamp;
        fullBloomTex.filterMode = FilterMode.Trilinear;
        fullBloomTex.useMipMap = true;
        //fullBloomTex.generateMips = true;
        fullBloomTex.Create();

        Graphics.Blit(fullTex, fullBloomTex, material2);

        smallTex = new RenderTexture(source.width / 8, source.height / 8, 0, RenderTextureFormat.Default);
        smallTex.wrapMode = TextureWrapMode.Clamp;
        smallTex.filterMode = FilterMode.Trilinear;
        smallTex.useMipMap = true;
        //smallTex.generateMips = true;
        smallTex.Create();

        Graphics.Blit(fullBloomTex, smallTex, material2);

        material.SetTexture("_BloomTex", smallTex);

        Graphics.Blit(source, destination, material);

        fullTex.Release();
        fullBloomTex.Release();
        smallTex.Release();


    }

}