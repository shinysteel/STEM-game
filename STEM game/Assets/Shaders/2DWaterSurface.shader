Shader "Custom/2DWaterSurface" 
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_MainColor("Color", Color) = (1, 1, 1, 1)
		_WaveFrequency("Wave Frequency", Range(0, 25)) = 5
	}

	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct vertexOutput
			{
				float4 pos : SV_POSITION;
				half2 uv : TEXCOORD0;
			};

			half _WaveFrequency;

			vertexOutput vert(appdata_base v)
			{
				vertexOutput o;
				v.vertex.z += sin(v.vertex.x * _WaveFrequency + _Time[1]);
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;
				return o;
			}

			sampler2D _MainTex;
			fixed4 _MainColor;

			fixed4 frag(vertexOutput i) : COLOR
			{
				half4 c = tex2D(_MainTex, i.uv);
				c *= _MainColor;
				return c;
			}
			ENDCG
		}
	}
}