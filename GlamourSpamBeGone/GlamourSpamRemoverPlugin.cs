using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using System.Collections.Generic;
using System.Linq;

namespace GlamourSpamRemover;

public sealed class GlamourSpamRemoverPlugin : IDalamudPlugin
{
    [PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;
    [PluginService] internal static IChatGui ChatGui { get; private set; } = null!;

    private const ushort GlamourCastMessageId = 2105;
    private static readonly List<string> GlamourMessages =
    [
        "You cast a glamour.",              // EN
        "Glamours projected from plate ",   // EN
        "Vous projetez un mirage.",         // FR
        "Vous vous équipez de la tenue ",   // FR
        "Du projizierst ein",               // DE
        "Die Projektionsplatte ",           // DE
        "選択したポートレート",                // JP
        "の外見を武具投影した"                 // JP
    ];

    public GlamourSpamRemoverPlugin()
    {
        ChatGui.ChatMessage += OnChatMessageDelegate;
    }

    public void Dispose()
    {
        ChatGui.ChatMessage -= OnChatMessageDelegate;
    }

    public static void OnChatMessageDelegate(XivChatType type, int timestamp, ref SeString sender, ref SeString message, ref bool isHandled)
    {
        if ((ushort)type == GlamourCastMessageId) {
            var messageText = message.TextValue;

            //Note: Test if there are any other messages using this chat id. If not, can drop the language checks and just remove always
            if (GlamourMessages.Any(glamourMessage => messageText.Contains(glamourMessage)))
            {
                isHandled = true;
            }
        }
    }
}
