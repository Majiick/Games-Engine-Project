using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private Grid _grid;

    // Use this for initialization
    void Start () {
		//Villager.Create();
        _grid = GameObject.FindObjectOfType<Grid>();

        Terrain.Instance.Regenerate();

        for (int x = 0; x < WIDTH; x++) {
            for (int y = 0; y < LENGTH; y++) {
                if (Terrain.Instance.TerrainTypes[x, y] == Terrain.TerrainType.Land) {
                    if (Random.Range(0f, 100f) > 99f) {
                        // TODO GET HEIGHT
                        Tree.Create(_grid.CellToWorld(new Vector3Int(x, 0, y)));
                    }
                }
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
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
