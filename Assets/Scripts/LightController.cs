using UnityEngine;

public class LightController : MonoBehaviour
{
    [Header("鼠标灵敏度（度 / 鼠标单位）")]
    public float sensitivity = 3f;

    [Header("俯仰限制（度）")]
    public float minPitch = -20f;
    public float maxPitch = 80f;

    [Header("强度（滚轮）")]
    public Light targetLight;
    public float minIntensity = 0.2f;
    public float maxIntensity = 3f;
    public float scrollStep = 2f;

    // 当前朝向：绝对角度，不是增量——这才能存下来、复现
    private float yaw = 180f;
    private float pitch = 30f;

    void Start()
    {
        if (targetLight == null) targetLight = GetComponent<Light>();

        Vector3 e = transform.eulerAngles;
        pitch = Mathf.Clamp(NormalizeAngle(e.x), minPitch, maxPitch);
        yaw = e.y;
        ApplyRotation();
    }

    void Update()
    {
        // 左键拖动改方向（鼠标输入已经是位移，不要再乘 deltaTime）
        if (Input.GetMouseButton(0))
        {
            yaw += Input.GetAxis("Mouse X") * sensitivity;
            pitch -= Input.GetAxis("Mouse Y") * sensitivity;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
            ApplyRotation();
        }

        // 滚轮改强度
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.001f && targetLight != null)
        {
            targetLight.intensity = Mathf.Clamp(
                targetLight.intensity + scroll * scrollStep, minIntensity, maxIntensity);
        }

        // 出图预设：yaw 与"光从哪来"差 180°
        if (Input.GetKeyDown(KeyCode.Alpha1)) SetPose(180f, 30f);  // 顺光：光从相机方向来
        if (Input.GetKeyDown(KeyCode.Alpha2)) SetPose(225f, 25f);  // 侧光 45°：角色右脸受光（画面左侧亮）
        if (Input.GetKeyDown(KeyCode.Alpha3)) SetPose(0f, 15f);    // 逆光：光从角色背后低角度来
        if (Input.GetKeyDown(KeyCode.Alpha4)) SetPose(135f, 25f);  // 镜像侧光：角色左脸受光（T12 用）
    }

    public void SetPose(float newYaw, float newPitch)
    {
        yaw = newYaw;
        pitch = Mathf.Clamp(newPitch, minPitch, maxPitch);
        ApplyRotation();
    }

    void ApplyRotation()
    {
        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    static float NormalizeAngle(float a) => a > 180f ? a - 360f : a;
}