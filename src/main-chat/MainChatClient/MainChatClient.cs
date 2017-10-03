using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MainChatClient
{
    public class MainChatClient : BaseScript
    {
        #region Private Fields
        private bool _firstTick = false;
        private bool _chatActive = false;
        private bool _chatActivating = false;
        #endregion

        public MainChatClient()
        {
            Tick += OnTick;

            EventHandlers["chatMessage"] += new Action<string, List<object>, string>(OnChatMessage);
            EventHandlers["__cfx_internal:serverPrint"] += new Action<string>(OnServerPrint);
            EventHandlers["chat:addMessage"] += new Action<string>(OnAddMessage);
            EventHandlers["chat:clear"] += new Action<string>(OnChatClear);
        }

        private void OnLoad()
        {
            API.SetTextChatEnabled(false);
            API.SetNuiFocus(false, false);
            RegisterNUICallback("chatResult", NUI_OnChatResult);
            RegisterNUICallback("loaded", NUI_OnLoaded);
        }

        private class Data
        {
            public bool canceled { get; set; }
            public string message { get; set; }
        }

        #region Private Methods
        private CallbackDelegate NUI_OnChatResult(IDictionary<string, object> data, CallbackDelegate result)
        {
            _chatActive = false;
            API.SetNuiFocus(false, false);

            string message = data.GetVal<string>("message", null);
            bool cancel = data.GetVal("canceled", false);

            if (!string.IsNullOrWhiteSpace(message) && !cancel)
            {
                if (message.StartsWith("/"))
                    API.ExecuteCommand(message.Substring(1));
                else
                    TriggerServerEvent("_chat:messageEntered", Game.Player.Name, new byte[] { 0, 0x99, 255 }, message);
            }
            result("ok");
            return result;
        }

        private CallbackDelegate NUI_OnLoaded(IDictionary<String, object> data, CallbackDelegate result)
        {
            TriggerServerEvent("chat:init");
            result("ok");
            return result;
        }

        private void OnChatMessage(string author, List<object> color, string text)
        {
            SendNUIMessage("{\"type\":\"ON_MESSAGE\", \"message\": { \"color\": [" + color[0] + "," + color[1] + "," + color[2] + "], \"multiline\":true, \"args\":[" + (string.IsNullOrWhiteSpace(text) ? "\"" + author + "\"" : "\"" + author + "\"" + "," + "\"" + text + "\"") + "]}}");
        }

        private void OnServerPrint(string msg)
        {
            SendNUIMessage("{\"type\":\"ON_MESSAGE\", \"message\": { \"color\": [255,255,255], \"multiline\":true, \"args\":[\"" + msg + "\"]}}");
        }

        private void OnAddMessage(string message)
        {
            throw new NotImplementedException();
        }

        private void OnChatClear(string name)
        {
            SendNUIMessage("{\"type\":\"ON_CLEAR\"}");
        }
        #endregion

        #region Task
        public async Task OnTick()
        {
            if(!_firstTick)
            {
                OnLoad();
                _firstTick = true;
            }
            if (!_chatActive)
            {
                if (Game.IsControlJustPressed(0, Control.MpTextChatAll))
                {
                    _chatActive = true;
                    _chatActivating = true;

                    SendNUIMessage("{\"type\":\"ON_OPEN\"}");
                }
            }
            if (_chatActivating)
            {
                if (!Game.IsControlJustPressed(0, Control.MpTextChatAll))
                {
                    API.SetNuiFocus(true, true);
                    _chatActivating = false;
                }
            }
        }
        #endregion

        #region NUI Implementation
        private void RegisterNUICallback(string msg, Func<IDictionary<string, object>, CallbackDelegate, CallbackDelegate> callback)
        {
            API.RegisterNuiCallbackType(msg);

            EventHandlers[$"__cfx_nui:{msg}"] += new Action<ExpandoObject, CallbackDelegate>((body, resultCallback) =>
            {
                CallbackDelegate err = callback.Invoke(body, resultCallback);

                //if (!string.IsNullOrWhiteSpace(err)) TriggerServerEvent("_chat:messageEntered", Game.Player.Name, new byte[] { 0, 0x99, 255 }, "null");
                //Debug.WriteLine("error during NUI callback " + msg + ": " + err);
            });
        }

        private void SendNUIMessage(string message)
        {
            API.SendNuiMessage(message);
        }

        #endregion
    }

    public static class DictionaryExtensions
    {
        public static T GetVal<T> (this IDictionary<string, object> dict, string key, T defaultVal)
        {
            if(dict.ContainsKey(key))
                if (dict[key] is T)
                    return (T) dict[key];
            return defaultVal;
        }
    }
}
