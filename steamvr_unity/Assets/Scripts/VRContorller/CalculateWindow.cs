using UnityEngine;
public class CalculateWindow : MonoBehaviour
{
    private Vector2 windowOffset = new Vector2(0.2f, 0.2f);//偏移量
    public TextAnchor windowAnchor = TextAnchor.LowerCenter;//偏移类型
    private Transform window;//跟随的窗体
    public Transform cameraTransform;//相机
    private float windowFollowSpeed = 200f;//跟随速度
    public float distance = 0.5f;//跟随的距离
    // Start is called before the first frame update
    protected void Awake()
    {
        window = transform;
        if (cameraTransform == null) cameraTransform = Camera.main.transform;
    }

    private void OnEnable()
    {
        resetWindowPositon();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        setWindowPositon();
    }

    private void resetWindowPositon()
    {
        window.position = CalculateWindowPosition(cameraTransform);
        window.rotation = cameraTransform.rotation;
    }

    private void setWindowPositon()
    {
        float t = Time.deltaTime * windowFollowSpeed;
        window.position = Vector3.Lerp(window.position, CalculateWindowPosition(cameraTransform), t);
        window.rotation = Quaternion.Slerp(window.rotation, cameraTransform.rotation, t);
    }

    private Vector3 CalculateWindowPosition(Transform cameraTransform)
    {
        float windowDistance = Mathf.Max(16.0f / Camera.main.fieldOfView, Camera.main.nearClipPlane + distance);
        Vector3 position = cameraTransform.position + (cameraTransform.forward * windowDistance);
        Vector3 horizontalOffset = cameraTransform.right * windowOffset.x;
        Vector3 verticalOffset = cameraTransform.up * windowOffset.y;

        switch (windowAnchor)
        {
            case TextAnchor.UpperLeft: position += verticalOffset - horizontalOffset; break;
            case TextAnchor.UpperCenter: position += verticalOffset; break;
            case TextAnchor.UpperRight: position += verticalOffset + horizontalOffset; break;
            case TextAnchor.MiddleLeft: position -= horizontalOffset; break;
            case TextAnchor.MiddleRight: position += horizontalOffset; break;
            case TextAnchor.LowerLeft: position -= verticalOffset + horizontalOffset; break;
            case TextAnchor.LowerCenter: position -= verticalOffset; break;
            case TextAnchor.LowerRight: position -= verticalOffset - horizontalOffset; break;
        }

        return position;
    }
}
