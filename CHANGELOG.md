# 更新日志

这里是 `Bingyan DevKit` 的更新日志！

## [1.5.0] - 2025-1-15

`1.5.0` 引入了大量新功能，移除了并不好用的部分功能，并进行了 1 mol 优化工作，是一次大型更新！

### 新增
- `EasyPositionEditAttribute` 允许你通过 `Scene` 窗口中的手柄来调整 `Vector2` 或 `Vector3` 的值！
- `AdvancedInspectorAttribute` 解锁更高级的 `Inspector` 绘制功能。开启后将可以使用：
  - `CategoryAttribute` 绘制一个可折叠的分类区，所有属于同一分类的属性将被绘制到一起
  - `SubCategoryAttribute` 绘制一个可折叠的二级分类区，所有属于同一分类的属性将被绘制到一起
- `LayerAttribute` 让 `int` 属性变成一个层级选择框
- `CSVParser` 可以简便快速地解析一个带表头的 CSV 文件
- `RandomUtils` 提供与随机数有关的快速工具
- `TransformExtensions` 提供对 `Transform` 的快速操作

### 更改
- `IntRange` 和 `FloatRange` 现在可以与 `int` 和 `float` 值进行乘法运算了
- `AnimatedValue` 重命名至 `FlexValue`
- 重构了 `Tween` 的控制逻辑，现在 `Tween` 的 `Stop` `Pause` 变得更可控了
- 删除了部分多余的 `using`

### 移除
- `Callback` 事实上，这种方式没有解决策划不会连事件的问题
- `AudioCallback` 没有考虑很多实际的用例，并不实用


## [1.4.2] - 2024-9-9
### 新增
- `ColorUtils`, `VectorExtensions` 等一系列工具，实现了一些基础功能
- `FloatRange` 等常用数据结构

### 更改
- 修正了 `Flow` 的命名空间
- 修改了 `Processer` 的访问修饰，使其可以被重写
- 修改了状态机【是否使用 Unity Update】的描述与访问修饰
- 优化 `Tween.Lerp` 的更新逻辑


## [1.4.1] - 2024-5-27
### 新增
- `Flow` 链式调用流，可以链式地指定并调用一系列（可能带有延时的）操作  
- `VectorExtensions` 现在增加对 `Vector2` 和 `Vector3` 的各个元素进行遍历的工具方法
- `GridAdapter` 现在支持 【拉伸元素】与【拉伸空隙】两种模式  

### 更改
- 优化了 `TitleAttribute` 在结构体等嵌套元素上的表现
- 优化了 `StrIDAttribute` 在设置界面的 ID 排序方式
- 补充了一部分文档注释
  

## [1.4.0] - 2024-4-9
### 新增
- 音频工具 `AudioUtils` ，可以实现分贝与绝对音量之间转换
- UI 网格布局辅助工具 `GridAdapter`, 可以规定行/列数，并让网格内元素尽量占据空间
- 向量工具 `VectorExtensions`, 可以更方便地修改向量的某一个分量  
  

## [1.3.5] - 2024-4-2
### 新增
- `Tween` 新增 `LimitDeltaTime()` 和 `Unscaled()` 选项
  - `LimitDeltaTime()` 可以在 `Time.deltaTime` 过大时进行限制，防止因为帧率原因，`Tween.Linear()` 过快完成
  - `Unscaled()` 允许 `Tween` 在 `Time.timeScale == 0` 的情况下继续运行

### 修复
- 修复了切换场景时，`Tween` 没有正确终止执行的 bug
- 修复了在 `IProcessable.Process(float)` 或 `IProcessable.PhysicsProcess(float)` 时使用 `Add()` 或 `Remove()` 导致执行顺序出错的问题  

  
## [1.3.4] - 2024-2-29
### 新增
- `DataStoreSet`: 使用 `HashSet` 实现的数据存储模型  
- `IEnumerableExtension`: 针对集合的拓展函数，包含 `ForEach` 和 `ForEachIndexed`  


## [1.3.3] - 2024-2-24
### 更改
- 现在 `Processer` 和 `RelayProcesser` 们都使用 `for` 进行循环，以避免在枚举期间调用 `Add()` `Remove()` 出错  


## [1.3.2] - 2024-2-22
### 新增
- 现在 `TitleAttribute` 对于结构体和 `MultilineAttribute` 增加了支持
  
### 更改
- `LinedPropertyDrawer` 现在使用 `EditorGUIUtility.standardVerticalSpacing` 作为默认纵向间距  

  
## [1.3.1] - 2024-2-16
### 更改
- `FSM` 现在不再是泛型模式了
- `FSM` 现在需要重写 `FSM.DefineStates()` 与 `FSM.GetDefaultState()` 来实现定义所有状态和初始状态  


## [1.3.0] - 2024-2-16
### 新增
- 添加了状态机 `FSM<T>` 和对应的状态 `FSMState`
  - `FSM<T>` 将会自动实例化 `T` 指定的状态对象


### 更改
- 现在 `Tween` 改用 `Stack` 作为对象池，而非 Unity 内置的 `ObjectPool<Tween>`
- 添加了 `Tween` 的内存安全检查，可以阻止用户代码访问已被回收的 `Tween`
- 添加了 `Tween.Verbose` ，用于打印额外的 Debug 信息

  
## [1.2.4] - 2024-2-10
### 新增
- `StrID` 现在支持一种新前缀 `$scene`
  - 使用该前缀的 ID 会自动以当前场景的名称作为前缀
  - 要筛选，只需要在其他 `StrID` 传入 `prefix:"你的场景名称"`
  - 非常适合【场景限定】ID 使用
  
### 修复
- 修复了 `CompositeConfigEditor` 内无法编辑配置的 bug  
  

## [1.2.2] - 2024-2-8
### 新增
- `RelayProcesser<T>` 中继处理器，用于实现多层处理结构
- `DefaultRelayProcesser` 泛用型中继处理器
  
### 更改
- `Processer<T>` 现在成为对 `RelayProcesser<T>` 的 Mono 封装


## [1.2.1] - 2024-1-30
### 修复
- 调优了 `CustomConfigEditor` 在组件被删除时的表现
  

## [1.2.0] -2024-1-29
### 新增
- `CompositeConfigEditor`: 将 `ScriptableObject` 拆分为组件化配置的编辑器拓展抽象
- `CompositeComponentAttribute`: 用于注册适用于 `CompositeConfigEditor` 的组件的 `Attribute`
  
### 更改
- `CustomConfigEditor` 更名为 `CompositeConfigEditor`
  
### 修复
- 修复了打包时未将 `DevKit.Editor` 集合排除引发的报错


## [1.1.6] - 2024-1-25
### 新增
- `CustomConfigEditor`: 用于创建组件化 `ScriptableObject` 型配置文件的编辑器支持(初步)
  

### 修复
- 修复了 `Tween` 在切换场景时报错的 bug

  
## [1.1.5] - 2024-1-24
### 新增
- `DraggableUI.ResetDragObj()` 方法，让拖拽物还原到原来的位置与父子关系
  

### 更改
- 将 `DraggableUI` 的 Unity 消息改为 `virtual`, 便于拓展
- 现在 `DraggableUI` 在拖拽时，会将拖拽物放置于 `Canvas` 的直接子物体处，防止遮罩等影响
  

### 修复
- 修复了 `Tween` 可能被多次回收入对象池的 bug


## [1.2.3] - 2024-2-8
### 新增
- `RelayProcesserMono<T>` 和 `DefaultRelayProcesserMono`: 可添加为组件的中继处理器
- `IProcesser<T>`: 处理器抽象接口
  

### 更改
- `Processer<T>` 更名为 `ProcesserMono<T>`
- `DefaultProcesser` 更名为 `DefaultProcesserMono`  


## [1.1.4] - 2024-1-9
### 更改
- 为说明文件 `README.md` `CHANGELOG.md` 等添加 `.meta` 文件以使引擎识别
  
### 修复
- 修复了 `Processable` 无法重写 `Process` 和 `PhysicsProcess` 方法的问题
  

## [1.1.3] - 2023-12-2
### 修复
- 修复了部分情况下，`Archive` 解析器匹配报错的bug

  
## [1.1.2] - 2023-12-1
### 新增
- 对 `Archive` 类的单元测试

### 更改
- 现在 `StringIDDrawer` 的绘制不再需要 `FieldInfo` 了

### 修复
- 修复了 `StrID` 的 `prefix` 参数无效的 bug
  


## [1.1.1] - 2023-12-1
### 修复
- 修复了无法正常读取存档中的 `float` 值的 bug

  
## [1.1.0] - 2023-12-1
### 新增
- 将原 `GameData` 框架的功能并入 `DevKit` 中
- 增加了对 `Vector2`, `Vector3`, `Quaternion` 数据类型进行 JSON 序列化、反序列化的支持
- 增加了对 JSON 序列化、反序列化过程进行自定义的接口

### 更改
- 将 `Data` 类重命名为 `Archive` 类
- 将 `LitJson` 库移动至 `Runtime` 文件夹

  
## [1.0.2] - 2023-11-28
### 更改
- 将创建 `StrID` 时的“确定”按钮移动到左侧，以符合操作习惯
- 修改了 `StrID` 无法找到原先设定的值时，反馈的警告信息

### 修复
- 修复了无法在 Project Settings 中删除字符串ID的问题


## [1.0.1] - 2023-11-28
### 更改
- 将部分仅内部使用的类型访问性改为 `internal`
- 将编辑器拓展相关代码移动至 `Bingyan.Editor` 命名空间下


## [1.0.0] - 2023-11-21
### 新增
- 将原 `.unitypackage` 包完全改为 Unity Package 
- 移植了所有原有特性
  - 方法回调选择器 - `Callback`
  - 音频触发器 - `AudioCallback`
  - 场景选择器 - `SceneName`
  - 字符串ID - `StrID`
  - 自定义属性标签 - `Title`
  - Tween - `Tween`
  - 可拖拽UI - `DraggableUI`
  - 自定义处理器 - `Processer` 