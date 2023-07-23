using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class RandomizeSpriteLibrary : MonoBehaviour {
    public SpriteLibrary SpriteLibrary;
    public SpriteLibraryAsset[] SpriteLibraryAssets;
    
    // Start is called before the first frame update
    void Start() {
        if (SpriteLibraryAssets is { Length: > 0 })
            SpriteLibrary.spriteLibraryAsset = SpriteLibraryAssets [Random.Range(0, SpriteLibraryAssets.Length)];
    }

}
