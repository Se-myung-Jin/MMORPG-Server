using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_LoginScene : UI_Scene
{
	enum GameObjects
	{
		AccountName,
		Password
	}

	enum Images
	{
		CreateBtn,
		LoginBtn
	}

    public override void Init()
	{
        base.Init();

		Bind<GameObject>(typeof(GameObjects));
		Bind<Image>(typeof(Images));

		GetImage((int)Images.CreateBtn).gameObject.BindEvent(OnClickCreateButton);
		GetImage((int)Images.LoginBtn).gameObject.BindEvent(OnClickLoginButton);
	}

	public void OnClickCreateButton(PointerEventData evt)
	{
		string account = Get<GameObject>((int)GameObjects.AccountName).GetComponent<InputField>().text;
		string password = Get<GameObject>((int)GameObjects.Password).GetComponent<InputField>().text;

		CreateAccountPacketReq packet = new CreateAccountPacketReq()
		{
			AccountName = account,
			Password = password
		};

		Managers.Web.SendPostRequest<CreateAccountPacketRes>("account/create", packet, (res) =>
		{
			Debug.Log(res.CreateOk);
			Get<GameObject>((int)GameObjects.AccountName).GetComponent<InputField>().text = "";
			Get<GameObject>((int)GameObjects.Password).GetComponent<InputField>().text = "";
		});
	}

	public void OnClickLoginButton(PointerEventData evt)
	{
		Debug.Log("OnClickLoginButton");

		string account = Get<GameObject>((int)GameObjects.AccountName).GetComponent<InputField>().text;
		string password = Get<GameObject>((int)GameObjects.Password).GetComponent<InputField>().text;


		LoginAccountPacketReq packet = new LoginAccountPacketReq()
		{
			AccountName = account,
			Password = password
		};

		Managers.Web.SendPostRequest<LoginAccountPacketRes>("account/login", packet, (res) =>
		{
			Debug.Log(res.LoginOk);
			Get<GameObject>((int)GameObjects.AccountName).GetComponent<InputField>().text = "";
			Get<GameObject>((int)GameObjects.Password).GetComponent<InputField>().text = "";

			if (res.LoginOk)
			{
				Managers.Network.AccountId = res.AccountId;
				Managers.Network.Token = res.Token;

				UI_SelectServerPopup popup = Managers.UI.ShowPopupUI<UI_SelectServerPopup>();
				popup.SetServers(res.ServerList);
			}
		});
	}
}
