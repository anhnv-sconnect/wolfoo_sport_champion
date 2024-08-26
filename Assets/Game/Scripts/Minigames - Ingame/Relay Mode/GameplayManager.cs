using AnhNV.GameBase;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using SCN;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        [SerializeField] private Vector3 cameraRange;
        [SerializeField] private float levelScore;

        private IMinigame.Data myData;

        private int levelIdx;
        private int playerScore;
        private int mapCount;
        private int totalMode = (int)System.Enum.GetValues(typeof(Player.Mode)).Cast<Player.Mode>().Last();

        private Camera camera_;
        private Transform[] maps;
        private MinigameUI ui;

        private Dictionary<Player.Mode, List<LevelStage>> levels;
        private (Transform Current, Transform Next) map;
        private (LevelStage Current, LevelStage Next) level;
        private (Player Current, Player Next) player;

        private TweenerCore<Vector3, Vector3, VectorOptions> camTween;
        private Tutorial tutorialSwipeUp;
        private TutorialSwipe currentTutStep;
        private Barrier tutBarrier;
        private List<Barrier> tutBarriers = new List<Barrier>();
        private Tween _tweenDelay;

        public Transform GameplayHolder { get => transform; }
        public IMinigame.Data ExternalData { get => myData; set => myData = value; }

        private void Awake()
        {
            ui = FindAnyObjectByType<MinigameUI>();
        }
        private void Start()
        {
            OnGameStart();

            EventManager.OnPlayerClaimNewStar += OnClaimExperience;
            EventManager.OnPlayerIsMoving += OnPlayerIsMoving;
            EventManager.OnPlayerIsPassedHalfStage += OnPlayerIsPassedHalfStage;
            EventManager.OnFinishStage += OnGameWining;
            EventManager.OnTimeOut += OnGameLosing;
            EventManager.OnBarrierCompareDistanceWithPlayer += OnCompareDistancePlayer;
            EventDispatcher.Instance.RegisterListener<EventKeyBase.OpenDialog>(OnOpenDialog);
            EventDispatcher.Instance.RegisterListener<EventKeyBase.OnClosingDialog>(OnClosingDialog);

            EventManager.OnInitGame?.Invoke();
        }

        private void OnDestroy()
        {
            camTween?.Kill();
            _tweenDelay?.Kill();
            EventManager.OnPlayerClaimNewStar -= OnClaimExperience;
            EventManager.OnPlayerIsMoving -= OnPlayerIsMoving;
            EventManager.OnPlayerIsPassedHalfStage -= OnPlayerIsPassedHalfStage;
            EventManager.OnFinishStage -= OnGameWining;
            EventManager.OnTimeOut -= OnGameLosing;
            EventManager.OnBarrierCompareDistanceWithPlayer -= OnCompareDistancePlayer;
            EventDispatcher.Instance.RemoveListener<EventKeyBase.OpenDialog>(OnOpenDialog);
            EventDispatcher.Instance.RemoveListener<EventKeyBase.OnClosingDialog>(OnClosingDialog);
        }

        public void OnGameWining()
        {
            if (level.Current.IsFinal)
            {
                Debug.Log("Completed All Level !!!!!!!!!!!");
                ui.PauseTime();
                OnGameStop();
            }
            else
            {
                level.Current = level.Next;
                player.Current = player.Next;

                StartCoroutine("PlayNextLevel");
            }
        }

        public void OnGameLosing()
        {
            player.Current.Lose();
            OnGameStop();
        }

        public void OnGamePause()
        {
            ui.PauseTime();
            player.Current.Pause(true);
        }

        public void OnGameResume()
        {
            ui.PlayTime();
            player.Current.Play();
        }

        public void OnGameStart()
        {
            if (myData == null)
            {
                myData = new IMinigame.Data()
                {
                    coin = 0,
                    playTime = 180,
                    timelineScore = new float[] { levelScore / 3, levelScore * 2 / 3, levelScore }
                };
            }
            levelScore = myData.timelineScore[myData.timelineScore.Length - 1];

            levelIdx = (int)CurrentMode;
            level = (null, null);
            camera_ = Camera.main;

            ui.Setup(myData.playTime, myData.timelineScore);

            // Set Map
            InitMode();
            SetupMap();
            SetupNextStage();

            SetupTutorial();
            PlayTutorial();
            //     StartCoroutine("PlayNextLevel");
        }

        private void PlayTutorial()
        {
            CamMovingTo(player.Current.transform, 1, () =>
            {
                player.Current.AssignTutorial();
            });
        }

        private void OnCompareDistancePlayer(Barrier barrier, float distance)
        {
            if (!tutorialSwipeUp.IsAllStepCompleted && barrier != tutBarrier && distance < 3f)
            {
                tutBarrier = barrier;
                tutBarriers.Add(tutBarrier);

                player.Current.PlayTutorial();

                currentTutStep = tutorialSwipeUp.GetNextStep<TutorialSwipe>();
                currentTutStep.Setup(player.Current.transform, AnimatorHelper.Direction.Up);
                currentTutStep.Play();
                currentTutStep.OnSwipeCorrectDirection += OnCompleteStep;
            }
        }
        private void OnCompleteStep()
        {
            currentTutStep.OnSwipeCorrectDirection -= OnCompleteStep;

            Debug.Log("ON COmpleted Step");
            currentTutStep.Stop();

            if (tutorialSwipeUp.IsAllStepCompleted)
            {
                _tweenDelay = DOVirtual.DelayedCall(1, () =>
                {
                    ui.OpenLoading(() =>
                    {
                        ui.OpenCountingToStart(() =>
                        {
                            StartCoroutine("PlayNextLevel");
                        });
                    },
                    () =>
                    {
                        player.Current.Pause(true);
                        player.Current.ResetDefault();
                        foreach (var item in tutBarriers)
                        {
                            item.ResetDefault();
                        }

                        CamMovingTo(player.Current.transform, 0, null);
                    });
                });
            }
            else
            {
                player.Current.AssignTutorial();
            }
        }

        private void SetupTutorial()
        {
            var tutorialController = TutorialController.Instance;

            tutorialSwipeUp = tutorialController.CreateTutorial("RelayMode");
            tutorialController.CreateStep<TutorialSwipe>(tutorialSwipeUp);
            tutorialController.CreateStep<TutorialSwipe>(tutorialSwipeUp);
            tutorialController.CreateStep<TutorialSwipe>(tutorialSwipeUp);
        }

        public void OnGameStop()
        {
        }

        private void OnClosingDialog(EventKeyBase.OnClosingDialog obj)
        {
            if (obj.dialog == PopupManager.DialogName.Pause)
            {
                OnGameResume();
            }
        }

        private void OnOpenDialog(EventKeyBase.OpenDialog obj)
        {
            if (obj.dialog == PopupManager.DialogName.Pause)
            {
                OnGamePause();
            }
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

        private IEnumerator PlayNextLevel()
        {
            Holder.PlayTutorial?.Invoke();
            /// Wait Until tutorial is Completed

            yield return new WaitForEndOfFrame();

            CamMovingTo(player.Current.transform, 1, () =>
            {
                ui.PlayTime();
                player.Current.Play();
            });
        }

        private void OnPlayerIsPassedHalfStage(Base.Player player)
        {
            Debug.Log("OnPlayerIsPassedHalfStage.....");
            if(!level.Current.IsFinal)
            {
                SetupNextStage();
            }
        }

        private void SetupNextStage()
        {
            CreateNextLevel();
            CreateNextPlayer();
        }

        /// <summary>
        /// Create a Player follwing Level Postion
        /// </summary>
        private void CreateNextPlayer()
        {
            if (player.Current == null) // Init First Player
            {
                var pos = new Vector3(level.Current.BeginerPoint.position.x, playerPb.transform.position.y, playerPb.transform.position.z);

                player.Current = Instantiate(playerPb, transform);
                player.Current.transform.position = pos;
                player.Current.Setup(level.Current.Mode);
                level.Current.Assign(player.Current);
            }
            else
            {
                var pos = new Vector3(level.Next.BeginerPoint.position.x, playerPb.transform.position.y, playerPb.transform.position.z);

                player.Next = Instantiate(playerPb, transform);
                player.Next.transform.position = pos;
                player.Next.Setup(level.Next.Mode);
                level.Next.Assign(player.Next);
            }
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
                    levelIdx++;
                    return Player.Mode.Pathway2;
                case 3:
                    return Player.Mode.Pathway2;
            }
            return Player.Mode.Hurdling;
        }
        void CreateNextLevel()
        {
            if(level.Current == null) /// Init First Level
            {
                var nextMode = GetLevelMode(levelIdx);
                var nextLevelPb = levels[nextMode][Random.Range(0, levels[nextMode].Count)];

                var lastPos = Vector2.zero;

                var myLevel = Instantiate(nextLevelPb,
                    new Vector3(
                        lastPos.x + (nextLevelPb.transform.position.x - nextLevelPb.BeginerPoint.position.x),
                        nextLevelPb.transform.position.y,
                        nextLevelPb.transform.position.z),
                    nextLevelPb.transform.rotation,
                    transform);
                myLevel.Assign(levelIdx == totalMode);
                level.Current = myLevel;
                level.Next = myLevel;
            }
            else
            {
                var nextMode = GetLevelMode(levelIdx);
                var nextLevelPb = levels[nextMode][Random.Range(0, levels[nextMode].Count)];

                var lastPos = level.Current.FinisherPoint.position;

                var myLevel = Instantiate(nextLevelPb,
                    new Vector3(
                        lastPos.x + Mathf.Abs(nextLevelPb.BeginerPoint.position.x) + 7,
                        nextLevelPb.transform.position.y,
                        nextLevelPb.transform.position.z),
                    nextLevelPb.transform.rotation,
                    transform);
                myLevel.Assign(levelIdx == totalMode);
                level.Next = myLevel;
            }

            levelIdx++;
            if (levelIdx > totalMode) levelIdx = 0;
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
            CamFollowingPlayer(this.player.Current);
            if(this.player.Current.transform.position.x >= map.Next.position.x)
            {
                SetupMap();
            }
        }
        private void CamFollowingPlayer(Player player)
        {
            camera_.transform.position = new Vector3(player.transform.position.x + cameraRange.x, camera_.transform.position.y, camera_.transform.position.z);
        }
        internal void CamMovingTo(Transform trans, float time, System.Action OnComplete)
        {
            camTween?.Kill();
            camTween = camera_.transform.DOMoveX(trans.position.x + cameraRange.x, time).OnComplete(() =>
            {
                OnComplete?.Invoke();
            });
        }

        private void OnClaimExperience(Base.Player player)
        {
            playerScore++;
            ui.UpdateLoadingBar((float)playerScore / levelScore);
        }
    }
}
