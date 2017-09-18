using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColumnItem : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SelectObject(GameObject columnItem)
    {
        if (columnItem.transform.Find("Toggle").GetComponent<Toggle>().isOn)
        {
            ServerScript.Instance.selectedColumns.Add(this.transform.Find("Name").GetComponent<Text>().text);
        }
        else
        {
            ServerScript.Instance.selectedColumns.Remove(this.transform.Find("Name").GetComponent<Text>().text);
        }
    }
}
