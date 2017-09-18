using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectItem : MonoBehaviour {

    public Transform objectReference;

    // Use this for initialization
    void Start()
    {

    }

	// Update is called once per frame
	void Update ()
    {
		
	}

    public void SelectObject(GameObject objectItem)
    {
        if (objectItem.transform.parent == InteractionsScript.Instance.SelectionScrollviewContent.transform)
        {
            Toggle(objectItem, InteractionsScript.Instance.NotToManipulate);
        }
        else
        {
            Toggle(objectItem, InteractionsScript.Instance.Hidden);
            //InteractionsScript.Instance.PopulateSelectionMenu();
        }
    }

    public void Toggle(GameObject objectItem, GameObject parent)
    {
        foreach (Transform obj in InteractionsScript.Instance.AllObjectsList)
        {
            if (obj == objectReference)
            {
                if (objectItem.transform.Find("Toggle").GetComponent<Toggle>().isOn)
                {
                    obj.SetParent(InteractionsScript.Instance.ToManipulate.transform);
                }
                else
                {
                    obj.SetParent(parent.transform);
                }
            }
        }
    }
}
