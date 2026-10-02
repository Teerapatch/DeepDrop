using UnityEngine;

public class FishPassiveManager : MonoBehaviour
{
    private FishController fishStats;
    private FishGestureController fishMove;

    [Header("Magnet Settings (Shrimp)")]
    public float magnetRadius = 5f;
    public float magnetSpeed = 8f;
    private bool hasMagnet = false;

    void Start()
    {
        fishStats = GetComponent<FishController>();
        fishMove = GetComponent<FishGestureController>();

        ApplyDNA(); // เรียกใช้ตอนเริ่มเกม
    }

    void ApplyDNA()
    {
        // ==========================================
        // 1. หมวดพลังงาน (Energy Core)
        // ==========================================
        int energy = FishDNA.energyCore;
        if (energy == 1) // 0 = กุ้ง (หลอดพลังงาน 2 เท่า)
        {
            fishStats.maxHunger = 200f;
            fishStats.currentHunger = 200f;
        }
        else if (energy == 3) // 2 = แบคทีเรีย (ไม่หิวเลย / อมตะในบ่อเกลือ)
        {
            fishStats.hungerDrainRate = 2f;
        }

        // ==========================================
        // 2. หมวดเคลื่อนที่ (Movement Tail)
        // ==========================================
        int move = FishDNA.movementTail;
        if (move == 1) // 0 = กุ้ง (ว่ายเร็ว เลี้ยวไว)
        {
            fishMove.baseMoveSpeed *= 1.5f;
            fishMove.rotationSpeed *= 1.5f;
        }
        else if (move == 2) // 1 = หอย (ช้า หนืด)
        {
            fishMove.baseMoveSpeed *= 0.8f;
            fishMove.rotationSpeed *= 0.5f;
        }
        else if (move == 3) // 2 = แบคทีเรีย (แรงเฉื่อยสูง เบรกยาก ไหลไปได้ไกล)
        {
            fishMove.decelerationRate = 0.5f; // จากปกติ 3f ลดเหลือ 0.5f ทำให้ลื่นปรื๊ด
        }

        // ==========================================
        // 3. หมวดระยางค์ (Utility Appendage)
        // ==========================================
        int utility = FishDNA.utilityAppendage;
        if (utility == 1) // 0 = กุ้ง (แม่เหล็กดูดอาหาร)
        {
            hasMagnet = true;
        }
        else if (utility == 2) // 1 = หอย (กันกระเด็น 100%)
        {
            fishStats.knockbackResistance = 1f;
        }
        else if (utility == 3) // 2 = แบคทีเรีย (โล่กันดาเมจ 1 ฮิต)
        {
            fishStats.hasSlimeShield = true;
            fishStats.isShieldActive = true;
        }
    }

    void Update()
    {
        // ----------------------------------------------------
        // Passive: หอยสองฝา (รีเจนพลังงานเมื่ออยู่ริมจอซ้าย-ขวา)
        // ----------------------------------------------------
        if (FishDNA.energyCore == 1)
        {
            // แปลงพิกัดปลาเป็นพิกัดหน้าจอ (0.0 ถึง 1.0)
            Vector3 screenPos = Camera.main.WorldToViewportPoint(transform.position);

            // ถ้าอยู่ชิดขอบซ้าย (น้อยกว่า 0.15) หรือขอบขวา (มากกว่า 0.85)
            if (screenPos.x < 0.15f || screenPos.x > 0.85f)
            {
                fishStats.currentHunger += 5f * Time.deltaTime; // ค่อยๆ ฮีล
                fishStats.currentHunger = Mathf.Clamp(fishStats.currentHunger, 0, fishStats.maxHunger);
                fishStats.UpdateHungerUI();
            }
        }

        // ----------------------------------------------------
        // Passive: กุ้ง (แม่เหล็กดูดอาหารที่อยู่ใกล้ๆ)
        // ----------------------------------------------------
        if (hasMagnet)
        {
            // ยิงรัศมีวงกลมค้นหาว่ามีอะไรอยู่ใกล้บ้าง
            Collider[] foods = Physics.OverlapSphere(transform.position, magnetRadius);
            foreach (Collider col in foods)
            {
                if (col.CompareTag("Food"))
                {
                    // ดูดอาหารพุ่งเข้ามาหาตัวปลา
                    col.transform.position = Vector3.MoveTowards(col.transform.position, transform.position, magnetSpeed * Time.deltaTime);
                }
            }
        }
    }

    // เอาไว้วาดเส้นรัศมีแม่เหล็กดูดอาหารในหน้า Scene 
    private void OnDrawGizmos()
    {
        if (hasMagnet)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, magnetRadius);
        }
    }
}