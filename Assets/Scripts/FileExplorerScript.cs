using UnityEngine;
using System.Collections;
using SimpleFileBrowser;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.VR;

public class FileExplorerScript : MonoBehaviour {

    #region Instance and Variables

    public static FileExplorerScript Instance;
    public string selectedPath;
    public string selectedFile;

    public GameObject CenterEyeAnchor;
    public GameObject DialogMenu;

    #endregion

    #region Initialization

    void Start()
    {
        Instance = this;
        if (SceneManager.GetActiveScene().name.Equals("Controller") || SceneManager.GetActiveScene().name.Equals("Main")) VRSettings.enabled = true;
        else VRSettings.enabled = false;
    }

    #endregion

    #region Button Functions

    /// <summary>
    /// Open file explorer for selecting a file
    /// </summary>
    public void OpenFile()
    {
        DialogMenu.gameObject.SetActive(false);
        OpenDialog("file");
    }

    /// <summary>
    /// Open file explorer for selecting a directory
    /// </summary>
    public void OpenDirectory()
    {
        DialogMenu.gameObject.SetActive(false);
        OpenDialog("directory");
    }

    #endregion

    #region Dialog Functions
    /// <summary>
    /// Instantiate a file browser (if null) and open the dialog
    /// </summary>
    public void OpenDialog(string type)
    {
        // Set filters (optional)
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Images", ".jpg", ".png"), new FileBrowser.Filter("Text Files", ".txt", ".pdf"));

        // Set excluded file extensions (optional) (by default, .lnk and .tmp extensions are excluded)
        FileBrowser.SetExcludedExtensions(".lnk", ".tmp", ".zip", ".rar", ".exe");

        // Add a new quick link to the browser (optional) (returns true if quick link is added successfully)
        FileBrowser.AddQuickLink(null, "Users", "C:\\Users");

        if (SceneManager.GetActiveScene().name.Equals("Main") || SceneManager.GetActiveScene().name.Equals("Controller"))
        {
            FileBrowser.m_instance.transform.SetParent(CenterEyeAnchor.transform);
            FileBrowser.m_instance.transform.transform.rotation = Camera.main.transform.rotation;
            FileBrowser.m_instance.transform.localPosition = new Vector3(0, -0.08f, 0.6f);
        }

        FileBrowser.m_instance.gameObject.GetComponent<Canvas>().worldCamera = Camera.main;


        if (type.Equals("directory"))
        {
            // Show a select folder dialog 
            FileBrowser.ShowLoadDialog((path) => { DoStuff(true, "Selected:", path); },
                                           () => { DoStuff(false, "Cancelled.", ""); },
                                           true, null, "Select Folder", "Select");
        }

        if (type.Equals("file")) StartCoroutine(ShowLoadDialogCoroutine());
    }

    IEnumerator ShowLoadDialogCoroutine()
    {
        // Show a load file dialog and wait for a response from user
        // Load file/folder: file, Initial path: default (Documents), Title: "Load File", submit button text: "Load"
        yield return FileBrowser.WaitForLoadDialog(false, null, "Open File", "Open");

        // Dialog is closed
        // Print whether a file is chosen (FileBrowser.Success)
        // and the path to the selected file (FileBrowser.Result) (null, if FileBrowser.Success is false)
        DoStuff(FileBrowser.Success, FileBrowser.Success.ToString(), FileBrowser.Result);
    }

    #endregion

    /// <summary>
    /// Do something with the path or folder that was returned from the file explorer dialog
    /// </summary>
    public void DoStuff(bool success, string uh, string path)
    {
        if (success)
        {
            Debug.Log(uh + " " + path);
            this.selectedPath = path;
            this.selectedFile = FileBrowser.m_instance.SelectedFile.Name;
            if (SceneManager.GetActiveScene().name.Equals("Server"))
            {
                this.DialogMenu.transform.Find("Panel/PathInputField").GetComponent<InputField>().text = path;
                DialogMenu.gameObject.SetActive(true); //Opens the menu dialog
            }
            if (SceneManager.GetActiveScene().name.Equals("Main") || SceneManager.GetActiveScene().name.Equals("Controller"))
            {
                DialogMenu.gameObject.SetActive(false); //Closes the menu dialog
                print(path);
                MeshReader.Instance.CreateMeshes(path);
            }
        }
        else
        {
            DialogMenu.gameObject.SetActive(true); //Reopens the menu dialog
        }
    }
}
