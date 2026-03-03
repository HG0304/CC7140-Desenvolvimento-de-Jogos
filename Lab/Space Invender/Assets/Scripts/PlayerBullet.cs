using UnityEngine;

// Tiro do jogador - sobe no eixo Y
public class PlayerBullet : MonoBehaviour
{
    public float speed = 14f;
    public float lifetime = 3f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.Translate(Vector2.up * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("MotherShip"))
        {
            // A pontuação/morte é tratada no Enemy/MotherShip via OnTriggerEnter2D deles
            Destroy(gameObject);
        }
        if (other.CompareTag("EnemyBullet"))
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
        if (other.CompareTag("Border"))
        {
            Destroy(gameObject);
        }
    }
}
