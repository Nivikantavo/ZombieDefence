using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private GameObject _loadingPanel;
    [SerializeField] private Slider _loadingBar;
    [SerializeField] private TextMeshProUGUI _loadingText;

    public void LoadScene(int sceneId) 
    {
        StartCoroutine(LoadSceneAsync(sceneId));
    }

    private IEnumerator LoadSceneAsync(int sceneId) 
    {
        AsyncOperation loading = SceneManager.LoadSceneAsync(sceneId);

        _loadingPanel.SetActive(true);

        while (!loading.isDone) 
        {
            float fillValue = Mathf.Clamp01(loading.progress / 0.9f);
            _loadingBar.value = fillValue;
            _loadingText.text = fillValue * 100f + "%";
            yield return null;
        }
    }
}
