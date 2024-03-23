using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public void LoadScene(int sceneIdx)
    {
        StartCoroutine(LoadingScene(sceneIdx));
    }
    IEnumerator LoadingScene(int idx)
    {
        yield return SceneManager.LoadSceneAsync(1);

        Slider slider = FindObjectOfType<Slider>();

        yield return new WaitForSeconds(2.0f);

        yield return StartCoroutine(Loading(slider, idx));
    }

    IEnumerator Loading(Slider slider, int idx)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(idx);

        ao.allowSceneActivation = false;
        while (!ao.isDone)
        {
            slider.value = ao.progress / 0.9f;
            if (Mathf.Approximately(slider.value, 1.0f))
            {
                break;
            }
            yield return null;
        }

        //로딩 완료
        slider?.GetComponent<LoadingBar>()?.OnCompleteLoad();

        yield return new WaitForSeconds(1.0f);

        while (true)
        {
            if (Input.anyKeyDown)
            {
                ao.allowSceneActivation = true;
                break;
            }
            yield return null;
        }


        yield return null;
    }
}
