using UnityEngine;
using UnityEngine.EventSystems;

namespace DungeonLootRush.UI
{
    /// Drag-based on-screen joystick (GDD 4.1): drag within the background
    /// radius, direction is read every frame by PlayerController.
    public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [SerializeField] private RectTransform background;
        [SerializeField] private RectTransform handle;
        [SerializeField] private float handleRange = 100f;

        public Vector2 Direction { get; private set; } = Vector2.zero;

        public void OnPointerDown(PointerEventData eventData)
        {
            OnDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                background, eventData.position, eventData.pressEventCamera, out var localPoint);

            Vector2 clamped = Vector2.ClampMagnitude(localPoint, handleRange);
            handle.anchoredPosition = clamped;
            Direction = clamped / handleRange;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            handle.anchoredPosition = Vector2.zero;
            Direction = Vector2.zero;
        }
    }
}
