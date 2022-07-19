using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class City : MonoBehaviour
{
    public enum CityState
    {
        BlueTeam,RedTeam,Blank
    }

    public CityState cityState;

    private int citizenCount;

    public int CitizenCount
    {
        get
        {
            return citizenCount;
        }
        set
        {
            citizenCount = value;
            UpdateUI();
        }
    }

    public TextMeshProUGUI citizenText;

    private void Start()
    {
        StartCoroutine(IncreaseCitizen());
    }

    private void UpdateUI()
    {
        citizenText.text = CitizenCount.ToString();
    }

    private IEnumerator IncreaseCitizen()
    {
        VariableManager VM = VariableManager.instance;
        yield return new WaitForSeconds(VM.gameFlowSpeed);
        if ((cityState == CityState.BlueTeam || cityState == CityState.RedTeam) &&
            CitizenCount == VM.maxCitizenCountTakenCity)
            yield break;
        if(cityState == CityState.Blank && CitizenCount == VM.maxCitizenCountBlankCity)
            yield break;
        CitizenCount += 1;
        StartCoroutine(IncreaseCitizen());
    }
}
