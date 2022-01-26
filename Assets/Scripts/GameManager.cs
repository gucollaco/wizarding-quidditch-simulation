using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public float startDelay = 2f;
    public float endDelay = 5f;
    public Team[] teams;
    public TextMeshProUGUI scores;
    public TextMeshProUGUI round;
    public TextMeshProUGUI center;
    public TextMeshProUGUI lastRoundWinner;
    public TextMeshProUGUI winningStreak;
    public GameSettings gameSettings;
    public GameObject snitchTemplate;

    private GameObject snitch = null;
    private GameObject[] wizards;
    private SnitchController snitchController;
    private WaitForSeconds startWait;
    private WaitForSeconds endWait;
    private int roundNumber = 0;
    private Team roundWinner;
    private Team previousRoundWinner;
    private bool hasWinner = false;
    private bool roundFinished = false;
    private int streak = 0;

    private void Start()
    {
        ResetScores();
        ResetTexts();

        startWait = new WaitForSeconds(startDelay);
        endWait = new WaitForSeconds(endDelay);

        StartCoroutine(GameLoop());
    }

    private void SpawnWizards()
    {
        foreach (Team team in teams)
            team.Initialize();
    }

    private void SpawnSnitch()
    {
        snitch = GameObject.Instantiate(snitchTemplate);
        snitchController = snitch.GetComponent<SnitchController>();
        snitchController.OnRoundEnd.AddListener(OnRoundEnd);
    }

    public void OnRoundEnd(Team team)
    {
        previousRoundWinner = roundWinner;
        roundWinner = team;
        snitch.SetActive(false);
        roundFinished = true;
    }

    private void InitializeWizards()
    {
        foreach (Team team in teams)
        {
            team.MoveWizards();
        }
    }

    private void InitializeSnitch()
    {
        snitchController.Initialize();
    }

    private void DestroyWizards()
    {
        foreach (Team team in teams)
        {
            team.DestroyWizards();
        }
    }

    private void DestroySnitch()
    {
        if (snitch != null)
            GameObject.Destroy(snitch);
    }

    private void ResetScores()
    {
        foreach (Team team in teams)
            team.ResetScore();
    }

    private void ResetMatch()
    {
        roundFinished = false;
    }

    private void ResetTexts()
    {
        scores.text = string.Empty;
        round.text = string.Empty;
        center.text = string.Empty;
        lastRoundWinner.text = string.Empty;
        winningStreak.text = string.Empty;
    }

    private void RoundStartingText()
    {
        center.text = $"Round {roundNumber}";
    }

    private void RoundPlayingText()
    {
        center.text = string.Empty;

        foreach (Team team in teams)
            scores.text += $"<color=#{ColorUtility.ToHtmlStringRGB(team.teamTraits.color)}>{team.teamTraits.identifier}: {team.teamTraits.points} points</color>\n";
    
        if (roundNumber > 1)
        {
            lastRoundWinner.text = $"Last Round Winner: <color=#{ColorUtility.ToHtmlStringRGB(roundWinner.teamTraits.color)}>{roundWinner.teamTraits.identifier}</color>";
            winningStreak.text = $"Winning Streak: {streak}";
        }

        round.text = $"Round {roundNumber}";
    }
    
    private void RoundEndingText()
    {
        string message = string.Empty;
        
        if (hasWinner)
        {
            center.text += $"<color=#{ColorUtility.ToHtmlStringRGB(roundWinner.teamTraits.color)}>{roundWinner.teamTraits.identifier}</color> wins the game!\n\n";
            scores.text = string.Empty;
            winningStreak.text = string.Empty;
            lastRoundWinner.text = string.Empty;
            round.text = string.Empty;
        }
        else
            center.text += $"<color=#{ColorUtility.ToHtmlStringRGB(roundWinner.teamTraits.color)}>{roundWinner.teamTraits.identifier}</color> wins the {(hasWinner ? "game" : "round")}!\n\n";
        
        foreach (Team team in teams)
            center.text += $"<color=#{ColorUtility.ToHtmlStringRGB(team.teamTraits.color)}><size=36>{team.teamTraits.identifier}: {team.teamTraits.points} points</size></color>\n";
    }

    private IEnumerator GameLoop()
    {
        yield return StartCoroutine(RoundStarting());
        yield return StartCoroutine(RoundPlaying());
        yield return StartCoroutine(RoundEnding());

        if (hasWinner)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            StartCoroutine(GameLoop());
        }
    }

    private IEnumerator RoundStarting()
    {
        roundNumber++;

        ResetMatch();
        ResetTexts();
        DestroyWizards();
        DestroySnitch();
        SpawnSnitch();
        SpawnWizards();

        RoundStartingText();

        yield return startWait;
    }

    private bool HasRoundFinished()
    {
        return roundFinished == true;
    }

    private IEnumerator RoundPlaying()
    {
        RoundPlayingText();
        InitializeSnitch();
        InitializeWizards();

        // wait until someone collides with the snitch
        while (!HasRoundFinished())
        {
            yield return null;
        }
    }

    private IEnumerator RoundEnding()
    {
        roundWinner.teamTraits.points++;

        if (previousRoundWinner != null)
        {
            if (previousRoundWinner.name == roundWinner.name)
            {
                roundWinner.teamTraits.points += streak;
                streak++;
            }
            else
                streak = 1;
        }
        else
            streak = 1;

        hasWinner = CheckWinner();

        RoundEndingText();
    
        yield return endWait;
    }

    private bool CheckWinner()
    {
        return roundWinner.teamTraits.points >= gameSettings.targetPoints;
    }
}
