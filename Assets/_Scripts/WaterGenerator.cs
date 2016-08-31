using UnityEngine;
using System.Collections;

public class WaterGenerator : MonoBehaviour {


    private int size;
    private float xTileSize;
    private float zTileSize;

    private PointLight pointLight;
    private float waterHeight = 0;

	void Start () {
        pointLight = FindObjectOfType<PointLight>().GetComponent<PointLight>();

	    GameObject p = gameObject.transform.parent.gameObject;
	    int n = p.GetComponent<TerrainGenerator>().n;
	    xTileSize = p.GetComponent<TerrainGenerator>().xTileSize;
	    zTileSize = p.GetComponent<TerrainGenerator>().zTileSize;
	    size = (int) Mathf.Pow(2, n) + 1;

	    float[][] a = GenerateWaterHeight(2, waterHeight);

        MeshFilter waterMesh = gameObject.AddComponent<MeshFilter>();
	    waterMesh.mesh = MeshGenerator.PlaneFromArray(a, xTileSize*(size-1), zTileSize*(size-1), 0, 0, true, Vector3.up, MeshGenerator.HexToColor("039be5", 120))[0];
        MeshRenderer waterRenderer = gameObject.AddComponent<MeshRenderer>();

        waterRenderer.material.shader = Shader.Find("Custom/WaterShader");
	}

    void Update() {
        MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
        renderer.material.SetColor("_PointLightColor", pointLight.color);
        renderer.material.SetVector("_PointLightPosition", pointLight.GetWorldPosition());
    }

    private float[][] GenerateWaterHeight(int size, float h) {
        float[][] w = new float[size][];

        for (int i = 0; i < w.Length; i++) {
            w[i] = new float[size];
            for (int j = 0; j < w[i].Length; j++) {
                w[i][j] = h;
            }
        }

        return w;
    }
}
