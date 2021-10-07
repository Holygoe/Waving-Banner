using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;
using UnityEngine.EventSystems;
using RaycastHit = Unity.Physics.RaycastHit;

namespace WavingBanner
{
    public class Raycaster : MonoBehaviour
    {
        [SerializeField] private new Camera camera;
        
        private BuildPhysicsWorld _buildPhysicsWorld;
        private BannerBuildingSystem _bannerBuildingSystem;

        private void Start()
        {
            var world = World.DefaultGameObjectInjectionWorld;
            
            _buildPhysicsWorld = world.GetExistingSystem<BuildPhysicsWorld>();
            _bannerBuildingSystem = world.GetExistingSystem<BannerBuildingSystem>();
        }

        private void Update()
        {
            if (!Input.GetMouseButtonDown(0) || EventSystem.current.IsPointerOverGameObject()) return;

            var ray = camera.ScreenPointToRay(Input.mousePosition);
            
            if (CastRay(ray.origin, ray.origin + ray.direction * 100000, out var hit))
            {
                _bannerBuildingSystem.DestroyCube(hit.Entity);
            }
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
