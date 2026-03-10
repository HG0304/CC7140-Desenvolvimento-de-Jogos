using UnityEngine;
using System.Collections.Generic;

public class EnemyFormation : MonoBehaviour
{
    [HideInInspector] public GameObject enemyBulletPrefab;

    private int columns = 11;
    private int rows = 5;
    private float spacingX = 1.3f;
    private float spacingY = 1.1f;
    private float baseSpeed = 1.15f;
    private float speedIncreasePerKill = 0.02f;
    private float edgeMargin = 7.2f;
    private float stepDown = 0.28f;

    private float currentSpeed;
    private int direction = 1;
    private bool needsStep = false;
    private float stepTimer = 0f;
    private const float stepDuration = 0.18f;
    private bool victoryTriggered = false;

    private List<Enemy> enemies = new List<Enemy>();
    private int totalEnemies;

    public void Init()
    {
        currentSpeed = baseSpeed;
        SpawnEnemies();
        totalEnemies = enemies.Count;
    }

    private void SpawnEnemies()
    {
        // Pontuações por linha: linha0=30, linha1-2=20, linha3-4=10
        int[] scores = { 30, 20, 20, 10, 10 };
        Color[] colors = {
            new Color(1f, 0.4f, 0.8f),   // rosa - topo 30pts
            new Color(0.4f, 0.8f, 1f),   // azul - 20pts
            new Color(0.4f, 0.8f, 1f),
            new Color(0.6f, 1f, 0.4f),   // verde - 10pts
            new Color(0.6f, 1f, 0.4f)
        };
        int[] types = { 1, 2, 2, 3, 3 };

        float startX = -(columns - 1) * spacingX / 2f;
        float startY = (rows - 1) * spacingY / 2f;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                Vector3 pos = transform.position + new Vector3(
                    startX + col * spacingX,
                    startY - row * spacingY, 0f);

                GameObject go = new GameObject("Enemy_" + row + "_" + col);
                go.transform.SetParent(transform);
                go.transform.position = pos;
                go.tag = "Enemy";
                go.layer = LayerMask.NameToLayer("Default");

                SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = SpriteLoader.CreateAlienSprite(colors[row], types[row]);
                sr.sortingOrder = 1;

                Rigidbody2D rb = go.AddComponent<Rigidbody2D>();
                rb.bodyType = RigidbodyType2D.Kinematic;
                rb.gravityScale = 0f;

                BoxCollider2D col2d = go.AddComponent<BoxCollider2D>();
                col2d.isTrigger = true;
                col2d.size = new Vector2(0.9f, 0.9f);

                Enemy e = go.AddComponent<Enemy>();
                e.scoreValue = scores[row];
                e.enemyBulletPrefab = enemyBulletPrefab;
                enemies.Add(e);
            }
        }
    }

    private void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.gameOver) return;

        if (!victoryTriggered && GetAliveEnemiesCount() == 0)
        {
            victoryTriggered = true;
            GameManager.Instance?.TriggerVictory();
            return;
        }

        if (needsStep)
        {
            stepTimer += Time.deltaTime;
            transform.position += Vector3.down * stepDown * (Time.deltaTime / stepDuration);
            if (stepTimer >= stepDuration)
            {
                stepTimer = 0f;
                needsStep = false;
                direction *= -1;
            }
            return;
        }

        transform.position += Vector3.right * direction * currentSpeed * Time.deltaTime;

        float halfWidth = (columns - 1) * spacingX / 2f;
        float fx = transform.position.x;

        if (direction == 1 && fx + halfWidth >= edgeMargin)
            needsStep = true;
        else if (direction == -1 && fx - halfWidth <= -edgeMargin)
            needsStep = true;
    }

    public void OnEnemyDestroyed()
    {
        enemies.RemoveAll(e => e == null);
        int killed = totalEnemies - enemies.Count;
        currentSpeed = baseSpeed + speedIncreasePerKill * killed;

        if (!victoryTriggered && GetAliveEnemiesCount() == 0)
        {
            victoryTriggered = true;
            GameManager.Instance?.TriggerVictory();
        }
    }

    private int GetAliveEnemiesCount()
    {
        enemies.RemoveAll(e => e == null);
        int byList = enemies.Count;
        int byHierarchy = GetComponentsInChildren<Enemy>(false).Length;
        return Mathf.Max(byList, byHierarchy);
    }
}
