

public struct Block
{
    public readonly bool IsSolid;
    public readonly BlockTypes BlockType;
    public readonly byte Tileset;
    
    public Block(BlockTypes type, byte tileset)
    {
        IsSolid = true;
        BlockType = type;
        Tileset = tileset;
    }

    private Block(bool isSolid)
    {
        IsSolid = isSolid;
        BlockType = BlockTypes.Dirt;
        Tileset = 0;
    }

    public static Block Air()
    {
        return new Block(false);
    }

    public override bool Equals(object obj)
    {
        return obj is Block block &&
               IsSolid == block.IsSolid &&
               BlockType == block.BlockType &&
               Tileset == block.Tileset;
    }

    public override int GetHashCode()
    {
        var hashCode = -1043336251;
        hashCode = hashCode * -1521134295 + IsSolid.GetHashCode();
        hashCode = hashCode * -1521134295 + BlockType.GetHashCode();
        hashCode = hashCode * -1521134295 + Tileset.GetHashCode();
        return hashCode;
    }
}
