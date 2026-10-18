using UnityEngine;

namespace BizimKosemiz.Environment
{
    /// <summary>
    /// URP (Universal Render Pipeline/Lit) veya Built-in (Standard) aktif olmasına göre
    /// doğru shader'ı seçer ve pembe/magenta (Missing Shader) materyal hatasını önler.
    /// </summary>
    public static class ShaderCompatibilityUtility
    {
        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
        private static readonly int ColorId = Shader.PropertyToID("_Color");
        private static readonly int SmoothnessId = Shader.PropertyToID("_Smoothness");
        private static readonly int GlossinessId = Shader.PropertyToID("_Glossiness");

        public static Shader GetCompatibleLitShader()
        {
            Shader urpLit = Shader.Find("Universal Render Pipeline/Lit");
            if (urpLit != null)
            {
                return urpLit;
            }

            Shader urpSimpleLit = Shader.Find("Universal Render Pipeline/Simple Lit");
            if (urpSimpleLit != null)
            {
                return urpSimpleLit;
            }

            Shader standard = Shader.Find("Standard");
            if (standard != null)
            {
                return standard;
            }

            return Shader.Find("Sprites/Default");
        }

        public static Material CreateStylizedMaterial(string name, Color color, float smoothness = 0.2f)
        {
            Shader shader = GetCompatibleLitShader();
            Material mat = new Material(shader)
            {
                name = name
            };
            ApplyColorAndSmoothness(mat, color, smoothness);
            return mat;
        }

        public static void EnsureValidMaterial(Renderer renderer, Color fallbackColor, float smoothness = 0.2f)
        {
            if (renderer == null)
            {
                return;
            }

            Material current = renderer.sharedMaterial;
            if (current == null || current.shader == null || current.shader.name == "Hidden/InternalErrorShader")
            {
                renderer.sharedMaterial = CreateStylizedMaterial(renderer.gameObject.name + "_Mat", fallbackColor, smoothness);
                return;
            }

            ApplyColorAndSmoothness(current, fallbackColor, smoothness);
        }

        public static void ApplyColorAndSmoothness(Material mat, Color color, float smoothness)
        {
            if (mat == null)
            {
                return;
            }

            if (mat.HasProperty(BaseColorId))
            {
                mat.SetColor(BaseColorId, color);
            }
            if (mat.HasProperty(ColorId))
            {
                mat.SetColor(ColorId, color);
            }
            if (mat.HasProperty(SmoothnessId))
            {
                mat.SetFloat(SmoothnessId, smoothness);
            }
            if (mat.HasProperty(GlossinessId))
            {
                mat.SetFloat(GlossinessId, smoothness);
            }
        }
    }
}
