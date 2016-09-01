using UnityEngine;
using System.Collections;

public class PointLight : MonoBehaviour {

    public float rotationSpeed;
    public float colorShift;
    public Color color;

    private TerrainGenerator gen;
    private Vector3 rotPoint;

    void Start() {
        GameObject terrain = GameObject.Find("GroundTerrain");
        gen = terrain.GetComponent<TerrainGenerator>();

        float midX = gen.xTileSize*(Mathf.Pow(2, gen.n) + 1)/2;
        float midZ = gen.zTileSize*(Mathf.Pow(2, gen.n) + 1)/2;
        gameObject.transform.position = new Vector3(midX, gen.maxHeight + midX*2, midZ);
        rotPoint = new Vector3(midX, 0, midZ);
    }

    void Update() {
        this.transform.RotateAround(rotPoint, Vector3.left, -rotationSpeed*Time.deltaTime);
        float curr = this.transform.localEulerAngles.x;

        if (curr > 180) {
            color -= Color.red*colorShift*Time.deltaTime*1.114f;
            color -= Color.yellow*colorShift*Time.deltaTime;
        } else {
            color += Color.red*colorShift*Time.deltaTime*1.114f;
            color += Color.yellow*colorShift*Time.deltaTime;
        }
        color = Vector4.Normalize(color);
    }

    public Vector3 GetWorldPosition() {
        return this.transform.position;
    }
}
