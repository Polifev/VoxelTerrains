using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace VoxelTerrains.ScalarField
{
    public class ShaderScalarField : AbstractScalarField
    {
        [SerializeField]
        private float _maxHeight = 10.0f;
        [SerializeField]
        private Material _material = null;
        private IDictionary<Vector2Int, Texture2D> _textures = new Dictionary<Vector2Int, Texture2D>();

        public override event TerrainChangedEventHandler OnTerrainChanged;

        public override float ValueAt(Vector3 vector)
        {
            Vector2 offset = new Vector2(vector.x, vector.z);
            Vector3Int chunkIndex = Util.GetChunkIndex(vector, Vector3Int.one * 2048);
            Vector2Int textureIndex = new Vector2Int(chunkIndex.x, chunkIndex.z);
            Vector2 textureOffset = textureIndex * 2048;

            if (!_textures.ContainsKey(textureIndex))
            {
                var renderTexture = RenderTexture.GetTemporary(2048, 2048);

                _material.SetVector("_Position", new Vector4(textureIndex.x, textureIndex.y, 0, 0));
                _material.SetFloat("_Scale", 16.0f);
                Graphics.Blit(null, renderTexture, _material);
                
                Texture2D texture = new Texture2D(2048, 2048);
                RenderTexture.active = renderTexture;
                texture.ReadPixels(new Rect(Vector2.zero, new Vector2(2048, 2048)), 0, 0);
                _textures.Add(textureIndex, texture);

                RenderTexture.active = null;
                RenderTexture.ReleaseTemporary(renderTexture);
            }
            var delta = (offset - textureOffset) / 2048;
            var height = ValueFromColor(_textures[textureIndex].GetPixelBilinear(delta.x, delta.y));
            return (vector.y > _maxHeight * height) ? -1 : 1;
        }

        private float ValueFromColor(Color c)
        {
            return (c.r) * 2 - 1;
        }
    }
}
