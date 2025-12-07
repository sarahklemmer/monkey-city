using UnityEngine;
using UnityEngine.Assertions;

namespace GameLogic.Buildings.Building_Controllers
{
    public class Library : BuildingBase
    {
        private bool pointBonusActive = false;
        private bool isBeingDestroyed = false;
        
        void Awake()
        {
            base.SharedAwakeBehavior();
            
            type = BuildingType.Library;
            canNeverBeUpgraded = true;
            canContainMonkeys = false;
        }

        void Start()
        {
            BuildingSoundManager.instance.PlayBuildingPlacedSound();
            ActivatePointBonus();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            
            if (!pointBonusActive && !isBeingDestroyed)
            {
                ActivatePointBonus();
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (isBeingDestroyed)
            {
                DeactivatePointBonus();
            }
        }

        private void ActivatePointBonus()
        {
            if (WaveSpawner.instance != null && !pointBonusActive)
            {
                WaveSpawner.instance.RegisterLibrary(this);
                pointBonusActive = true;
            }
            else if (WaveSpawner.instance == null)
            {
                Debug.LogError("Library: WaveSpawner.instance is NULL! Cannot register library.");
            }
            else if (pointBonusActive)
            {
                Debug.LogWarning("Library: Bonus already active, skipping registration");
            }
        }

        private void DeactivatePointBonus()
        {
            if (WaveSpawner.instance != null && pointBonusActive)
            {
                WaveSpawner.instance.UnregisterLibrary(this);
                pointBonusActive = false;
            }
        }

        void Update()
        {
            base.UpdateBehavior();
            bananasPerDay = 0;
        }

        public override string GetDescription()
        {
            string status = pointBonusActive ? "ACTIVE" : "INACTIVE";
            string libraryCount = WaveSpawner.instance != null ? 
                $"Registered Libraries: {WaveSpawner.instance.GetRegisteredLibraryCount()}" : 
                "WaveSpawner not found";
            
            return $"Library\n" +
                   $"Status: {status}\n" +
                   $"Effect: Doubles path points from waves\n" +
                   $"Special: Always active, limit 1 per game\n" +
                   $"{libraryCount}";
        }

        public override void OnDayCycle()
        {
        }

        public override void Die()
        {
            isBeingDestroyed = true;
            DeactivatePointBonus();
            
            if (BuildingUnlock.instance != null)
            {
                BuildingUnlock.instance.OnBuildingDestroyed(BuildingType.Library);
            }
            base.Die();
        }

        public override void OnDestroy()
        {
            isBeingDestroyed = true;
            if (Application.isPlaying)
            {
                DeactivatePointBonus();
                if (BuildingUnlock.instance != null)
                {
                    BuildingUnlock.instance.OnBuildingDestroyed(BuildingType.Library);
                }
            }
        }
    }
}