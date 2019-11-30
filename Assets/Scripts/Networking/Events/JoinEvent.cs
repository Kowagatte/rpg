using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LiteNetLib.Utils;
using LiteNetLib;


public class JoinEvent : INetworkEvent
{
	public NetworkEvents ID => NetworkEvents.Testing;
	private string value = "Hi Gary, this is a networked message";

	public bool TryAddPacket(NetDataWriter writer)
	{
		writer.Put((byte)ID);
		writer.Put(value);
		return true;
	}

	public void ReadPacket(NetPacketReader reader)
	{
		value = reader.GetString();
	}

	public void Invoke(){
		Debug.Log(value);
	}


}
