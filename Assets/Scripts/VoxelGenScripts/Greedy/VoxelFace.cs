
public class VoxelFace
{
    public int Type;
    public int Side;
    public bool Transparent;


    public VoxelFace(int type, int side = 0, bool transparent = false )
    {
        Type = type;
        Side = side;
        Transparent = transparent;
    }

    public override bool Equals(object obj)
    {
        return obj is VoxelFace face &&
               Transparent == face.Transparent &&
               Type == face.Type;
    }

    public override int GetHashCode()
    {
        var hashCode = 461711220;
        hashCode = hashCode * -1521134295 + Transparent.GetHashCode();
        hashCode = hashCode * -1521134295 + Type.GetHashCode();
        hashCode = hashCode * -1521134295 + Side.GetHashCode();
        return hashCode;
    }
}
