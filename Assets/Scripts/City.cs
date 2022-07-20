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
    public enum RaidState
    {
        Stable,Raiding,GetRaid
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
            ChangeCityColor(value.ToString());
        }
    }
    
    [SerializeField]
    private RaidState raidState;

    public RaidState _RaidState
    {
        get
        {
            return raidState;
        }
        set
        {
            raidState = value;
        }
    }
    
    
    [SerializeField]
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
    private void OnValidate()
    {
        ColorManager CM = GetComponent<ColorManager>();
        if (_CityState == CityState.BlueCity)
        {
            outerCity.color = CM.outsideBlue;
            innerCity.color = CM.insideBlue;
        }
        if (_CityState == CityState.RedCity)
        {
            outerCity.color = CM.outsiteRed;
            innerCity.color = CM.insideRed;
        }
        if (_CityState == CityState.BlankCity)
        {
            outerCity.color = CM.outsideBlank;
            innerCity.color = CM.insideBlank;
        }
    }
    
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
        {
            StartCoroutine(IncreaseCitizen());
            yield break;
        }

        if (cityState == CityState.BlankCity && CitizenCount == VM.maxCitizenCountBlankCity)
        {
            StartCoroutine(IncreaseCitizen());
            yield break;
        }

        if (_RaidState != RaidState.Stable)
        {
            StartCoroutine(IncreaseCitizen());
            yield break;
        }
        CitizenCount += 1;
        StartCoroutine(IncreaseCitizen());
    }

    public void ChangeCityColor(string CityName)
    {
        ColorManager CM = GetComponent<ColorManager>();
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

    private void Update()
    {
        CitizenCount = Mathf.Clamp(CitizenCount, 0, 50);
    }
}
