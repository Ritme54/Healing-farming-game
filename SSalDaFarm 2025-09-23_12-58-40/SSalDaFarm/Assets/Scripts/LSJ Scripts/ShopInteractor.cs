using UnityEngine;
using static IShopOpener;

public class ShopInteractor : MonoBehaviour
{
    [Header("Shop Controller Lookup")]
    public ShopUIController shop; // ���� �ϳ���� ����ΰ� �ڵ� Ž�� ����

    [Header("Open Triggers")]
    public bool allowClickToOpen = true;   // ���콺 Ŭ������ ����
    public bool allowKeyToOpen = true;     // Ű �Է�(E/Enter)�� ����
    public KeyCode key1 = KeyCode.E;       // ��ȣ�ۿ� Ű 1
    public KeyCode key2 = KeyCode.Return;  // ��ȣ�ۿ� Ű 2(Enter)

    [Header("Standalone Test Range (No Player Needed)")]
    public bool useFakeRange = false;      // �÷��̾� ���� ���� ������ �䳻����
    public float fakeRange = 2.0f;         // ��� �Ÿ�(ī�޶� �߽� �Ǵ� ���������� �Ÿ�)
    public Transform rangeCenter;          // ������(���� �� ������Ʈ)

    private Camera _cam;

    private void Awake()
    {
        // ������ ShopUIController�� �ڵ����� ã�� ����(��� ���ᵵ ����)
        if (shop == null) shop = FindObjectOfType<ShopUIController>();

        _cam = Camera.main;
        if (rangeCenter == null) rangeCenter = transform;
    }

    private void Update()
    {
        // Ű �Է����� ����/�ݱ� ���
        if (shop == null) return;

        if (allowKeyToOpen && (Input.GetKeyDown(key1) || Input.GetKeyDown(key2)))
        {
            // ��¥ ���� ��� �� ���� �ȿ����� ���
            if (!useFakeRange || InFakeRange())
                ToggleOpen();
        }
    }

    private void OnMouseDown()
    {
        // ������Ʈ�� Collider2D/Collider�� �ְ� ī�޶� �ش� ���̾ ���� ���õǾ� �־�� ȣ���
        if (!allowClickToOpen || shop == null) return;

        if (!useFakeRange || InFakeRange())
            ToggleOpen();
    }

    private bool InFakeRange()
    {
        // �÷��̾ ���� ȯ�濡�� ������ üũ���� �䳻���� �Լ�
        // ����: ī�޶� �߽ɰ� rangeCenter ������ �Ÿ��� ����(�ӽ�)
        if (_cam == null) return true;
        var camPos = _cam.transform.position;
        camPos.z = rangeCenter.position.z; // 2D ��� �񱳸� ���� z�� ����
        return Vector2.Distance(camPos, rangeCenter.position) <= fakeRange;
    }

    private void ToggleOpen()
    {
        // ���� ���¸� �ݰ�, ���� ������ ����
        if (shop.IsOpen) shop.CloseShop();
        else shop.OpenShop();
    }
}