using System.Collections;
using UnityEngine;
using TMPro;

public class MissTextController : MonoBehaviour
{
    private TextMeshProUGUI missText;
    private CanvasGroup canvasGroup;

    void Start()
    {
        missText = GetComponent<TextMeshProUGUI>();
        if (missText == null)
        {
            Debug.LogError("TextMeshProUGUI komponent ikke fundet på MissText GameObject!");
        }
        else
        {
            Debug.Log("TextMeshProUGUI komponent fundet.");
        }
        missText.text = "";

        // Tilføj en CanvasGroup komponent for at kontrollere gennemsigtighed
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
    }

    public void ShowMissText()
    {
        Debug.Log("ShowMissText() kaldt");
        missText.text = "Miss!";
        StartCoroutine(FadeOutText());
    }
    IEnumerator FadeOutText()
    {
        canvasGroup.alpha = 1;

        yield return new WaitForSeconds(1f); // Vent før fade-out starter

        float fadeDuration = 1f;
        float fadeSpeed = 1 / fadeDuration;

        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= fadeSpeed * Time.deltaTime;
            yield return null;
        }

        missText.text = "";
    }
}
