using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace QuickOutline
{
    [DisallowMultipleComponent]
    public class Outline : MonoBehaviour
    {
        private static readonly HashSet<Mesh> registeredMeshes = new();
        private static readonly Dictionary<Mesh, List<Vector3>> cachedSmoothNormals = new();

        public enum Mode
        {
            OutlineAll,
            OutlineVisible,
            OutlineHidden,
            OutlineAndSilhouette,
            SilhouetteOnly
        }

        [SerializeField]
        private Mode outlineMode;

        [SerializeField]
        private Color outlineColor = Color.white;

        [SerializeField, Range(0f, 10f)]
        private float outlineWidth = 2f;

        [SerializeField]
        private bool precomputeOutline = false;

        [Header("Optional")]
        public Renderer customMesh;

        private Renderer[] renderers;
        private Material outlineMaskMaterial;
        private Material outlineFillMaterial;
        private bool needsUpdate;

        private static Material sharedMaskMaterial;
        private static Material sharedFillMaterial;

        public Mode OutlineMode
        {
            get => outlineMode;
            set { outlineMode = value; needsUpdate = true; }
        }

        public Color OutlineColor
        {
            get => outlineColor;
            set { outlineColor = value; needsUpdate = true; }
        }

        public float OutlineWidth
        {
            get => outlineWidth;
            set { outlineWidth = value; needsUpdate = true; }
        }

        private void Awake()
        {
            if (customMesh)
                renderers = new[] { customMesh };
            else
                renderers = GetComponentsInChildren<Renderer>();

            if (sharedMaskMaterial == null)
                sharedMaskMaterial = Resources.Load<Material>("Materials/OutlineMask");

            if (sharedFillMaterial == null)
                sharedFillMaterial = Resources.Load<Material>("Materials/OutlineFill");

            outlineMaskMaterial = Instantiate(sharedMaskMaterial);
            outlineFillMaterial = Instantiate(sharedFillMaterial);

            outlineMaskMaterial.name = "OutlineMask (Instance)";
            outlineFillMaterial.name = "OutlineFill (Instance)";

            LoadSmoothNormals();

            needsUpdate = true;
        }

        private void OnEnable()
        {
            foreach (var renderer in renderers)
            {
                var materials = renderer.sharedMaterials.ToList();
                materials.Add(outlineMaskMaterial);
                materials.Add(outlineFillMaterial);
                renderer.materials = materials.ToArray();
            }

            UpdateMaterialProperties();
        }

        private void OnDisable()
        {
            foreach (var renderer in renderers)
            {
                var materials = renderer.sharedMaterials.ToList();
                materials.Remove(outlineMaskMaterial);
                materials.Remove(outlineFillMaterial);
                renderer.materials = materials.ToArray();
            }
        }

        private void OnDestroy()
        {
            Destroy(outlineMaskMaterial);
            Destroy(outlineFillMaterial);
        }

        private void OnValidate()
        {
            needsUpdate = true;
        }

        private void Update()
        {
            if (needsUpdate)
            {
                needsUpdate = false;
                UpdateMaterialProperties();
            }
        }

        private void LoadSmoothNormals()
        {
            foreach (var meshFilter in GetComponentsInChildren<MeshFilter>())
            {
                Mesh mesh = meshFilter.sharedMesh;
                if (!registeredMeshes.Add(mesh)) continue;

                if (!cachedSmoothNormals.TryGetValue(mesh, out var smoothNormals))
                {
                    smoothNormals = GenerateSmoothNormals(mesh);
                    cachedSmoothNormals.Add(mesh, smoothNormals);
                }

                mesh.SetUVs(3, smoothNormals);
            }
        }

        private List<Vector3> GenerateSmoothNormals(Mesh mesh)
        {
            var vertices = mesh.vertices;
            var normals = mesh.normals;
            var smoothNormals = new List<Vector3>(normals);
            var groups = new Dictionary<Vector3, List<int>>();

            for (int i = 0; i < vertices.Length; i++)
            {
                var key = Quantize(vertices[i]);
                if (!groups.TryGetValue(key, out var list))
                    groups[key] = list = new List<int>();

                list.Add(i);
            }

            foreach (var group in groups.Values)
            {
                if (group.Count < 2) continue;

                Vector3 average = Vector3.zero;
                foreach (var index in group)
                    average += normals[index];

                average.Normalize();

                foreach (var index in group)
                    smoothNormals[index] = average;
            }

            return smoothNormals;
        }

        private Vector3 Quantize(Vector3 v, float tolerance = 1e-4f)
        {
            return new Vector3(
                Mathf.Round(v.x / tolerance) * tolerance,
                Mathf.Round(v.y / tolerance) * tolerance,
                Mathf.Round(v.z / tolerance) * tolerance
            );
        }

        private void UpdateMaterialProperties()
        {
            outlineFillMaterial.SetColor("_OutlineColor", outlineColor);

            switch (outlineMode)
            {
                case Mode.OutlineAll:
                    outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
                    outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
                    break;
                case Mode.OutlineVisible:
                    outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
                    outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
                    break;
                case Mode.OutlineHidden:
                    outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
                    outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Greater);
                    break;
                case Mode.OutlineAndSilhouette:
                    outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
                    outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
                    break;
                case Mode.SilhouetteOnly:
                    outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
                    outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Greater);
                    break;
            }

            outlineFillMaterial.SetFloat("_OutlineWidth", outlineWidth);
        }
    }
}
