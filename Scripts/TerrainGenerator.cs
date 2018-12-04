using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour {
    int length = 128;
    int width = 128;
    int height = 10;

    private float _scale = 5f;
    private float _offsetx = 0f;
    private float _offsety = 0f;

	// Use this for initialization
	void Start() {
	    Terrain t = GetComponent<Terrain>();
	    t.terrainData.size = new Vector3(width, height, length);
	    t.terrainData.heightmapResolution = width;

	    float[,] heights = new float[width, length];
	    for (int x = 0; x < width; x++) {
	        for (int y = 0; y < length; y++) {
	            heights[x, y] = Mathf.PerlinNoise(((float)x + _offsetx) / width * _scale, ((float)y + _offsety) / length * _scale);
	        }
	    }

        //Make texture work TODO
	    Texture2D texture = new Texture2D(width, length);
	    for (int x = 0; x < width; x++) {
	        for (int y = 0; y < length; y++) {
	            if (heights[x, y] > 0.2f) {
	                texture.SetPixel(x, y, Color.blue);
	            }
	        }
	    }
	    SplatPrototype splat = new SplatPrototype();
	    splat.texture = texture;
	    t.terrainData.splatPrototypes = new SplatPrototype[] { splat };

	    t.terrainData.SetHeights(0, 0, heights);
    }

    void Update() {
        
    }
}
