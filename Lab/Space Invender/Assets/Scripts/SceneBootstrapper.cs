using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Este script monta toda a cena do Space Invaders automaticamente.
/// Basta ter este componente em um GameObject vazio na SampleScene e apertar Play.
/// </summary>
public class SceneBootstrapper : MonoBehaviour
{
    private GameObject playerBulletPrefab;
    private GameObject enemyBulletPrefab;

    private void Awake()
    {
        // Garante que as tags existam (Unity ignora AddTag em runtime, mas evita erros)
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        SetupPhysics();
        BuildGameManager();
        BuildPrefabs();
        BuildCamera();
        BuildUI();
        BuildPlayer();
        BuildEnemyFormation();
        BuildMotherShip();
        BuildBase();
        BuildBoundaries();
    }

    // ─── Physics ──────────────────────────────────────────────────────────────
    private void SetupPhysics()
    {
        Physics2D.gravity = Vector2.zero;
    }

    // ─── Prefabs ──────────────────────────────────────────────────────────────
    private void BuildPrefabs()
    {
        if (GameManager.Instance != null)
        {
            playerBulletPrefab = GameManager.Instance.PlayerBulletPrefab;
            enemyBulletPrefab = GameManager.Instance.EnemyBulletPrefab;
            if (playerBulletPrefab != null && enemyBulletPrefab != null)
                return;
        }

        // Tiro do jogador (sobe)
        playerBulletPrefab = new GameObject("PlayerBullet");
        playerBulletPrefab.tag = "PlayerBullet";
        SpriteRenderer psr = playerBulletPrefab.AddComponent<SpriteRenderer>();
        psr.sprite = SpriteLoader.CreateBulletSprite(new Color(0.2f, 1f, 0.2f));
        psr.sortingOrder = 2;
        Rigidbody2D prb = playerBulletPrefab.AddComponent<Rigidbody2D>();
        prb.bodyType = RigidbodyType2D.Kinematic;
        prb.gravityScale = 0f;
        BoxCollider2D pbc = playerBulletPrefab.AddComponent<BoxCollider2D>();
        pbc.isTrigger = true;
        pbc.size = new Vector2(0.1f, 0.5f);
        playerBulletPrefab.AddComponent<PlayerBullet>();
        // Manter como template fora da cena principal
        DontDestroyOnLoad(playerBulletPrefab);
        playerBulletPrefab.SetActive(false);

        // Tiro do inimigo (desce)
        enemyBulletPrefab = new GameObject("EnemyBullet");
        enemyBulletPrefab.tag = "EnemyBullet";
        SpriteRenderer esr = enemyBulletPrefab.AddComponent<SpriteRenderer>();
        esr.sprite = SpriteLoader.CreateBulletSprite(new Color(1f, 0.3f, 0.3f));
        esr.sortingOrder = 2;
        Rigidbody2D erb = enemyBulletPrefab.AddComponent<Rigidbody2D>();
        erb.bodyType = RigidbodyType2D.Kinematic;
        erb.gravityScale = 0f;
        BoxCollider2D ebc = enemyBulletPrefab.AddComponent<BoxCollider2D>();
        ebc.isTrigger = true;
        ebc.size = new Vector2(0.1f, 0.4f);
        enemyBulletPrefab.AddComponent<EnemyBullet>();
        // Manter como template fora da cena principal
        DontDestroyOnLoad(enemyBulletPrefab);
        enemyBulletPrefab.SetActive(false);

        GameManager.Instance?.RegisterBulletPrefabs(playerBulletPrefab, enemyBulletPrefab);
    }

    // ─── Camera ───────────────────────────────────────────────────────────────
    private void BuildCamera()
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            GameObject camGo = new GameObject("Main Camera");
            camGo.tag = "MainCamera";
            cam = camGo.AddComponent<Camera>();
            camGo.AddComponent<AudioListener>();
        }
        cam.orthographic = true;
        cam.orthographicSize = 6f;
        cam.transform.position = new Vector3(0, 0, -10f);
        cam.backgroundColor = Color.black;
        cam.clearFlags = CameraClearFlags.SolidColor;
    }

    // ─── GameManager ──────────────────────────────────────────────────────────
    private void BuildGameManager()
    {
        if (GameManager.Instance != null) return;
        GameObject gm = new GameObject("GameManager");
        gm.AddComponent<GameManager>();
    }

    // ─── UI ───────────────────────────────────────────────────────────────────
    private void BuildUI()
    {
        // Canvas
        GameObject canvasGo = new GameObject("Canvas");
        Canvas canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGo.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasGo.AddComponent<GraphicRaycaster>();

        UIManager uiMgr = canvasGo.AddComponent<UIManager>();

        // Score
        GameObject scoreGo = new GameObject("ScoreText");
        scoreGo.transform.SetParent(canvasGo.transform, false);
        TextMeshProUGUI scoreTxt = scoreGo.AddComponent<TextMeshProUGUI>();
        scoreTxt.text = "SCORE: 000000";
        scoreTxt.fontSize = 22;
        scoreTxt.color = Color.white;
        RectTransform scoreRect = scoreGo.GetComponent<RectTransform>();
        scoreRect.anchorMin = new Vector2(0, 1);
        scoreRect.anchorMax = new Vector2(0, 1);
        scoreRect.pivot = new Vector2(0, 1);
        scoreRect.anchoredPosition = new Vector2(10, -5);
        scoreRect.sizeDelta = new Vector2(300, 40);

        // Lives
        GameObject livesGo = new GameObject("LivesText");
        livesGo.transform.SetParent(canvasGo.transform, false);
        TextMeshProUGUI livesTxt = livesGo.AddComponent<TextMeshProUGUI>();
        livesTxt.text = "LIVES: 3";
        livesTxt.fontSize = 22;
        livesTxt.color = Color.white;
        livesTxt.alignment = TextAlignmentOptions.Right;
        RectTransform livesRect = livesGo.GetComponent<RectTransform>();
        livesRect.anchorMin = new Vector2(1, 1);
        livesRect.anchorMax = new Vector2(1, 1);
        livesRect.pivot = new Vector2(1, 1);
        livesRect.anchoredPosition = new Vector2(-10, -5);
        livesRect.sizeDelta = new Vector2(200, 40);

        uiMgr.SetTexts(scoreTxt, livesTxt);
    }

    // ─── Player ───────────────────────────────────────────────────────────────
    private void BuildPlayer()
    {
        GameObject go = new GameObject("Player");
        go.tag = "Player";
        go.transform.position = new Vector3(0, -4.5f, 0);

        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteLoader.CreatePlayerSprite();
        sr.sortingOrder = 1;

        Rigidbody2D rb = go.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;

        BoxCollider2D bc = go.AddComponent<BoxCollider2D>();
        bc.isTrigger = true;
        bc.size = new Vector2(1.2f, 0.5f);

        Player player = go.AddComponent<Player>();
        player.bulletPrefab = playerBulletPrefab;
    }

    // ─── Enemy Formation ──────────────────────────────────────────────────────
    private void BuildEnemyFormation()
    {
        GameObject go = new GameObject("EnemyFormation");
        go.transform.position = new Vector3(0, 1.5f, 0);

        EnemyFormation formation = go.AddComponent<EnemyFormation>();
        formation.enemyBulletPrefab = enemyBulletPrefab;
        formation.Init();
    }

    // ─── Mother Ship ──────────────────────────────────────────────────────────
    private void BuildMotherShip()
    {
        GameObject go = new GameObject("MotherShip");
        go.tag = "MotherShip";

        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteLoader.CreateMotherShipSprite();
        sr.sortingOrder = 1;

        Rigidbody2D rb = go.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;

        BoxCollider2D bc = go.AddComponent<BoxCollider2D>();
        bc.isTrigger = true;
        bc.size = new Vector2(2.8f, 0.6f);

        go.AddComponent<MotherShip>();
    }

    // ─── Base (derrota se inimigo chegar) ─────────────────────────────────────
    private void BuildBase()
    {
        GameObject go = new GameObject("Base");
        go.tag = "Base";
        go.transform.position = new Vector3(0, -5.2f, 0);

        BoxCollider2D bc = go.AddComponent<BoxCollider2D>();
        bc.isTrigger = true;
        bc.size = new Vector2(20f, 0.2f);

        go.AddComponent<BaseCollider>();
    }

    // ─── Limites laterais (destroem balas que saem da tela) ───────────────────
    private void BuildBoundaries()
    {
        CreateBorder("BorderTop",    new Vector3(0, 6.5f, 0),   new Vector2(20f, 0.2f));
        CreateBorder("BorderLeft",   new Vector3(-10f, 0, 0),   new Vector2(0.2f, 15f));
        CreateBorder("BorderRight",  new Vector3(10f, 0, 0),    new Vector2(0.2f, 15f));
    }

    private void CreateBorder(string name, Vector3 pos, Vector2 size)
    {
        GameObject go = new GameObject(name);
        go.transform.position = pos;
        go.tag = "Border";
        BoxCollider2D bc = go.AddComponent<BoxCollider2D>();
        bc.isTrigger = true;
        bc.size = size;
        go.AddComponent<BorderDestroyer>();
    }
}
