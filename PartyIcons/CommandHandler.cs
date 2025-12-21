using System;
using Dalamud.Game.Command;
using PartyIcons.Runtime;

namespace PartyIcons;

public class CommandHandler : IDisposable
{
    private const string commandName = "/ppi";
    
    public CommandHandler()
    {
        Service.CommandManager.AddHandler(commandName, new CommandInfo(OnCommand)
        {
            HelpMessage =
                "打开配置窗口；\"reset\" 或 \"r\" 重置所有分配"
        });
    }
    
    public void Dispose()
    {
        Service.CommandManager.RemoveHandler(commandName);
    }

    private void OnCommand(string command, string arguments)
    {
        arguments = arguments.Trim().ToLower();

        if (arguments == "" || arguments == "config")
        {
            Plugin.WindowManager.ToggleSettings();
        }
        else if (arguments == "reset" || arguments == "r")
        {
            Plugin.RoleTracker.ResetOccupations();
            Plugin.RoleTracker.ResetAssignments();
            Plugin.RoleTracker.CalculateUnassignedPartyRoles();
            Service.ChatGui.Print("职位已重置，角色已自动分配。", Service.PluginInterface.InternalName, 45);
        }
        else if (arguments == "dbg r")
        {
            Plugin.RoleTracker.ResetOccupations();
            Plugin.RoleTracker.ResetAssignments();
            Service.ChatGui.Print("职位/分配已重置。", Service.PluginInterface.InternalName, 45);
        }
        else if (arguments == "dbg state")
        {
            Service.Log.Info($"Current mode is {Plugin.NameplateView.PartyDisplay.Mode}, party count {Service.PartyList.Length}", Service.PluginInterface.InternalName, 45);
            Service.Log.Info(Plugin.RoleTracker.DebugDescription(), Service.PluginInterface.InternalName, 45);
        }
        else if (arguments == "dbg party")
        {
            PartyListHUDUpdater.DebugPartyData();
        }
    }
}