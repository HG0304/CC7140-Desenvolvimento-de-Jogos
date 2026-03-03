using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    public float speed = 5f;
    public float fireRate = 0.4f;
    public float minX = -8.5f;
    public float maxX = 8.5f;

    private float nextFireTime = 0f;
    private bool isDead = false;
    private Vector3 startPosition;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    // Definido pelo Bootstrapper
    [HideInInspector] public GameObject bulletPrefab;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        startPosition = transform.position;
    }

    private void Update()
    {
        if (isDead || GameManager.Instance == null || GameManager.Instance.gameOver) return;
        HandleMovement();
        HandleShooting();
    }

    private void HandleMovement()
    {
        float input = Input.GetAxisRaw("Horizontal");
        Vector2 newPos = rb.position + Vector2.right * input * speed * Time.deltaTime;
        newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
        rb.MovePosition(newPos);
    }

    private void HandleShooting()
    {
        if (Input.GetKey(KeyCode.Space) && Time.time >= nextFireTime && bulletPrefab != null)
        {
            nextFireTime = Time.time + fireRate;
            Vector3 fp = transform.position + Vector3.up * 0.5f;
            GameObject b = Instantiate(bulletPrefab, fp, Quaternion.identity);
            b.SetActive(true);
        }
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;
        sr.enabled = false;
        GameManager.Instance?.LoseLife();
    }

    public void Respawn()
    {
        isDead = false;
        transform.position = startPosition;
        sr.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("EnemyBullet") || other.CompareTag("Enemy"))
        {
            if (!other.CompareTag("Enemy")) Destroy(other.gameObject);
            Die();
        }
    }
}
