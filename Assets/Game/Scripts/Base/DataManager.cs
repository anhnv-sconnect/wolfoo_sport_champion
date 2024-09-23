using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WFSport.Gameplay;

namespace WFSport.Base
{
    public class DataManager : SingletonBind<DataManager>
    {
        [SerializeField] public ConfigDataManager configDataManager;
        [SerializeField] private AssetDataManager assetDataManager;
        [SerializeField] public LocalDataManager localSaveloadData;
        [SerializeField] private string minigamesPath;

        internal T OrderAsset<T>(Minigame game) where T: IAsset
        {
            var rs = default(T);
            switch (game)
            {
                case Minigame.Archery:
                    break;
                case Minigame.BasketBall:
                    break;
                case Minigame.CatchMoreToys:
                    break;
                case Minigame.CreateEnergy:
                    break;
                case Minigame.Latin:
                    break;
                case Minigame.Relay:
                    break;
                case Minigame.Snowball:
                    break;
                case Minigame.Furniture:
                    rs = (T) (object) assetDataManager.FurnitureAsset;
                    return rs;
            }

            return rs;
        }

        internal GameObject OrderMinigame(Minigame mode)
        {
            var modes = Resources.LoadAll<GameObject>(minigamesPath);

            GameObject data = null;
            switch (mode)
            {
                case Minigame.Archery:
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
                case Minigame.BasketBall:
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
                case Minigame.CatchMoreToys:
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
                case Minigame.CreateEnergy:
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
                case Minigame.Latin:
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
                case Minigame.Relay:
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
                case Minigame.Snowball:
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
                case Minigame.Furniture:
                    foreach (var item in modes)
                    {
                        var temp = item.GetComponent<Gameplay.FurnitureMode.GameplayManager>();
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
