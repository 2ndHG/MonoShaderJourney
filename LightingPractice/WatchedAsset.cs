using System;
using Microsoft.Xna.Framework.Content;

namespace LightingPractice;

public class WatchedAsset<T>
{
    /// <summary>
    /// The latest version of the asset.
    /// </summary>
    public T Asset { get; set; }

    /// <summary>
    /// The last time the <see cref="Asset"/> was loaded into memory.
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }

    /// <summary>
    /// The name of the <see cref="Asset"/>. This is the name used to load the asset from disk. 
    /// </summary>
    public string AssetName { get; init; }
    public ContentManager Owner { get; init; }

    /// <summary>  
    /// Attempts to refresh the asset if it has changed on disk using the registered owner <see cref="ContentManager"/>.  
    /// </summary>
    public bool TryRefresh(out T oldAsset)
    {
        return Owner.TryRefresh(this, out oldAsset);
    }
}