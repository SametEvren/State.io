using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CityInteraction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool isEntered;
    public bool isSelected;
    public GameObject arrow;
    public int attackingCitizensCount;
    public void OnPointerEnter(PointerEventData eventData)
    {
        isEntered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isEntered = false;
    }

    private void Update()
    {
        GameManager GM = GameManager.instance;
        VariableManager VM = VariableManager.instance;
        
        if (Input.GetMouseButtonDown(0) && GetComponent<City>()._CityState.ToString() ==  GM.cityName && isEntered)
        {
            if (!isSelected)
            {
                arrow.SetActive(true);
                isSelected = true;
                GM.citySelected = true;
                GM.attackingCities.Add(gameObject.GetComponent<City>());
            }
        }

        if (Input.GetMouseButtonDown(1) && GetComponent<City>()._CityState.ToString() == GM.cityName)
        {
            arrow.SetActive(false);
            GM.citySelected = false;
            GM.attackingCities.Clear();
        }
        
        if (Input.GetMouseButtonDown(0) && GetComponent<City>()._CityState.ToString() !=  GM.cityName && isEntered && GM.citySelected == true)
        {
            attackingCitizensCount = 0;
            for (int j = 0; j < GM.attackingCities.Count; j++)
            {
                AttackToOtherCity(GM.attackingCities[j].CitizenCount,GM.attackingCities[j].gameObject,gameObject);
                int a = j;
                GM.attackingCities[j]._RaidState = City.RaidState.Raiding;
                GM.attackingCities[j].GetComponent<CityInteraction>().arrow.SetActive(false);
                attackingCitizensCount += GM.attackingCities[j].CitizenCount;
                DOTween.To(() => GM.attackingCities[a].CitizenCount, x => GM.attackingCities[a].CitizenCount = x,
                    0, VM.gameFlowSpeed).SetEase(Ease.Linear);
            }
            GetComponent<City>()._RaidState = City.RaidState.GetRaid;

            
            if (attackingCitizensCount <= GetComponent<City>().CitizenCount)
            {
                DOTween.To(() => GetComponent<City>().CitizenCount, x => GetComponent<City>().CitizenCount = x,
                    GetComponent<City>().CitizenCount - attackingCitizensCount, VM.gameFlowSpeed).OnComplete(() =>
                {
                    for (int i = 0; i < GM.attackingCities.Count; i++)
                    {
                        GM.attackingCities[i]._RaidState = City.RaidState.Stable;
                        GM.attackingCities[i].GetComponent<CityInteraction>().isSelected = false;
                    }

                    GetComponent<City>()._RaidState = City.RaidState.Stable;
                    GM.attackingCities.Clear();
                }).SetEase(Ease.Linear);
            }

            if (attackingCitizensCount > GetComponent<City>().CitizenCount)
            {
                int plusValue = attackingCitizensCount - GetComponent<City>().CitizenCount;
                DOTween.To(() => GetComponent<City>().CitizenCount, x => GetComponent<City>().CitizenCount = x,
                    0, VM.gameFlowSpeed/2f).OnComplete(() =>
                {

                    GetComponent<City>()._CityState = GM.attackingCities[0]._CityState;
                    DOTween.To(() => GetComponent<City>().CitizenCount, x => GetComponent<City>().CitizenCount = x,
                        plusValue, VM.gameFlowSpeed/2f).OnComplete(() =>
                    {
                        for (int i = 0; i < GM.attackingCities.Count; i++)
                        {
                            GM.attackingCities[i]._RaidState = City.RaidState.Stable;
                            GM.attackingCities[i].GetComponent<CityInteraction>().isSelected = false;

                        }

                        GetComponent<City>()._RaidState = City.RaidState.Stable;
                        GM.attackingCities.Clear();
                    }).SetEase(Ease.Linear);
                }).SetEase(Ease.Linear);
                
            }


        }
    }

    public void AttackToOtherCity(int citizenCount, GameObject fromCity, GameObject toCity)
    {
        GameManager GM = GameManager.instance;
        VariableManager VM = VariableManager.instance;
        for (int i = 0; i < citizenCount; i++)
        {
            Vector3 randFrom = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);

            GameObject citizen = Instantiate(GM.citizenPrefab, fromCity.GetComponent<RectTransform>().position + randFrom,
                Quaternion.identity,GM.canvas.transform);

            if (fromCity.GetComponent<City>()._CityState == City.CityState.BlueCity)
                citizen.GetComponent<Image>().color = fromCity.GetComponent<ColorManager>().insideBlue;
            if (fromCity.GetComponent<City>()._CityState == City.CityState.RedCity)
                citizen.GetComponent<Image>().color = fromCity.GetComponent<ColorManager>().insideRed;
            
            
            citizen.transform.DOMove(toCity.GetComponent<RectTransform>().position, VariableManager.instance.gameFlowSpeed).OnComplete(() =>
            {
                Destroy(citizen);
            }).SetEase(Ease.Linear);
        }
    }
}

