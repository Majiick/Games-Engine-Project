using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour, IRegeneratable {
    int length = 128;
    int width = 128;
    int height = 10;

    private float _scale = 5f;
    private float _offsetx = 0f;
    private float _offsety = 0f;

	// Use this for initialization
	void Start() {
        GameManager.Instance.RegisterRegeneratable(this);
	    Regenerate();
	}

    void Update() {
        
    }

    public void Regenerate() {
        _offsetx = Random.Range(0, 1000000f);
        _offsety = Random.Range(0, 1000000f);

        Terrain t = GetComponent<Terrain>();
        t.terrainData.size = new Vector3(width, height, length);
        t.terrainData.heightmapResolution = width;

        // Generate the heights
        float[,] heights = new float[width, length];
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < length; y++) {
                heights[x, y] = Mathf.PerlinNoise(((float)x + _offsetx) / width * _scale, ((float)y + _offsety) / length * _scale);
            }
        }
        

        // Clamp the heights
        Texture2D texture = new Texture2D(width, length);
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < length; y++) {
                if (heights[x, y] < 0.3f) {  // Water
                    texture.SetPixel(y, x, Color.blue);
                    heights[x, y] = 0.3f;
                }
                else if (heights[x, y] >= 0.8) {  // Mountain
                    texture.SetPixel(y, x, Color.gray);
                    heights[x, y] = 1f;
                }
                else {  // Land
                    texture.SetPixel(y, x, Color.green);
                    // Gravitate towards 0.55f instead of clamping it completely.
                    float newHeight = Mathf.Pow(Mathf.Abs(0.55f - heights[x, y]), 1.5f);
                    if (heights[x, y] > 0.55f) {
                        newHeight = 0.55f - newHeight;
                    }
                    else {
                        newHeight = 0.55f + newHeight;
                    }

                    heights[x, y] = newHeight;
                }
            }
        }

        for (int i = 0; i < 2; i++) {
            heights = SmoothOutTransitions(heights);
        }


        SplatPrototype splat = new SplatPrototype();
        splat.texture = texture;
        splat.tileOffset = new Vector2(0, 0);
        splat.tileSize = new Vector2(width, length);
        splat.texture.Apply(true);
        t.terrainData.splatPrototypes = new SplatPrototype[] { splat };
        
        t.terrainData.SetHeights(0, 0, heights);
    }

    float[,] SmoothOutTransitions(float[,] heights) {
        float[,] newHeights = new float[heights.GetLength(0), heights.GetLength(1)];
        int kernelSize = 3;
        Debug.Assert(kernelSize % 2 == 1);

        for (int x = 0; x < heights.GetLength(0); x++) {
            for (int y = 0; y < heights.GetLength(1); y++) {
                float average = 0;
                int values_added = 0;
                for (int dx = -(kernelSize / 2); dx < kernelSize / 2 + 1; dx++) {
                    for (int dy = -(kernelSize / 2); dy < kernelSize / 2 + 1; dy++) {
                        if (x + dx > 0 && y + dy > 0 && x + dx < heights.GetLength(0) && y + dy < heights.GetLength(1)) {
                            values_added++;
                            average += heights[x + dx, y + dy];
                        }
                    }
                }

                average = average / values_added;
                newHeights[x, y] = average;
            }
        }

        return newHeights;
    }
}
