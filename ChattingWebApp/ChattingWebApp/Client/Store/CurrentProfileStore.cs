using ChattingWebApp.Shared.Models;
using Fluxor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ChattingWebApp.Client.Store
{
    public record UserProfileState
    {
        public bool IsLoading { get; init; }
        public string ErrorMessage { get; init; }
        public Profile UserProfile { get; init; }

    }
    public class UserProfileFeature : Feature<UserProfileState>
    {
        public override string GetName() => nameof(UserProfileState);

        protected override UserProfileState GetInitialState()
        {
            return new UserProfileState()
            {
                IsLoading = false,
                UserProfile = new(),
                ErrorMessage = string.Empty
            };
        }
    }

    public class UserProfileOnSetProfileAction
    {
        public UserProfileOnSetProfileAction(Profile profile)
        {
            Profile = profile;
        }

        public Profile Profile { get; }
    }
    public class UserProfileUpdateProfileAction
    {
        public UserProfileUpdateProfileAction(Profile profile)
        {
            Profile = profile;
        }

        public Profile Profile { get; }
    }

    public class UserProfileOnLoadingAction { }
    public class UserProfileGetAction { }
    public class UserProfileSetErrorMessageAction 
    {
        public UserProfileSetErrorMessageAction( string message)
        {
            Message = message;
        }

        public string Message { get; }
    }

    public class UserProfileReducers
    {
        [ReducerMethod(typeof(UserProfileOnLoadingAction))]
        public static UserProfileState OnLoading(UserProfileState state)
        {
            return state with
            {
                IsLoading = true,
                ErrorMessage = string.Empty
            };
        }
        [ReducerMethod]
        public static UserProfileState OnSetProfile(UserProfileState state, UserProfileOnSetProfileAction action)
        {
            return state with
            {
                IsLoading = false,
                UserProfile = action.Profile
            };
        }
        [ReducerMethod]
        public static UserProfileState OnSetErrorMessage(UserProfileState state, UserProfileSetErrorMessageAction action)
        {
            return state with
            {
                IsLoading = false,
                ErrorMessage = action.Message
            };
        }
    }
    
    public class UserProfileEffects
    {
        private readonly IHttpClientFactory httpClient;

        public UserProfileEffects(IHttpClientFactory httpClient)
        {
            this.httpClient = httpClient;
        }

        [EffectMethod]
        public async Task OnUpdateProfile(UserProfileUpdateProfileAction action, IDispatcher dispatcher)
        {
            if(action.Profile.Nickname.Length <= 5)
            {
                dispatcher.Dispatch(new UserProfileSetErrorMessageAction("Nickname is too short. Must have at least 6 characters."));
                return;
            }
            else if (string.IsNullOrWhiteSpace(action.Profile.Nickname))
            {
                dispatcher.Dispatch(new UserProfileSetErrorMessageAction("Nickname field is required."));
                return;
            }

            dispatcher.Dispatch(new UserProfileOnLoadingAction());
            var http = httpClient.CreateClient("ContactsClient");
            await http.PutAsJsonAsync($"contacts/updateprofile/{action.Profile.UserID}", action.Profile);
            dispatcher.Dispatch(new UserProfileOnSetProfileAction(action.Profile));
        }
        [EffectMethod(typeof(UserProfileGetAction))]
        public async Task OnGetProfile(IDispatcher dispatcher)
        {
            dispatcher.Dispatch(new UserProfileOnLoadingAction());
            var http = httpClient.CreateClient("ContactsClient");
            var profile = await http.GetFromJsonAsync<Profile>("contacts/getcurrentprofile");
            dispatcher.Dispatch(new UserProfileOnSetProfileAction(profile));
        }
    }
}

