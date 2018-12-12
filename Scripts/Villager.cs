using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Villager : MonoBehaviour, IColored {
    private GameObject _head;
    private GameObject _rightLeg;
    private GameObject _leftLeg;
    public Vector2Int pos;
    private float hunger = 1.0f;

    protected List<Vector3Int> pathToObjective; // 0 is Objective and .Count-1 is next square from starting location.
    protected int nextSquare;
    protected float fractionMovedToNextSquare = 0;
    private float speed = 2f;
    private GameObject objective;

    public void ColorIn() {
        _head.GetComponent<Renderer>().material.color = ColorScheme.Main1;
        _leftLeg.GetComponent<Renderer>().material.color = ColorScheme.Ternary1;
        _rightLeg.GetComponent<Renderer>().material.color = ColorScheme.Ternary2;
    }

    // Use this for initialization
    void Start () {
	    GameObject cone = Resources.Load<GameObject>("cone");
        
        _head = GameObject.Instantiate(cone);
	    _head.transform.SetParent(this.transform);
	    _rightLeg = GameObject.Instantiate(cone);
	    _rightLeg.transform.SetParent(this.transform);
	    _leftLeg = GameObject.Instantiate(cone);
	    _leftLeg.transform.SetParent(this.transform);

	    _head.transform.localScale = new Vector3(0.8f, 0.8f, 1.2f);
	    _rightLeg.transform.localScale = new Vector3(0.3f, 0.3f, 0.5f);
	    _leftLeg.transform.localScale = new Vector3(0.3f, 0.3f, 0.5f);

	    _leftLeg.transform.localRotation = Quaternion.Euler(90, 0, 0);
	    _rightLeg.transform.localRotation = Quaternion.Euler(90, 0, 0);

        _leftLeg.transform.localPosition = new Vector3(0f, -0.25f, -0.5f);
	    _rightLeg.transform.localPosition = new Vector3(0f, -0.25f, 0.5f);

        ColorIn();

        transform.position = Terrain.Instance.GetGrid().CellToWorld(new Vector3Int(pos.x, 0, pos.y)) + Vector3.up * Terrain.Instance.GetWorldHeight(new Vector3(pos.x, 0, pos.y));

        objective = Terrain.Instance.GetNearestTree(pos).gameObject;
        pathToObjective = PathFinding.FindPath(pos, objective.GetComponent<Tree>().pos);
        nextSquare = pathToObjective.Count - 1;
    }

    // Update is called once per frame
    void Update () {
        if (objective != null) {
            Move();
        }



        hunger -= Time.deltaTime / 60;

    }

    private void ReachedObjective() {

    }

    private void Move() {
        if (nextSquare == -1) {
            ReachedObjective();
            return;
        }

        fractionMovedToNextSquare += speed * Time.deltaTime;
//        Debug.Log(fractionMovedToNextSquare);

        // Lerp position.
        transform.position = Vector3.Lerp(Terrain.Instance.GetGrid().CellToWorld(new Vector3Int(pos.x, 0, pos.y)),
            Terrain.Instance.GetGrid().CellToWorld(new Vector3Int(pathToObjective[nextSquare].x, 0, pathToObjective[nextSquare].y)),
            fractionMovedToNextSquare) + Vector3.up * Terrain.Instance.GetWorldHeight(new Vector3(pos.x, 0, pos.y));

        if (fractionMovedToNextSquare >= 1.0f) {
            fractionMovedToNextSquare = fractionMovedToNextSquare % 1.0f;
            if (nextSquare != -1) { // Don't change grid position if we're at the destination.
                pos = new Vector2Int(pathToObjective[nextSquare].x, pathToObjective[nextSquare].y);
            }

            nextSquare--;
        }
    }

    public static Villager Create(Vector2Int pos) {
        GameObject newVillager = new GameObject();
        newVillager.transform.name = "Villager";
        newVillager.AddComponent<Villager>();
        GameManager.Instance.RegisterColored(newVillager.GetComponent<Villager>());

        newVillager.GetComponent<Villager>().pos = pos;
        return newVillager.GetComponent<Villager>();
    }
}
