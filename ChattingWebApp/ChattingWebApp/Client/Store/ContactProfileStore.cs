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
    public record ContactProfileState
    {
        public Profile ContactProfile { get; init; }
        public bool IsLoading { get; init; }
    }
    public class ContactProfileFeature : Feature<ContactProfileState>
    {
        public override string GetName() => nameof(ContactProfileState);

        protected override ContactProfileState GetInitialState()
        {
            return new ContactProfileState
            {
                ContactProfile = new(),
                IsLoading = false
            };
        }
    }

    public class ContactProfileGetProfileAction
    {
        public ContactProfileGetProfileAction(int Id)
        {
            this.Id = Id;
        }

        public int Id { get; }
    }
    public class ContactProfileSetProfileAction
    {
        public ContactProfileSetProfileAction(Profile currentProfile)
        {
            CurrentProfile = currentProfile;
        }

        public Profile CurrentProfile { get; }
    }

    public class ContactProfileLoadingAction { }

    public class ContactProfileReducers
    {
        [ReducerMethod(typeof(ContactProfileLoadingAction))]
        public static ContactProfileState OnLoading(ContactProfileState state)
        {
            return state with
            {
                IsLoading = true
            };
        }
        [ReducerMethod]
        public static ContactProfileState OnSetContactProfile(ContactProfileState state, ContactProfileSetProfileAction action)
        {
            return state with
            {
                ContactProfile = action.CurrentProfile,
                IsLoading = false
            };
        }
    }
    public class ContactProfileEffects
    {
        private readonly IHttpClientFactory httpClientFactory;

        public ContactProfileEffects(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }
        [EffectMethod]
        public async Task OnGetContactProfile(ContactProfileGetProfileAction action, IDispatcher dispatcher)
        {
            dispatcher.Dispatch(new ContactProfileLoadingAction());
            var http = httpClientFactory.CreateClient("ContactsClient");
            var contactProfile = await http.GetFromJsonAsync<Profile>($"contacts/getprofile/{action.Id}");
            dispatcher.Dispatch(new ContactProfileSetProfileAction(contactProfile));
        }
    }
}
