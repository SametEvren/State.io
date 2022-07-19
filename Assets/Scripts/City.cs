using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class City : MonoBehaviour
{
    public enum CityState
    {
        BlueCity,RedCity,BlankCity
    }

    [SerializeField]
    private CityState cityState;

    public CityState _CityState
    {
        get
        {
            return cityState;
        }
        set
        {
            cityState = value;
            print(value.ToString());
            ChangeCityColor(value.ToString());
        }
    }

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
    public Image outerCity, innerCity;
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
        if ((cityState == CityState.BlueCity || cityState == CityState.RedCity) &&
            CitizenCount == VM.maxCitizenCountTakenCity)
            yield break;
        if(cityState == CityState.BlankCity && CitizenCount == VM.maxCitizenCountBlankCity)
            yield break;
        CitizenCount += 1;
        
        StartCoroutine(IncreaseCitizen());
    }

    public void ChangeCityColor(string CityName)
    {
        ColorManager CM = ColorManager.instance;
        if (CityName == "BlueCity")
        {
            outerCity.color = CM.outsideBlue;
            innerCity.color = CM.insideBlue;
        }
        if (CityName == "RedCity")
        {
            outerCity.color = CM.outsiteRed;
            innerCity.color = CM.insideRed;
        }
        if (CityName == "BlankCity")
        {
            outerCity.color = CM.outsideBlank;
            innerCity.color = CM.insideBlank;
        }
    }
}
