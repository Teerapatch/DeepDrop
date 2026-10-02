using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game Settings")]
    public float surviveTimeLimit = 120f;
    private float currentTime = 0f;
    private bool isGameOver = false;

    [Header("UI References")]
    public GameObject gameplayUI;
    public GameObject endScreenPanel;
    public TextMeshProUGUI resultTitleText;
    public TextMeshProUGUI triviaText;

    [Header("Timer UI")]
    public TextMeshProUGUI timerText;
    public Color normalTimerColor = Color.white;
    public Color alertTimerColor = Color.red;

    [Header("Audio")]
    public AudioClip win;
    public AudioClip lose;


    void Awake()
    {
        Instance = this;
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (isGameOver) return;

        currentTime += Time.deltaTime;

        float timeLeft = surviveTimeLimit - currentTime;
        UpdateTimerUI(timeLeft);

        // Check win condition: Survived the time limit
        if (currentTime >= surviveTimeLimit)
        {
            EndGame(true);
        }
    }

    void UpdateTimerUI(float timeRemaining)
    {
        if (timerText != null)
        {
            if (timeRemaining < 0) timeRemaining = 0;

            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);

            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

            // Change timer color to red when less than 15 seconds remain
            if (timeRemaining <= 15f)
                timerText.color = alertTimerColor;
            else
                timerText.color = normalTimerColor;
        }
    }

    public void EndGame(bool isWin)
    {
        if (isGameOver) return;
        isGameOver = true;

        // Freeze the game
        Time.timeScale = 0f;

        // Toggle UI panels
        if (gameplayUI != null) gameplayUI.SetActive(false);
        if (endScreenPanel != null) endScreenPanel.SetActive(true);

        if (isWin)
        {
            resultTitleText.text = "Mission Accomplished! Survived for 2 Minutes";
            resultTitleText.color = Color.green;
            if (win != null)
            {
                AudioManager.Instance.PlaySFX(win);
            }
        }
        else
        {
            resultTitleText.text = "Out of Energy! Your fish has starved.";
            resultTitleText.color = Color.red;
            if (lose != null)
            {
                AudioManager.Instance.PlaySFX(lose);
            }
        }

        GenerateTrivia();
    }

    void GenerateTrivia()
    {
        string energyFact = GetEnergyFact(FishDNA.energyCore);
        string moveFact = GetMovementFact(FishDNA.movementTail);
        string utilityFact = GetUtilityFact(FishDNA.utilityAppendage);

        triviaText.text = "Biological Data of Your Created Creature:\n\n" +
                          "Energy: " + energyFact + "\n\n" +
                          "Movement: " + moveFact + "\n\n" +
                          "Adaptation: " + utilityFact;
    }

    string GetEnergyFact(int index)
    {
        if (index == 1) return "The scavenger";
        if (index == 2) return "The bivalve";
        if (index == 3) return "Extremophile bacteria";
        return "What!?";
        //if (index == 1) return "The scavenger shrimp has a heart that pumps blood efficiently in normal water (Zone 1) but will instantly suffocate if it wanders into the anoxic brine pool.";
        //if (index == 2) return "The bivalve uses its gills to filter-feed on biological particles in the water, conserving energy by not having to hunt for prey.";
        //return "Extremophile bacteria utilize sulfate-reducing processes to generate energy, allowing them to survive in completely anoxic environments.";
    }

    string GetMovementFact(int index)
    {
        if (index == 1) return "The scavenger";
        if (index == 2) return "The bivalve";
        if (index == 3) return "Extremophile bacteria";
        return "What!?";
        //if (index == 1) return "The streamlined body and strong tail of the shrimp allow it to dart quickly to evade danger or falling debris.";
        //if (index == 2) return "The thick, heavy shell of the bivalve provides excellent resistance against water currents and allows it to sink rapidly to the bottom.";
        //return "The microbe uses a flagellum to drift slowly with the currents, an adaptation that significantly reduces energy consumption.";
    }

    string GetUtilityFact(int index)
    {
        if (index == 1) return "The scavenger";
        if (index == 2) return "The bivalve";
        if (index == 3) return "Extremophile bacteria";
        return "What!?";
        //if (index == 1) return "The shrimp's exceptionally long claws allow it to reach and pinch food debris floating above the brine layer without risking its life diving into it.";
        //if (index == 2) return "The bivalve possesses strong byssal threads to anchor itself tightly to rocks, preventing it from being swept away by impacts or strong currents.";
        //return "A protective slime shield encapsulates the microbe, shielding it from harsh environments and acting as a shock absorber against falling debris.";
    }

    public void RetryGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Envi_Build");
    }
}