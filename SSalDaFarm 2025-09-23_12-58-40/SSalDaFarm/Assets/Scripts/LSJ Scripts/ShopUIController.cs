using UnityEngine;
using UnityEngine.EventSystems;

public interface IShopOpener
{
    void OpenShop(); // ���� UI�� ����
    void CloseShop(); // ���� UI�� �ݴ´�
    bool IsOpen { get; } // ���� ���� UI ���� ���� }
}

public class ShopUIController : MonoBehaviour, IShopOpener
{
    [Header("UI References")] public GameObject shopPanel; // ���� UI ��Ʈ �г�(ó������ ��Ȱ��ȭ ����)
    public GameObject defaultFocus; // ������ ���� �� ���ʷ� ��Ŀ�� �� UI(��: Buy ��ư)
    public CanvasGroup dim; // ��� ��(����: Ŭ�� ����/���� �����)

    [Header("Behavior")]
    public bool lockTimeOnOpen = false; // ���� �� ���� �Ͻ�����(Time.timeScale=0) ���� ����

    public bool IsOpen { get; private set; } // ���� ���� ����
    private float _prevTimeScale = 1f;       // ���� �� Ÿ�ӽ����� ����

    private void Start()
    {
        // ���� �� ������ ���� ���·� �ʱ�ȭ
        SetOpen(false);
    }

    public void OpenShop()
    {
        // �̹� ���� ������ ����
        if (IsOpen) return;
        SetOpen(true);
    }

    public void CloseShop()
    {
        // �̹� ���� ������ ����
        if (!IsOpen) return;
        SetOpen(false);
    }

    private void Update()
    {
        // ���� ���� �� ESC�� �ݱ� ����
        if (!IsOpen) return;
        if (Input.GetKeyDown(KeyCode.Escape))
            CloseShop();
    }

    private void SetOpen(bool open)
    {
        // ���������� ����/���� ���¸� ��ȯ�ϰ�, ���� UI/�ð�/��Ŀ���� ó��
        IsOpen = open;

        // �г� ǥ��/����
        if (shopPanel != null)
            shopPanel.SetActive(open);

        // ��� �� ó��(���� ����)
        if (dim != null)
        {
            dim.alpha = open ? 0.5f : 0f;     // ����
            dim.blocksRaycasts = open;        // �� ���� Ŭ�� ����
            dim.interactable = open;          // ��ȣ�ۿ� ����
        }

        // Ÿ�ӽ����� ���(����)
        if (open)
        {
            if (lockTimeOnOpen)
            {
                _prevTimeScale = Time.timeScale;
                Time.timeScale = 0f;
            }

            // UI ��Ŀ�� ����
            if (EventSystem.current != null && defaultFocus != null)
                EventSystem.current.SetSelectedGameObject(defaultFocus);
        }
        else
        {
            if (lockTimeOnOpen)
                Time.timeScale = _prevTimeScale;

            // ��Ŀ�� ����
            if (EventSystem.current != null)
                EventSystem.current.SetSelectedGameObject(null);
        }
    }
}
