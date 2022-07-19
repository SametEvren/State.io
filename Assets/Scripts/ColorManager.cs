using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorManager : MonoBehaviour
{
    #region Singleton
    public static ColorManager instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    #endregion
    public Color insideBlue, outsideBlue, insideRed, outsiteRed, insideBlank, outsideBlank;
}
