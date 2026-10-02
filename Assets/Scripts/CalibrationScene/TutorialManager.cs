using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    public enum TutorialStep { SteerLeft, SteerRight, Accelerate, Rules }
    public TutorialStep currentStep = TutorialStep.SteerLeft;

    [Header("Waypoints (3D Objects)")]
    public GameObject leftWaypoint;
    public GameObject rightWaypoint;
    public GameObject forwardWaypoint; // จุดที่อยู่ไกลๆ บังคับให้ต้องเร่งความเร็วไปหา

    [Header("UI Top Progress (เกจด้านบน)")]
    public Image[] stepGauges; // ลากภาพเกจ 3 ช่องมาใส่ (ตั้งเป็นสีเทาไว้ก่อน)
    public Color completedColor = Color.green; // สีเมื่อทำสเต็ปนั้นผ่าน

    [Header("UI Visual Icons (สัญลักษณ์กลางจอ)")]
    public GameObject steerIcon; // รูปลูกศรซ้ายขวา
    public GameObject accelerateIcon; // รูปกางมือเร่งความเร็ว
    public GameObject rulesIcon; // รูปนาฬิกา 2 นาที + รูปเนื้อ (ห้ามหิวตาย)

    void Start()
    {
        // เริ่มต้น: เปิดแค่เป้าหมายซ้าย และไอคอนเลี้ยว
        ResetAllVisuals();
        leftWaypoint.SetActive(true);
        steerIcon.SetActive(true);
    }

    // ฟังก์ชันนี้จะถูกเรียกจาก TutorialWaypoint
    public void WaypointReached(string waypointName)
    {
        if (currentStep == TutorialStep.SteerLeft && waypointName == "Left")
        {
            // ผ่านเลี้ยวซ้าย -> สั่งให้ไปขวาต่อ
            currentStep = TutorialStep.SteerRight;
            rightWaypoint.SetActive(true);
        }
        else if (currentStep == TutorialStep.SteerRight && waypointName == "Right")
        {
            // ผ่านเลี้ยวขวา (จบหมวดเลี้ยว) -> อัปเดตเกจช่อง 1
            CompleteStepUI(0);

            // เข้าสู่หมวดเร่งความเร็ว
            currentStep = TutorialStep.Accelerate;
            ResetAllVisuals();
            forwardWaypoint.SetActive(true);
            accelerateIcon.SetActive(true);
        }
        else if (currentStep == TutorialStep.Accelerate && waypointName == "Forward")
        {
            // ผ่านหมวดเร่งความเร็ว -> อัปเดตเกจช่อง 2
            CompleteStepUI(1);

            // เข้าสู่หมวดกฎการเล่น
            currentStep = TutorialStep.Rules;
            ResetAllVisuals();
            rulesIcon.SetActive(true);

            // เริ่มนับถอยหลังเข้าเกมจริง
            StartCoroutine(ShowRulesAndStartGame());
        }
    }

    private void CompleteStepUI(int index)
    {
        if (index < stepGauges.Length)
        {
            stepGauges[index].color = completedColor; // เปลี่ยนสีเกจเพื่อบอกว่าผ่านแล้ว
        }
    }

    private void ResetAllVisuals()
    {
        leftWaypoint.SetActive(false);
        rightWaypoint.SetActive(false);
        forwardWaypoint.SetActive(false);

        steerIcon.SetActive(false);
        accelerateIcon.SetActive(false);
        rulesIcon.SetActive(false);
    }

    private IEnumerator ShowRulesAndStartGame()
    {
        // อัปเดตเกจช่องสุดท้ายทันที
        CompleteStepUI(2);

        // โชว์ภาพกฎการเล่นค้างไว้ 4 วินาทีให้ผู้เล่นทำความเข้าใจ
        yield return new WaitForSeconds(4f);

        // เข้าสู่ด่านเล่นจริง
        SceneManager.LoadScene("GameplayScene");
    }
}