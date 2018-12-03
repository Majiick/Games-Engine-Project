using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorScheme {
    private static Color _main;
    private static Color _ternary1;
    private static Color _ternary2;
    private static bool _generatedColors = false;

    private static void GenerateColors() {
        if (_generatedColors) return;
        _generatedColors = true;

        float saturation = Random.Range(0.1f, 1f);
        float lightness = Random.Range(0.1f, 0.9f);
        float hue = Random.Range(0.0f, 1f);
        _main = Color.HSVToRGB(hue, saturation, lightness);

        float distance = Random.Range(0.1f, 0.9f);
        _ternary1 = Color.HSVToRGB((hue + distance) % 1, saturation, lightness);
        _ternary2 = Color.HSVToRGB((hue - distance) % 1, saturation, lightness);
    }

    public static void NewColors() {
        _generatedColors = false;
    }

    public static Color Main1 {
        get { GenerateColors();return _main; }
    }

    public static Color Ternary1 {
        get { GenerateColors(); return _ternary1; }
    }

    public static Color Ternary2 {
        get { GenerateColors(); return _ternary2; }
    }
}
