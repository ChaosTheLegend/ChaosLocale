using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TestAsset")]
public class TestAsset : ScriptableObject
{
    public Sprite spr1;
    public Sprite spr2;
    public bool useSprite1;

    public static explicit operator Sprite(TestAsset asset)
    {
        var spr = asset.useSprite1 ? asset.spr1 : asset.spr2;
        return spr;
    }
    
    public static explicit operator TestAsset(Sprite sprite)
    {
        var asset = ScriptableObject.CreateInstance<TestAsset>();
        asset.spr1 = sprite;
        asset.useSprite1 = true;
        return asset;
    }
}

