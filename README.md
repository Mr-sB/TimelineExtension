# TimelineExtension 功能与使用说明

## 概述

`UnityEngine.Timeline.Extensions` 命名空间下的 Timeline 扩展库，包含两部分：**PlayableDirector 扩展方法**和**基于字符串 Key 的增强 Signal 系统**。

---

## 一、PlayableDirectorExtensions

对 `PlayableDirector` 的扩展方法，简化 Track 绑定操作。

### 方法列表

| 方法 | 说明 |
|------|------|
| `FindTrackAsset(trackName)` | 按名称查找 Track |
| `GetBingingObject(trackName)` | 获取指定 Track 当前绑定的对象 |
| `SetBingingObject(trackName, obj)` | 设置指定 Track 的绑定对象 |
| `SetBingingObjectAndCheck(trackName, go)` | 智能绑定：自动根据 Track 的 `TrackBindingType` 提取/添加所需组件 |

### 使用示例

```csharp
// 直接按 Track 名字绑定对象
playableDirector.SetBingingObject("AnimationTrack", targetAnimator);

// 智能绑定：传入 GameObject，自动取出 Track 需要的组件类型
// 若 Track 需要 Animator，会自动调用 go.GetComponent<Animator>()，没有则 AddComponent
playableDirector.SetBingingObjectAndCheck("AnimationTrack", targetGO);
```

**`SetBingingObjectAndCheck` 的处理逻辑：**
- Track 需要 `GameObject` → 直接绑定 GO
- Track 需要 `Transform` → 绑定 `go.transform`
- 其他组件类型 → `GetComponent`，没有则 `AddComponent`

---

## 二、增强 Signal 系统（Events）

对 Unity 内置 Signal 系统的扩展，**用字符串 Key 替代 SignalAsset**，并新增 **Range（片段式）信号**和**暂停事件**。

### 核心组件

#### `SignalTrackEx`
Timeline 中的自定义 Track，绑定目标必须是 `SignalReceiverEx` 组件。支持放置两种信号：`SignalEmitterEx`（点标记）和 `SignalEmitterRange`（片段）。

#### `SignalEmitterEx`（点标记）
在 Timeline 上的某一**时间点**触发信号，等同于内置 SignalEmitter，但使用字符串 Key。

| 属性 | 说明 |
|------|------|
| `notificationKey` | 信号标识 Key，对应 Receiver 注册的监听 |
| `notificationValue` | 附带传递的字符串数据 |
| `retroactive` | 若播放起点在此标记之后，是否补发信号 |
| `emitOnce` | 循环播放时是否只触发一次 |
| `isPause` | 固定为 `false`（点标记不区分暂停） |

#### `SignalEmitterRange`（片段/Clip）
在 Timeline 上占据**一段时间范围**的信号片段，会在两个时机触发：
- **进入片段时**（`ProcessFrame` 首帧）：触发普通事件（`isPause = false`）
- **离开/暂停时**（`OnBehaviourPause`）：触发暂停事件（`isPause = true`）

属性与 `SignalEmitterEx` 相同。

#### `SignalReceiverEx`
挂载在 GameObject 上的接收器，实现 `INotificationReceiver`，维护两组事件字典：

- **普通事件**：对应 `SignalEmitterEx` 触发，以及 `SignalEmitterRange` 进入时
- **暂停事件**：对应 `SignalEmitterRange` 离开/暂停时

所有事件回调签名均为 `UnityAction<string>`，参数是 `notificationValue`。

### API

```csharp
// 注册普通事件
receiver.AddAction("key_name", OnSignalFired);

// 注册暂停事件（Range 片段结束时触发）
receiver.AddPauseAction("key_name", OnSignalPaused);

// 移除监听
receiver.RemoveAction("key_name", OnSignalFired);
receiver.RemovePauseAction("key_name", OnSignalPaused);

void OnSignalFired(string value) { /* value 是 notificationValue */ }
void OnSignalPaused(string value) { }
```

---

## 三、与内置 Signal 系统的对比

| 特性 | Unity 内置 Signal | TimelineExtension Signal |
|------|------------------|--------------------------|
| 信号标识 | SignalAsset（资源文件） | 字符串 Key |
| 携带数据 | 不支持 | 支持字符串 Value |
| 触发形式 | 仅点标记 | 点标记 + 片段两端 |
| 暂停回调 | 无 | `SignalEmitterRange` 支持 |
| 配置复杂度 | 需创建 ScriptableObject | 直接填字符串，无需额外资源 |

---

## 四、典型使用场景

1. **动态替换 Timeline 绑定**：切换角色时，用 `SetBingingObjectAndCheck` 批量替换各 Track 目标，无需手动取组件。

2. **Timeline 区间进入/退出回调**：用 `SignalEmitterRange` 在某段动画播放期间启用某个系统，片段结束时自动关闭。

3. **带参数的 Timeline 事件**：通过 `notificationValue` 传递字符串参数（如道具 ID、状态名），在回调中读取处理。
