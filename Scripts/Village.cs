using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Accessibility;

public class Village : Structure {
    private GameObject _model;
    public Vector2Int pos;

	// Use this for initialization
	override public void Start () {
        base.Start();

	    GameObject cone = Resources.Load<GameObject>("cone");
	    _model = GameObject.Instantiate(cone);
        _model.transform.localScale = new Vector3(1, 1, 5f);
        _model.transform.SetParent(transform);
	    transform.position = Terrain.Instance.GetGrid().CellToWorld(new Vector3Int(pos.x, 0, pos.y)) + Vector3.up * Terrain.Instance.GetWorldHeight(new Vector3(pos.x, 0, pos.y));

	    _model.GetComponent<Renderer>().material.color = Color.red;

	    Villager.Create(pos + Vector2Int.down, this);
	    Villager.Create(pos + Vector2Int.up, this);
	    Villager.Create(pos + Vector2Int.right, this);
	    Villager.Create(pos + Vector2Int.left, this);
	    Villager.Create(pos + Vector2Int.left + Vector2Int.up, this);
	    Villager.Create(pos + Vector2Int.right + Vector2Int.up, this);
	    Villager.Create(pos + Vector2Int.left + Vector2Int.down, this);
	    Villager.Create(pos + Vector2Int.right + Vector2Int.down, this);


    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public static Village Create(Vector2Int pos) {
        GameObject villageGO = new GameObject();
        villageGO.transform.name = "Village";
        Village v = villageGO.AddComponent<Village>();
        v.pos = pos;
        Terrain.Instance.RegisterObject(villageGO, pos);

        return v;
    }
}
