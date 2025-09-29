using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Assets.Scripts;


public class ShopItemCardView : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image itemImage; // �����
    [SerializeField] private TMP_Text nameText; // �̸�
    [SerializeField] private Image coinIcon; // ���� ������
    [SerializeField] private TMP_Text priceText; // ����

    [Header("Optional")]
    [SerializeField] private Button clickButton;      // ī�� Ŭ����(������ ��Ʈ�� �̺�Ʈ Ʈ���ŷ� ��ü ����)

    private ItemData _item;
    private Action<ItemData> _onClick;

    // �ܺο��� �����Ϳ� Ŭ�� �ݹ��� ����
    public void SetData(ItemData item, Action<ItemData> onClick = null)
    {
        _item = item;
        _onClick = onClick;

        // �̹���
        if (itemImage != null)
            itemImage.sprite = item?.icon;

        // �̸�
        if (nameText != null)
            nameText.text = item != null ? item.name : string.Empty;

        // ����
        if (priceText != null)
            priceText.text = item != null ? item.price.ToString() : string.Empty;

        // ���� �������� �����λ� ���� ��������Ʈ�� �� ����(���ٸ� �״�� �α�)
        // coinIcon�� null�̾ ����

        // ��ư ����
        WireButton();
    }

    private void Awake()
    {
        // Ȥ�� �ν����Ϳ��� �����ٸ� ��Ʈ���� Button Ž�� �õ�(����)
        if (clickButton == null)
            clickButton = GetComponent<Button>();

        // �����Ϳ��� �̸� ��ġ�� ��� ����� �ʱ�ȭ
        WireButton();
    }

    private void OnDestroy()
    {
        if (clickButton != null)
            clickButton.onClick.RemoveListener(OnClick);
    }

    private void WireButton()
    {
        if (clickButton == null) return;

        clickButton.onClick.RemoveListener(OnClick);
        clickButton.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        if (_item == null) return;
        _onClick?.Invoke(_item);
    }
}
