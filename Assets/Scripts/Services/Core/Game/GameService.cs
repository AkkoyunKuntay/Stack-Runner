using UnityEngine.SceneManagement;
using UnityEngine;
using Zenject;

public interface IGameFlowService
{
    public event System.Action LevelStartedEvent;
    public event System.Action LevelEndedEvent;
    public event System.Action LevelSuccessEvent;
    public event System.Action LevelFailedEvent;
    public event System.Action LevelAboutToChangeEvent;

    bool IsLevelActive { get; }
    bool IsLevelSuccessful { get; }
    void StartGame();
    void EndGame(bool success);
    void NextStage();
    void RestartStage();
}
public class GameService : MonoBehaviour, IGameFlowService, IInitializable
{
    [Header("Config")]
    [SerializeField] int randomLevelOffset = 0;

    [Header("Debug")]
    [SerializeField] bool isLevelActive = false;
    [SerializeField] bool isLevelSuccessful = false;

    public event System.Action LevelStartedEvent;
    public event System.Action LevelEndedEvent;
    public event System.Action LevelSuccessEvent;
    public event System.Action LevelFailedEvent;
    public event System.Action LevelAboutToChangeEvent;

    [Header ("Contant Params")]
    const string lastPlayedStageKey = "n_lastPlayedStage";
    const string cumulativeStagePlayedKey = "n_cumulativeStages";
    const string randomizeStagesKey = "n_randomizeStages";

    public void Initialize()
    {
        Application.targetFrameRate = 999;
        QualitySettings.vSyncCount = 0;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        if (!PlayerPrefs.HasKey(cumulativeStagePlayedKey)) PlayerPrefs.SetInt(cumulativeStagePlayedKey, 1);

        if (!PlayerPrefs.HasKey(lastPlayedStageKey))
            PlayerPrefs.SetInt(lastPlayedStageKey, SceneManager.GetActiveScene().buildIndex);
        Debug.Log("[GameService] has been initialized.");
    }

    public bool IsLevelActive => isLevelActive;
    public bool IsLevelSuccessful => isLevelSuccessful;

    public void StartGame()
    {
        isLevelActive = true;
        LevelStartedEvent?.Invoke();
    }

    public void EndGame(bool success)
    {
        isLevelActive = false;
        isLevelSuccessful = success;

        LevelEndedEvent?.Invoke();

        if (success) LevelSuccessEvent?.Invoke();
        else LevelFailedEvent?.Invoke();
    }

    public void NextStage()
    {
        LevelAboutToChangeEvent?.Invoke();

        int target = CalcNextBuildIndex();
        PlayerPrefs.SetInt(lastPlayedStageKey, target);
        PlayerPrefs.SetInt(cumulativeStagePlayedKey, PlayerPrefs.GetInt(cumulativeStagePlayedKey, 1) + 1);
        LevelStartedEvent?.Invoke();
        isLevelActive = true;
    }

    public void RestartStage()
    {
        LevelAboutToChangeEvent?.Invoke();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    #region Helper Methods
    int CalcNextBuildIndex()
    {
        bool rnd = PlayerPrefs.GetInt(randomizeStagesKey, 0) == 1;
        int cur = SceneManager.GetActiveScene().buildIndex;

        if (!rnd)
        {
            int next = cur + 1;
            if (next >= SceneManager.sceneCountInBuildSettings)
            {
                PlayerPrefs.SetInt(randomizeStagesKey, 1);
                return RandomStage();
            }
            return next;
        }
        return RandomStage();
    }
    int RandomStage()
    {
        return Random.Range(randomLevelOffset, SceneManager.sceneCountInBuildSettings);
    }
    #endregion

}
