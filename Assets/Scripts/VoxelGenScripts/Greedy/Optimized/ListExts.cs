using System.Collections.Generic;
public static class DictionaryExts
{
    public static Dictionary<WorldPos, Chunk> AddThen(this Dictionary<WorldPos, Chunk> dict, WorldPos worldPos, Chunk chunk)
    {
        dict.Add(worldPos, chunk);
        return dict;
    }
}