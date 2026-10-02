using UnityEngine;
using UnityEngine.EventSystems; // ต้องใช้ตัวนี้สำหรับการตรวจจับเมาส์บน UI

// ใส่ Interface เข้าไปหลัง MonoBehaviour เพื่อให้มันดักจับการคลิกและชี้เมาส์ได้อัตโนมัติ
public class UIButtonSound : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [Header("UI Sounds")]
    public AudioClip clickSound; // ลากเสียงตอนกดปุ่มมาใส่
    public AudioClip hoverSound; // ลากเสียงตอนเอาเมาส์ไปวางมาใส่ (ถ้าไม่มีก็เว้นว่างไว้ได้)

    // ฟังก์ชันนี้จะทำงานอัตโนมัติเมื่อ "เอาเมาส์ไปชี้ที่ปุ่ม" (Hover)
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverSound != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(hoverSound);
        }
    }

    // ฟังก์ชันนี้จะทำงานอัตโนมัติเมื่อ "คลิกที่ปุ่ม" (Click)
    public void OnPointerClick(PointerEventData eventData)
    {
        if (clickSound != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(clickSound);
        }
    }
}