using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager instance;

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

    public bool citySelected;
    public List<City> attackingCities = new List<City>();
    private void OnValidate()
    {
        UpdateCityName(_MyCity.ToString());
    }
    
    public enum MyCity
    {
        BlueCity,RedCity,BlankCity
    }

    [SerializeField]
    private MyCity myCity;

    public MyCity _MyCity
    {
        get
        {
            return myCity;
        }
        set
        {
            myCity = value;
            UpdateCityName(value.ToString());
        }
    }

    public string cityName;

    void UpdateCityName(string nameOfCity)
    {
        cityName = nameOfCity;
    }
}
