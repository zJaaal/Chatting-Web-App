using ChattingWebApp.Shared.Models;
using Fluxor;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ChattingWebApp.Client.Store
{
    public record RegisterState
    {
        public bool IsValid { get; init; }
        public string ErrorMessage { get; init; }
        public bool Validating { get; init; }
        public User User { get; init; }
    }

    public class RegisterFeature : Feature<RegisterState>
    {
        public override string GetName() => nameof(RegisterState);


        protected override RegisterState GetInitialState()
        {
            return new RegisterState()
            {
                IsValid = false,
                ErrorMessage = string.Empty,
                Validating = false,
                User = new User()
            };
        }
    }
    public class RegisterValidateAction 
    {
        public User user { get; }

        public RegisterValidateAction(User user)
        {
            this.user = user;
        }
    }
    public class RegisterOnValidatingAction { }
    public class RegisterOnResetAction { }
    public class RegisterIsValidSetAction 
    {
        public bool isValid { get; }

        public RegisterIsValidSetAction(bool IsValid)
        {
            isValid = IsValid;
        }
    }
    public class RegisterErrorMessageSetAction
    {
        public string message;

        public RegisterErrorMessageSetAction(string message)
        {
            this.message = message;
        }
    }
    public class RegisterRegisteringAction
    {
        public User user { get; }

        public RegisterRegisteringAction(User user)
        {
            this.user = user;
        }
    }

    public static class RegisterReducers
    {
        [ReducerMethod(typeof(RegisterOnValidatingAction))]
        public static RegisterState OnValidating(RegisterState state)
        {
            return state with
            {
                Validating = true
            };
        }
        [ReducerMethod(typeof(RegisterOnResetAction))]
        public static RegisterState OnReset(RegisterState state)
        {
            return state with
            {
                IsValid = false,
                ErrorMessage = string.Empty,
                Validating = false,
                User = new User()
            };
        }
        [ReducerMethod]
        public static RegisterState SetIsValid(RegisterState state, RegisterIsValidSetAction action)
        {
            return state with
            {
                IsValid = action.isValid,
                Validating = false
                
            };
        }
        [ReducerMethod]
        public static RegisterState SetErrorMessage(RegisterState state,RegisterErrorMessageSetAction action)
        {
            return state with
            {
                ErrorMessage = action.message
            };
        }
    }
    public class RegisterEffects
    {
        private readonly HttpClient http;
        private readonly NavigationManager _nav;

        public RegisterEffects(HttpClient http, NavigationManager nav)
        {
            this.http = http;
            _nav = nav;
        }

        [EffectMethod]
        public async Task ValidateUser(RegisterValidateAction validateAction, IDispatcher dispatcher)
        {
            dispatcher.Dispatch(new RegisterOnValidatingAction());
            var user = await http.GetFromJsonAsync<User>($"user/getuserbynickname/{validateAction.user.Nickname}");

            if (!validateAction.user.Nickname.ToLower().Equals(user.Nickname.ToLower()))
            {
                dispatcher.Dispatch(new RegisterIsValidSetAction(true));
                dispatcher.Dispatch(new RegisterErrorMessageSetAction("Username is Valid"));
                return;
            }
            dispatcher.Dispatch(new RegisterIsValidSetAction(false));
            dispatcher.Dispatch(new RegisterErrorMessageSetAction("Username is currently in use"));
        }
        [EffectMethod]
        public async Task RegisterUser(RegisterRegisteringAction registerAction, IDispatcher dispatcher)
        {
            await http.PostAsJsonAsync("user/registeruser", registerAction.user);
            _nav.NavigateTo("/", true);
        }
    }
}
