using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : Structure, IColored, IRegeneratable, IObjective {
    private GameObject _body;
    private List<GameObject> _branches = new List<GameObject>();
    public Vector2Int pos;
    public bool targeted = false;
    private int uses = 50;


    // Use this for initialization
    override public void Start () {
        base.Start();
        transform.position = Terrain.Instance.GetGrid().CellToWorld(new Vector3Int(pos.x, 0, pos.y)) + Vector3.up * Terrain.Instance.GetWorldHeight(new Vector3(pos.x, 0, pos.y));
        Regenerate();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Regenerate() {
        // Delete previous model
        _branches.Clear();
        foreach (Transform child in transform) {
            Destroy(child.gameObject);
        }

        GameObject cone = Resources.Load<GameObject>("cone");
        _body = GameObject.Instantiate(cone);
        _body.transform.SetParent(transform);
        _body.transform.localPosition = Vector3.zero;

        // Choose random size for tree
        float bodyDiameter = Random.Range(1.0f, 3f);
        transform.localScale = new Vector3(1.0f / bodyDiameter, 1.0f, 1.0f / bodyDiameter);
        float bodyHeigth = Random.Range(2.0f, 5f) / cone.transform.localScale.z;
        _body.transform.localScale = new Vector3(bodyDiameter, bodyDiameter, bodyHeigth * cone.transform.localScale.z);

        // Spawn branches on tree
        int branchAmount = Random.Range(10, 60);
        float averageBranchLength = Random.Range(0.1f, 1.5f);
        float averageBranchDiameter = Random.Range(0.05f, 0.5f);
        float averageBranchInclination = Random.Range(0, 60f);
        for (int i = 0; i < branchAmount; i++) {
            // Randomize branch parameters
            float branchLength = Random.Range(averageBranchLength - 0.1f, averageBranchLength + 0.3f);
            float branchDiameter = Random.Range(averageBranchDiameter - 0.005f, averageBranchDiameter + 0.25f);
            float yPosition = Mathf.Clamp(Random.Range(0f, bodyHeigth), branchDiameter * 2, bodyHeigth - (branchDiameter * 2));
            float branchRadiansPositionOnBody = Random.Range(0f, Mathf.PI * 2);

            // Get the radius of the tree at the yPosition.
            float treeRadius = bodyDiameter / 2f / cone.transform.localScale.x;
            float reversedYPostion = Helper.Remap(yPosition, branchDiameter, bodyHeigth - branchDiameter, bodyHeigth - branchDiameter, branchDiameter);
            float branchSpawnRadius = (treeRadius * reversedYPostion) / bodyHeigth - 0.2f;

            GameObject newBranch = GameObject.Instantiate(cone);
            newBranch.transform.SetParent(transform);
            newBranch.transform.localScale = new Vector3(branchDiameter, branchDiameter, branchLength);

            Vector3 branchPosition = new Vector3(
                                                Mathf.Sin(branchRadiansPositionOnBody) * branchSpawnRadius,
                                                yPosition,
                                                Mathf.Cos(branchRadiansPositionOnBody) * branchSpawnRadius);
            newBranch.transform.localPosition = branchPosition;
            newBranch.transform.rotation = Quaternion.LookRotation(branchPosition -
                                                                   new Vector3(_body.transform.localPosition.x,
                                                                       _body.transform.localPosition.y + yPosition,
                                                                       _body.transform.localPosition.z));
            newBranch.transform.Rotate(new Vector3(-averageBranchInclination, 0, 0));

            _branches.Add(newBranch);
        }

        ColorIn();
    }

    public void ColorIn() {
        _body.GetComponent<Renderer>().material.color = ColorScheme.Main1;
        foreach (var branch in _branches) {
            branch.GetComponent<Renderer>().material.color = ColorScheme.Ternary1;
        }
    }

    public static void Create(Vector2Int pos) {
        GameObject newTree = new GameObject();
        newTree.transform.name = "Tree";
        Tree t = newTree.AddComponent<Tree>();
        t.pos = pos;
        GameManager.Instance.RegisterColored(newTree.GetComponent<Tree>());
        GameManager.Instance.RegisterRegeneratable(newTree.GetComponent<Tree>());

        Terrain.Instance.RegisterObject(newTree, pos);
    }

    public bool IsTargeted() {
        return targeted;
    }

    public void SetTargeted(bool s) {
        targeted = s;
    }

    public void Use() {
        uses--;
        SetTargeted(false);
        if (uses == 0) {
            Terrain.Instance.UnregisterObject(gameObject, pos);
            Destroy(gameObject);
        }
    }
}
