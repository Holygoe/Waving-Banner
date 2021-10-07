using System;
using Unity.Entities;
using Unity.Rendering;

namespace WavingBanner
{
    [Serializable]
    [GenerateAuthoringComponent]
    [MaterialProperty("_ColorOffset", MaterialPropertyFormat.Float)]
    public struct ColorOffset : IComponentData
    {
        public float Value;
    }
}
