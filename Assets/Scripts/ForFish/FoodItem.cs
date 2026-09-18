using UnityEngine;

public class FoodItem : MonoBehaviour
{
    public float nutritionValue = 20f;
    public float lifeTime = 10f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    // แก้ตรงนี้! เอาคำว่า 2D ออกให้หมด
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<FishController>().EatFood(nutritionValue);
            Destroy(gameObject);
        }
    }
}