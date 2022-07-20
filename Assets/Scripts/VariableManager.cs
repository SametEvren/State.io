using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariableManager : MonoBehaviour
{
    #region Singleton
    public static VariableManager instance;

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

    public int gameFlowSpeed;
    public int maxCitizenCountTakenCity;
    public int maxCitizenCountBlankCity;
    public int aiAttackFrequencyMin, aiAttackFrequencyMax;
}
