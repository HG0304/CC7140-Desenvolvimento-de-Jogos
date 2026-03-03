using UnityEngine;

/// <summary>
/// Carrega sprites do SpaceInvaders.png em Runtime usando apenas a API da Unity.
/// O PNG está em Assets/SpaceInvaders.png e é carregado via Resources ou WWW/UnityWebRequest.
/// Como não podemos garantir que está em Resources/, usamos Texture2D com LoadImage.
/// </summary>
public static class SpriteLoader
{
    // Retorna uma textura sólida de cor pura como fallback quando o PNG não está acessível
    public static Sprite CreateColorSprite(Color color, int width = 32, int height = 32)
    {
        Texture2D tex = new Texture2D(width, height);
        Color[] pixels = new Color[width * height];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = color;
        tex.SetPixels(pixels);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 32f);
    }

    // Cria sprite de um bloco colorido com uma forma simples (alien)
    public static Sprite CreateAlienSprite(Color bodyColor, int type)
    {
        int w = 24, h = 20;
        Texture2D tex = new Texture2D(w, h);
        tex.filterMode = FilterMode.Point;
        Color[] pixels = new Color[w * h];
        Color t = Color.clear;

        // Preenche transparente
        for (int i = 0; i < pixels.Length; i++) pixels[i] = t;

        // Padrão de pixel art para cada tipo
        int[,] pattern;
        if (type == 1) // Calamar - topo (30pts)
        {
            pattern = new int[,] {
                {0,0,0,1,1,0,0,0,0,0,1,1,0,0,0},
                {0,0,1,1,1,1,0,0,0,1,1,1,1,0,0},
                {0,1,1,1,1,1,1,0,1,1,1,1,1,1,0},
                {1,1,0,1,1,0,1,1,1,0,1,1,0,1,1},
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
                {0,1,1,1,1,1,1,1,1,1,1,1,1,1,0},
                {0,0,1,0,0,0,1,1,1,0,0,0,1,0,0},
                {0,1,0,1,0,0,0,0,0,0,0,1,0,1,0},
            };
        }
        else if (type == 2) // Caranguejo - meio (20pts)
        {
            pattern = new int[,] {
                {0,0,0,0,1,0,0,0,0,0,1,0,0,0,0},
                {0,0,0,1,1,1,0,0,0,1,1,1,0,0,0},
                {0,0,1,1,1,1,1,0,1,1,1,1,1,0,0},
                {0,1,1,0,1,1,0,0,0,1,1,0,1,1,0},
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
                {1,0,1,1,1,1,1,1,1,1,1,1,1,0,1},
                {1,0,1,0,0,0,0,0,0,0,0,0,1,0,1},
                {0,0,0,1,1,0,0,0,0,0,1,1,0,0,0},
            };
        }
        else // Cogumelo - baixo (10pts)
        {
            pattern = new int[,] {
                {0,0,0,1,1,1,1,1,1,1,1,1,0,0,0},
                {0,1,1,1,1,1,1,1,1,1,1,1,1,1,0},
                {1,1,1,0,0,1,1,1,1,1,0,0,1,1,1},
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
                {0,0,1,1,1,0,0,0,0,0,1,1,1,0,0},
                {0,1,1,0,0,1,0,0,0,1,0,0,1,1,0},
                {1,1,0,0,0,0,0,0,0,0,0,0,0,1,1},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            };
        }

        int rows = pattern.GetLength(0);
        int cols = pattern.GetLength(1);
        int offX = (w - cols) / 2;
        int offY = (h - rows) / 2;

        for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++)
                if (pattern[r, c] == 1)
                    pixels[(offY + (rows - 1 - r)) * w + (offX + c)] = bodyColor;

        tex.SetPixels(pixels);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f), 16f);
    }

    public static Sprite CreatePlayerSprite()
    {
        int w = 24, h = 16;
        Texture2D tex = new Texture2D(w, h);
        tex.filterMode = FilterMode.Point;
        Color[] pixels = new Color[w * h];
        Color t = Color.clear;
        Color c = new Color(0.2f, 1f, 0.2f);

        for (int i = 0; i < pixels.Length; i++) pixels[i] = t;

        int[,] pattern = {
            {0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0},
            {0,1,1,0,0,0,1,1,1,1,1,1,1,1,1,1,0,0,0,1,1,0,0,0},
            {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0},
            {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0},
        };

        int rows = pattern.GetLength(0);
        int cols = pattern.GetLength(1);

        for (int r = 0; r < rows; r++)
            for (int cc = 0; cc < cols; cc++)
                if (cc < w && pattern[r, cc] == 1)
                    pixels[(r) * w + cc] = c;

        tex.SetPixels(pixels);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f), 16f);
    }

    public static Sprite CreateBulletSprite(Color color)
    {
        int w = 4, h = 12;
        Texture2D tex = new Texture2D(w, h);
        tex.filterMode = FilterMode.Point;
        Color[] pixels = new Color[w * h];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = color;
        tex.SetPixels(pixels);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f), 16f);
    }

    public static Sprite CreateMotherShipSprite()
    {
        int w = 48, h = 16;
        Texture2D tex = new Texture2D(w, h);
        tex.filterMode = FilterMode.Point;
        Color[] pixels = new Color[w * h];
        Color t = Color.clear;
        Color c = new Color(1f, 0.2f, 0.2f);

        for (int i = 0; i < pixels.Length; i++) pixels[i] = t;

        int[,] pattern = {
            {0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0},
            {0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0},
            {0,0,1,1,1,0,1,1,0,1,1,0,1,1,0,1,1,0,1,1,0,1,1,0,1,1,0,1,1,0,1,1,0,1,1,0,1,1,0,1,1,0,1,1,1,0,0,0},
            {0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0},
        };

        int rows = pattern.GetLength(0);
        int cols = pattern.GetLength(1);

        for (int r = 0; r < rows; r++)
            for (int cc = 0; cc < cols; cc++)
                if (cc < w && pattern[r, cc] == 1)
                    pixels[(h - 1 - r) * w + cc] = c;

        tex.SetPixels(pixels);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f), 16f);
    }
}
