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

    [Header("Passive Skills")]
    public float knockbackResistance = 0f; // 0 = กระเด็นเต็มๆ, 1 = ไม่กระเด็นเลย
    public bool hasSlimeShield = false;
    public bool isShieldActive = false;

    [Header("Audio")]
    public AudioClip eatSound; // ลากไฟล์เสียงงับอาหารมาใส่ช่องนี้

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
        UpdateHungerUI();

        // ถ้าหิวตาย ให้เรียก GameManager จบเกมพร้อมส่งค่า false (แพ้)
        if (currentHunger <= 0)
        {
            GameManager.Instance.EndGame(false);
        }
    }

    public void EatFood(float nutrition)
    {
        if (eatSound != null)
        {
            AudioManager.Instance.PlaySFX(eatSound);
        }

        currentHunger += nutrition;
        currentHunger = Mathf.Clamp(currentHunger, 0, maxHunger);
        UpdateHungerUI();

        foodEatenCount++; // บวกค่าทีละ 1
        UpdateScoreUI();  // สั่งอัปเดตตัวหนังสือบนหน้าจอ

        Debug.Log("ง่ำ! กินไปแล้ว: " + foodEatenCount + " ชิ้น");
    }
    public void TakeDamage(float damage, Vector3 knockbackDir, float force)
    {
        // เช็คว่ามีโล่แบคทีเรียหรือไม่
        if (hasSlimeShield && isShieldActive)
        {
            isShieldActive = false;
            Debug.Log("🛡️ โล่เมือกป้องกันดาเมจไว้ได้!");
            Invoke("RechargeShield", 5f); // สั่งชาร์จโล่ใหม่ใน 5 วินาที
            return; // จบการทำงาน หินแตกไปฟรีๆ โดยไม่เสียเลือด
        }

        currentHunger -= damage;
        currentHunger = Mathf.Clamp(currentHunger, 0, maxHunger);
        UpdateHungerUI();

        // คำนวณแรงกระเด็นหักลบกับค่าต้านทาน (ถ้าเป็นหอย = 1 จะคูณ 0 ทำให้ไม่กระเด็น)
        knockbackVelocity = knockbackDir.normalized * (force * (1f - knockbackResistance));
    }
    void RechargeShield()
    {
        isShieldActive = true;
        Debug.Log("🟢 โล่เมือกชาร์จเต็ม พร้อมใช้งาน!");
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Food Eaten: " + foodEatenCount;
        }
    }

    // ฟังก์ชันใหม่สำหรับคำนวณและแสดงผลหลอดความหิว
    public void UpdateHungerUI()
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