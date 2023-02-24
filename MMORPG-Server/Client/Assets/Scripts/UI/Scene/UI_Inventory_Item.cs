using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Inventory_Item : UI_Base
{
	[SerializeField]
	Image _icon;

	public override void Init()
	{

	}

	public void SetItem(int templateId, int count)
	{
		Data.ItemData itemData = null;
		Managers.Data.ItemDict.TryGetValue(templateId, out itemData);

		Sprite icon = Managers.Resource.Load<Sprite>(itemData.iconPath);
		_icon.sprite = icon;
	}
}
