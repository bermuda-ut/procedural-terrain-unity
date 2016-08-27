using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class TerrainGenerator : MonoBehaviour {
    [Header("Size Settings")]
    [Range(1, 6)]
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

    [Header("Extra")]
    [Range(0, 5)]
    public int naturalizer;
    [Range(0f, 40f)]
    public float naturalLimit;

    [Header("Internal Usage")]
    public float[][] terrainArray;

    private int trueSize;
    private System.Random rand;

    void Start () {
        terrainArray = GenerateTerrainHeight(n, maxHeight, minHeight);

        MeshFilter terrainMesh = gameObject.AddComponent<MeshFilter>();
        terrainMesh.mesh = MeshGenerator.PlaneFromArray(terrainArray, xTileSize, zTileSize, minHeight, maxHeight);
        MeshRenderer terrainRenderer = gameObject.AddComponent<MeshRenderer>();

        terrainRenderer.material.shader = Shader.Find("Custom/TerrainShader");
    }

    private float noise() {
        if(rand.NextDouble() < 0.5)
            return (float) rand.NextDouble()*noiseRange;
        return (float) rand.NextDouble()*noiseRange*-1.0f;
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
        t[0][0] = (float)rand.NextDouble() * (maxHeight - minHeight) + minHeight;
        t[size-1][0] = (float)rand.NextDouble() * (maxHeight - minHeight) + minHeight;
        t[0][size-1] = (float)rand.NextDouble() * (maxHeight - minHeight) + minHeight;
        t[size-1][size-1] = (float)rand.NextDouble() * (maxHeight - minHeight) + minHeight;

        FillDiamondSquare(t, t.Length);

        // Naturalize Tip
        //AddSquare(t, size/2, size/2, 1, true);

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

        // x . . . 2 . . . x  
        // . . . . . . . . .  
        // . . 1 . . . 1 . .  
        // . . . . . . . . .  
        // 2 . . . x . . . 2  
        // . . . . . . . . .  
        // . . . . . . . . .  
        // . . . . . . . . .  
        // x . . . 2 . . . x  

        while (mid >= 1) {
            for (int z = mid; z < trueSize; z += (mid*2)) {
                for (int x = mid; x < trueSize; x += (mid*2)) {
                    AddSquare(t, x, z, size);
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
        t[resPointz][resPointx] += noise()/Mathf.Pow(2, trueSize-size-1);
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
        t[resPointz][resPointx] += noise()/Mathf.Pow(2, trueSize-size-1);
        t[resPointz][resPointx] += noise()/trueSize*size*roughness;

        if (t[resPointz][resPointx] > maxHeight)
            t[resPointz][resPointx] = maxHeight;
        else if (t[resPointz][resPointx] < minHeight)
            t[resPointz][resPointx] = minHeight;
    }
}

//AddSquare(t, startx + mid/2, startz + mid/2, mid + 1, trueSize);
//AddSquare(t, startx + 3*mid/2, startz + mid/2, mid + 1, trueSize);
//AddSquare(t, startx + mid/2, startz + 3*mid/2, mid + 1, trueSize);
//AddSquare(t, startx + 3*mid/2, startz + 3*mid/2, mid + 1, trueSize);

// Stage 2
//AddDiamond(t, startx + mid, startz, size, trueSize); // top
//AddDiamond(t, startx + mid, startz + size - 1, size, trueSize); // bottom
//AddDiamond(t, startx, startz + mid, size, trueSize); // left
//AddDiamond(t, startx + size - 1, startz + mid, size, trueSize); // right

// slice into 4 smaller squares and fill
//FillDiamondSquare(t, mid + 1, startx, startz, trueSize);
//FillDiamondSquare(t, mid + 1, startx, startz + mid, trueSize);
//FillDiamondSquare(t, mid + 1, startx + mid, startz, trueSize);
//FillDiamondSquare(t, mid + 1, startx + mid, startz + mid, trueSize);