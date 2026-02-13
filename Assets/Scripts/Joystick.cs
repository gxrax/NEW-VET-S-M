using UnityEngine;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    [Header("Joystick UI Elements")]
    public RectTransform joystickBackground; // Background objesi
    public RectTransform joystickHandle;     // Handle objesi

    private Vector2 inputVector;

    // Dokunmaya basınca
    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    // Dokunmayı bıraktığında
    public void OnPointerUp(PointerEventData eventData)
    {
        inputVector = Vector2.zero;
        joystickHandle.anchoredPosition = Vector2.zero;
    }

    // Sürükleme hareketi
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(joystickBackground, eventData.position, eventData.pressEventCamera, out pos))
        {
            // pozisyonu normalize et (-1,1 arası)
            pos.x = (pos.x / joystickBackground.sizeDelta.x) * 2;
            pos.y = (pos.y / joystickBackground.sizeDelta.y) * 2;

            inputVector = new Vector2(pos.x, pos.y);
            if (inputVector.magnitude > 1.0f)
                inputVector = inputVector.normalized;

            // Handle’ı hareket ettir
            joystickHandle.anchoredPosition = new Vector2(
                inputVector.x * (joystickBackground.sizeDelta.x / 2),
                inputVector.y * (joystickBackground.sizeDelta.y / 2)
            );
        }
    }

    // Yatay yön
    public float Horizontal()
    {
        return inputVector.x;
    }

    // Dikey yön
    public float Vertical()
    {
        return inputVector.y;
    }

    // 2D yön
    public Vector2 Direction()
    {
        return new Vector2(Horizontal(), Vertical());
    }
}