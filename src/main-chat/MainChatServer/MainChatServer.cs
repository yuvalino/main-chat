using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainChatServer
{
    public class MainChatServer : BaseScript
    {
        public MainChatServer()
        {
            EventHandlers["_chat:messageEntered"] += new Action<Player, string, List<object>, string>(OnMessageEntered);
            EventHandlers["__cfx_internal:commandFallback"] += new Action<Player, string>(OnCommandFallback);
        }

        private void OnMessageEntered([FromSource]Player player, string author, List<object> color, string message)
        {
            if (string.IsNullOrWhiteSpace(message) || string.IsNullOrWhiteSpace(author)) return;

            TriggerEvent("chatMessage", int.Parse(player.Handle), author, message);

            if (!Function.Call<bool>(Hash.WAS_EVENT_CANCELED))
                TriggerClientEvent("chatMessage", author, new byte[] { 255, 255, 255 }, message);

            Debug.WriteLine(author + ": " + message);
        }

        private void OnCommandFallback([FromSource]Player player, string command)
        {
            var name = player.Name;
            
            TriggerEvent("chatMessage", int.Parse(player.Handle), name, "/" + command);

            if (!Function.Call<bool>(Hash.WAS_EVENT_CANCELED))
                TriggerClientEvent("chatMessage", -1, name, new byte[] { 255, 255, 255 }, "/" + command);

            Function.Call(Hash.CANCEL_EVENT);
        }
    }
}
