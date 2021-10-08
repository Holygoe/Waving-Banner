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

        private float _wavingTime;

        public float GetCubeZPosition(float2 cubePosition)
        {
            return GetCubeZPosition(_wavingTime, cubePosition);
        }
        
        protected override void OnCreate()
        {
            _wavingTime = 0;
        }

        protected override void OnUpdate()
        {
            _wavingTime += Time.DeltaTime;
            var wavingTime = _wavingTime;
            
            Banner.SetColorTime(wavingTime);

            Entities.WithAll<CubeIndex>().ForEach((ref Translation translation) =>
            {
                translation.Value.z = GetCubeZPosition(wavingTime, translation.Value.xy);
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
