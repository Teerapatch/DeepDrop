using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CalibrationManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Text instructionText; // ข้อความสอนเล่น
    public Text countdownText;   // เวลานับถอยหลังของแต่ละบทเรียน
    public GameObject startButton; // ปุ่มเข้าสู่ด่านจริง

    [Header("Tutorial Settings")]
    public float timePerStep = 8f; // ให้เวลาผู้เล่นลองทำท่าละ 8 วินาที
    private float stepTimer;
    private int currentStep = 0;

    void Start()
    {
        startButton.SetActive(false);
        stepTimer = timePerStep;
        UpdateInstruction();
    }

    void Update()
    {
        // ทำงานเฉพาะตอนที่ยังสอนไม่จบ (Step 0 และ 1)
        if (currentStep < 2)
        {
            stepTimer -= Time.deltaTime;
            countdownText.text = "บทเรียนถัดไปใน: " + Mathf.Ceil(stepTimer).ToString() + " วินาที";

            if (stepTimer <= 0)
            {
                currentStep++;
                stepTimer = timePerStep;
                UpdateInstruction();
            }
        }
    }

    void UpdateInstruction()
    {
        switch (currentStep)
        {
            case 0:
                instructionText.text = "บททดสอบที่ 1: การบังคับทิศทาง\nลองกวาดมือของคุณไปทางซ้าย-ขวา\nเพื่อบังคับให้ปลาหันหน้าตาม";
                break;
            case 1:
                instructionText.text = "บททดสอบที่ 2: การเร่งความเร็ว\nลองกางมือออกให้กว้างขึ้นเพื่อ 'เร่งความเร็ว'\nและหุบมือเข้าหากันเพื่อ 'ชะลอความเร็ว'";
                break;
            case 2:
                // สรุปกฎการเล่น
                instructionText.text = "เป้าหมายภารกิจ:\n1. เอาชีวิตรอดในทะเลแดงให้ครบ 2 นาที\n2. ระวังหินถล่ม และกินอาหารอย่าให้หลอดความหิวหมด!\n\nเตรียมตัวให้พร้อม!";
                countdownText.text = "";
                startButton.SetActive(true); // โชว์ปุ่มให้คนคุมบูธ (หรือผู้เล่น) กดเริ่มเกมจริง
                break;
        }
    }

    public void LoadGameplay()
    {
        // โหลดเข้าด่านจริง
        SceneManager.LoadScene("GameplayScene");
    }
}