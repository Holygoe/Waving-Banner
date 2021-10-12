using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;
using UnityEngine.EventSystems;
using RaycastHit = Unity.Physics.RaycastHit;

namespace WavingBanner
{
    public class RaycastSystem : SystemBase
    {
        private Camera _camera;
        private BuildPhysicsWorld _buildPhysicsWorld;
        private BannerBuildingSystem _bannerBuildingSystem;
        private EntityCommandBufferSystem _commandBufferSystem;

        public void DoCastRay()
        {
            if (!Input.GetMouseButtonDown(0) || EventSystem.current.IsPointerOverGameObject()) return;

            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            
            if (CastRay(ray.origin, ray.origin + ray.direction * 100000, out var hit))
            {
                _bannerBuildingSystem.DestroyCube(hit.Entity);
            }
        }
        
        protected override void OnStartRunning()
        {
            _camera = Camera.main;
            
            var world = World.DefaultGameObjectInjectionWorld;
            _buildPhysicsWorld = world.GetExistingSystem<BuildPhysicsWorld>();
            _bannerBuildingSystem = world.GetExistingSystem<BannerBuildingSystem>();
        }

        protected override void OnUpdate()
        {
            if (!TryGetSingletonEntity<RaycastEventTag>(out var raycastEventTag)) return;
            
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            
            if (CastRay(ray.origin, ray.origin + ray.direction * 100000, out var hit))
            {
                _bannerBuildingSystem.DestroyCube(hit.Entity);
            }
            
            EntityManager.DestroyEntity(raycastEventTag);
        }

        private bool CastRay(Vector3 start, Vector3 end, out RaycastHit hit)
        {
            var raycastInput = new RaycastInput
            {
                Start = start,
                End = end,
                Filter = CollisionFilter.Default
            };

            return _buildPhysicsWorld.PhysicsWorld.CollisionWorld.CastRay(raycastInput, out hit);
        }
    }
}
