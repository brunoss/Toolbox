using System;
using System.Collections.Generic;
using System.IO;

namespace ToolBox
{
    public class Scanner
    {
        private readonly StreamReader _reader;
        private readonly StreamWriter _writer;
        public Scanner(StreamReader reader, StreamWriter writer)
        {
            _reader = reader;
            _writer = writer;
        }

        public int ReadInt(string message)
        {
            int num;
            string input;
            do
            {
                input = ReadString(message);
            }while(!int.TryParse(input, out num));
            return num;
        }

        public string ReadString(string message)
        {
            _writer.WriteLine(message);
            return _reader.ReadLine();
        }

        public bool ReadBoolean(string message)
        {
            var options = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "y", "yes", "n", "no"
            };
            string value;
            do
            {
                value = ReadString(message + " y (yes)/n (no)?");
            } while (!options.Contains(value));
            return value.StartsWith("y");
        }
        
        public int ReadOption(params string[] options)
        {
            for (int i = 0; i < options.Length; ++i)
            {
                _writer.WriteLine("{0}. {1}", i + 1, options[i]);
            }
            return ReadInt($"Please choose an option between 1 and {options.Length}");
        }
    }
}
