using UnityEngine;

public class WorldAxis : MonoBehaviour
{
    public Vector3 origin = new Vector3(0, -6.5f, -0.5f);
    public float axisLength = 5000f;
    public Vector3 groundSize = new Vector3(5000, 0.1f, 5000); // �����С
    public Color groundColor = Color.black; // ������ɫ

    void OnDrawGizmos()
    {
        // ����������������
        Gizmos.color = Color.red;
        Gizmos.DrawLine(origin, origin + Vector3.right * axisLength);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(origin, origin + Vector3.up * axisLength);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(origin, origin + Vector3.forward * axisLength);
    }

    void Start()
    {
        CreateGroundPlane();
        CreateAxisLines();
    }

    void CreateGroundPlane()
    {
        // �������棬λ����ԭ�㣬��չ���������X��Z��
        GameObject groundPlane = GameObject.CreatePrimitive(PrimitiveType.Cube);
        groundPlane.transform.position = new Vector3(origin.x + groundSize.x / 2, origin.y - groundSize.y / 2, origin.z + groundSize.z / 2);
        groundPlane.transform.localScale = groundSize;

        // ���õ�����ɫ
        Renderer renderer = groundPlane.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = groundColor;
        }
    }

    void CreateAxisLines()
    {
        // ����X��
        GameObject xAxis = GameObject.CreatePrimitive(PrimitiveType.Cube);
        xAxis.transform.position = origin + new Vector3(axisLength / 2, 0, 0);
        xAxis.transform.localScale = new Vector3(axisLength, 0.1f, 0.1f);
        xAxis.GetComponent<Renderer>().material.color = Color.red;
        xAxis.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        // ����Y��
        GameObject yAxis = GameObject.CreatePrimitive(PrimitiveType.Cube);
        yAxis.transform.position = origin + new Vector3(0, axisLength / 2, 0);
        yAxis.transform.localScale = new Vector3(0.1f, axisLength, 0.1f);
        yAxis.GetComponent<Renderer>().material.color = Color.green;
        yAxis.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        // ����Z��
        GameObject zAxis = GameObject.CreatePrimitive(PrimitiveType.Cube);
        zAxis.transform.position = origin + new Vector3(0, 0, axisLength / 2);
        zAxis.transform.localScale = new Vector3(0.1f, 0.1f, axisLength);
        zAxis.GetComponent<Renderer>().material.color = Color.blue;
        zAxis.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
    }
}
