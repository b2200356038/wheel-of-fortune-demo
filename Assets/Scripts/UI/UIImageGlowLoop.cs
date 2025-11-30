using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIImageGlowLoop : MonoBehaviour
    {
        [Header("Glow Settings")]
        [SerializeField] private float minAlpha = 0.2f;
        [SerializeField] private float maxAlpha = 0.8f;
        [SerializeField] private float duration = 1.2f;
        [SerializeField] private Ease ease = Ease.InOutSine;

        private Image _image;
        private Tweener _tween;

        private void Awake()
        {
            _image = GetComponent<Image>();
        }

        private void OnEnable()
        {
            if (_image == null) return;
            Color c = _image.color;
            c.a = minAlpha;
            _image.color = c;

            _tween = _image
                .DOFade(maxAlpha, duration)
                .SetEase(ease)
                .SetLoops(-1, LoopType.Yoyo);
        }

        private void OnDisable()
        {
            _tween?.Kill();
        }
    }
}