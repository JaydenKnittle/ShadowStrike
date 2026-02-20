using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;

    [Header("Transition Settings")]
    public float transitionDuration = 0.5f;
    public CanvasGroup fadeCanvasGroup;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(TransitionToScene(sceneName));
    }

    private IEnumerator TransitionToScene(string sceneName)
    {
        yield return StartCoroutine(Fade(1f));
        SceneManager.LoadScene(sceneName);
        yield return StartCoroutine(Fade(0f));
    }

    private IEnumerator Fade(float targetAlpha)
    {
        if (fadeCanvasGroup == null) yield break;

        float startAlpha = fadeCanvasGroup.alpha;
        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / transitionDuration);
            yield return null;
        }

        fadeCanvasGroup.alpha = targetAlpha;
    }
}