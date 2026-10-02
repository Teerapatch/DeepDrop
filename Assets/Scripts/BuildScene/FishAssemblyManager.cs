using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; // สำคัญ: ต้องเรียกใช้ TextMeshPro

public class FishAssemblyManager : MonoBehaviour
{
    [Header("Preview Images")]
    public Image energyPreview;
    public Image movementPreview;
    public Image utilityPreview;

    [Header("Part Sprites (0=Shrimp, 1=Bivalve, 2=Bacteria)")]
    public Sprite[] energySprites;
    public Sprite[] movementSprites;
    public Sprite[] utilitySprites;

    [Header("Descriptions (TextMeshPro)")]
    public TextMeshProUGUI energyText;
    public TextMeshProUGUI movementText;
    public TextMeshProUGUI utilityText;

    // ==========================================
    // ชุดข้อความอธิบายความสามารถ (แก้ไขข้อความตรงนี้ได้เลย)
    // ==========================================
    private string[] energyDescriptions = {
        "",
        "<color=#FF8C00></color> Increase Stamina",
        "<color=#8B4513></color> Increase Food",
        "<color=#32CD32></color> Decrease Stamina Drain"
    };

    private string[] movementDescriptions = {
        "",
        "<color=#FF8C00></color> Increase Fish Speed",
        "<color=#8B4513></color> Decrease Fish Speed",
        "<color=#32CD32></color> High inertia"
    };

    private string[] utilityDescriptions = {
        "",
        "<color=#FF8C00></color> Have magnet to eat food",
        "<color=#8B4513></color> Cannot be Knockback",
        "<color=#32CD32></color> Has a shield to protects"
    };

    void Start()
    {
        // อัปเดตภาพและข้อความให้ตรงกับค่าเริ่มต้นตอนเปิดหน้ามาครั้งแรก
        UpdatePreviews();
    }

    public void SetEnergyCore(int speciesIndex)
    {
        FishDNA.energyCore = speciesIndex;
        UpdatePreviews();
    }

    public void SetMovementTail(int speciesIndex)
    {
        FishDNA.movementTail = speciesIndex;
        UpdatePreviews();
    }

    public void SetUtilityAppendage(int speciesIndex)
    {
        FishDNA.utilityAppendage = speciesIndex;
        UpdatePreviews();
    }

    // อัปเดตทั้งภาพบนหน้าจอและข้อความ
    private void UpdatePreviews()
    {
        // อัปเดตภาพชิ้นส่วน
        if (energySprites.Length > 0) energyPreview.sprite = energySprites[FishDNA.energyCore];
        if (movementSprites.Length > 0) movementPreview.sprite = movementSprites[FishDNA.movementTail];
        if (utilitySprites.Length > 0) utilityPreview.sprite = utilitySprites[FishDNA.utilityAppendage];

        // อัปเดตข้อความ Description (ดึงข้อความจาก Array มาแสดงตามหมายเลข DNA)
        if (energyText != null) energyText.text = energyDescriptions[FishDNA.energyCore];
        if (movementText != null) movementText.text = movementDescriptions[FishDNA.movementTail];
        if (utilityText != null) utilityText.text = utilityDescriptions[FishDNA.utilityAppendage];
    }

    public void StartGame()
    {
        // โหลดเข้าเกมจริง (ถ้ากลับไปทำ Tutorial เมื่อไหร่ค่อยเปลี่ยนชื่อในนี้ครับ)
        SceneManager.LoadScene("Envi_Stage01");
    }
}