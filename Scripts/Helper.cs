using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helper {
    public static float Remap(float value, float fromMin, float fromMax, float toMin, float toMax) {
        var fromAbs = value - fromMin;
        var fromMaxAbs = fromMax - fromMin;

        var normal = fromAbs / fromMaxAbs;

        var toMaxAbs = toMax - toMin;
        var toAbs = toMaxAbs * normal;

        var to = toAbs + toMin;

        return to;
    }

    public static int ManhattanDistance(Vector3Int a, Vector3Int b) {
        return Mathf.Abs(b.x - a.x) + Mathf.Abs(b.y - a.y);
    }

    public static int ManhattanDistance(Vector2Int a, Vector2Int b) {
        return Mathf.Abs(b.x - a.x) + Mathf.Abs(b.y - a.y);
    }
}
