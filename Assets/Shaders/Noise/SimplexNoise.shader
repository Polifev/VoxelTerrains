Shader "Noise/SimplexNoise"
{
    Properties
    {
        _Position("Position", Vector) = (0,0,0,0)
        _Scale ("Scale", Range (0.1, 30.0)) = 2.0
        _Lacunarity("Lacunarity", Range(0,2)) = 2.0
        _Persistance("Persistance", Range(0,1)) = 0.5
        _Octave("Octave", Int) = 6
        _Seed("Seed", Int) = 0 
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST, _Position;
            float _Scale, _Lacunarity, _Persistance;
            int _Octave, _Seed;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }


            float4 hash( float4 position )
            {
                position = float4( dot(position,float4(127.1,311.7,865.14,58.1203)), 
                                   dot(position,float4(269.5,183.3,185.734,45.127)), 
                                   dot(position,float4(126.847,167.512,471.654,212.406)), 
                                   dot(position,float4(945.801,740.965,720.987,515.568)));
                return -1.0 + 2.0 * frac(sin(position)*43758.5453123);
            }

            float noise4d( float4 position )
            {
                position *= _Scale;
                const float K1 = 0.366025404;
                const float K2 = 0.211324865;

                float4 cell = floor( position + (position.x + position.y) * K1 );
                float4 a = position - cell + (cell.x + cell.y) * K2;
                float  m = step( a.y, a.x); 
                float4 o = float4(m, 1.0 - m, m , 1.0 - m);
                float4 b = a - o + K2;
                float4 c = a - 1.0 + 2.0 * K2;
                float4 d = a - c + c * K2;

                float4  h = max( 0.5 - float4(dot(a,a), 
                                              dot(b,b),
                                              dot(c,c),
                                              dot(d,d) ), 0.0 );
                float4  n = h * h * h * float4( dot(a,hash(cell + 0.0)), 
                                                    dot(b,hash(cell + o)), 
                                                    dot(c,hash(cell + 1.0)), 
                                                    dot(d,hash(cell + c)));

                return dot( n, float4(70.0, 70.0, 70.0, 70.0) );
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float noise = 0.0;
                float increment = 0.0;

                for (int j = 0; j < _Octave; j++)
                {
                    float frequency = pow(_Lacunarity, j);
                    float amplitude = pow(_Persistance, j);
                    float currentOctave =  noise4d((_Position + float4(i.uv - float2(0.5,0.5),0.0,0.0)) * frequency) * amplitude;
                    noise = noise + currentOctave;
                    increment += amplitude;
                }
                noise = noise / increment;
                
                float3 col = float3(1.0,1.0,1.0) * ((noise + 1.0) * 0.5);
                return float4(col, 1.0);
            }
            ENDCG
        }
    }
}
