using System;
using UnityEngine;
using ThunderRoad;

namespace BaronShopCustomizer
{
    public class BaronShopThunderScript : ThunderScript
    {
        public static BaronShopThunderScript Instance { get; private set; }
        public string ModFolder { get; private set; }

        private GameObject managerObject;
        private BaronShopController shopController;

        public override void ScriptLoaded(ModManager.ModData modData)
        {
            base.ScriptLoaded(modData);
            Instance = this;

            if (managerObject == null)
            {
                managerObject = new GameObject("BaronShopCustomizer_Manager");
                GameObject.DontDestroyOnLoad(managerObject);
                shopController = managerObject.AddComponent<BaronShopController>();
                shopController.Initialize();
            }
        }

        public override void ScriptEnable()
        {
            base.ScriptEnable();
            EventManager.onLevelLoad -= OnLevelLoaded;
            EventManager.onLevelLoad += OnLevelLoaded;
            if (shopController != null) { shopController.enabled = true; shopController.OnLevelReady(null); }
        }

        public override void ScriptDisable()
        {
            base.ScriptDisable();
            EventManager.onLevelLoad -= OnLevelLoaded;
            if (shopController != null) shopController.enabled = false;
        }

        private void OnLevelLoaded(LevelData levelData, LevelData.Mode mode, EventTime eventTime)
        {
            if (eventTime == EventTime.OnEnd && shopController != null)
            {
                shopController.OnLevelReady(levelData);
            }
        }
    }
}
