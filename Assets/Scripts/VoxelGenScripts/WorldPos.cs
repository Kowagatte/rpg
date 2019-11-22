using System.Threading;
using System.Reflection.Emit;
using System.Collections;
using UnityEngine;

public struct WorldPos
{
    public readonly int x, y, z;
    public WorldPos(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public override bool Equals(object obj)
    {
        if(!(obj is WorldPos)) return false;

        WorldPos pos = (WorldPos)obj;
        if( pos.x != x || pos.y != y || pos.z != z )
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public override int GetHashCode()
{
    unchecked // Overflow is fine, just wrap
    {
        int hash = 17;
        // Suitable nullity checks etc, of course :)
        hash = hash * 23 + x.GetHashCode();
        hash = hash * 23 + y.GetHashCode();
        hash = hash * 23 + z.GetHashCode();
        return hash;
    }
}
}
