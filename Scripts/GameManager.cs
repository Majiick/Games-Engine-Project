using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour {
    #region SINGLETON PATTERN
    public static GameManager _instance;
    public static GameManager Instance {
        get {
            if (_instance == null) {
                _instance = GameObject.FindObjectOfType<GameManager>();
            }

            return _instance;
        }
    }
    #endregion

    public static readonly int LENGTH = 128;
    public static readonly int WIDTH = 128;


    private List<IColored> _coloreds = new List<IColored>();
    private List<IRegeneratable> _regeneratables = new List<IRegeneratable>();
    public List<Village> _villages;

    [Range(1f, 50f)]
    public float speed = 1.0f;

    // Use this for initialization
    void Start () {
		//Villager.Create();
        
        Terrain.Instance.Regenerate();

        for (int i = 0; i < 2; i++) {
            Vector2Int villagePos;
            do {
                villagePos = new Vector2Int(Random.Range(0, LENGTH), Random.Range(0, WIDTH));
            } while (Terrain.Instance.GetTerrainType(villagePos) == global::Terrain.TerrainType.Land);

            _villages.Add(Village.Create(villagePos));
        }
        
        // Spawn trees
        for (int x = 0; x < LENGTH; x++) {
            for (int y = 0; y < WIDTH; y++) {
                if (Terrain.Instance.GetTerrainType(new Vector2Int(y, x)) == Terrain.TerrainType.Land && Random.Range(0.0f, 100f) > 99.7f) {
                    Tree.Create(new Vector2Int(x, y));
                }
            }
        }

        StartCoroutine("SpawnFood");

        for (int i = 0; i < 20; i++) {
            SpawnFood();
        }
    }

    void tst() {
        List<Vector3Int> path = PathFinding.FindPath(new Vector3Int(0, 0, 0), new Vector3Int(2, 100, 0));
        foreach (var pos in path) {
            Tree.Create(new Vector2Int(pos.x, pos.y));
        }
    }

    IEnumerator SpawnFood() {
        while (true) {
            Vector2Int randomPos;
            do {
                randomPos = new Vector2Int(Random.Range(0, WIDTH - 1), Random.Range(0, LENGTH - 1));
            } while (Terrain.Instance.GetTerrainType(new Vector2Int(randomPos.y, randomPos.x)) != Terrain.TerrainType.Land);

            Food.Create(randomPos);
            yield return new WaitForSeconds(4f);
        }
    }
	
	// Update is called once per frame
	void Update () {
	    Time.timeScale = speed;

        if (Input.GetKeyDown(KeyCode.C)) {
	        ColorScheme.NewColors();

            foreach (var colored in _coloreds) {
	            colored.ColorIn();
	        }
	    }

	    if (Input.GetKeyDown(KeyCode.R)) {
	        ColorScheme.NewColors();

            foreach (var regeneratable in _regeneratables) {
	            regeneratable.Regenerate();
            }
	    }
    }

    public void RegisterColored(IColored colored) {
        _coloreds.Add(colored);
    }

    public void RegisterRegeneratable(IRegeneratable regeneratable) {
        _regeneratables.Add(regeneratable);
    }
}
