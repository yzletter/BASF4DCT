using Assets.SimpleFileBrowserForWindows;
using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using System.Text;
//using UnityEditor.Search;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class LigamentSimulation : MonoBehaviour
{
    public GameObject bone1;                    // Attachment point on the first bone
    public GameObject bone2;                    // Attachment point on the second bone
    public GameObject model1;
    public GameObject model2;

    public Vector3 vector1;
    public Vector3 vector2;

    public GameObject ligamentSegmentPrefab;    // Prefab for ligament segments
    public int segmentCount = 20;                // Number of segments in the ligament
    public float springForce = 2000.0f;         // Spring force for Spring Joints
    public float springDamper = 2000.0f;        // Spring damper for Spring Joints
    public float segmentSpacing = 0.01f;        // The spacing between segments

    private List<GameObject> segments = new List<GameObject>();
    private LineRenderer lineRenderer;

    private float timer = 0.0f;
    private float Alltimer = 0.0f;
    private bool Saveflag = false;
    private List<string> dataContents = new List<string>();
    private int count = 0;


    public bool start_move = false;

    public float speed = 1f;
    // ����Ƿ񵽴�Ŀ���
    private bool object1Reached = false;
    private bool object2Reached = false;
    private bool have_success1 = false;
    private bool have_success2 = false;

    void Start()
    {
        CreateLigament();
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        // Configure the LineRenderer component
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.positionCount = segmentCount;

        Vector3 SpawnscaleFactor = new Vector3(0.07f, 0.07f, 0.07f);
        bone1.transform.parent = model1.transform;
        bone2.transform.parent = model2.transform;
        bone1.transform.localScale = SpawnscaleFactor;
        bone2.transform.localScale = SpawnscaleFactor;
    }


    void Update()
    {
        // �ƶ�bone1��vector1
        if (!object1Reached)
        {
            float step = speed * Time.deltaTime; // �����ƶ�����
            bone1.transform.localPosition = Vector3.MoveTowards(bone1.transform.localPosition, vector1, step);

            // ���object1�Ƿ񵽴�Ŀ���
            if (Vector3.Distance(bone1.transform.localPosition, vector1) < 0.001f)
            {
                object1Reached = true;
                have_success1 = true;
            }
        }

        // �ƶ�object2��vector2
        if (!object2Reached)
        {
            float step = speed * Time.deltaTime;
            bone2.transform.localPosition = Vector3.MoveTowards(bone2.transform.localPosition, vector2, step);

            // ���object2�Ƿ񵽴�Ŀ���
            if (Vector3.Distance(bone2.transform.localPosition, vector2) < 0.001f)
            {
                object2Reached = true;
                have_success2 = true;
            }
        }

        if (have_success1 && have_success2 && !start_move)
        {
            start_move = true;
        }


        // Update the LineRenderer to follow the segments
        for (int i = 0; i < segmentCount; i++)
        {
            lineRenderer.SetPosition(i, segments[i].transform.position);
        }

        // Calculate and output the current length of the ligament
        float length = 0f;

        if (!Saveflag)
        {
            length = CalculateLigamentLength();
            Debug.Log("Ligament Length: " + length);
        }

        if (have_success1 && have_success2)
        {
            timer += Time.deltaTime;
            Alltimer += Time.deltaTime;

            if (Alltimer > 0.5 && timer >= 1.0f && !Saveflag && count <= 30) // ÿ1���¼һ������
            {
                string dataEntry = count.ToString() + "," + length.ToString();
                timer = 0f;
                dataContents.Add(dataEntry);
                count++;
            }

            if (Alltimer >= 30.5f && !Saveflag && count > 30)
            {
                StartCoroutine(WindowsFileBrowser.SaveFile("Save", "E:\\TestData", "result", "Text file", ".csv", dataContents, FileSavedCallback));
                Saveflag = true;
            }
        }
    }

    void FileSavedCallback(bool success, string path)
    {
        if (success) Debug.Log("savesuccess");
        else Debug.Log("savefailed");
    }

    // version v3
    void CreateLigament()
    {
        // ��ʼ������Ϊ��
        GameObject previousSegment = null;

        // ��������֮��ľ���
        Vector3 bone1Position = bone1.transform.position;
        Vector3 bone2Position = bone2.transform.position;

        // ѭ��������
        for (int i = 0; i < segmentCount; i++)
        {
            // ���㵱ǰ�ε�λ��
            Vector3 position = Vector3.Lerp(bone1Position, bone2Position, (float)i / (segmentCount - 1));
            GameObject segment = Instantiate(ligamentSegmentPrefab, position, Quaternion.identity);

            // ���ǰһ���δ��ڣ���ʹ�õ��ɹؽ�����
            if (previousSegment != null)
            {
                //AttachSpringJoint(segment, previousSegment);
                // ����ʹ�ÿ����ùؽ�
                AttachConfigurableJoint(segment, previousSegment);
            }
            else
            {
                // ����ǵ�һ���Σ����ù̶��ؽ����ӵ�bone1
                AttachFixedJoint(segment, bone1);
            }

            // ��������һ���Σ����ù̶��ؽ����ӵ�bone2
            if (i == segmentCount - 1)
            {
                AttachFixedJoint(segment, bone2);
            }

            // ����ǰ������Ϊǰһ���Σ���������뵽���б�
            previousSegment = segment;
            segments.Add(segment);
        }
    }

    // ��ӹ̶��ؽڵķ���
    void AttachFixedJoint(GameObject segment, GameObject bone)
    {
        FixedJoint fixedJoint = segment.AddComponent<FixedJoint>();
        fixedJoint.connectedBody = bone.GetComponent<Rigidbody>();
    }

    // ��ӿ����ùؽڵķ�����������ɹؽڣ�
    void AttachConfigurableJoint(GameObject segment, GameObject previousSegment)
    {
        ConfigurableJoint joint = segment.AddComponent<ConfigurableJoint>();
        joint.connectedBody = previousSegment.GetComponent<Rigidbody>();
        joint.xMotion = ConfigurableJointMotion.Locked;
        joint.yMotion = ConfigurableJointMotion.Locked;
        joint.zMotion = ConfigurableJointMotion.Locked;
        joint.angularXMotion = ConfigurableJointMotion.Limited;
        joint.angularYMotion = ConfigurableJointMotion.Limited;
        joint.angularZMotion = ConfigurableJointMotion.Limited;

        SoftJointLimitSpring spring = new SoftJointLimitSpring();
        spring.spring = springForce * 1.5f; // ���ӵ��ɳ���
        spring.damper = springDamper * 0.75f; // ƽ������ϵ��
        joint.angularXLimitSpring = spring;
        joint.angularYZLimitSpring = spring;

        SoftJointLimit limit = new SoftJointLimit();
        limit.limit = 10f; // ���ýǶ�����
        joint.lowAngularXLimit = limit;
        joint.highAngularXLimit = limit;
        joint.angularYLimit = limit;
        joint.angularZLimit = limit;
    }

    float CalculateLigamentLength()
    {
        float length = 0f;
        for (int i = 0; i < segments.Count - 1; i++)
        {
            length += Vector3.Distance(segments[i].transform.position, segments[i + 1].transform.position);
        }
        return length;
    }
}