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
    public record ContactsState
    {
        public List<Profile> Profiles { get; init; }
        public bool IsLoading { get; init; }
    }

    public class ContactsFeature : Feature<ContactsState>
    {
        public override string GetName() => nameof(ContactsState);

        protected override ContactsState GetInitialState()
        {
            return new ContactsState()
            {
                Profiles = new(),
                IsLoading = false,
            };
        }
    }

    public class ContactsLoadingAction { }
    public class ContactsLoadProfilesAction { }
    public class ContactsLoadProfilesByFilterAction 
    {
        public ContactsLoadProfilesByFilterAction(string filter)
        {
            Filter = filter;
        }

        public string Filter { get; }
    }
    public class ContactsSetProfilesAction
    {
        public List<Profile> Profiles { get; }
        public ContactsSetProfilesAction(List<Profile> profiles)
        {
            Profiles = profiles;
        }
    }

    public class ContactsReducer
    {
        [ReducerMethod(typeof(ContactsLoadingAction))]
        public static ContactsState OnLoading(ContactsState state)
        {
            return state with
            {
                IsLoading = true
            };
        }
        [ReducerMethod]
        public static ContactsState OnLoaded(ContactsState state, ContactsSetProfilesAction action)
        {
            return state with
            {
                IsLoading = false,
                Profiles = action.Profiles
            };
        }
    }

    public class ContactsEffects
    {
        private readonly IHttpClientFactory httpClientFactory;

        public ContactsEffects(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        [EffectMethod(typeof(ContactsLoadProfilesAction))]
        public async Task LoadProfiles(IDispatcher dispatcher)
        {
            dispatcher.Dispatch(new ContactsLoadingAction());
            var http = httpClientFactory.CreateClient("ContactsClient");
            var profiles = await http.GetFromJsonAsync<List<Profile>>("contacts/getcontacts");
            dispatcher.Dispatch(new ContactsSetProfilesAction(profiles));
        }

        [EffectMethod]
        public async Task LoadProfilesByNickname(ContactsLoadProfilesByFilterAction action, IDispatcher dispatcher)
        {
            dispatcher.Dispatch(new ContactsLoadingAction());
            var http = httpClientFactory.CreateClient("ContactsClient");
            var profiles = await http.GetFromJsonAsync<List<Profile>>($"contacts/getcontacts/{action.Filter}");
            dispatcher.Dispatch(new ContactsSetProfilesAction(profiles));
        }
    }
}
