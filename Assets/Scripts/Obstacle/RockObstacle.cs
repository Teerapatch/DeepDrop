using UnityEngine;

public class RockObstacle : MonoBehaviour
{
    [Header("Rock Settings")]
    public float damageAmount = 15f;
    public float knockbackForce = 15f;
    public float fallSpeed = 25f; // เพิ่มความเร็วขึ้นหน่อยเพราะระยะทางทะแยงจะไกลกว่าตกตรงๆ       
    [HideInInspector] public Vector3 targetPos; // รับพิกัดจุดตกเป๊ะๆ มาจาก Spawner

    [Header("Visual Effects (VFX)")]
    public GameObject shadowPrefab;
    public GameObject hitParticlePrefab;

    [Header("Shadow Scaling")]
    public Vector3 minShadowScale = new Vector3(0.5f, 0.5f, 0.5f);
    public Vector3 maxShadowScale = new Vector3(3f, 3f, 3f);

    private GameObject currentShadow;
    private float totalDistance;
    private Vector3 fallDirection;

    void Start()
    {
        // คำนวณทิศทางให้หินพุ่งจากหน้ากล้อง ทะแยงลงไปหาเป้าหมาย
        fallDirection = (targetPos - transform.position).normalized;

        // เก็บระยะทางทั้งหมดไว้ใช้คำนวณการขยายของเงา
        totalDistance = Vector3.Distance(transform.position, targetPos);

        if (shadowPrefab != null)
        {
            // วางเงาไว้ที่เป้าหมายบนระนาบเดียวกับปลา
            currentShadow = Instantiate(shadowPrefab, targetPos, Quaternion.identity);
            currentShadow.transform.localScale = minShadowScale;
        }
    }

    void Update()
    {
        // 1. หินพุ่งทะแยงมุมแบบ 3D
        transform.Translate(fallDirection * fallSpeed * Time.deltaTime, Space.World);

        // ระยะทางปัจจุบันที่หินอยู่ห่างจากเป้าหมาย
        float currentDistance = Vector3.Distance(transform.position, targetPos);

        // 2. ขยายขนาดเงาตามระยะความใกล้
        if (currentShadow != null)
        {
            float t = 1f - Mathf.Clamp01(currentDistance / totalDistance);
            currentShadow.transform.localScale = Vector3.Lerp(minShadowScale, maxShadowScale, t);
        }

        // 3. เช็คว่าหินตกทะลุระนาบแกน Z ของปลาไปหรือยัง (ถ้าแกน Z ของหินมากกว่าเป้าหมายแปลว่าตกถึงพื้นแล้ว)
        if (transform.position.z >= targetPos.z || currentDistance <= 0.2f)
        {
            ShatterRock();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FishController fish = other.GetComponent<FishController>();
            if (fish != null)
            {
                // ผลักปลากระเด็นเฉพาะแกน X, Y
                Vector3 knockbackDir = other.transform.position - targetPos; // ใช้ targetPos เป็นจุดกึ่งกลางแรงระเบิด
                knockbackDir.z = 0;
                fish.TakeDamage(damageAmount, knockbackDir, knockbackForce);
            }

            ShatterRock();
        }
    }

    void ShatterRock()
    {
        if (hitParticlePrefab != null)
        {
            // สร้างฝุ่นตรงจุดตก (ระนาบปลา)
            Instantiate(hitParticlePrefab, targetPos, Quaternion.identity);
        }

        if (currentShadow != null)
        {
            Destroy(currentShadow);
        }

        Destroy(gameObject);
    }
}