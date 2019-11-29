using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LiteNetLib.Utils;

public interface INetworkEvent
{
	NetworkEvents ID {get;}

	/// <summary>
	/// Returns true if wrote to the writer
	/// </summary>
	/// <param name="writer"></param>
	/// <returns></returns>
	bool TryAddPacket(NetDataWriter writer);
	/// <summary>
	/// Reads packet from writer
	/// </summary>
	/// <param name="reader"></param>
	void ReadPacket(NetDataReader reader);
	void Invoke();
}
