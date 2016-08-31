using UnityEngine;
using System.Collections;

public class PointLight : MonoBehaviour {

    public float rotationSpeed;
    public Color color;

    private TerrainGenerator gen;
    private Vector3 rotPoint;

    public float curr;

    void Start() {
        GameObject terrain = GameObject.Find("GroundTerrain");
        gen = terrain.GetComponent<TerrainGenerator>();
        curr = 0;

        float midX = gen.xTileSize*(Mathf.Pow(2, gen.n) + 1)/2;
        float midZ = gen.zTileSize*(Mathf.Pow(2, gen.n) + 1)/2;
        gameObject.transform.position = new Vector3(midX, gen.maxHeight + 1000, midZ);
        rotPoint = new Vector3(midX, 0, midZ);
    }

    void Update() {
        this.transform.RotateAround(rotPoint, Vector3.left, -rotationSpeed*Time.deltaTime);
        curr = this.transform.localEulerAngles.x;

        if (curr > 180) {
            color -= Color.red*0.001f;
            color -= Color.yellow*0.001f;
        } else {
            color += Color.red*0.001f;
            color += Color.yellow*0.001f;
        }
    }

    public Vector3 GetWorldPosition() {
        return this.transform.position;
    }
}
