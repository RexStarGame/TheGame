using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public GameObject ammoUIContainer;
    public TextMeshProUGUI ammoText;
    public GameObject reloadPrompt; // Tilf�j reference til reloadPrompt

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}