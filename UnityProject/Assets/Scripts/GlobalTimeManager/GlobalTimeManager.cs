using UnityEngine;
using Unity.Netcode;
using System;
using Unity.Mathematics;
public class GlobalTimeManager : NetworkBehaviour
{
    public static GlobalTimeManager instance { get; private set; }
    public enum Season { Spring,Summer,Autumn,Winter}
    public enum Weather { Sunny,Raining,Storm,Snowy}
    public enum DailyEvent { None,HarvestFestival,MarketDay }

    public static event Action<int> OnDayChanged;
    public static event Action<Season> OnSeasonChanged;
    public static event Action<Weather> OnWeatherChanged;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("每日广播变量  网络状态同步")]
    public NetworkVariable<int> currentDay = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone);
    public NetworkVariable<Season> season=new NetworkVariable<Season> (Season.Spring,NetworkVariableReadPermission.Everyone);
    public NetworkVariable<Weather> weather = new NetworkVariable<Weather>(Weather.Sunny, NetworkVariableReadPermission.Everyone);
    [Tooltip("游戏开局时间戳）")]
    public NetworkVariable<long> serverStartTimeStamp = new NetworkVariable<long>(0, NetworkVariableReadPermission.Everyone);

    [Header("时间设定")]
    [Tooltip("现实多少秒等于游戏中的一天")]
    public float realSecondsPerDay = 300f;
    //public NetworkVariable<float> everyDayTime = new NetworkVariable<float>(5f,NetworkVariableReadPermission.Everyone);

    // Update is called once per frame
    private void Awake()
    {
        if(instance!=null&&instance!=this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    public override void OnNetworkSpawn()
    {
       if (IsServer)
        {
            if(serverStartTimeStamp.Value==0)
            {
                serverStartTimeStamp.Value = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();//初始化时间戳
            }
        }
        currentDay.OnValueChanged += (oldValue, newValue) => OnDayChanged?.Invoke(newValue);
        weather.OnValueChanged += (oldValue,newValue) => OnWeatherChanged?.Invoke(newValue);
        season.OnValueChanged += (oldValue, newValue) => OnSeasonChanged?.Invoke(newValue);
        if(IsClient)
        {
            OnDayChanged?.Invoke(currentDay.Value);
            OnWeatherChanged?.Invoke(weather.Value);
            OnSeasonChanged?.Invoke(season.Value);
        }
    }
    public override void OnNetworkDespawn()
    {
        currentDay.OnValueChanged-=(oldValue,newValue)=>OnDayChanged?.Invoke(newValue);
        season.OnValueChanged -= (oldValue, newValue) => OnSeasonChanged?.Invoke(newValue);
        weather.OnValueChanged -= (oldValue, newValue) => OnWeatherChanged?.Invoke(newValue);
    }
    private void Update()
    {
        if (!IsServer || serverStartTimeStamp.Value == 0) return;
        long currentTimeMs=DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        long elapsedMs=currentTimeMs- serverStartTimeStamp.Value;
        long durationMsPerDay = (long)(realSecondsPerDay * 1000);
        int calculatedDay = (int)(elapsedMs / durationMsPerDay) + 1;
        if(calculatedDay>currentDay.Value)
        {
            currentDay.Value = calculatedDay;//天数增加
            //生成随机天气
            GenerateNewWeather();
        }

    }
    private void GenerateNewWeather()
    {
        if (!IsServer) return;
        Array weatherValues =Enum.GetValues(typeof(Weather));
        System.Random random=new System.Random();
        Weather nextWeather = (Weather)weatherValues.GetValue(random.Next(weatherValues.Length));
        weather.Value = nextWeather;
        Debug.Log($"今日天气{weather.Value}");
    }

}
