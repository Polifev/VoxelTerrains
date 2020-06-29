using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelTerrains.Noise
{
    public class ValueNoise : iNoise
    {
        int seed = 0;

        public ValueNoise()
        {
            this.seed = 0;
        }

        public ValueNoise(int seed)
        {
            this.seed = seed;
        }

        float Hash(Vector4 position)
        {
            position = (seed + 1f) * 5614f * new Vector4((position.x * 54.32045f + 543.9845f) % 1f, 
                                            (position.y * 453.78654f + 783.5751f) % 1f,
                                            (position.z * 746.2835f + 483.64245f) % 1f,
                                            (position.w * 1863.1426f + 483.45614f) % 1f);
            position = new Vector4((position.x) % 1f,
                                    (position.y) % 1f,
                                    (position.z) % 1f,
                                    (position.w) % 1f);

            return (position.x * position.y * position.z * position.w * (position.x + position.y + position.z + position.w)) % 1f;
        }

        float Hash(Vector3 position)
        {
            position = (seed + 1f) * 564f * new Vector3((position.x * 54.32045f + 543.9845f) % 1f,
                                                         (position.y * 453.78654f + 783.5751f) % 1f,
                                                         (position.z * 746.2835f + 483.64245f) % 1f);
            position = new Vector4( (position.x) % 1f,
                                     (position.y) % 1f,
                                     (position.z) % 1f);

            return (position.x * position.y * position.z * (position.x + position.y + position.z) ) % 1f;
        }

        float Hash(Vector2 position)
        {
            position = (seed + 1f) * 5614f * new Vector2((position.x * 54.32045f + 543.9845f) % 1f,
                                                         (position.y * 453.78654f + 783.5751f) % 1f);
            position = new Vector4( (position.x) % 1f,
                                     (position.y) % 1f);

            return (position.x * position.y * (position.x + position.y) ) % 1f;
        }

        public float eval(float x, float y)
        {
            Vector2 floor = new Vector2(Mathf.Floor(x), Mathf.Floor(y));
            Vector2 decimalValue = new Vector2( (x),  (y) % 1f);
            Vector2 LerpValue = new Vector2(decimalValue.x * decimalValue.x * (3.0f - 2.0f * decimalValue.x),
                                                decimalValue.y * decimalValue.y * (3.0f - 2.0f * decimalValue.y));

            float noise = Mathf.Lerp(Mathf.Lerp(Hash(floor),
                                                Hash(floor + new Vector2(1.0f, 0.0f)), LerpValue.x),
                                     Mathf.Lerp(Hash(floor + new Vector2(0.0f, 1.0f)),
                                                Hash(floor + new Vector2(1.0f,1.0f)), LerpValue.x), LerpValue.y);
            return noise;
        }

        public float eval(float x, float y, float z)
        {
            Vector3 floor = new Vector3(Mathf.Floor(x), Mathf.Floor(y), Mathf.Floor(z));
            Vector3 decimalValue = new Vector3( (x) % 1f,  (y) % 1f,  (z) % 1f);
            Vector3 LerpValue = new Vector3(decimalValue.x * decimalValue.x * (3.0f - 2.0f * decimalValue.x),
                                                decimalValue.y * decimalValue.y * (3.0f - 2.0f * decimalValue.y),
                                                decimalValue.z * decimalValue.z * (3.0f - 2.0f * decimalValue.z));

            float noise = Mathf.Lerp(Mathf.Lerp(Mathf.Lerp(Hash(floor),
                                                           Hash(floor + new Vector3(1.0f, 0.0f, 0.0f)), LerpValue.x),
                                                Mathf.Lerp(Hash(floor + new Vector3(0.0f, 1.0f, 0.0f)),
                                                           Hash(floor + new Vector3(1.0f, 1.0f, 0.0f)), LerpValue.x), LerpValue.y),
                                     Mathf.Lerp(Mathf.Lerp(Hash(floor + new Vector3(0.0f, 0.0f, 1.0f)),
                                                           Hash(floor + new Vector3(1.0f, 0.0f, 1.0f)), LerpValue.x),
                                                Mathf.Lerp(Hash(floor + new Vector3(0.0f, 1.0f, 1.0f)),
                                                           Hash(floor + new Vector3(1.0f, 1.0f, 1.0f)), LerpValue.x), LerpValue.y), LerpValue.z);
            return noise;
        }

        public float eval(float x, float y, float z, float w)
        {
            Vector4 floor = new Vector4(Mathf.Floor(x), Mathf.Floor(y), Mathf.Floor(z), Mathf.Floor(w));
            Vector4 decimalValue = new Vector4( (x) % 1f,  (y) % 1f,  (z) % 1f,  (w));
            Vector4 LerpValue = new Vector4(decimalValue.x * decimalValue.x * (3.0f - 2.0f * decimalValue.x),
                                                decimalValue.y * decimalValue.y * (3.0f - 2.0f * decimalValue.y),
                                                decimalValue.z * decimalValue.z * (3.0f - 2.0f * decimalValue.z),
                                                decimalValue.w * decimalValue.w * (3.0f - 2.0f * decimalValue.w));

            float noise = Mathf.Lerp(Mathf.Lerp(Mathf.Lerp(Mathf.Lerp(Hash(floor),
                                                                      Hash(floor + new Vector4(1.0f, 0.0f, 0.0f, 0.0f)), LerpValue.x),
                                                           Mathf.Lerp(Hash(floor + new Vector4(0.0f, 1.0f, 0.0f, 0.0f)),
                                                                      Hash(floor + new Vector4(1.0f, 1.0f, 0.0f, 0.0f)), LerpValue.x), LerpValue.y),
                                                Mathf.Lerp(Mathf.Lerp(Hash(floor + new Vector4(0.0f, 0.0f, 1.0f, 0.0f)),
                                                                      Hash(floor + new Vector4(1.0f, 0.0f, 1.0f, 0.0f)), LerpValue.x),
                                                           Mathf.Lerp(Hash(floor + new Vector4(0.0f, 1.0f, 1.0f, 0.0f)),
                                                                      Hash(floor + new Vector4(1.0f, 1.0f, 1.0f, 0.0f)), LerpValue.x), LerpValue.y), LerpValue.z),
                                     Mathf.Lerp(Mathf.Lerp(Mathf.Lerp(Hash(floor + new Vector4(1.0f, 0.0f, 0.0f, 1.0f)),
                                                                      Hash(floor + new Vector4(1.0f, 0.0f, 0.0f, 1.0f)), LerpValue.x),
                                                           Mathf.Lerp(Hash(floor + new Vector4(0.0f, 1.0f, 0.0f, 1.0f)),
                                                                      Hash(floor + new Vector4(1.0f, 1.0f, 0.0f, 1.0f)), LerpValue.x), LerpValue.y),
                                                Mathf.Lerp(Mathf.Lerp(Hash(floor + new Vector4(0.0f, 0.0f, 1.0f, 1.0f)),
                                                                      Hash(floor + new Vector4(1.0f, 0.0f, 1.0f, 1.0f)), LerpValue.x),
                                                           Mathf.Lerp(Hash(floor + new Vector4(0.0f, 1.0f, 1.0f, 1.0f)),
                                                                      Hash(floor + new Vector4(1.0f, 1.0f, 1.0f, 1.0f)), LerpValue.x), LerpValue.y), LerpValue.z), LerpValue.w);
            return noise;
        }
    }

}