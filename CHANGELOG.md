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

# [1.1.1] - 2023-12-1
### 修复
* 修复了无法正常读取存档中的 `float` 值的 bug