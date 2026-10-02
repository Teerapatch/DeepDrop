using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("SFX Pitch Settings")]
    public float minPitch = 0.85f;
    public float maxPitch = 1.15f;

    void Awake()
    {
        // ตรวจสอบว่ามี AudioManager อยู่ในเกมแล้วหรือยัง
        if (Instance == null)
        {
            Instance = this;

            // คำสั่งนี้จะทำให้ Object นี้ไม่ถูกทำลายเมื่อเปลี่ยน Scene
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // ถ้ามีอยู่แล้ว (เช่น ผู้เล่นกด Retry กลับมาหน้าแรก) ให้ทำลายตัวที่เกิดใหม่ทิ้ง เพื่อไม่ให้มีซ้อนกัน 2 ตัว
            Destroy(gameObject);
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;

        GameObject tempSFX = new GameObject("SFX_" + clip.name);

        // ทำให้เสียงที่ถูกสร้างขึ้นมาใหม่ เกาะอยู่กับ AudioManager ตัวหลัก จะได้ไม่รกหน้าต่าง Hierarchy
        tempSFX.transform.SetParent(transform);

        AudioSource audioSource = tempSFX.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.volume = 1f;
        audioSource.pitch = Random.Range(minPitch, maxPitch);
        audioSource.Play();

        Destroy(tempSFX, clip.length / audioSource.pitch);
    }
}