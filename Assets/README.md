#### HybridCLRData目录
1. HotUpdateDlls/
   存放热更新程序集的编译输出（DLL 文件）
   对应配置项：hotUpdateDllCompileOutputRootDir
   这些是可以动态加载和替换的热更新代码
2. AssembliesPostIl2CppStrip/
   存放经过 IL2CPP 裁剪后的 AOT 程序集
   对应配置项：strippedAOTDllOutputRootDir
   用于生成泛型引用和桥接方法
3. hybridclr_repo/
   HybridCLR 框架的源代码仓库
   从 https://gitee.com/focus-creative-games/hybridclr 克隆
4. il2cpp_plus_repo/
   修改版 il2cpp 源码仓库
   从 https://gitee.com/focus-creative-games/il2cpp_plus 克隆
   提供对热更新的支持
5. LocalIl2CppData-WindowsEditor/
   本地 IL2CPP 数据（Windows 编辑器环境）
6. StrippedAOTDllsTempProj/
   临时项目目录，用于处理 AOT DLL 裁剪

#### YooAssets

* 构建输出目录
Bundles/StandaloneWindows64/GamePackage/
└── {版本号}/                    # 例如: 2026-04-07-1105
├── GamePackage.version      # 版本文件
├── GamePackage_{version}.bytes   # 清单文件（二进制）
├── GamePackage_{version}.hash    # 哈希校验文件
├── GamePackage_{version}.json    # 清单文件（JSON格式）
├── GamePackage_{version}.report  # 构建报告
├── buildlogtep.json         # 构建日志
└── link.xml                 # 链接配置文件

* StreamingAssets 运行时目录
Assets/StreamingAssets/yoo/
└── GamePackage/
├── BuildinCatalog.bytes     # 内置资源目录（二进制）
├── BuildinCatalog.json      # 内置资源目录（JSON）
├── GamePackage.version      # 版本标识
├── *.bundle.meta            # Bundle 元数据文件
└── GamePackage_{version}.bytes/hash  # 对应版本的清单