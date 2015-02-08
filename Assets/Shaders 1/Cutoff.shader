Shader "Custom/Cutoff" {
	Properties {
		_MainTex ("Background (RGB)", 2D) = "white" {}
		_BarTex ("Bar (RGB)", 2D) = "white" {}
		_CutoffTex ("Cutoff Texture (RGB)", 2D) = "white" {}
		_Cutoff ("Cutoff", Range(0, 1)) = 0.5
	}
	SubShader {
		Tags { "RenderType"="Transparent" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert alpha

		sampler2D _MainTex;
		sampler2D _BarTex;
		sampler2D _CutoffTex;
		float _Cutoff;

		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half cutoff = tex2D (_CutoffTex, IN.uv_MainTex).r;
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			
			
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
