Shader "qoockle/enviroment/unlit_vertexanimation_frag"
{
	Properties
	{
		_MainTex("MainTex", 2D) = "white" {}
		[NoScaleOffset]_VertexAnimationMask("VertexAnimationMask", 2D) = "white" {}
		_VertexOffsetPower("VertexOffsetPower", Float) = 1
		_TimePeriodScale("TimePeriodScale", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}
	
	SubShader
	{
		
		
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		LOD 100

		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off
		ColorMask RGBA
		ZWrite On
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

			uniform sampler2D _VertexAnimationMask;
			uniform half _VertexOffsetPower;
			uniform half _TimePeriodScale;
			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				float2 uv_VertexAnimationMask8 = v.ase_texcoord;
				float mulTime9 = _Time.y * _TimePeriodScale;
				
				o.ase_color = v.color;
				o.ase_texcoord.xy = v.ase_texcoord.xy;

				o.ase_texcoord.zw = 0;
				float3 vertexValue = ( ( 1.0 - tex2Dlod( _VertexAnimationMask, float4( uv_VertexAnimationMask8, 0, 0.0) ).r ) * v.ase_normal * _VertexOffsetPower * sin( mulTime9 ) );
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
				float2 uv_MainTex = i.ase_texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				
				
				finalColor = ( i.ase_color * tex2D( _MainTex, uv_MainTex ) );
				return finalColor;
			}
			ENDCG
		}
	}
	Fallback "Mobile/Particles/Alpha Blended"
}