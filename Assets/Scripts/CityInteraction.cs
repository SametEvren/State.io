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

    private void Start()
    {
        StartCoroutine(CheckAI(Random.Range(VariableManager.instance.aiAttackFrequencyMin,VariableManager.instance.aiAttackFrequencyMax)));
    }

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
            isSelected = false;
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
            
            //Succesfully raided
            if (attackingCitizensCount > GetComponent<City>().CitizenCount)
            {
                int plusValue = attackingCitizensCount - GetComponent<City>().CitizenCount;
                DOTween.To(() => GetComponent<City>().CitizenCount, x => GetComponent<City>().CitizenCount = x,
                    0, VM.gameFlowSpeed/2f).OnComplete(() =>
                {
                    if (GetComponent<City>()._CityState == City.CityState.BlankCity)
                    {
                        CityManager.instance.BlankCities.Remove(GetComponent<City>());
                        
                        if(GameManager.instance._MyCity == GameManager.MyCity.BlueCity)
                            CityManager.instance.BlueCities.Add(GetComponent<City>());
                        if(GameManager.instance._MyCity == GameManager.MyCity.RedCity)
                            CityManager.instance.RedCities.Add(GetComponent<City>());
                    }
                    if (GetComponent<City>()._CityState == City.CityState.RedCity)
                    {
                        CityManager.instance.RedCities.Remove(GetComponent<City>());
                        
                        if(GameManager.instance._MyCity == GameManager.MyCity.BlueCity)
                            CityManager.instance.BlueCities.Add(GetComponent<City>());
                    }
                    if (GetComponent<City>()._CityState == City.CityState.BlueCity)
                    {
                        CityManager.instance.BlueCities.Remove(GetComponent<City>());
                        
                        if(GameManager.instance._MyCity == GameManager.MyCity.RedCity)
                            CityManager.instance.RedCities.Add(GetComponent<City>());
                    }
                    
                    
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
    
    public IEnumerator CheckAI(float randomSeconds)
    {
        yield return new WaitForSeconds(randomSeconds);
        GameManager GM = GameManager.instance;
        VariableManager VM = VariableManager.instance;

        if(GetComponent<City>()._CityState.ToString() ==  "BlankCity") 
            yield break;

        if (GetComponent<City>()._CityState.ToString() != GM.cityName && GetComponent<City>()._RaidState == City.RaidState.Stable)
        {
            int rand = Random.Range(0, 2);
            attackingCitizensCount = 0;
            
            
            
            
            
            
            if (GameManager.instance._MyCity == GameManager.MyCity.BlueCity)
            {
                if(CityManager.instance.BlueCities.Count == 0 && CityManager.instance.BlankCities.Count == 0)
                    yield break;
                if(rand == 0)
                {
                    if (CityManager.instance.BlueCities.Count == 0)
                    {
                        StartCoroutine(CheckAI(1));
                        yield break;
                    }
                    GetComponent<City>()._RaidState = City.RaidState.Raiding;
                    attackingCitizensCount +=  GetComponent<City>().CitizenCount;
                    DOTween.To(() =>  GetComponent<City>().CitizenCount, x => GetComponent<City>().CitizenCount = x,
                        0, VM.gameFlowSpeed).SetEase(Ease.Linear);
                    int randCity = Random.Range(0, CityManager.instance.BlueCities.Count);
                    AttackToOtherCity(GetComponent<City>().CitizenCount, gameObject, 
                        CityManager.instance.BlueCities[randCity].gameObject);
                    Attack(GetComponent<City>().CitizenCount,GetComponent<City>(),CityManager.instance.BlueCities[randCity]);
                }
                if(rand == 1)
                {
                    if (CityManager.instance.BlankCities.Count == 0)
                    {
                        StartCoroutine(CheckAI(0));
                        yield break;
                    }
                    GetComponent<City>()._RaidState = City.RaidState.Raiding;
                    attackingCitizensCount +=  GetComponent<City>().CitizenCount;
                    DOTween.To(() =>  GetComponent<City>().CitizenCount, x => GetComponent<City>().CitizenCount = x,
                        0, VM.gameFlowSpeed).SetEase(Ease.Linear);
                    int randCity = Random.Range(0, CityManager.instance.BlankCities.Count);
                    AttackToOtherCity(GetComponent<City>().CitizenCount, gameObject, 
                        CityManager.instance.BlankCities[randCity].gameObject);
                    Attack(GetComponent<City>().CitizenCount,GetComponent<City>(),CityManager.instance.BlankCities[randCity]);
                }
            }
            
            if (GameManager.instance._MyCity == GameManager.MyCity.RedCity)
            {
                if(CityManager.instance.RedCities.Count == 0 && CityManager.instance.BlankCities.Count == 0)
                    yield break;
                
                if (rand == 0)
                {
                    if (CityManager.instance.RedCities.Count == 0)
                    {
                        StartCoroutine(CheckAI(1));
                        yield break;
                    }
                    GetComponent<City>()._RaidState = City.RaidState.Raiding;
                    attackingCitizensCount +=  GetComponent<City>().CitizenCount;
                    DOTween.To(() =>  GetComponent<City>().CitizenCount, x => GetComponent<City>().CitizenCount = x,
                        0, VM.gameFlowSpeed).SetEase(Ease.Linear);
                    int randCity = Random.Range(0, CityManager.instance.RedCities.Count);
                    AttackToOtherCity(GetComponent<City>().CitizenCount, gameObject,
                        CityManager.instance.RedCities[randCity].gameObject);
                    Attack(GetComponent<City>().CitizenCount,GetComponent<City>(),CityManager.instance.RedCities[randCity]);
                }

                if(rand == 1)
                {
                    if (CityManager.instance.BlankCities.Count == 0)
                    {
                        StartCoroutine(CheckAI(0));
                        yield break;
                    }
                    GetComponent<City>()._RaidState = City.RaidState.Raiding;
                    attackingCitizensCount +=  GetComponent<City>().CitizenCount;
                    DOTween.To(() =>  GetComponent<City>().CitizenCount, x => GetComponent<City>().CitizenCount = x,
                        0, VM.gameFlowSpeed).SetEase(Ease.Linear);
                    int randCity = Random.Range(0, CityManager.instance.BlankCities.Count);
                    AttackToOtherCity(GetComponent<City>().CitizenCount, gameObject, 
                        CityManager.instance.BlankCities[randCity].gameObject);
                    Attack(GetComponent<City>().CitizenCount,GetComponent<City>(),CityManager.instance.BlankCities[randCity]);
                }
            }
            
        }
        else if (GetComponent<City>()._CityState.ToString() != GM.cityName &&
                 GetComponent<City>()._RaidState != City.RaidState.Stable)
        {
            StartCoroutine(CheckAI(Random.Range(VariableManager.instance.aiAttackFrequencyMin,
                VariableManager.instance.aiAttackFrequencyMax)));
        }
    }


    public void Attack(int citizenCount, City attackingCity, City attackedCity)
    {
        GameManager GM = GameManager.instance;
        VariableManager VM = VariableManager.instance;
        
        if (citizenCount <= attackedCity.CitizenCount)
        {
            attackingCity._RaidState = City.RaidState.Raiding;
            attackedCity._RaidState = City.RaidState.GetRaid;
            DOTween.To(() => attackingCity.CitizenCount, x => attackingCity.CitizenCount = x,
                0, VM.gameFlowSpeed).SetEase(Ease.Linear);
            
            DOTween.To(() => attackedCity.CitizenCount, x => attackedCity.CitizenCount = x,
                attackedCity.CitizenCount - citizenCount, VM.gameFlowSpeed).OnComplete(() =>
            {
                attackedCity._RaidState = City.RaidState.Stable;
                attackingCity._RaidState = City.RaidState.Stable;
                GM.attackingCities.Clear();
                StartCoroutine(CheckAI(Random.Range(VariableManager.instance.aiAttackFrequencyMin,VariableManager.instance.aiAttackFrequencyMax)));
            }).SetEase(Ease.Linear);
        }
            
        
        
        
            //Succesfully raided
            if (citizenCount > attackedCity.CitizenCount)
            {
                int plusValue = citizenCount - attackedCity.CitizenCount;
                DOTween.To(() => attackedCity.CitizenCount, x => attackedCity.CitizenCount = x,
                    0, VM.gameFlowSpeed/2f).OnComplete(() =>
                {
                    if (attackedCity._CityState == City.CityState.BlankCity)
                    {
                        CityManager.instance.BlankCities.Remove(attackedCity);
                        
                        if(attackingCity._CityState == City.CityState.BlueCity)
                            CityManager.instance.BlueCities.Add(attackedCity);
                        if(attackingCity._CityState == City.CityState.RedCity)
                            CityManager.instance.RedCities.Add(attackedCity);
                    }
                    if (attackedCity._CityState == City.CityState.RedCity)
                    {
                        CityManager.instance.RedCities.Remove(attackedCity);
                        
                        if(attackingCity._CityState == City.CityState.BlueCity)
                            CityManager.instance.BlueCities.Add(attackedCity);
                    }
                    if (attackedCity._CityState == City.CityState.BlueCity)
                    {
                        CityManager.instance.BlueCities.Remove(attackedCity);
                        
                        if(attackingCity._CityState == City.CityState.RedCity)
                            CityManager.instance.RedCities.Add(attackedCity);
                    }
                    
                    
                    attackedCity._CityState = attackingCity._CityState;
                    
                    DOTween.To(() => attackedCity.CitizenCount, x => attackedCity.CitizenCount = x,
                        plusValue, VM.gameFlowSpeed/2f).OnComplete(() =>
                    {
                            attackingCity._RaidState = City.RaidState.Stable;
                            attackedCity._RaidState = City.RaidState.Stable;
                            attackedCity.GetComponent<CityInteraction>().StartChecking();
                            StartCoroutine(CheckAI(Random.Range(VariableManager.instance.aiAttackFrequencyMin,VariableManager.instance.aiAttackFrequencyMax)));
                    }).SetEase(Ease.Linear);
                }).SetEase(Ease.Linear);
                
            }
    }

    public void StartChecking()
    {
        StartCoroutine(CheckAI(
    Random.Range(VariableManager.instance.aiAttackFrequencyMin,
        VariableManager.instance.aiAttackFrequencyMax)));    
    }

}

