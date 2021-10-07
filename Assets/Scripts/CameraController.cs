using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using Vector3 = UnityEngine.Vector3;

namespace WavingBanner
{
    public class CameraController : MonoBehaviour
    {
        private const string DISTANCE_PLAYER_PREF = "Camera Distance";
        private const string ROTATION_PLAYER_PREF = "Camera Rotation";
        private const float DEFAULT_ROTATION = 1.2f;
        
        [SerializeField] private Slider distanceSlider;
        [SerializeField] private Slider rotationSlider;

        private float _cameraDistance;

        private Vector3 LookAtPosition => Vector3.up * _cameraDistance * 0.1f;
        
        private void OnEnable()
        {
            var thisTransform = transform;
            distanceSlider.onValueChanged.AddListener(ChangeDistance);
            rotationSlider.onValueChanged.AddListener(ChangeRotation);

            _cameraDistance = PlayerPrefs.GetFloat(DISTANCE_PLAYER_PREF, thisTransform.position.magnitude);
            distanceSlider.value = math.sqrt(_cameraDistance);
            rotationSlider.value = PlayerPrefs.GetFloat(ROTATION_PLAYER_PREF, DEFAULT_ROTATION);
        }

        private void OnDisable()
        {
            distanceSlider.onValueChanged.RemoveListener(ChangeDistance);
            rotationSlider.onValueChanged.RemoveListener(ChangeRotation);
        }

        private void Update()
        {
            if (!Input.GetMouseButton(1)) return;

            var translation = Input.mouseScrollDelta * Time.deltaTime;
            transform.RotateAround(Vector3.zero, Vector3.up, translation.x);
        }

        private void ChangeDistance(float value)
        {
            var thisTransform = transform;
            _cameraDistance = math.lengthsq(value);
            thisTransform.position = _cameraDistance * thisTransform.position.normalized;
            
            PlayerPrefs.SetFloat(DISTANCE_PLAYER_PREF, _cameraDistance);
            PlayerPrefs.Save();
        }

        private void ChangeRotation(float value)
        {
            const float yPositionMultiplier = 0.3f;
            var thisTransform = transform;
            var position = new Vector3(math.cos(value), 0, math.sin(value)) * _cameraDistance;
            position += _cameraDistance * yPositionMultiplier * Vector3.up;
            
            thisTransform.position = position;
            thisTransform.LookAt(LookAtPosition);
            
            PlayerPrefs.SetFloat(ROTATION_PLAYER_PREF, value);
            PlayerPrefs.Save();
        }
    }
}