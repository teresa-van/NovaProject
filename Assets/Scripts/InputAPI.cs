using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using System.Text;


/// <summary>
/// Class for manipulating game objects via any controller input. Controller input should simply call
/// these for the transformations
/// </summary>
public static class InputAPI
{
    #region SetPosition
    /// <summary>
    /// Given a GameObject and a vector, sets the transform.position of the game object to the vector3
    /// </summary>
    /// <param name="g">the game object</param>
    /// <param name="v">the vector3 to set the position to</param>
    public static void SetPosition(GameObject g, Vector3 v)
    {
        g.transform.position = v;
    }

    /// <summary>
    /// Given a name and a vector, sets the transform.position of the game object to the vector3.
    /// Will debug log out an error if no object is found.
    /// </summary>
    /// <param name="g">the game object</param>
    /// <param name="v">the vector3 to set the position to</param>
    public static void SetPosition(string name, Vector3 v)
    {
        GameObject found = GameObject.Find(name);
        if (found == null)
        {
            Debug.Log("Cannot find " + name + " to set transform.");
        }
        else
        {
            found.transform.position = v;
        }
    }

    /// <summary>
    /// Given a name, and three coordinate strings, will attempt to place the game object associatd with the name at the given coords.
    /// a debug error will be printed if anything errors.
    /// </summary>
    /// <param name="name">the name</param>
    /// <param name="x">the x coord(float)</param>
    /// <param name="y">the y coord(float)</param>
    /// <param name="z">the z coord(float)</param>
    public static void SetPosition(string name, string x, string y, string z)
    {
        GameObject found = GameObject.Find(name);
        if (found == null)
        {
            Debug.Log("Cannot find " + name + " to set transform.");
            return;
        }
        Vector3 val;
        if (generateVector3(x, y, z, out val))
        {
            found.transform.position = val;
        }
    }
    #endregion

    #region RotateByVector
    /// <summary>
    /// Given a GameOBject and a Vector3, rotates the object by the vector.
    /// </summary>
    /// <param name="g">the game object</param>
    /// <param name="v">the vector to set the position to</param>
    public static void RotateByVector(GameObject g, Vector3 v)
    {
        g.transform.Rotate(v);
    }

    /// <summary>
    /// Given a name and a Vector3, rotates the object by the vector.
    /// Will debug log out an error if no object is found.
    /// </summary>
    /// <param name="g">the game object</param>
    /// <param name="v">the vector to set the position to</param>
    public static void RotateByVector(string name, Vector3 v)
    {
        GameObject found = GameObject.Find(name);
        if (found == null)
        {
            Debug.Log("Cannot find " + name + " to rotate by vector.");
        }
        else
        {
            found.transform.Rotate(v);
        }
    }

    /// <summary>
    /// Rotates a given object by a vector defined by strings
    /// reports any errors to the debug log
    /// </summary>
    /// <param name="name"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    public static void RotateByVector(string name, string x, string y, string z)
    {
        GameObject found = GameObject.Find(name);
        if (found == null)
        {
            Debug.Log("Cannot find " + name + " to rotate by vector.");
            return;
        }
        Vector3 val;
        if (generateVector3(x, y, z, out val))
        {
            found.transform.Rotate(val);
        }
    }
    #endregion

    #region RotateAroundAxis
    /// <summary>
    /// Given a GameObject, a vector, and a value, rotates the game object around the vector(axis) by the value in degrees.
    /// </summary>
    /// <param name="g">the game object</param>
    /// <param name="axis">the axis</param>
    /// <param name="degrees">the amount to rotate(degrees)</param>
    public static void RotateAroundAxis(GameObject g, Vector3 axis, float degrees)
    {
        g.transform.Rotate(axis, degrees);
    }

    /// <summary>
    /// Given a name, a vector, and a value, rotates the game object around the vector(axis) by the value in degrees.
    /// Will debug log out an error if no object is found.
    /// </summary>
    /// <param name="name">the name of the object</param>
    /// <param name="axis">the axis</param>
    /// <param name="degrees">the amount to rotate(degrees)</param>
    public static void RotateAroundAxis(string name, Vector3 axis, float degrees)
    {
        GameObject found = GameObject.Find(name);
        if (found == null)
        {
            Debug.Log("Cannot find " + name + " to rotate around axis.");
        }
        else
        {
            found.transform.Rotate(axis, degrees);
        }
    }

    /// <summary>
    /// Rotates an object around an axis defined by strings.
    /// reports any errors to the debug log
    /// </summary>
    /// <param name="name"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <param name="degrees"></param>
    public static void RotateAroundAxis(string name, string x, string y, string z, string degrees)
    {
        GameObject found = GameObject.Find(name);
        if (found == null)
        {
            Debug.Log("Cannot find " + name + " to rotate around axis.");
            return;
        }
        Vector3 axis;
        if (generateVector3(x, y, z, out axis))
        {
            float deg;
            if (float.TryParse(degrees, out deg))
            {
                found.transform.Rotate(axis, deg);
            }
        }

    }
    #endregion

    #region RotateAroundAxisThroughPoint
    /// <summary>
    /// Given a GameObject, a vector, and a value, rotates the game object around the vector(axis) by the value in degrees.
    /// </summary>
    /// <param name="g">the game object</param>
    /// <param name="axis">the axis</param>
    /// <param name="point">the point to rotate through</param>
    /// <param name="degrees">the amount to rotate(degrees)</param>
    public static void RotateAroundAxisThroughPoint(GameObject g, Vector3 axis, Vector3 point, float degrees)
    {
        g.transform.RotateAround(point, axis, degrees);
    }

    /// <summary>
    /// Given a name, a vector, and a value, rotates the game object around the vector(axis) by the value in degrees.
    /// Will debug log out an error if no object is found.
    /// </summary>
    /// <param name="g">the game object</param>
    /// <param name="axis">the axis</param>
    /// <param name="point">the point to rotate through</param>
    /// <param name="degrees">the amount to rotate(degrees)</param>
    public static void RotateAroundAxisThroughPoint(string name, Vector3 axis, Vector3 point, float degrees)
    {
        GameObject found = GameObject.Find(name);
        if (found == null)
        {
            Debug.Log("Cannot find " + name + " to rotate around axis through a point.");
        }
        else
        {
            found.transform.RotateAround(point, axis, degrees);
        }
    }

    /// <summary>
    /// Given a name, rotates the associated game object through a point around a plane.
    /// Debug logs any errors if they occur.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="axis">[x,y,z]</param>
    /// <param name="point">[x,y,z]</param>
    /// <param name="degrees"></param>
    public static void RotateAroundAxisThroughPoint(string name, string[] axis, string[] point, string degrees)
    {
        if (axis.Length != 3 || point.Length != 3)
        {
            Debug.Log("String arrays must be of size three");
            return;
        }
        GameObject found = GameObject.Find(name);
        if (found == null)
        {
            Debug.Log("Cannot find " + name + " to rotate around axis through a point.");
            return;
        }
        Vector3 ax;
        if (generateVector3(axis[0], axis[1], axis[2], out ax))
        {
            float deg;
            if (float.TryParse(degrees, out deg))
            {
                Vector3 pt;

                if (generateVector3(point[0], point[1], point[2], out pt))
                    found.transform.RotateAround(pt, ax, deg);
            }
        }
    }
    #endregion
    
    #region Scale

    /// <summary>
    /// Given a GameObject and a Float, scale object by float.
    /// </summary>
    /// <param name="g"></param>
    /// <param name="scaleFactor"></param>
    public static void Scale(GameObject g, float scaleFactor)
    {
        g.transform.localScale *= scaleFactor;
    }

    /// <summary>
    /// Scales a given GameObject by a scale factor that is defined by strings.
    /// Will debug log an error if the scale factor is invalid.
    /// </summary>
    /// <param name="g"></param>
    /// <param name="scaleFactor"></param>
    public static void Scale(GameObject g, string scaleFactor)
    {
        float scaleVal;
        if (float.TryParse(scaleFactor, out scaleVal))
        {
            g.transform.localScale *= scaleVal;
        }
        Debug.Log("Scale factor of " + scaleFactor + " is invalid.");
    }

    /// <summary>
    /// Scales a Gameobject, defined by strings, by a scale factor.
    /// Will debug log an error if the game object cannot be found.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="scaleFactor"></param>
    public static void Scale(string name, float scaleFactor)
    {
        GameObject found = GameObject.Find(name);
        if (found == null)
        {
            Debug.Log("Cannot find " + name + " to scale by scale factor.");
        }
        else
        {
            found.transform.localScale *= scaleFactor;
        }
    }

    /// <summary>
    /// Scales a Gameobject, defined by strings, by a scale factor, also defined by strings. 
    /// Will debug log an error if the game obejct cannot be found.
    /// Will debug log an error if the scale factor is invalid.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="scaleFactor"></param>
    public static void Scale(string name, string scaleFactor)
    {
        GameObject found = GameObject.Find(name);
        if (found == null)
        {
            Debug.Log("Cannot find " + name + " to scale by scale factor.");
            return;
        }
        float scaleVal;
        if (float.TryParse(scaleFactor, out scaleVal))
        {
            found.transform.localScale *= scaleVal;
        }
        Debug.Log("Scale factor of " + scaleFactor + " is invalid.");
    }


    #endregion

    #region ScaleByAllAxis

    /// <summary>
    /// Scale entire GameObject by a vector.
    /// </summary>
    /// <param name="g"></param>
    /// <param name="scaleFactor"></param>
    public static void ScaleByAllAxis(GameObject g, Vector3 scaleFactor)
    {
        g.transform.localScale = Vector3.Scale(g.transform.localScale, scaleFactor);
    }

    /// <summary>
    /// Scale entire GameObject by a vector defined by strings.
    /// Will debug log an error if vector is invalid.
    /// </summary>
    /// <param name="g"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    public static void ScaleByAllAxis(GameObject g, string x, string y, string z)
    {
        Vector3 scaleVal;
        if (generateVector3(x, y, z, out scaleVal))
        {
            g.transform.localScale = Vector3.Scale(g.transform.localScale, scaleVal);
        }
    }

    /// <summary>
    /// Scale entire GameObject, defined by strings, by a vector.
    /// Will debug log an error if game object cannot be found.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="scaleFactor"></param>
    public static void ScaleByAllAxis(string name, Vector3 scaleFactor)
    {
        GameObject found = GameObject.Find(name);
        if (found == null)
        {
            Debug.Log("Cannot find " + name + " to scale by vector.");
        }
        else
        {
            found.transform.localScale = Vector3.Scale(found.transform.localScale, scaleFactor);
        }
    }

    /// <summary>
    /// Scale entire GameObject, defined by strings, by a vector, defined by strings.
    /// Will debug log an error if game object cannot be found.
    /// Will debug log an error if vector is invalid. 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    public static void ScaleByAllAxis(string name, string x, string y, string z)
    {
        GameObject found = GameObject.Find(name);
        if (found == null)
        {
            Debug.Log("Cannot find " + name + " to scale by vector.");
            return;
        }
        Vector3 scaleVal;
        if (generateVector3(x, y, z, out scaleVal))
        {
            found.transform.localScale = Vector3.Scale(found.transform.localScale, scaleVal);
        }
    }

    #endregion

    #region ScaleOnAxis

    /// <summary>
    /// Scale given GameObject's selected axis (string) by a given scale factor.
    /// Will debug log an error if axis selected is invalid.
    /// </summary>
    /// <param name="g"></param>
    /// <param name="axisToScaleAround"></param>
    /// <param name="scaleFactor"></param>
    public static void ScaleOnAxis(GameObject g, string axisToScaleAround, float scaleFactor)
    {
        axisToScaleAround.ToLower();
        switch (axisToScaleAround[0])
        {
            case 'x':
                g.transform.localScale = new Vector3(g.transform.localScale.x * scaleFactor, g.transform.localScale.y, g.transform.localScale.z);
                break;
            case 'y':
                g.transform.localScale = new Vector3(g.transform.localScale.x, g.transform.localScale.y * scaleFactor, g.transform.localScale.z);
                break;
            case 'z':
                g.transform.localScale = new Vector3(g.transform.localScale.x, g.transform.localScale.y, g.transform.localScale.z * scaleFactor);
                break;
            default:
                Debug.Log("Axis " + axisToScaleAround[0] + " is invalid.");
                break;
        }
    }

    /// <summary>
    /// Scale given GameObject's selected axis (string) by a given scale factor (string).
    /// Will debug log an error if axis selected is invalid.
    /// Will debug log an error if scale factor is invalid.
    /// </summary>
    /// <param name="g"></param>
    /// <param name="axisToScaleAround"></param>
    /// <param name="scaleFactor"></param>
    public static void ScaleOnAxis(GameObject g, string axisToScaleAround, string scaleFactor)
    {
        axisToScaleAround.ToLower();
        float scaleVal;
        if (float.TryParse(scaleFactor, out scaleVal))
        {
            switch (axisToScaleAround[0])
            {
                case 'x':
                    g.transform.localScale = new Vector3(g.transform.localScale.x * scaleVal, g.transform.localScale.y, g.transform.localScale.z);
                    break;
                case 'y':
                    g.transform.localScale = new Vector3(g.transform.localScale.x, g.transform.localScale.y * scaleVal, g.transform.localScale.z);
                    break;
                case 'z':
                    g.transform.localScale = new Vector3(g.transform.localScale.x, g.transform.localScale.y, g.transform.localScale.z * scaleVal);
                    break;
                default:
                    Debug.Log("Axis " + axisToScaleAround[0] + " is invalid.");
                    break;
            }
        }

    }

    /// <summary>
    /// Scale a given (by string) GameObject's selected axis (string) by a given scale factor.
    /// Will debug log an error if axis selected is invalid.
    /// Will debug log an error if GameObejct cannot be found. 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="axisToScaleAround"></param>
    /// <param name="scaleFactor"></param>
    public static void ScaleOnAxis(string name, string axisToScaleAround, float scaleFactor)
    {
        GameObject found = GameObject.Find(name);
        if (found == null)
        {
            Debug.Log("Cannot find " + name + " to scale by vector.");
            return;
        }
        axisToScaleAround.ToLower();
        switch (axisToScaleAround[0])
        {
            case 'x':
                found.transform.localScale = new Vector3(found.transform.localScale.x * scaleFactor, found.transform.localScale.y, found.transform.localScale.z);
                break;
            case 'y':
                found.transform.localScale = new Vector3(found.transform.localScale.x, found.transform.localScale.y * scaleFactor, found.transform.localScale.z);
                break;
            case 'z':
                found.transform.localScale = new Vector3(found.transform.localScale.x, found.transform.localScale.y, found.transform.localScale.z * scaleFactor);
                break;
            default:
                Debug.Log("Axis " + axisToScaleAround[0] + " is invalid.");
                break;
        }
    }

    /// <summary>
    /// Scale a given (by string) GameObject's  selected axis (string) by a given scale factor (string).
    /// Will debug log an error if axis selected is invalid.
    /// Will debug log an error if GameOBject cannot be found.
    /// Will debug log an error if scale factor is invalid.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="axisToScaleAround"></param>
    /// <param name="scaleFactor"></param>
    public static void ScaleOnAxis(string name, string axisToScaleAround, string scaleFactor)
    {
        GameObject found = GameObject.Find(name);
        float scaleVal;
        if (found == null)
        {
            Debug.Log("Cannot find " + name + " to scale by vector.");
            return;
        }
        if (float.TryParse(scaleFactor, out scaleVal))
        {
            switch (axisToScaleAround[0])
            {
                case 'x':
                    found.transform.localScale = new Vector3(found.transform.localScale.x * scaleVal, found.transform.localScale.y, found.transform.localScale.z);
                    break;
                case 'y':
                    found.transform.localScale = new Vector3(found.transform.localScale.x, found.transform.localScale.y * scaleVal, found.transform.localScale.z);
                    break;
                case 'z':
                    found.transform.localScale = new Vector3(found.transform.localScale.x, found.transform.localScale.y, found.transform.localScale.z * scaleVal);
                    break;
                default:
                    Debug.Log("Axis " + axisToScaleAround[0] + " is invalid.");
                    break;
            }
        }
        else
        {
            Debug.Log("Scale factor of " + scaleFactor + " is invalid.");
        }
    }


    #endregion

    #region Zoom

    /// <summary>
    /// Given GameObject, zoom, in reference to object, by a given zoom factor.
    /// </summary>
    /// <param name="g"></param>
    /// <param name="zoomFactor"></param>
    public static void Zoom(GameObject g, float zoomFactor) // Zooms in by factor
    {
        Vector3 distance = (Camera.main.transform.position - g.transform.position) / zoomFactor;
        Camera.main.transform.position = distance + g.transform.position;
    }

    /// <summary>
    /// Given GameObject, zoom, in reference to object, by a given zoom factor (string).
    /// Will debug log an error if the zoom factor is invalid. 
    /// </summary>
    /// <param name="g"></param>
    /// <param name="zoomFactor"></param>
    public static void Zoom(GameObject g, string zoomFactor)
    {
        float zoomVal;
        if (float.TryParse(zoomFactor, out zoomVal))
        {
            Vector3 distance = (Camera.main.transform.position - g.transform.position) / zoomVal;
            Camera.main.transform.position = distance + g.transform.position;
        }
        else
        {
            Debug.Log("Zoom factor of " + zoomFactor + " is invalid.");
        }
    }

    /// <summary>
    /// Given GameObject (string), zoom, in reference to object, by a given zoom factor.
    /// Will debug log an error if GameObject cannot be found.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="zoomFactor"></param>
    public static void Zoom(string name, float zoomFactor)
    {
        GameObject found = GameObject.Find(name);
        if (found == null)
        {
            Debug.Log("Cannot find GameObject with name " + name + ".");
            return;
        }
        Vector3 distance = (Camera.main.transform.position - found.transform.position) / zoomFactor;
        Camera.main.transform.position = distance + found.transform.position;
    }

    /// <summary>
    /// Given GameObject (string), zoom, in reference to object, by a given zoom factor (string).
    /// Will debug log an error if GameObject cannot be found.
    /// Will debug log an error if zoom factor is invalid.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="zoomFactor"></param>
    public static void Zoom(string name, string zoomFactor)
    {
        GameObject found = GameObject.Find(name);
        if (found == null)
        {
            Debug.Log("Cannot find GameObject with name " + name + ".");
            return;
        }
        float zoomVal;
        if (float.TryParse(zoomFactor, out zoomVal))
        {
            Vector3 distance = (Camera.main.transform.position - found.transform.position) / zoomVal;
            Camera.main.transform.position = distance + found.transform.position;
        }
        else
        {
            Debug.Log("Zoom factor of " + zoomFactor + " is invalid.");
        }
    }

    #endregion

    #region Object Splitting

    public static void SplitByPlane(GameObject g, Plane[] planes)
    {
        if (g.GetComponentInChildren<MeshFilter>())
        {
            for (int i = 0; i < g.transform.childCount; i++)
            {
                SplitByPlane(g.transform.GetChild(i).gameObject, planes);
            }
        }

        if (!g.GetComponent<MeshFilter>())
            return;

        List<Vector3> oldVertices = g.GetComponent<MeshFilter>().mesh.vertices.ToList();
        List<int> oldTriangles = g.GetComponent<MeshFilter>().mesh.triangles.ToList();

        List<Vector3> newVertices = new List<Vector3>();
        List<int> newTriangles = new List<int>();

        for (int i = 0; i < oldTriangles.Count; i += 3)
        {
            Vector3[] triangle = new Vector3[3] { oldVertices[oldTriangles[i]], oldVertices[oldTriangles[i + 1]], oldVertices[oldTriangles[i + 2]] };
            bool[] pointInPlane = new bool[] { false, false, false };

            for(int j = 0; j < triangle.Length; j++)
            {
                Vector3 closestPoint = Vector3.one;
                float distance = float.MaxValue;
                foreach (Plane plane in planes)
                {
                    if (plane.GetSide(g.transform.TransformPoint(triangle[j])))
                    {
                        triangle[j] = g.transform.TransformPoint(triangle[j]);
                        pointInPlane[j] = true;
                        break;
                    }

                    else
                    {
                        if (distance == 0)
                            closestPoint = plane.ClosestPointOnPlane(g.transform.TransformPoint(triangle[j]));
                        else
                        {
                            Vector3 temp = plane.ClosestPointOnPlane(g.transform.TransformPoint(triangle[j]));
                            if (distance > Vector3.Distance(temp, triangle[j]))
                            {
                                distance = Vector3.Distance(temp, triangle[j]);
                                closestPoint = temp;
                            }
                        }

                    }
                }

                if (!pointInPlane[j])
                    triangle[j] = closestPoint;
            }

            if (pointInPlane.All(boolVal => boolVal == false))
                continue;

            foreach (Vector3 t in triangle)
            {
                if (!newVertices.Contains(g.transform.InverseTransformPoint(t)))
                    newVertices.Add(g.transform.InverseTransformPoint(t));

                newTriangles.Add(newVertices.IndexOf(g.transform.InverseTransformPoint(t)));
            }
        }

        Mesh m = new Mesh();

        m.vertices = newVertices.ToArray();
        m.triangles = newTriangles.ToArray();
        m.RecalculateNormals();

        Debug.Log(oldVertices.Count);
        Debug.Log(newVertices.Count);

        g.GetComponent<MeshFilter>().mesh = m;

    }
    #endregion

    #region Object Deletion
    /// <summary>
    /// Given a game object, destroys it.
    /// </summary>
    /// <param name="g">the game object</param>
    public static void DeleteObject(GameObject g)
    {
        UnityEngine.Object.Destroy(g);
    }

    /// <summary>
    /// Given a name, destroys that game object.
    /// Will debug log out an error if no object is found.
    /// </summary>
    /// <param name="g">the name</param>
    public static void DeleteObject(string name)
    {
        GameObject found = GameObject.Find(name);
        if (found == null)
        {
            Debug.Log("Cannot find " + name + " to delete.");
        }
        else
        {
            UnityEngine.Object.Destroy(GameObject.Find(name));
        }
    }

    /// <summary>
    /// Given a list of game objects, destroys them.
    /// </summary>
    /// <param name="objs">the list of game object</param>
    public static void DeleteListOfObjects(List<GameObject> objs)
    {
        foreach (GameObject g in objs)
        {
            UnityEngine.Object.Destroy(g);
        }
    }

    /// <summary>
    /// Given a list of names, destroys them.
    /// Will debug log out an error if no object is found.
    /// </summary>
    /// <param name="objs">the list of names</param>
    public static void DeleteListOfObjects(List<string> names)
    {
        for (int i = 0; i < names.Count; i++)
        {
            string name = names[i];
            GameObject found = GameObject.Find(name);
            if (found == null)
            {
                Debug.Log("Cannot find " + name + " to delete.");
            }
            else
            {
                UnityEngine.Object.Destroy(GameObject.Find(name));
            }
        }
    }
    #endregion

    #region ActiveToggle
    /// <summary>
    /// Given a game object, toggles its active state.
    /// </summary>
    /// <param name="g">the game object to toggle</param>
    public static void ActiveToggle(GameObject g)
    {
        g.SetActive(!g.activeSelf);
    }

    /// <summary>
    /// Given a name, toggles its active state.
    /// Will debug log if no object is found
    /// </summary>
    /// <param name="name">the game object to toggle</param>
    public static void ActiveToggle(string name)
    {
        GameObject found = GameObject.Find(name);
        if (found == null)
        {
            Debug.Log("Cannot find " + name + " to toggle active.");
        }
        else
        {
            found.SetActive(!found.activeSelf);
        }
    }
    #endregion

    #region Helper Methods
    /// <summary>
    /// Attempts to generate a Vector 3 from three strings.
    /// Returns true if worked, false otherwise.
    /// If false, an error is printed in the debug log.
    /// </summary>
    /// <param name="x">the x val</param>
    /// <param name="y">the y val</param>
    /// <param name="z">the z val</param>
    /// <param name="ret">the returned value</param>
    /// <returns></returns>
    private static bool generateVector3(string x, string y, string z, out Vector3 ret)
    {
        Vector3 val = new Vector3();
        ret = val;
        try
        {
            float xval = float.Parse(x);
            float yval = float.Parse(y);
            float zval = float.Parse(z);
            val = new Vector3(xval, yval, zval);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            return false;
        }
        ret = val;
        return true;
    }

    /// <summary>
    /// Attempts to generate a Quaternion from four strings.
    /// Returns true if worked, false otherwise.
    /// If false, an error is printed in the debug log.
    /// </summary>
    /// <param name="x">the x val</param>
    /// <param name="y">the y val</param>
    /// <param name="z">the z val</param>
    /// <param name="w">the w val</param>
    /// <param name="ret">the returned value</param>
    /// <returns></returns>
    private static bool generateQuaternion(string x, string y, string z, string w, out Quaternion ret)
    {
        Quaternion val = new Quaternion();
        ret = val;
        try
        {
            float xval = float.Parse(x);
            float yval = float.Parse(y);
            float zval = float.Parse(z);
            float wval = float.Parse(w);
            val = new Quaternion(xval, yval, zval, wval);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            return false;
        }
        ret = val;
        return true;
    }
    #endregion


}