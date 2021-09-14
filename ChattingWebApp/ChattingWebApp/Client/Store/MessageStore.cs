using ChattingWebApp.Shared.Models;
using Fluxor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChattingWebApp.Client.Store
{
    public record MessageState
    {
        public Message CurrentMessage { get; init; }
    }

    public class MessageFeature : Feature<MessageState>
    {
        public override string GetName()=> nameof(MessageState);

        protected override MessageState GetInitialState()
        {
            return new MessageState()
            {
                CurrentMessage = new()
            };
        }
    }

    public class OnSetMessageAction
    {
        public OnSetMessageAction(Message message)
        {
            Message = message;
        }

        public Message Message { get; }
    }
    public class OnResetMessageAction { }

    public class MessageReducers
    {
        [ReducerMethod]
        public static MessageState OnSetMessage(MessageState state, OnSetMessageAction action)
        {
            return state with
            {
                CurrentMessage = action.Message
            };
        }
        [ReducerMethod(typeof(OnResetMessageAction))]
        public static MessageState OnResetMessage(MessageState state)
        {
            return state with
            {
                CurrentMessage = new() 
                { 
                    FromUserID = state.CurrentMessage.FromUserID, 
                    ToUserID = state.CurrentMessage.ToUserID,
                    MessageText = string.Empty
                }
            };
        }
    }
}
