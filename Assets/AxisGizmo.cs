using UnityEngine;

public class WorldAxis : MonoBehaviour
{
    public Vector3 origin = new Vector3(0, -6.5f, -0.5f);
    public float axisLength = 5000f;
    public Vector3 groundSize = new Vector3(5000, 0.1f, 5000); // 地面大小
    public Color groundColor = Color.black; // 地面颜色

    void OnDrawGizmos()
    {
        // 绘制正方向坐标轴
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
        // 创建地面，位置在原点，扩展到正方向的X和Z轴
        GameObject groundPlane = GameObject.CreatePrimitive(PrimitiveType.Cube);
        groundPlane.transform.position = new Vector3(origin.x + groundSize.x / 2, origin.y - groundSize.y / 2, origin.z + groundSize.z / 2);
        groundPlane.transform.localScale = groundSize;

        // 设置地面颜色
        Renderer renderer = groundPlane.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = groundColor;
        }
    }

    void CreateAxisLines()
    {
        // 创建X轴
        GameObject xAxis = GameObject.CreatePrimitive(PrimitiveType.Cube);
        xAxis.transform.position = origin + new Vector3(axisLength / 2, 0, 0);
        xAxis.transform.localScale = new Vector3(axisLength, 0.1f, 0.1f);
        xAxis.GetComponent<Renderer>().material.color = Color.red;
        xAxis.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        // 创建Y轴
        GameObject yAxis = GameObject.CreatePrimitive(PrimitiveType.Cube);
        yAxis.transform.position = origin + new Vector3(0, axisLength / 2, 0);
        yAxis.transform.localScale = new Vector3(0.1f, axisLength, 0.1f);
        yAxis.GetComponent<Renderer>().material.color = Color.green;
        yAxis.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        // 创建Z轴
        GameObject zAxis = GameObject.CreatePrimitive(PrimitiveType.Cube);
        zAxis.transform.position = origin + new Vector3(0, 0, axisLength / 2);
        zAxis.transform.localScale = new Vector3(0.1f, 0.1f, axisLength);
        zAxis.GetComponent<Renderer>().material.color = Color.blue;
        zAxis.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
    }
}
