using UnityEngine;

// Tiro dos inimigos - desce no eixo Y
public class EnemyBullet : MonoBehaviour
{
    public float speed = 6f;
    public float lifetime = 5f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.Translate(Vector2.down * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Border"))
            Destroy(gameObject);
    }
}
