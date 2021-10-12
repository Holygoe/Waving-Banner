using Unity.Entities;
using UnityEngine;
using UnityEngine.EventSystems;

namespace WavingBanner
{
    public class InputSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                EntityManager.CreateEntity(typeof(RaycastEventTag));
            }
        }
    }
}