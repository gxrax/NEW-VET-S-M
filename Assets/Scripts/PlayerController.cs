using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public Joystick joystick; // Inspector'dan joysticki buraya sürükle

    void Update()
    {
        // Joystick yönünü al
        Vector3 move = new Vector3(joystick.Horizontal(), 0, joystick.Vertical());

        // Karakteri hareket ettir
        transform.Translate(move * speed * Time.deltaTime, Space.World);

        // Karakter yönünü joystick'e göre döndür
        if (move != Vector3.zero)
        {
            transform.forward = move;
        }
    }
}