using UnityEngine;
using System.Collections;

namespace AndysUnityFramework
{
    public class ScrollingTextures : MonoBehaviour
    {
        public enum TextureType
        {
            MainAlbedo,
            DetailAlbedo,
            Both
        }

        [Header("Configuration")]
        [SerializeField]
        [Tooltip("The texture to scroll")]
        TextureType _textureType = TextureType.DetailAlbedo;
        [SerializeField]
        [Tooltip("The material to scroll, if mesh on this object contains multiple materials")]
        Material _scrollingMaterial;
        [SerializeField]
        [Tooltip("Should just this object scroll, or all objects with this material?")]
        bool _scrollSharedMaterial = false;

        [Space(10)]
        [SerializeField]
        [Tooltip("In UV units per second")]
        float _scrollSpeed = 1.0f;
        [SerializeField]
        Vector2 _scrollDirection = Vector2.one;

        // References
        Renderer _renderer;
        Material _rendererMaterial;

        public void Awake()
        {
            _renderer = GetComponent<Renderer>();

            // Ensure we have valid references
            if (_renderer == null)
            {
                Debug.LogErrorFormat(this, "Can't find any attached renderers on this object. Either attach one, or move the script to the GameObject with one.");

                enabled = false;
                return;
            }

            // Ensure user has assigned the material to scroll
            if (_scrollingMaterial == null)
            {
                Debug.LogErrorFormat(this, "Please assign the material to scroll");

                enabled = false;
                return;
            }
        }

        void Start()
        {
            // Find the assigned material to scroll, from the renderer
            Material[] rendererMaterials = _renderer.sharedMaterials;
            for (int i = 0; i < rendererMaterials.Length; ++i)
            {
                Material material = rendererMaterials[i];

                if (material == _scrollingMaterial)
                {
                    _rendererMaterial = (_scrollSharedMaterial) ? _renderer.sharedMaterials[i] : _renderer.materials[i];
                }
            }

            // Ensure the assigned material to scroll is used within the renderer
            if (_rendererMaterial == null)
            {
                Debug.LogErrorFormat(this, "Can't find scrolling material on the renderer. Please ensure you assign the correct material to scroll in the inspector.");

                enabled = false;
                return;
            }

            // Ensure the scrolling material is using the Standard shader
            if (_rendererMaterial.shader.name != "Standard" &&
                _rendererMaterial.shader.name != "Standard (Specular setup)")
            {
                Debug.LogErrorFormat(this, "No support for this shader. Please use the Standard shader, or the specular setup Standard shader");

                enabled = false;
                return;
            }

            StartCoroutine(ScrollTexture());
        }

        public void OnEnable()
        {
            StartCoroutine(ScrollTexture());
        }

        public void OnDisable()
        {
            StopCoroutine(ScrollTexture());
        }

        IEnumerator ScrollTexture()
        {
            // Preserve initial texture offset
            Vector2 mainTexOffset   = _rendererMaterial.GetTextureOffset("_MainTex");
            Vector2 detailTexOffset = _rendererMaterial.GetTextureOffset("_DetailAlbedoMap");

            while (true)
            {
                // Calculate new texture offset
                mainTexOffset   += _scrollDirection.normalized * (_scrollSpeed * Time.deltaTime);
                detailTexOffset += _scrollDirection.normalized * (_scrollSpeed * Time.deltaTime);

                // Apply new offset to the selected texture
                switch (_textureType)
                {
                    case TextureType.MainAlbedo:
                        _rendererMaterial.SetTextureOffset("_MainTex", detailTexOffset);
                        break;

                    case TextureType.DetailAlbedo:
                        _rendererMaterial.SetTextureOffset("_DetailAlbedoMap", detailTexOffset);
                        break;

                    case TextureType.Both:
                        _rendererMaterial.SetTextureOffset("_MainTex", detailTexOffset);
                        _rendererMaterial.SetTextureOffset("_DetailAlbedoMap", detailTexOffset);
                        break;
                }

                yield return null;
            }
        }
    }
}
