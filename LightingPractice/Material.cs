using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LightingPractice;

public class Material
{
    /// <summary>
    /// The hot-reloadable asset that this material is using
    /// </summary>
    public WatchedAsset<Effect> Asset;

    /// <summary>
    /// The currently loaded Effect that this material is using
    /// </summary>
    public Effect Effect => Asset.Asset;
    /// <summary>  
    /// A cached version of the parameters available in the shader  
    /// </summary>  
    public Dictionary<string, EffectParameter> ParameterMap;

    /// <summary>  
    /// Rebuild the <see cref="ParameterMap"/> based on the current parameters available in the effect instance  
    /// </summary>  

    public Material(WatchedAsset<Effect> asset)
    {
        Asset = asset;
        UpdateParameterCache();
    }
    public void UpdateParameterCache()
    {
        ParameterMap = Effect.Parameters.ToDictionary(p => p.Name);
    }
    public bool TryGetParameter(string name, out EffectParameter parameter)
    {
        return ParameterMap.TryGetValue(name, out parameter);
    }
    public void SetParameter(string name, float value)
    {
        if (TryGetParameter(name, out var parameter))
        {
            parameter.SetValue(value);
        }
        else
        {
            Console.WriteLine($"Warning: cannot set parameter=[{name}] as it does not exist in the shader=[{Asset.AssetName}]");
        }
    }
    public void SetParameter(string name, Matrix value)
    {
        if (TryGetParameter(name, out var parameter))
        {
            parameter.SetValue(value);
        }
        else
        {
            Console.WriteLine($"Warning: cannot set shader parameter=[{name}] because it does not exist in the compiled shader=[{Asset.AssetName}]");
        }
    }

    public void SetParameter(string name, Vector2 value)
    {
        if (TryGetParameter(name, out var parameter))
        {
            parameter.SetValue(value);
        }
        else
        {
            Console.WriteLine($"Warning: cannot set shader parameter=[{name}] because it does not exist in the compiled shader=[{Asset.AssetName}]");
        }
    }

    public void SetParameter(string name, Texture2D value)
    {
        if (TryGetParameter(name, out var parameter))
        {
            parameter.SetValue(value);
        }
        else
        {
            Console.WriteLine($"Warning: cannot set shader parameter=[{name}] because it does not exist in the compiled shader=[{Asset.AssetName}]");
        }
    }
    [Conditional("DEBUG")]
    public void Update()
    {
        if (Asset.TryRefresh(out var oldAsset))
        {
            UpdateParameterCache();

            // foreach (var oldParam in oldAsset.Parameters)
            // {
            //     if (!TryGetParameter(oldParam.Name, out var newParam))
            //     {
            //         continue;
            //     }

            //     switch (oldParam.ParameterClass)
            //     {
            //         case EffectParameterClass.Scalar:
            //             newParam.SetValue(oldParam.GetValueSingle());
            //             break;
            //         default:
            //             Console.WriteLine("Warning: shader reload system was not able to re-apply property. " +
            //                               $"shader=[{Effect.Name}] " +
            //                               $"property=[{oldParam.Name}] " +
            //                               $"class=[{oldParam.ParameterClass}]");
            //             break;
            //     }
            // }
        }
    }
}