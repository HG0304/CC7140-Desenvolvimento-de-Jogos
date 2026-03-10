using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

/// <summary>
/// Monta a cena de vitória automaticamente.
/// Anexe este script a um GameObject vazio na cena Victory.
/// </summary>
public class VictoryScreen : MonoBehaviour
{
    private void Start()
    {
        EnsureEventSystem();

        // Camera
        Camera cam = Camera.main;
        if (cam == null)
        {
            GameObject camGo = new GameObject("Main Camera");
            camGo.tag = "MainCamera";
            cam = camGo.AddComponent<Camera>();
        }
        cam.orthographic = true;
        cam.orthographicSize = 6f;
        cam.transform.position = new Vector3(0, 0, -10f);
        cam.backgroundColor = new Color(0f, 0.05f, 0f);
        cam.clearFlags = CameraClearFlags.SolidColor;

        // Canvas
        GameObject canvasGo = new GameObject("Canvas");
        Canvas canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280, 720);
        canvasGo.AddComponent<GraphicRaycaster>();

        // Título
        CreateText(canvasGo, "YOU WIN!", 72, Color.green, new Vector2(0, 100));

        // Score
        int sc = GameManager.Instance != null ? GameManager.Instance.score : 0;
        CreateText(canvasGo, "FINAL SCORE: " + sc.ToString("D6"), 36, Color.white, new Vector2(0, 0));

        // Botão
        CreateButton(canvasGo, "PLAY AGAIN", new Vector2(0, -120), () =>
        {
            GameManager.Instance?.RestartGame();
        });

        CreateButton(canvasGo, "MAIN MENU", new Vector2(0, -200), () =>
        {
            GameManager.Instance?.GoToMenu();
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
        rt.sizeDelta = new Vector2(800, 100);
    }

    private void CreateButton(GameObject parent, string label, Vector2 anchoredPos, UnityEngine.Events.UnityAction onClick)
    {
        GameObject go = new GameObject("Button_" + label);
        go.transform.SetParent(parent.transform, false);

        Image img = go.AddComponent<Image>();
        img.color = new Color(0.1f, 0.5f, 0.1f);

        Button btn = go.AddComponent<Button>();
        btn.targetGraphic = img;
        btn.onClick.AddListener(onClick);

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = new Vector2(260, 60);

        GameObject textGo = new GameObject("Label");
        textGo.transform.SetParent(go.transform, false);
        TextMeshProUGUI txt = textGo.AddComponent<TextMeshProUGUI>();
        txt.text = label;
        txt.fontSize = 28;
        txt.color = Color.white;
        txt.alignment = TextAlignmentOptions.Center;
        RectTransform trt = textGo.GetComponent<RectTransform>();
        trt.anchorMin = Vector2.zero;
        trt.anchorMax = Vector2.one;
        trt.offsetMin = trt.offsetMax = Vector2.zero;
    }
}
