using ColossalFramework;
using ICities;
using System;
using System.Collections.Generic;

namespace CSL_NoScaffoldingAnimation
{
    public class LoadingExtension : LoadingExtensionBase
    {
        private Dictionary<BuildingInfo, int> cachedConstructionTimes = new Dictionary<BuildingInfo, int>();

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);
            Singleton<SimulationManager>.instance.AddAction(delegate
            {
                for (uint num = 0u; num < PrefabCollection<BuildingInfo>.LoadedCount(); num++)
                {
                    BuildingInfo loaded = PrefabCollection<BuildingInfo>.GetLoaded(num);
                    if (!(loaded == null) && loaded.m_buildingAI is PrivateBuildingAI)
                    {
                        if (!cachedConstructionTimes.TryGetValue(loaded, out int _))
                        {
                            cachedConstructionTimes.Add(loaded, ((PrivateBuildingAI)loaded.m_buildingAI).m_constructionTime);
                        }
                        ((PrivateBuildingAI)loaded.m_buildingAI).m_constructionTime = 0;
                    }
                }
            });
        }

        public override void OnLevelUnloading()
        {
            base.OnLevelUnloading();
            Singleton<SimulationManager>.instance.AddAction(delegate
            {
                foreach (KeyValuePair<BuildingInfo, int> cachedConstructionTime in cachedConstructionTimes)
                {
                    if (!(cachedConstructionTime.Key == null))
                    {
                        try
                        {
                            ((PrivateBuildingAI)cachedConstructionTime.Key.m_buildingAI).m_constructionTime = cachedConstructionTime.Value;
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            });
        }
    }
}
