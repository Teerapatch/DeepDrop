using UnityEngine.Android;
using UnityEngine;

public class FishGestureController : MonoBehaviour
{
    [Header("UI Tracking Indicators")]
    public RectTransform headIcon; // ลาก HeadIcon มาใส่
    public RectTransform tailIcon; // ลาก TailIcon มาใส่
    public bool showIndicators = true; // เปิด/ปิดการแสดงผล
    public float uiLerpSpeed = 100f;

    [Header("Dependencies")]
    public MediaPipeUdpReceiver udpReceiver;

    [Header("Movement Settings")]
    public float baseMoveSpeed = 5f;
    public float rotationSpeed = 5f;
    private float speedModifier;
    private float rotateModifer;

    [Header("Scaling Settings")]
    public float smallSizeMultiplier = 0.75f;
    public float mediumSizeMultiplier = 1.00f;
    public float largeSizeMultiplier = 1.25f;
    public float scaleLerpSpeed = 3f;

    [Header("Coordinate Adjustments")]
    public bool invertX = false;
    public bool invertY = false;
    public float modelAngleOffset = 0f; 
    
    [Header("Timeout")]
    public float trackingTimeout = 1.0f; // Force tracking lost if Python crashes

    private float currentSpeed = 0f;
    private Vector3 targetScale;
    private Vector3 initialBaseScale;

    void Start()
    {
        initialBaseScale = transform.localScale;
        targetScale = initialBaseScale * mediumSizeMultiplier;
        speedModifier = smallSizeMultiplier;
        
    }

    void Update()
    {
        if (udpReceiver == null) return;

        MediaPipeData data = udpReceiver.GetLatestData();
        if (data == null) return;

        bool isTrackingLostLocal = (Time.time - udpReceiver.lastPacketTime) > trackingTimeout;
        
        bool isTrackingActive = data.tracking && !isTrackingLostLocal;
        string activeControlStatus = isTrackingLostLocal ? "TRACKING_LOST" : data.controlStatus;

        HandleSize(data, isTrackingActive);
        HandleMovementAndRotation(data, isTrackingActive, activeControlStatus);
        HandleUIIndicators(data, isTrackingActive);

        Debug.Log($"Distance: {data.handDistance}");
    }

    private void HandleSize(MediaPipeData data, bool isTrackingActive)
    {
        //if (isTrackingActive)
        //{
        //    switch (data.fishSize)
        //    {
        //        case "SMALL":
        //            targetScale = initialBaseScale * smallSizeMultiplier;
        //            break;
        //        case "MEDIUM":
        //            targetScale = initialBaseScale * mediumSizeMultiplier;
        //            break;
        //        case "LARGE":
        //            targetScale = initialBaseScale * largeSizeMultiplier;
        //            break;
        //    }
        //}
        if (data.handDistance <= 0.4)
        {
            speedModifier = smallSizeMultiplier;
            rotateModifer = largeSizeMultiplier;
        }
        else if (data.handDistance > 0.4 && data.handDistance <= 0.6)
        {
            speedModifier = mediumSizeMultiplier;
            rotateModifer = mediumSizeMultiplier;
        }
        else if (data.handDistance > 0.6)
        {
            speedModifier = largeSizeMultiplier;
            rotateModifer = smallSizeMultiplier;
        }

        Debug.Log($"speedMod is now: {speedModifier}");
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * scaleLerpSpeed);
    }

    private void HandleMovementAndRotation(MediaPipeData data, bool isTrackingActive, string controlStatus)
    {
        if (isTrackingActive && (controlStatus == "VALID" || controlStatus == "HOLDING"))
        {
            // Accelerate
            float distanceSpeed = baseMoveSpeed * speedModifier;
            float distanceRotation = rotationSpeed * rotateModifer;
            currentSpeed = Mathf.Lerp(currentSpeed, distanceSpeed, Time.deltaTime * 2f);

            // Calculate Angle
            float dirX = data.directionX;
            float dirY = data.directionY;

            if (invertX) dirX = -dirX;
            if (invertY) dirY = -dirY;

            // Atan2 gives angle in radians from X axis. Convert to degrees.
            float targetAngle = Mathf.Atan2(dirY, dirX) * Mathf.Rad2Deg;
            
            // Apply Model offset
            targetAngle += modelAngleOffset;

            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);

            // Slerp rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * distanceRotation);
        }
        else
        {
            // Decelerate smoothly when tracking is lost or invalid gesture
            currentSpeed = Mathf.Lerp(currentSpeed, 0f, Time.deltaTime * 3f);
        }

        // Move forward locally
        //transform.Translate(Vector3.right * currentSpeed * Time.deltaTime, Space.Self);
        transform.Translate(Vector3.up * currentSpeed * Time.deltaTime, Space.Self);
    }
    private void HandleUIIndicators(MediaPipeData data, bool isTrackingActive)
    {
        // ถ้าปิดการแสดงผล หรือ กล้องหามือไม่เจอ ให้ซ่อนไอคอน
        if (!showIndicators || !isTrackingActive || headIcon == null || tailIcon == null)
        {
            if (headIcon != null) headIcon.gameObject.SetActive(false);
            if (tailIcon != null) tailIcon.gameObject.SetActive(false);
            return;
        }

        // เปิดการแสดงผลไอคอน
        headIcon.gameObject.SetActive(true);
        tailIcon.gameObject.SetActive(true);

        /* 
         * หมายเหตุสำคัญ: 
         * ต้องเช็คในคลาส MediaPipeData ของคุณว่าส่งค่าพิกัด x, y ของมือซ้าย/ขวา มาในชื่อตัวแปรอะไร
         * สมมติว่าตั้งชื่อไว้เป็น headX, headY, tailX, tailY และมีค่าตั้งแต่ 0.0 ถึง 1.0 
         */

        // 1. ดึงพิกัด Normalized จาก Python (สมมติชื่อตัวแปรตามนี้ โปรดแก้ให้ตรงกับของคุณ)
        float hX = data.headX;
        float hY = data.headY;
        float tX = data.tailX;
        float tY = data.tailY;

        // ถ้าคุณตั้งค่า Invert แกนซ้ายขวา/บนล่าง ไว้ใน Inspector ให้กลับด้าน UI ด้วย
        if (invertX) { hX = 1f - hX; tX = 1f - tX; }
        if (invertY) { hY = 1f - hY; tY = 1f - tY; }

        // 2. แปลงค่า 0.0-1.0 ให้เป็น Pixel หน้าจอ Unity
        // ข้อควรระวัง: MediaPipe แกน Y ชี้ลงพื้น แต่ Unity UI แกน Y ชี้ขึ้นฟ้า จึงต้องเอา 1.0f ไปลบเสมอ
        Vector2 headScreenPos = new Vector2(hX * UnityEngine.Screen.width, (1f - hY) * UnityEngine.Screen.height);
        Vector2 tailScreenPos = new Vector2(tX * UnityEngine.Screen.width, (1f - tY) * UnityEngine.Screen.height);

        // 3. สั่งย้ายตำแหน่ง UI
        headIcon.position = headScreenPos;
        tailIcon.position = tailScreenPos;
        //headIcon.position = Vector2.Lerp(headIcon.position, headScreenPos, Time.deltaTime * uiLerpSpeed);
        //tailIcon.position = Vector2.Lerp(tailIcon.position, tailScreenPos, Time.deltaTime * uiLerpSpeed);
    }
}
