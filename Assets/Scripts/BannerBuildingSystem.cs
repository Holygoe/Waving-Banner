using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace WavingBanner
{
    public class BannerBuildingSystem : SystemBase
    {
        private NativeArray<Entity> _cubes;
        private readonly Queue<RestoreRecord> _restoreRecords = new Queue<RestoreRecord>();

        public void Build()
        {
            var bannerSize = Banner.Size;

            _cubes = new NativeArray<Entity>(bannerSize.x * bannerSize.y, Allocator.Persistent);
            
            for (var i = 0; i < bannerSize.x; i++)
            {
                for (var j = 0; j < bannerSize.y; j++)
                {
                    var index = new int2(i, j);
                    InstantiateCube(index);
                }
            }
        }

        public void Clear()
        {
            _restoreRecords.Clear();
            EntityManager.DestroyEntity(_cubes);
            _cubes.Dispose();
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
            Build();
        }

        protected override void OnUpdate()
        {
            var elapsedTime = Time.ElapsedTime;
            
            while (_restoreRecords.Count > 0 && _restoreRecords.Peek().Timestamp < elapsedTime)
            {
                InstantiateCube(_restoreRecords.Dequeue().Index);
            }
        }

        protected override void OnDestroy()
        {
            _cubes.Dispose();
        }

        private void InstantiateCube(int2 index)
        {
            var cubeEntity = EntityManager.Instantiate(Banner.CubeSourceEntity);
                    
            var cubeData = new CubeIndex { Value = index };
            var position = (float2)index * Banner.CUBE_SPACING - (float2)Banner.Size * Banner.CUBE_SPACING / 2;
            
            var cubeTranslation = new Translation
            {
                Value = new float3(position, 0)
            };
                    
            EntityManager.SetComponentData(cubeEntity, cubeData);
            EntityManager.SetComponentData(cubeEntity, cubeTranslation);

            var sectorIndex = index.y / Banner.SectorSize;
            var colorOffset = new ColorOffset { Value = (float)sectorIndex / (Banner.SectorCount + 1) };
            EntityManager.SetComponentData(cubeEntity, colorOffset);
            
            var linerIndex = index.x + index.y * Banner.Size.x;

            _cubes[linerIndex] = cubeEntity;
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
