using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace WavingBanner
{
    public class BannerWavingSystem : SystemBase
    {
        private const float WAVING_SPEED = 6f;
        private const float WAVING_SIZE = 15f;
        private const float WAVING_VERTICAL_OFFSET = 0.1f;
        private const float WAVING_AMPLITUDE = 3f;
        
        [BurstCompile]
        protected override void OnUpdate()
        {
            var elapsedTime = (float) Time.ElapsedTime;

            Entities.ForEach((ref Translation translation, in CubeIndex _) =>
            {
                translation.Value.z = GetCubeZPosition(elapsedTime, translation.Value.xy);
            }).Schedule();
        }

        private static float GetCubeZPosition(float elapsedTime, float2 cubePosition)
        {
            return WAVING_AMPLITUDE * math.sin(elapsedTime * WAVING_SPEED 
                                               + cubePosition.x / WAVING_SIZE 
                                               + cubePosition.y * WAVING_VERTICAL_OFFSET);
        }
    }
}
