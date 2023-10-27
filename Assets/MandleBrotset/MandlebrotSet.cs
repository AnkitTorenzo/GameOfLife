using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MandlebrotSet : MonoBehaviour
{

    const int MAX_ITTER = 100;
    const int SUDO_INFINITY = int.MaxValue >> 2;

    [Range(0, 1)]
    [SerializeField] private float fractalScale = 1;
    [SerializeField] private Vector2 xBounds = new Vector2(-2.5f, 2.5f);
    [SerializeField] private Vector2 yBounds = new Vector2(-2.5f, 2.5f);

    [Range(-1, 1)]
    [SerializeField] private Vector2 offset;

    [SerializeField] private RawImage image;
    [SerializeField] private float scale = 3f;
    [SerializeField] private Vector2Int tSize;
    Texture2D texture;
    private Color32[] colors;

    private void Start()
    {
        Rect rect = image.GetComponent<RectTransform>().rect;
        tSize = new Vector2Int(Mathf.FloorToInt(rect.width / scale), Mathf.FloorToInt(rect.height / scale));
        texture = new Texture2D(tSize.x, tSize.y);
        texture.filterMode = FilterMode.Point;
        colors = new Color32[tSize.x * tSize.y];
    }

    private void Update()
    {

        Vector2 nbx = xBounds * fractalScale;
        Vector2 nby = yBounds * fractalScale;

        for (int y = 0; y < tSize.y; y++)
        {
            for (int x = 0; x < tSize.x; x++)
            {
                int i = y * tSize.x + x;

                float a, oa = Remap(x, 0, tSize.x, nbx.x, nbx.y);
                float b, ob = Remap(y, 0, tSize.y, nby.x, nby.y);
                a = oa;
                b = ob;
                float j = 0;
                for (; j < MAX_ITTER; j++)
                {
                    float square = (a * a) - (b * b);
                    float ab2 = 2 * a * b;
                    a = square + oa;
                    b = ab2 + ob;

                    float add = a + b;
                    if (add > SUDO_INFINITY || add < -SUDO_INFINITY)
                    {
                        break;
                    }
                }
                if (j == MAX_ITTER)
                {
                    colors[i] = Color.black;
                }
                else
                {
                    colors[i] = Color.HSVToRGB((float)j / MAX_ITTER, 1, 1);
                }
            }
        }
        texture.SetPixels32(colors);
        texture.Apply();
        image.texture = texture;
    }

    private float Remap(float value, float fromA, float fromB, float toA, float toB)
    {
        float norm = Mathf.InverseLerp(fromA, fromB, value);
        return Mathf.Lerp(toA, toB, norm);
    }
}
