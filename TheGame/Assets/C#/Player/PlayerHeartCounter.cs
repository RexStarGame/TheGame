using UnityEngine;
using UnityEngine.UI;

public class PlayerHeartCounter : MonoBehaviour
{
    [Header("Heart Settings")]
    public int maxHearts = 3;
    public int currentHearts;
    public Image[] heartImages;
    public Sprite fullHeartSprite;
    public Sprite emptyHeartSprite;

    void Start()
    {
        currentHearts = maxHearts;
        UpdateHeartsUI();
    }

    public void LoseHeart()
    {
        if (currentHearts > 0)
        {
            currentHearts--;
            UpdateHeartsUI();
        }
    }

    public bool HasHeartsLeft()
    {
        return currentHearts > 0;
    }

    void UpdateHeartsUI()
    {
        for (int i = 0; i < heartImages.Length; i++)
        {
            if (i < currentHearts)
            {
                heartImages[i].sprite = fullHeartSprite;
            }
            else
            {
                heartImages[i].sprite = emptyHeartSprite;
            }
        }
    }

    public void ResetHearts()
    {
        currentHearts = maxHearts;
        UpdateHeartsUI();
    }
}
