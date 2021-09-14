using ChattingWebApp.Shared.Models;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChattingWebApp.Client.Store
{
    public record ChatHubState
    {
        public List<Message> Messages { get; init; }
        public bool IsConnected { get; init; }
    }
    public class ChatHubFeature : Feature<ChatHubState>
    {
        public override string GetName() => nameof(ChatHubState);

        protected override ChatHubState GetInitialState()
        {
            return new()
            {
                Messages = new(),
                IsConnected = false
            };
        }
    }

    public class OnSetIsConnectedAction 
    {
        public OnSetIsConnectedAction(bool isConnected)
        {
            IsConnected = isConnected;
        }

        public bool IsConnected { get; }
    }

    public class OnGetMessagesFromDatabaseAction //Needs endpoints
    {
        public OnGetMessagesFromDatabaseAction(List<Message> messages)
        {
            Messages = messages;
        }

        public List<Message> Messages { get; }
    }

    public class OnSendMessageChatHubAction { }
    public class OnStartChatHubAction { }
    public class OnResetListAction { }

    public class OnAddMessageToListAction
    {
        public OnAddMessageToListAction(Message message)
        {
            Message = message;
        }

        public Message Message { get; }
    }

    public class ChatHubReducers
    {
        [ReducerMethod]
        public static ChatHubState SetIsConnected(ChatHubState state, OnSetIsConnectedAction action)
        {
            return state with
            {
                IsConnected = action.IsConnected
            };
        }

        [ReducerMethod]
        public static ChatHubState AddMessage(ChatHubState state, OnAddMessageToListAction action)
        {
            state.Messages.Add(action.Message);
            return state;
        }
        [ReducerMethod(typeof(OnResetListAction))]
        public static ChatHubState OnResetList(ChatHubState state)
        {
            return state with
            {
                Messages = new()
            };
        }
    }

    public class ChatHubEffects
    {
        private readonly HubConnection _hubConnection;
        private readonly IState<MessageState> _messageState;

        public ChatHubEffects(NavigationManager navigationManager, IState<MessageState> messageState)
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(navigationManager.ToAbsoluteUri("/chathub"))
                .WithAutomaticReconnect()
                .Build();

            _messageState = messageState;
        }

        [EffectMethod(typeof(OnSendMessageChatHubAction))]
        public async Task OnSendMessage(IDispatcher dispatcher)
        {
            try
            {
                if (_hubConnection.State == HubConnectionState.Connected)
                {
                    dispatcher.Dispatch(new OnAddMessageToListAction(_messageState.Value.CurrentMessage));
                    await _hubConnection.SendAsync("SendMessage", _messageState.Value.CurrentMessage);
                }
            }
            catch (Exception ex)
            { 
            }
        }

        [EffectMethod(typeof(OnStartChatHubAction))]
        public async Task Start(IDispatcher dispatcher)
        { 
            _hubConnection.Reconnecting += (ex) =>
            {
                dispatcher.Dispatch(new OnSetIsConnectedAction(false));
                return Task.CompletedTask;
            };

            _hubConnection.Reconnected += (connectionId) =>
            {
                dispatcher.Dispatch(new OnSetIsConnectedAction(true));
                return Task.CompletedTask;
            };

            _hubConnection.On<Message>("ReceiveMessage", (message) =>
            {
                if(message.ToUserID == _messageState.Value.CurrentMessage.FromUserID && 
                   message.FromUserID == _messageState.Value.CurrentMessage.ToUserID)
                {
                    dispatcher.Dispatch(new OnAddMessageToListAction(message));
                }
            });

            await _hubConnection.StartAsync();

            dispatcher.Dispatch(new OnSetIsConnectedAction(true));
        }
    }
}
