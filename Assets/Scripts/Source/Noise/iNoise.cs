using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoxelTerrains.Noise
{
    interface iNoise
    {
        float eval(float x, float y);
        float eval(float x, float y, float z);
        float eval(float x, float y, float z, float w);
    }
}
