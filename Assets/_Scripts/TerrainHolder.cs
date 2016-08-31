using UnityEngine;
using System.Collections;

public class TerrainHolder : MonoBehaviour {

	// Use this for initialization
    public PointLight pointLight;

    void Update() {
        MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
        renderer.material.SetColor("_PointLightColor", pointLight.color);
        renderer.material.SetVector("_PointLightPosition", pointLight.GetWorldPosition());
    }
}
