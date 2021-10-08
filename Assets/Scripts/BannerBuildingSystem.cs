using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace WavingBanner
{
    public class BannerBuildingSystem : SystemBase
    {
        private readonly Queue<RestoreRecord> _restoreRecords = new Queue<RestoreRecord>();
        
        private BannerWavingSystem _bannerWavingSystem;
        private EntityQuery _cubeQuery;
        
        public void BuildBanner()
        {
            var bannerSize = Banner.Size;

            for (var i = 0; i < bannerSize.x; i++)
            {
                for (var j = 0; j < bannerSize.y; j++)
                {
                    var index = new int2(i, j);
                    InstantiateCube(index);
                }
            }
        }

        public void ClearBanner()
        {
            _restoreRecords.Clear();
            
            using var cubes = _cubeQuery.ToEntityArray(Allocator.Temp);
            EntityManager.DestroyEntity(cubes);
        }

        public void DestroyCube(Entity cube)
        {
            var cubeIndex = EntityManager.GetComponentData<CubeIndex>(cube).Value;
            var restoreTimestamp = (float)Time.ElapsedTime + Banner.RestoreCubeDelay;
            var restoreRecord = new RestoreRecord(restoreTimestamp, cubeIndex);
            
            _restoreRecords.Enqueue(restoreRecord);
            EntityManager.DestroyEntity(cube);
        }
        
        protected override void OnStartRunning()
        {
            _bannerWavingSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<BannerWavingSystem>();
            _cubeQuery = GetEntityQuery(typeof(CubeIndex));
            
            BuildBanner();
        }

        protected override void OnUpdate()
        {
            var elapsedTime = Time.ElapsedTime;
            
            while (_restoreRecords.Count > 0 && _restoreRecords.Peek().Timestamp < elapsedTime)
            {
                InstantiateCube(_restoreRecords.Dequeue().Index);
            }
        }

        private void InstantiateCube(int2 index)
        {
            var cubeEntity = EntityManager.Instantiate(Banner.CubeSourceEntity);
                    
            var cubeData = new CubeIndex { Value = index };
            var position = (float2)index * Banner.CUBE_SPACING - (float2)Banner.Size * Banner.CUBE_SPACING / 2;
            var zPosition = _bannerWavingSystem.GetCubeZPosition(position);
            var cubeTranslation = new Translation { Value = new float3(position, zPosition) };
                    
            EntityManager.SetComponentData(cubeEntity, cubeData);
            EntityManager.SetComponentData(cubeEntity, cubeTranslation);

            var sectorIndex = index.y / Banner.SectorSize;
            var colorOffset = new ColorOffset { Value = (float)sectorIndex / (Banner.SectorCount + 1) };
            EntityManager.SetComponentData(cubeEntity, colorOffset);
        }

        private readonly struct RestoreRecord
        {
            public readonly float Timestamp;
            public readonly int2 Index;

            public RestoreRecord(float timestamp, int2 index)
            {
                Timestamp = timestamp;
                Index = index;
            }
        }
    }
}
