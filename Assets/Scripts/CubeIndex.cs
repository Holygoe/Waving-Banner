using System;
using Unity.Entities;
using Unity.Mathematics;

namespace WavingBanner
{
    [Serializable]
    [GenerateAuthoringComponent]
    public struct CubeIndex : IComponentData
    {
        public int2 Value;
    }
}
