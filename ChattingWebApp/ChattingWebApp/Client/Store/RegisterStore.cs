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
    public class UserValidateAction 
    {
        public User user { get; }

        public UserValidateAction(User user)
        {
            this.user = user;
        }
    }
    public class UserOnValidatingAction { }
    public class UserOnResetAction { }
    public class UserIsValidSetAction 
    {
        public bool isValid { get; }

        public UserIsValidSetAction(bool IsValid)
        {
            isValid = IsValid;
        }
    }
    public class UserErrorMessageSetAction
    {
        public string message;

        public UserErrorMessageSetAction(string message)
        {
            this.message = message;
        }
    }
    public class UserRegisterAction
    {
        public User user { get; }

        public UserRegisterAction(User user)
        {
            this.user = user;
        }
    }

    public static class UserReducers
    {
        [ReducerMethod(typeof(UserOnValidatingAction))]
        public static RegisterState OnValidating(RegisterState state)
        {
            return state with
            {
                Validating = true
            };
        }
        [ReducerMethod(typeof(UserOnResetAction))]
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
        public static RegisterState SetIsValid(RegisterState state, UserIsValidSetAction action)
        {
            return state with
            {
                IsValid = action.isValid,
                Validating = false
                
            };
        }
        [ReducerMethod]
        public static RegisterState SetErrorMessage(RegisterState state,UserErrorMessageSetAction action)
        {
            return state with
            {
                ErrorMessage = action.message
            };
        }
    }
    public class UserEffects
    {
        private readonly HttpClient http;
        private readonly NavigationManager _nav;

        public UserEffects(HttpClient http, NavigationManager nav)
        {
            this.http = http;
            _nav = nav;
        }

        [EffectMethod]
        public async Task ValidateUser(UserValidateAction validateAction, IDispatcher dispatcher)
        {
            dispatcher.Dispatch(new UserOnValidatingAction());
            var user = await http.GetFromJsonAsync<User>($"user/getuserbynickname/{validateAction.user.Nickname}");

            if (!validateAction.user.Nickname.ToLower().Equals(user.Nickname.ToLower()))
            {
                dispatcher.Dispatch(new UserIsValidSetAction(true));
                dispatcher.Dispatch(new UserErrorMessageSetAction("Username is Valid"));
                return;
            }
            dispatcher.Dispatch(new UserIsValidSetAction(false));
            dispatcher.Dispatch(new UserErrorMessageSetAction("Username is currently in use"));
        }
        [EffectMethod]
        public async Task RegisterUser(UserRegisterAction registerAction, IDispatcher dispatcher)
        {
            await http.PostAsJsonAsync<User>("user/registeruser", registerAction.user);
            _nav.NavigateTo("/");
        }
    }
}
