using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;
public static class Networker
{
	private static Dictionary<NetworkEvents, INetworkEvent> idToEvent = new Dictionary<NetworkEvents, INetworkEvent>();
	public static readonly EventBasedNetListener listener = new EventBasedNetListener();
	private static NetManager client;
	private static NetManager server;
	private static Queue<INetworkEvent> packetsToSend = new Queue<INetworkEvent>();
	public static bool IsServer => client == null;

	private static NetDataWriter globalWriter = new NetDataWriter();
	private static NetDataReader globalReader = new NetDataReader();



	public static void JoinServer()
	{
		//Currently only joining local host
		client = new NetManager(listener);
		client.Start();
		client.Connect("127.0.0.1", 3212, "");
	}

	public static void HostServer()
	{
		server = new NetManager(listener);
		server.Start(3212);
		listener.ConnectionRequestEvent += request =>
		{
			if (server.PeersCount < 10 /* max connections */)
				request.Accept();
			else
				request.Reject();
		};
		listener.NetworkReceiveEvent += OnNetworkRecieve;
	}

	private static void SendMessage()
	{
		globalWriter = new NetDataWriter();
		globalWriter.Put(packetsToSend.Count);
		while (packetsToSend.Count > 0)
		{
			INetworkEvent netEvent = packetsToSend.Dequeue();
			netEvent.TryAddPacket(globalWriter);
		}
	}

	public static void AddPacket(INetworkEvent networkEvent)
	{
		packetsToSend.Enqueue(networkEvent);
	}


	private static void OnNetworkRecieve(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
	{
		globalReader = new NetDataReader(reader.RawData);
		if (IsServer)
		{
			//send to all but the source
		}
		
		int packetCount = globalReader.GetInt();
		for (int i = 0; i < packetCount; i++)
		{
			NetworkEvents id = (NetworkEvents)globalReader.GetByte();
			idToEvent[id].ReadPacket(reader);
		}
	}
}
