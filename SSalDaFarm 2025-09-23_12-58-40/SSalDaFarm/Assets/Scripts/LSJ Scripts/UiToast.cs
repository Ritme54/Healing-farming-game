using UnityEngine;
using TMPro;
using System.Collections;

public class UiToast : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private float showSeconds = 1.5f;
    [SerializeField] private CanvasGroup canvasGroup; // ���̵��(������ �ڵ� �߰�)
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
        // ���̵� ��
        yield return Fade(0f, 1f, 0.15f);
        // ����
        yield return new WaitForSeconds(showSeconds);
        // ���̵� �ƿ�
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