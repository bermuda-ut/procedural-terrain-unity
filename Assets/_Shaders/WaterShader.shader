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
