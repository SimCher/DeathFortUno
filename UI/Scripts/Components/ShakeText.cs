using System.Collections;
using DeathFortUnoCard.Scripts.Common.Interfaces;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DeathFortUnoCard.UI.Scripts.Components
{
    [RequireComponent(typeof(TMP_Text))]
    public class ShakeText : MonoBehaviour, IEnable
    {
        [Header("Настройки")]
        [SerializeField] private float amount = 10f;

        [SerializeField] private float speed = 0.05f;

        private Coroutine _shakeCoroutine;
        private WaitForSeconds _shakeDelay;

        private TMP_Text _textMesh;

        private Vector3[] _originalVertices;

        private bool _isEnabled;

        private void Awake()
        {
            _textMesh = GetComponent<TMP_Text>();
            _shakeDelay = new WaitForSeconds(speed);
        }

        private Vector3 GetVertices(int vertexIndex, int offset)
            => _originalVertices[vertexIndex + offset] + Random.insideUnitSphere * amount;

        private IEnumerator ShakeTextRoutine()
        {
            while (true)
            {
                _textMesh.ForceMeshUpdate();
                var textInfo = _textMesh.textInfo;

                for (int i = 0; i < textInfo.characterCount; i++)
                {
                    var charInfo = textInfo.characterInfo[i];
                    
                    if(!charInfo.isVisible)
                        continue;

                    var vertexIndex = charInfo.vertexIndex;
                    var vertices = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;

                    _originalVertices ??= (Vector3[]) vertices.Clone();

                    vertices[vertexIndex + 0] = GetVertices(vertexIndex, 0);
                    vertices[vertexIndex + 1] = GetVertices(vertexIndex, 1);
                    vertices[vertexIndex + 2] = GetVertices(vertexIndex, 2);
                    vertices[vertexIndex + 3] = GetVertices(vertexIndex, 3);
                }

                for (int i = 0; i < textInfo.meshInfo.Length; i++)
                {
                    textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
                    _textMesh.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
                }

                yield return _shakeDelay;
            }
        }

        [ContextMenu(nameof(Enable))]
        public void Enable()
        {
            if (_isEnabled)
                return;

            _shakeCoroutine = StartCoroutine(ShakeTextRoutine());

            _isEnabled = true;
        }

        [ContextMenu(nameof(Disable))]
        public void Disable()
        {
            if (!_isEnabled)
                return;

            if (_shakeCoroutine != null)
            {
                StopCoroutine(_shakeCoroutine);
                _shakeCoroutine = null;
            }

            _originalVertices = null;

            _isEnabled = false;
        }
    }
}