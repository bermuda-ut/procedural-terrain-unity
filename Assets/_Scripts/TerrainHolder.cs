using UnityEngine;
using System.Collections;

public class TerrainHolder : MonoBehaviour {

	// Use this for initialization
    private PointLight[] pointLight;
    private MeshRenderer renderer;

    void Start() {
        renderer = gameObject.GetComponent<MeshRenderer>();
    }

    void Update() {
        pointLight = GameObject.FindObjectsOfType<PointLight>();

        linkLight(renderer.material, "_PointLight", pointLight);
    }

    public static void linkLight(Material material, string name, PointLight[] array) {
        for (int i = 0; i < array.Length; i++) {
            material.SetColor(name + "Color" + i, array[i].color);
            material.SetVector(name + "Position" + i, array[i].GetWorldPosition());
        };
    }
}
