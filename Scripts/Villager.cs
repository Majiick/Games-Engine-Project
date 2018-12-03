using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Villager : MonoBehaviour, IColored {
    private GameObject _head;
    private GameObject _rightLeg;
    private GameObject _leftLeg;

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

        GameManager.Instance.RegisterColored(this);
        ColorIn();
    }

    // Update is called once per frame
    void Update () {
		
	}

    public static void Create() {
        GameObject newVillager = new GameObject();
        newVillager.transform.name = "Villager";
        newVillager.AddComponent<Villager>();
    }
}
