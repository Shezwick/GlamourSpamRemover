using Dalamud.Interface.Windowing;
using ImGuiNET;
using System;

namespace GlamourSpamRemover.Windows
{
    public class DebugConsole : Window, IDisposable
    {
        private readonly GlamourSpamRemoverPlugin plugin;

        public DebugConsole(GlamourSpamRemoverPlugin plugin) : base("Glamour Spam Remover Debug Console")
        {
            Flags = ImGuiWindowFlags.NoCollapse;

            this.plugin = plugin;
            IsOpen = false;
        }

        public void Dispose() { }

        public override void Draw()
        {
            ImGui.Text("Detected Glamour Messages:");
            ImGui.SameLine();

            if (ImGui.Button("Clear"))
            {
                GlamourSpamRemoverPlugin.DetectedGlamourMessages.Clear();
            }

            ImGui.SameLine();

            if (ImGui.Button("Copy"))
            {
                var messages = string.Join("\n", GlamourSpamRemoverPlugin.DetectedGlamourMessages);
                ImGui.SetClipboardText(messages);
            }
            
            ImGui.Separator();
            ImGui.Spacing();

            ImGui.BeginChildFrame(1, new System.Numerics.Vector2(0, -ImGui.GetFrameHeightWithSpacing()));

            foreach (var message in GlamourSpamRemoverPlugin.DetectedGlamourMessages)
            {
                ImGui.Text(message);
            }
            ImGui.EndChildFrame();
        }
    }
}
