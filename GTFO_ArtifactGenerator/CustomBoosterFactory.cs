using System;
using System.Collections.Generic;
using UnityEngine;
using BoosterImplants;

namespace GTFO_ArtifactGenerator
{
    /// <summary>
    /// Factory class for building custom booster implant data from templates
    /// </summary>
    public static class CustomBoosterFactory
    {
        /// <summary>
        /// Creates a pBoosterEffectData structure with the given effect ID and value
        /// </summary>
        private static pBoosterEffectData MakeEffect(uint id, float value)
        {
            return new pBoosterEffectData
            {
                BoosterEffectID = id,
                EffectValue = value
            };
        }

        /// <summary>
        /// Builds a complete pBoosterImplantData from a template ID with custom effects, conditions, and uses
        /// </summary>
        /// <param name="templateId">The template ID from BoosterImplantTemplate constants</param>
        /// <param name="effects">List of (effectId, value) tuples to apply</param>
        /// <param name="conditions">List of condition IDs to apply</param>
        /// <param name="uses">Number of uses (1-3)</param>
        /// <returns>A fully configured pBoosterImplantData ready for sync</returns>
        public static pBoosterImplantData BuildFromTemplate(
            uint templateId,
            IReadOnlyList<(uint effectId, float value)> effects,
            IReadOnlyList<uint> conditions,
            int uses)
        {
            // Start with base template data
            var data = BoosterImplantManager.CreateImplantFromDataBlock(templateId);

            // Configure effects
            int effectCount = Math.Min(effects?.Count ?? 0, pBoosterImplantData.IMPLANT_EFFECT_SYNC_COUNT);
            if (data.BoosterEffectDatas == null || data.BoosterEffectDatas.Length < pBoosterImplantData.IMPLANT_EFFECT_SYNC_COUNT)
                data.BoosterEffectDatas = new pBoosterEffectData[pBoosterImplantData.IMPLANT_EFFECT_SYNC_COUNT];

            for (int i = 0; i < effectCount; i++)
            {
                var (id, val) = effects[i];
                data.BoosterEffectDatas[i] = MakeEffect(id, Mathf.Clamp(val, 0f, 5f));
            }

            // Clear remaining effect slots
            for (int i = effectCount; i < pBoosterImplantData.IMPLANT_EFFECT_SYNC_COUNT; i++)
                data.BoosterEffectDatas[i] = default;

            data.BoosterEffectCount = effectCount;

            // Configure conditions
            int condCount = Math.Min(conditions?.Count ?? 0, pBoosterImplantData.IMPLANT_CONDITION_SYNC_COUNT);
            if (data.Conditions == null || data.Conditions.Length < pBoosterImplantData.IMPLANT_CONDITION_SYNC_COUNT)
                data.Conditions = new uint[pBoosterImplantData.IMPLANT_CONDITION_SYNC_COUNT];

            for (int i = 0; i < condCount; i++)
                data.Conditions[i] = conditions[i];

            // Clear remaining condition slots
            for (int i = condCount; i < pBoosterImplantData.IMPLANT_CONDITION_SYNC_COUNT; i++)
                data.Conditions[i] = 0;

            data.ConditionCount = condCount;
            data.UseCount = Mathf.Clamp(uses, 1, 3);

            return data;
        }
    }
}
