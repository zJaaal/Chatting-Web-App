using Blazored.LocalStorage;
using Fluxor;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ChattingWebApp.Client.Store
{
    public record LogoutState
    {
        public string Nickname { get; init; }
    }

    public class LogoutFeature : Feature<LogoutState>
    {
        public override string GetName() => nameof(LogoutState);

        protected override LogoutState GetInitialState()
        {
            return new LogoutState()
            {
                Nickname = string.Empty
            };
        }
    }

    public class LogoutOnSetNicknameAction 
    {
        public LogoutOnSetNicknameAction(string Nickname)
        {
            this.Nickname = Nickname;
        }

        public string Nickname { get; }
    }
    public class LogoutOnExitAction
    {
        public LogoutOnExitAction(string Nickname)
        {
            this.Nickname = Nickname;
        }

        public string Nickname { get; }
    }

    public class LogoutReducer
    {
        [ReducerMethod]
        public static LogoutState OnSetNickname(LogoutState state, LogoutOnSetNicknameAction action)
        {
            return state with
            {
                Nickname = action.Nickname
            };
        }
    }

    public class LogoutEffects
    {
        private readonly HttpClient httpClient;
        private readonly ILocalStorageService localStorage;
        private readonly NavigationManager navigationManager;

        public LogoutEffects(HttpClient httpClient, ILocalStorageService localStorage, NavigationManager navigationManager)
        {
            this.httpClient = httpClient;
            this.localStorage = localStorage;
            this.navigationManager = navigationManager;
        }

        [EffectMethod]
        public async Task OnLogOut(LogoutOnExitAction action, IDispatcher dispatcher)
        {
            await httpClient.PutAsJsonAsync("user/logoutstatus", action.Nickname);
            await localStorage.RemoveItemAsync("jwt_token");
            navigationManager.NavigateTo("/", true);

        }
    }
}
