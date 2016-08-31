using System;
using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    public float maxSpeed;
    public float acceleration;
    public float decceleration;

    public float horzTurnSpeed;
    public float vertTurnSpeed;
    public float rollSpeed;
    public float cameraYOffset;
    public float cameraZOffset;
    public float cameraXOffset;

    public GameObject terrain;

    private Vector3 move;
    private float yaw;
    private float pitch;
    private float roll;

    private float zMin;
    private float zMax;
    private float xMin;
    private float xMax;

    private TerrainGenerator gen;

    // Use this for initialization
    void Start () {
	    move = new Vector3();
        yaw = 0f;
        pitch = 0f;
        roll = 0f;

        // Get terrain
        terrain = GameObject.Find("GroundTerrain");
        gen = terrain.GetComponent<TerrainGenerator>();

        // relocate
        float midX = gen.xTileSize*(Mathf.Pow(2, gen.n) + 1)/2;
        float midZ = gen.zTileSize*(Mathf.Pow(2, gen.n) + 1)/2;

        // set limit
        zMin = 0;
        xMin = 0;
        xMax = gen.xTileSize*(Mathf.Pow(2, gen.n));
        zMax = gen.zTileSize*(Mathf.Pow(2, gen.n));

        gameObject.transform.position = new Vector3(midX, getTerrainY(new Vector3(midX, 0, midZ), Vector3.zero), midZ);
    }

    float getTerrainY(Vector3 pos, Vector3 move) {
        pos += move.normalized;
        float y = 0f;
        float zRaw = (pos.z + cameraZOffset) / gen.zTileSize;
        float xRaw = (pos.x + cameraXOffset) / gen.xTileSize;
        int zSlot = Mathf.FloorToInt(zRaw);
        int xSlot = Mathf.FloorToInt(xRaw);
        int size = gen.terrainArray.Length;
        xSlot = (xSlot < 0) ? 0 : xSlot;
        zSlot = (zSlot < 0) ? 0 : zSlot;
        xSlot = (xSlot >= size) ? size-1 : xSlot;
        zSlot = (zSlot >= size) ? size-1 : zSlot;

        float zFactor = zRaw - zSlot;
        float xFactor = xRaw - xSlot;

        if (xSlot + 1 < size && zSlot + 1 < size) {
            float baseX = (gen.terrainArray[zSlot][xSlot + 1] - gen.terrainArray[zSlot][xSlot])*xFactor +
                          gen.terrainArray[zSlot][xSlot];
            float zX = (gen.terrainArray[zSlot + 1][xSlot + 1] - gen.terrainArray[zSlot + 1][xSlot])*xFactor +
                       gen.terrainArray[zSlot][xSlot];
            y = (zX - baseX)*zFactor + baseX;
        } else {
            y = gen.terrainArray[zSlot][xSlot];
        }

        return y + cameraYOffset;
    }

    bool MoveCheck() {
        if (transform.position.x + move.x > xMax ||
            transform.position.x + move.x < xMin ||
            transform.position.z + move.z > zMax ||
            transform.position.z + move.z < zMin)
            return false;
        return true;
    }

    void PitchYawRoll() {
        yaw += horzTurnSpeed*Input.GetAxis("Mouse X");
        pitch -= vertTurnSpeed*Input.GetAxis("Mouse Y");

        if (Input.GetKey(KeyCode.E))
            roll += rollSpeed;
        if (Input.GetKey(KeyCode.Q))
            roll -= rollSpeed;
        if (Input.GetKey(KeyCode.Space))
            roll = 0;

        transform.rotation = Quaternion.Euler(pitch, yaw, roll);
    }

    void KeyboardMovement() {
        if (Mathf.Abs(Vector3.Distance(move, Vector3.zero)) < maxSpeed) {
            if (Input.GetKey(KeyCode.W)) {
                move += gameObject.transform.forward * acceleration * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.S)) {
                move -= gameObject.transform.forward * acceleration * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.A)) {
                move -= gameObject.transform.right * acceleration * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.D)) {
                move += gameObject.transform.right * acceleration * Time.deltaTime;
            }
        }

        if (MoveCheck()) {
            float tY = getTerrainY(transform.position, move);
            transform.position += move;
            if(transform.position.y < tY)
                transform.position = new Vector3(transform.position.x, tY, transform.position.z);
        }
        move *= decceleration*Time.deltaTime;
    }
	
	// Update is called once per frame
	void Update () {
        KeyboardMovement();
	    PitchYawRoll();
	}
}
