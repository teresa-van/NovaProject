using DataStructures;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MeshReader : MonoBehaviour
{
    public static MeshReader Instance;
    public Gradient gradient;
    public Material shaded;
    // Use this for initialization
    void Start()
    {
        Instance = this;
    }

    public void CreateMeshes(string filepath)
    {
        string[] paths = Directory.GetFiles(filepath);
        float maxTemp = getMaxTemp(paths);
        for (int i = 0; i < paths.Length; i++)
        {
            IM intermediate = null;
            try
            {
                intermediate = IM.ReadIntermediateFromFile(paths[i]);
            }
            catch (Exception e) { continue; }
            Mesh mesh = intermediate.convertToMesh();

            GameObject obj = new GameObject();
            obj.name = Path.GetFileNameWithoutExtension(paths[i]);
            MeshFilter filter = obj.AddComponent<MeshFilter>();
            MeshRenderer renderer = obj.AddComponent<MeshRenderer>();
            MeshCollider collider = obj.AddComponent<MeshCollider>();
            renderer.material = shaded;
            renderer.material.color = gradient.Evaluate(getCurrentTemp(obj.name) / maxTemp);
            filter.mesh = mesh;
            collider.sharedMesh = mesh;
            mesh.RecalculateNormals();
            filter.mesh = mesh;

            obj.transform.SetParent(this.transform, false);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.position = Vector3.zero;
        }

        InteractionsScript.Instance.Initialize();
    }

    private float getMaxTemp(string[] fileNames)
    {
        float maxTemp = float.MinValue;
        foreach(string path in fileNames)
        {
            string rangeName = Path.GetFileNameWithoutExtension(path);
            string[] range = rangeName.Split('_');
            float highEnd = float.Parse(range[1]);
            if(highEnd > maxTemp)
            {
                maxTemp = highEnd;
            }
        }
        return maxTemp;
    }

    private float getCurrentTemp(string rangeName)
    {
        //first we must figure out what our float value is for this mesh file
        string[] range = rangeName.Split('_');
        float lowEnd = float.Parse(range[0]);
        float highEnd = float.Parse(range[1]);
        float tempVal = (highEnd + lowEnd) / 2;
        return tempVal;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
