using UnityEngine;

public class MotherShip : MonoBehaviour
{
    private float speed = 3.5f;
    private int scoreValue = 50;
    private float minInterval = 25f;
    private float maxInterval = 45f;
    private float spawnX = -11f;
    private float exitX = 11f;
    private float yPos = 4f;

    private bool active = false;
    private float spawnTimer;
    private SpriteRenderer sr;
    private Collider2D col;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        Deactivate();
        ScheduleNext();
    }

    private void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.gameOver) return;

        if (!active)
        {
            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0f) Activate();
            return;
        }

        transform.position += Vector3.right * speed * Time.deltaTime;
        if (transform.position.x >= exitX) { Deactivate(); ScheduleNext(); }
    }

    private void Activate()
    {
        active = true;
        transform.position = new Vector3(spawnX, yPos, 0f);
        sr.enabled = true;
        col.enabled = true;
    }

    private void Deactivate()
    {
        active = false;
        sr.enabled = false;
        col.enabled = false;
    }

    private void ScheduleNext() => spawnTimer = Random.Range(minInterval, maxInterval);

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerBullet"))
        {
            Destroy(other.gameObject);
            GameManager.Instance?.AddScore(scoreValue);
            Deactivate();
            ScheduleNext();
        }
    }
}
