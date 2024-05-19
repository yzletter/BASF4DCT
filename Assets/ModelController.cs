using Assets.SimpleFileBrowserForWindows;
using IxMilia.ThreeMf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using TriLibCore;
using TriLibCore.Interfaces;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TriLibCore.SFB;
using UnityEngine.Windows;

public class ModelController : MonoBehaviour
{

    public LigamentSimulation scriptA;

    public Canvas MyCanvas;                     // Ҫ���ص�Canvas������

    // Camera��
    public Transform CameraTransform;           // �����Transform������
    private bool Focus_Flag = false;            // �����Ƿ�۽�Ŀ��Ĳ���ֵ
    public float distanceFromMidpoint = 10f;     // ����ͷ���м��֮��ľ���

    // Button��
    public Button BoneModelButton1;
    public Button BoneModelButton2;
    public Button StartButton;
    public Button FileSelectButton1;
    public Button FileSelectButton2;

    public InputField inputField1;  // �������������ֶ�
    public InputField inputField2;  // �������������ֶ�

    private bool BoneModelButton1Clicked = false;
    private bool BoneModelButton2Clicked = false;
    private bool StartButtonClicked = false;
    private bool FileSelectButton1Clicked = false;
    private bool FileSelectButton2Clicked = false;

    // Model��
    public bool Has_Load = false;
    public string model_path_1 = "";
    public string model_path_2 = "";
    public string name1;
    public string name2;
    ModelLoader modelLoader = new ModelLoader();
 
    public GameObject model1;
    public GameObject model2;

    // �ű���
    public MonoBehaviour Script1; 
    public MonoBehaviour Script2;
    public LunareMovement lunareBoneMovement;
    public NutBoneMovement nutBoneMovement;
    string script_path_1 = "";
    string script_path_2 = "";

    public bool ffff = true;
    // Start is called before the first frame update
    void Start()
    {
        // ����ѡ��ģ�Ͱ�ť
        BoneModelButton1.onClick.AddListener(() => OpenFile_BoneModelButton1(ref BoneModelButton1Clicked));
        BoneModelButton2.onClick.AddListener(() => OpenFile_BoneModelButton2(ref BoneModelButton2Clicked));
        // ����ѡ���ļ���ť
        FileSelectButton1.onClick.AddListener(() => OpenFile_FileSelectButton1(ref FileSelectButton1Clicked));
        FileSelectButton2.onClick.AddListener(() => OpenFile_FileSelectButton2(ref FileSelectButton2Clicked));

        // ������ʼ��ť
        StartButton.onClick.AddListener(() => Start_Game(ref StartButtonClicked));
    }

    // Update is called once per frame
    void Update()
    { 

        if (scriptA != null)
        {
            if (scriptA.start_move && ffff && Has_Load)
            {
                // ִ����ز���
                Debug.Log("ScriptA's isReached is true!");

                // �˶����
                lunareBoneMovement = model1.AddComponent<LunareMovement>();
                lunareBoneMovement.folderPath = script_path_1;

                nutBoneMovement = model2.AddComponent<NutBoneMovement>();
                nutBoneMovement.folderPath = script_path_2;

                ffff = false;
            }
        }

        // ����ͷ�۽�
        //if (Focus_Flag && Has_Load)
        //{
        //    // ��ȡ����ģ�͵�λ��
        //    Vector3 position1 = model1.transform.position;
        //    Vector3 position2 = model2.transform.position;

        //    // �����м��
        //    Vector3 midpoint = (position1 + position2) / 2f;

        //    // ������ͷ�����м��
        //    CameraTransform.LookAt(midpoint);

        //    // ��������ͷ���м�������
        //    Vector3 cameraToMidpoint = CameraTransform.position - midpoint;

        //    // ��������ͷ���м��֮��ľ���
        //    CameraTransform.position = midpoint + cameraToMidpoint.normalized * distanceFromMidpoint;
        //}
    }

    void Start_Game(ref bool buttonClicked)
    {
        buttonClicked = true;                  // ��ǵ�ǰ��ťΪ�ѵ��
        // �л����ļ���״̬
        MyCanvas.gameObject.SetActive(false);  // ����Canvas

        Focus_Flag = true;                     // ��������ͷ
        Debug.Log("����ͷ����");

        if (BoneModelButton1Clicked && BoneModelButton2Clicked && StartButtonClicked && FileSelectButton1Clicked && FileSelectButton2Clicked)
        {
            Start_Model();                  // ��������ť�����������ִ��ģ������
        }
    }

    void Start_Model()
    {
        modelLoader.LoadModels(model_path_1, model_path_2);

        name1 = Substr(model_path_1);
        name2 = Substr(model_path_2);

        Debug.Log(name1 + name2);

        StartCoroutine(FindBones(1.0f, name1, name2)); // �ȴ�
        IEnumerator FindBones(float waitTime, string name1, string name2)
        {
            yield return new WaitForSeconds(waitTime);

            model1 = GameObject.Find(name1).transform.GetChild(0).gameObject;
            model2 = GameObject.Find(name2).transform.GetChild(0).gameObject;

            Vector3 BonescaleFactor = new Vector3(5, 5, 5);
            Vector3 SpawnscaleFactor = new Vector3(0.07f, 0.07f, 0.07f);
            Vector3 rotationAngles = new Vector3(-90, 0, 0);

            Vector3 newPosition1 = new Vector3(1.222f, -4.001f, 0.476f);
            Vector3 newPosition2 = new Vector3(2.87f, -3.91f, -0.11f);

            Model_Set(model1, "Bone1", BonescaleFactor, rotationAngles, newPosition1);
            Model_Set(model2, "Bone2", BonescaleFactor, rotationAngles, newPosition2);

            
            Debug.Log(name1 + "ģ���Ѿ�������ϣ�" + name2 + "ģ���Ѿ��������");

            // �ʹ��������
            string inputText1 = inputField1.text;           // ��ȡ�����ֶε��ı�
            Vector3 vector1 = ParseVector3(inputText1);     // �����ַ���ΪVector3
            string inputText2 = inputField2.text;           // ��ȡ�����ֶε��ı�
            Vector3 vector2 = ParseVector3(inputText2);     // �����ַ���ΪVector3


            GameObject FirstSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            GameObject SecondSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);

            FirstSphere.transform.localScale = SpawnscaleFactor;
            SecondSphere.transform.localScale = SpawnscaleFactor;

            FirstSphere.transform.eulerAngles = rotationAngles;
            SecondSphere.transform.eulerAngles = rotationAngles;

            Rigidbody FirstSphererigidbody = FirstSphere.AddComponent<Rigidbody>();
            Rigidbody SecondSphererigidbody = SecondSphere.AddComponent<Rigidbody>();

            FirstSphererigidbody.useGravity = false;
            FirstSphererigidbody.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
            SecondSphererigidbody.useGravity = false;
            SecondSphererigidbody.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;

            Vector3 Init1 = new Vector3(0, 5, 0);
            Vector3 Init2 = new Vector3(0.5f, 5, 0);
            FirstSphere.transform.localPosition = Init1;
            SecondSphere.transform.localPosition = Init2;


            // ��ʼģ��
            LigamentSimulation ligamentSimulation;
            GameObject ligamentSimulator = GameObject.Find("LigamentSimulator");
            ligamentSimulation = ligamentSimulator.AddComponent<LigamentSimulation>();

            Vector3 Delta = new Vector3(0.05f, 0.05f, 0.05f);
            GameObject Practicle = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Rigidbody rigidbody = Practicle.AddComponent<Rigidbody>();
            rigidbody.useGravity = true;
            rigidbody.mass = 0;
            rigidbody.constraints = RigidbodyConstraints.FreezeRotation;  // ������ת
            Practicle.transform.localScale = Delta;

            ligamentSimulation.ligamentSegmentPrefab = Practicle;
            ligamentSimulation.bone1 = FirstSphere;
            ligamentSimulation.bone2 = SecondSphere;

            ligamentSimulation.model1 = model1;
            ligamentSimulation.model2 = model2;
            ligamentSimulation.vector1 = vector1;
            ligamentSimulation.vector2 = vector2;

            ligamentSimulation.segmentCount = 25;           // Number of segments in the ligament
            ligamentSimulation.springForce = 100.0f;        // Spring force for Spring Joints
            ligamentSimulation.springDamper = 1000.0f;      // Spring damper for Spring Joints
            ligamentSimulation.segmentSpacing = 0.1f;       // The spacing between segments
            Debug.Log("ģ���ʼ���ɹ�");

            Has_Load = true;

            if (scriptA == null)
            {
                if (ligamentSimulator != null)
                {
                    scriptA = ligamentSimulator.GetComponent<LigamentSimulation>();
                }
            }
        }
    }


    // �ַ����������
#if UNITY_EDITOR
    string Substr(string path)
    {
        int lastIndex = path.LastIndexOf('/');
        string lastPart = lastIndex != -1 ? path.Substring(lastIndex + 1) : path;

        // ȥ����֮�������
        int dotIndex = lastPart.IndexOf('.');
        string finalPart = dotIndex != -1 ? lastPart.Substring(0, dotIndex) : lastPart;

        return finalPart;
    }
#elif UNITY_STANDALONE_WIN
    string Substr(string path)
    {
        int lastIndex = path.LastIndexOf('\\');
        string lastPart = lastIndex != -1 ? path.Substring(lastIndex + 1) : path;

        // ȥ����֮�������
        int dotIndex = lastPart.IndexOf('.');
        string finalPart = dotIndex != -1 ? lastPart.Substring(0, dotIndex) : lastPart;

        return finalPart;
    }
#endif
    Vector3 ParseVector3(string input)
    {
        // ȥ�������ַ����е�����
        input = input.Trim(new char[] { '(', ')', ' ', '\t', '\n', '\r' });
        input = input.Trim('(', ')');
        // ���ݶ��ŷָ��ַ���
        string[] splitInput = input.Split(',');
        // �ֱ����x, y, z����
        float x = float.Parse(splitInput[0]);
        float y = float.Parse(splitInput[1]);
        float z = float.Parse(splitInput[2]);
        // ����һ���µ�Vector3���󲢷���
        return new Vector3(x, y, z);
    }

    // �ű�ѡ�����
    void OpenFile_FileSelectButton1(ref bool buttonClicked)
    {
        buttonClicked = true; // ��ǵ�ǰ��ťΪ�ѵ��
        StartCoroutine(WindowsFileBrowser.OpenFolder("Select Folder", "",FileSelectedCallback_FileSelectButton1));
    }

    void OpenFile_FileSelectButton2(ref bool buttonClicked)
    {
        buttonClicked = true; // ��ǵ�ǰ��ťΪ�ѵ��
        StartCoroutine(WindowsFileBrowser.OpenFolder("Select Folder", "",FileSelectedCallback_FileSelectButton2));
    }

    void FileSelectedCallback_FileSelectButton1(bool success, string path)
    {
        if (success)    Debug.Log(path + "����ѡ��ɹ�");
        script_path_1 = path;  // �洢�ļ�1·��
    }

    void FileSelectedCallback_FileSelectButton2(bool success, string path)
    {
        if (success) Debug.Log(path + "����ѡ��ɹ�");
        script_path_2 = path;  // �洢�ļ�1·��
    }

    
    // ����ģ�����
    void Model_Set(GameObject model,string bonename,Vector3 scaleFactor, Vector3 rotationAngles, Vector3 newPosition)
    {
        model.name = bonename;
        model.transform.parent = null;
        model.transform.localScale = scaleFactor;
        model.transform.eulerAngles = rotationAngles;
        model.transform.position = newPosition;
        // ��ȡGameObject��MeshFilter���
        MeshCollider meshCollider = model.AddComponent<MeshCollider>();
        // ����MeshCollider��͹������
        meshCollider.convex = true;
    }

    void OpenFile_BoneModelButton1(ref bool buttonClicked)
    {
        buttonClicked = true; // ��ǵ�ǰ��ťΪ�ѵ��
        StartCoroutine(WindowsFileBrowser.OpenFile("Open", null, "Text file", new[] { ".fbx" }, FileSelectedCallback_BoneModelButton1));
    }

    void OpenFile_BoneModelButton2(ref bool buttonClicked)
    {
        buttonClicked = true; // ��ǵ�ǰ��ťΪ�ѵ��
        StartCoroutine(WindowsFileBrowser.OpenFile("Open", null, "Text file", new[] { ".fbx" }, FileSelectedCallback_BoneModelButton2));
    }

    void FileSelectedCallback_BoneModelButton1(bool success, string path, byte[] data)
    {
        model_path_1 = path;  // �洢�ļ�1·��
    }

    void FileSelectedCallback_BoneModelButton2(bool success, string path, byte[] data)
    {
        model_path_2 = path;  // �洢�ļ�2·��
    }

}
