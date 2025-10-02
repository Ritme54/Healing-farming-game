using UnityEngine;

[CreateAssetMenu(fileName = "ItemDataAsset", menuName = "Game/Item Data Asset", order = 0)]
public class ItemDataAsset : ScriptableObject
{
    public string id;
    public string itemName;
    public string description;
    public Sprite icon;
    public int price;
    // ���� POCO ��(Assets.Scripts.ItemData)�� ��ȯ�ϴ� ����
    public Assets.Scripts.ItemData ToRuntime()
    {
        return new Assets.Scripts.ItemData
        {
            id = id,
            name = itemName,
            description = description,
            icon = icon,
            price = price
        };
    }
}
