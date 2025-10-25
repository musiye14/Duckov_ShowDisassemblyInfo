using System;
using System.Collections.Generic;
using System.Reflection;
using Duckov.UI;
using Duckov.Utilities;
using ItemStatsSystem;
using TMPro;
using UnityEngine;
using System.Text;
using System.Linq;
using Duckov.Economy; 

namespace ShowDisassemblyInfo
{
    public class ModBehaviour : Duckov.Modding.ModBehaviour
    {
        private TextMeshProUGUI _outputsTextInstance = null;
        private TextMeshProUGUI _sourcesTextInstance = null;

        private Dictionary<int, List<int>> _sourceLookupCache = null;
        private const string LogPrefix = "[ShowDisassemblyInfo] ";

        private void Awake()
        {

        }

        void OnDestroy()
        {
            Debug.Log(LogPrefix + "OnDestroy called. Destroying UI elements...");
            if (_outputsTextInstance != null && _outputsTextInstance.gameObject != null) Destroy(_outputsTextInstance.gameObject);
            if (_sourcesTextInstance != null && _sourcesTextInstance.gameObject != null) Destroy(_sourcesTextInstance.gameObject);
        }

        void OnEnable()
        {
            Debug.Log(LogPrefix + "OnEnable called. Subscribing to events...");
            ItemHoveringUI.onSetupItem += OnSetupItemHoveringUI;
            ItemHoveringUI.onSetupMeta += OnSetupMetaHoveringUI;
        }

        void OnDisable()
        {
            Debug.Log(LogPrefix + "OnDisable called. Unsubscribing from events...");
            ItemHoveringUI.onSetupItem -= OnSetupItemHoveringUI;
            ItemHoveringUI.onSetupMeta -= OnSetupMetaHoveringUI;
            // 确保 Mod 禁用时 UI 也隐藏
            HideLabels();
        }

        private TextMeshProUGUI OutputsText
        {
            get
            {
                // 如果实例还不存在...
                if (_outputsTextInstance == null)
                {
                    Debug.Log(LogPrefix + "Lazy initializing OutputsText...");
                    try
                    {
                        // ...就在这里创建它
                        _outputsTextInstance = Instantiate(GameplayDataSettings.UIStyle.TemplateTextUGUI);
                        if (_outputsTextInstance != null)
                        {
                             _outputsTextInstance.fontSize = 18f;
                             _outputsTextInstance.color = Color.cyan;
                             _outputsTextInstance.gameObject.SetActive(false); // 初始隐藏
                             // 【重要】设置 DontDestroyOnLoad 防止场景切换时丢失 (可选但推荐)
                             DontDestroyOnLoad(_outputsTextInstance.gameObject); 
                        }
                        else
                        {
                             Debug.LogError(LogPrefix + "Instantiate failed for OutputsText template!");
                        }
                    } catch (Exception ex) { Debug.LogError(LogPrefix + $"Error lazy initializing OutputsText: {ex.Message}"); }
                }
                return _outputsTextInstance;
            }
        }
        
        private TextMeshProUGUI SourcesText
        {
            get
            {
                 // 如果实例还不存在...
                if (_sourcesTextInstance == null)
                {
                    Debug.Log(LogPrefix + "Lazy initializing SourcesText...");
                     try
                    {
                        // ...就在这里创建它
                        _sourcesTextInstance = Instantiate(GameplayDataSettings.UIStyle.TemplateTextUGUI);
                         if (_sourcesTextInstance != null)
                        {
                            _sourcesTextInstance.fontSize = 18f;
                            _sourcesTextInstance.color = Color.yellow;
                            _sourcesTextInstance.gameObject.SetActive(false); // 初始隐藏
                            // 【重要】设置 DontDestroyOnLoad
                            DontDestroyOnLoad(_sourcesTextInstance.gameObject); 
                        }
                        else
                        {
                             Debug.LogError(LogPrefix + "Instantiate failed for SourcesText template!");
                        }
                    } catch (Exception ex) { Debug.LogError(LogPrefix + $"Error lazy initializing SourcesText: {ex.Message}"); }
                }
                return _sourcesTextInstance;
            }
        }
        
        private void OnSetupItemHoveringUI(ItemHoveringUI uiInstance, Item item)
        {
            // Debug.Log(LogPrefix + $"OnSetupItemHoveringUI triggered. Item: {item?.DisplayName ?? "null"} (ID: {item?.TypeID ?? -1})");
            if (item == null)
            {
                HideLabels();
                return;
            }
            UpdateLabels(uiInstance, item.TypeID);
        }
        
        private void OnSetupMetaHoveringUI(ItemHoveringUI uiInstance, ItemMetaData meta)
        {
            // Debug.Log(LogPrefix + $"OnSetupMetaHoveringUI triggered. MetaData: {meta.DisplayName} (ID: {meta.id})");
            if (meta.id < 0)
            {
                HideLabels();
                return;
            }
            UpdateLabels(uiInstance, meta.id);
        }

        private void UpdateLabels(ItemHoveringUI uiInstance, int itemID)
        {
            // Debug.Log(LogPrefix + $"UpdateLabels called for itemID: {itemID}"); // 这条日志可以暂时注释掉
            
            var currentOutputsText = OutputsText; 
            var currentSourcesText = SourcesText;
            
            if (DecomposeDatabase.Instance == null || uiInstance == null || uiInstance.LayoutParent == null || currentOutputsText == null || currentSourcesText == null)
            {
                // Debug.LogError(LogPrefix + "UpdateLabels prerequisites failed!"); 
                HideLabels();
                return;
            }
            
            bool outputFound = false;
            bool sourceFound = false;

            // --- 1. 获取可拆解 Outputs ---
            try 
            {
                DecomposeFormula formula = DecomposeDatabase.Instance.GetFormula(itemID);
                if (formula.valid)
                {
                    var outputs = GetOutputsFromCost(formula.result);
                    if (outputs.Any())
                    {
                        StringBuilder sb = new StringBuilder(ModLocalization.GetTranslation(ModLocalization.OutputsPrefixKey));
                        #region Build Output String (不变)
                        bool metaDataValid = true;
                        for (int i = 0; i < outputs.Count; i++)
                        {
                            var output = outputs[i];
                            ItemMetaData meta = ItemAssetsCollection.GetMetaData(output.itemId);
                            if (string.IsNullOrEmpty(meta.Name)) { metaDataValid = false; break; }
                            sb.Append($"{meta.DisplayName} x{output.count}");
                            
                            bool isLastItemInLine = (i + 1) % 10 == 0; 
                            bool isLastItemOverall = i == outputs.Count - 1; 
                            if (!isLastItemOverall) 
                            {
                                if (isLastItemInLine)
                                {
                                    sb.Append("\n"); 
                                }
                                else
                                {
                                    sb.Append(", "); 
                                }
                            }
                        }
                        #endregion

                        if (metaDataValid) 
                        {
                            currentOutputsText.text = sb.ToString();
                            currentOutputsText.gameObject.SetActive(true);
                            currentOutputsText.transform.SetParent(uiInstance.LayoutParent, false);
                            currentOutputsText.transform.SetAsLastSibling();
                            outputFound = true;
                        }
                    }
                }
            } catch (Exception ex) { Debug.LogError(LogPrefix + $"Error getting Outputs: {ex.Message}"); outputFound = false; }


            // --- 2. 获取可由此拆解 Sources ---
            try 
            {
                BuildSourceLookupCacheIfNeeded();
                if (_sourceLookupCache != null && _sourceLookupCache.TryGetValue(itemID, out List<int> sourceItemIDs))
                {
                    if (sourceItemIDs.Any())
                    {
                        StringBuilder sb = new StringBuilder(ModLocalization.GetTranslation(ModLocalization.SourcesPrefixKey));
                         #region Build Source String (不变)
                        bool metaDataValid = true;
                        
                        int displayCount = Math.Min(sourceItemIDs.Count, 20);
                        
                        for (int i = 0; i < displayCount; i++)
                        {
                            int sourceId = sourceItemIDs[i];
                            ItemMetaData meta = ItemAssetsCollection.GetMetaData(sourceId);
                             if (string.IsNullOrEmpty(meta.Name)) { metaDataValid = false; break; }
                            sb.Append(meta.DisplayName);
                            bool isLastItemInLine = (i + 1) % 10 == 0;
                            bool isLastItemOverall = i == displayCount - 1;
                            
                            if (!isLastItemOverall)
                            {
                                if (isLastItemInLine)
                                {
                                    sb.Append("\n");
                                }
                                else
                                {
                                    sb.Append(", ");
                                }
                            }
                        }
                         #endregion
                        
                        if (metaDataValid)
                        {
                            
                            if (sourceItemIDs.Count > 20)
                            {
                                sb.Append("..."); 
                            }
                            
                            currentSourcesText.text = sb.ToString();
                            currentSourcesText.gameObject.SetActive(true);
                            currentSourcesText.transform.SetParent(uiInstance.LayoutParent, false);
                            currentSourcesText.transform.SetAsLastSibling();
                            sourceFound = true;
                        }
                    }
                }
            } catch (Exception ex) { Debug.LogError(LogPrefix + $"Error getting Sources: {ex.Message}"); sourceFound = false; }
            
            if (!outputFound)
            {
                if (currentOutputsText != null && currentOutputsText.gameObject != null) currentOutputsText.gameObject.SetActive(false);
            }
            if (!sourceFound)
            {
                if (currentSourcesText != null && currentSourcesText.gameObject != null) currentSourcesText.gameObject.SetActive(false);
            }
        }

        private void HideLabels()
        {
             var currentOutputsText = OutputsText; 
             var currentSourcesText = SourcesText;
             if (currentOutputsText != null && currentOutputsText.gameObject != null) currentOutputsText.gameObject.SetActive(false);
             if (currentSourcesText != null && currentSourcesText.gameObject != null) currentSourcesText.gameObject.SetActive(false);
        }

        private List<(int itemId, int count)> GetOutputsFromCost(Cost cost)
        {
            #region GetOutputsFromCost (保持不变)
            var results = new List<(int itemId, int count)>();
            if (cost.items == null) return results;
            foreach (var entry in cost.items)
            {
                results.Add((entry.id, (int)entry.amount));
            }
            return results;
            #endregion
        }

       private void BuildSourceLookupCacheIfNeeded()
        {
             if (_sourceLookupCache != null) return; 

             // Debug.Log(LogPrefix + "BuildSourceLookupCacheIfNeeded called. Building cache...");
             _sourceLookupCache = new Dictionary<int, List<int>>();
             var database = DecomposeDatabase.Instance;
             if (database == null) { Debug.LogError(LogPrefix + "DecomposeDatabase.Instance is null! Cannot build cache."); return; }
             
             IEnumerable<DecomposeFormula> formulas = null;
             var dicField = typeof(DecomposeDatabase).GetField("_dic", BindingFlags.NonPublic | BindingFlags.Instance);
             if (dicField != null)
             {
                 var dic = (Dictionary<int, DecomposeFormula>)dicField.GetValue(database);
                 if (dic != null && dic.Count > 0)
                 {
                      formulas = dic.Values;
                      Debug.Log(LogPrefix + $"Using dictionary ({dic.Count} entries) for cache.");
                 }
             }

             if (formulas == null)
             {
                 var entriesField = typeof(DecomposeDatabase).GetField("entries", BindingFlags.NonPublic | BindingFlags.Instance);
                 if (entriesField != null)
                 {
                     var entries = (DecomposeFormula[])entriesField.GetValue(database);
                     if (entries != null)
                     {
                          formulas = entries;
                          Debug.Log(LogPrefix + $"Using entries array ({entries.Length} entries) for cache.");
                     }
                 }
             }

             if (formulas == null) { Debug.LogError(LogPrefix + "Failed to access DecomposeDatabase formulas via reflection!"); return; }
             
             ProcessFormulas(formulas);
        }

        private void ProcessFormulas(IEnumerable<DecomposeFormula> formulas)
        {
             int processedCount = 0;
             int sourceEntriesAdded = 0;
             foreach (var formula in formulas)
             {
                 if (!formula.valid) continue;
                 processedCount++;

                 var outputs = GetOutputsFromCost(formula.result);
                 foreach (var output in outputs)
                 {
                     if (output.itemId <= 0) continue; 
                     if (!_sourceLookupCache.ContainsKey(output.itemId))
                     {
                         _sourceLookupCache[output.itemId] = new List<int>();
                     }
                     if (!_sourceLookupCache[output.itemId].Contains(formula.item))
                     {
                         _sourceLookupCache[output.itemId].Add(formula.item);
                         sourceEntriesAdded++; 
                     }
                 }
             }
             // Debug.Log(LogPrefix + $"Cache built: Processed {processedCount} formulas. Found sources for {_sourceLookupCache.Count} items. Total source entries added: {sourceEntriesAdded}.");
        }
    }
}