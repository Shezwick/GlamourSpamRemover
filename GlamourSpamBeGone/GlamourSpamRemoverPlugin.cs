using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

namespace GlamourSpamRemover;

public sealed class GlamourSpamRemoverPlugin : IDalamudPlugin
{
    [PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;
    [PluginService] internal static IChatGui ChatGui { get; private set; } = null!;

    private const ushort GlamourCastMessageId = 2105;

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

            if (messageText.Contains("You cast a glamour.") || messageText.Contains("Glamours projected from plate "))
            {
                isHandled = true;
            }
        }
    }
}
