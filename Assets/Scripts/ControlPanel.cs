using System.Globalization;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace WavingBanner
{
    public class ControlPanel : MonoBehaviour
    {
        private const int MIN_BANNER_SIZE = 20;
        private const int MAX_BANNER_SIZE = 500;
        private const int MIN_SECTOR_COUNT = 1;
        private const int MAX_SECTOR_COUNT = 10;

        [SerializeField] private InputField columnCount;
        [SerializeField] private InputField rowCount;
        [SerializeField] private InputField sectorCount;
        [SerializeField] private Slider restoreDelay;
        [SerializeField] private Text restoreDelayText;
        [SerializeField] private Text pauseText;

        private void OnEnable()
        {
            var bannerSize = Banner.Size;

            columnCount.text = bannerSize.x.ToString();
            rowCount.text = bannerSize.y.ToString();
            sectorCount.text = Banner.SectorCount.ToString();
            restoreDelay.value = Banner.RestoreCubeDelay;
            UpdateRestoreDelayText(true);
        }

        public void Pause()
        {
            pauseText.text = Banner.PauseWaving() ? "Play" : "Pause";
        }

        public void Apply()
        {
            var columnCountValue = GetRangedValue(columnCount, MIN_BANNER_SIZE, MAX_BANNER_SIZE);
            var rowCountValue = GetRangedValue(rowCount, MIN_BANNER_SIZE, MAX_BANNER_SIZE);
            var bannerSize = new int2(columnCountValue, rowCountValue);
            Banner.Size = bannerSize;

            Banner.SectorCount = GetRangedValue(sectorCount, MIN_SECTOR_COUNT, MAX_SECTOR_COUNT);
            
            Banner.RestoreCubeDelay = restoreDelay.value;
            UpdateRestoreDelayText(true);
            
            var bannerBuildingSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<BannerBuildingSystem>();
            bannerBuildingSystem.ClearBanner();
            bannerBuildingSystem.BuildBanner();
        }

        public void UpdateRestoreDelayText(bool isApplied)
        {
            var value = math.round(restoreDelay.value * 100) / 100;

            var text = isApplied
                ? $"{value.ToString(CultureInfo.InvariantCulture)} sec"
                : $"{value.ToString(CultureInfo.InvariantCulture)} sec (not applied)";
            
            restoreDelayText.text = text;
        }

        private static int GetRangedValue(InputField inputField, int minValue, int maxValue)
        {
            int value;
            
            if (string.IsNullOrEmpty(inputField.text))
            {
                value =  minValue;
            }
            else
            {
                value = int.Parse(inputField.text);
                value = math.clamp(value, minValue, maxValue);
            }
            
            inputField.text = value.ToString();

            return value;
        }
    }
}
