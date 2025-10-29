using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BoosterImplants;

namespace GTFO_ArtifactGenerator
{
    /// <summary>
    /// IMGUI-based Artifact Booster Selector UI
    /// Simple and IL2CPP compatible
    /// </summary>
    public class ArtifactSelectorUI : MonoBehaviour
    {
        // UI State
        private bool _visible = false;
        private Rect _windowRect = new Rect(100, 100, 800, 600);

        // Data mappings
        private Dictionary<string, uint> _effectMap = new Dictionary<string, uint>();
        private Dictionary<string, uint> _conditionMap = new Dictionary<string, uint>();
        private Dictionary<string, uint> _templateMap = new Dictionary<string, uint>();
        private bool _dataLoaded = false;
        private bool _isLoadingData = false;

        // Current selection
        private readonly List<(uint effectId, float value)> _selectedEffects = new List<(uint effectId, float value)>();
        private readonly List<uint> _selectedConditions = new List<uint>();
        private int _uses = 2;
        private uint? _pickedTemplate;

        // UI Control State
        private int _templateIndex = 0;
        private int _effectIndex = 0;
        private int _conditionIndex = 0;
        private string _valueInput = "0.20";
        private Vector2 _scrollPos = Vector2.zero;

        private string _statusMessage = "Press APPLY to grant booster.";

        private void Start()
        {
            // Don't load data immediately - wait until UI is first opened
            // This ensures GameData is fully initialized
        }

        private void LoadBoosterData()
        {
            // Prevent multiple simultaneous loads
            if (_dataLoaded || _isLoadingData) return;

            _isLoadingData = true;
            var logger = BepInEx.Logging.Logger.CreateLogSource("ArtifactGenerator");
            logger.LogInfo("Loading booster data using improved reflection...");

            try
            {
                _effectMap = FindAndBuildIdMap("BoosterImplantEffect", logger);
                logger.LogInfo($"Loaded {_effectMap.Count} effects");

                _conditionMap = FindAndBuildIdMap("BoosterImplantCondition", logger);
                logger.LogInfo($"Loaded {_conditionMap.Count} conditions");

                _templateMap = FindAndBuildIdMap("BoosterImplantTemplate", logger);
                logger.LogInfo($"Loaded {_templateMap.Count} templates");

                if (_effectMap.Count > 0 && _conditionMap.Count > 0 && _templateMap.Count > 0)
                {
                    _dataLoaded = true;
                    _statusMessage = "Data loaded! Select template and effects, then press APPLY.";
                    logger.LogInfo("Successfully loaded all booster data!");
                }
                else
                {
                    _statusMessage = "Data load incomplete. Click 'Retry Load' button below.";
                    logger.LogWarning($"Some types have no data: Effects={_effectMap.Count}, Conditions={_conditionMap.Count}, Templates={_templateMap.Count}");
                }
            }
            catch (Exception ex)
            {
                _statusMessage = $"Load failed: {ex.Message}";
                logger.LogError($"Failed to load booster data: {ex.Message}");
            }
            finally
            {
                _isLoadingData = false;
            }
        }

        private static Dictionary<string, uint> FindAndBuildIdMap(string typeName, BepInEx.Logging.ManualLogSource logger)
        {
            try
            {
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    try
                    {
                        var type = assembly.GetTypes().FirstOrDefault(t => t.Name == typeName);
                        if (type != null)
                        {
                            logger.LogInfo($"Found {typeName} in {assembly.GetName().Name}");
                            var map = BuildIdMap(type, logger);
                            return map;
                        }
                    }
                    catch (System.Reflection.ReflectionTypeLoadException) { }
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Error finding {typeName}: {ex.Message}");
            }

            return new Dictionary<string, uint>();
        }

        public void Toggle()
        {
            _visible = !_visible;
        }

        private void OnGUI()
        {
            if (!_visible) return;

            // Enable cursor and unlock when UI is visible
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            // Try to load data when UI first opens
            if (!_dataLoaded)
            {
                LoadBoosterData();
            }

            // Draw the window - IL2CPP compatible
            _windowRect = UnityEngine.GUI.Window(12345, _windowRect, (UnityEngine.GUI.WindowFunction)DrawWindow, "Artifact Booster Selector (Host Only) - Press ESC to close");
        }

        private void Update()
        {
            // Toggle with F6
            if (Input.GetKeyDown(KeyCode.F6))
            {
                Toggle();
            }

            // Close with ESC
            if (_visible && Input.GetKeyDown(KeyCode.Escape))
            {
                _visible = false;
            }
        }

        private void DrawWindow(int windowID)
        {
            GUILayout.BeginVertical();

            // Status message at top
            GUILayout.Label(_statusMessage, GUI.skin.box);
            GUILayout.Space(10);

            // Scrollable content area
            _scrollPos = GUILayout.BeginScrollView(_scrollPos, GUILayout.Height(450));

            // === TEMPLATE SECTION ===
            GUILayout.BeginHorizontal();
            GUILayout.Label("Template:", EditorStyles.boldLabel);
            if (!_dataLoaded && !_isLoadingData && GUILayout.Button("Refresh Data", GUILayout.Width(120)))
            {
                _dataLoaded = false; // Force reload
                _isLoadingData = false; // Reset loading flag
                LoadBoosterData();
            }
            GUILayout.EndHorizontal();

            if (_templateMap.Count > 0)
            {
                var templateNames = _templateMap.Keys.ToArray();
                // Guard against invalid index after data refresh
                if (_templateIndex >= templateNames.Length)
                {
                    _templateIndex = 0;
                    _pickedTemplate = null;
                }
                _templateIndex = GUILayout.SelectionGrid(_templateIndex, templateNames, 3);
                if (_templateIndex >= 0 && _templateIndex < templateNames.Length)
                {
                    _pickedTemplate = _templateMap[templateNames[_templateIndex]];
                }
            }
            else
            {
                GUILayout.Label("No templates found! Try entering a lobby/expedition first.");
                if (GUILayout.Button("Retry Load", GUILayout.Width(150)))
                {
                    _dataLoaded = false;
                    _isLoadingData = false; // Reset loading flag
                    LoadBoosterData();
                }
            }

            GUILayout.Space(15);

            // === EFFECT SECTION ===
            GUILayout.Label("Effects:", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();

            if (_effectMap.Count > 0)
            {
                var effectNames = _effectMap.Keys.ToArray();
                // Guard against invalid index after data refresh
                if (_effectIndex >= effectNames.Length)
                {
                    _effectIndex = 0;
                }

                GUILayout.BeginVertical(GUILayout.Width(300));
                GUILayout.Label("Select Effect:");
                _effectIndex = GUILayout.SelectionGrid(_effectIndex, effectNames, 1, GUILayout.Height(150));
                GUILayout.EndVertical();

                GUILayout.BeginVertical(GUILayout.Width(200));
                GUILayout.Label("Effect Value (0-5):");
                _valueInput = GUILayout.TextField(_valueInput, GUILayout.Width(100));
                if (GUILayout.Button("Add Effect", GUILayout.Width(150)))
                {
                    AddEffect();
                }
                GUILayout.EndVertical();
            }
            else
            {
                GUILayout.Label("No effects found! Check console or retry load.");
            }

            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            // Display selected effects
            if (_selectedEffects.Count > 0)
            {
                GUILayout.Label("Selected Effects:", EditorStyles.boldLabel);
                foreach (var effect in _selectedEffects)
                {
                    var name = _effectMap.FirstOrDefault(p => p.Value == effect.effectId).Key ?? effect.effectId.ToString();
                    GUILayout.Label($"  - {name} (ID: {effect.effectId}) = {effect.value:F2}");
                }
            }

            GUILayout.Space(15);

            // === CONDITION SECTION ===
            GUILayout.Label("Conditions (Optional):", EditorStyles.boldLabel);
            if (_conditionMap.Count > 0)
            {
                var conditionNames = _conditionMap.Keys.ToArray();
                // Guard against invalid index after data refresh
                if (_conditionIndex >= conditionNames.Length)
                {
                    _conditionIndex = 0;
                }

                GUILayout.BeginVertical(GUILayout.Width(400));
                _conditionIndex = GUILayout.SelectionGrid(_conditionIndex, conditionNames, 2, GUILayout.Height(100));
                GUILayout.EndVertical();

                if (GUILayout.Button("Add Condition", GUILayout.Width(150)))
                {
                    AddCondition();
                }
            }
            else
            {
                GUILayout.Label("No conditions found! Check console or retry load.");
            }

            GUILayout.Space(10);

            // Display selected conditions
            if (_selectedConditions.Count > 0)
            {
                GUILayout.Label("Selected Conditions:", EditorStyles.boldLabel);
                foreach (var condId in _selectedConditions)
                {
                    var name = _conditionMap.FirstOrDefault(p => p.Value == condId).Key ?? condId.ToString();
                    GUILayout.Label($"  - {name} (ID: {condId})");
                }
            }

            GUILayout.Space(15);

            // === USES SLIDER ===
            GUILayout.Label($"Uses: {_uses}", EditorStyles.boldLabel);
            _uses = (int)GUILayout.HorizontalSlider(_uses, 1, 3, GUILayout.Width(300));

            GUILayout.Space(15);

            GUILayout.EndScrollView();

            // === BOTTOM BUTTONS ===
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("APPLY", GUILayout.Height(40), GUILayout.Width(200)))
            {
                Apply();
            }

            GUILayout.Space(20);

            if (GUILayout.Button("Clear All", GUILayout.Height(40), GUILayout.Width(150)))
            {
                ClearAll();
            }

            GUILayout.Space(20);

            if (GUILayout.Button("CLOSE", GUILayout.Height(40), GUILayout.Width(150)))
            {
                _visible = false;
            }

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            // Make window draggable
            GUI.DragWindow(new Rect(0, 0, _windowRect.width, 20));
        }

        // === EVENT HANDLERS ===

        private void AddEffect()
        {
            if (_effectMap.Count == 0) return;

            var effectNames = _effectMap.Keys.ToArray();
            if (_effectIndex < 0 || _effectIndex >= effectNames.Length) return;

            var name = effectNames[_effectIndex];
            if (!_effectMap.TryGetValue(name, out var id)) return;

            if (!float.TryParse(_valueInput, out float val))
                val = 0.2f;

            val = Mathf.Clamp(val, 0f, 5f);

            _selectedEffects.Add((id, val));
            _statusMessage = $"Added Effect: {name} (ID: {id}) = {val:F2}";
        }

        private void AddCondition()
        {
            if (_conditionMap.Count == 0) return;

            var conditionNames = _conditionMap.Keys.ToArray();
            if (_conditionIndex < 0 || _conditionIndex >= conditionNames.Length) return;

            var name = conditionNames[_conditionIndex];
            if (!_conditionMap.TryGetValue(name, out var id)) return;

            if (!_selectedConditions.Contains(id))
            {
                _selectedConditions.Add(id);
                _statusMessage = $"Added Condition: {name} (ID: {id})";
            }
        }

        private void ClearAll()
        {
            _selectedEffects.Clear();
            _selectedConditions.Clear();
            _pickedTemplate = null;
            _uses = 2;
            _statusMessage = "Cleared all selections.";
        }

        private void Apply()
        {
            // Host-only check using reflection (SNet may be in different namespace)
            if (!IsHost())
            {
                _statusMessage = "ERROR: Host only!";
                return;
            }

            if (!_pickedTemplate.HasValue)
            {
                _statusMessage = "ERROR: Pick a Template first!";
                return;
            }

            if (!BoosterImplantManager.TryGetLocalPlayer(out var snetPlayer))
            {
                _statusMessage = "ERROR: No local SNet_Player found!";
                return;
            }

            try
            {
                // Build custom booster data
                var data = CustomBoosterFactory.BuildFromTemplate(
                    _pickedTemplate.Value,
                    _selectedEffects,
                    _selectedConditions,
                    _uses
                );

                // Sync to host player
                if (!BoosterImplantManager.SyncSetBoosterImplant(snetPlayer, data))
                {
                    _statusMessage = "ERROR: Sync failed!";
                    return;
                }

                var templateName = _templateMap.FirstOrDefault(p => p.Value == _pickedTemplate.Value).Key ?? "Unknown";
                _statusMessage = $"SUCCESS! Granted booster: {templateName} with {_selectedEffects.Count} effects";
            }
            catch (Exception ex)
            {
                _statusMessage = $"ERROR: {ex.Message}";
            }
        }

        // === HELPER METHODS ===

        /// <summary>
        /// Checks if the local player is the host using reflection
        /// </summary>
        private static bool IsHost()
        {
            try
            {
                // Find SNet type dynamically
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    var snetType = assembly.GetTypes().FirstOrDefault(t => t.Name == "SNet");
                    if (snetType != null)
                    {
                        var isMasterProp = snetType.GetProperty("IsMaster", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                        if (isMasterProp != null)
                        {
                            return (bool)isMasterProp.GetValue(null);
                        }
                    }
                }
            }
            catch { }

            // If we can't find SNet, assume not host for safety
            return false;
        }

        /// <summary>
        /// Builds a dictionary mapping constant field names to their uint values via reflection
        /// Searches both the type itself and any nested types
        /// </summary>
        private static Dictionary<string, uint> BuildIdMap(Type t, BepInEx.Logging.ManualLogSource logger)
        {
            var map = new Dictionary<string, uint>();
            var flags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.FlattenHierarchy;

            try
            {
                // First try the type itself
                var fields = t.GetFields(flags);
                logger.LogInfo($"Type {t.Name} has {fields.Length} static fields");

                foreach (var f in fields)
                {
                    if (TryAddField(f, map, logger))
                    {
                        // Field added successfully
                    }
                }

                // If no fields found, check nested types (common in GTFO)
                if (map.Count == 0)
                {
                    var nestedTypes = t.GetNestedTypes(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                    logger.LogInfo($"Type {t.Name} has {nestedTypes.Length} nested types");

                    foreach (var nested in nestedTypes)
                    {
                        logger.LogInfo($"  Checking nested type: {nested.Name}");
                        var nestedFields = nested.GetFields(flags);
                        logger.LogInfo($"    Found {nestedFields.Length} static fields in {nested.Name}");

                        foreach (var f in nestedFields)
                        {
                            if (TryAddField(f, map, logger))
                            {
                                // Field added successfully
                            }
                        }
                    }
                }

                logger.LogInfo($"Built map with {map.Count} entries for {t.Name}");
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to build map for {t.Name}: {ex.Message}");
            }

            return map;
        }

        private static bool TryAddField(System.Reflection.FieldInfo f, Dictionary<string, uint> map, BepInEx.Logging.ManualLogSource logger)
        {
            logger.LogDebug($"  Field: {f.Name}, Type: {f.FieldType.Name}");

            // Try multiple type checks for IL2CPP compatibility
            if (f.FieldType == typeof(uint) || f.FieldType.Name == "UInt32")
            {
                try
                {
                    var value = f.GetValue(null);
                    if (value != null)
                    {
                        uint uintValue = Convert.ToUInt32(value);
                        map[f.Name] = uintValue;
                        logger.LogDebug($"    Added: {f.Name} = {uintValue}");
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    logger.LogDebug($"    Failed to read {f.Name}: {ex.Message}");
                }
            }

            return false;
        }

        /// <summary>
        /// Custom EditorStyles for better UI appearance
        /// </summary>
        private static class EditorStyles
        {
            private static GUIStyle _boldLabel;
            public static GUIStyle boldLabel
            {
                get
                {
                    if (_boldLabel == null)
                    {
                        _boldLabel = new GUIStyle(GUI.skin.label);
                        _boldLabel.fontStyle = FontStyle.Bold;
                        _boldLabel.fontSize = 14;
                    }
                    return _boldLabel;
                }
            }
        }
    }
}
