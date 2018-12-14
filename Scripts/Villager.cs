using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Villager : MonoBehaviour, IColored {
    private GameObject _head;
    private GameObject _rightLeg;
    private GameObject _leftLeg;
    public Vector2Int pos;
    public Village myVillage;
    private float hunger = 0.0f;

    protected List<Vector3Int> pathToObjective; // 0 is Objective and .Count-1 is next square from starting location.
    protected int nextSquare;
    protected float fractionMovedToNextSquare = 0;
    private float speed = 2f;
    private GameObject objective;
    private TextMesh info;

    public void ColorIn() {
        _head.GetComponent<Renderer>().material.color = ColorScheme.Main1;
        _leftLeg.GetComponent<Renderer>().material.color = ColorScheme.Ternary1;
        _rightLeg.GetComponent<Renderer>().material.color = ColorScheme.Ternary2;
    }

    void Awake () {
        var textMeshGO = new GameObject();
        textMeshGO.transform.SetParent(this.transform);
        textMeshGO.transform.localPosition = new Vector3(0, 3f, 0);
        info = textMeshGO.AddComponent<TextMesh>();
        info.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        info.alignment = TextAlignment.Center;
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
        pathToObjective.RemoveAt(0); // Remove objective so we're only standing next to it.
        nextSquare = pathToObjective.Count - 1;

        
    }

    // Update is called once per frame
    void Update () {
        string walkingTo = "";
        try {
            if (objective.GetComponent<Tree>() != null) {
                walkingTo = "Tree";
            }

            if (objective.GetComponent<Village>() != null) {
                walkingTo = "Village";
            }

            if (objective.GetComponent<Food>() != null) {
                walkingTo = "Food";
            }
        }
        catch (Exception e) {
            objective = null;
        }

        info.text = "Hunger: " + hunger.ToString() + "\n" +
                    "Walking to: " + walkingTo;

        if (SceneView.currentDrawingSceneView) {
            info.gameObject.transform.LookAt(SceneView.currentDrawingSceneView.camera.transform);
            info.gameObject.transform.localRotation = Quaternion.LookRotation(-info.gameObject.transform.forward, Vector3.up);
        }
        

        if (objective != null) {
            Move();
        }

        if (objective == null) {
            ChooseNewObjective();
        }

        hunger -= Time.deltaTime / 60;

    }

    private void ReachedObjective() {
        if (objective.GetComponent<Food>() != null) {
            hunger = 1;
        }

        if (objective.GetComponent<IObjective>() != null) {
            objective.GetComponent<IObjective>().Use();
        }

        ChooseNewObjective();
    }

    private void ChooseNewObjective() {

        try {
            if (hunger < 0) {
                objective = Terrain.Instance.GetNearestFood(pos).gameObject;
                pathToObjective = PathFinding.FindPath(pos, objective.GetComponent<Food>().pos);
                pathToObjective.RemoveAt(0); // Remove objective so we're only standing next to it.
                nextSquare = pathToObjective.Count - 1;
                return;
            }

            if (objective.GetComponent<Tree>() != null) {
                objective = myVillage.gameObject;
                pathToObjective = PathFinding.FindPath(pos, objective.GetComponent<Village>().pos);
                pathToObjective.RemoveAt(0); // Remove objective so we're only standing next to it.
                nextSquare = pathToObjective.Count - 1;
                return;
            }

            if (objective.GetComponent<Food>() != null) {
                objective = Terrain.Instance.GetNearestTree(pos).gameObject;
                pathToObjective = PathFinding.FindPath(pos, objective.GetComponent<Tree>().pos);
                pathToObjective.RemoveAt(0); // Remove objective so we're only standing next to it.
                nextSquare = pathToObjective.Count - 1;
                return;
            }

            if (objective.GetComponent<Village>() != null) {
                objective = Terrain.Instance.GetNearestTree(pos).gameObject;
                pathToObjective = PathFinding.FindPath(pos, objective.GetComponent<Tree>().pos);
                pathToObjective.RemoveAt(0); // Remove objective so we're only standing next to it.
                nextSquare = pathToObjective.Count - 1;
                return;
            }
        }
        catch (NullReferenceException e) {

        }

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
                Terrain.Instance.Roads[pathToObjective[nextSquare].x, pathToObjective[nextSquare].y] += 0.1f;
                if (Terrain.Instance.Roads[pathToObjective[nextSquare].x, pathToObjective[nextSquare].y] > 0.9f) {
                    Terrain.Instance.Roads[pathToObjective[nextSquare].x, pathToObjective[nextSquare].y] = 0.9f;
                }
                pos = new Vector2Int(pathToObjective[nextSquare].x, pathToObjective[nextSquare].y);
            }

            nextSquare--;
        }
    }

    public static Villager Create(Vector2Int pos, Village fromVillage) {
        GameObject newVillager = new GameObject();
        newVillager.transform.name = "Villager";
        newVillager.AddComponent<Villager>();
        GameManager.Instance.RegisterColored(newVillager.GetComponent<Villager>());

        newVillager.GetComponent<Villager>().pos = pos;
        newVillager.GetComponent<Villager>().myVillage = fromVillage;
        return newVillager.GetComponent<Villager>();
    }
}
