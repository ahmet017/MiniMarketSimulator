using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ButtonClickAnimation : MonoBehaviour
{
    public float scaleFactor = 0.9f; // Küçülme oranı
    public float duration = 0.1f; // Animasyon süresi

    public void OnClick()
    {
        StartCoroutine(ScaleButton());
    }

    IEnumerator ScaleButton()
    {
        Vector3 initialScale = transform.localScale;
        Vector3 targetScale = initialScale * scaleFactor;

        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            transform.localScale = Vector3.Lerp(initialScale, targetScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return
 null;
        }

        // Geriye dön
        elapsedTime = 0;
        while (elapsedTime < duration)
        {
            transform.localScale = Vector3.Lerp(targetScale, initialScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return
 null;
        }
    }
}