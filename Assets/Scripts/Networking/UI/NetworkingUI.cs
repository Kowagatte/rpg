using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkingUI : MonoBehaviour
{
	public void HostServer()
	{
		Networker.HostServer();
	}
	public void JoinServer()
	{
		Networker.JoinServer();
	}

	void Update()
	{
		if (Networker.IsConnected) Networker.Poll();
	}

	void OnApplicationQuit()
	{
		Networker.Disconnect();
	}

}
