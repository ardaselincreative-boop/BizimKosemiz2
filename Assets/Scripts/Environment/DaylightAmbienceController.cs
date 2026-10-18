using UnityEngine;

namespace BizimKosemiz.Environment
{
    /// <summary>
    /// Bahçe sahnesinin sıcak altın saat / sabah güneşi aydınlatmasını ve ortam ışığını yapılandırır.
    /// </summary>
    public class DaylightAmbienceController : MonoBehaviour
    {
        [SerializeField] private Color sunLightColor = new Color(1.0f, 0.95f, 0.84f, 1f);
        [SerializeField] private float sunIntensity = 1.25f;
        [SerializeField] private Color skyAmbientColor = new Color(0.68f, 0.82f, 0.94f, 1f);
        [SerializeField] private Color equatorAmbientColor = new Color(0.62f, 0.72f, 0.64f, 1f);
        [SerializeField] private Color groundAmbientColor = new Color(0.35f, 0.42f, 0.32f, 1f);

        private void Awake()
        {
            ApplyLightingSettings();
        }

        public void ApplyLightingSettings()
        {
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = skyAmbientColor;
            RenderSettings.ambientEquatorColor = equatorAmbientColor;
            RenderSettings.ambientGroundColor = groundAmbientColor;

            Light directional = GetComponent<Light>();
            if (directional != null && directional.type == LightType.Directional)
            {
                directional.color = sunLightColor;
                directional.intensity = sunIntensity;
                directional.shadows = LightShadows.Soft;
            }
        }
    }
}
