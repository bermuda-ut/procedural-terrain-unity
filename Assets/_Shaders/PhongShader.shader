// Original Cg/HLSL code stub copyright (c) 2010-2012 SharpDX - Alexandre Mutel
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
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
// Adapted further (again) by Alex Zable (port to Unity), 19 Aug 2016
// Adapted further (again and again) by Max Lee (for assignments), Sep 2016

// variable defining array length > SLOW!
Shader "Custom/PhongShader" {
	Properties {
	}

	SubShader {
		Pass {
			// Ambient and custom light
			Tags{ "LightMode" = "ForwardBase" } // for additional light sources

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0

			#include "UnityCG.cginc"

			uniform float3 _PointLightColor[10];
			uniform float3 _PointLightPosition[10];

			struct vertIn {
				float4 vertex : POSITION;
				float4 normal : NORMAL;
				float4 color : COLOR;
			};

			struct vertOut {
				float4 vertex : SV_POSITION;
				float4 color : COLOR;
				float4 worldVertex : TEXCOORD0;
				float3 worldNormal : TEXCOORD1;
			};

			// Implementation of the vertex shader
			vertOut vert(vertIn v) {
				vertOut o;

				o.worldVertex = mul(_Object2World, v.vertex);
				o.worldNormal = normalize(mul(transpose((float3x3)_World2Object), v.normal.xyz));
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.color = v.color;

				return o;
			}

			// Implementation of the fragment shader
			fixed4 frag(vertOut v) : SV_Target {
				// Our interpolated normal might not be of length 1
				float3 interpNormal = normalize(v.worldNormal);

				// Calculate ambient RGB intensities
				float Ka = 1;
				float3 amb = v.color.rgb * UNITY_LIGHTMODEL_AMBIENT.rgb * Ka;

				float3 dif = float3(0,0,0);
				float3 spe = float3(0,0,0);

				float Kd = 1;
				float Ks = 1;
				float specN = 5; // Values>>1 give tighter highlights

				// custom light
				// ignore distance from the light source!
				for (int i = 0; i < 10; i++) {
					// Calculate diffuse RBG reflections, we save the results of L.N because we will use it again
					// (when calculating the reflected ray in our specular component)
					float3 rawL = _PointLightPosition[i] - v.worldVertex.xyz;
					float3 L = normalize(rawL);
					float LdotN = dot(L, interpNormal);
					float fAtt = 1.0;// / length(rawL);

					dif += fAtt * _PointLightColor[i].rgb * Kd * v.color.rgb * saturate(LdotN);

					float3 V = normalize(_WorldSpaceCameraPos - v.worldVertex.xyz);
					float3 H = normalize(V + L);


					spe += fAtt * _PointLightColor[i].rgb * Ks * pow(saturate(dot(interpNormal, H)), specN);
					// light source on the correct side
					//if (dot(interpNormal, L) >= 0.0) {
					//}
				}

				// Combine Phong illumination model components
				float4 returnColor = float4(0.0f, 0.0f, 0.0f, 0.0f);
				returnColor.rgb = amb.rgb + dif.rgb + spe.rgb;
				returnColor.a = v.color.a;

				return returnColor;
			}
			ENDCG
		}

		Pass {
			Tags{ "LightMode" = "ForwardAdd" } // for additional light sources
			Blend One One

			// unity light handler
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0

			#include "UnityCG.cginc"
			#include "Lighting.cginc"

			uniform float3 _PointLightColor[10];
			uniform float3 _PointLightPosition[10];

			struct vertIn {
				float4 vertex : POSITION;
				float4 normal : NORMAL;
				float4 color : COLOR;
			};

			struct vertOut {
				float4 vertex : SV_POSITION;
				float4 color : COLOR;
				float4 worldVertex : TEXCOORD0;
				float3 worldNormal : TEXCOORD1;
			};

			// Implementation of the vertex shader
			vertOut vert(vertIn v) {
				vertOut o;

				o.worldVertex = mul(_Object2World, v.vertex);
				o.worldNormal = normalize(mul(transpose((float3x3)_World2Object), v.normal.xyz));
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.color = v.color;

				return o;
			}

			// Implementation of the fragment shader
			fixed4 frag(vertOut v) : SV_Target {
				// Our interpolated normal might not be of length 1
				float3 interpNormal = normalize(v.worldNormal);

				float3 dif = float3(0,0,0);
				float3 spe = float3(0,0,0);

				float fAtt = 1;
				float3 lightDirection;

				// calculate fAtt
				if (_WorldSpaceLightPos0.w == 0.0) {
					// directional light
					lightDirection = normalize(_WorldSpaceLightPos0.xyz);
				} else {
					// point or spot light
					float3 vectorToLight = _WorldSpaceLightPos0.xyz - v.worldVertex.xyz;
					fAtt = 1.0 / length(vectorToLight); // linear since quadratic gives unrealistic results
					lightDirection = normalize(vectorToLight);
				}

				// Calculate diffuse
				float Kd = 1;
				float3 L = normalize(_WorldSpaceLightPos0 - v.worldVertex.xyz);
				float LdotN = dot(L, interpNormal);
				dif += fAtt * _LightColor0.xyz.rgb * Kd * v.color.rgb * saturate(LdotN);

				float Ks = 1;
				float specN = 25; // Values>>1 give tighter highlights
				// light source on the correct side
				if (dot(interpNormal, lightDirection) >= 0.0) {
					// Calculate specular
					float3 V = normalize(_WorldSpaceLightPos0 -v.worldVertex.xyz);
					float3 H = normalize(V + L);
					spe += fAtt * Ks * _LightColor0.xyz.rgb * pow(saturate(dot(interpNormal, H)), specN);
				}

				// Combine Phong illumination model components
				float4 returnColor = float4(0.0f, 0.0f, 0.0f, 0.0f);
				returnColor.rgb = dif.rgb + spe.rgb; // no ambient since added in ForwardBase
				returnColor.a = v.color.a;

				return returnColor;
			}

			ENDCG
		}
	}
}
