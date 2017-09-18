using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using System.IO;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Collections;
using UnityEngine.SceneManagement;

public class ServerScript : MonoBehaviour {

    public static ServerScript Instance;
    private string process;
    public List<string> selectedColumns;
    public string[] selectedAxis;

    public GameObject ProgressBar;
    public GameObject columnItem;
    public GameObject columnContainer;
    public GameObject columnScroll;
    public InputField divisions;
    public GameObject ParseMenu;
    public GameObject SplitMenu;
    public GameObject MarchMenu;
    public GameObject MainMenu;
    public GameObject MainMainMenu;
    public GameObject ContinueMenu;

    public GameObject AllMeshMenu;
    public GameObject SingularMeshMenu;
    public Dropdown ColumnDropdown;
    public Dropdown xAxis;
    public Dropdown yAxis;
    public Dropdown zAxis;

    public Text Message;

    private bool parsing;
    private bool splitting;
    private bool marching;
    private float t = 0.0f;

    private Thread parsefile;
    private Thread splitfile;
    private Thread marchingcubes;

    private List<string> columnsToIgnore;
    private string MarchColumn;
    private bool SplitColumnsPopulated;

    void Awake()
    {
        Instance = this;
    }

	void Start()
    {
        parsing = false;
        splitting = false;
        marching = false;
    }

    public void SelectProcess(Text process)
    {
        ResetProgressBar();

        if (process.text.Equals("PARSE")) InitializeParsing();
        else if (process.text.Equals("SPLIT"))
        {
            InitializeSplitting();
            if (FileExplorerScript.Instance.selectedPath != null) SplitMenu.transform.Find("Panel/PathInputField").GetComponent<InputField>().text = FileExplorerScript.Instance.selectedPath;
        }
        else if (process.text.Equals("MARCH"))
        {
            InitializeMarchingCubes();
            if (FileExplorerScript.Instance.selectedPath != null) AllMeshMenu.transform.Find("Panel/PathInputField").GetComponent<InputField>().text = FileExplorerScript.Instance.selectedPath;
        }
    }

    void Update()
    {
        if (parsing)
        {
            if (ServerSide.Tools.PercentageClass.percentages.ContainsKey("Parsing")) UpdatePercentage(ServerSide.Tools.PercentageClass.percentages["Parsing"]);
            if (!parsefile.IsAlive)
            {
                parsing = false;
                print("Finished.");
                OpenContinueMenu("parsing");
            }
        }
        if (splitting)
        {
            Message.text = ServerSide.Parsing.FieldSplitter.message;
            if ((selectedColumns.Count <= 2))
            {
                if (ServerSide.Tools.PercentageClass.percentages.Count > 0) UpdatePercentage(100);
                else FakeLerp();
            }
            else
            {
                foreach (string hdr in selectedColumns)
                {
                    if (ServerSide.Tools.PercentageClass.percentages.ContainsKey("Splitting " + hdr))
                    {
                        if (!columnsToIgnore.Contains(hdr)) columnsToIgnore.Add(hdr);
                        float percent = ((float)columnsToIgnore.Count / (float)selectedColumns.Count) * 100;
                        UpdatePercentage(percent);
                    }
                }
            }

            if (!splitfile.IsAlive && ProgressBar.transform.Find("Radial/Fill").GetComponent<Image>().fillAmount == 1)
            {
                splitting = false;
                print("Finished.");
                OpenContinueMenu("splitting");
            }
        }
        if (marching)
        {
            Message.text = ServerSide.MarchingAlgorithm.AllMeshes.message;
            if (FileExplorerScript.Instance.DialogMenu == AllMeshMenu)
            {
                if (ServerSide.Tools.PercentageClass.percentages.ContainsKey("Creating Mesh"))
                {
                    print(ServerSide.Tools.PercentageClass.percentages["Creating Mesh"]);
                    UpdatePercentage(ServerSide.Tools.PercentageClass.percentages["Creating Mesh"]);
                }
            }
            else FakeLerp();

            if (!marchingcubes.IsAlive)
            {
                if (FileExplorerScript.Instance.DialogMenu == SingularMeshMenu) UpdatePercentage(100);
                if (ProgressBar.transform.Find("Radial/Fill").GetComponent<Image>().fillAmount == 1)
                {
                    marching = false;
                    print("Finished.");
                    OpenContinueMenu("marching cubes");
                }
            }
        }
	}

    void UpdatePercentage(double percent)
    {
        ProgressBar.transform.Find("Radial/Fill").GetComponent<Image>().fillAmount = Mathf.Lerp(ProgressBar.transform.Find("Radial/Fill").GetComponent<Image>().fillAmount, (float)percent / 100, t);
        //ProgressBar.transform.Find("Radial/Percentage").GetComponent<Text>().text = percent.ToString("#.##") + "%";
        float truePercent = ProgressBar.transform.Find("Radial/Fill").GetComponent<Image>().fillAmount * 100;
        ProgressBar.transform.Find("Radial/Percentage").GetComponent<Text>().text = truePercent.ToString("#.##") + "%";

        t += Time.deltaTime * 0.25f;
    }

    void FakeLerp()
    {
        ProgressBar.transform.Find("Radial/Fill").GetComponent<Image>().fillAmount = Mathf.Lerp(ProgressBar.transform.Find("Radial/Fill").GetComponent<Image>().fillAmount, 0.95f, t);
        float truePercent = ProgressBar.transform.Find("Radial/Fill").GetComponent<Image>().fillAmount * 100;

        System.Random rdm = new System.Random();
        if (Math.Truncate(((truePercent - Math.Truncate(truePercent)) * 100)) % rdm.Next(0, 50) == 0 && truePercent > 0.1) ProgressBar.transform.Find("Radial/Percentage").GetComponent<Text>().text = truePercent.ToString("#.##") + "%";

        t += Time.deltaTime * 0.000001f;
    }

    #region Parsing

    void InitializeParsing()
    {
        CloseAllMenus();
        FileExplorerScript.Instance.DialogMenu = ParseMenu;
        ParseMenu.SetActive(true);
        print("Parsing Initialized");
    }

    public void PreParseFile()
    {
        if (!ProgressBar.activeSelf)
        {
            ProgressBar.SetActive(true);
            FileExplorerScript.Instance.DialogMenu.SetActive(false);
        }

        ProgressBar.transform.Find("Title").GetComponent<Text>().text = "Parsing...";

        ThreadStart start = new ThreadStart(ParseFile);
        parsefile = new Thread(start);
        parsefile.Start();
    }

    void ParseFile()
    {
        parsing = true;
        print("Parsing...");
        ServerSide.Parsing.FileParser.parseDataIntoCache(FileExplorerScript.Instance.selectedPath);
    }

    #endregion

    #region Splitting

    void InitializeSplitting()
    {
        columnsToIgnore = new List<string>();

        CloseAllMenus();
        FileExplorerScript.Instance.DialogMenu = SplitMenu;
        if (!SplitColumnsPopulated)
        {
            PopulateColumnsList();
            selectedColumns = new List<string>();
        }
        SplitMenu.SetActive(true);
        print("Splitting Initialized");
    }

    void PopulateColumnsList()
    {
        string[] splitPath = FileExplorerScript.Instance.selectedFile.Split('.');
        string fileName = splitPath[0];
        DirectoryInfo dir = new DirectoryInfo(Path.Combine(ServerSide.Parsing.FileStructure.cacheDirectory, "01-Metal-BaseCase", "fields"));
        DirectoryInfo[] columns = dir.GetDirectories();

        int i = 0;
        foreach (DirectoryInfo column in columns)
        {
            GameObject clm = Instantiate(columnItem);

            clm.transform.SetParent(columnContainer.transform);
            clm.transform.rotation = Camera.main.transform.rotation;
            clm.transform.localPosition = new Vector3(140f, (i * -30) - 21, 0);
            clm.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);

            clm.transform.Find("Name").GetComponent<Text>().text = column.Name;
            selectedColumns.Add(column.Name);

            i++;
        }
        SplitColumnsPopulated = true;
    }

    public void OpenColumnsList()
    {
        columnScroll.SetActive(true);
    }

    public void CloseColumnsList()
    {
        columnScroll.SetActive(false);
    }

    public void PreSplitFile()
    {
        if (!ProgressBar.activeSelf)
        {
            ProgressBar.SetActive(true);
            FileExplorerScript.Instance.DialogMenu.SetActive(false);
        }

        ProgressBar.transform.Find("Title").GetComponent<Text>().text = "Splitting...";

        ThreadStart start = new ThreadStart(SplitFile);
        splitfile = new Thread(start);
        splitfile.Start();
    }

    public void SplitFile()
    {
        splitting = true;
        print("Splitting...");
        string[] splitPath = FileExplorerScript.Instance.selectedFile.Split('.');
        string fileName = splitPath[0];
        print(fileName);
        foreach (string hdr in selectedColumns.ToArray()) print(hdr);
        print(Int32.Parse(divisions.text));
        ServerSide.Parsing.FieldSplitter.splitFileGiven(fileName, selectedColumns.ToArray(), Int32.Parse(divisions.text));
    }

    #endregion

    #region Marching

    public void InitializeMarchingCubes()
    {
        PopulateMarchingColumnsList(ColumnDropdown);
        MarchColumn = ColumnDropdown.options[ColumnDropdown.value].text;
        selectedAxis = new string[3];

        CloseAllMenus();
        MarchMenu.SetActive(true);
        print("Marching Initialized");
    }

    public void OpenAllMesh()
    {
        xAxis = AllMeshMenu.transform.Find("XAxisDropdown").GetComponent<Dropdown>();
        yAxis = AllMeshMenu.transform.Find("YAxisDropdown").GetComponent<Dropdown>();
        zAxis = AllMeshMenu.transform.Find("ZAxisDropdown").GetComponent<Dropdown>();
        PopulateMarchingColumnsList(xAxis);
        PopulateMarchingColumnsList(yAxis);
        PopulateMarchingColumnsList(zAxis);

        FileExplorerScript.Instance.DialogMenu = AllMeshMenu;
        CloseAllMenus();
        AllMeshMenu.SetActive(true);
    }

    public void OpenSingularMesh()
    {
        xAxis = SingularMeshMenu.transform.Find("XAxisDropdown").GetComponent<Dropdown>();
        yAxis = SingularMeshMenu.transform.Find("YAxisDropdown").GetComponent<Dropdown>();
        zAxis = SingularMeshMenu.transform.Find("ZAxisDropdown").GetComponent<Dropdown>();
        PopulateMarchingColumnsList(xAxis);
        PopulateMarchingColumnsList(yAxis);
        PopulateMarchingColumnsList(zAxis);

        FileExplorerScript.Instance.DialogMenu = SingularMeshMenu;
        CloseAllMenus();
        SingularMeshMenu.SetActive(true);
    }

    void PopulateMarchingColumnsList(Dropdown menu)
    {
        string[] splitPath = FileExplorerScript.Instance.selectedFile.Split('.');
        string fileName = splitPath[0];
        DirectoryInfo dir = new DirectoryInfo(Path.Combine(ServerSide.Parsing.FileStructure.cacheDirectory, "01-Metal-BaseCase", "fields"));
        DirectoryInfo[] columns = dir.GetDirectories();

        List<string> options = new List<string>();
        foreach (DirectoryInfo column in columns) options.Add(column.Name);
        if (menu.options.Count == 0) menu.AddOptions(options);
    }

    public void ChooseColumn()
    {
        MarchColumn = ColumnDropdown.options[ColumnDropdown.value].text;
    }

    public void PreMarchingCubes()
    {
        if (!ProgressBar.activeSelf)
        {
            ProgressBar.SetActive(true);
            FileExplorerScript.Instance.DialogMenu.SetActive(false);
        }

        ProgressBar.transform.Find("Title").GetComponent<Text>().text = "Creating Mesh...";

        ThreadStart start = new ThreadStart(marchingCubes);
        marchingcubes = new Thread(start);
        marchingcubes.Start();
    }

    public void marchingCubes()
    {
        marching = true;
        print("Marching...");
        string fileName;
        selectedAxis[0] = xAxis.options[xAxis.value].text; selectedAxis[1] = yAxis.options[yAxis.value].text; selectedAxis[2] = zAxis.options[zAxis.value].text;
        if (FileExplorerScript.Instance.DialogMenu == AllMeshMenu)
        {
            string[] splitPath = FileExplorerScript.Instance.selectedFile.Split('.');
            fileName = splitPath[0];
            ServerSide.MarchingAlgorithm.AllMeshes.allFromDivisions(fileName, MarchColumn, 1000000, 4, selectedAxis, 0.005f);
        }
        else
        {
            fileName = FileExplorerScript.Instance.selectedPath;
            ServerSide.MarchingAlgorithm.SingularMesh mcAlg = new ServerSide.MarchingAlgorithm.SingularMesh();
            DataStructures.VoxelArray array = mcAlg.createPointCloud(fileName, 1000000, selectedAxis[0], selectedAxis[1], selectedAxis[2], null);
            MarchingCubes march = new MarchingCubes();
            march.SetTarget(0.005f); //Set the target isoLevel
            DataStructures.IM intermediate = march.CreateMesh(array.toFloatArray());
            intermediate.WriteIntermediateToFile(mcAlg.outputPath + ".IMF");
        }
    }

    #endregion

    #region Helpers

    void CloseAllMenus()
    {
        ContinueMenu.SetActive(false);
        MainMenu.SetActive(false);
        MainMainMenu.SetActive(false);
        ParseMenu.SetActive(false);
        SplitMenu.SetActive(false);
        MarchMenu.SetActive(false);

        AllMeshMenu.SetActive(false);
        SingularMeshMenu.SetActive(false);
    }

    void ResetProgressBar()
    {
        ProgressBar.SetActive(false);
        ProgressBar.transform.Find("Radial/Percentage").GetComponent<Text>().text = "0%";
        ProgressBar.transform.Find("Radial/Fill").GetComponent<Image>().fillAmount = 0;
        Message.text = "";
        t = 0.0f;
    }

    public void OpenMainMenu()
    {
        CloseAllMenus();
        ProgressBar.SetActive(false);
        MainMenu.SetActive(true);
    }

    void OpenContinueMenu(string process)
    {
        CloseAllMenus();
        ResetProgressBar();
        ContinueMenu.SetActive(true);
        ContinueMenu.transform.Find("Panel/Details").GetComponent<Text>().text = "Process: " + process + " has been completed successfully.";
    }

    public void OpenMainMainMenu()
    {
        CloseAllMenus();
        MainMainMenu.SetActive(true);
    }

    public void ContinueToInteractions()
    {
        SceneManager.LoadScene("Controller");
    }

    #endregion
}
