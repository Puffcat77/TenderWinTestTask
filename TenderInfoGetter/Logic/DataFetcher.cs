using TenderInfoGetter.Configuration;
using TenderInfoGetter.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace TenderInfoGetter.Logic
{
    public static class DataFetcher
    {
        private static readonly HttpClient httpClient = new HttpClient();
        public static async Task<List<TenderDocument>> GetTenderDocumentationJson(int id)
        {
            try
            {
                return JsonSerializer.Deserialize<List<TenderDocument>>(await GetResponseFromUri(URLPaths.GetTenderDocumentationURL(id))); ;
            }
            catch (Exception ex) { throw FormErrorMessage("документации", ex.Message); }
        }

        public static async Task<string> GetTenderNotificationHtml(int id)
        {
            try
            {
                return await GetResponseFromUri(URLPaths.GetTenderNotification(id));
            }
            catch (Exception ex) { throw FormErrorMessage("извещения", ex.Message); }
        }

        public static async Task<List<TenderModel>> PostTendersById(int id)
        {
            List<TenderModel> models;
            var request = new Dictionary<string, string> { { "page", "1" }, { "itemsPerPage", "10" }, { "Id", $"{id}" } };
            var response = await httpClient.PostAsync(URLPaths.GetTendersByPeriodURL(), new FormUrlEncodedContent(request));
            var reponseContent = await response.Content.ReadAsStringAsync();
            if (((int)response.StatusCode) != 200)
                throw new Exception(reponseContent);
            try
            {
                models = JsonSerializer.Deserialize<TenderListModel>(reponseContent).Invdata;
                foreach (var model in models)
                    model.ConvertTimeToLocal(Offsets.NovosibirskOffset);
            }
            catch (Exception ex) { throw ex; }
            return models;
        }

        private static Exception FormErrorMessage(string fetchingData, string message)
        {
            throw new Exception($"Произошла ошибка при получении {fetchingData}:\n{message}"); ;
        }

        private static async Task<string> GetResponseFromUri(Uri uri)
        {
            HttpResponseMessage response = await httpClient.GetAsync(uri);
            string responseResult = await response.Content.ReadAsStringAsync();
            if (((int)response.StatusCode) != 200)
                throw new Exception(responseResult);
            return responseResult;
        }
    }
}
