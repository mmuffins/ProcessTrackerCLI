using Microsoft.Extensions.Configuration;
using ProcessTracker.Core.Helpers;
using ProcessTracker.Core.Interfaces;
using System.Globalization;
using System.Runtime.InteropServices;

namespace ProcessTracker.Core.Menus
{
    public class ProcessMenu : ConsoleMenu
    {
        private readonly IHttpService _httpRequests;
        private readonly IConfiguration _configuration;

        public ProcessMenu(IHttpService httpRequests, IConfiguration configuration)
        {
            _httpRequests = httpRequests;
            _configuration = configuration;
        }

        private AppSettings AppSettings
        {
            get
            {
                return _configuration.GetSection("AppSettings").Get<AppSettings>();
            }
        }
        public override void CreateMenu()
        {
            // initial menu items and options.
            _menuItems.Clear();
            _menuItems.Add(new TagMenu(_httpRequests, AppSettings));
            _menuItems.Add(new FiltersMenu(_httpRequests));
            _menuItems.Add(new ReportMenuItem(_httpRequests, AppSettings));
            _menuItems.Add(new TrackingMenuItem(_httpRequests));
            _menuItems.Add(new ExitMenuItem(this));
        }

        public override string MenuText()
        {
            return null;
        }
    }

    #region tags
    internal class TagMenu : ConsoleMenu
    {
        private readonly IHttpService _httpRequests;
        private readonly AppSettings _appSettings;

        public TagMenu(IHttpService httpRequests, AppSettings appSettings)
        {
            _httpRequests = httpRequests;
            _appSettings = appSettings;
        }

        public override string MenuText()
        {
            return "Tag.";
        }

        public override void CreateMenu()
        {
            // menu related to the tag
            _menuItems.Clear();
            _menuItems.Add(new DisplayTagMenuItem(_httpRequests));
            _menuItems.Add(new DisplayActiveTagMenuItem(_httpRequests));
            _menuItems.Add(new CreateTagMenuItem(_httpRequests));
            _menuItems.Add(new RemoveTagMenuItem(_httpRequests));
            _menuItems.Add(new EnableDisableTagMenuItem(_httpRequests));
            _menuItems.Add(new AddTagSessionMenuItem(_httpRequests, _appSettings));
            _menuItems.Add(new ExitMenuItem(this));
        }
    }
    internal class DisplayTagMenuItem : MenuItem
    {

        private readonly IHttpService _httpRequests;

        public DisplayTagMenuItem(IHttpService httpRequests)
        {
            _httpRequests = httpRequests;
        }

        public override string MenuText()
        {
            return "Display tags.";
        }

        public override async Task Select()
        {
            // api call to get tags
            var tags = await _httpRequests.GetTags();

            // print table
            if (tags != null)
            {
                ConsoleHelpers.PrintLine();
                ConsoleHelpers.PrintRow("Name", "Disabled");
                ConsoleHelpers.PrintLine();
                foreach (var _item in tags)
                {
                    ConsoleHelpers.PrintRow(_item.Name, _item.Inactive ? "Yes" : "No");
                }
                ConsoleHelpers.PrintLine();
            }
        }
    }
    internal class DisplayActiveTagMenuItem : MenuItem
    {

        private readonly IHttpService _httpRequests;

        public DisplayActiveTagMenuItem(IHttpService httpRequests)
        {
            _httpRequests = httpRequests;
        }

        public override string MenuText()
        {
            return "Display active tags.";
        }

        public override async Task Select()
        {
            // api call to get active tags
            var tags = await _httpRequests.GetActiveTags();

            // print table
            if (tags != null)
            {
                ConsoleHelpers.PrintLine();
                ConsoleHelpers.PrintRow("Name", "Disabled");
                ConsoleHelpers.PrintLine();
                foreach (var _item in tags)
                {
                    ConsoleHelpers.PrintRow(_item.Name, _item.Inactive ? "Yes" : "No");
                }
                ConsoleHelpers.PrintLine();
            }
        }
    }
    internal class CreateTagMenuItem : MenuItem
    {
        private readonly IHttpService _httpRequests;

        public CreateTagMenuItem(IHttpService httpRequests)
        {
            _httpRequests = httpRequests;
        }

        public override string MenuText()
        {
            return "Create a tag.";
        }

        public override async Task Select()
        {
            Console.Write("Enter name of a tag: ");
            // read tag value from the user input
            string name = Console.ReadLine();

            // api call to add tag in the DB
            await _httpRequests.AddTag(name);
        }
    }
    internal class RemoveTagMenuItem : MenuItem
    {
        private readonly IHttpService _httpRequests;

        public RemoveTagMenuItem(IHttpService httpRequests)
        {
            _httpRequests = httpRequests;
        }

        public override string MenuText()
        {
            return "Remove a tag.";
        }

        public override async Task Select()
        {
            Console.Write("Enter name of a tag: ");
            // read user input
            string name = Console.ReadLine();

            // api call to remove tag in the DB
            await _httpRequests.RemoveTag(name);
        }
    }
    internal class EnableDisableTagMenuItem : MenuItem
    {
        private readonly IHttpService _httpRequests;

        public EnableDisableTagMenuItem(IHttpService httpRequests)
        {
            _httpRequests = httpRequests;
        }

        public override string MenuText()
        {
            return "Enable/Disable tag.";
        }

        public override async Task Select()
        {
            Console.Write("Enter name of a tag: ");
            // read user input
            string name = Console.ReadLine();

            // get tag details from the DB via api call
            var tag = await _httpRequests.GetTagByName(name);
            if (tag != null)
            {
                // display yes/no option in the console
                var result = ConsoleHelpers.GetYesOrNoAnswer("Tag is " + (tag.Inactive ? "disabled" : "enabled") + ", do you want to " + (tag.Inactive ? "enable it?" : "disable it?"));
                if (result)
                    // enable or disable tag in the database
                    await _httpRequests.ToggleTag(name);
            }
        }
    }
    internal class AddTagSessionMenuItem : MenuItem
    {
        private readonly IHttpService _httpRequests;
        private readonly AppSettings _appSettings;

        public AddTagSessionMenuItem(IHttpService httpRequests, AppSettings appSettings)
        {
            _httpRequests = httpRequests;
            _appSettings = appSettings;
        }

        public override string MenuText()
        {
            return "Add session.";
        }

        public override async Task Select()
        {
            // get report filters or skip if nothing is typed
            Console.Write("Enter name of a tag: ");
            string tagName = Console.ReadLine();
            var tag = await _httpRequests.GetTagByName(tagName);
            if (tag == null)
            {
                Console.WriteLine("Error: Tag not found.");
                return;
            }

            Console.Write("Enter start date (" + _appSettings.DateTimeFormat + "):  ");
            string startDateStr = Console.ReadLine();
            var isValidDate = DateTime.TryParseExact(startDateStr, _appSettings.DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime startDate);
            if (!isValidDate)
            {
                Console.WriteLine("Error: Invalid datetime format.");
                return;
            }

            Console.Write("Enter end date (" + _appSettings.DateTimeFormat + "): ");
            string endDateStr = Console.ReadLine();
            isValidDate = DateTime.TryParseExact(endDateStr, _appSettings.DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime endDate);
            if (!isValidDate)
            {
                Console.WriteLine("Error: Invalid datetime format.");
                return;
            }

            Console.WriteLine();

            // api call to add session
            await _httpRequests.AddSession(tagName, startDateStr, endDateStr);
        }
    }
    #endregion

    #region filters
    internal class FiltersMenu : ConsoleMenu
    {
        private readonly IHttpService _httpRequests;

        public FiltersMenu(IHttpService httpRequests)
        {
            _httpRequests = httpRequests;
        }

        public override string MenuText()
        {
            return "Filters.";
        }

        public override void CreateMenu()
        {
            // menu related to the filters
            _menuItems.Clear();
            _menuItems.Add(new DisplayFiltersMenuItem(_httpRequests));
            _menuItems.Add(new DisplayActiveFiltersMenuItem(_httpRequests));
            _menuItems.Add(new CreateFilterMenuItem(_httpRequests));
            _menuItems.Add(new RemoveFilterMenuItem(_httpRequests));
            _menuItems.Add(new EnableDisableFilterMenuItem(_httpRequests));
            _menuItems.Add(new ExitMenuItem(this));
        }
    }
    internal class DisplayFiltersMenuItem : MenuItem
    {

        private readonly IHttpService _httpRequests;

        public DisplayFiltersMenuItem(IHttpService httpRequests)
        {
            _httpRequests = httpRequests;
        }

        public override string MenuText()
        {
            return "Display filters.";
        }

        public override async Task Select()
        {
            Console.Write("Enter name of a tag: ");
            // read user input
            var tagName = Console.ReadLine();

            // api call to get all the filters
            var filters = await _httpRequests.GetFilters(tagName);

            // print table
            if (filters != null)
            {
                ConsoleHelpers.PrintLine();
                ConsoleHelpers.PrintRow("Id", "Filter", "Type", "Value", "Disabled");
                ConsoleHelpers.PrintLine();
                foreach (var _item in filters)
                {
                    ConsoleHelpers.PrintRow(_item.Id.ToString(), _item.Filter, _item.Type, _item.Value, _item.Disabled ? "Yes" : "No");
                }
                ConsoleHelpers.PrintLine();
            }
        }
    }
    internal class DisplayActiveFiltersMenuItem : MenuItem
    {

        private readonly IHttpService _httpRequests;

        public DisplayActiveFiltersMenuItem(IHttpService httpRequests)
        {
            _httpRequests = httpRequests;
        }

        public override string MenuText()
        {
            return "Display active filters.";
        }

        public override async Task Select()
        {
            Console.Write("Enter name of a tag: ");
            // read user input
            var tagName = Console.ReadLine();

            // api call to get active filters
            var filters = await _httpRequests.GetActiveFilters(tagName);

            // print table
            if (filters != null)
            {
                ConsoleHelpers.PrintLine();
                ConsoleHelpers.PrintRow("Id", "Filter", "Type", "Value", "Disabled");
                ConsoleHelpers.PrintLine();
                foreach (var _item in filters)
                {
                    ConsoleHelpers.PrintRow(_item.Id.ToString(), _item.Filter, _item.Type, _item.Value, _item.Disabled ? "Yes" : "No");
                }
                ConsoleHelpers.PrintLine();
            }
        }
    }
    internal class CreateFilterMenuItem : MenuItem
    {
        private readonly IHttpService _httpRequests;

        public CreateFilterMenuItem(IHttpService httpRequests)
        {
            _httpRequests = httpRequests;
        }

        public override string MenuText()
        {
            return "Create a filter.";
        }

        public override async Task Select()
        {
            bool addMore = false;
            do
            {
                Console.Write("Enter name of a tag: ");
                // read input from the user
                string tagName = Console.ReadLine();

                // api call to get tag by name
                var tag = await _httpRequests.GetTagByName(tagName);
                if (tag == null)
                {
                    Console.WriteLine("Error: Tag not found.");
                    break;
                }

                int fieldType;
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    // sub menu selection
                    fieldType = ConsoleHelpers.GetSelectionFromStringArray("Select a filter.", new string[] { "Name", "Path", "Exit" });
                    // exit when press 3
                    if (fieldType == 3)
                        break;
                }
                else
                {// sub menu selection
                    fieldType = ConsoleHelpers.GetSelectionFromStringArray("Select a filter.", new string[] { "Name", "Path", "Description", "Main Window Title", "Exit" });
                    // exit when press 5
                    if (fieldType == 5)
                        break;
                }

                // sub menu selection
                int filterType = ConsoleHelpers.GetSelectionFromStringArray("Select type of filter.", new string[] { "Starts with", "Ends with", "Contain", "Exact match", "Exit" });
                // exit when press 5
                if (filterType == 5)
                    break;

                Console.Write("Enter value of a filter: ");
                // read user input
                string value = Console.ReadLine();

                // api call to add filter in the database
                await _httpRequests.AddFilter(tagName, fieldType, filterType, value);

                // yes/no option selection
                addMore = ConsoleHelpers.GetYesOrNoAnswer("Do you want to add more");
            }
            // keep adding until user press N
            while (addMore);
        }
    }
    internal class RemoveFilterMenuItem : MenuItem
    {
        private readonly IHttpService _httpRequests;

        public RemoveFilterMenuItem(IHttpService httpRequests)
        {
            _httpRequests = httpRequests;
        }

        public override string MenuText()
        {
            return "Remove a filter.";
        }

        public override async Task Select()
        {
            var id = ConsoleHelpers.GetInteger("Enter id of a filter: ");

            // api call to remove a filter from the DB
            await _httpRequests.RemoveFilter(id);
        }
    }
    internal class EnableDisableFilterMenuItem : MenuItem
    {
        private readonly IHttpService _httpRequests;

        public EnableDisableFilterMenuItem(IHttpService httpRequests)
        {
            _httpRequests = httpRequests;
        }

        public override string MenuText()
        {
            return "Enable/Disable filter.";
        }

        public override async Task Select()
        {
            // funtion to get integer value from the user input
            var id = ConsoleHelpers.GetInteger("Enter id of a filter: ");

            // get filter from the database
            var filter = await _httpRequests.GetFilterByID(id);
            if (filter != null)
            {
                // yes/no option selection
                var result = ConsoleHelpers.GetYesOrNoAnswer("Filter is " + (filter.Disabled ? "disabled" : "enabled") + ", do you want to " + (filter.Disabled ? "enable it?" : "disable it?"));
                if (result)
                    // api call to enable/disable filter
                    await _httpRequests.ToggleFilter(id);
            }
        }
    }
    #endregion

    #region reports
    //internal class ReportMenuItem : MenuItem
    //{
    //    private readonly IHttpService _httpRequests;
    //    private readonly AppSettings _appSettings;

    //    public ReportMenuItem(IHttpService httpRequests, AppSettings appSettings)
    //    {
    //        _httpRequests = httpRequests;
    //        _appSettings = appSettings;
    //    }

    //    public override string MenuText()
    //    {
    //        return "Report.";
    //    }
    //    public override async Task Select()
    //    {

    //        // get report filters or skip if nothing is typed
    //        Console.Write("Enter name of a tag or press enter to skip: ");
    //        string tagName = Console.ReadLine();

    //        if (!string.IsNullOrEmpty(tagName))
    //        {
    //            var tag = await _httpRequests.GetTagByName(tagName);
    //            if (tag == null)
    //            {
    //                Console.WriteLine("Error: Tag not found.");
    //                return;
    //            }
    //        }

    //        Console.Write("Enter start date (" + _appSettings.DateTimeFormat + ") or press enter to skip: ");
    //        string startDateStr = Console.ReadLine();
    //        if (!string.IsNullOrEmpty(startDateStr))
    //        {
    //            var isValidDate = DateTime.TryParseExact(startDateStr, _appSettings.DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime startDate);
    //            if (!isValidDate)
    //            {
    //                Console.WriteLine("Error: Invalid datetime format.");
    //                return;
    //            }
    //        }

    //        Console.Write("Enter end date (" + _appSettings.DateTimeFormat + ") or press enter to skip: ");
    //        string endDateStr = Console.ReadLine();
    //        if (!string.IsNullOrEmpty(endDateStr))
    //        {
    //            var isValidDate = DateTime.TryParseExact(endDateStr, _appSettings.DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime endDate);
    //            if (!isValidDate)
    //            {
    //                Console.WriteLine("Error: Invalid datetime format.");
    //                return;
    //            }
    //        }

    //        Console.WriteLine();

    //        // api call to get report
    //        var report = await _httpRequests.GetReport(tagName, startDateStr, endDateStr);
    //        if (report != null)
    //        {
    //            // print table
    //            ConsoleHelpers.PrintLine();
    //            ConsoleHelpers.PrintRow("Name", "Total Active Time", "First Occurence", "Last Occurence");
    //            ConsoleHelpers.PrintLine();
    //            foreach (var _item in report)
    //            {
    //                ConsoleHelpers.PrintRow(_item.Name, _item.TotalActiveTime, _item.FirstOccurence, _item.LastOccurence);
    //            }
    //            ConsoleHelpers.PrintLine();
    //        }
    //    }
    //}
    internal class ReportMenuItem : MenuItem
    {
        private readonly IHttpService _httpRequests;
        private readonly AppSettings _appSettings;

        public ReportMenuItem(IHttpService httpRequests, AppSettings appSettings)
        {
            _httpRequests = httpRequests;
            _appSettings = appSettings;
        }

        public override string MenuText()
        {
            return "Report.";
        }
        public override async Task Select()
        {

            // get report filters or skip if nothing is typed
            Console.Write("Enter name of a tag or press enter to skip: ");
            string tagName = Console.ReadLine();

            if (!string.IsNullOrEmpty(tagName))
            {
                var tag = await _httpRequests.GetTagByName(tagName);
                if (tag == null)
                {
                    Console.WriteLine("Error: Tag not found.");
                    return;
                }
            }

            var defaultStartDate = DateTime.Now.AddMonths(-6);
            Console.Write($"Enter start date ({_appSettings.DateFormat}) or press enter to use default [{defaultStartDate.ToString(_appSettings.DateFormat)}]: ");
            string input = Console.ReadLine();
            string startDateStr = string.IsNullOrWhiteSpace(input)
                ? defaultStartDate.ToString(_appSettings.DateFormat)
                : input;

            if (!string.IsNullOrEmpty(startDateStr))
            {
                var isValidDate = DateTime.TryParseExact(startDateStr, _appSettings.DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime startDate);
                if (!isValidDate)
                {
                    Console.WriteLine("Error: Invalid date format.");
                    return;
                }
            }

            Console.Write("Enter end date (" + _appSettings.DateFormat + ") or press enter to skip: ");
            string endDateStr = Console.ReadLine();
            if (!string.IsNullOrEmpty(endDateStr))
            {
                var isValidDate = DateTime.TryParseExact(endDateStr, _appSettings.DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime endDate);
                if (!isValidDate)
                {
                    Console.WriteLine("Error: Invalid date format.");
                    return;
                }
            }

            Console.WriteLine();

            // api call to get report
            var report = await _httpRequests.GetReport(tagName, startDateStr, endDateStr);
            if (report != null)
            {
                // print table
                ConsoleHelpers.PrintLine();
                ConsoleHelpers.PrintRow("Name", "Total Active Time", "First Occurence", "Last Occurence");
                ConsoleHelpers.PrintLine();
                foreach (var _item in report)
                {
                    ConsoleHelpers.PrintRow(_item.Name, _item.TotalActiveTime, _item.FirstOccurence, _item.LastOccurence);
                }
                ConsoleHelpers.PrintLine();
            }
        }
    }
    #endregion

    #region tracking
    internal class TrackingMenuItem : MenuItem
    {
        private readonly IHttpService _httpRequests;
        private string trackingStatus;

        public TrackingMenuItem(IHttpService httpRequests)
        {
            _httpRequests = httpRequests;

            // get current status of tracking to show it in the menu
            var trackingValue = _httpRequests.GetTracking().GetAwaiter().GetResult();
            trackingStatus = trackingValue != null ? (trackingValue.Value ? "paused" : "unpaused") : "unknown";
        }

        public override string MenuText()
        {
            // show current tracking status
            return "Tracking (status: " + trackingStatus + ")";
        }
        public override async Task Select()
        {
            // yes/no option selection
            var result = ConsoleHelpers.GetYesOrNoAnswer("Tracking is " + trackingStatus + ", do you want to " + (trackingStatus == "paused" ? "unpause it?" : "pause it?"));
            if (result)
            {
                // api call to pause/unpause tracking
                var res = await _httpRequests.UpdateTracking(trackingStatus == "paused" ? false : true);
                if (res)
                    // change status in the menu
                    trackingStatus = trackingStatus == "paused" ? "unpaused" : "paused";
            }
        }
    }
    #endregion
}
