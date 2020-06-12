Shader "Voxel/FlatShaded"
{
	Properties
	{
		_Gloss ("Glossiness", Range(0.0,1.0)) = 0.2
		_Metalness ("Metalness", Range(0.0,1.0)) = 0.0
	}
	SubShader
	{
		Pass
		{
			Tags {"LightMode"="ForwardBase"}
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 5.0
			#pragma multi_compile_fwdbase
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"
			
			half _Gloss, _Metalness;

			struct appData	
			{
				float4 vertex	: POSITION;
				half3 normal	: NORMAL;
				half4 tangent	: TANGENT;
				float2 texcoord	: TEXCOORD0;
				half4 color : COLOR;
			};

			struct v2f	
			{
				float4 pos		: SV_POSITION;
				half4 T_wPosX	: ATTR0;
				half4 B_wPosY	: ATTR1;
				half4 N_wPosZ	: ATTR2;
				float2 UV		: ATTR3;
				nointerpolation half4 color : COLOR;
				float3 wPos : ATTR6;
				LIGHTING_COORDS(4, 5)
			};

			v2f vert(appData v)
			{
				v2f o;
				o.UV = v.texcoord;
				o.color = v.color;	

				o.pos = UnityObjectToClipPos (v.vertex);
				o.N_wPosZ.xyz = normalize(mul(unity_ObjectToWorld, float4(v.normal,0)).xyz);
				o.T_wPosX.xyz = normalize(mul(unity_ObjectToWorld, half4(v.tangent.xyz,0)).xyz);
				o.B_wPosY.xyz = cross(o.N_wPosZ.xyz, o.T_wPosX.xyz) * v.tangent.w;

				o.wPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.T_wPosX.w = o.wPos.x;
				o.B_wPosY.w = o.wPos.y;
				o.N_wPosZ.w = o.wPos.z;
				
				TRANSFER_VERTEX_TO_FRAGMENT(o)
				
				return o;
			}

			float4 frag(v2f i) : COLOR
			{	
				float4 o;
				half3 N = normalize( cross( ddy( i.wPos ), ddx( i.wPos ) ) );
				half3 L = _WorldSpaceLightPos0;
				half3 wPos = float3(i.T_wPosX.w,i.B_wPosY.w,i.N_wPosZ.w);
				half3 V = normalize(_WorldSpaceCameraPos-wPos);
				half3 H = normalize(V+L);
				half3 R = reflect(-V,N);
				half NdotL = saturate(dot(N,L));
				half NdotH = saturate(dot(N,H));
				half LdotH = saturate(dot(L,H));
				half NdotV = saturate(dot(N,V));
				half3 baseColor = i.color;
				half gloss = _Gloss;
				half e = exp2(gloss*12);
				half metalness = _Metalness;
				
				//diffuse term
				half3 diffuse = NdotL * baseColor * (1-metalness) ;

				//analytic spec
				half specDistrib = NdotL * pow(NdotH,e) * (e *.125 + 1);

				half3 minSpec = float3(.05,.05,.05);
				half3 F0 = metalness * ( baseColor - minSpec) + minSpec;
				half3 invF0 = 1-F0;
				half fCurve = 1-LdotH;
				fCurve *= fCurve;
				fCurve *= fCurve;
				half3 fresnel = fCurve * (invF0) + F0;
				
				//env spec
				half envCurve = 1-NdotV;
				envCurve*=envCurve;
				envCurve*=envCurve;
				half3 envFresnel = envCurve * (invF0) + F0;
		
				half mipOffset = 1-saturate(gloss+envCurve);
				half4 envData = UNITY_SAMPLE_TEXCUBE_LOD(unity_SpecCube0, R,  mipOffset*7);
                half3 env = DecodeHDR (envData, unity_SpecCube0_HDR); 				
				
				//geovis
				half geoVis = NdotV * (1-gloss) + gloss;

				//spec term
				half3 specular = specDistrib * fresnel * geoVis;
				
				//ambient specular term
				half3 ambSpec = env * envFresnel * geoVis;
				
				//ambient diffuse term
				half4 ambData = UNITY_SAMPLE_TEXCUBE_LOD(unity_SpecCube0, N, 7);
                half3 ambient = DecodeHDR (ambData, unity_SpecCube0_HDR) * baseColor * (1-metalness); 				
				
				//final compo
				o.rgb = (diffuse + specular) * _LightColor0 * LIGHT_ATTENUATION(i) + ambSpec + ambient;
				o.a = 1;
				
				return o;
			}
			ENDCG
		}
		Pass
		{
			Tags {"LightMode" = "ShadowCaster"}
			Cull Back
			//Offset 0,-4 //PS4 oddity
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 5.0
			#include "Lighting.cginc"
			
			struct appData
			{
				float4 vertex	:	POSITION;
			};

			struct v2f
			{
				V2F_SHADOW_CASTER;
			};

			v2f vert(appData v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				TRANSFER_SHADOW_CASTER(o)
				return o;
			}
			
			half4 frag(v2f i) : SV_Target
			{		
				SHADOW_CASTER_FRAGMENT(i)				
			}
			ENDCG
		}	
	}
	FallBack "Diffuse"
}
