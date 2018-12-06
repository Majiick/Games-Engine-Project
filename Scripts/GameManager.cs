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
    private Grid _grid;

    // Use this for initialization
    void Start () {
		//Villager.Create();
        _grid = GameObject.FindObjectOfType<Grid>();

        Terrain.Instance.Regenerate();

        Invoke("tst", 3.0f);
    }

    void tst() {
        List<Vector3Int> path = PathFinding.FindPath(new Vector3Int(0, 0, 0), new Vector3Int(2, 100, 0));
        foreach (var pos in path) {
            //Tree.Create(_grid.CellToWorld(new Vector3Int(pos.x, 0, pos.y)) + Vector3.up * Terrain.Instance.GetWorldHeight(new Vector3(pos.x, 0, pos.y)));
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
