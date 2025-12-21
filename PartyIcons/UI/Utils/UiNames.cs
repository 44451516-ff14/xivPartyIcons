using System;
using PartyIcons.Configuration;
using PartyIcons.Runtime;

namespace PartyIcons.UI.Utils;

public static class UiNames
{
    public static string GetName(NameplateMode mode)
    {
        return mode switch
        {
            NameplateMode.Default => "游戏默认",
            NameplateMode.Hide => "隐藏",
            NameplateMode.BigJobIcon => "大职业图标",
            NameplateMode.SmallJobIcon => "小职业图标和名称",
            NameplateMode.SmallJobIconAndRole => "小职业图标、角色和名称",
            NameplateMode.BigJobIconAndPartySlot => "大职业图标和队伍编号",
            NameplateMode.RoleLetters => "角色字母",
            _ => $"未知 ({(int)mode}/{mode.ToString()})"
        };
    }

    public static string GetName(ZoneType zoneType)
    {
        return zoneType switch
        {
            ZoneType.Overworld => "野外",
            ZoneType.Dungeon => "地下城",
            ZoneType.Raid => "团队副本",
            ZoneType.AllianceRaid => "大型任务",
            ZoneType.ChaoticRaid => "混乱副本",
            ZoneType.FieldOperation => "特殊区域",
            _ => $"未知 ({(int)zoneType}/{zoneType.ToString()})"
        };
    }

    public static string GetName(StatusConfig config)
    {
        return config.Preset switch
        {
            StatusPreset.Custom => config.Name ?? "<未命名>",
            StatusPreset.Overworld => "野外",
            StatusPreset.Instances => "副本",
            StatusPreset.FieldOperations => "特殊区域",
            StatusPreset.OverworldLegacy => "野外（旧版）",
            _ => config.Preset + "/" + config.Name + "/" + config.Id
        };
    }

    public static string GetName(StatusSelector selector)
    {
        return GetName(Plugin.Settings.GetStatusConfig(selector));
    }

    public static string GetName(DisplayConfig config)
    {
        if (config.Preset == DisplayPreset.Custom) {
            return $"{GetName(config.Mode)} ({config.Name})";
        }

        return GetName(config.Mode);
    }

    public static string GetName(DisplaySelector selector)
    {
        return GetName(Plugin.Settings.GetDisplayConfig(selector));
    }

    public static string GetName(IconSetId id)
    {
        return id switch
        {
            IconSetId.EmbossedFramed => "带框，角色颜色",
            IconSetId.EmbossedFramedSmall => "带框，角色颜色（小）",
            IconSetId.Gradient => "渐变，角色颜色",
            IconSetId.Glowing => "发光",
            IconSetId.Embossed => "浮雕",
            IconSetId.Inherit => "<使用全局设置>",
            _ => id.ToString()
        };
    }

    public static string GetName(ChatMode mode)
    {
        return mode switch
        {
            ChatMode.GameDefault => "游戏默认",
            ChatMode.Role => "角色",
            ChatMode.Job => "职业缩写",
            _ => throw new ArgumentException($"Unknown chat mode {mode}")
        };
    }

    public static string GetName(RoleDisplayStyle style)
    {
        return style switch
        {
            RoleDisplayStyle.None => "无",
            RoleDisplayStyle.Role => "角色",
            RoleDisplayStyle.PartyNumber => "队伍编号",
            _ => throw new ArgumentException($"Unknown RoleDisplayStyle {style}")
        };
    }

    public static string GetName(StatusVisibility visibility)
    {
        return visibility switch
        {
            StatusVisibility.Hide => "隐藏",
            StatusVisibility.Show => "显示",
            StatusVisibility.Important => "重要",
            _ => visibility.ToString()
        };
    }

    public static string GetName(StatusSwapStyle style)
    {
        return style switch
        {
            StatusSwapStyle.None => "无",
            StatusSwapStyle.Swap => "交换",
            StatusSwapStyle.Replace => "替换",
            _ => style.ToString()
        };
    }

    public static string GetName(NameplateSizeMode mode)
    {
        return mode switch
        {
            NameplateSizeMode.Smaller => "较小",
            NameplateSizeMode.Medium => "中等",
            NameplateSizeMode.Bigger => "较大",
            NameplateSizeMode.Custom => "自定义",
            _ => mode.ToString()
        };
    }
}