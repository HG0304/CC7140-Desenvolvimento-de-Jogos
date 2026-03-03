using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int scoreValue = 10;

    // Definido pelo Bootstrapper
    [HideInInspector] public GameObject enemyBulletPrefab;

    private float nextFireTime;
    private EnemyFormation formation;

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
        nextFireTime = Time.time + Random.Range(2f, 7f);
    }

    private void Shoot()
    {
        if (enemyBulletPrefab != null)
        {
            GameObject b = Instantiate(enemyBulletPrefab, transform.position + Vector3.down * 0.3f, Quaternion.identity);
            b.SetActive(true);
        }
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
        GameManager.Instance?.AddScore(scoreValue);
        formation?.OnEnemyDestroyed();
        Destroy(gameObject);
    }
}
