using UnityEngine;
using UnityEngine.UI; // UI(����� �������̽�)
using TMPro; // TextMeshPro(�ؽ�Ʈ�޽�����)

public class ItemModal : MonoBehaviour
{
    [Header("Refs (����)")]

    public GameObject overlay; // ModalOverlay(��� ��������, ��ü ���)
    public RectTransform window; // ModalWindow(��� ������)
    public Image iconImage; // IconImage(������ �̹���)
    public TMP_Text nameText; // NameText(�̸� �ؽ�Ʈ)
    public TMP_Text priceText; // PriceText(���� �ؽ�Ʈ)
    public TMP_Text descText; // DescText(���� �ؽ�Ʈ)
    public Button closeButton; // CloseButton(�ݱ� ��ư, X)
    public Button cancelButton; // CancelButton(���/�ݱ� ��ư)
    public Button confirmButton; // ConfirmButton(Ȯ��/���� ��ư)
                               // Ȯ��(Confirm) �� �ܺη� ������ �ݹ�� ���� ������ ������
    private System.Action<ItemData> onConfirm;  // Confirm �ݹ�(Ȯ�� �� ȣ��)
    private ItemData current;                    // ���� ǥ�� ���� ������ ������

    void Awake()
    {
        // ���� �� ���� ���·�
        HideImmediate();

        // ��ư �̺�Ʈ ����
        if (closeButton != null) closeButton.onClick.AddListener(Close);
        if (cancelButton != null) cancelButton.onClick.AddListener(Close);

        // Confirm(Ȯ��) ��ư�� ���� �޼��� ���
        if (confirmButton != null) confirmButton.onClick.AddListener(ClickConfirm);
    }

    // ��� ǥ��: ������ ���ε� + �ݹ� ����
    public void Show(ItemData data, System.Action<ItemData> onConfirmHandler = null)
    {
        if (data == null)
        {
            Debug.LogWarning("ItemModal.Show ȣ�� �� data�� null�Դϴ�.");
            return;
        }

        current = data;
        onConfirm = onConfirmHandler;

        // ������ ���ε�
        if (iconImage != null) iconImage.sprite = data.icon;
        if (nameText != null) nameText.text = data.name;
        if (priceText != null) priceText.text = $"{data.price:N0} ����";
        if (descText != null) descText.text = "�ӽ� �����Դϴ�. ���� ���� �������� ��ü�ϼ���.";

        // ǥ��
        if (overlay != null) overlay.SetActive(true);

        // ������ ���� ����(����)
        if (window != null)
        {
            window.localScale = Vector3.one * 0.96f;
            // Ʈ��(�ִϸ��̼�) ���̺귯���� ����ϽŴٸ� ���⼭ �ε巴�� 1.0���� �����Ͻø� �˴ϴ�.
        }

        // ���ټ� ����: Ȯ�� ��ư�� ��Ŀ�� �ֱ�(����)
        if (confirmButton != null) confirmButton.Select();

        Debug.Log("�������� Ȱ��ȭ");
    }

    // ��� Ŭ���� ���� ��������, ModalOverlay(��������)�� Button(OnClick)�� �� Close�� ����
    public void Close()
    {
        if (overlay != null) overlay.SetActive(false);

        // ���� �ʱ�ȭ
        current = null;
        onConfirm = null;
        Debug.Log("�������� �� Ȱ��ȭ");
    }

    // ��� �����(�ʱ�ȭ �� ���)
    public void HideImmediate()
    {
        if (overlay != null) overlay.SetActive(false);
    }

    // Ȯ�� ��ư Ŭ�� ��: �ݹ� ȣ�� �� �ݱ�
    public void ClickConfirm()
    {
        if (current != null)
        {
            onConfirm?.Invoke(current);
        }
        Close();
    }
}