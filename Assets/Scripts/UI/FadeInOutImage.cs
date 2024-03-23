using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeInOutImage : MonoBehaviour
{
    Image img;

    const float FADE_DURATION = 1.0f;

    private void Awake()
    {
        img = GetComponent<Image>();

        img.enabled = true;
        img.color = new Color(0, 0, 0, 1f);
    }

    private void Start()
    {
        FadeIn();
    }

    /// <summary>
    /// 점점 밝아지는 효과(씬 들어왔을 때, 실행)
    /// </summary>
    public void FadeIn()
    {
        img.enabled = true;
        img.color = new Color(0, 0, 0, 1f);

        StartCoroutine(FadeInCo());
    }
    IEnumerator FadeInCo()
    {
        //yield return new WaitForSeconds(0.3f);

        float time = FADE_DURATION;
        while (time > 0f)
        {
            time -= Time.unscaledDeltaTime;
            if (time < 0f)
                time = 0f;
            img.color = new Color(0, 0, 0, time / FADE_DURATION);

            yield return null;
        }

        img.enabled = false;
    }

    /// <summary>
    /// 점점 어두워지는 효과
    /// </summary>
    public void FadeOut()
    {
        img.enabled = true;
        img.color = new Color(0, 0, 0, 0f);

        StartCoroutine(FadeOutCo());
    }
    IEnumerator FadeOutCo()
    {
        float time = 0f;
        while (time < FADE_DURATION)
        {
            time += Time.unscaledDeltaTime;
            if (time > FADE_DURATION)
                time = FADE_DURATION;
            img.color = new Color(0, 0, 0, time / FADE_DURATION);

            yield return null;
        }
    }
}
