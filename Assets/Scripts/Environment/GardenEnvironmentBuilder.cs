using UnityEngine;

namespace BizimKosemiz.Environment
{
    /// <summary>
    /// Bizim Köşemiz bahçe ortamının stilize renk paletini, materyal onarımını ve
    /// çevre öğelerinin (zemin, yürüyüş yolu, ağaçlar, çiçek tarhları, banklar, çitler)
    /// doğru ölçek ve çarpışıcılarla (Collider) kurulumunu yönetir.
    /// </summary>
    public class GardenEnvironmentBuilder : MonoBehaviour
    {
        public static readonly Color GrassColor = new Color(0.36f, 0.67f, 0.35f, 1f);
        public static readonly Color PathStoneColor = new Color(0.84f, 0.78f, 0.68f, 1f);
        public static readonly Color TreeTrunkColor = new Color(0.46f, 0.31f, 0.20f, 1f);
        public static readonly Color TreeCanopyColor = new Color(0.24f, 0.56f, 0.29f, 1f);
        public static readonly Color BenchWoodColor = new Color(0.62f, 0.42f, 0.25f, 1f);
        public static readonly Color FlowerCoralColor = new Color(0.95f, 0.48f, 0.42f, 1f);
        public static readonly Color FlowerYellowColor = new Color(0.98f, 0.82f, 0.30f, 1f);
        public static readonly Color PergolaRoofColor = new Color(0.76f, 0.34f, 0.28f, 1f);

        [SerializeField] private Transform environmentRoot;

        public void ValidateAndPopulateGarden()
        {
            if (environmentRoot == null)
            {
                GameObject existing = GameObject.Find("GardenEnvironment");
                environmentRoot = existing != null ? existing.transform : new GameObject("GardenEnvironment").transform;
            }

            EnsureMainGround(environmentRoot);
            EnsureStonePathway(environmentRoot);
            EnsureSocialPergolaAndBenches(environmentRoot);
            EnsureStylizedTrees(environmentRoot);
            EnsureFlowerPatches(environmentRoot);
            EnsureBoundaryFences(environmentRoot);
        }

        private static void EnsureMainGround(Transform root)
        {
            Transform ground = root.Find("GardenGround");
            if (ground == null)
            {
                GameObject groundObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                groundObj.name = "GardenGround";
                groundObj.transform.SetParent(root, false);
                // Üst yüzey tam olarak y = 0.0f seviyesindedir (pivot merkezde olduğu için y = -0.5f, scale.y = 1.0f)
                groundObj.transform.localPosition = new Vector3(0f, -0.5f, 0f);
                groundObj.transform.localScale = new Vector3(60f, 1f, 60f);
                ground = groundObj.transform;
            }

            int groundLayer = LayerMask.NameToLayer("Ground");
            if (groundLayer >= 0)
            {
                ground.gameObject.layer = groundLayer;
            }

            BoxCollider col = ground.GetComponent<BoxCollider>();
            if (col == null)
            {
                col = ground.gameObject.AddComponent<BoxCollider>();
            }
            col.isTrigger = false;

            Renderer rend = ground.GetComponent<Renderer>();
            ShaderCompatibilityUtility.EnsureValidMaterial(rend, GrassColor, 0.12f);
        }

        private static void EnsureStonePathway(Transform root)
        {
            Transform pathRoot = root.Find("StonePathways");
            if (pathRoot != null)
            {
                RepairChildrenMaterials(pathRoot, PathStoneColor, 0.15f);
                return;
            }

            pathRoot = new GameObject("StonePathways").transform;
            pathRoot.SetParent(root, false);

            for (int i = -5; i <= 5; i++)
            {
                GameObject stone = GameObject.CreatePrimitive(PrimitiveType.Cube);
                stone.name = $"SteppingStone_{i + 6}";
                stone.transform.SetParent(pathRoot, false);
                stone.transform.localPosition = new Vector3(0f, 0.015f, i * 2.2f);
                stone.transform.localScale = new Vector3(2.2f, 0.03f, 1.6f);
                ShaderCompatibilityUtility.EnsureValidMaterial(stone.GetComponent<Renderer>(), PathStoneColor, 0.15f);
            }
        }

        private static void EnsureSocialPergolaAndBenches(Transform root)
        {
            Transform socialHub = root.Find("SocialSeatingArea");
            if (socialHub != null)
            {
                RepairChildrenMaterials(socialHub, BenchWoodColor, 0.25f);
                return;
            }

            socialHub = new GameObject("SocialSeatingArea").transform;
            socialHub.SetParent(root, false);

            CreateBench(socialHub, "GardenBench_West", new Vector3(-3.8f, 0f, 0f), Quaternion.Euler(0f, 90f, 0f));
            CreateBench(socialHub, "GardenBench_East", new Vector3(3.8f, 0f, 0f), Quaternion.Euler(0f, -90f, 0f));
            CreateBench(socialHub, "GardenBench_North", new Vector3(0f, 0f, 4.5f), Quaternion.Euler(0f, 180f, 0f));
        }

        private static void CreateBench(Transform parent, string name, Vector3 position, Quaternion rotation)
        {
            GameObject benchRoot = new GameObject(name);
            benchRoot.transform.SetParent(parent, false);
            benchRoot.transform.localPosition = position;
            benchRoot.transform.localRotation = rotation;

            GameObject seat = GameObject.CreatePrimitive(PrimitiveType.Cube);
            seat.name = "BenchSeat";
            seat.transform.SetParent(benchRoot.transform, false);
            seat.transform.localPosition = new Vector3(0f, 0.45f, 0f);
            seat.transform.localScale = new Vector3(2.2f, 0.12f, 0.65f);
            ShaderCompatibilityUtility.EnsureValidMaterial(seat.GetComponent<Renderer>(), BenchWoodColor, 0.25f);

            GameObject backrest = GameObject.CreatePrimitive(PrimitiveType.Cube);
            backrest.name = "BenchBackrest";
            backrest.transform.SetParent(benchRoot.transform, false);
            backrest.transform.localPosition = new Vector3(0f, 0.82f, -0.26f);
            backrest.transform.localScale = new Vector3(2.2f, 0.55f, 0.1f);
            ShaderCompatibilityUtility.EnsureValidMaterial(backrest.GetComponent<Renderer>(), BenchWoodColor, 0.25f);
        }

        private static void EnsureStylizedTrees(Transform root)
        {
            Transform treesRoot = root.Find("StylizedTrees");
            if (treesRoot != null)
            {
                return;
            }

            treesRoot = new GameObject("StylizedTrees").transform;
            treesRoot.SetParent(root, false);

            Vector3[] treePositions =
            {
                new Vector3(-9f, 0f, 8f),
                new Vector3(9f, 0f, 8f),
                new Vector3(-11f, 0f, -6f),
                new Vector3(11f, 0f, -6f),
                new Vector3(-6f, 0f, 14f),
                new Vector3(7f, 0f, 14f),
            };

            for (int i = 0; i < treePositions.Length; i++)
            {
                GameObject tree = new GameObject($"StylizedTree_{i + 1}");
                tree.transform.SetParent(treesRoot, false);
                tree.transform.localPosition = treePositions[i];

                GameObject trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                trunk.name = "Trunk";
                trunk.transform.SetParent(tree.transform, false);
                trunk.transform.localPosition = new Vector3(0f, 1.2f, 0f);
                trunk.transform.localScale = new Vector3(0.55f, 1.2f, 0.55f);
                ShaderCompatibilityUtility.EnsureValidMaterial(trunk.GetComponent<Renderer>(), TreeTrunkColor, 0.15f);

                GameObject canopyLower = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                canopyLower.name = "CanopyLower";
                canopyLower.transform.SetParent(tree.transform, false);
                canopyLower.transform.localPosition = new Vector3(0f, 3.0f, 0f);
                canopyLower.transform.localScale = new Vector3(2.8f, 2.2f, 2.8f);
                ShaderCompatibilityUtility.EnsureValidMaterial(canopyLower.GetComponent<Renderer>(), TreeCanopyColor, 0.18f);

                GameObject canopyTop = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                canopyTop.name = "CanopyTop";
                canopyTop.transform.SetParent(tree.transform, false);
                canopyTop.transform.localPosition = new Vector3(0f, 4.3f, 0f);
                canopyTop.transform.localScale = new Vector3(1.9f, 1.6f, 1.9f);
                ShaderCompatibilityUtility.EnsureValidMaterial(canopyTop.GetComponent<Renderer>(), TreeCanopyColor, 0.18f);
            }
        }

        private static void EnsureFlowerPatches(Transform root)
        {
            Transform flowersRoot = root.Find("FlowerPatches");
            if (flowersRoot != null)
            {
                return;
            }

            flowersRoot = new GameObject("FlowerPatches").transform;
            flowersRoot.SetParent(root, false);

            Vector3[] patchCenters =
            {
                new Vector3(-5.5f, 0f, 4.5f),
                new Vector3(5.5f, 0f, 4.5f),
                new Vector3(-5.5f, 0f, -4.5f),
                new Vector3(5.5f, 0f, -4.5f),
            };

            for (int p = 0; p < patchCenters.Length; p++)
            {
                GameObject patch = new GameObject($"FlowerPatch_{p + 1}");
                patch.transform.SetParent(flowersRoot, false);
                patch.transform.localPosition = patchCenters[p];

                for (int f = 0; f < 5; f++)
                {
                    float angle = f * Mathf.PI * 2f / 5f;
                    Vector3 offset = new Vector3(Mathf.Cos(angle) * 0.8f, 0.15f, Mathf.Sin(angle) * 0.8f);
                    GameObject blossom = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    blossom.name = $"Blossom_{f + 1}";
                    blossom.transform.SetParent(patch.transform, false);
                    blossom.transform.localPosition = offset;
                    blossom.transform.localScale = new Vector3(0.32f, 0.25f, 0.32f);
                    Collider col = blossom.GetComponent<Collider>();
                    if (col != null)
                    {
                        col.enabled = false; // Çiçekler oyuncunun yürümesini engellemez
                    }

                    Color flowerColor = (f % 2 == 0) ? FlowerCoralColor : FlowerYellowColor;
                    ShaderCompatibilityUtility.EnsureValidMaterial(blossom.GetComponent<Renderer>(), flowerColor, 0.3f);
                }
            }
        }

        private static void EnsureBoundaryFences(Transform root)
        {
            Transform boundsRoot = root.Find("GardenBoundary");
            if (boundsRoot != null)
            {
                return;
            }

            boundsRoot = new GameObject("GardenBoundary").transform;
            boundsRoot.SetParent(root, false);

            CreateBoundaryWall(boundsRoot, "Boundary_North", new Vector3(0f, 0.6f, 28f), new Vector3(56f, 1.2f, 0.4f));
            CreateBoundaryWall(boundsRoot, "Boundary_South", new Vector3(0f, 0.6f, -28f), new Vector3(56f, 1.2f, 0.4f));
            CreateBoundaryWall(boundsRoot, "Boundary_East", new Vector3(28f, 0.6f, 0f), new Vector3(0.4f, 1.2f, 56f));
            CreateBoundaryWall(boundsRoot, "Boundary_West", new Vector3(-28f, 0.6f, 0f), new Vector3(0.4f, 1.2f, 56f));
        }

        private static void CreateBoundaryWall(Transform parent, string name, Vector3 pos, Vector3 scale)
        {
            GameObject fence = GameObject.CreatePrimitive(PrimitiveType.Cube);
            fence.name = name;
            fence.transform.SetParent(parent, false);
            fence.transform.localPosition = pos;
            fence.transform.localScale = scale;
            ShaderCompatibilityUtility.EnsureValidMaterial(fence.GetComponent<Renderer>(), BenchWoodColor, 0.2f);
        }

        private static void RepairChildrenMaterials(Transform parent, Color color, float smoothness)
        {
            Renderer[] renderers = parent.GetComponentsInChildren<Renderer>(true);
            foreach (Renderer r in renderers)
            {
                ShaderCompatibilityUtility.EnsureValidMaterial(r, color, smoothness);
            }
        }
    }
}
