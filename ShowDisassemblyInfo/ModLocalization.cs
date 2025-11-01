using System.Collections.Generic;
using UnityEngine;
using SodaCraft.Localizations;

namespace ShowDisassemblyInfo
{
    public static class ModLocalization
    {
        public const string OutputsPrefixKey = "MOD_SHOWDISASSEMBLYINFO_OUTPUTS_PREFIX";
        public const string SourcesPrefixKey = "MOD_SHOWDISASSEMBLYINFO_SOURCES_PREFIX";
        public const string ConfigShowOutputsKey = "MOD_SHOWDISASSEMBLYINFO_CONFIG_SHOW_OUTPUTS";
        public const string ConfigShowSourcesKey = "MOD_SHOWDISASSEMBLYINFO_CONFIG_SHOW_SOURCES";

        private static readonly Dictionary<SystemLanguage, Dictionary<string, string>> Translations = new Dictionary<SystemLanguage, Dictionary<string, string>>()
        {
            // --- 简体中文 ---
            { SystemLanguage.ChineseSimplified, new Dictionary<string, string>() {
                { OutputsPrefixKey, "拆解出: " },
                { SourcesPrefixKey, "可由以下物品分解出: " }, // 或 "可由以下物品分解获得: "
                { ConfigShowOutputsKey, "显示拆解产物" },
                { ConfigShowSourcesKey, "显示拆解来源" }
            }},
            // --- 繁体中文 ---
            { SystemLanguage.ChineseTraditional, new Dictionary<string, string>() {
                { OutputsPrefixKey, "拆解出: " },
                { SourcesPrefixKey, "可由以下物品分解出: " }, // 或 "可由以下物品分解獲得: "
                { ConfigShowOutputsKey, "顯示拆解產物" },
                { ConfigShowSourcesKey, "顯示拆解來源" }
            }},
            // --- 日语 ---
            { SystemLanguage.Japanese, new Dictionary<string, string>() {
                { OutputsPrefixKey, "分解結果: " },
                { SourcesPrefixKey, "以下のアイテムを分解して入手:" },
                { ConfigShowOutputsKey, "分解結果を表示" },
                { ConfigShowSourcesKey, "分解元を表示" }
            }},
            // --- 韩语 ---
            { SystemLanguage.Korean, new Dictionary<string, string>() {
                { OutputsPrefixKey, "분해 결과: " },
                { SourcesPrefixKey, "다음 아이템을 분해하여 획득:" },
                { ConfigShowOutputsKey, "분해 결과 표시" },
                { ConfigShowSourcesKey, "분해 출처 표시" }
            }},
            // --- 法语 ---
            { SystemLanguage.French, new Dictionary<string, string>() {
                { OutputsPrefixKey, "Se démonte en: " },
                { SourcesPrefixKey, "S'obtient en démontant : " },
                { ConfigShowOutputsKey, "Afficher les résultats du démontage" },
                { ConfigShowSourcesKey, "Afficher les sources de démontage" }
            }},
            // --- 德语 ---
            { SystemLanguage.German, new Dictionary<string, string>() {
                { OutputsPrefixKey, "Zerlegt in: " },
                { SourcesPrefixKey, "Erhältlich durch Zerlegen von:" },
                { ConfigShowOutputsKey, "Zerlegungsergebnisse anzeigen" },
                { ConfigShowSourcesKey, "Zerlegungsquellen anzeigen" }
            }},
            // --- 俄语 ---
            { SystemLanguage.Russian, new Dictionary<string, string>() {
                { OutputsPrefixKey, "Разбирается на: " },
                { SourcesPrefixKey, "Можно получить при разборке:" },
                { ConfigShowOutputsKey, "Показать результаты разборки" },
                { ConfigShowSourcesKey, "Показать источники разборки" }
            }},
            // --- 西班牙语 ---
            { SystemLanguage.Spanish, new Dictionary<string, string>() {
                { OutputsPrefixKey, "Se desmonta en: " },
                { SourcesPrefixKey, "Se obtiene al desmontar:" },
                { ConfigShowOutputsKey, "Mostrar resultados del desmontaje" },
                { ConfigShowSourcesKey, "Mostrar fuentes de desmontaje" }
            }},
            // --- 意大利语 ---
            { SystemLanguage.Italian, new Dictionary<string, string>() {
                { OutputsPrefixKey, "Si smonta in: " },
                { SourcesPrefixKey, "Si ottiene smontando:" },
                { ConfigShowOutputsKey, "Mostra risultati smontaggio" },
                { ConfigShowSourcesKey, "Mostra fonti smontaggio" }
            }},
            // --- 英语 (默认) ---
            { SystemLanguage.English, new Dictionary<string, string>() {
                { OutputsPrefixKey, "Disassembles into: " },
                { SourcesPrefixKey, "Can be disassembled from: " },
                { ConfigShowOutputsKey, "Show Disassembly Outputs" },
                { ConfigShowSourcesKey, "Show Disassembly Sources" }
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