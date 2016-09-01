Shader "Custom/TerrainShader"
{
	Properties{
		_PointLightColor("Point Light Color", Color) = (0, 0, 0)
		_PointLightPosition("Point Light Position", Vector) = (0.0, 0.0, 0.0)
	}

	SubShader{
		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			uniform float3 _PointLightColor;
			uniform float3 _PointLightPosition;

			struct vertIn
			{
				float4 vertex : POSITION;
				float4 normal : NORMAL; // normal to calculate diffuse and specular lighting
				float4 color : COLOR;
			};

			struct vertOut
			{
				float4 vertex : SV_POSITION;
				float4 worldVertex : POSITIONT;
				float3 worldNormal : NORMAL; // world normal
				float4 color : COLOR;
			};

			// Implementation of the vertex shader
			vertOut vert(vertIn v)
			{
				vertOut o;

				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.worldNormal = normalize(mul(transpose((float3x3)_World2Object), v.normal.xyz));
				o.worldVertex = mul(_Object2World, v.vertex);
				o.color = v.color;

				return o;
			}

			// Implementation of the fragment shader
			// pixel-shader
			fixed4 frag(vertOut v) : SV_Target
			{
				vertOut o;

				// Calculate ambient RGB intensities
				float Ka = 1;
				float3 amb = v.color.rgb * UNITY_LIGHTMODEL_AMBIENT.rgb * Ka;

				float fAtt = 1;
				float Kd = 1;
				float3 L = normalize(_PointLightPosition - v.worldVertex.xyz);
				float LdotN = dot(L, v.worldNormal.xyz);
				float3 dif = fAtt * _PointLightColor.rgb * Kd * v.color.rgb * saturate(LdotN);

				// Calculate specular reflections
				float Ks = 0.9;
				float specN = 1; // Values>>1 give tighter highlights
				float3 V = normalize(_WorldSpaceCameraPos - v.worldVertex.xyz);
				float3 R = 2 * dot(L, v.worldNormal) * v.worldNormal - L;
				float3 spe = fAtt * _PointLightColor.rgb * Ks * pow(saturate(dot(V, R)), specN);

				// Combine Phong illumination model components
				o.color.rgb = amb.rgb + dif.rgb + spe.rgb;
				o.color.a = v.color.a;

				return o.color;
			}
		ENDCG
		}
	}
}

/*
Shader "Custom/TerrainShader" {
Properties{
//_Color ("Color", Color) = (1,1,1,1)
//_MainTex ("Albedo (RGB)", 2D) = "white" {}
//_Glossiness ("Smoothness", Range(0,1)) = 0.5
//_Metallic ("Metallic", Range(0,1)) = 0.0
}

SubShader {
Pass {
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"

struct vertIn {
float4 vertex : POSITION;
float4 color : COLOR;
};

struct vertOut {
float4 vertex : SV_POSITION;
float4 color : COLOR;
};


// Implementation of the vertex shader
vertOut vert(vertIn v) {
vertOut o;
o.vertex = mul(UNITY_MATRIX_MVP, v.vertex); // model > world space > camera space
o.color = v.color;
return o;
}

// Implementation of the fragment shader
fixed4 frag(vertOut v) : SV_Target {
return v.color;
}

ENDCG
}
}
*/
// Original Cg/HLSL code stub copyright (c) 2010-2012 SharpDX - Alexandre Mutel
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documetion files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
// Adapted for COMP30019 by Jeremy Nicholson, 10 Sep 2012
// Adapted further by Chris Ewin, 23 Sep 2013
// Adapted further (again) by Alex Zable (port to Unity), 19 Aug 201nta6
