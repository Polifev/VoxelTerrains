namespace VoxelTerrains
{
    public class Chunk
    {
        public static readonly int SIZE = 64;
        public float[] Data;

        public Chunk(float[] data)
        {
            Data = data;
        }
    }
}
