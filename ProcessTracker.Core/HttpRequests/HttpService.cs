using Newtonsoft.Json;
using ProcessTracker.Core.Interfaces;
using ProcessTracker.Core.ViewModels;
using RestSharp;

namespace ProcessTracker.Core.HttpRequests
{
    public class HttpService : IHttpService
    {
        private readonly IRestClient _restClient;
        public HttpService(IRestClient restClient)
        {
            _restClient = restClient;
        }

        #region tags
        public async Task<List<TagViewModel>> GetTags()
        {
            var request = new RestRequest("/api/tag", Method.Get);
            var response = await _restClient.ExecuteGetAsync<GenericResponse>(request);
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = JsonConvert.DeserializeObject<GenericResponse>(response.Content);
                if (apiResponse.Success)
                    return apiResponse.Tags;
                else
                    return new List<TagViewModel>();
            }
            else if (response.StatusCode > 0)
                Console.WriteLine(response.StatusCode + " -> " + response.StatusDescription);
            else
                Console.WriteLine("Error: " + response.ErrorMessage);
            return null;
        }
        public async Task<List<TagViewModel>> GetActiveTags()
        {
            var request = new RestRequest("/api/tag/active", Method.Get);
            var response = await _restClient.ExecuteGetAsync<GenericResponse>(request);
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = JsonConvert.DeserializeObject<GenericResponse>(response.Content);
                if (apiResponse.Success)
                    return apiResponse.Tags;
                else
                    return new List<TagViewModel>();
            }
            else if (response.StatusCode > 0)
                Console.WriteLine(response.StatusCode + " -> " + response.StatusDescription);
            else
                Console.WriteLine("Error: " + response.ErrorMessage);
            return null;
        }
        public async Task<TagViewModel> GetTagByName(string tagName)
        {
            var request = new RestRequest("/api/tag", Method.Get);
            request.AddQueryParameter("name", tagName);
            var response = await _restClient.ExecuteGetAsync<GenericResponse>(request);
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = JsonConvert.DeserializeObject<GenericResponse>(response.Content);
                if (apiResponse.Success)
                    return apiResponse.Tag;
            }
            else if (response.StatusCode > 0)
                Console.WriteLine(response.StatusCode + " -> " + response.StatusDescription);
            else
                Console.WriteLine("Error: " + response.ErrorMessage);
            return null;
        }
        public async Task RemoveTag(string tagName)
        {
            var request = new RestRequest("/api/tag", Method.Delete);
            request.AddQueryParameter("name", tagName);
            var response = await _restClient.ExecuteDeleteAsync<GenericResponse>(request);
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = JsonConvert.DeserializeObject<GenericResponse>(response.Content);
                Console.WriteLine(apiResponse.Message);
            }
            else if (response.StatusCode > 0)
                Console.WriteLine(response.StatusCode + " -> " + response.StatusDescription);
            else
                Console.WriteLine("Error: " + response.ErrorMessage);
        }
        public async Task AddTag(string tagName)
        {
            var request = new RestRequest("/api/tag/add", Method.Post);
            request.AddBody(new
            {
                name = tagName
            });
            var response = await _restClient.ExecutePostAsync<GenericResponse>(request);
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = JsonConvert.DeserializeObject<GenericResponse>(response.Content);
                Console.WriteLine(apiResponse.Message);
            }
            else if (response.StatusCode > 0)
                Console.WriteLine(response.StatusCode + " -> " + response.StatusDescription);
            else
                Console.WriteLine("Error: " + response.ErrorMessage);
        }
        public async Task ToggleTag(string tagName)
        {
            var request = new RestRequest("/api/tag/toggleactive", Method.Put);
            request.AddBody(new
            {
                name = tagName
            });
            var response = await _restClient.ExecutePutAsync<GenericResponse>(request);
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = JsonConvert.DeserializeObject<GenericResponse>(response.Content);
                Console.WriteLine(apiResponse.Message);
            }
            else if (response.StatusCode > 0)
                Console.WriteLine(response.StatusCode + " -> " + response.StatusDescription);
            else
                Console.WriteLine("Error: " + response.ErrorMessage);
        }
        public async Task AddSession(string tagName, string startDate, string endDate)
        {
            var request = new RestRequest("/api/session/add", Method.Post);
            request.AddBody(new
            {
                TagName = tagName,
                StartDate = startDate,
                EndDate = endDate
            });
            var response = await _restClient.ExecutePostAsync<GenericResponse>(request);
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = JsonConvert.DeserializeObject<GenericResponse>(response.Content);
                Console.WriteLine(apiResponse.Message);
            }
            else if (response.StatusCode > 0)
                Console.WriteLine(response.StatusCode + " -> " + response.StatusDescription);
            else
                Console.WriteLine("Error: " + response.ErrorMessage);
        }
        #endregion

        #region filters
        public async Task<List<FilterViewModel>> GetFilters(string tagName)
        {
            var request = new RestRequest("/api/filter", Method.Get);
            request.AddQueryParameter("name", tagName);
            var response = await _restClient.ExecuteGetAsync<GenericResponse>(request);
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = JsonConvert.DeserializeObject<GenericResponse>(response.Content);
                if (apiResponse.Success)
                    return apiResponse.Filters;
                else
                    return new List<FilterViewModel>();
            }
            else if (response.StatusCode > 0)
                Console.WriteLine(response.StatusCode + " -> " + response.StatusDescription);
            else
                Console.WriteLine("Error: " + response.ErrorMessage);
            return null;
        }
        public async Task<List<FilterViewModel>> GetActiveFilters(string tagName)
        {
            var request = new RestRequest("/api/filter/active", Method.Get);
            request.AddQueryParameter("name", tagName);
            var response = await _restClient.ExecuteGetAsync<GenericResponse>(request);
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = JsonConvert.DeserializeObject<GenericResponse>(response.Content);
                if (apiResponse.Success)
                    return apiResponse.Filters;
                else
                    return new List<FilterViewModel>();
            }
            else if (response.StatusCode > 0)
                Console.WriteLine(response.StatusCode + " -> " + response.StatusDescription);
            else
                Console.WriteLine("Error: " + response.ErrorMessage);
            return null;
        }
        public async Task<FilterViewModel> GetFilterByID(int filterID)
        {
            var request = new RestRequest("/api/filter", Method.Get);
            request.AddQueryParameter("id", filterID);
            var response = await _restClient.ExecuteGetAsync<GenericResponse>(request);
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = JsonConvert.DeserializeObject<GenericResponse>(response.Content);
                if (apiResponse.Success)
                    return apiResponse.Filter;
            }
            else if (response.StatusCode > 0)
                Console.WriteLine(response.StatusCode + " -> " + response.StatusDescription);
            else
                Console.WriteLine("Error: " + response.ErrorMessage);
            return null;
        }

        public async Task RemoveFilter(int filterID)
        {
            var request = new RestRequest("/api/filter", Method.Delete);
            request.AddQueryParameter("id", filterID);
            var response = await _restClient.ExecuteDeleteAsync<GenericResponse>(request);
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = JsonConvert.DeserializeObject<GenericResponse>(response.Content);
                Console.WriteLine(apiResponse.Message);
            }
            else if (response.StatusCode > 0)
                Console.WriteLine(response.StatusCode + " -> " + response.StatusDescription);
            else
                Console.WriteLine("Error: " + response.ErrorMessage);
        }
        public async Task AddFilter(string tagName, int fieldType, int filterType, string value)
        {
            var request = new RestRequest("/api/filter/add", Method.Post);
            request.AddBody(new
            {
                TagName = tagName,
                FieldType = fieldType,
                FilterType = filterType,
                Value = value
            });
            var response = await _restClient.ExecutePostAsync<GenericResponse>(request);
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = JsonConvert.DeserializeObject<GenericResponse>(response.Content);
                Console.WriteLine(apiResponse.Message);
            }
            else if (response.StatusCode > 0)
                Console.WriteLine(response.StatusCode + " -> " + response.StatusDescription);
            else
                Console.WriteLine("Error: " + response.ErrorMessage);
        }
        public async Task ToggleFilter(int filterID)
        {
            var request = new RestRequest("/api/filter/toggleactive", Method.Put);
            request.AddBody(new
            {
                FilterID = filterID
            });
            var response = await _restClient.ExecutePutAsync<GenericResponse>(request);
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = JsonConvert.DeserializeObject<GenericResponse>(response.Content);
                Console.WriteLine(apiResponse.Message);
            }
            else if (response.StatusCode > 0)
                Console.WriteLine(response.StatusCode + " -> " + response.StatusDescription);
            else
                Console.WriteLine("Error: " + response.ErrorMessage);
        }
        public async Task<List<TagsReportViewModel>> GetReport(string tagName, string startDate, string endDate)
        {
            var request = new RestRequest("/api/report", Method.Post);
            request.AddBody(new
            {
                TagName = tagName,
                StartDate = startDate,
                EndDate = endDate
            });
            var response = await _restClient.ExecutePostAsync<GenericResponse>(request);
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = JsonConvert.DeserializeObject<GenericResponse>(response.Content);
                if (apiResponse.Success)
                    return apiResponse.Report;
                else
                    return new List<TagsReportViewModel>();
            }
            else if (response.StatusCode > 0)
                Console.WriteLine(response.StatusCode + " -> " + response.StatusDescription);
            else
                Console.WriteLine("Error: " + response.ErrorMessage);
            return null;
        }
        public async Task<bool?> GetTracking()
        {
            var request = new RestRequest("/api/tracking", Method.Get);
            var response = await _restClient.ExecuteGetAsync<GenericResponse>(request);
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = JsonConvert.DeserializeObject<GenericResponse>(response.Content);
                if (apiResponse.Success)
                    return bool.Parse(apiResponse.SettingValue);
                else
                    Console.WriteLine("Error: Invalid value returned from the server.");
            }
            else if (response.StatusCode > 0)
                Console.WriteLine(response.StatusCode + " -> " + response.StatusDescription);
            else
                Console.WriteLine("Error: " + response.ErrorMessage);
            return null;
        }
        public async Task<bool> UpdateTracking(bool pauseTracking)
        {
            var request = new RestRequest("/api/tracking", Method.Put);
            request.AddQueryParameter("value", pauseTracking.ToString());
            var response = await _restClient.ExecutePutAsync<GenericResponse>(request);
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = JsonConvert.DeserializeObject<GenericResponse>(response.Content);
                if (apiResponse.Success)
                {
                    Console.WriteLine("Tracking " + (pauseTracking ? "paused" : "unpaused") + " successfully.");
                    return true;
                }
                else
                    Console.WriteLine(apiResponse.Message);
            }
            else if (response.StatusCode > 0)
                Console.WriteLine(response.StatusCode + " -> " + response.StatusDescription);
            else
                Console.WriteLine("Error: " + response.ErrorMessage);
            return false;
        }
        #endregion
    }
}
