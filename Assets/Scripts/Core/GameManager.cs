using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] private GameObject m_GameEnviroment;
    [SerializeField] private PlayerDeck m_PlayerDeck;
    [SerializeField] private TurnSystem m_TurnSystem;
    [SerializeField] private ColorPicker m_ColorPicker;

    [Header("House Rules")]
    [SerializeField] private bool m_MultiCard = false;
    [SerializeField] private bool m_Draw2On4 = true;
    [SerializeField] private bool m_InfiniteDraw = false;
    [SerializeField] private bool m_WildSample = false;

    private void Awake()
    {
        if (!Instance) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
        m_GameEnviroment.SetActive(false);
    }

    public void InitGame() => m_GameEnviroment.SetActive(true);

    public void ResetGame()
    {
        m_PlayerDeck.Reset();
        m_TurnSystem.Reset();
        m_ColorPicker.Reset();
    }

    public bool GetHouseRule(string rule)
    {
        switch (rule)
        {
            case "MultiCard": return m_MultiCard;
            case "Draw2On4": return m_Draw2On4;
            case "InfiniteDraw": return m_InfiniteDraw;
            case "WildSample": return m_WildSample;
            default: throw new System.ArgumentException("House rule not found: " + rule);
        }
    }

    public void ToggleHouseRule(string rule)
    {
        switch (rule)
        {
            case "MultiCard": m_MultiCard = !m_MultiCard; break;
            case "Draw2On4": m_Draw2On4 = !m_Draw2On4; break;
            case "InfiniteDraw": m_InfiniteDraw = !m_InfiniteDraw; break;
            case "WildSample": m_WildSample = !m_WildSample; break;
            default: throw new System.ArgumentException("House rule not found: " + rule);
        }
    }
}
