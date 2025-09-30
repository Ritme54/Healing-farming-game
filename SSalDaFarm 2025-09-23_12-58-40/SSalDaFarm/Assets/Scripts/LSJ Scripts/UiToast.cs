using UnityEngine;
using TMPro;
using System.Collections;

public class UiToast : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private float showSeconds = 1.5f;
    [SerializeField] private CanvasGroup canvasGroup; // 페이드용(없으면 자동 추가)
    private Coroutine _routine;

    private void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = gameObject.GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
    }

    public void Show(string message)
    {
        if (text != null) text.text = message ?? string.Empty;
        if (_routine != null) StopCoroutine(_routine);
        _routine = StartCoroutine(CoShow());
    }

    private IEnumerator CoShow()
    {
        // 페이드 인
        yield return Fade(0f, 1f, 0.15f);
        // 유지
        yield return new WaitForSeconds(showSeconds);
        // 페이드 아웃
        yield return Fade(1f, 0f, 0.25f);
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }
        canvasGroup.alpha = to;
    }
}