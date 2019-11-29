using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LiteNetLib.Utils;


public class JoinEvent : INetworkEvent
{
	public NetworkEvents ID => NetworkEvents.Testing;
	private string value = "See!";

	public bool TryAddPacket(NetDataWriter writer)
	{
		writer.Put((byte)ID);
		writer.Put(value);
		return true;
	}

	public void ReadPacket(NetDataReader reader)
	{
		value = reader.GetString();
	}

	public void Invoke(){
		Debug.Log("Invoked!");
	}


}
