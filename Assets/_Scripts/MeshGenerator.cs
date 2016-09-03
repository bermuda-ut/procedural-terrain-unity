using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Policy;

public class MeshGenerator : MonoBehaviour {
    private static int vertexLimit = 64000;

    public static Mesh[] PlaneFromArray(float[][] t, float xSize, float zSize, float minHeight, float maxHeight, bool useCustom=false, Vector3 normal = new Vector3(), Color c = new Color()) {
        int total = t.Length * t[0].Length * 6;
        int meshCount = (int) (total/vertexLimit) + 1;
        Mesh[] mArr= new Mesh[meshCount+1];

        int breakLeap = (int) t.Length / meshCount;
        int breakIndex = 0;

        int i = 0;
        for (int z = 0; z < meshCount+1; z++) {
            breakIndex = (breakLeap + breakIndex > t.Length - 1) ? t.Length - 1 : breakLeap + breakIndex;
            mArr[z] = new Mesh();

            Mesh m = mArr[z];
            m.name = "GeneratedMesh_" + z;

            List<int> tList = new List<int>();
            List<Color> cList = new List<Color>();
            List<Vector3> vList = new List<Vector3>();

            // add verticies and triangles
            int k = 0;
            for (; i < breakIndex; i++) {
                for (int j = 0; j < t[i].Length - 1; j++) {
                    // verticies
                    Vector3 v1 = new Vector3(j * xSize, t[i][j], i * zSize);
                    Vector3 v2 = new Vector3(j * xSize, t[i + 1][j], (i + 1) * zSize);
                    Vector3 v3 = new Vector3((j + 1) * xSize, t[i][j + 1], i * zSize);
                    Vector3 v4 = new Vector3(xSize * j, t[i + 1][j], zSize * (i + 1));
                    Vector3 v5 = new Vector3(xSize * (j + 1), t[i + 1][j + 1], zSize * (i + 1));
                    Vector3 v6 = new Vector3(xSize * (j + 1), t[i][j + 1], zSize * i);

                    vList.Add(v1);
                    vList.Add(v2);
                    vList.Add(v3);
                    vList.Add(v4);
                    vList.Add(v5);
                    vList.Add(v6);

                    // triangles
                    int a = k + 6;
                    for (; k < a; k++)
                        tList.Add(k);
                }
            }

            // add colours
            if (useCustom)
                foreach (Vector3 v in vList)
                    cList.Add(c);
            else
                foreach (Vector3 v in vList)
                    cList.Add(HeightColour(v, minHeight, maxHeight));

            m.vertices = vList.ToArray();
            m.colors = cList.ToArray();
            m.triangles = tList.ToArray();

            // add normals (surface normal)
            m.RecalculateNormals();
        }

        return mArr;
    }

    public static Color HexToColor(string hex, byte alpha) {
        byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        return new Color32(r, g, b, alpha);
    }

    private static Color HeightColour(Vector3 v, float minHeight, float maxHeight) {
        Color c;

        if (v.y <= minHeight/2)
            c = MeshGenerator.HexToColor("424242", 255);
        if (v.y <= 2*minHeight/3)
            c = MeshGenerator.HexToColor("9e9e9e", 255);
        else if(v.y <= 0f)
            c = MeshGenerator.HexToColor("6d4c41", 255);
        else if(v.y <= maxHeight/4)
            c = MeshGenerator.HexToColor("689f38", 255);
        else if(v.y <= maxHeight/2)
            c = MeshGenerator.HexToColor("8bc34a", 255);
        else if(v.y <= 3*maxHeight/4)
            c = MeshGenerator.HexToColor("eeeeee", 255);
        else
            c = MeshGenerator.HexToColor("fafafa", 255);

        return c;
    }
	
}
