using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LaserScript : MonoBehaviour {

    public static LaserScript Instance;

    public GameObject primaryHand;
    public GameObject rightHand;
    public GameObject leftHand;

    //Laser variables
    public LineRenderer laserLineRenderer;
    public float laserWidth = 0.1f;
    public float laserMaxLength = 500f;

    //Raycast Variables
    public RaycastHit vision;
    public float rayLength;

    // Use this for initialization
    void Start ()
    {
        Instance = this;
        Vector3[] initLaserPositions = new Vector3[2] { Vector3.zero, Vector3.zero };
        laserLineRenderer.SetPositions(initLaserPositions);
        laserLineRenderer.SetWidth(laserWidth, laserWidth);
        laserLineRenderer.enabled = true;

        //Initialize laser stuff
        primaryHand = rightHand;
        EventSystem.current.transform.GetComponent<OVRInputModule>().rayTransform = rightHand.transform;

        //Initialize raycast stuff
        rayLength = laserMaxLength;
    }
	
	// Update is called once per frame
	void Update () {
        ShootLaserFromTargetPosition(primaryHand.transform.position, primaryHand.transform.TransformDirection(Vector3.forward), laserMaxLength);
        Debug.DrawRay(primaryHand.transform.position, primaryHand.transform.TransformDirection(Vector3.forward) * rayLength, Color.red, 0.5f);
    }

    #region Laser
    void ShootLaserFromTargetPosition(Vector3 targetPosition, Vector3 direction, float length)
    {
        Ray ray = new Ray(targetPosition, direction);
        RaycastHit raycastHit;
        Vector3 endPosition = targetPosition + (length * direction);

        if (Physics.Raycast(ray, out raycastHit, length))
        {
            endPosition = raycastHit.point;
        }

        laserLineRenderer.SetPosition(0, targetPosition);
        laserLineRenderer.SetPosition(1, endPosition);
    }
    #endregion
}
