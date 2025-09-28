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
    public TMP_Text confirmLabel;
    // Ȯ��(Confirm) �� �ܺη� ������ �ݹ�� ���� ������ ������
    private System.Action<ItemData> onConfirm;  // Confirm �ݹ�(Ȯ�� �� ȣ��)
    private ItemData current;                    // ���� ǥ�� ���� ������ ������
    public TMP_InputField qtyInput;
    public Button minusButton;
    public Button plusButton;
    public TMP_Text rangeHint;
    public TMP_Text totalText;
    public TMP_Text walletText;

    private int minQty = 1;
    private int maxQty = 99;
    private int currentQty = 1;
    private ItemSource currentSource = ItemSource.Shop; // ������/�Ǹš� �б��


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
        if (priceText != null) priceText.text = $"{data.price:N0} Coin";
        if (descText != null) descText.text = "Test Text 123456789.";

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
    public void SetConfirmLabel(string label)
    { // null/���� ���: label�� ��� ������ �⺻������ ��ü if (string.IsNullOrWhiteSpace(label)) label = "Ȯ��";
      // 1) ���� �� ������ �ִٸ� �켱 ���
        if (confirmLabel != null)
        {
            confirmLabel.text = label;
            return;
        }

        // 2) ���� ���� ������, ��ư �ڽĿ��� �����ϰ� �˻�
        if (confirmButton != null)
        {
            // ��Ȱ�� �ڽı��� ������ Ž���ϵ��� true ����
            var text = confirmButton.GetComponentInChildren<TMPro.TMP_Text>(true);
            if (text != null)
            {
                text.text = label;
                return;
            }
        }

        // 3) ������ �����ϸ� ��� �α�
        Debug.LogWarning("SetConfirmLabel: ���� ������ �� �����ϴ�. confirmLabel �Ǵ� confirmButton ���� TMP_Text ������ Ȯ���ϼ���.");


    }

    public void ConfigureQuantity(ItemSource source, int min, int max, int initial = 1, int playerCoins = 0, int ownedCount = 0)
    {
        currentSource = source; minQty = Mathf.Max(1, min); maxQty = Mathf.Max(minQty, max); currentQty = Mathf.Clamp(initial, minQty, maxQty);
        // �Է� �ʵ� ǥ��
        if (qtyInput != null)
        {
            qtyInput.contentType = TMP_InputField.ContentType.IntegerNumber;
            qtyInput.text = currentQty.ToString();
            qtyInput.onValueChanged.RemoveAllListeners();
            qtyInput.onValueChanged.AddListener(OnQtyChangedByInput);
            qtyInput.onEndEdit.RemoveAllListeners();
            qtyInput.onEndEdit.AddListener(OnQtyEndEdit);
        }

        // ���� ��ư(������) ����
        if (minusButton != null)
        {
            minusButton.onClick.RemoveAllListeners();
            minusButton.onClick.AddListener(() => SetQty(currentQty - 1));
        }
        if (plusButton != null)
        {
            plusButton.onClick.RemoveAllListeners();
            plusButton.onClick.AddListener(() => SetQty(currentQty + 1));
        }

        // ���� ��Ʈ
        if (rangeHint != null)
            rangeHint.text = $"�ּ� {minQty} ~ �ִ� {maxQty}";

        // ����/���� �ȳ�
        if (walletText != null)
        {
            if (source == ItemSource.Shop)
                walletText.text = $"���� ���� {playerCoins:N0}";
            else
                walletText.text = $"���� {ownedCount:N0}��";
        }

        UpdateTotal(); // �հ� ����


    }

    private void OnQtyChangedByInput(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return;
        int parsed;
        if (!int.TryParse(text, out parsed))
            return;

        SetQty(parsed, updateInput: false); // �Է� �ݹ� ���� ����


    }

    private void OnQtyEndEdit(string text)
    { // ��Ŀ���� ���� �� ���� ����
        int parsed = currentQty;
        int.TryParse(text, out parsed);
        SetQty(parsed);
    }

    private void SetQty(int value, bool updateInput = true)
    {
        int clamped = Mathf.Clamp(value, minQty, maxQty);
        currentQty = clamped;

        if (updateInput && qtyInput != null)
            qtyInput.text = currentQty.ToString();

        UpdateTotal();


    }

    private void Update()
    {
        // �����: ESC Ű�� �ݱ�
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Close();
        }
    }


    private void UpdateTotal()
    {
        if (totalText != null && priceText != null && current != null) { long total = (long)current.price * currentQty; totalText.text = $"�հ� {total:N0} ����"; }


        // Ȯ�� ��ư Ȱ��ȭ ����(�ʿ� ��)
        if (confirmButton != null)
            confirmButton.interactable = (currentQty >= minQty && currentQty <= maxQty);
    }

    public int GetSelectedQuantity() => currentQty;

}
