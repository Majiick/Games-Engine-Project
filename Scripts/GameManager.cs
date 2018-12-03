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

	// Use this for initialization
	void Start () {
		Villager.Create();
	}
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetKeyDown(KeyCode.C)) {
	        ColorScheme.NewColors();

            foreach (var colored in _coloreds) {
	            colored.ColorIn();
	        }
	    }
	}

    public void RegisterColored(IColored colored) {
        _coloreds.Add(colored);
    }
}
