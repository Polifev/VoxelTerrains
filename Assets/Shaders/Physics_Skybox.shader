Shader "Skybox/PhysicSky" {
Properties {
    _Samples ("Samples", Int) = 16
    _LightSamples ("Light Samples", Int) = 8
    _PosOffset ("Position Offset", Vector) = (0.0,0.0,0.0,0.0)
    [NoScaleOffset] _StarMapTexture ("Star Map Texture", 2D) = "black" {}
    _StarMapIntensity ("Star Map Intensity", Range(0,10)) = 1.0
    _StarMapSpeed ("Star Map Speed", Range(-3,3)) = 0.25
    [HDR] _SunColor("Sun Color", Color) = (1.0,0.965,0.93,1.0)
    _SunIntensity ("Sun Intensity", Int) = 20
    _SunSize("Sun Size", Range(0.0,5.0)) = 2.0
    _SunFade("Sun Fade", Range(0.0,1.0)) = 0.5
    _PlanetRadius ("Planet Radius", Range(50000,50000000)) = 6360000.0
    _AtmosphereRadius ("Atmosphere Radius", Range(50000,50000000)) = 6420000.0
    _ScaleHeightRayleight ("Scale Height Rayleight", Float) = 7994.0
    _ScaleHeightMie ("Scale Height Mie", Float) = 1200.0
    _RayleighMultiplication ("Rayleigh Multiplication", Range(0.0,5.0)) = 1.0
    _MieMultiplication ("_Mie Multiplication", Range(0.0,5.0)) = 1.0
    _MieScatteringCoefficient ("Mie Scattering Coefficient", Range(0.000001,0.00005)) = 0.000021
    _MolecularDensityRed ("Molecular Density Red", Range(0.0000001,0.000005)) = 0.00000038
    _MolecularDensityGreen ("Molecular Density Green", Range(0.0000001,0.000005)) = 0.00000135
    _MolecularDensityBlue ("Molecular Density Blue", Range(0.0000001,0.000005)) = 0.00000331
}

SubShader {
    Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
    Cull Off ZWrite Off

    Pass {

        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag

        #include "UnityCG.cginc"
        #include "Lighting.cginc"

        sampler _StarMapTexture;
        float3 _PosOffset;
        half3 _SunColor;
        int _Samples, _LightSamples;
        half _SunIntensity, _PlanetRadius, _AtmosphereRadius, _ScaleHeightMie, _RayleighMultiplication, _MieMultiplication, 
        _ScaleHeightRayleight, _MieScatteringCoefficient, _MolecularDensityBlue, _MolecularDensityGreen, _MolecularDensityRed,
        _SunSize, _SunFade, _StarMapIntensity, _StarMapSpeed;

        #define RED   0.000000440 //Red Wavelength
        #define GREEN 0.000000550 //Green Wavelength
        #define BLUE  0.000000680 //Blue Wavelength
        #define AIRREFRACTION 1.00027717 //Index of redraction of air
        #define AEROSOLSANISOTROPY 0.76 //Aeorosols anisotropy value use in some paper        
        #define PI 3.141592653589793

        struct appdata
        {
            float4 vertex : POSITION;
            float2 uv : TEXCOORD0;
        };

        struct v2f
        {
            float3 wPos : TEXCOORD0;
            float4 pos : SV_POSITION;
        };

        v2f vert (appdata v)
        {
            v2f o;
            o.pos = UnityObjectToClipPos(v.vertex);
            o.wPos = mul(unity_ObjectToWorld, v.vertex).xyz;
            return o;
        }

        half rayleighScattering(half height, half wavelength, half molecularDensity)
        {
            //βsR(h,λ)=((8πe3*(ne2−1)e2)/(3Nλe4))e(−h/Hr)  I subtitute 8π3 by it'result to speed up
            half bsr = pow((15875.21366 * ((AIRREFRACTION * AIRREFRACTION - 1.0)*(AIRREFRACTION * AIRREFRACTION - 1.0)))
                        / (((3.0 * molecularDensity * wavelength)*(3.0 * molecularDensity * wavelength))*((3.0 * molecularDensity * wavelength)*(3.0 * molecularDensity * wavelength)))
                        , -(height / _ScaleHeightRayleight));
            return bsr;
        }

        half rayleightPhase (half mu)
        {
            half phase = 0.059683 * (1.0 + mu * mu); 
            return phase;
        }

        half mieScattering(half height, half wavelength)
        {
            //βsM(h,λ)=βsM(0,λ)e(−h/HM)
            //βsM(0,λ) = Mie scatterin coefficient = MIESCATTERINGCOEFFICIENT
            half bsm = pow(_MieScatteringCoefficient, -(height/_ScaleHeightMie));
            return bsm;
        }

        half miePhase (half mu)
        {
            // PM(μ)=(3/8π)*(((1−ge2)*(1+μe2))/((2+ge2)*(1+ge2−2gμ)e3/2))
            half anisotropySquare = AEROSOLSANISOTROPY * AEROSOLSANISOTROPY;
            half phase = 0.11936620731 * ((1.0 - anisotropySquare) * (1.0 + mu * mu)) 
                                        / ((2.0 + anisotropySquare) * pow(1.0 + anisotropySquare  - 2.0 * AEROSOLSANISOTROPY * mu, 1.5));
            return phase;
        }

        bool solveQuadratic(half a, half b, half c, out half x1, out half x2)
        {
            if (b == 0.0) 
            { 
                // Handle special case where the the two vector ray.dir and V are perpendicular
                // with V = ray.orig - sphere.centre
                if (a == 0.0) return false; 
                x1 = 0.0; 
                x2 = sqrt(-c / a); 
                return true; 
            } 
            half discr = b * b - 4.0 * a * c; 
        
            if (discr < 0.0) return false; 
        
            half q = 0.0;
            if(b < 0.0)
            {
                q = -0.5 * (b - sqrt(discr));
            }
            else
            {
                q = -0.5 * (b + sqrt(discr));
            }

            x1 = q / a; 
            x2 = c / q; 
        
            return true; 
        }

        bool raySphereIntersect(half3 ro, half3 rd, half radius, out half t0, out half t1) 
        {
            half a = rd.x * rd.x + rd.y * rd.y + rd.z * rd.z; 
            half b = 2 * (rd.x * ro.x + rd.y * ro.y + rd.z * ro.z); 
            half c = ro.x * ro.x + ro.y * ro.y + ro.z * ro.z - radius * radius; 

            if (!solveQuadratic(a, b, c, t0, t1)) return false;

            if (t0 > t1)
            {
                half temp = t0;
                t0 = t1;
                t1 = temp;
            }

            return true;
        } 
 
        half2 radialCoords(half3 cCoord)
        {
            half3 cCoord_n = normalize(cCoord);
            half lon = atan2(cCoord_n.z, cCoord_n.x);
            half lat = acos(cCoord_n.y);
            half2 sphereCoords = half2(lon, lat) * (1.0 / PI);
            return half2(sphereCoords.x * 0.5 + 0.5, 1 - sphereCoords.y);
        }

        float3x3 rotateX(half rad)
        {
            half c = cos(rad);
            half s = sin(rad);
            return float3x3(1,0,0,0,c,-s,0,s,c);
        }

        half3 computeIncidentLight(half3 ro, half3 rd, half tmin, half tmax)
        {
            half t0;
            half t1;

            if(!raySphereIntersect(ro, rd, _AtmosphereRadius, t0, t1) || t1 < 0) 
            {
                return half3(0.0,0.0,0.0);
            }
            if(t0 > tmin && t0 > 0) tmin = t0;
            if(t1 < tmax) tmax = t1;
            half segmentlength = (tmax - tmin) / _Samples;
            half tCurrent = tmin;
            half3 sumM = half3(0.0,0.0,0.0);
            half3 sumR = half3(0.0,0.0,0.0);
            half opticalDepthR = 0.0;
            half opticalDepthM = 0.0;
            half mu = dot(rd, _WorldSpaceLightPos0.xyz);
            half phaseR = rayleightPhase(mu);
            half phaseM = miePhase(mu);
            half3 betaR = half3(_MolecularDensityRed, _MolecularDensityGreen, _MolecularDensityBlue);

            for(int i = 0; i < _Samples; i++)
            {
                half3 samplePosition = ro + (tCurrent + segmentlength * 0.5) * rd;
                half height = length(samplePosition) - _PlanetRadius;
                half hr = exp(-height / _ScaleHeightRayleight) * segmentlength;
                half hm = exp(-height / _ScaleHeightMie) * segmentlength;
                opticalDepthR += hr;
                opticalDepthM += hm;
                half t0Light;
                half t1Light;
                raySphereIntersect(samplePosition, _WorldSpaceLightPos0.xyz, _AtmosphereRadius, t0Light, t1Light);
                half segmentLengthLight = t1Light / _LightSamples;
                half tCurrentLight = 0.0;
                half opticalDepthLightR = 0.0;
                half opticalDepthLightM = 0.0;
                int j = 0;
                for(j = 0; j < _LightSamples; j++)
                {
                    half3 samplePositionLight = samplePosition + (tCurrentLight + segmentLengthLight * 0.5) * _WorldSpaceLightPos0.xyz;
                    half heightLight = length(samplePositionLight) - _PlanetRadius;
                    if(heightLight < 0.0 ) break;
                    opticalDepthLightR += exp(-heightLight / _ScaleHeightRayleight) * segmentLengthLight;
                    opticalDepthLightM += exp(-heightLight / _ScaleHeightMie) * segmentLengthLight;
                    tCurrentLight += segmentLengthLight;
                }
                if(j == _LightSamples)
                {
                    half3 tau = betaR * (opticalDepthR + opticalDepthLightR) + _MieScatteringCoefficient * 1.1 * (opticalDepthM + opticalDepthLightM);
                    half3 attenuation = half3(exp(-tau));
                    sumR += attenuation * hr;
                    sumM += attenuation * hm;
                }
                tCurrent += segmentlength;
            }
            half sun = smoothstep(_SunSize -_SunFade, _SunSize, phaseM) * max(0, dot(rd - float3(0.0,0.11,0.0), float3(0.0,1.0,0.0)));
            half3 color = (sumR * betaR * phaseR * _RayleighMultiplication + sumM * _MieScatteringCoefficient * phaseM * _SunColor * _MieMultiplication) * _SunIntensity;
            half2 starMapUV = radialCoords(mul(rotateX(frac(_Time.x * _StarMapSpeed)*PI*2.0) ,mul(float3x3(1,0,0,0,0,-1,0,1,0), rd)));
            starMapUV.x *= 2;
            half starMapMask = smoothstep( -0.0, -0.2 , _WorldSpaceLightPos0.y);
            half3 starMap = tex2D(_StarMapTexture, starMapUV).rgb * _StarMapIntensity * starMapMask;
            return color + sun * _SunColor + starMap;
        }


        half4 frag (v2f i) : SV_Target
        {
            half3 ro = _WorldSpaceCameraPos.xyz + _PosOffset;
            ro.y += _PlanetRadius;
            half3 rd = normalize(i.wPos);
            
            float groundMask = smoothstep(-0.05, -0.1, rd.y);
            rd = lerp(rd, abs(rd), groundMask);

            half3 color = computeIncidentLight(ro, rd, 0.0, 340282300000000000000000000000000000000.0);
            return half4(color * saturate((1.0 - groundMask) + 0.5),1.0);
        }
        ENDCG
    }
}


Fallback Off
}
