using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace WavingBanner
{
    [DisallowMultipleComponent]
    public class Banner : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
    {
        public const float CUBE_SPACING = 1.1f; 
        
        private const int DEFAULT_WIDTH = 71;
        private const int DEFAULT_HEIGHT = 71;
        private const float DEFAULT_RESTORE_CUBE_DELAY = 2f;
        private const int DEFAULT_SECTOR_COUNT = 3;

        private const string WIDTH_PREFS_KEY = "Banner Width";
        private const string HEIGHT_PREFS_KEY = "Banner Height";
        private const string RESTORE_CUBE_DELAY_PREFS_KEY = "Restore Cube Delay";
        private const string SECTOR_COUNT_PREFS_KEY = "Sector Count";

        public static Entity CubeSourceEntity;
        
        [SerializeField] private GameObject cubePrefab;

        private static int2 _size;
        private static float _restoreCubeDelay;
        private static int _sectorCount;
        
        public static int2 Size
        {
            get => _size;

            set
            {
                _size = value;
                
                PlayerPrefs.SetInt(WIDTH_PREFS_KEY, value.x);
                PlayerPrefs.SetInt(HEIGHT_PREFS_KEY, value.y);
                PlayerPrefs.Save();
            }
        }

        public static float RestoreCubeDelay
        {
            get => _restoreCubeDelay;

            set
            {
                _restoreCubeDelay = value;
                PlayerPrefs.SetFloat(RESTORE_CUBE_DELAY_PREFS_KEY, value);
                PlayerPrefs.Save();
            }
        }

        public static int SectorCount
        {
            get => _sectorCount;

            set
            {
                _sectorCount = value;
                SectorSize = (int) math.ceil((float) Size.y / value);
                PlayerPrefs.SetInt(SECTOR_COUNT_PREFS_KEY, value);
                PlayerPrefs.Save();
            }
        }
        
        public static int SectorSize { get; private set; }

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            CubeSourceEntity = conversionSystem.GetPrimaryEntity(cubePrefab);
        }

        public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
        {
            referencedPrefabs.Add(cubePrefab);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize()
        {
            var width = PlayerPrefs.GetInt(WIDTH_PREFS_KEY, DEFAULT_WIDTH);
            var height = PlayerPrefs.GetInt(HEIGHT_PREFS_KEY, DEFAULT_HEIGHT);

            _size = new int2(width, height);
            _restoreCubeDelay = PlayerPrefs.GetFloat(RESTORE_CUBE_DELAY_PREFS_KEY, DEFAULT_RESTORE_CUBE_DELAY);
            SectorCount = PlayerPrefs.GetInt(SECTOR_COUNT_PREFS_KEY, DEFAULT_SECTOR_COUNT);
        }
    }
}
