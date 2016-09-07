using UnityEngine;
using System.Collections;

public class TerrainHolder : MonoBehaviour {

	// Use this for initialization
    private PointLight[] pointLight;
    private int prevLen = 0;
    private MeshRenderer renderer;

    void Start() {
        renderer = gameObject.GetComponent<MeshRenderer>();
    }

    void updateLight() {
        pointLight = GameObject.FindObjectsOfType<PointLight>();
    }

    void Update() {
        updateLight();

        if (pointLight.Length > 0) {
            linkLight(renderer.material, "_PointLight", pointLight);
            prevLen = pointLight.Length;
        } else {
            unlinkLight(renderer.material, "_PointLight", prevLen);
            prevLen = 0;
        }
    }

    public static void unlinkLight(Material material, string name, int len) {
        for (int i = 0; i < len; i++) {
            material.SetColor(name + "Color" + i.ToString(), Color.black);
            material.SetVector(name + "Position" + i.ToString(), Vector3.zero);
        }
    }

    public static void linkLight(Material material, string name, PointLight[] array) {
        for (int i = 0; i < array.Length; i++) {
            material.SetColor(name + "Color" + i.ToString(), array[i].color);
            material.SetVector(name + "Position" + i.ToString(), array[i].GetWorldPosition());
        }
    }
}
