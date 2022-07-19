using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public void ClickedTheCity(string CityState)
    {
        if (CityState == GameManager.instance.cityName)
        {
            print("My City");
        }
        else
        {
            print("Another City");
        }
    }
}
