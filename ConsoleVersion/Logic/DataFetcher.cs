using ConsoleVersion.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ConsoleVersion.Logic
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
            catch (Exception ex)
            {
                FormErrorMessage("документации", ex.Message);
                return new List<TenderDocument>();
            }
        }

        public static async Task<string> GetTenderNotificationHtml(int id)
        {
            try
            {
                return await GetResponseFromUri(URLPaths.GetTenderNotification(id));
            }
            catch (Exception ex) 
            {
                FormErrorMessage("извещения", ex.Message);
                return "";
            }
        }

        public static async Task<List<TenderModel>> PostTendersById(int id)
        {
            List<TenderModel> models = new List<TenderModel>();
            var request = new Dictionary<string, string> { { "page", "1" }, { "itemsPerPage", "10" }, { "Id", $"{id}" } };
            var response = await httpClient.PostAsync(URLPaths.GetTendersByPeriodURL(), new FormUrlEncodedContent(request));
            var reponseContent = await response.Content.ReadAsStringAsync();
            if (((int)response.StatusCode) != 200)
            {
                UI.PrintError(reponseContent);
            }
            else
                try
                {
                    models = JsonSerializer.Deserialize<TenderListModel>(reponseContent).Invdata;
                    foreach (var model in models)
                        model.ConvertTimeToLocal(Offsets.NovosibirskOffset);
                }
                catch (Exception ex) { UI.PrintError(ex.Message); }
            return models;
        }

        private static void FormErrorMessage(string fetchingData, string message)
        {
            UI.PrintError($"Произошла ошибка при получении {fetchingData}:\n{message}"); ;
        }

        private static async Task<string> GetResponseFromUri(Uri uri)
        {
            HttpResponseMessage response = await httpClient.GetAsync(uri);
            string responseResult = await response.Content.ReadAsStringAsync();
            if (((int)response.StatusCode) != 200)
            {
                UI.PrintError(responseResult);
                return "";
            }
            return responseResult;
        }
    }
}
