using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Bootstrap : MonoBehaviour
{
    [SerializeField] private bool preloadAds = true;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (preloadAds)
        {
            PreloadAds();
        }
    }
    
partial void PreloadAds();
}
