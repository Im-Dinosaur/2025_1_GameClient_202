using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Inventory/Database")]
public class ItemDatabaseSO : ScriptableObject
{
    public List<ItemSO> items = new List<ItemSO>(); //ItemSO�� ����Ʈ�� �����Ѵ�.

    //ĳ���� ���� ����
    private Dictionary<int, ItemSO> itemsByld;  //ID�� ������ ã�� ���� ĳ��
    private Dictionary<string, ItemSO> itemsByName;

    public void Initialize()  //�ʱ� ���� �Լ�
    {
        itemsByld = new Dictionary<int, ItemSO>(); //���� ���� �߱� ������ Dictionary �Ҵ�
        itemsByName = new Dictionary<string, ItemSO>();

        foreach(var item in items)  //items ����Ʈ�� ����Ǿ� �ִ°��� ������ Dictionary�� �Է��Ѵ�.
        {
            itemsByld[item.id] = item;
            itemsByName[item.itemName] = item;
        }
    }

    // ID�� ������ ã��
    public ItemSO GetItemById(int id)
    {
        if(itemsByld == null)  //itemsById�� ĳ���� �Ǿ� ���� ������ �ʱ�ȭ �Ѵ�
        {
            Initialize();
        }
        if (itemsByld.TryGetValue(id, out ItemSO item))  //Id ���� ã�Ƽ� ItemSO�� �����Ѵ�
            return item;

        return null;  //���� ��� Null
    }

    //�̸����� ������ ã��
    public ItemSO GetItemByName(string name)
    {
        if(itemsByName == null)  //itemsByName �� ĳ���� �Ǿ� ���� �ʴٸ� �ʱ�ȭ �Ѵ�
        {
            Initialize();
        }
        if (itemsByName.TryGetValue(name, out ItemSO item)) //Name ���� ã�Ƽ� ItemSO�� �����Ѵ�.
            return item;
        return null;
    }

    //Ÿ������ ������ ���͸�
    public List<ItemSO>GetItemByType(ItemType type)
    {
        return items.FindAll(item => item.itemType == type);
    }
}
