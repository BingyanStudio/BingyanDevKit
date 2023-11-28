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