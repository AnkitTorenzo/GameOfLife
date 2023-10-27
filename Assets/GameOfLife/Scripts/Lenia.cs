using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Lenia : MonoBehaviour
{

    [SerializeField] private RawImage img;
    [SerializeField] private RawImage borders;
    [SerializeField] private Vector2Int texSize = Vector2Int.one * 50;

    [SerializeField] private Vector2Int center = Vector2Int.one;
    [SerializeField] int radius = 10;
    [SerializeField] int inRadius = 1;
    private Texture2D texture;

    private int[] currentGeneration;
    private int[] prevGeneration;
    private int length = 0;

    private void Awake()
    {
        length = texSize.x * texSize.y;
        Init();
    }

    private void OnValidate()
    {
        var r = borders.uvRect;
        r.width = texSize.x;
        r.height = texSize.y;
        borders.uvRect = r;
    }

    private void Init()
    {
        texture = new Texture2D(texSize.x, texSize.y);
        texture.filterMode = FilterMode.Point;
        currentGeneration = new int[length];
        prevGeneration = new int[length];
        for (int i = 0; i < length; i++)
        {
            currentGeneration[i] = Random.Range(0f, 1f) <= 0.2f ? 1 : 0;
        }
        SetColorToTexture(currentGeneration);
        Array.Copy(currentGeneration, prevGeneration, length);
    }

    private void SetColorToTexture(int[] currentGeneration)
    {
        int length = currentGeneration.Length;
        Color[] colors = new Color[length];
        for (int i = 0; i < length; i++)
        {
            colors[i] = Vector4.one * currentGeneration[i];
        }
        texture.SetPixels(colors);
        texture.Apply();
        img.texture = texture;

    }

    private void FixedUpdate()
    {
        //CalculateNextGen();
        DrawCircle();
    }

    private void DrawCircle()
    {
        int[] array = new int[length];
        int r = radius;

        do
        {
            DrawCircle(center.x, center.y, r, array);
            r -= 1;
        }
        while (r > inRadius);

        SetColorToTexture(array);

    }

    private void DrawCircle(int centerX, int centerY, int radius, int[] array)
    {
        int d = 3 - (2 * radius);
        int x = 0;
        int y = radius;

        do
        {
            array[GetIndex(centerX + x, centerY + y)] = 1;
            if (centerY - y > 0)
                array[GetIndex(centerX + x, centerY - y)] = 1;

            array[GetIndex(centerX - x, centerY + y)] = 1;

            if (centerY - y > 0)
                array[GetIndex(centerX - x, centerY - y)] = 1;

            array[GetIndex(centerX + y, centerY + x)] = 1;

            if (centerY - x > 0)
                array[GetIndex(centerX + y, centerY - x)] = 1;

            array[GetIndex(centerX - y, centerY + x)] = 1;

            if (centerY - x > 0)
                array[GetIndex(centerX - y, centerY - x)] = 1;

            if (d < 0)
            {
                d = d + (4 * x) + 6;
            }
            else
            {
                d = d + 4 * (x - y) + 10;
                y--;
            }
            x++;
        } while (x <= y);
    }
    private void CalculateNextGen()
    {
        for (int y = 1; y < texSize.y; y++)
        {
            for (int x = 1; x < texSize.x; x++)
            {
                int i = GetIndex(x, y);
                bool isAlive = currentGeneration[i] == 1;
                int sum = 0;

                sum += GetNeighborValue(x, y, 1, 0);
                sum += GetNeighborValue(x, y, -1, 0);
                sum += GetNeighborValue(x, y, 0, 1);
                sum += GetNeighborValue(x, y, 0, -1);
                sum += GetNeighborValue(x, y, 1, 1);
                sum += GetNeighborValue(x, y, 1, -1);
                sum += GetNeighborValue(x, y, -1, 1);
                sum += GetNeighborValue(x, y, -1, -1);

                if (!isAlive && sum == 3)
                    isAlive = true;
                else if (isAlive && (sum < 2 || sum > 3))
                    isAlive = false;

                currentGeneration[i] = isAlive ? 1 : 0;
            }
        }

        SetColorToTexture(currentGeneration);
        Array.Copy(currentGeneration, prevGeneration, length);
    }

    private int GetNeighborValue(int x, int y, int xDir, int yDir)
    {
        int nx = x + xDir;
        int ny = y + yDir;
        if (nx < 0 || ny < 0 || nx >= texSize.x || ny >= texSize.y)
            return 0;

        return prevGeneration[GetIndex(nx, ny)];
    }

    private int GetIndex(int x, int y, bool isDebug = false)
    {
        int index = (y * texSize.x) + x;

        if (isDebug)
            Debug.Log($"{index}, {x}, {y}");

        return index;
    }
}