using UnityEngine;

public class RockSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject rockPrefab;
    public float spawnInterval = 1.5f;

    [Header("Target Tracking (Near Player)")]
    public Transform playerTransform;
    public float spawnRadius = 8f;

    [Header("3D Drop Settings")]
    public float spawnHeightY = 15f; // ความสูงด้านบน (แกน Y)
    public float spawnDepthZ = -20f; // ความลึกพุ่งมาจากหน้ากล้อง (แกน Z ติดลบ = ลอยมาใกล้คนเล่น)

    private float timer;

    void Update()
    {
        if (playerTransform == null) return;

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnRockNearPlayer();
            timer = 0f;
        }
    }

    void SpawnRockNearPlayer()
    {
        // 1. สุ่มพิกัดจุดตก (เงา) รอบๆ ตัวปลา
        Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
        float targetX = playerTransform.position.x + randomCircle.x;
        float targetY = playerTransform.position.y + randomCircle.y;
        float lockedZ = playerTransform.position.z;

        // 2. กำหนดจุดเป้าหมายที่หินต้องพุ่งชนบนระนาบของปลา
        Vector3 targetPos = new Vector3(targetX, targetY, lockedZ);

        // 3. กำหนดจุดเกิดให้ลอยสูงขึ้น และ ลอยออกมาหน้ากล้อง (สร้างมุมทะแยง 3D)
        Vector3 spawnPos = new Vector3(targetX, targetY + spawnHeightY, lockedZ + spawnDepthZ);

        // 4. สร้างหิน
        GameObject newRock = Instantiate(rockPrefab, spawnPos, Random.rotation);

        // 5. ส่งพิกัด "จุดตก" ไปบอกก้อนหิน
        RockObstacle rockScript = newRock.GetComponent<RockObstacle>();
        if (rockScript != null)
        {
            rockScript.targetPos = targetPos;
        }
    }
}