using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CurlNoiseParticle.Demo
{
    public class UnityChanEffect : MonoBehaviour
    {
        [SerializeField]
        private Color _flashColor = Color.white;

        [SerializeField]
        private Renderer[] _renderers;

        [SerializeField]
        private float _duration = 3f;

        [SerializeField]
        private float _stayTime = 2f;

        [SerializeField]
        private float _delay = 1f;

        private bool _isStarted = false;

        private MaterialPropertyBlock _propertyBlock;
        private int _flashWeightId;
        private int _flashColorId;

        private void Awake()
        {
            _propertyBlock = new MaterialPropertyBlock();
            _flashColorId = Shader.PropertyToID("_FlashColor");
            _flashWeightId = Shader.PropertyToID("_FlashWeight");
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                Play();
            }
        }

        public void Play()
        {
            if (_isStarted)
            {
                return;
            }

            _isStarted = true;

            StartCoroutine(PlayAnimation());
        }

        private IEnumerator PlayAnimation()
        {
            float time = 0;

            while (true)
            {
                time += Time.deltaTime;

                if (time >= _duration)
                {
                    Emission(1f);
                    break;
                }

                float t = time / _duration;
                t = t * t;

                Emission(t);

                yield return null;
            }

            time = 0;

            while (true)
            {
                time += Time.deltaTime;

                if (time >= _stayTime)
                {
                    EnableMeshes(false);
                    PlayParticle();
                    break;
                }

                yield return null;
            }

            time = 0;

            while (true)
            {
                time += Time.deltaTime;

                if (time >= 5f)
                {
                    break;
                }

                yield return null;
            }

            _isStarted = false;

            EnableMeshes(true);

            Emission(0);
        }

        private void Emission(float t)
        {
            _propertyBlock.SetFloat(_flashWeightId, t);
            _propertyBlock.SetColor(_flashColorId, _flashColor);

            foreach (var ren in _renderers)
            {
                ren.SetPropertyBlock(_propertyBlock);
            }
        }

        private void EnableMeshes(bool enabled)
        {
            foreach (var ren in _renderers)
            {
                ren.enabled = enabled;
            }
        }

        private void PlayParticle()
        {
            List<Vector3> vertices = new List<Vector3>();

            foreach (var ren in _renderers)
            {
                Mesh mesh = new Mesh();

                if (ren is SkinnedMeshRenderer)
                {
                    (ren as SkinnedMeshRenderer).BakeMesh(mesh);
                }
                else if (ren is MeshRenderer)
                {
                    mesh = ren.GetComponent<MeshFilter>().sharedMesh;
                }

                vertices.AddRange(mesh.vertices);
            }

            CurlParticle particle = CurlParticleSystem.Instance.Get();

            particle.EmitWithVertices(vertices, 2, _delay);
        }
    }
}
