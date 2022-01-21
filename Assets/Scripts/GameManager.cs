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

    private GameObject[] wizards;
    private WaitForSeconds startWait;
    private WaitForSeconds endWait;
    private int roundNumber = 0;
    private Team roundWinner;
    private Team previousRoundWinner;
    private bool hasWinner = false;
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

    private void InitializeWizards()
    {
        wizards = GameObject.FindGameObjectsWithTag("Wizard");
        foreach (GameObject wizard in wizards)
        {
            Wizard wizardScript = wizard.GetComponent<Wizard>();
            wizardScript.Initialize();
        }
    }

    private void DestroyWizards()
    {
        wizards = GameObject.FindGameObjectsWithTag("Wizard");
        foreach (GameObject wizard in wizards)
            GameObject.Destroy(wizard);
    }

    private void ResetScores()
    {
        foreach (Team team in teams)
            team.ResetScore();
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
                center.text += $"<color=#{ColorUtility.ToHtmlStringRGB(team.teamTraits.color)}><size=32>{team.teamTraits.identifier}: {team.teamTraits.points} points</size></color>\n";
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

        ResetTexts();
        DestroyWizards();
        SpawnWizards();

        RoundStartingText();

        yield return startWait;
    }

    private IEnumerator RoundPlaying()
    {
        RoundPlayingText();

        yield return startWait;
        // wait until someone collides with the snitch
        // while (true)
        //     yield return null;
    }

    private IEnumerator RoundEnding()
    {
        previousRoundWinner = roundWinner;
        roundWinner = GetRoundWinner();
        
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

    private Team GetRoundWinner()
    {
        GameObject wizard = GameObject.FindGameObjectsWithTag("Wizard")[Random.Range(0, 20)];
        return wizard.GetComponentInParent<Team>();
    }

    private bool CheckWinner()
    {
        return roundWinner.teamTraits.points >= gameSettings.targetPoints;
    }
}
