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
	/*
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
	*/
}
