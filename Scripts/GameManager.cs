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

    private List<IColored> _coloreds = new List<IColored>();
    private List<IRegeneratable> _regeneratables = new List<IRegeneratable>();
    private Grid _grid;

    // Use this for initialization
    void Start () {
		//Villager.Create();
        _grid = GameObject.FindObjectOfType<Grid>();

        for (int i = 0; i < 10; i++) {
	        Tree.Create(_grid.CellToWorld(new Vector3Int(i, 0, i)));
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
