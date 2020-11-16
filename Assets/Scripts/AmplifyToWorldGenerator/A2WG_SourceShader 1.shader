// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "A2WG_SourceShader"
{
	Properties
	{
		_WorldPosition("WorldPosition", Vector) = (0,0,0,0)

	}
	
	SubShader
	{
		
		
		Tags { "RenderType"="Opaque" }
	LOD 0

		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend Off
		Cull Back
		ColorMask RGBA
		ZWrite On
		ZTest LEqual
		Offset 0 , 0
		
		
		
		Pass
		{
			Name "Unlit"
			Tags { "LightMode"="ForwardBase" }
			CGPROGRAM

			

			#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
			//only defining to not throw compilation error over Unity 5.5
			#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
			#endif
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"
			

			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 worldPos : TEXCOORD0;
#endif
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
				
			};

			uniform float3 _WorldPosition;
			float GetFBM_GetRandom( float useless , float2 st )
			{
				return frac(sin(dot(st.xy, float2(12.9898, 78.233))) * 43758.5453123);
			}
			
			float GetFBM_GetCustomNoise( float useless , float2 st )
			{
					float2 i = floor(st);
					float2 f = frac(st);
					float a = GetFBM_GetRandom(0,i);
					float b = GetFBM_GetRandom(0,i + float2(1.0, 0.0));
					float c = GetFBM_GetRandom(0,i + float2(0.0, 1.0));
					float d = GetFBM_GetRandom(0,i + float2(1.0, 1.0));
					float2 u = f * f*(3.0 - 2.0*f);
					return lerp(a, b, u.x) + (c - a)* u.y * (1.0 - u.x) + (d - b) * u.x * u.y;
			}
			
			float GetFBM( float useless , float2 pos , float baseRoughness , int octaves , float lacunarity , float persistance , float minValue , float strength , out float finalNoiseValue )
			{
				float noiseValue = 0;
					float frequency = baseRoughness;
					float amplitude = 1;
					for (int i = 0; i < octaves; i++)
					{
						float v = GetFBM_GetCustomNoise(0, pos * frequency);
						noiseValue += (v + 1) * 0.5 * amplitude;
						frequency *= lacunarity;
						amplitude *= persistance;
					}
					noiseValue = max(0, noiseValue - minValue);
					finalNoiseValue = noiseValue * strength;
					if (strength == 0)
					{
						finalNoiseValue = 0;
					}
					return finalNoiseValue;
			}
			
			float3 mod3D289( float3 x ) { return x - floor( x / 289.0 ) * 289.0; }
			float4 mod3D289( float4 x ) { return x - floor( x / 289.0 ) * 289.0; }
			float4 permute( float4 x ) { return mod3D289( ( x * 34.0 + 1.0 ) * x ); }
			float4 taylorInvSqrt( float4 r ) { return 1.79284291400159 - r * 0.85373472095314; }
			float snoise( float3 v )
			{
				const float2 C = float2( 1.0 / 6.0, 1.0 / 3.0 );
				float3 i = floor( v + dot( v, C.yyy ) );
				float3 x0 = v - i + dot( i, C.xxx );
				float3 g = step( x0.yzx, x0.xyz );
				float3 l = 1.0 - g;
				float3 i1 = min( g.xyz, l.zxy );
				float3 i2 = max( g.xyz, l.zxy );
				float3 x1 = x0 - i1 + C.xxx;
				float3 x2 = x0 - i2 + C.yyy;
				float3 x3 = x0 - 0.5;
				i = mod3D289( i);
				float4 p = permute( permute( permute( i.z + float4( 0.0, i1.z, i2.z, 1.0 ) ) + i.y + float4( 0.0, i1.y, i2.y, 1.0 ) ) + i.x + float4( 0.0, i1.x, i2.x, 1.0 ) );
				float4 j = p - 49.0 * floor( p / 49.0 );  // mod(p,7*7)
				float4 x_ = floor( j / 7.0 );
				float4 y_ = floor( j - 7.0 * x_ );  // mod(j,N)
				float4 x = ( x_ * 2.0 + 0.5 ) / 7.0 - 1.0;
				float4 y = ( y_ * 2.0 + 0.5 ) / 7.0 - 1.0;
				float4 h = 1.0 - abs( x ) - abs( y );
				float4 b0 = float4( x.xy, y.xy );
				float4 b1 = float4( x.zw, y.zw );
				float4 s0 = floor( b0 ) * 2.0 + 1.0;
				float4 s1 = floor( b1 ) * 2.0 + 1.0;
				float4 sh = -step( h, 0.0 );
				float4 a0 = b0.xzyw + s0.xzyw * sh.xxyy;
				float4 a1 = b1.xzyw + s1.xzyw * sh.zzww;
				float3 g0 = float3( a0.xy, h.x );
				float3 g1 = float3( a0.zw, h.y );
				float3 g2 = float3( a1.xy, h.z );
				float3 g3 = float3( a1.zw, h.w );
				float4 norm = taylorInvSqrt( float4( dot( g0, g0 ), dot( g1, g1 ), dot( g2, g2 ), dot( g3, g3 ) ) );
				g0 *= norm.x;
				g1 *= norm.y;
				g2 *= norm.z;
				g3 *= norm.w;
				float4 m = max( 0.6 - float4( dot( x0, x0 ), dot( x1, x1 ), dot( x2, x2 ), dot( x3, x3 ) ), 0.0 );
				m = m* m;
				m = m* m;
				float4 px = float4( dot( x0, g0 ), dot( x1, g1 ), dot( x2, g2 ), dot( x3, g3 ) );
				return 42.0 * dot( m, px);
			}
			

			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				
				float3 vertexValue = float3(0, 0, 0);
				#if ASE_ABSOLUTE_VERTEX_POS
				vertexValue = v.vertex.xyz;
				#endif
				vertexValue = vertexValue;
				#if ASE_ABSOLUTE_VERTEX_POS
				v.vertex.xyz = vertexValue;
				#else
				v.vertex.xyz += vertexValue;
				#endif
				o.vertex = UnityObjectToClipPos(v.vertex);

#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
#endif
				return o;
			}
			
			fixed4 frag (v2f i ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
				fixed4 finalColor;
#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 WorldPosition = i.worldPos;
#endif
				float3 WorldPosition30 = _WorldPosition;
				float useless1_g1 = 0.0;
				float2 temp_output_6_0_g1 = ( (WorldPosition30).xz * 1.0 * 0.1 );
				float2 st1_g1 = temp_output_6_0_g1;
				float localGetFBM_GetRandom1_g1 = GetFBM_GetRandom( useless1_g1 , st1_g1 );
				float useless2_g1 = localGetFBM_GetRandom1_g1;
				float2 st2_g1 = float2( 0,0 );
				float localGetFBM_GetCustomNoise2_g1 = GetFBM_GetCustomNoise( useless2_g1 , st2_g1 );
				float useless3_g1 = localGetFBM_GetCustomNoise2_g1;
				float2 pos3_g1 = temp_output_6_0_g1;
				float baseRoughness3_g1 = 0.319;
				int octaves3_g1 = 12;
				float lacunarity3_g1 = 1.793;
				float persistance3_g1 = 0.534;
				float minValue3_g1 = 1.45;
				float strength3_g1 = 20.0;
				float finalNoiseValue3_g1 = 0.0;
				float localGetFBM3_g1 = GetFBM( useless3_g1 , pos3_g1 , baseRoughness3_g1 , octaves3_g1 , lacunarity3_g1 , persistance3_g1 , minValue3_g1 , strength3_g1 , finalNoiseValue3_g1 );
				float temp_output_240_0 = ( localGetFBM3_g1 * 1.0 );
				float simplePerlin3D220 = snoise( ( WorldPosition30 * 0.01 ) );
				simplePerlin3D220 = simplePerlin3D220*0.5 + 0.5;
				float lerpResult231 = lerp( -10.0 , -100.0 , simplePerlin3D220);
				float lerpResult228 = lerp( 1.0 , simplePerlin3D220 , step( (WorldPosition30).y , lerpResult231 ));
				float temp_output_4_0_g2 = ( step( (WorldPosition30).y , max( pow( temp_output_240_0 , 0.5 ) , pow( temp_output_240_0 , 2.0 ) ) ) * lerpResult228 );
				float3 break12_g2 = float3( 0,0,0 );
				float4 appendResult9_g2 = (float4(( ( temp_output_4_0_g2 * 2.0 ) - 1.0 ) , break12_g2.x , break12_g2.y , break12_g2.z));
				
				
				finalColor = appendResult9_g2;
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=18000
-1920;109;1920;1019;-1716.817;-376.7684;1;True;False
Node;AmplifyShaderEditor.CommentaryNode;40;625.9085,681.0405;Inherit;False;479.0181;211;;2;30;166;WorldPosition;1,1,1,1;0;0
Node;AmplifyShaderEditor.Vector3Node;166;662.4075,732.6873;Inherit;False;Property;_WorldPosition;WorldPosition;0;0;Create;True;0;0;False;0;0,0,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RegisterLocalVarNode;30;849.9262,732.9284;Float;False;WorldPosition;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;221;1117.019,1479.773;Inherit;False;30;WorldPosition;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;224;1207.019,1630.773;Inherit;False;Constant;_Float0;Float 0;1;0;Create;True;0;0;False;0;0.01;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;185;599.5365,1122.477;Inherit;False;30;WorldPosition;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;223;1386.019,1570.773;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ComponentMaskNode;186;800.5717,1120.92;Inherit;False;True;False;True;True;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FunctionNode;240;1035.13,1118.843;Inherit;False;GetFBM;-1;;1;9cc1fa23bddc498499ca0c4ccb51791e;0;3;4;FLOAT2;0,0;False;5;FLOAT;1;False;7;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;220;1528.675,1566.751;Inherit;False;Simplex3D;True;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;201;1275.493,1116.876;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;161;1169.448,862.6371;Inherit;False;30;WorldPosition;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ComponentMaskNode;229;1322.019,1482.773;Inherit;False;False;True;False;True;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;231;1758.019,1586.773;Inherit;False;3;0;FLOAT;-10;False;1;FLOAT;-100;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;202;1274.493,1216.876;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;230;1814.019,1436.773;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;-10;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;170;1366.283,860.1818;Inherit;False;False;True;False;True;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;208;1531.812,1190.05;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;176;2142.283,879.1819;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;228;2074.019,1577.773;Inherit;False;3;0;FLOAT;1;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;218;2346.164,871.7766;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;258;2692.817,872.7684;Inherit;False;Output;-1;;2;c62a35cdb84ea5840886e76820a92b2a;1,14,1;2;4;FLOAT;0;False;7;FLOAT3;0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.DistanceOpNode;214;1938.114,1989.001;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;216;1772.114,2093.001;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;213;1319.114,1978.001;Inherit;False;30;WorldPosition;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.BreakToComponentsNode;215;1545.114,2094.001;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.StepOpNode;217;2085.114,1988.001;Inherit;False;2;0;FLOAT;20;False;1;FLOAT;10;False;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;2990.193,877.1817;Float;False;True;-1;2;ASEMaterialInspector;0;1;A2WG_SourceShader;0770190933193b94aaa3065e307002fa;True;Unlit;0;0;Unlit;2;True;0;1;False;-1;0;False;-1;0;1;False;-1;0;False;-1;True;0;False;-1;0;False;-1;True;False;True;0;False;-1;True;True;True;True;True;0;False;-1;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;RenderType=Opaque=RenderType;True;2;0;False;False;False;False;False;False;False;False;False;True;1;LightMode=ForwardBase;False;0;;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;1;True;False;;0
WireConnection;30;0;166;0
WireConnection;223;0;221;0
WireConnection;223;1;224;0
WireConnection;186;0;185;0
WireConnection;240;4;186;0
WireConnection;220;0;223;0
WireConnection;201;0;240;0
WireConnection;229;0;221;0
WireConnection;231;2;220;0
WireConnection;202;0;240;0
WireConnection;230;0;229;0
WireConnection;230;1;231;0
WireConnection;170;0;161;0
WireConnection;208;0;201;0
WireConnection;208;1;202;0
WireConnection;176;0;170;0
WireConnection;176;1;208;0
WireConnection;228;1;220;0
WireConnection;228;2;230;0
WireConnection;218;0;176;0
WireConnection;218;1;228;0
WireConnection;258;4;218;0
WireConnection;214;0;213;0
WireConnection;214;1;216;0
WireConnection;216;0;215;0
WireConnection;215;0;213;0
WireConnection;217;1;214;0
WireConnection;0;0;258;0
ASEEND*/
//CHKSM=42AC3176853ABDAD70C62D59076ED08CF41623B8