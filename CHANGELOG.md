# 更新日志 / ChangeLog

这里是 Bingyan DevKit 包的更新日志！

## [1.0.0] - 2023-11-21
### 新增
* 将原 `.unitypackage` 包完全改为 Unity Package 
* 移植了所有原有特性
  * 方法回调选择器 - `Callback`
  * 音频触发器 - `AudioCallback`
  * 场景选择器 - `SceneName`
  * 字符串ID - `StrID`
  * 自定义属性标签 - `Title`
  * Tween - `Tween`
  * 可拖拽UI - `DraggableUI`
  * 自定义处理器 - `Processer` 
  


## [1.0.1] - 2023-11-28
### 更改
* 将部分仅内部使用的类型访问性改为 `internal`
* 将编辑器拓展相关代码移动至 `Bingyan.Editor` 命名空间下
  


## [1.0.2] - 2023-11-28
### 更改
* 将创建 `StrID` 时的“确定”按钮移动到左侧，以符合操作习惯
* 修改了 `StrID` 无法找到原先设定的值时，反馈的警告信息

### 修复
* 修复了无法在 Project Settings 中删除字符串ID的问题
  


## [1.1.0] - 2023-12-1
### 新增
* 将原 `GameData` 框架的功能并入 `DevKit` 中
* 增加了对 `Vector2`, `Vector3`, `Quaternion` 数据类型进行 JSON 序列化、反序列化的支持
* 增加了对 JSON 序列化、反序列化过程进行自定义的接口

### 更改
* 将 `Data` 类重命名为 `Archive` 类
* 将 `LitJson` 库移动至 `Runtime` 文件夹
  


## [1.1.1] - 2023-12-1
### 修复
* 修复了无法正常读取存档中的 `float` 值的 bug
  
  

## [1.1.2] - 2023-12-1
### 新增
* 对 `Archive` 类的单元测试

### 更改
* 现在 `StringIDDrawer` 的绘制不再需要 `FieldInfo` 了

### 修复
* 修复了 `StrID` 的 `prefix` 参数无效的 bug
  

  
## [1.1.3] - 2023-12-2
### 修复
* 修复了部分情况下，`Archive` 解析器匹配报错的bug
  

## [1.1.4] - 2024-1-9
### 更改
* 为说明文件 `README.md` `CHANGELOG.md` 等添加 `.meta` 文件以使引擎识别
  

### 修复
* 修复了 `Processable` 无法重写 `Process` 和 `PhysicsProcess` 方法的问题
  

## [1.1.5] - 2024-1-24
### 新增
* `DraggableUI.ResetDragObj()` 方法，让拖拽物还原到原来的位置与父子关系
  

### 更改
* 将 `DraggableUI` 的 Unity 消息改为 `virtual`, 便于拓展
* 现在 `DraggableUI` 在拖拽时，会将拖拽物放置于 `Canvas` 的直接子物体处，防止遮罩等影响
  

### 修复
* 修复了 `Tween` 可能被多次回收入对象池的 bug
  

## [1.1.6] - 2024-1-25
### 新增
* `CustomConfigEditor`: 用于创建组件化 `ScriptableObject` 型配置文件的编辑器支持(初步)
  

### 修复
* 修复了 `Tween` 在切换场景时报错的 bug
  

## [1.2.0] -2024-1-29
### 新增
* `CompositeConfigEditor`: 将 `ScriptableObject` 拆分为组件化配置的编辑器拓展抽象
* `CompositeComponentAttribute`: 用于注册适用于 `CompositeConfigEditor` 的组件的 `Attribute`
  

### 更改
* `CustomConfigEditor` 更名为 `CompositeConfigEditor`
  
  
### 修复
* 修复了打包时未将 `DevKit.Editor` 集合排除引发的报错


## [1.2.1] - 2024-1-30
### 修复
* 调优了 `CustomConfigEditor` 在组件被删除时的表现


## [1.2.2] - 2024-2-8
### 新增
* `RelayProcesser<T>` 中继处理器，用于实现多层处理结构
* `DefaultRelayProcesser` 泛用型中继处理器
  

### 更改
* `Processer<T>` 现在成为对 `RelayProcesser<T>` 的 Mono 封装
  


## [1.2.3] - 2024-2-8
### 新增
* `RelayProcesserMono<T>` 和 `DefaultRelayProcesserMono`: 可添加为组件的中继处理器
* `IProcesser<T>`: 处理器抽象接口
  

### 更改
* `Processer<T>` 更名为 `ProcesserMono<T>`
* `DefaultProcesser` 更名为 `DefaultProcesserMono`  
  

## [1.2.4] - 2024-2-10
### 新增
* `StrID` 现在支持一种新前缀 `$scene`
  * 使用该前缀的 ID 会自动以当前场景的名称作为前缀
  * 要筛选，只需要在其他 `StrID` 传入 `prefix:"你的场景名称"`
  * 非常适合【场景限定】ID 使用
  

### 修复
* 修复了 `CompositeConfigEditor` 内无法编辑配置的 bug  

  
## [1.3.0] - 2024-2-16
### 新增
* 添加了状态机 `FSM<T>` 和对应的状态 `FSMState`
  * `FSM<T>` 将会自动实例化 `T` 指定的状态对象


### 更改
* 现在 `Tween` 改用 `Stack` 作为对象池，而非 Unity 内置的 `ObjectPool<Tween>`
* 添加了 `Tween` 的内存安全检查，可以阻止用户代码访问已被回收的 `Tween`
* 添加了 `Tween.Verbose` ，用于打印额外的 Debug 信息


## [1.3.1] - 2024-2-16
### 更改
* `FSM` 现在不再是泛型模式了
* `FSM` 现在需要重写 `FSM.DefineStates()` 与 `FSM.GetDefaultState()` 来实现定义所有状态和初始状态  
  

## [1.3.2] - 2024-2-22
### 新增
* 现在 `TitleAttribute` 对于结构体和 `MultilineAttribute` 增加了支持
  

### 更改
* `LinedPropertyDrawer` 现在使用 `EditorGUIUtility.standardVerticalSpacing` 作为默认纵向间距