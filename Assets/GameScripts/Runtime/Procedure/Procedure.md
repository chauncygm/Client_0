graph TD
A[ProcedureLaunch<br/>启动器] --> B[ProcedureSplash<br/>闪屏动画]
B --> C[ProcedureInitPackage<br/>初始化资源包]
C --> D[ProcedureUpdateVersion<br/>更新静态版本]
D --> E[ProcedureUpdateManifest<br/>更新资源清单]
E --> F{是否为Web模式<br/>或边玩边下?}
F -->|是| G[ProcedurePreload<br/>预加载资源]
F -->|否| H[ProcedureCreateDownloader<br/>创建下载器]
H --> I{是否有需要下载的文件?}
I -->|否| J[ProcedureDownloadOver<br/>下载完成]
I -->|是| K[提示用户确认下载]
K --> L[ProcedureDownloadFile<br/>下载文件]
L --> J
J --> M{是否需要清理缓存?}
M -->|是| N[ProcedureClearCache<br/>清理缓存]
M -->|否| O[ProcedureLoadAssembly<br/>加载程序集]
N --> O
G --> O
O --> P[ProcedureStartGame<br/>启动游戏]


各流程核心功能说明

1. ProcedureLaunch - 启动器
核心功能: 初始化语言和声音配置
主要工作:
设置当前语言(支持中文简体/繁体/英文)
从配置读取或默认使用系统语言
初始化音乐、音效、UI音效的音量和静音状态
流转: → ProcedureSplash
2. ProcedureSplash - 闪屏动画
核心功能: 播放启动动画和初始化UI管理器
主要工作:
初始化UILoadMgr(UI加载管理器)
显示并播放Splash动画
初始化热更新文本配置
流转: → ProcedureInitPackage
3. ProcedureInitPackage - 初始化资源包
核心功能: 初始化YooAsset资源包
主要工作:
设置资源包名称为"GamePackage"
调用YooAssets初始化资源包
失败时提供重试机制
流转: → ProcedureUpdateVersion
4. ProcedureUpdateVersion - 更新静态版本
核心功能: 获取最新的资源包版本号
主要工作:
检查网络连接状态
异步请求远程资源包版本信息
更新本地PackageVersion
失败时提示用户重试或退出
流转: → ProcedureUpdateManifest
5. ProcedureUpdateManifest - 更新资源清单
核心功能: 下载并更新资源清单文件
主要工作:
根据PackageVersion获取对应的资源清单
判断运行模式(Web模式/边玩边下/普通模式)
Web模式或边玩边下 → ProcedurePreload
普通模式 → ProcedureCreateDownloader
流转: → ProcedurePreload 或 ProcedureCreateDownloader
6. ProcedurePreload - 预加载资源
核心功能: 预加载关键资源(仅Web模式或边玩边下模式)
主要工作:
根据配置的预加载标签获取资源列表
异步加载指定资源到内存
显示加载进度百分比
所有资源加载完成后进入下一阶段
流转: → ProcedureLoadAssembly
7. ProcedureCreateDownloader - 创建下载器
核心功能: 检测并准备资源更新下载
主要工作:
创建资源下载器
统计需要下载的文件数量和总大小
无更新文件 → 直接进入下载完成流程
有更新文件 → 弹窗提示用户确认下载
流转: → ProcedureDownloadFile 或 ProcedureDownloadOver
8. ProcedureDownloadFile - 下载文件
核心功能: 执行资源文件下载
主要工作:
注册下载进度和错误回调
开始批量下载更新文件
实时显示下载进度、速度、剩余时间
下载失败时提供重试选项
下载成功后进入完成流程
流转: → ProcedureDownloadOver
9. ProcedureDownloadOver - 下载完成
核心功能: 处理下载完成后的逻辑分支
主要工作:
标记下载完成状态
根据_needClearCache标志决定下一步:
true → 清理缓存
false → 直接加载程序集
流转: → ProcedureClearCache 或 ProcedureLoadAssembly
10. ProcedureClearCache - 清理缓存
核心功能: 清理未使用的缓存文件释放空间
主要工作:
调用YooAsset清理未使用的缓存文件
异步等待清理完成
清理完成后进入程序集加载
流转: → ProcedureLoadAssembly
11. ProcedureLoadAssembly - 加载程序集 
核心功能: 加载热更新DLL和HybridCLR元数据
主要工作:
加载AOT程序集的元数据(HybridCLR特性)
异步加载热更新DLL(TextAsset形式)
将DLL字节码转换为Assembly对象
收集所有热更新程序集
通过反射调用GameApp.Entrance入口方法
流转: → ProcedureStartGame
12. ProcedureStartGame - 启动游戏
核心功能: 完成启动流程,进入游戏主逻辑
主要工作:
隐藏所有加载界面
移交控制权给热更新代码(GameApp.Entrance)
正式启动游戏业务逻辑
流转: 流程结束,进入游戏主循环