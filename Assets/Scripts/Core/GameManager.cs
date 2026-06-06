using System.Collections;
using UnityEngine;

/// <summary>
/// Drives the round loop: for each of N rounds, reset the count, scatter that round's fruit,
/// pose the matching question, and update the HUD. A correct answer awards a star, cheers the
/// mascot, and (after a short celebration beat) advances; after the last round it fires
/// OnGameComplete and plays a fanfare. Difficulty ramps via roundCounts.
/// </summary>
public class GameManager : MonoBehaviour
{
    public FruitSpawner spawner;
    public AnswerPanel answerPanel;
    public CountManager countManager;
    public HudController hud;

    [Tooltip("Fruit count per round — the difficulty ramp.")]
    public int[] roundCounts = { 3, 4, 5, 6, 7 };
    public float nextRoundDelay = 1.5f;

    [Header("Audio")]
    public AudioClip fanfareClip;

    public int CurrentRound { get; private set; }
    public int Stars { get; private set; }
    public bool Complete { get; private set; }
    public System.Action<int, int> OnGameComplete; // (stars, totalRounds)

    void Start()
    {
        if (answerPanel != null) answerPanel.OnSolved += HandleCorrect;
        StartRound(0);
    }

    void OnDestroy()
    {
        if (answerPanel != null) answerPanel.OnSolved -= HandleCorrect;
    }

    void StartRound(int index)
    {
        CurrentRound = index;
        int n = roundCounts[Mathf.Clamp(index, 0, roundCounts.Length - 1)];
        if (countManager != null) countManager.ResetCount();
        if (spawner != null) spawner.Spawn(n);
        if (answerPanel != null) answerPanel.SetQuestion(n);
        if (hud != null) hud.SetRound(index + 1, roundCounts.Length);
    }

    void HandleCorrect()
    {
        Stars++;
        if (hud != null) { hud.SetStars(Stars); hud.Cheer(); }
        StartCoroutine(NextRoundAfterDelay());
    }

    IEnumerator NextRoundAfterDelay()
    {
        yield return new WaitForSeconds(nextRoundDelay);
        int next = CurrentRound + 1;
        if (next < roundCounts.Length) StartRound(next);
        else EndGame();
    }

    void EndGame()
    {
        Complete = true;
        SfxPlayer.Play(fanfareClip);
        OnGameComplete?.Invoke(Stars, roundCounts.Length);
    }

    /// <summary>Restart from round 1 (used by the end-summary replay button).</summary>
    public void Restart()
    {
        Complete = false;
        Stars = 0;
        if (hud != null) hud.SetStars(0);
        StartRound(0);
    }
}
