using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : Structure, IObjective {
    public Vector2Int pos;
    private GameObject _model;
    private bool _targeted = false;

    // Use this for initialization
    void Start () {
        GameObject cone = Resources.Load<GameObject>("cone");
        _model = GameObject.Instantiate(cone);
        _model.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        _model.transform.SetParent(transform);
        _model.GetComponent<Renderer>().material.color = Color.red;

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
        return _targeted;
    }

    public void SetTargeted(bool s) {
        _targeted = s;
    }

    public void Use() {
        Terrain.Instance.UnregisterObject(gameObject, pos);
        Destroy(gameObject);
    }
}
