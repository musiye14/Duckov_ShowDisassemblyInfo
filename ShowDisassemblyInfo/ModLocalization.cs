using System.Collections.Generic;
using UnityEngine;
using SodaCraft.Localizations;

namespace ShowDisassemblyInfo
{
    public static class ModLocalization
    {
        public const string OutputsPrefixKey = "MOD_SHOWDISASSEMBLYINFO_OUTPUTS_PREFIX";
        public const string SourcesPrefixKey = "MOD_SHOWDISASSEMBLYINFO_SOURCES_PREFIX";

        private static readonly Dictionary<SystemLanguage, Dictionary<string, string>> Translations = new Dictionary<SystemLanguage, Dictionary<string, string>>()
        {
            // --- 简体中文 ---
            { SystemLanguage.ChineseSimplified, new Dictionary<string, string>() {
                { OutputsPrefixKey, "拆解出: " },
                { SourcesPrefixKey, "可由此分解出: " } 
            }},
            // --- 繁体中文 ---
             { SystemLanguage.ChineseTraditional, new Dictionary<string, string>() {
                { OutputsPrefixKey, "拆解出: " },
                { SourcesPrefixKey, "可由此分解出: " } 
            }},
             // --- 日语 ---
             { SystemLanguage.Japanese, new Dictionary<string, string>() {
                { OutputsPrefixKey, "分解結果: " },
                { SourcesPrefixKey, "分解元: " }
            }},
             // --- 韩语 ---
             { SystemLanguage.Korean, new Dictionary<string, string>() {
                { OutputsPrefixKey, "분해 결과: " },
                { SourcesPrefixKey, "분해 가능: " }
            }},
            // --- 法语 ---
            { SystemLanguage.French, new Dictionary<string, string>() {
                { OutputsPrefixKey, "Se démonte en: " },
                { SourcesPrefixKey, "Peut être démonté de: " }
            }},
            // --- 德语 ---
            { SystemLanguage.German, new Dictionary<string, string>() {
                { OutputsPrefixKey, "Zerlegt in: " },
                { SourcesPrefixKey, "Kann zerlegt werden aus: " }
            }},
            // --- 俄语 ---
            { SystemLanguage.Russian, new Dictionary<string, string>() {
                { OutputsPrefixKey, "Разбирается на: " },
                { SourcesPrefixKey, "Можно разобрать из: " }
            }},
            // --- 西班牙语 ---
            { SystemLanguage.Spanish, new Dictionary<string, string>() {
                { OutputsPrefixKey, "Se desmonta en: " },
                { SourcesPrefixKey, "Se puede desmontar de: " }
            }},
            // --- 意大利语 ---
             { SystemLanguage.Italian, new Dictionary<string, string>() {
                { OutputsPrefixKey, "Si smonta in: " },
                { SourcesPrefixKey, "Smontabile da: " }
            }},
             // --- 英语 (默认) ---
             { SystemLanguage.English, new Dictionary<string, string>() {
                { OutputsPrefixKey, "Disassembles into: " },
                { SourcesPrefixKey, "Can be disassembled from: " }
            }}
        };
        
        public static string GetTranslation(string key)
        {
            #region GetTranslation 
            SystemLanguage currentLanguage = SystemLanguage.English; 
            try { currentLanguage = LocalizationManager.CurrentLanguage; }
            catch { Debug.LogError($"..."); currentLanguage = SystemLanguage.English; }

            if (Translations.TryGetValue(currentLanguage, out var langDict))
            {
                if (langDict.TryGetValue(key, out var translation)) return translation;
                Debug.LogWarning($"...");
            } else { Debug.LogWarning($"..."); }

            if (Translations.TryGetValue(SystemLanguage.English, out var englishDict) && englishDict.TryGetValue(key, out var englishTranslation))
            {
                return englishTranslation;
            }
            Debug.LogError($"...");
            return key; 
            #endregion
        }
    }
}