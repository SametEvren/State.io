using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CityInteraction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool isEntered;
    public GameObject arrow;
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
        if (Input.GetMouseButtonDown(0) && GetComponent<City>()._CityState.ToString() ==  GameManager.instance.cityName && isEntered)
        {
            arrow.SetActive(true);
        }
    }
}
