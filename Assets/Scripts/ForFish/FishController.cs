using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FishController : MonoBehaviour
{
    [Header("Score Settings")]
    public int foodEatenCount = 0; // ตัวแปรเก็บจำนวนอาหารที่กินไป
    public TextMeshProUGUI scoreText; // ช่องสำหรับลาก UI Text มาใส่

    [Header("Hunger Settings")]
    public float maxHunger = 100f;
    public float currentHunger;
    public float hungerDrainRate = 5f;

    [Header("UI Settings")]
    public Image hungerFillImage; // เอาไว้รับค่า UI Image ที่เราเซ็ตเป็น Filled

    [Header("Knockback Settings")]
    private Vector3 knockbackVelocity = Vector3.zero;
    public float knockbackDecay = 5f; // ความเร็วในการเบรกหลังถูกชนกระเด็น

    void Start()
    {
        currentHunger = maxHunger;
        UpdateHungerUI(); // อัปเดต UI ทันทีที่เริ่มเกม
    }

    void Update()
    {
        if (knockbackVelocity.magnitude > 0.1f)
        {
            // ทำให้ปลาขยับตามแรงกระเด็น
            transform.position += knockbackVelocity * Time.deltaTime;
            // ค่อยๆ ลดแรงกระเด็นลงจนเหลือ 0 (เบรก)
            knockbackVelocity = Vector3.Lerp(knockbackVelocity, Vector3.zero, Time.deltaTime * knockbackDecay);
        }


        HandleHunger();
    }

    void HandleHunger()
    {
        currentHunger -= hungerDrainRate * Time.deltaTime;
        currentHunger = Mathf.Clamp(currentHunger, 0, maxHunger);

        UpdateHungerUI(); // อัปเดตหลอดภาพทุกเฟรม

        if (currentHunger <= 0)
        {
            Debug.Log("ปลาหิวตายแล้ว! Game Over");
            // ลอจิก Game Over
        }
    }

    public void EatFood(float nutrition)
    {
        currentHunger += nutrition;
        currentHunger = Mathf.Clamp(currentHunger, 0, maxHunger);
        UpdateHungerUI();

        foodEatenCount++; // บวกค่าทีละ 1
        UpdateScoreUI();  // สั่งอัปเดตตัวหนังสือบนหน้าจอ

        Debug.Log("ง่ำ! กินไปแล้ว: " + foodEatenCount + " ชิ้น");
    }
    public void TakeDamage(float damage, Vector3 knockbackDir, float force)
    {
        // 1. ลดความหิว
        currentHunger -= damage;
        currentHunger = Mathf.Clamp(currentHunger, 0, maxHunger);
        UpdateHungerUI();

        // 2. ออกแรงกระเด็นไปตามทิศทางที่โดนชน
        knockbackVelocity = knockbackDir.normalized * force;

        Debug.Log("โอ๊ย! โดนหินชน พลังงานลดเหลือ: " + currentHunger);
    }
    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Food Eaten: " + foodEatenCount;
        }
    }

    // ฟังก์ชันใหม่สำหรับคำนวณและแสดงผลหลอดความหิว
    void UpdateHungerUI()
    {
        if (hungerFillImage != null)
        {
            // fillAmount รับค่า 0 ถึง 1 เท่านั้น จึงต้องเอา ค่าปัจจุบัน มาหาร ค่าสูงสุด
            hungerFillImage.fillAmount = currentHunger / maxHunger;

            // ลูกเล่นเพิ่มเติม: เปลี่ยนสีหลอดตามค่าความหิว
            if (hungerFillImage.fillAmount > 0.5f)
                hungerFillImage.color = Color.green;
            else if (hungerFillImage.fillAmount > 0.2f)
                hungerFillImage.color = new Color(1f, 0.5f, 0f); // สีส้ม
            else
                hungerFillImage.color = Color.red;
        }
    }
}