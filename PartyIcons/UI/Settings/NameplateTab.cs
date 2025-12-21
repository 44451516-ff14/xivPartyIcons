using Dalamud.Interface.Colors;
using Dalamud.Interface.Components;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using PartyIcons.Configuration;
using PartyIcons.UI.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;

namespace PartyIcons.UI.Settings;

public sealed class NameplateTab
{
    private readonly Dictionary<NameplateMode, IDalamudTextureWrap> _nameplateExamples = new();

    public NameplateTab()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var examplesImageNames = new Dictionary<NameplateMode, string>
        {
            { NameplateMode.SmallJobIcon, "PartyIcons.Resources.1.png" },
            { NameplateMode.BigJobIcon, "PartyIcons.Resources.2.png" },
            { NameplateMode.BigJobIconAndPartySlot, "PartyIcons.Resources.3.png" },
            { NameplateMode.RoleLetters, "PartyIcons.Resources.4.png" }
        };

        foreach (var kv in examplesImageNames) {
            using var fileStream = assembly.GetManifestResourceStream(kv.Value);

            if (fileStream == null) {
                Service.Log.Error($"Failed to get resource stream for {kv.Value}");

                continue;
            }

            _nameplateExamples[kv.Key] = Service.TextureProvider.CreateFromImageAsync(fileStream).Result;
        }
    }

    public void Draw()
    {
        ImGui.Dummy(new Vector2(0, 2f));

        ImGui.Text("图标集：");
        ImGui.SameLine();
        ImGuiExt.SetComboWidth(Enum.GetValues<IconSetId>().Select(UiNames.GetName));

        ImGuiExt.DrawIconSetCombo("##icon_set", false, () => Plugin.Settings.IconSetId, iconSetId =>
        {
            Plugin.Settings.IconSetId = iconSetId;
            Plugin.Settings.Save();
        });

        var iconSizeMode = Plugin.Settings.SizeMode;
        ImGui.Text("名牌大小：");
        ImGui.SameLine();
        ImGuiExt.SetComboWidth(Enum.GetValues<NameplateSizeMode>().Select(x => UiNames.GetName(x)));

        using (var combo = ImRaii.Combo("##icon_size", UiNames.GetName(iconSizeMode))) {
            if (combo) {
                foreach (var mode in Enum.GetValues<NameplateSizeMode>()) {
                    if (ImGui.Selectable(UiNames.GetName(mode) + "##icon_set_" + mode)) {
                        Plugin.Settings.SizeMode = mode;
                        Plugin.Settings.Save();
                    }
                }
            }
        }

        ImGuiComponents.HelpMarker("影响所有预设，除了游戏默认。");

        if (Plugin.Settings.SizeMode == NameplateSizeMode.Custom) {
            var scale = Plugin.Settings.SizeModeCustom;
            if (ImGui.SliderFloat("自定义缩放", ref scale, 0.3f, 3f)) {
                Plugin.Settings.SizeModeCustom = Math.Clamp(scale, 0.1f, 10f);
                Plugin.Settings.Save();
            }

            ImGuiComponents.HelpMarker("按住Control并点击滑块以输入精确值");
        }

        var hideLocalNameplate = Plugin.Settings.HideLocalPlayerNameplate;
        if (ImGui.Checkbox("##hidelocal", ref hideLocalNameplate)) {
            Plugin.Settings.HideLocalPlayerNameplate = hideLocalNameplate;
            Plugin.Settings.Save();
        }

        ImGui.SameLine();
        ImGui.Text("隐藏自己的名牌");
        ImGuiComponents.HelpMarker(
            "您可以开启自己的名牌，同时开启此设置，仅使用名牌来显示自己的团队位置。\n如果您不想显示位置，可以简单地在角色设置中禁用名牌。");

        ImGuiExt.Spacer(6);

        ImGuiExt.SectionHeader("野外");
        using (ImRaii.PushIndent(15f)) {
            NameplateModeSection("##np_overworld", () => Plugin.Settings.DisplaySelectors.DisplayOverworld,
                sel => Plugin.Settings.DisplaySelectors.DisplayOverworld = sel,
                "队伍：");

            NameplateModeSection("##np_others", () => Plugin.Settings.DisplaySelectors.DisplayOthers,
                sel => Plugin.Settings.DisplaySelectors.DisplayOthers = sel,
                "其他：");
        }

        ImGuiExt.SectionHeader("副本");
        using (ImRaii.PushIndent(15f)) {
            NameplateModeSection("##np_dungeon", () => Plugin.Settings.DisplaySelectors.DisplayDungeon,
                (sel) => Plugin.Settings.DisplaySelectors.DisplayDungeon = sel,
                "地下城：");

            NameplateModeSection("##np_raid", () => Plugin.Settings.DisplaySelectors.DisplayRaid,
                sel => Plugin.Settings.DisplaySelectors.DisplayRaid = sel,
                "团队副本：");

            NameplateModeSection("##np_alliance", () => Plugin.Settings.DisplaySelectors.DisplayAllianceRaid,
                sel => Plugin.Settings.DisplaySelectors.DisplayAllianceRaid = sel,
                "大型任务：");

            NameplateModeSection("##np_chaotic", () => Plugin.Settings.DisplaySelectors.DisplayChaoticRaid,
               sel => Plugin.Settings.DisplaySelectors.DisplayChaoticRaid = sel,
               "混乱副本：");
        }

        ImGuiExt.SectionHeader("特殊区域");

        using (ImRaii.PushIndent(15f)) {
            ImGui.TextDisabled("例如：优雷卡、博兹雅");

            NameplateModeSection("##np_field_party", () => Plugin.Settings.DisplaySelectors.DisplayFieldOperationParty,
                sel => Plugin.Settings.DisplaySelectors.DisplayFieldOperationParty = sel, "队伍：");

            NameplateModeSection("##np_field_others",
                () => Plugin.Settings.DisplaySelectors.DisplayFieldOperationOthers,
                sel => Plugin.Settings.DisplaySelectors.DisplayFieldOperationOthers = sel, "其他：");
        }

        ImGuiExt.SectionHeader("PvP");

        using (ImRaii.PushIndent(15f)) {
            ImGui.TextDisabled("此插件在PvP比赛中被有意禁用。");
        }

        ImGuiExt.Spacer(15);

        if (ImGui.CollapsingHeader("示例")) {
            foreach (var kv in _nameplateExamples) {
                CollapsibleExampleImage(kv.Key, kv.Value);
            }
        }
    }

    private static void CollapsibleExampleImage(NameplateMode mode, IDalamudTextureWrap tex)
    {
        if (ImGui.CollapsingHeader(UiNames.GetName(mode))) {
            ImGui.Image(tex.Handle, new Vector2(tex.Width, tex.Height));
        }
    }

    private static void NameplateModeSection(string label, Func<DisplaySelector> getter, Action<DisplaySelector> setter,
        string title = "名牌：")
    {
        ImGui.SetCursorPosY(ImGui.GetCursorPos().Y + 3f);
        ImGui.Text(title);
        ImGui.SameLine(100f);
        ImGui.SetCursorPosY(ImGui.GetCursorPos().Y - 3f);
        ImGuiExt.SetComboWidth(Plugin.Settings.DisplayConfigs.Selectors.Select(UiNames.GetName));

        using var combo = ImRaii.Combo(label, UiNames.GetName(getter()));
        if (!combo) return;

        foreach (var selector in Plugin.Settings.DisplayConfigs.Selectors) {
            using var col = ImRaii.PushColor(ImGuiCol.Text, ImGuiColors.HealerGreen,
                selector.Preset == DisplayPreset.Custom);
            if (ImGui.Selectable(UiNames.GetName(selector), selector == getter())) {
                setter(selector);
                Plugin.Settings.Save();
            }
        }
    }
}