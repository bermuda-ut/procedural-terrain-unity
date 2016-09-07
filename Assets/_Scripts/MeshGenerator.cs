using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Xml;

public class MeshGenerator : MonoBehaviour {
    private static int vertexLimit = 60000;

    public static Mesh[] PlaneFromArray(float[][] t, float xSize, float zSize, float minHeight, float maxHeight, bool useCustom=false, Vector3 normal = new Vector3(), Color c = new Color()) {
        int total = t.Length * t[0].Length;
        int meshCount = (int) (total/vertexLimit) + 1;
        Mesh[] mArr = new Mesh[meshCount + 1];

        int breakLeap = (int) (t.Length/meshCount);
        int breakIndex = 0;

        int i = 0;
        for (int z = 0; z < meshCount+1; z++) {
            breakIndex = (breakLeap + breakIndex > t.Length) ? t.Length : breakLeap + breakIndex;
            mArr[z] = new Mesh();

            Mesh m = mArr[z];
            m.name = "GeneratedMesh_" + z;
            Vector3[][] n = calcNormals(t, xSize, zSize);

            List<int> tList = new List<int>();
            List<Color> cList = new List<Color>();
            List<Vector3> vList = new List<Vector3>();
            List<Vector3> nList = new List<Vector3>();

            // add verticies and triangles
            for (int k = 0; i < breakIndex; i++, k++) {
                for (int j = 0; j < t[i].Length; j++) {
                    // verticies
                    vList.Add(new Vector3(j*xSize, t[i][j], i*zSize));

                    // normals
                    nList.Add(n[i][j]);

                    // triangles
                    if (j < t[i].Length - 1 && i < breakIndex - 1) {
                        tList.Add(k*t[i].Length + j);
                        tList.Add((k + 1)*t[i].Length + j);
                        tList.Add(k*t[i].Length + j + 1);

                        tList.Add((k + 1)*t[i].Length + j);
                        tList.Add((k + 1)*t[i].Length + j + 1);
                        tList.Add(k*t[i].Length + j + 1);
                    }
                }
            }
            i--;

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
            m.normals = nList.ToArray();
            m.RecalculateBounds();
        }

        return mArr;
    }

    public static Vector3[][] calcNormals(float[][] t, float xSize, float zSize) {
        Vector3[][] n = new Vector3[t.Length][];

        for(int i = 0; i < t.Length; i++) {
            n[i] = new Vector3[t[i].Length];
            for (int j = 0; j < t[i].Length; j++) {
                Vector3 a = new Vector3();
                Vector3 curr = new Vector3(j*xSize, t[i][j], i*zSize);
                int c = 0;

                if (i > 0) {
                    if (j > 0) {
                        a += Vector3.Cross(
                            new Vector3(j*xSize, t[i-1][j], (i-1)*zSize) - curr,
                            new Vector3((j-1)*xSize, t[i][j-1], i*zSize) - curr
                        );
                        c++;
                    } 
                    if(j < t[i].Length - 1) {
                        a += Vector3.Cross(
                            new Vector3((j+1)*xSize, t[i-1][j+1], (i-1)*zSize) - curr,
                            new Vector3(j*xSize, t[i-1][j], (i-1)*zSize) - curr
                        );

                        a += Vector3.Cross(
                            new Vector3((j+1)*xSize, t[i][j+1], i*zSize) - curr,
                            new Vector3((j+1)*xSize, t[i-1][j+1], (i-1)*zSize) - curr
                        );

                        c+=2;
                    }
                }

                if (i < t.Length - 1) {
                    if (j > 0) {
                        a += Vector3.Cross(
                            new Vector3((j-1)*xSize, t[i+1][j-1], (i+1)*zSize) - curr,
                            new Vector3(j*xSize, t[i+1][j], (i+1)*zSize) - curr
                        );

                        a += Vector3.Cross(
                            new Vector3((j-1)*xSize, t[i][j-1], i*zSize) - curr,
                            new Vector3((j-1)*xSize, t[i+1][j-1], (i+1)*zSize) - curr
                        );

                        c+=2;
                    } 
                    if(j < t[i].Length - 1) {
                        a += Vector3.Cross(
                            new Vector3(j*xSize, t[i+1][j], (i+1)*zSize) - curr,
                            new Vector3((j+1)*xSize, t[i][j+1], i*zSize) - curr
                        );
                        c++;
                    }
                    
                }

                n[i][j] = (a / c).normalized;
            }
        }

        return n;
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
        else if (v.y <= 2*minHeight/3)
            c = MeshGenerator.HexToColor("9e9e9e", 255);
        else if(v.y <= 0f)
            c = MeshGenerator.HexToColor("6d4c41", 255);
        else if(v.y <= maxHeight/25)
            c = MeshGenerator.HexToColor("ffe57f", 255);
        else if(v.y <= maxHeight/4)
            c = MeshGenerator.HexToColor("689f38", 255);
        else if(v.y <= maxHeight/2)
            c = MeshGenerator.HexToColor("8bc34a", 255);
        else if(v.y <= 5*maxHeight/8)
            c = MeshGenerator.HexToColor("616161", 255);
        else if(v.y <= 3*maxHeight/4)
            c = MeshGenerator.HexToColor("e1f5fe", 255);
        else
            c = MeshGenerator.HexToColor("eeeeee", 255);

        return c;
    }
	
}
