using UnityEngine;
using System.Collections;

public class WaterGenerator : MonoBehaviour {

    public float waterHeight;

    private int size;
    private float xTileSize;
    private float zTileSize;
	// Use this for initialization
	void Start () {
	    GameObject p = gameObject.transform.parent.gameObject;
	    int n = p.GetComponent<TerrainGenerator>().n;
	    xTileSize = p.GetComponent<TerrainGenerator>().xTileSize;
	    zTileSize = p.GetComponent<TerrainGenerator>().zTileSize;
	    size = (int) Mathf.Pow(2, n) + 1;

	    float[][] a = GenerateWaterHeight(size, waterHeight);

        MeshFilter waterMesh = gameObject.AddComponent<MeshFilter>();
        waterMesh.mesh = MeshGenerator.PlaneFromArray(a, xTileSize, zTileSize, 0, 0, true, MeshGenerator.HexToColor("039be5", 120));
        MeshRenderer waterRenderer = gameObject.AddComponent<MeshRenderer>();

        waterRenderer.material.shader = Shader.Find("Custom/WaterShader");
	    waterRenderer.material.color = MeshGenerator.HexToColor("039be5", 120);
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
