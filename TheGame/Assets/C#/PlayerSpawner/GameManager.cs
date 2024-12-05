using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public void GameOver()
    {
        // Eventuelt vis en "Game Over"-sk�rm her
        // For nu genstarter vi scenen

        Debug.Log("Game Over! Genstarter spillet...");

        // Genstart den aktuelle scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
