using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using OVR.Scripts;
using Newtonsoft.Json;
using System.IO;
using System;

public class InteractionsScript : MonoBehaviour
{
    #region Variables
    public static InteractionsScript Instance;
    private bool initializeComplete;

    public int PositionSensitivity = 30;

    //Object variables
    public GameObject Objects;
    public GameObject Visible;
    public GameObject Hidden;

    public GameObject ToManipulate;
    public GameObject NotToManipulate;

    //Menu variables
    public GameObject SelectionMenu;
    public GameObject ShowHideMenu;
    public GameObject SaveLoadMenu;
    public GameObject ControlsMenu;

    public GameObject SelectionScrollviewContent;
    public GameObject ShowHideScrollviewContent;
    public GameObject SaveLoadScrollviewContent;
    public Text Message;

    public List<Transform> AllObjectsList;
    private List<ObjectModel> InitalObjectPositions;

    //Interaction variables
    private bool getInit;
    private Vector3 controllerInit;
    private Vector3 objectsInit;

    private bool raycastHit;
    private bool isPositioning;
    private bool isRotating;
    private bool isScaling;

    //Save and load variables
    private string path;
    public string selected;

    //Prefabs
    public GameObject objectItem;
    public GameObject viewItem;
    #endregion

    #region Initialization

    void Awake ()
    {
        Instance = this;
        initializeComplete = false;
    }

    void Start()
    {
        //Initialize(); //Leave ths commented if using file explorer; uncomment if file explorer is not needed.
        Message.text = "";
        Message.color = new Color32(1, 1, 1, 0);
    }

    public void Initialize()
    {
        AllObjectsList = new List<Transform>();
        foreach (Transform obj in ToManipulate.transform)
        {
            if (!obj.name.Equals("Hidden")) AllObjectsList.Add(obj);
        }
        PopulateSelectionMenu();
        PopulateShowHideMenu();
        PopulateSaveLoadMenu();
        print("Views saved in: " + Application.persistentDataPath);

        //Initialize manipulation stuff
        getInit = false;

        raycastHit = false;
        isPositioning = false;
        isRotating = false;
        isScaling = false;

        MoveToCenterPoint();

        InitalObjectPositions = new List<ObjectModel>();

        SavePositions(InitalObjectPositions);

        initializeComplete = true;
    }

    #endregion

    #region Update/Checking Button Presses

    void Update ()
    {
        if (!initializeComplete) return;
        if (selected == null) SaveLoadMenu.transform.Find("Load").GetComponent<Button>().interactable = false;
        else SaveLoadMenu.transform.Find("Load").GetComponent<Button>().interactable = true;
        CheckButtonPresses();
    }

    public void CheckButtonPresses ()
    {
        #region Hide Objects
        //Check if raycast has hit an object
        if (Physics.Raycast(LaserScript.Instance.primaryHand.transform.position, LaserScript.Instance.primaryHand.transform.TransformDirection(Vector3.forward), out LaserScript.Instance.vision, LaserScript.Instance.rayLength))
        {
            if (AllObjectsList.Contains(LaserScript.Instance.vision.transform)) raycastHit = true;
            //HIDE OBJECT
            if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))
            {
                if (AllObjectsList.Contains(LaserScript.Instance.vision.transform)) LaserScript.Instance.vision.transform.SetParent(Hidden.transform);
                foreach (Transform obj in ShowHideScrollviewContent.transform)
                {
                    if (obj.transform.GetComponent<ObjectItem>().objectReference == LaserScript.Instance.vision.transform)
                    {
                        obj.transform.Find("Toggle").GetComponent<Toggle>().isOn = false;
                    }
                }
            }
        }
        #endregion

        #region Position
        //POSITIONING METHODS
        if ((OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger) > 0) && !isRotating)// && raycastHit)
        {
            isPositioning = true;
            //if (!SelectionMenu.activeSelf) OpenSelectionMenu();
            //CloseSaveLoadMenu();

            //Get initial controller and object positions
            if (!getInit)
            {
                controllerInit = this.GetComponent<LineRenderer>().GetPosition(0);
                objectsInit = ToManipulate.transform.position;
                getInit = true;
            }

            //Calculate the new position
            Vector3 vectorChange = (this.GetComponent<LineRenderer>().GetPosition(0) - controllerInit);
            Vector3 newPosition = new Vector3(vectorChange.x, vectorChange.y, vectorChange.z) * PositionSensitivity;

            //Position
            InputAPI.SetPosition(ToManipulate, (objectsInit + newPosition));
        }
        //When finished positioning, reset variables
        if ((OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger) == 0) && isPositioning)
        {
            controllerInit = Vector3.zero;
            getInit = false;
            isPositioning = false;
            raycastHit = false;
        }
        #endregion

        #region Rotation
        //ROTATION METHODS
        if (OVRInput.Get(OVRInput.Button.Three) && !isPositioning)// && raycastHit)
        {
            isRotating = true;
            //if (!SelectionMenu.activeSelf) OpenSelectionMenu();
            //CloseSaveLoadMenu();

            //Get initial controller and object rotations
            if (!getInit)
            {
                controllerInit = LaserScript.Instance.primaryHand.transform.rotation.eulerAngles;

                objectsInit = ToManipulate.transform.eulerAngles;
                getInit = true;
            }

            //Calculate the vector to rotate by
            Vector3 vectorChange = LaserScript.Instance.primaryHand.transform.rotation.eulerAngles - controllerInit;

            vectorChange = Quaternion.Euler(Camera.main.transform.rotation.eulerAngles) * vectorChange;
            //Bandaid fix for the snapping issue... Will improve later on (maybe)
            if (vectorChange.x > 20 || vectorChange.z > 20 || vectorChange.x < -20 || vectorChange.x < -20) vectorChange = Vector3.zero;

            //Rotate
            InputAPI.RotateAroundAxisThroughPoint(ToManipulate, Vector3.right, CalculateCenterPoint(), vectorChange.x);
            //InputAPI.RotateAroundAxisThroughPoint(ToManipulate, Vector3.up, ToManipulate.transform.position, -vectorChange.y);
            InputAPI.RotateAroundAxisThroughPoint(ToManipulate, Vector3.forward, CalculateCenterPoint(), vectorChange.z);

            //Reassign initial controller position to current position
            controllerInit = LaserScript.Instance.primaryHand.transform.rotation.eulerAngles;

        }
        //When finished rotating, reset variables
        if (!OVRInput.Get(OVRInput.Button.Three) && isRotating)
        {
            controllerInit = Vector3.zero;
            getInit = false;
            isRotating = false;
            raycastHit = false;
        }
        #endregion

        #region Scale
        //SCALING METHODS
        if (OVRInput.Get(OVRInput.Button.Four))// && raycastHit)
        {
            isScaling = true;
            //if (!SelectionMenu.activeSelf) OpenSelectionMenu();
            //CloseSaveLoadMenu();

            //Get initial controller position
            if (!getInit)
            {
                controllerInit = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch) - OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch);
                getInit = true;
            }

            //Calculate the scale factor
            float scaleFactor = (OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch).x - OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch).x)
                / controllerInit.x;

            MoveToCenterPoint();
            //Check if objects are at minimum or maximum size, set scale factor to 1 if so
            if (ToManipulate.transform.localScale.x * scaleFactor > 20 || ToManipulate.transform.localScale.x * scaleFactor < 0.1) scaleFactor = 1;

            //Scale
            InputAPI.Scale(ToManipulate, scaleFactor);
        }
        //When finished scaling, reset variables
        if (!OVRInput.Get(OVRInput.Button.Four) && isScaling)
        {
            controllerInit = Vector3.zero;
            getInit = false;
            isScaling = false;
            raycastHit = false;
        }
        #endregion

        #region Capture Screenshot
        //CAPTURE SCREENSHOT
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
        {
            ScreenCapture.CaptureScreenshot("Screenshot" + System.DateTime.Now.ToFileTime() + ".png");
            UpdateMessage("Screenshot Captured!");
            print("Screenshot captured.");
        }
        #endregion

        #region Menu
        //CLOSE MENU
        if (OVRInput.GetDown(OVRInput.RawButton.B))
        {
            if (!SelectionMenu.activeSelf) OpenSelectionMenu();
            else CloseSelectionMenu();
        }
        if (OVRInput.GetDown(OVRInput.RawButton.Start))
        {
            if (!ControlsMenu.activeSelf) OpenControlsMenu();
            else CloseControlsMenu();
        }
        //OPEN SHOW/HIDE MENU
        if (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstick))
        {
            if (ShowHideMenu.activeSelf) CloseShowHideMenu();
            else OpenShowHideMenu();
        }
        //OPEN SAVE/LOAD MENU
        if (OVRInput.GetDown(OVRInput.Button.SecondaryThumbstick))
        {
            if (SaveLoadMenu.activeSelf) CloseSaveLoadMenu();
            else
            {
                isPositioning = false; isRotating = false; isScaling = false;
                OpenSaveLoadMenu();
            }
        }
        #endregion

        #region Toggle Primary Hand
        //if (OVRInput.GetDown(OVRInput.RawButton.LHandTrigger))
        //{
        //    if (LaserScript.Instance.primaryHand == LaserScript.Instance.rightHand)
        //    {
        //        LaserScript.Instance.primaryHand = LaserScript.Instance.leftHand;
        //        EventSystem.current.transform.GetComponent<OVRInputModule>().rayTransform = LaserScript.Instance.leftHand.transform;
        //    }
        //    else
        //    {
        //        LaserScript.Instance.primaryHand = LaserScript.Instance.rightHand;
        //        EventSystem.current.transform.GetComponent<OVRInputModule>().rayTransform = LaserScript.Instance.rightHand.transform;
        //    }
        //}

        #endregion
    }
    #endregion

    #region Menu Functions

    #region Selection Menu
    public void OpenSelectionMenu()
    {
        HideAllMenus();
        SelectionMenu.gameObject.SetActive(true);
        //PopulateSelectionMenu();
    }
    public void CloseSelectionMenu()
    {
        SelectionMenu.gameObject.SetActive(false);
        //foreach (Transform obj in AllObjectsList)
        //{
        //    if (obj.parent != Hidden.transform) obj.SetParent(ToManipulate.transform);
        //}
    }

    public void PopulateSelectionMenu()
    {
        List<Transform> SelectableObjects = new List<Transform>();
        foreach (Transform obj in ToManipulate.transform)
        {
            if (!obj.name.Equals("Hidden")) SelectableObjects.Add(obj);
        }

        foreach (Transform obj in SelectionScrollviewContent.transform) Destroy(obj.gameObject);
        PopulateMenu(SelectionScrollviewContent, SelectableObjects);
    }
    #endregion

    #region Show/Hide Menu
    public void OpenShowHideMenu()
    {
        HideAllMenus();
        ShowHideMenu.gameObject.SetActive(true);
    }
    public void CloseShowHideMenu()
    {
        ShowHideMenu.gameObject.SetActive(false);
    }

    public void PopulateShowHideMenu()
    {
        PopulateMenu(ShowHideScrollviewContent, AllObjectsList);
    }
    #endregion

    #region Save/Load Menu
    public void OpenSaveLoadMenu()
    {
        HideAllMenus();
        SaveLoadMenu.gameObject.SetActive(true);
    }
    public void CloseSaveLoadMenu()
    {
        SaveLoadMenu.gameObject.SetActive(false);
    }

    public void PopulateSaveLoadMenu()
    {
        foreach (Transform obj in SaveLoadScrollviewContent.transform) Destroy(obj.gameObject);

        DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath);
        FileInfo[] info = dir.GetFiles("*.txt");
        info.Select(f => f.FullName).ToArray();

        int i = 0;
        foreach (FileInfo f in info)
        {
            GameObject vwItem = Instantiate(viewItem);

            vwItem.transform.SetParent(SaveLoadScrollviewContent.transform);
            vwItem.transform.rotation = Camera.main.transform.rotation;
            vwItem.transform.localPosition = new Vector3(235, -25 + (i * -50), 0);
            vwItem.transform.localScale = new Vector3(1, 0.8f, 1);

            vwItem.GetComponent<Text>().text = f.Name;
            i++;
        }
    }
    #endregion

    #region Controls Menu
    public void OpenControlsMenu()
    {
        HideAllMenus();
        ControlsMenu.SetActive(true);
    }

    public void CloseControlsMenu()
    {
        ControlsMenu.SetActive(false);
    }
    #endregion

    public void PopulateMenu(GameObject menu, List<Transform> objectList)
    {
        int i = 0;
        foreach (Transform obj in objectList)
        {
            //Instantiate the object item to add to the list
            GameObject objItem = Instantiate(objectItem);

            //Set parent, position, and scale
            objItem.transform.SetParent(menu.transform);
            objItem.transform.rotation = Camera.main.transform.rotation;
            objItem.transform.localPosition = new Vector3(235, (i * -50) - 20, 0);
            objItem.transform.localScale = new Vector3(4, 3, 1);

            //Set name and color, and object reference
            objItem.transform.Find("Name").GetComponent<Text>().text = obj.name;
            objItem.transform.Find("Color").GetComponent<Image>().color = obj.GetComponent<Renderer>().material.color;
            objItem.GetComponent<ObjectItem>().objectReference = obj;

            i++;
        }
    }

    public void ToggleAllObjectsOn(GameObject menu)
    {
        foreach (Transform obj in menu.transform)
        {
            obj.transform.Find("Toggle").GetComponent<Toggle>().isOn = true;
        }
    }

    public void ToggleAllObjectsOff(GameObject menu)
    {
        foreach (Transform obj in menu.transform)
        {
            obj.transform.Find("Toggle").GetComponent<Toggle>().isOn = false;
        }
    }

    #endregion

    #region Save/Load Functions
    public void SaveView()
    {
        path = Path.Combine(Application.persistentDataPath, 
            System.DateTime.Now.Day + "-" + System.DateTime.Now.Month + "-" + System.DateTime.Now.Hour + System.DateTime.Now.Minute + System.DateTime.Now.Second + ".txt");
        File.Create(path).Dispose();

        ToggleAllObjectsOn(SelectionScrollviewContent);

        //Save all other game objects
        List<ObjectModel> AllObjectsInfo = new List<ObjectModel>();
        SavePositions(AllObjectsInfo);
        string view = JsonConvert.SerializeObject(AllObjectsInfo);

        File.WriteAllText(path, view);

        PopulateSaveLoadMenu();
        print("View saved!");

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }

    public void LoadView()
    {
        path = Path.Combine(Application.persistentDataPath, selected);
        print(path);
        //Check if file exists
        if (!(File.Exists(path)) || selected == null) return;

        string view = File.ReadAllText(path);

        //Decode and reference
        List<ObjectModel> AllObjectsInfo = JsonConvert.DeserializeObject<List<ObjectModel>>(view);
        SetPositions(AllObjectsInfo);

        CloseSaveLoadMenu();
    }
    #endregion

    #region Helper Functions
    public void SetInitialPositions()
    {
        SetPositions(InitalObjectPositions);
    }

    public void SavePositions(List<ObjectModel> list)
    {
        foreach (Transform obj in ToManipulate.transform)
        {
            if (!obj.name.Equals("Hidden"))
            {
                ObjectModel model = new ObjectModel(obj.name, obj.parent.name,
                    obj.transform.position.x, obj.transform.position.y, obj.transform.position.z,
                    obj.transform.rotation.eulerAngles.x, obj.transform.rotation.eulerAngles.y, obj.transform.rotation.eulerAngles.z,
                    obj.transform.localScale.x, obj.transform.localScale.y, obj.transform.localScale.z);
                list.Add(model);
            }
        }
        //Save hidden game objects
        foreach (Transform obj in Hidden.transform)
        {
            ObjectModel model = new ObjectModel(obj.name, obj.parent.name,
                obj.transform.position.x, obj.transform.position.y, obj.transform.position.z,
                obj.transform.rotation.eulerAngles.x, obj.transform.rotation.eulerAngles.y, obj.transform.rotation.eulerAngles.z,
                obj.transform.localScale.x, obj.transform.localScale.y, obj.transform.localScale.z);
            list.Add(model);
        }
        //Save ToManipulate
        ObjectModel uh = new ObjectModel(ToManipulate.name, "",
            ToManipulate.transform.position.x, ToManipulate.transform.position.y, ToManipulate.transform.position.z,
            ToManipulate.transform.rotation.eulerAngles.x, ToManipulate.transform.rotation.eulerAngles.y, ToManipulate.transform.rotation.eulerAngles.z,
            ToManipulate.transform.localScale.x, ToManipulate.transform.localScale.y, ToManipulate.transform.localScale.z);
        list.Add(uh);
    }

    public void SetPositions(List<ObjectModel> list)
    {
        foreach (ObjectModel o in list)
        {
            if (o.Name.Equals("ToManipulate"))
            {
                ToManipulate.transform.position = new Vector3(o.PositionX, o.PositionY, o.PositionZ);
                ToManipulate.transform.eulerAngles = new Vector3(o.RotationX, o.RotationY, o.RotationZ);
                ToManipulate.transform.localScale = new Vector3(o.ScaleX, o.ScaleY, o.ScaleZ);
                list.Remove(o);
                break;
            }
        }
        foreach (ObjectModel o in list)
        {
            foreach (Transform obj in AllObjectsList)
            {
                if (o.Name == obj.name)
                {
                    if (o.Parent.Equals("Hidden")) obj.SetParent(Hidden.transform);
                    else obj.SetParent(ToManipulate.transform);
                    obj.transform.position = new Vector3(o.PositionX, o.PositionY, o.PositionZ);
                    obj.transform.eulerAngles = new Vector3(o.RotationX, o.RotationY, o.RotationZ);
                    obj.transform.localScale = new Vector3(o.ScaleX, o.ScaleY, o.ScaleZ);
                }
            }
        }
    }

    public Vector3 CalculateCenterPoint()
    {
        List<float> xPositions = new List<float>();
        List<float> yPositions = new List<float>();
        List<float> zPositions = new List<float>();

        foreach (Transform obj in ToManipulate.transform)
        {
            if (!obj.name.Equals("Hidden"))
            {
                xPositions.Add(obj.GetComponent<Collider>().bounds.center.x);
                yPositions.Add(obj.GetComponent<Collider>().bounds.center.y);
                zPositions.Add(obj.GetComponent<Collider>().bounds.center.z);
            }
        }

        Vector3 centerPoint = new Vector3(xPositions.Average(), yPositions.Average(), zPositions.Average());

        return centerPoint;
    }

    public void MoveToCenterPoint()
    {
        Vector3 anchor = CalculateCenterPoint();
        Vector3 positionDifference = ToManipulate.transform.position - CalculateCenterPoint();
        foreach (Transform obj in ToManipulate.transform)
        {
            if (!obj.name.Equals("Hidden")) obj.transform.position += positionDifference;
        }
        ToManipulate.transform.position = anchor;
    }

    public void HideAllMenus()
    {
        CloseSaveLoadMenu();
        CloseSelectionMenu();
        CloseShowHideMenu();
    }

    public void UpdateMessage(string message)
    {
        Message.text = message;
        Message.color = Color.black;
        StartCoroutine(FadeText());
    }
    
    IEnumerator FadeText()
    {
        float elapsedTime = 0;

        yield return new WaitForSeconds(3);

        while (elapsedTime < 1.5f)
        {
            elapsedTime += Time.deltaTime;
            Message.color = Color32.Lerp(Message.color, new Color32(1, 1, 1, 0), elapsedTime / 1.5f);
            yield return null;
        }
    }

    #endregion
}