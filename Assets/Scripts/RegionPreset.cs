#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public class RegionPreset : MonoBehaviour {

    public Color ambientColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);

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
        Shader.SetGlobalFloat("_ScreenRatio", (float)Screen.height / (float)Screen.width);
    }

    void Update () {
	
	}
}
