using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Standard.MasterDataCustomization.Model;
using MasterDataCustomization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Web;

namespace Standard.MasterDataCustomization
{
    public class CustomizationHelper
    {
        public readonly AuthenticatedClient Client;
        public readonly CustomizationContext Context;
        private readonly Action<string> onFeedback;

        public CustomizationHelper(AuthenticatedClient client, CustomizationContext ctx, Action<string> onFeedback)
        {
            Client = client;
            Context = ctx;
            this.onFeedback = onFeedback;
        }

        public void SendFeedback(string message)
        {
            onFeedback?.Invoke(message);
        }

        public bool WhatIf => Context.WhatIf;

        public async Task<Site> GetSite(Source src)
        {
            var uri = $"{Context.OpisenseApiUrl}sites?displayLevel=VerboseSite&siteIds={src.SiteId}";
            return (await Client.SendAndGetResponseAsync<Site[]>(uri, HttpMethod.Get)).SingleOrDefault();
        }

        public async Task<IEnumerable<Site>> GetSites(IEnumerable<Source> sources)
        {
            var sourceList = string.Join("&", sources.Select(s => $"sourceIds={s.Id}"));
            var uri = $"{Context.OpisenseApiUrl}sites?displayLevel=VerboseSite&{sourceList}";
            return await Client.SendAndGetResponseAsync<Site[]>(uri, HttpMethod.Get);
        }

        public async Task<Account> GetAccount(int accountId)
        {
            var uri = $"{Context.OpisenseApiUrl}accounts/{accountId}";
            return await Client.SendAndGetResponseAsync<Account>(uri, HttpMethod.Get);
        }

        public async Task<IEnumerable<Source>> GetSources(IEnumerable<int> sourceIds)
        {
            var sourceList = string.Join("&", sourceIds.Select(s => $"ids={s}"));
            var uri = $"{Context.OpisenseApiUrl}sources?displayLevel=Site&{sourceList}";
            return await Client.SendAndGetResponseAsync<Source[]>(uri, HttpMethod.Get);
        }

        public async Task<Source> GetSourceFromEan(int siteId, string eanNumber)
        {
            var uri = $"{Context.OpisenseApiUrl}sources?displayLevel=Site&siteId={siteId}&customFilter={HttpUtility.UrlEncode($"EanNumber={eanNumber}")}";
            return (await Client.SendAndGetResponseAsync<Source[]>(uri, HttpMethod.Get)).FirstOrDefault();
        }

        public async Task<Variable[]> GetVariables(int sourceId)
        {
            var uri = $"{Context.OpisenseApiUrl}variables/source/{sourceId}";
            return await Client.SendAndGetResponseAsync<Variable[]>(uri, HttpMethod.Get);
        }

        public async Task<IEnumerable<int>> GetSourceIdsWithoutVariables(IList<int> sourceIds)
        {
            var sourceList = string.Join("&", sourceIds.Select(s => $"sourceIds={s}"));
            var variables = await Client.SendAndGetResponseAsync<Variable[]>($"{Context.OpisenseApiUrl}variables?displayLevel=Normal&{sourceList}", HttpMethod.Get);
            return sourceIds.Except(variables.Select(v => v.SourceId));
        }

        public async Task<int> CreateVariable(Variable variable)
        {
            var uri = $"{Context.OpisenseApiUrl}variables/source/{variable.SourceId}";
            SendFeedback($"  => Creating variable <{variable.Name}> on <{uri}>: {JsonConvert.SerializeObject(variable)}");
            if (WhatIf)
            {
                return -1;
            }
            var jsonResponse = await Client.SendAndGetResponseAsync<string>(uri, HttpMethod.Post, variable);

            var idStr = StringValueOf(JObject.Parse(jsonResponse), "id");

            if (idStr == null)
                throw new Exception($"CreateVariable on source Id<{variable.SourceId}> returned '{jsonResponse}' which is not a valid variable representation");
            if (!Int32.TryParse(idStr, out var id))
            {
                throw new Exception($"CreateVariable on source Id<{variable.SourceId}> returned '{jsonResponse}' which contains an Id that is not convertible to an int");
            }
            SendFeedback($"  => New variable Id = {id}");
            return id;
        }

        public async Task UpdateVariable(Variable variable)
        {
            var uri = $"{Context.OpisenseApiUrl}sources/{variable.SourceId}/variables/{variable.Id}";
            SendFeedback($"  => Updating variable <{variable.Id}/{variable.Name}> on <{uri}>: {JsonConvert.SerializeObject(variable)}");
            if (Context.WhatIf)
                return;

            await Client.SendAsync(uri, HttpMethod.Put, variable);
        }

        public string StringValueOf(JObject jobject, params string[] properties)
        {
            while (true)
            {
                var property = properties[0];

                if (jobject is null)
                    return null;

                var jsonProperty = jobject.Properties().ToList().FirstOrDefault(t => t.Name.Equals(property, StringComparison.InvariantCultureIgnoreCase))?.Value;
                if (jsonProperty is null)
                    return null;

                if (properties.Length == 1) return jsonProperty.ToString();
                jobject = jsonProperty as JObject;
                properties = properties.Skip(1).ToArray();
            }
        }

        public List<string> ArrayValueOf(JObject jobject, string propertyName)
        {
            try
            {
                if (jobject is null)
                    return new List<string>();

                var jsonValues = jobject.Properties().ToList().FirstOrDefault(t => t.Name.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase))?.Value;
                if (jsonValues == null)
                    return new List<string>();

                var values = jsonValues.ToObject<List<string>>();
                return values;
            }
            catch
            {
                return new List<string>();
            }
        }

        public T EnsureValid<T>(T obj, Predicate<T> pred, string errorMessage)
        {
            if (!pred(obj))
            {
                throw new Exception(errorMessage);
            }
            return obj;
        }

        public IEnumerable<IList<T>> Split<T>(IList<T> list, int size)
        {
            for (var i = 0; i < (float)list.Count / size; i++)
            {
                yield return list.Skip(i * size).Take(size).ToList();
            }
        }

        public int VariableIdCounter;
        public int NextVariableId()
        {
            return Interlocked.Decrement(ref VariableIdCounter);
        }
    }
}
