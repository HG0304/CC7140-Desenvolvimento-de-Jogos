using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

/// <summary>
/// Monta a cena inicial (menu) automaticamente.
/// Anexe este script a um GameObject vazio na cena Start.
/// </summary>
public class StartScreen : MonoBehaviour
{
    private void Start()
    {
        Application.targetFrameRate = 60;
        EnsureEventSystem();

        if (GameManager.Instance == null)
        {
            GameObject gm = new GameObject("GameManager");
            gm.AddComponent<GameManager>();
        }

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

        GameObject canvasGo = new GameObject("Canvas");
        Canvas canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280, 720);

        canvasGo.AddComponent<GraphicRaycaster>();

        CreateText(canvasGo, "SPACE INVADERS", 72, Color.white, new Vector2(0, 140));
        CreateText(canvasGo, "Setas/A-D para mover", 28, new Color(0.7f, 0.9f, 1f), new Vector2(0, 20));
        CreateText(canvasGo, "Espaco para atirar", 28, new Color(0.7f, 0.9f, 1f), new Vector2(0, -20));

        CreateButton(canvasGo, "START GAME", new Vector2(0, -140), () =>
        {
            if (GameManager.Instance != null)
                GameManager.Instance.StartNewGame();
            else
                SceneManager.LoadScene("SampleScene");
        });
    }

    private void EnsureEventSystem()
    {
        if (FindFirstObjectByType<EventSystem>() != null) return;

        GameObject es = new GameObject("EventSystem");
        es.AddComponent<EventSystem>();
#if ENABLE_INPUT_SYSTEM
        es.AddComponent<InputSystemUIInputModule>();
#else
        es.AddComponent<StandaloneInputModule>();
#endif
    }

    private void CreateText(GameObject parent, string text, int size, Color color, Vector2 anchoredPos)
    {
        GameObject go = new GameObject("Text_" + text.Substring(0, Mathf.Min(5, text.Length)));
        go.transform.SetParent(parent.transform, false);

        TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = size;
        tmp.color = color;
        tmp.alignment = TextAlignmentOptions.Center;

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = new Vector2(900, 100);
    }

    private void CreateButton(GameObject parent, string label, Vector2 anchoredPos, UnityEngine.Events.UnityAction onClick)
    {
        GameObject go = new GameObject("Button_" + label);
        go.transform.SetParent(parent.transform, false);

        Image img = go.AddComponent<Image>();
        img.color = new Color(0.12f, 0.35f, 0.12f);

        Button btn = go.AddComponent<Button>();
        btn.targetGraphic = img;
        btn.onClick.AddListener(onClick);

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = new Vector2(300, 70);

        GameObject textGo = new GameObject("Label");
        textGo.transform.SetParent(go.transform, false);

        TextMeshProUGUI txt = textGo.AddComponent<TextMeshProUGUI>();
        txt.text = label;
        txt.fontSize = 30;
        txt.color = Color.white;
        txt.alignment = TextAlignmentOptions.Center;

        RectTransform trt = textGo.GetComponent<RectTransform>();
        trt.anchorMin = Vector2.zero;
        trt.anchorMax = Vector2.one;
        trt.offsetMin = trt.offsetMax = Vector2.zero;
    }
}
