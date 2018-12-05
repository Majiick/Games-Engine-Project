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

        const int octaveAmount = 6;
        const float octaveScaleMultiplier = 4f;
        const float amplitudeMultiplier = 0.3f;


        List<float[,]> octaves = new List<float[,]>();
        float amplitude = 1f;
        float scale = 1f;
        for (int i = 0; i < octaveAmount; i++) {
            octaves.Add(new float[width,length]);

            for (int x = 0; x < width; x++) {
                for (int y = 0; y < length; y++) {
                    octaves[i][x, y] = Mathf.PerlinNoise(((float)x + _offsetx) / width * _scale * scale, ((float)y + _offsety) / length * _scale * scale) * amplitude;
                }
            }

            amplitude = amplitude * amplitudeMultiplier;
            scale = scale * octaveScaleMultiplier;
        }

        // Generate the heights
        float[,] heights =  new float[width,length];
        foreach (var octave in octaves) {
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < length; y++) {
                    heights[x, y] += octave[x, y];
                }
            }
        }
        
        

        // Clamp the heights
        Texture2D texture = new Texture2D(width, length);
        bool[,] land = new bool[width, length];
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < length; y++) {
                if (heights[x, y] < 0.35f) {  // Water
                    texture.SetPixel(y, x, Color.blue);
                    heights[x, y] = 0.3f;
                }
                else if (heights[x, y] >= 0.8) {  // Mountain
                    texture.SetPixel(y, x, Color.gray);
                }
                else {  // Land
                    if (x % 2 == 0) {
                        texture.SetPixel(y, x, Color.green);
                    }
                    else {
                        texture.SetPixel(y, x, Color.green + new Color(0, -0.05f, 0));
                    }

                    land[x, y] = true;
                }
            }
        }

        for (int i = 0; i < 1; i++) {
            heights = SmoothOutTransitions(heights, land);
        }


        SplatPrototype splat = new SplatPrototype();
        splat.texture = texture;
        splat.tileOffset = new Vector2(0, 0);
        splat.tileSize = new Vector2(width, length);
        splat.texture.Apply(true);
        t.terrainData.splatPrototypes = new SplatPrototype[] { splat };
        
        t.terrainData.SetHeights(0, 0, heights);
    }

    float[,] SmoothOutTransitions(float[,] heights, bool[,] mask=null) {
        float[,] newHeights = (float[,])heights.Clone();
        int kernelSize = 3;
        Debug.Assert(kernelSize % 2 == 1);

        for (int x = 0; x < heights.GetLength(0); x++) {
            for (int y = 0; y < heights.GetLength(1); y++) {
                if (mask != null && !mask[x, y]) continue;

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
