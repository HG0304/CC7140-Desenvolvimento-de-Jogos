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
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null) enemy.Die();
            Destroy(gameObject);
            return;
        }

        if (other.CompareTag("MotherShip"))
        {
            Destroy(gameObject);
            return;
        }

        if (other.CompareTag("EnemyBullet"))
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
            return;
        }

        if (other.CompareTag("Border"))
        {
            Destroy(gameObject);
        }
    }
}
