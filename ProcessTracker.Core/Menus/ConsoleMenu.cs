using ProcessTracker.Core.Helpers;
using System.Text;

namespace ProcessTracker.Core.Menus
{
    public abstract class ConsoleMenu : MenuItem
    {
        public List<MenuItem> _menuItems = new List<MenuItem>();

        public bool IsActive { get; set; }

        public abstract void CreateMenu();

        public override async Task Select()
        {
            IsActive = true;
            do
            {
                CreateMenu();
                string output = $"{MenuText()}{Environment.NewLine}";
                int selection = ConsoleHelpers.GetIntegerInRange(1, _menuItems.Count, this.ToString()) - 1;
                await _menuItems[selection].Select();
            } while (IsActive);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            //sb.AppendLine(MenuText());
            sb.AppendLine();
            for (int i = 0; i < _menuItems.Count; i++)
            {
                sb.AppendLine($"{i + 1}. {_menuItems[i].MenuText()}");
            }
            return sb.ToString();
        }
    }

    public abstract class MenuItem
    {
        public abstract string MenuText();
        public abstract Task Select();
    }

    class ExitMenuItem : MenuItem
    {
        private ConsoleMenu _menu;

        public ExitMenuItem(ConsoleMenu _parentItem)
        {
            _menu = _parentItem;
        }

        public override string MenuText()
        {
            return "Exit.";
        }

        public override async Task Select()
        {
            Console.Clear();
            _menu.IsActive = false;
        }
    }
}
