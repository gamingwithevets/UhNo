using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] private GameObject m_GameEnviroment;
    [SerializeField] private PlayerDeck m_PlayerDeck;
    [SerializeField] private TurnSystem m_TurnSystem;
    [SerializeField] private ColorPicker m_ColorPicker;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        m_GameEnviroment.SetActive(false);
    }

    public void InitGame()
    {
        m_GameEnviroment.SetActive(true);
    }

    public void ResetGame()
    {
        m_PlayerDeck.Reset();
        m_TurnSystem.Reset();
        m_ColorPicker.Reset();
    }

}
