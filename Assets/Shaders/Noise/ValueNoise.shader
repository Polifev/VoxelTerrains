Shader "Noise/ValueNoise"
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


            float random(float4 value, float4 dotDir)
            {
                float3 smallV = sin(value);
                float random = dot(smallV, dotDir);
                random = frac(sin(random) * 187410.43212);
                return random;
            }
            
            float hash(float4 position)
            {
                position  = frac( position * 47835.63875 + 0.738475 );
                position *= 17.0;
                return frac( position.x * position.y * position.z * position.w * (position.x + position.y + position.z + position.w) + frac((_Seed + 1.0) * 15613.5741));
            }

            float noise4d( float4 position )
            {
                position *= _Scale;
                float4 cell = floor(position);
                float4 decimal = frac(position);
                decimal = decimal * decimal * (3.0 - 2.0 * decimal);
                
                return lerp(lerp(lerp(lerp(hash(cell + float4(0.0,0.0,0.0,0.0)), 
                                           hash(cell + float4(1.0,0.0,0.0,0.0)),decimal.x),
                                      lerp(hash(cell + float4(0.0,1.0,0.0,0.0)), 
                                           hash(cell + float4(1.0,1.0,0.0,0.0)),decimal.x),decimal.y),
                                 lerp(lerp(hash(cell + float4(0.0,0.0,1.0,0.0)), 
                                           hash(cell + float4(1.0,0.0,1.0,0.0)),decimal.x),
                                      lerp(hash(cell + float4(0.0,1.0,1.0,0.0)), 
                                           hash(cell + float4(1.0,1.0,1.0,0.0)),decimal.x),decimal.y),decimal.z),
                            lerp(lerp(lerp(hash(cell + float4(0.0,0.0,0.0,1.0)), 
                                           hash(cell + float4(1.0,0.0,0.0,1.0)),decimal.x),
                                      lerp(hash(cell + float4(0.0,1.0,0.0,1.0)), 
                                           hash(cell + float4(1.0,1.0,0.0,1.0)),decimal.x),decimal.y),
                                 lerp(lerp(hash(cell + float4(0.0,0.0,1.0,1.0)), 
                                           hash(cell + float4(1.0,0.0,1.0,1.0)),decimal.x),
                                      lerp(hash(cell + float4(0.0,1.0,1.0,1.0)), 
                                           hash(cell + float4(1.0,1.0,1.0,1.0)),decimal.x),decimal.y),decimal.z), decimal.w);
            }


            fixed4 frag (v2f i) : SV_Target
            {
                float noise = 0.0;
                float increment = 0.0;
                for (int j = 0; j < _Octave; j++)
                {
                    float frequency = pow(_Lacunarity, j);
                    float amplitude = pow(_Persistance, j);
                    float currentOctave =  noise4d((_Position + float4(i.uv - float2(0.5,0.5), 0.0, 0.0)) * frequency) * amplitude;
                    noise = noise + currentOctave;
                    increment += amplitude;
                }
                noise = noise / increment;
                
                float3 col = float3(1.0,1.0,1.0) * noise;
                return float4(col, 1.0);
            }
            ENDCG
        }
    }
}
