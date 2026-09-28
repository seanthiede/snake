using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    private Vector3 originalPosition;
    private Coroutine running;

    private void Awake()
    {
        originalPosition = transform.localPosition;
    }

    public void Shake(float duration, float magnitude)
    {
        if (running != null)
        {
            StopCoroutine(running);
        }
        running = StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float strength = magnitude * (1.0f - (elapsed / duration));
            Vector2 offset = Random.insideUnitCircle * strength;

            transform.localPosition = originalPosition + new Vector3(offset.x, offset.y, 0f);
            yield return null;
        }

        transform.localPosition = originalPosition;
        running = null;
    }
}