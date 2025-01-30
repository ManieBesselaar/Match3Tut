// Based on  https://www.youtube.com/watch?v=ErHEZ5YGQ5M
//Thank you git-amend!
using System;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Match3
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Gem:MonoBehaviour 
    {
        public Gemtype Type;

        public void SetType(Gemtype type)
        {
            Type = type;
            GetComponent<SpriteRenderer>().sprite = type.sprite;
        }
        public Gemtype GetGemType() => Type;

        internal void DestroyGem()
        {
           Destroy(gameObject);
        }
    }
   
        }
    
