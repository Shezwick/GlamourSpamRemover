using Dalamud.Game.Command;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using GlamourSpamRemover.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace GlamourSpamRemover;

public sealed class GlamourSpamRemoverPlugin : IDalamudPlugin
{
    [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;
    [PluginService] internal static IChatGui ChatGui { get; private set; } = null!;

    public readonly WindowSystem WindowSystem = new("GlamourSpamRemover");
    private const string DebugCommandName = "/gsrdebug";
    private DebugConsole DebugConsole { get; init; }
    public static readonly List<string> DetectedGlamourMessages = [];

    private const ushort GlamourCastMessageId = 2105;
    private static readonly List<string> GlamourMessages =
    [
        "(cast|casts) a glamour\\.",        // EN
        "Glamours projected from plate ",   // EN
        "(projetez|projette) un mirage\\.", // FR
        "Vous vous équipez de la tenue ",   // FR
        "(projizierst|projiziert) ein",     // DE
        "Die Projektionsplatte ",           // DE
        "選択したポートレート",                // JP
        "の外見を武具投影した",                // JP
        "외관을 투영했습니다\\.",              // KR
        "환영을 장비에 투영했습니다\\."        // KR
    ];

    public GlamourSpamRemoverPlugin()
    {
        DebugConsole = new DebugConsole(this);
        WindowSystem.AddWindow(DebugConsole);

        ChatGui.ChatMessage += OnChatMessageDelegate;

        CommandManager.AddHandler(DebugCommandName, new CommandInfo(OnDebugCommand)
        {
            HelpMessage = "Toggle debug console"
        });

        PluginInterface.UiBuilder.Draw += DrawUI;
    }

    public void Dispose()
    {
        ChatGui.ChatMessage -= OnChatMessageDelegate;
        CommandManager.RemoveHandler(DebugCommandName);
        PluginInterface.UiBuilder.Draw -= DrawUI;
    }

    public static void OnChatMessageDelegate(XivChatType type, int timestamp, ref SeString sender, ref SeString message, ref bool isHandled)
    {
        var messageText = message.TextValue;
        var hasGlamourMessageId = (ushort)type == GlamourCastMessageId;
        var isAGlamoutMessage = GlamourMessages.Any(glamourMessage => Regex.IsMatch(messageText, glamourMessage));

        //Note: Other system messages use the same id (like retainer ventures), need both checks
        if (hasGlamourMessageId && isAGlamoutMessage)
        {
            isHandled = true;
        }

        if (hasGlamourMessageId || isAGlamoutMessage)
        {
            DetectedGlamourMessages.Insert(0, $"[{(ushort)type}] {messageText}");

            //Capped to avoid memory issues. Removes oldest message
            if (DetectedGlamourMessages.Count > 50)
            {
                DetectedGlamourMessages.RemoveAt(DetectedGlamourMessages.Count - 1);
            }
        }
    }
    
    private void OnDebugCommand(string command, string args)
    {
        DebugConsole.Toggle();
    }

    private void DrawUI() => WindowSystem.Draw();
    public void ToggleDebugConsole() => DebugConsole.Toggle();
}
