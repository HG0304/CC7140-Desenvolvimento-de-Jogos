using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int scoreValue = 10;
    private const int MaxEnemyBulletsOnScreen = 5;

    // Definido pelo Bootstrapper
    [HideInInspector] public GameObject enemyBulletPrefab;

    private float nextFireTime;
    private EnemyFormation formation;
    private bool isDead = false;

    private void Start()
    {
        formation = GetComponentInParent<EnemyFormation>();
        ScheduleNextShot();
    }

    private void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.gameOver) return;
        if (Time.time >= nextFireTime)
        {
            Shoot();
            ScheduleNextShot();
        }
    }

    private void ScheduleNextShot()
    {
        nextFireTime = Time.time + Random.Range(3.2f, 7.2f);
    }

    private void Shoot()
    {
        if (!CanShootNow()) return;

        if (enemyBulletPrefab != null)
        {
            GameObject b = Instantiate(enemyBulletPrefab, transform.position + Vector3.down * 0.3f, Quaternion.identity);
            b.SetActive(true);
        }
    }

    private bool CanShootNow()
    {
        // Limita volume de tiros simultâneos
        if (GameObject.FindGameObjectsWithTag("EnemyBullet").Length >= MaxEnemyBulletsOnScreen)
            return false;

        // Apenas naves da "linha da frente" (sem inimigo diretamente abaixo) podem atirar
        Vector2 origin = (Vector2)transform.position + Vector2.down * 0.55f;
        RaycastHit2D[] hits = Physics2D.RaycastAll(origin, Vector2.down, 20f);
        for (int i = 0; i < hits.Length; i++)
        {
            Collider2D c = hits[i].collider;
            if (c == null) continue;
            if (!c.CompareTag("Enemy")) continue;
            if (c.transform == transform) continue;
            return false;
        }

        return true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerBullet"))
        {
            Destroy(other.gameObject);
            Die();
        }
        if (other.CompareTag("Base"))
            GameManager.Instance?.TriggerDefeat();
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;
        GameManager.Instance?.AddScore(scoreValue);
        formation?.OnEnemyDestroyed();
        Destroy(gameObject);
    }
}
