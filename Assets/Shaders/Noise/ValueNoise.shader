Shader "Noise/ValueNoise"
{
    Properties
    {
        _Position("Position", Vector) = (0,0,0,0)
        _Scale ("Scale", Range (0.1, 30.0)) = 2.0
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
            float _Scale;

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
            
            float4 random4d(float4 value)
            {
                return float4 ( random(value, float4(12.898, 68.54, 37.7298, 10.3548)),
                                random(value, float4(39.898, 26.54, 85.7238, 56.368)),
                                random(value, float4(76.898, 12.54, 8.6788, 37.4687)),
                                random(value, float4(81.642, 18.794, 5.684, 65.487)));
            }
            
            float noise4d(float4 value)
            {
                value *= _Scale;
                float4 interp = frac(value);
                interp = smoothstep(0.0, 1.0, interp);
                
                float4 WValues[2];
                for (int w = 0; w <= 1; w++)
                {
                    float4 ZValues[2];
                    for(int z = 0; z <= 1; z++)
                    {
                        float4 YValues[2];
                        for(int y = 0; y <= 1; y++)
                        {
                            float4 XValues[2];
                            for(int x = 0; x <= 1; x++)
                            {
                                float4 cell = floor(value) + float4(x,y,z,w);
                                XValues[x] = random4d(cell);
                            }
                            YValues[y] = lerp(XValues[0], XValues[1], interp.x);
                        }
                        ZValues[z] = lerp(YValues[0], YValues[1], interp.y);
                    }
                    WValues[w] = lerp(ZValues[0], ZValues[1], interp.z);
                }                
                float noise = lerp(WValues[0], WValues[1], interp.w);

                return noise;
            }


            fixed4 frag (v2f i) : SV_Target
            {
                float noise = 0.0;
                float increment = 0.0;
                float lacunarity = 2.0;
                float persistance = 0.5;
                int octave = 6;

                for (int j = 0; j < octave; j++)
                {
                    float frequency = pow(lacunarity, j);
                    float amplitude = pow(persistance, j);
                    float currentOctave =  noise4d((_Position + float4(i.uv.xy, 0.0, 0.0)) * frequency) * amplitude;
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
