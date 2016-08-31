Shader "Custom/WaterShader" {
	Properties{
		_PointLightColor("Point Light Color", Color) = (0, 0, 0)
		_PointLightPosition("Point Light Position", Vector) = (0.0, 0.0, 0.0)
	}

	SubShader{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag alpha
			#include "UnityCG.cginc"

			uniform float3 _PointLightColor;
			uniform float3 _PointLightPosition;

			struct vertIn {
				float4 vertex : POSITION;
				float4 normal : NORMAL; // normal to calculate diffuse and specular lighting
				float4 color : COLOR;
			};

			struct vertOut {
				float4 vertex : SV_POSITION;
				float4 worldVertex : POSITIONT;
				float3 worldNormal : NORMAL; // world normal
				float4 color : COLOR;
			};

			// Implementation of the vertex shader
			vertOut vert(vertIn v) {
				vertOut o;

				o.worldNormal = normalize(mul(transpose((float3x3)_World2Object), v.normal.xyz));
				o.worldVertex = mul(_Object2World, v.vertex);
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.color = v.color;

				return o;
			};

			// Implementation of the fragment shader
			// pixel-shader
			fixed4 frag(vertOut v) : SV_Target {
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
				float Ks = 1;
				float specN = 5; // Values>>1 give tighter highlights
				float3 V = normalize(_WorldSpaceCameraPos - v.worldVertex.xyz);
				float3 R = 2 * dot(L, v.worldNormal) * v.worldNormal - L;
				float3 spe = fAtt * _PointLightColor.rgb * Ks * pow(saturate(dot(V, R)), specN);

				// Combine Phong illumination model components
				o.color.rgb = amb.rgb + dif.rgb + spe.rgb;
				o.color.a = v.color.a;

				return o.color;
			};

			ENDCG
		}
	}
}
/*
Shader "Custom/WaterShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,0.25)
		_Glossiness ("Glossiness", Range(0,1)) = 0.15
	}
	SubShader {
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		Cull Off

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows alpha
		#pragma target 3.0

		half _Glossiness;
		fixed4 _Color;

		struct Input {
			float2 uv_MainTex;
		};

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
		fixed4 frag(vertOut v) : SV_Target{
			return v.color;
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = _Color;
			o.Albedo = c.rgb;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
*/
