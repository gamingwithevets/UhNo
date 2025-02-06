using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public TextMeshProUGUI versionText;
    public TextMeshProUGUI progressText;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        versionText.text = "v" + Application.version;
        StartCoroutine(StartGameAsync());
    }

    IEnumerator StartGameAsync() {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Main");
        while (!asyncLoad.isDone) {
            progressText.text = "HANG ON... " + (asyncLoad.progress * 100).ToString("F0") + "%";
            yield return null;
        }
    }
}
