using UnityEngine;

/// <summary>
/// Loader de sprites seguindo o slicing recomendado nos slides:
/// Grid by Cell Size
///   Cell Size: 102 x 80
///   Offset:    5 x 25
///   Padding:   8 x 40
///
/// O atlas possui 26 blocos (a..z) em uma grade de 9 colunas.
/// Este script extrai o bloco por letra, remove o fundo e retorna o sprite final.
/// </summary>
public static class SpriteLoader
{
    private static Texture2D _texture;

    private const int CellWidth = 102;
    private const int CellHeight = 80;
    private const int OffsetX = 5;
    private const int OffsetYTop = 25;
    private const int PaddingX = 8;
    private const int PaddingY = 40;
    private const int Columns = 9;

    private static Texture2D GetTexture()
    {
        if (_texture != null) return _texture;
        _texture = Resources.Load<Texture2D>("SpaceInvaders");
        return _texture;
    }

    private static bool IsNear(Color a, Color b, float tolerance)
    {
        return Mathf.Abs(a.r - b.r) <= tolerance
            && Mathf.Abs(a.g - b.g) <= tolerance
            && Mathf.Abs(a.b - b.b) <= tolerance;
    }

    private static Color DetectCellBackground(Color[] cellPixels, int w, int h)
    {
        System.Collections.Generic.Dictionary<int, int> counts = new System.Collections.Generic.Dictionary<int, int>();

        void Count(Color c)
        {
            int r = Mathf.RoundToInt(c.r * 15f);
            int g = Mathf.RoundToInt(c.g * 15f);
            int b = Mathf.RoundToInt(c.b * 15f);
            int key = (r << 8) | (g << 4) | b;
            if (counts.ContainsKey(key)) counts[key]++;
            else counts[key] = 1;
        }

        // Usa bordas da célula para detectar fundo
        for (int x = 0; x < w; x++)
        {
            Count(cellPixels[x]);
            Count(cellPixels[(h - 1) * w + x]);
        }
        for (int y = 1; y < h - 1; y++)
        {
            Count(cellPixels[y * w]);
            Count(cellPixels[y * w + (w - 1)]);
        }

        int bestKey = 0;
        int bestCount = -1;
        foreach (var kv in counts)
        {
            if (kv.Value > bestCount)
            {
                bestCount = kv.Value;
                bestKey = kv.Key;
            }
        }

        float rr = ((bestKey >> 8) & 0xF) / 15f;
        float gg = ((bestKey >> 4) & 0xF) / 15f;
        float bb = (bestKey & 0xF) / 15f;
        return new Color(rr, gg, bb, 1f);
    }

    private static Sprite BuildFromLetter(char letter, float pixelsPerUnit)
    {
        Texture2D source = GetTexture();
        if (source == null) return null;

        int index = Mathf.Clamp(char.ToLowerInvariant(letter) - 'a', 0, 25);
        int col = index % Columns;
        int row = index / Columns;

        int sx = OffsetX + col * (CellWidth + PaddingX);
        int syTop = OffsetYTop + row * (CellHeight + PaddingY);
        int sy = source.height - syTop - CellHeight;

        if (sx < 0 || sy < 0 || sx + CellWidth > source.width || sy + CellHeight > source.height)
            return null;

        Color[] cell = source.GetPixels(sx, sy, CellWidth, CellHeight);
        Color bg = DetectCellBackground(cell, CellWidth, CellHeight);

        int minX = CellWidth;
        int maxX = -1;
        int minY = CellHeight;
        int maxY = -1;

        for (int y = 0; y < CellHeight; y++)
        {
            for (int x = 0; x < CellWidth; x++)
            {
                Color p = cell[y * CellWidth + x];
                bool foreground = p.a > 0.05f && !IsNear(p, bg, 0.09f);
                if (foreground)
                {
                    if (x < minX) minX = x;
                    if (x > maxX) maxX = x;
                    if (y < minY) minY = y;
                    if (y > maxY) maxY = y;
                }
            }
        }

        if (maxX < minX || maxY < minY)
            return null;

        int w = Mathf.Max(1, maxX - minX + 1);
        int h = Mathf.Max(1, maxY - minY + 1);
        Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Point;

        Color[] dst = new Color[w * h];
        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                Color p = cell[(minY + y) * CellWidth + (minX + x)];
                bool foreground = p.a > 0.05f && !IsNear(p, bg, 0.09f);
                dst[y * w + x] = foreground ? new Color(p.r, p.g, p.b, 1f) : Color.clear;
            }
        }

        tex.SetPixels(dst);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f), pixelsPerUnit);
    }

    private static Sprite CreateFallbackAlien(Color bodyColor, int type)
    {
        int w = 24, h = 20;
        Texture2D tex = new Texture2D(w, h);
        tex.filterMode = FilterMode.Point;
        Color[] pixels = new Color[w * h];
        Color t = Color.clear;

        for (int i = 0; i < pixels.Length; i++) pixels[i] = t;

        int[,] pattern;
        if (type == 1)
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
        else if (type == 2)
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
        else
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

    public static Sprite CreateAlienSprite(Color bodyColor, int type)
    {
        if (type == 1)
            return CreateFallbackAlien(bodyColor, type);

        char letter = type == 1 ? 'a' : type == 2 ? 'b' : 'c';
        Sprite fromAtlas = BuildFromLetter(letter, 16f);
        return fromAtlas != null ? fromAtlas : CreateFallbackAlien(bodyColor, type);
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
        Sprite fromAtlas = BuildFromLetter('z', 16f);
        if (fromAtlas != null) return fromAtlas;

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
