using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class Terrain : MonoBehaviour, IRegeneratable {
    #region SINGLETON PATTERN
    public static Terrain _instance;
    public static Terrain Instance {
        get {
            if (_instance == null) {
                _instance = GameObject.FindObjectOfType<Terrain>();
            }

            return _instance;
        }
    }
    #endregion

    public enum TerrainType { Land, Water, Mountain };
    private float[,] _terrainHeights;
    private TerrainType[,] _terrainTypes;

    private int _height = 10;
    private float _scale = 5f;
    private float _offsetx = 0f;
    private float _offsety = 0f;

	// Use this for initialization
	void Start() {
        GameManager.Instance.RegisterRegeneratable(this);
	}

    void Update() {
        
    }

    public void Regenerate() {
        int width = GameManager.WIDTH;
        int length = GameManager.LENGTH;

        _offsetx = Random.Range(0, 1000000f);
        _offsety = Random.Range(0, 1000000f);

        UnityEngine.Terrain t = GetComponent<UnityEngine.Terrain>();
        t.terrainData.size = new Vector3(width, _height, length);
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
        _terrainHeights =  new float[width,length];
        foreach (var octave in octaves) {
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < length; y++) {
                    _terrainHeights[x, y] += octave[x, y];
                }
            }
        }

        _terrainTypes = new TerrainType[width,length];
        // Clamp the heights
        Texture2D texture = new Texture2D(width, length);
        bool[,] land = new bool[width, length];
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < length; y++) {
                if (_terrainHeights[x, y] < 0.35f) {  // Water
                    texture.SetPixel(y, x, Color.blue);
                    _terrainHeights[x, y] = 0.3f;
                    _terrainTypes[x, y] = TerrainType.Water;
                }
                else if (_terrainHeights[x, y] >= 0.8) {  // Mountain
                    texture.SetPixel(y, x, Color.gray);
                    _terrainTypes[x, y] = TerrainType.Mountain;
                }
                else {  // Land
                    if (x % 2 == 0) {
                        texture.SetPixel(y, x, Color.green);
                    }
                    else {
                        texture.SetPixel(y, x, Color.green + new Color(0, -0.05f, 0));
                    }

                    land[x, y] = true;
                    _terrainTypes[x, y] = TerrainType.Land;
                }
            }
        }

        // Smooth out the land a bit.
        for (int i = 0; i < 1; i++) {
            _terrainHeights = SmoothOutTransitions(_terrainHeights, land);
        }


        SplatPrototype splat = new SplatPrototype();
        splat.texture = texture;
        splat.tileOffset = new Vector2(0, 0);
        splat.tileSize = new Vector2(width, length);
        splat.texture.Apply(true);
        t.terrainData.splatPrototypes = new SplatPrototype[] { splat };
        
        t.terrainData.SetHeights(0, 0, _terrainHeights);
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

    public TerrainType GetTerrainType(Vector2Int pos) {
        return _terrainTypes[pos.x, pos.y];
    }

    public float GetWorldHeight(Vector3 pos) {
        UnityEngine.Terrain t = GetComponent<UnityEngine.Terrain>();
        return t.SampleHeight(pos);
    }

    public float GetHeight(Vector2Int pos) {
        return _terrainHeights[pos.x, pos.y];
    }
}
