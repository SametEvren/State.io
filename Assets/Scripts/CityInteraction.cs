using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class CityInteraction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool isEntered;
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
        
        if (Input.GetMouseButtonDown(0) && GetComponent<City>()._CityState.ToString() ==  GM.cityName && isEntered)
        {
            arrow.SetActive(true);
            GM.citySelected = true;
            GM.attackingCities.Add(gameObject.GetComponent<City>());
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
                int a = j;
                GM.attackingCities[j]._RaidState = City.RaidState.Raiding;
                attackingCitizensCount += GM.attackingCities[j].CitizenCount;
                DOTween.To(() => GM.attackingCities[a].CitizenCount, x => GM.attackingCities[a].CitizenCount = x,
                    0, 1);
            }
            GetComponent<City>()._RaidState = City.RaidState.GetRaid;

            if (attackingCitizensCount <= GetComponent<City>().CitizenCount)
            {
                DOTween.To(() => GetComponent<City>().CitizenCount, x => GetComponent<City>().CitizenCount = x,
                    GetComponent<City>().CitizenCount - attackingCitizensCount, 1).OnComplete(() =>
                {
                    for (int i = 0; i < GM.attackingCities.Count; i++)
                    {
                        GM.attackingCities[i]._RaidState = City.RaidState.Stable;
                    }

                    GetComponent<City>()._RaidState = City.RaidState.Stable;
                    GM.attackingCities.Clear();
                });
            }

            if (attackingCitizensCount > GetComponent<City>().CitizenCount)
            {
                int plusValue = attackingCitizensCount - GetComponent<City>().CitizenCount;
                DOTween.To(() => GetComponent<City>().CitizenCount, x => GetComponent<City>().CitizenCount = x,
                    0, 0.5f).OnComplete(() =>
                {

                    GetComponent<City>()._CityState = GM.attackingCities[0]._CityState;
                    DOTween.To(() => GetComponent<City>().CitizenCount, x => GetComponent<City>().CitizenCount = x,
                        plusValue, 0.5f).OnComplete(() =>
                    {
                        for (int i = 0; i < GM.attackingCities.Count; i++)
                        {
                            GM.attackingCities[i]._RaidState = City.RaidState.Stable;
                        }

                        GetComponent<City>()._RaidState = City.RaidState.Stable;
                        GM.attackingCities.Clear();
                    });
                });
                
            }


        }
        
    }
}
