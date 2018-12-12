using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : Structure, IObjective {
    public Vector2Int pos;

    // Use this for initialization
    void Start () {
        transform.position = Terrain.Instance.GetGrid().CellToWorld(new Vector3Int(pos.x, 0, pos.y)) + Vector3.up * Terrain.Instance.GetWorldHeight(new Vector3(pos.x, 0, pos.y));
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public static Food Create(Vector2Int pos) {
        GameObject foodGO = new GameObject();
        foodGO.transform.name = "Food";
        Food f = foodGO.AddComponent<Food>();
        f.pos = pos;
        Terrain.Instance.RegisterObject(foodGO, pos);

        return f;
    }

    public bool IsTargeted() {
        return false;
    }

    public void SetTargeted(bool s) {

    }
}
