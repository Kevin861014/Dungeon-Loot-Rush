using UnityEngine;
using DungeonLootRush.UI;

namespace DungeonLootRush.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private VirtualJoystick joystick;
        [SerializeField] private float moveSpeed = 4f;

        private Rigidbody2D _rigidbody;

        public Vector2 FacingDirection { get; private set; } = Vector2.down;
        public bool IsMoving { get; private set; }

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            Vector2 input = joystick != null ? joystick.Direction : Vector2.zero;
            _rigidbody.velocity = input * moveSpeed;

            IsMoving = input.sqrMagnitude > 0.01f;
            if (IsMoving)
            {
                FacingDirection = input.normalized;
            }
        }
    }
}
