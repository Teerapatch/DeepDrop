using UnityEngine;

public class TutorialWaypoint : MonoBehaviour
{
    public string waypointName; // ตั้งชื่อเช่น "Left", "Right", "Forward"
    public TutorialManager manager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // ส่งชื่อจุดที่ชนไปให้ Manager ตรวจสอบ
            manager.WaypointReached(waypointName);
            gameObject.SetActive(false); // ชนปุ๊บ ซ่อนตัวเองทันที
        }
    }
}