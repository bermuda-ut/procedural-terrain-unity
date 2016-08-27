using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class MeshGenerator : MonoBehaviour {

    public static Mesh PlaneFromArray(float[][] t, float xSize, float zSize, float minHeight, float maxHeight, bool useCustom=false, Color customColor=new Color()) {
        Mesh m = new Mesh();
        m.name = "generatedMesh";

        List<Vector3> vList = new List<Vector3>();
        for (int i = 0; i < t.Length - 1; i++) {
            for (int j = 0; j < t[i].Length - 1; j++) {
                vList.Add(new Vector3(j*xSize, t[i][j], i*zSize));
                vList.Add(new Vector3(j*xSize, t[i + 1][j], (i + 1)*zSize));
                vList.Add(new Vector3((j + 1)*xSize, t[i][j + 1], i*zSize));

                vList.Add(new Vector3(xSize*j, t[i + 1][j], zSize*(i + 1)));
                vList.Add(new Vector3(xSize*(j + 1), t[i + 1][j + 1], zSize*(i + 1)));
                vList.Add(new Vector3(xSize*(j + 1), t[i][j + 1], zSize*i));
            }
        }

        List<Color> cList = new List<Color>();
        if(useCustom)
            for(int i = 0; i < vList.Count; i++) {
                cList.Add(customColor);
            }
        else
            foreach (Vector3 v in vList) {
                cList.Add(HeightColour(v, minHeight, maxHeight));
            }

        m.vertices = vList.ToArray();
        m.colors = cList.ToArray();

        int[] triangles = new int[m.vertices.Length];

        for (int i = 0; i < m.vertices.Length; i++)
            triangles[i] = i;
        m.triangles = triangles;


        return m;
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
