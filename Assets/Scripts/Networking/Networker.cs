using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;
public static class Networker
{
	private static Dictionary<NetworkEvents, INetworkEvent> idToEvent = new Dictionary<NetworkEvents, INetworkEvent>();
	private static NetManager client;
	private static NetManager server;
	private static Queue<INetworkEvent> packetsToSend = new Queue<INetworkEvent>();

	public static bool IsServer { get; private set; } = false;

	private static NetDataWriter globalWriter = new NetDataWriter();

	public static readonly EventBasedNetListener listener = new EventBasedNetListener();

	private static bool initialized = false;

	public static bool IsConnected { get; private set; }



	public static void JoinServer()
	{
		//Currently only joining local host
		client = new NetManager(listener);
		client.Start();
		client.Connect("127.0.0.1", 3213, "");
		IsConnected = true;
		AddPacket(new JoinEvent());
		IsServer = false;
	}

	public static void HostServer()
	{
		server = new NetManager(listener);
		IsConnected = server.Start(3213);
		listener.ConnectionRequestEvent += request =>
		{
			if (server.PeersCount < 10 /* max connections */)
				request.Accept();
			else
				request.Reject();
		};
		listener.NetworkReceiveEvent += OnNetworkRecieve;
		IsServer = true;
	}

	private static void SendMessage()
	{
		if (packetsToSend.Count == 0) return;
		globalWriter = new NetDataWriter();
		globalWriter.Put(packetsToSend.Count);
		while (packetsToSend.Count > 0)
		{
			INetworkEvent netEvent = packetsToSend.Dequeue();
			netEvent.TryAddPacket(globalWriter);
		}

		if (IsServer) server.SendToAll(globalWriter, DeliveryMethod.ReliableOrdered);
		else client.SendToAll(globalWriter, DeliveryMethod.ReliableOrdered);

	}

	public static void AddPacket(INetworkEvent networkEvent)
	{
		packetsToSend.Enqueue(networkEvent);
	}

	public static void Poll()
	{
		if (!initialized) Initialize();
		if (IsServer && IsConnected) server.PollEvents();
		else if (IsConnected) client.PollEvents();
		SendMessage();
	}

	//TODO - Replace this with an attribute and reflection
	private static void Initialize()
	{
		if (initialized) return;
		idToEvent.Add(NetworkEvents.Testing, new JoinEvent());
		initialized = true;
	}

	private static void OnNetworkRecieve(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
	{
		if (IsServer)
		{
			//send to all but the source
		}

		int packetCount = reader.GetInt();
		for (int i = 0; i < packetCount; i++)
		{
			NetworkEvents id = (NetworkEvents)reader.GetByte();
			//replace the following by instanting a new value
			idToEvent[id].ReadPacket(reader);
			idToEvent[id].Invoke();
		}
	}

	public static void Disconnect()
	{
		if (IsConnected)
		{
			if (IsServer) server.Stop();
			else client.Stop();
		}

	}
}