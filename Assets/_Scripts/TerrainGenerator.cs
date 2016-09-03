using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine.Networking.Match;

public class TerrainGenerator : MonoBehaviour {
    [Header("Size Settings")]
    public int n;
    public float xTileSize;
    public float zTileSize;

    [Header("Generation Settings")]
    public float maxHeight;
    public float minHeight;
    [Range(0f, 200f)]
    public float noiseRange;
    [Range(0f, 2f)]
    public float roughness;
    [Range(0, 5)]
    public int naturalizer;
    [Range(0f, 0.5f)]
    public float naturalLimit;

    [Header("Water Link")]
    public float waterGurantee;

    [Header("Experimental")]
    public bool islandMode;
    public float islandTip;

    public float[][] terrainArray;

    private int trueSize;
    private System.Random rand;

    private float GR = 1.614f;

    void Start () {
        terrainArray = GenerateTerrainHeight(n, maxHeight, minHeight);

        float min = terrainArray[0][0];
        float max = terrainArray[0][0];
        for(int i = 0; i < terrainArray.Length; i++)
            for (int j = 0; j < terrainArray.Length; j++) {
                if (terrainArray[i][j] < min) {
                    min = terrainArray[i][j];
                } else if (terrainArray[i][j] > max) {
                    max = terrainArray[i][j];
                }
            }

        // gurantee water height
        Transform water = GameObject.FindObjectOfType<WaterGenerator>().gameObject.transform;
        min = min + (max - min)*waterGurantee;
        if (water.position.y < min) {
            water.position = new Vector3(water.position.x, min, water.position.z);
        }

        Mesh[] meshArr = MeshGenerator.PlaneFromArray(terrainArray, xTileSize, zTileSize, minHeight, maxHeight);
        for (int i = 0; i < meshArr.Length; i++) {
            GameObject child = new GameObject();
            child.AddComponent<TerrainHolder>();
            child.name = "Terrain" + i.ToString();
            child.transform.position = new Vector3();
            child.transform.parent = this.gameObject.transform;

            MeshFilter terrainMesh = child.AddComponent<MeshFilter>();
            terrainMesh.mesh = meshArr[i];
            MeshRenderer terrainRenderer = child.AddComponent<MeshRenderer>();
            terrainRenderer.material.shader = Shader.Find("Custom/TerrainShader");
            child.GetComponent<TerrainHolder>().pointLight = FindObjectOfType<PointLight>().GetComponent<PointLight>();
        }

    }

    private float noise(bool abs = false) {
        if(abs || rand.NextDouble() < 0.5)
            return (float) rand.NextDouble()*noiseRange*GR;
        return (float) rand.NextDouble()*noiseRange*-GR;
    }
	
    private float[][] GenerateTerrainHeight(int n, float maxHeight, float minHeight) {
        int seed = (int) System.DateTime.Now.Ticks & 0x0000FFFF; // True randomness
        int size = (int) Mathf.Pow(2f, (float)n) + 1;
        trueSize = size;

        rand = new System.Random(seed);

        float[][] t = new float[size][];
        for (int i = 0; i < size; i++)
            t[i] = new float[size];

        // initial fill

        if (islandMode) {
            t[0][0] = (float) rand.NextDouble()*minHeight;
            t[size - 1][0] = (float) rand.NextDouble()*minHeight;
            t[0][size - 1] = (float) rand.NextDouble()*minHeight;
            t[size - 1][size - 1] = (float) rand.NextDouble()*minHeight;
        } else {
            t[0][0] = (float)rand.NextDouble() * (maxHeight - minHeight) + minHeight;
            t[size-1][0] = (float)rand.NextDouble() * (maxHeight - minHeight) + minHeight;
            t[0][size-1] = (float)rand.NextDouble() * (maxHeight - minHeight) + minHeight;
            t[size-1][size-1] = (float)rand.NextDouble() * (maxHeight - minHeight) + minHeight;
        }

        FillDiamondSquare(t, t.Length);

        // Naturalize extremes
        for (int i = 0; i < naturalizer; i++) {
            for (int z = 0; z < t.Length; z++) {
                for (int x = 1; x < t.Length - 1; x++) {
                    float avg = (t[z][x - 1] + t[z][x + 1])/2;
                    if (Mathf.Abs(t[z][x] - avg) > naturalLimit)
                        t[z][x] = avg;
                }
            }
            for (int x = 0; x < t.Length; x++) {
                for (int z = 1; z < t.Length - 1; z++) {
                    float avg = (t[z - 1][x] + t[z + 1][x])/2;
                    if (Mathf.Abs(t[z][x] - avg) > naturalLimit)
                        t[z][x] = avg;
                }
            }
        }

        return t;
    }

    private void FillDiamondSquare(float[][] t, int size) {
        int mid = size/2;
        int trueSize = size; 

        while (mid >= 1) {
            for (int z = mid; z < trueSize; z += (mid*2)) {
                for (int x = mid; x < trueSize; x += (mid*2)) {
                    AddSquare(t, x, z, size);
                    if (trueSize == size && islandMode)
                        t[x][z] = islandTip;
                }
            }

            for (int z = mid; z < trueSize; z += (mid*2)) {
                for (int x = mid; x < trueSize; x += (mid*2)) {
                    AddDiamond(t, x, z-mid, size); // top
                    AddDiamond(t, x, z + mid, size); // bottom
                    AddDiamond(t, x-mid, z, size); // left
                    AddDiamond(t, x+mid, z, size); // right
                }
            }

            size = mid;
            mid /= 2;
        }
    }

    private void AddDiamond(float[][] t, int resPointx, int resPointz, int size, bool force=false) {
        // add from diamond vertices
        if (!force && t[resPointz][resPointx] != 0)
            return;

        int mid = size/2;
        int c = 0;

        if (resPointz - mid >= 0) {
            t[resPointz][resPointx] += t[resPointz - mid][resPointx];
            c++;
        }

        if (resPointz + mid < trueSize) {
            t[resPointz][resPointx] += t[resPointz + mid][resPointx];
            c++;
        }

        if (resPointx - mid >= 0) {
            t[resPointz][resPointx] += t[resPointz][resPointx - mid];
            c++;
        }

        if (resPointx + mid < trueSize) {
            t[resPointz][resPointx] += t[resPointz][resPointx + mid];
            c++;
        }

        t[resPointz][resPointx] /= c;
        t[resPointz][resPointx] += noise()/Mathf.Pow(GR, trueSize - size - 1)*Mathf.PerlinNoise(resPointx, resPointz);
        t[resPointz][resPointx] += noise()/trueSize*size*roughness;

        if (t[resPointz][resPointx] > maxHeight)
            t[resPointz][resPointx] = maxHeight;
        else if (t[resPointz][resPointx] < minHeight)
            t[resPointz][resPointx] = minHeight;
    }

    private void AddSquare(float[][] t, int resPointx, int resPointz, int size, bool force=false) {
        // add from square vertices
        if (!force && t[resPointz][resPointx] != 0)
            return;

        int mid = size / 2;
        int c = 0;

        if (resPointz - mid >= 0 && resPointx - mid >= 0) {
            t[resPointz][resPointx] += t[resPointz - mid][resPointx - mid];
            c++;
        }

        if (resPointz + mid < trueSize && resPointx - mid >= 0) {
            t[resPointz][resPointx] += t[resPointz + mid][resPointx - mid];
            c++;
        }

        if (resPointz - mid >= 0 && resPointx + mid < trueSize) {
            t[resPointz][resPointx] += t[resPointz - mid][resPointx + mid];
            c++;
        }

        if (resPointz + mid < trueSize && resPointx + mid < trueSize) {
            t[resPointz][resPointx] += t[resPointz + mid][resPointx + mid];
            c++;
        }

        t[resPointz][resPointx] /= c;
        t[resPointz][resPointx] += noise()/Mathf.Pow(GR, trueSize - size - 1)*Mathf.PerlinNoise(resPointx, resPointz);
        t[resPointz][resPointx] += noise()/trueSize*size*roughness;

        if (t[resPointz][resPointx] > maxHeight)
            t[resPointz][resPointx] = maxHeight;
        else if (t[resPointz][resPointx] < minHeight)
            t[resPointz][resPointx] = minHeight;
    }
}
