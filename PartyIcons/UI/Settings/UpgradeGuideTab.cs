using Dalamud.Interface.Colors;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using PartyIcons.Configuration;
using PartyIcons.Runtime;
using PartyIcons.UI.Utils;
using System.Linq;
using System.Numerics;

namespace PartyIcons.UI.Settings;

public static class UpgradeGuideTab
{
    public static bool ForceRedisplay { get; set; }

    public static void Draw()
    {
        var buttonSize = new Vector2(100f, 80f) * ImGuiHelpers.GlobalScale;

        using (ImRaii.PushColor(ImGuiCol.Text, ImGuiColors.ParsedGreen)) {
            ImGui.TextWrapped("队伍图标升级指南（适用于版本 1.2）");
        }

        ImGui.TextWrapped("Party Icons 的 Dawntrail 更新对状态图标的显示方式进行了一些更改。");
        using (ImRaii.PushColor(ImGuiCol.Text, ImGuiColors.DalamudGrey2)) {
            ImGui.TextWrapped("（状态图标是在线状态的图标，例如断开连接、观看过场动画、等待副本、导师、新冒险者（豆芽）等。）");
        }
        ImGui.TextWrapped("从这个版本开始，现在可以同时显示职业图标和状态图标，并且可以为每种名牌显示类型自定义此设置。");

        ImGuiExt.Spacer(8);
        using (ImRaii.PushColor(ImGuiCol.Text, ImGuiColors.DalamudYellow)) {
            ImGui.TextWrapped("您希望如何显示状态图标？");
        }
        ImGuiExt.Spacer(8);

        if (ImGui.Button("使用新的\n    默认值", buttonSize)) {
            UseNewDefaults();
        }
        ImGui.SameLine();
        ImGui.TextWrapped("在副本中，某些重要的状态会与职业图标交换，但大多数情况下是隐藏的。在野外，大多数状态会在自己的状态图标槽位中显示。");
        ImGui.Separator();

        if (ImGui.Button("复制旧的\n  优先系统", buttonSize)) {
            ReplicatePriority();
        }
        ImGui.SameLine();
        ImGui.TextWrapped("某些重要的状态图标（如断开连接、离开或观看过场动画）在适当时会完全替换职业图标，但状态图标在其他情况下是隐藏的。");
        ImGui.Separator();

        if (ImGui.Button("完全不显示\n  状态图标", buttonSize)) {
            NoIcons();
        }
        ImGui.SameLine();
        ImGui.TextWrapped("仅显示职业图标。");
        ImGui.Separator();

        if (ForceRedisplay) {
            if (ImGui.Button("取消", buttonSize)) {
                Cancel();
            }
            ImGui.SameLine();
            ImGui.TextWrapped("不会进行任何更改。");
            ImGui.Separator();
        }

        ImGui.TextWrapped("请选择上述选项之一以继续。您可以从\"通用\"标签页再次打开此窗口，或者从\"外观\"和\"状态图标\"标签页更详细地自定义状态图标可见性。");
    }

    private static void UseNewDefaults()
    {
        foreach (var config in Plugin.Settings.DisplayConfigs.Configs.Where(c => c.Preset != DisplayPreset.Custom)) {
            config.StatusIcon.Show = true;
            config.SwapStyle = StatusSwapStyle.Swap;
            config.StatusSelectors[ZoneType.Overworld] = new StatusSelector(StatusPreset.Overworld);
        }
        Plugin.Settings.SelectorsDialogComplete = true;
        Plugin.Settings.Save();
        ForceRedisplay = false;

    }

    private static void ReplicatePriority()
    {
        foreach (var config in Plugin.Settings.DisplayConfigs.Configs.Where(c => c.Preset != DisplayPreset.Custom)) {
            config.StatusIcon.Show = true;
            config.SwapStyle = StatusSwapStyle.Replace;
            config.StatusSelectors[ZoneType.Overworld] = new StatusSelector(StatusPreset.OverworldLegacy);
        }
        Plugin.Settings.SelectorsDialogComplete = true;
        Plugin.Settings.Save();
        ForceRedisplay = false;
    }

    private static void NoIcons()
    {
        foreach (var config in Plugin.Settings.DisplayConfigs.Configs.Where(c => c.Preset != DisplayPreset.Custom)) {
            config.StatusIcon.Show = false;
        }
        Plugin.Settings.SelectorsDialogComplete = true;
        Plugin.Settings.Save();
        ForceRedisplay = false;
    }

    private static void Cancel()
    {
        ForceRedisplay = false;
    }
}