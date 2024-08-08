using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay.RelayMode
{
    public class GameplayManager : MonoBehaviour, IMinigame
    {
        [SerializeField] private float mapLength;
        [SerializeField] private Transform mapPb;
        [SerializeField] private LevelStage[] levelData;
        [SerializeField] private Player playerPb;
        [SerializeField] private Player.Mode CurrentMode;

        private int levelIdx;
        private Transform[] maps;
        private IMinigame.Data myData;
        private MinigameUI ui;
        private int levelScore;
        private int playerScore;
        private Dictionary<Player.Mode, List<LevelStage>> levels;

        private int mapCount;
        private (Transform Current, Transform Next) map;
        private (LevelStage Current, Player.Mode CurMode, LevelStage Next) level;
        private (Player Current, Player Next) player;

        public Transform GameplayHolder { get => transform; }
        public IMinigame.Data ExternalData { get => myData; set => myData = value; }

        private void Awake()
        {
            ui = FindAnyObjectByType<MinigameUI>();
        }
        private void Start()
        {
            if (myData == null)
            {
                myData = new IMinigame.Data()
                {
                    coin = 0,
                    score = 0,
                };
            }
            levelScore = 10;

            levelIdx = (int)CurrentMode;
            level = (null, CurrentMode, null);

            // Set Map
            InitMode();
            SetupMap();
            CreatePlayer();
            CreateLevel();

            player.Current.Play();

            EventManager.OnInitGame?.Invoke();

            EventManager.OnPlayerClaimNewStar += OnClaimExperience;
            EventManager.OnPlayerIsMoving += OnPlayerIsMoving;
            EventManager.OnPlayerIsPassedHalfStage += OnPlayerIsPassedHalfStage;
            EventManager.OnFinishStage += OnFinishStage;
        }

        private void OnDestroy()
        {
            EventManager.OnPlayerClaimNewStar -= OnClaimExperience;
            EventManager.OnPlayerIsMoving -= OnPlayerIsMoving;
            EventManager.OnPlayerIsPassedHalfStage -= OnPlayerIsPassedHalfStage;
            EventManager.OnFinishStage -= OnFinishStage;
        }

        private void InitMode()
        {
            // Init Map
            maps = new Transform[]
            {
                Instantiate(mapPb, transform),
                Instantiate(mapPb, transform),
            };

            // Init Level
            levels = new Dictionary<Player.Mode, List<LevelStage>>();
            foreach (var item in levelData)
            {
                if (!levels.ContainsKey(item.Mode))
                {
                    var newStage = new List<LevelStage>();
                    newStage.Add(item);
                    levels.Add(item.Mode, newStage);
                }
                else
                {
                    levels[item.Mode].Add(item);
                }
            }

        }

        private void OnFinishStage()
        {
           
        }

        private void OnPlayerIsPassedHalfStage(Base.Player player)
        {
            if(level.Current.IsFinal)
            {
                Debug.Log("Completed All Level!!!!!!!!!!!");
            }
            else
            {
                CreateLevel();
                CreatePlayer();
            }
        }

        private void CreatePlayer()
        {
            var pos = Vector3.zero;
            if (level.Next == null)
            {
                pos = new Vector3(-4.5f, playerPb.transform.position.y, playerPb.transform.position.z);
            }
            else
            {
                pos = level.Next.transform.position + Vector3.right * 2;
            }
            player.Current = Instantiate(playerPb,transform);
            player.Current.transform.position = pos;
            player.Current.Setup(level.CurMode);
        }

        private Player.Mode GetLevelMode(int id)
        {
            switch (id)
            {
                case 0:
                    return Player.Mode.Hurdling;
                case 1:
                    return Player.Mode.Passthrough;
                case 2:
                    return Player.Mode.Pathway2;
            }
            return Player.Mode.Hurdling;
        }

        void CreateLevel()
        {
            if (level.Next == null) { level.Next = levels[CurrentMode][Random.Range(0, levels[CurrentMode].Count)]; }
            if (player.Current == null) CreatePlayer();

            var levelPb = level.Next;
            var myLevel = Instantiate(levelPb,
                new Vector3(player.Current.transform.position.x + 4.5f, levelPb.transform.position.y, levelPb.transform.position.z),
                levelPb.transform.rotation,
                transform);
            myLevel.Assign(level.CurMode == Player.Mode.Pathway2);

            levelIdx++;
            if (levelIdx == 3) levelIdx = 0;

            var nextMode = GetLevelMode(levelIdx);
            var nextLevel = levels[nextMode][Random.Range(0, levels[nextMode].Count)];

            level = (myLevel, myLevel.Mode, nextLevel);
        }
        void SetupMap()
        {
            if (mapCount >= maps.Length) mapCount = 0;
            map = (maps[mapCount], maps[mapCount + 1 >= maps.Length ? 0 : mapCount + 1]);
            mapCount++;

            map.Next.position = map.Current.position + Vector3.right * mapLength;
        }

        private void OnPlayerIsMoving(Base.Player player)
        {
            if(player.transform.position.x >= map.Next.position.x)
            {
                SetupMap();
            }
        }

        private void OnClaimExperience(Base.Player player)
        {
            playerScore++;
            ui.UpdateLoadingBar((float)playerScore / levelScore);
        }

        public void OnGamePause()
        {
        }

        public void OnGameResume()
        {
        }

        public void OnGameStart()
        {
        }

        public void OnGameStop()
        {
        }
    }
}
