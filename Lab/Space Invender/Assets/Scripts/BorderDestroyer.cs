using UnityEngine;

/// <summary>
/// Destroi balas que saem pelos limites da tela.
/// </summary>
public class BorderDestroyer : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerBullet") || other.CompareTag("EnemyBullet"))
            Destroy(other.gameObject);
    }
}
