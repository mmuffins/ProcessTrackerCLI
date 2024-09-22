namespace ProcessTracker.Core.ViewModels
{
    public class GenericResponse
    {
        public GenericResponse()
        {
        }

        public List<TagViewModel> Tags { get; set; }
        public TagViewModel Tag { get; set; }
        public List<FilterViewModel> Filters { get; set; }
        public FilterViewModel Filter { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public int StatusCode { get; set; }
        public List<TagsReportViewModel> Report { get; set; }
        public string SettingValue { get; set; }
    }
}
