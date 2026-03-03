using UnityEngine;

// Collider invisível na base da tela. Tag: "Base"
// Se um inimigo tocar aqui, o jogador perde.
public class BaseCollider : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            GameManager.Instance?.TriggerDefeat();
        }
    }
}
