using UnityEngine;

namespace KKH
{
    public class CleaningManager : MonoBehaviour
    {
        public static CleaningManager Instance { get; private set; }

        [Range(0, 100)] public int cleanliness = 0;
        public int maxCleanliness = 100;

        [Header("Ambience")]
        public Light mainLight;
        public float dirtyLightIntensity = 0.7f;
        public float cleanLightIntensity = 1.2f;

        void Awake()
        {
            Instance = this;
            ApplyAmbience();
        }

        public void OnItemSorted(CarryItem item)
        {
            cleanliness = Mathf.Clamp(cleanliness + item.cleanValue, 0, maxCleanliness);
            ApplyAmbience();
        }

        void ApplyAmbience()
        {
            float t = (maxCleanliness <= 0) ? 0f : (float)cleanliness / maxCleanliness;

            if (mainLight)
                mainLight.intensity = Mathf.Lerp(dirtyLightIntensity, cleanLightIntensity, t);

            // 여기에 추가로:
            // - Volume(포스트프로세싱) 파라미터 보간
            // - AudioMixer 스냅샷 전환
            // 등을 붙이면 “힐링 체감”이 확 올라감
        }
    }
}