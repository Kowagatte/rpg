[System.AttributeUsage(System.AttributeTargets.Struct)]
public class NetworkEvent : System.Attribute
{
	public NetworkEvents Event { get; private set; }
	public NetworkEvent(NetworkEvents e)
	{
		Event = e;
	}
}