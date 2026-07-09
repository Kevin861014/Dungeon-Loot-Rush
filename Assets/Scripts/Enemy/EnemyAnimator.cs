using UnityEngine;

namespace DungeonLootRush.Enemy
{
    /// Cycles through idle/run sprite frames (e.g. the 0x72 DungeonTilesetII
    /// per-enemy frame sets), switching sets based on EnemyController.IsMoving.
    [RequireComponent(typeof(SpriteRenderer))]
    public class EnemyAnimator : MonoBehaviour
    {
        [SerializeField] private Sprite[] idleFrames;
        [SerializeField] private Sprite[] runFrames;
        [SerializeField] private float framesPerSecond = 8f;

        private SpriteRenderer _spriteRenderer;
        private EnemyController _controller;
        private float _frameTimer;
        private int _frameIndex;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _controller = GetComponentInParent<EnemyController>();
        }

        private void Update()
        {
            var frames = _controller != null && _controller.IsMoving ? runFrames : idleFrames;
            if (frames == null || frames.Length == 0)
            {
                return;
            }

            _frameTimer += Time.deltaTime;
            if (_frameTimer >= 1f / framesPerSecond)
            {
                _frameTimer = 0f;
                _frameIndex = (_frameIndex + 1) % frames.Length;
            }

            _spriteRenderer.sprite = frames[_frameIndex % frames.Length];
            _spriteRenderer.flipX = _controller != null && _controller.FacingDirection.x < 0f;
        }
    }
}
