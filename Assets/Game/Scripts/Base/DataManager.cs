using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WFSport.Gameplay;
using static WFSport.Base.ConfigDataManager;

namespace WFSport.Base
{
    public class DataManager : SingletonBind<DataManager>
    {
        [SerializeField] public ConfigDataManager configDataManager;
        [SerializeField] public AssetDataManager assetDataManager;
        [SerializeField] public LocalDataManager localSaveloadData;
        [SerializeField] private string minigamesPath;

        internal GameObject OrderMinigame(GameController.Minigame mode)
        {
            var modes = Resources.LoadAll<GameObject>(minigamesPath);

            GameObject data = null;
            switch (mode)
            {
                case GameController.Minigame.Archery:
                    foreach (var item in modes)
                    {
                        var temp = item.GetComponent<Gameplay.ArcheryMode.GameplayManager>();
                        if (temp != null)
                        {
                            data = temp.gameObject;
                            break;
                        }
                    }
                    break;
                case GameController.Minigame.BasketBall:
                    foreach (var item in modes)
                    {
                        var temp = item.GetComponent<Gameplay.BasketballMode.GameplayManager>();
                        if (temp != null)
                        {
                            data = temp.gameObject;
                            break;
                        }
                    }
                    break;
                case GameController.Minigame.CatchMoreToys:
                    foreach (var item in modes)
                    {
                        var temp = item.GetComponent<Gameplay.CatchMoreToysMode.GameplayManager>();
                        if (temp != null)
                        {
                            data = temp.gameObject;
                            break;
                        }
                    }
                    break;
                case GameController.Minigame.CreateEnergy:
                    foreach (var item in modes)
                    {
                        var temp = item.GetComponent<Gameplay.CreateEnergyMode.GameplayManager>();
                        if (temp != null)
                        {
                            data = temp.gameObject;
                            break;
                        }
                    }
                    break;
                case GameController.Minigame.Latin:
                    foreach (var item in modes)
                    {
                        var temp = item.GetComponent<Gameplay.LatinDanceMode.GameplayManager>();
                        if (temp != null)
                        {
                            data = temp.gameObject;
                            break;
                        }
                    }
                    break;
                case GameController.Minigame.Relay:
                    foreach (var item in modes)
                    {
                        var temp = item.GetComponent<Gameplay.RelayMode.GameplayManager>();
                        if (temp != null)
                        {
                            data = temp.gameObject;
                            break;
                        }
                    }
                    break;
                case GameController.Minigame.Snowball:
                    foreach (var item in modes)
                    {
                        var temp = item.GetComponent<Gameplay.SnowballMode.GameplayManager>();
                        if (temp != null)
                        {
                            data = temp.gameObject;
                            break;
                        }
                    }
                    break;
            }
            if (data != null)
            {
                Debug.Log($"Order Minigame {mode} Successfully...");
                return data;
            }
            else
            {
                Debug.LogError($"<!> Order Minigame {mode} Error !!! Please Link your mode in folder Assets/Resources/{minigamesPath}");
                return null;
            }
        }
    }
}
