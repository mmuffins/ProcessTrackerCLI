using ProcessTracker.Core.ViewModels;

namespace ProcessTracker.Core.Interfaces
{
    public interface IHttpService
    {
        #region tags
        Task<List<TagViewModel>> GetTags();
        Task<List<TagViewModel>> GetActiveTags();
        Task<TagViewModel> GetTagByName(string tagName);
        Task RemoveTag(string tagName);
        Task AddTag(string tagName);
        Task ToggleTag(string tagName);
        Task AddSession(string tagName, string startDate, string endDate);
        #endregion

        #region filters
        Task<List<FilterViewModel>> GetFilters(string tagName);
        Task<List<FilterViewModel>> GetActiveFilters(string tagName);
        Task<FilterViewModel> GetFilterByID(int filterID);
        Task RemoveFilter(int filterID);
        Task AddFilter(string tagName, int fieldType, int filterType, string value);
        Task ToggleFilter(int filterID);
        #endregion

        #region report
        Task<List<TagsReportViewModel>> GetReport(string tagName, string startDate, string endDate);
        #endregion

        #region tracking
        Task<bool?> GetTracking();
        Task<bool> UpdateTracking(bool pauseTracking);
        #endregion

    }
}
