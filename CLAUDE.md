# Client_0 - Unity Game Client

Unity 2022.3.62f2c1 手机游戏客户端，基于 **UnityGameFramework** + **HybridCLR** 热更新框架，面向多平台（Android/iOS/Standalone/WebGL/主机）。

## Architecture: Dual-Layer (AOT + HotFix)

```
┌─────────────────────────────────────────────┐
│  AOT (主包) — Assets/Scripts/               │
│  Launcher.cs, Client_0.cs, FileUtils...      │
├─────────────────────────────────────────────┤
│  Runtime (AOT) — Assets/GameScripts/Runtime/ │
│  Procedure 启动流程, UILoadMgr, 资源初始化     │
├─────────────────────────────────────────────┤
│  HotFix (热更) — Assets/GameScripts/HotFix/  │
│  ├── GameBase/    核心框架 (单例/逻辑系统/UI扩展) │
│  ├── GameConfig/   配置表 (Luban → JSON)       │
│  ├── GameLogic/   业务逻辑 (UI/网络/流程/事件)    │
│  └── GameProto/   Protobuf 消息定义             │
└─────────────────────────────────────────────┘
```

**启动流程** (见 `Assets/GameScripts/Runtime/Procedure/Procedure.md`):
`ProcedureLaunch` → `ProcedureSplash` → `ProcedureInitPackage` (YooAsset) → `ProcedureUpdateVersion` → `ProcedureUpdateManifest` → 资源下载/更新 → `ProcedureLoadAssembly` (加载 HotFix DLL) → `ProcedureStartGame` → `GameApp.Entrance()` → `ProcedureLogin`

## Key Tech Stack

| 组件 | 用途 |
|------|------|
| **HybridCLR** (gitee) | C# 热更新，4 个 HotFix 程序集 (GameBase/GameConfig/GameLogic/GameProto) |
| **YooAsset** (2.3.18) | AssetBundle 资源管理、热更下载 |
| **XLua** | Lua 脚本层（Launcher 中初始化，主要用于启动阶段） |
| **UniTask** (2.5.10) | async/await |
| **Google.Protobuf** | 网络协议序列化 |
| **Newtonsoft.Json** (3.2.2) | 配置表 JSON 反序列化 |
| **Unity Input System** (1.14.2) | 输入映射 |
| **DOTween** | UI/动画补间 |
| **Cinemachine** (3.1.4) | 高级相机 |
| **TextMeshPro** (3.0.9) | 文本渲染 |

## Directory Map

```
Assets/
├── Scripts/               AOT 入口脚本 (Client_0.cs, Launcher.cs)
├── GameScripts/
│   ├── Runtime/           AOT 运行时 (Procedure 启动流程链, UILoadMgr)
│   ├── HotFix/
│   │   ├── GameBase/      框架层: Singleton, BaseLogicSys, CodeTypes, UI 扩展
│   │   ├── GameConfig/    配置层: ConfigManager, ConfigBase<T>, CfgBean
│   │   ├── GameLogic/     业务层: GameApp, UISystem, Procedure, Net, Event, Pool
│   │   └── GameProto/     Protobuf 消息
│   └── Editor/            编辑器工具 (Luban导出, YooAsset打包, 资源后处理)
├── Scenes/                Main.unity (主场景) + Sample 测试场景
├── Resources/             Unity Resources
├── Res/                   游戏资源
├── StreamingAssets/       YooAsset 内置资源目录
├── Plugins/               原生插件 (Android/iOS/OpenHarmony/xlua/ThirdParty)
└── AssetRaw/DLL/          各模块编译产物 (.bytes)
HybridCLRData/             HybridCLR 构建输出 (gitignore, 本地生成)
Bundles/                   YooAsset 打包输出
Builds/                    构建产物
```

## Core Patterns

### Singleton
- `Singleton<T>` — 纯 C# 单例 (`new T()`)
- `TSingleton<T> : ISingleton` — 带生命周期 (`Init/Active/Release`)，由 `SingletonMgr` 管理
- `UnitySingleton<T> : MonoBehaviour` — Unity 单例，自动创建 GameObject

### Logic System (模块化)
- `ILogicSys` 定义生命周期: `OnInit/OnStart/OnUpdate/OnLateUpdate/OnFixedUpdate/OnDestroy/OnDrawGizmos/OnApplicationPause`
- `BaseLogicSys<T>` 抽象基类，所有方法 virtual no-op
- `GameApp` 通过 `m_ListLogicMgr` 驱动所有 LogicSys 的 Unity 生命周期
- 在 `GameApp_RegisterSystem.cs` 的 `InitSystem()` 中注册新系统

### UI System
- `UIWindow` — 全屏/弹窗窗口，`[Window(UILayer, path, fullscreen)]` 特性标记
- `UIWidget` — 可复用 UI 组件
- `UIController` — 反射自动注册的 UI 控制器
- 五个层级: `Bottom(0)`, `UI(1)`, `Top(2)`, `Tips(3)`, `System(4)`
- `UISystem` (继承 `BaseLogicSys<UISystem>`) 管理窗口栈、深度排序、安全区适配

### Network
- `[MessageHandler]` 特性标记静态消息处理方法
- `MessageDispatcher` 根据 proto ID 分发到对应 Handler
- `ClientNetWorkChannelHelper` + TCP 通道 (`ProcedureLogin` 中建立连接)

### Config
- `ConfigManager : Singleton<ConfigManager>` 全局配置管理器
- `ConfigBase<T>` 基类，`[JsonConstructor]` + `[JsonProperty]` 不可变反序列化
- 配置表名定义在 `Bean/CfgXxx.TableName`

### Event
- `GameEvent.EventMgr.GetInterface<IXxx>().OnXxx()` — 接口式事件
- `AddUIEvent<T>(eventId, handler)` — UI 事件注册
- `Event/Gen/` — 代码生成的事件接口和实现

## Coding Conventions

- **注释语言**: XML doc 和行内注释使用**中文**
- **命名**: 命名空间 PascalCase，类/接口 PascalCase（接口 `I` 前缀），公开成员 PascalCase，私有字段 `_camelCase`，局部变量/参数 `camelCase`（部分 UI 代码遗留 `m_` 匈牙利前缀）
- **async**: 使用 `async UniTaskVoid`（即发即弃）或 `async UniTask<T>`（带 CancellationToken）
- **using 排序**: System → 第三方包 (Newtonsoft, Protobuf, Cysharp) → Unity/GameFramework → 项目命名空间
- **partial class**: `GameApp` 使用 partial 拆分注册逻辑

## Build & Workflow

- **HybridCLR 设置**: `ProjectSettings/HybridCLRSettings.asset`
- **资源打包**: 通过 YooAsset 编辑器工具 → `Bundles/`
- **配置导出**: `GameScripts/Editor/EditorTools/LubanTools.cs`
- **Git**: `dev` 分支开发，`master` 主线；`HybridCLRData/`、`Bundles/`、`StreamingAssets/` 不纳入版本控制
- **热更 DLL 编译**: 修改 HotFix 代码后需通过 HybridCLR 流程重新生成 `AssetRaw/DLL/*.bytes`
