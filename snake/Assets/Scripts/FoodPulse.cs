using UnityEngine;

public class FoodPulse : MonoBehaviour
{
    [SerializeField] private float pulseSpeed = 5f; // how fast
    [SerializeField] private float pulseAmount = 0.15f; // how much

    private Vector3 baseScale;

    private void Awake()
    {
        baseScale = transform.localScale;
    }

    private void Update()
    { 
        float factor = 1 + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
        transform.localScale = baseScale * factor;
    }
}
