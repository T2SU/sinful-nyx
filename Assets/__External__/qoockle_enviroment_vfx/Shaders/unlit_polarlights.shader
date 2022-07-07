Shader "qoockle/enviroment/unlit_polarlights"
{
	Properties
	{
		_MainTex("MainTex", 2D) = "white" {}
		[NoScaleOffset]_VertexAnimationMask("VertexAnimationMask", 2D) = "white" {}
		_MainColorR("MainColor[R]", Color) = (1,1,1,1)
		_SecondColorG("SecondColor[G]", Color) = (1,1,1,1)
		_SpeedMain("Speed Main", Float) = 0
		_SpeedSecondary("Speed Secondary", Float) = 0
		_OffsetPowerSecondary("OffsetPowerSecondary", Float) = 0.5
		_OffsetPowerMain("OffsetPowerMain", Float) = 0.5
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}
	
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		LOD 100

		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend One One
		Cull Off
		ColorMask RGBA
		ZWrite Off
		ZTest LEqual

		
		Pass
		{
			Name "Unlit"
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"
			#include "UnityShaderVariables.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				float4 ase_texcoord : TEXCOORD0;
				float3 ase_normal : NORMAL;
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
			};

			uniform half _OffsetPowerMain;
			uniform sampler2D _VertexAnimationMask;
			uniform half _SpeedMain;
			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform half _OffsetPowerSecondary;
			uniform half _SpeedSecondary;
			uniform half4 _SecondColorG;
			uniform half4 _MainColorR;
			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				float2 appendResult8 = (float2(_SpeedMain , 0.0));
				float2 uv0_MainTex = v.ase_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
				float2 panner4 = ( 1.0 * _Time.y * appendResult8 + uv0_MainTex);
				float temp_output_13_0 = ( _OffsetPowerMain * tex2Dlod( _VertexAnimationMask, float4( panner4, 0, 0.0) ).b );
				float2 appendResult17 = (float2(_SpeedSecondary , 0.0));
				float2 panner19 = ( 1.0 * _Time.y * appendResult17 + uv0_MainTex);
				
				o.ase_color = v.color;
				o.ase_texcoord.xy = v.ase_texcoord.xy;

				o.ase_texcoord.zw = 0;
				float3 vertexValue = ( (-1.0 + (( temp_output_13_0 * ( temp_output_13_0 + ( _OffsetPowerSecondary * tex2Dlod( _VertexAnimationMask, float4( panner19, 0, 0.0) ).b ) ) ) - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) * v.ase_normal );
				#if ASE_ABSOLUTE_VERTEX_POS
				v.vertex.xyz = vertexValue;
				#else
				v.vertex.xyz += vertexValue;
				#endif
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				fixed4 finalColor;
				float2 appendResult17 = (float2(_SpeedSecondary , 0.0));
				float2 uv0_MainTex = i.ase_texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				float2 panner19 = ( 1.0 * _Time.y * appendResult17 + uv0_MainTex);
				float2 uv_MainTex = i.ase_texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				float4 tex2DNode33 = tex2D( _MainTex, uv_MainTex );
				float2 appendResult8 = (float2(_SpeedMain , 0.0));
				float2 panner4 = ( 1.0 * _Time.y * appendResult8 + uv0_MainTex);
				
				
				finalColor = ( i.ase_color * ( ( tex2D( _MainTex, panner19 ).g * _SecondColorG * tex2DNode33.b * i.ase_color.a ) + ( _MainColorR * tex2D( _MainTex, panner4 ).r * i.ase_color.a * tex2DNode33.b ) ) );
				return finalColor;
			}
			ENDCG
		}
	}

	Fallback "Mobile/Particles/Additive"
}