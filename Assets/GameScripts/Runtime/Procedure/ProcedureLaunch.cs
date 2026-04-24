using GameFramework.Localization;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace GameMain
{
    /// <summary>
    /// 流程 => 启动器。
    /// </summary>
    public class ProcedureLaunch : ProcedureBase
    {
        private static readonly List<Language> SupportedLanguages = new()
        {
            Language.English,
            Language.ChineseSimplified,
            Language.ChineseTraditional
        };

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            // 语言配置：设置当前使用的语言，默认使用操作系统语言
            InitLanguageSettings();

            // 声音配置：根据用户配置数据，设置即将使用的声音选项
            InitSoundSettings();
            
            //热更新UI初始化
            UILoadMgr.Initialize();
            //热更新阶段文本初始化
            LoadText.Instance.InitConfigData(null);

            ChangeProcedure<ProcedureSplash>();
        }

        private static void InitLanguageSettings()
        {
            var isEditorMode = Application.platform == RuntimePlatform.WindowsEditor || 
                               Application.platform == RuntimePlatform.OSXEditor || 
                               Application.platform == RuntimePlatform.LinuxEditor;
            if (isEditorMode && GameModule.Base.EditorLanguage != Language.Unspecified)
            {
                // 编辑器资源模式直接使用 Inspector 上设置的语言
                return;
            }

            var language = GameModule.Localization.Language;
            if (GameModule.Setting.HasSetting(Constant.Setting.Language))
            {
                try
                {
                    var languageString = GameModule.Setting.GetString(Constant.Setting.Language);
                    language = (Language)Enum.Parse(typeof(Language), languageString);
                }
                catch(Exception exception)
                {
                    Log.Error("Init language error, reason {0}",exception.ToString());
                }
            }

            if (!SupportedLanguages.Contains(language))
            {
                // 若是暂不支持的语言，则使用英语
                language = Language.English;
                GameModule.Setting.SetString(Constant.Setting.Language, language.ToString());
                GameModule.Setting.Save();
            }

            GameModule.Localization.Language = language;
            Log.Info("Init language settings complete, current language is '{0}'.", language.ToString());
        }

        private static void InitSoundSettings()
        {
            GameModule.Sound.Mute("Music", GameModule.Setting.GetBool(Constant.Setting.MusicMuted, false));
            GameModule.Sound.SetVolume("Music", GameModule.Setting.GetFloat(Constant.Setting.MusicVolume, 0.5f));
            GameModule.Sound.Mute("Sound", GameModule.Setting.GetBool(Constant.Setting.SoundMuted, false));
            GameModule.Sound.SetVolume("Sound", GameModule.Setting.GetFloat(Constant.Setting.SoundVolume, 0.5f));
            GameModule.Sound.Mute("UISound", GameModule.Setting.GetBool(Constant.Setting.UISoundMuted, false));
            GameModule.Sound.SetVolume("UISound", GameModule.Setting.GetFloat(Constant.Setting.UISoundVolume, 0.5f));
            Log.Info("Init sound settings complete.");
        }
    }
}
