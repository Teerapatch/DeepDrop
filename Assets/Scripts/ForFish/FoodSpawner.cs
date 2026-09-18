using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject foodPrefab;
    public float spawnInterval = 2f; // สุ่มเกิดทุกๆ 2 วินาที

    [Header("Spawn Radius (Around Player)")]
    public Transform playerTransform; // ลากตัวปลา (Player) มาใส่ในช่องนี้
    public float minSpawnRadius = 3f; // ระยะ a: ต้องไกลกว่าระยะนี้
    public float maxSpawnRadius = 8f; // ระยะ b: ห้ามเกิดไกลกว่าระยะนี้

    private float timer;

    void Update()
    {
        // ถ้าไม่มี Player ให้หยุดการสุ่ม (ป้องกัน Error เวลาปลาตายหรือโดนทำลาย)
        if (playerTransform == null) return;

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnFood();
            timer = 0f;
        }
    }

    void SpawnFood()
    {
        // 1. สุ่มทิศทางแบบวงกลม 2D (ได้ค่า Vector2 ที่มีทิศทางชี้ไปแบบสุ่ม 360 องศา)
        Vector2 randomDirection = Random.insideUnitCircle.normalized;

        // 2. สุ่มระยะทางให้อยู่ระหว่างระยะขั้นต่ำ (a) และระยะสูงสุด (b)
        float randomDistance = Random.Range(minSpawnRadius, maxSpawnRadius);

        // 3. คำนวณตำแหน่งเกิด = จุดศูนย์กลาง(ผู้เล่น) + (ทิศทาง * ระยะทาง)
        Vector2 spawnPosition = (Vector2)playerTransform.position + (randomDirection * randomDistance);

        // สร้างอาหารขึ้นมาใน Scene
        Instantiate(foodPrefab, spawnPosition, Quaternion.identity);
    }

    // (Option เสริม) เอาไว้วาดเส้นวงแหวนในหน้า Scene ให้เราเห็นกรอบการเกิดได้ง่ายๆ
    private void OnDrawGizmos()
    {
        if (playerTransform != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(playerTransform.position, minSpawnRadius); // วงใน (a)
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(playerTransform.position, maxSpawnRadius); // วงนอก (b)
        }
    }
}