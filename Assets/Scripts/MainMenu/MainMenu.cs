using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public GameObject titleScreen;
    public TextMeshProUGUI pressStartText;
    public GameObject loading;
    public TextMeshProUGUI progressText;
    public bool fadeToggle = true;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (fadeToggle) {
            pressStartText.color = new Color(pressStartText.color.r, pressStartText.color.g, pressStartText.color.b, pressStartText.color.a - 0.5f * Time.deltaTime);
            if (pressStartText.color.a <= 0.25) fadeToggle = false;
        } else {
            pressStartText.color = new Color(pressStartText.color.r, pressStartText.color.g, pressStartText.color.b, pressStartText.color.a + 0.5f * Time.deltaTime);
            if (pressStartText.color.a >= 1) fadeToggle = true;
        }

    }

    public void StartGame() {
        StartCoroutine(StartGameAsync());
    }

    IEnumerator StartGameAsync() {
        titleScreen.SetActive(false);
        loading.SetActive(true);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Gameplay");
        while (!asyncLoad.isDone) {
            progressText.text = (asyncLoad.progress * 100).ToString("F0") + "%";
            yield return null;
        }
    }
}
