using System.Text;

namespace ProcessTracker.Core.Helpers
{
    public static class ConsoleHelpers
    {
        public static void Pause()
        {
            Console.WriteLine("Press any key to continue.");
            Console.ReadKey();
        }

        public static int GetIntegerInRange(int _min, int _max, string _message)
        {
            if (_min > _max)
            {
                throw new Exception($"Error: Minimum value {_min} cannot be greater than maximum value {_max}");
            }

            int result;

            do
            {
                Console.WriteLine(_message);
                Console.Write($"Please enter a number between {_min} and {_max} inclusive. ");

                string userInput = Console.ReadLine();

                try
                {
                    result = int.Parse(userInput);
                }
                catch
                {
                    Console.WriteLine($"Error: {userInput} is not a number");
                    continue;
                }

                if (result >= _min && result <= _max)
                {
                    //if (checkProjectID && CapDB.Projects.Any(x => x.ProjectID == result))
                    //{
                    //    Console.WriteLine($"Error: {userInput} already exists");
                    //    continue;
                    //}
                    return result;
                }
                Console.WriteLine($"Error: {result} is not between {_min} and {_max} inclusive.");
            } while (true);
        }
        public static decimal GetAmount(string _message)
        {
            decimal result;
            do
            {
                Console.WriteLine(_message);

                string userInput = Console.ReadLine();

                try
                {
                    result = decimal.Parse(userInput);
                }
                catch
                {
                    Console.WriteLine($"Error:  {userInput} is not a valid amount.");
                    continue;
                }
                if (result <= 0)
                {
                    Console.WriteLine("Error: Amount must be greater than 0.");
                    continue;
                }
                return result;
            } while (true);
        }
        public static int GetInteger(string _message)
        {
            int result;
            do
            {
                Console.Write(_message);

                string userInput = Console.ReadLine();

                try
                {
                    result = int.Parse(userInput);
                }
                catch
                {
                    Console.WriteLine($"Error:  {userInput} is not a valid number.");
                    continue;
                }
                if (result <= 0)
                {
                    Console.WriteLine("Error: number must be greater than 0.");
                    continue;
                }
                return result;
            } while (true);
        }
        public static int GetSelectionFromStringArray(string _message, string[] _options)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{_message}{Environment.NewLine}");
            for (int i = 0; i < _options.Length; i++)
            {
                sb.Append($"{(i + 1)}. {_options[i]} {Environment.NewLine}");
            }
            return GetIntegerInRange(1, _options.Length, sb.ToString());
        }

        public static bool GetYesOrNoAnswer(string _message)
        {
            string result = string.Empty;
            do
            {
                Console.WriteLine(Environment.NewLine + _message);
                Console.Write("Please answer Y or N: ");
                result = Console.ReadLine();
                if (result.ToUpper() == "Y" || result.ToUpper() == "N")
                {
                    return result.ToUpper() == "Y";
                }
            } while (true);
        }

        public static void PrintLine()
        {
            Console.WriteLine(new string('-', 90));
        }

        // Function responsible for printing the row of the table
        public static void PrintRow(params string[] _columns)
        {
            int _width = (90 - _columns.Length) / _columns.Length;
            string _row = "|";

            foreach (string _column in _columns)
            {
                _row += AlignCentre(_column, _width) + "|";
            }

            Console.WriteLine(_row);
        }

        // Function responsible for alligning values to center of the table
        public static string AlignCentre(string _text, int _width)
        {
            _text = _text.Length > _width ? _text.Substring(0, _width - 3) + "..." : _text;

            if (string.IsNullOrEmpty(_text))
            {
                return new string(' ', _width);
            }
            else
            {
                return _text.PadRight(_width - (_width - _text.Length) / 2).PadLeft(_width);
            }
        }
    }
}
