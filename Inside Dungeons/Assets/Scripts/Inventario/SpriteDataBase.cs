using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteDataBase : MonoBehaviour
{
    public spriteData[] sprites;
    

    public Sprite getSpriteByID(int id)
    {
        for (int i = 0; i < sprites.Length; i++)
        {
            if (sprites[i].Id == id)
            {
                return sprites[i].image;
            }
        }
        return null;
    }
}

[System.Serializable]
public struct spriteData
{
    public int Id;
    public Sprite image;
}

