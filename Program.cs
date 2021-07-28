using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;

namespace OneDimensionalCellularAutomata
{
    class Program
    {
        public static Dictionary<int, int> rules = new Dictionary<int, int>();
        
        static void Error(string errorMessage) {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("ERROR: " + errorMessage);
            Console.ResetColor();

            Environment.Exit(-1);
        }

        static void Main(string[] args)
        {
            if (args.Length != 3) Error("wrong usage");

            string rulesPath = args[0];
            int colCount = 0;
            int rowCount = 0;

            if (!File.Exists(rulesPath)) Error($"{rulesPath} is not a valid text file path");
            if (!int.TryParse(args[1], out colCount) || !int.TryParse(args[2], out rowCount)) Error($"can't parse {args[1]} and/or {args[2]}. 2nd and 3rd args should be integers");

            try {
                string[] lines = File.ReadAllLines(rulesPath);
                for (int i = 0; i < lines.Length; i++) {
                    string[] rule_res = lines[i].Split(",");

                    int rule = int.Parse(rule_res[0]);
                    int res = int.Parse(rule_res[1]);

                    rules.Add(rule, res);
                }
            }
            catch (Exception) {
                Error("unaccurate rules file format");
            }

            Bitmap bitmap = new Bitmap(colCount, rowCount);
            int[] row = RandomRow(colCount);
            for(int i = 0; i < rowCount; i++) {
                //PrintRow(row);
                DrawRow(ref bitmap, row, i);
                row = NextRow(row);
            }

            const string name = "\\result";
            string path = Environment.CurrentDirectory + name + ".png";

            for(int i = 1; File.Exists(path); i++) {
                path = Environment.CurrentDirectory + name + "_" + i.ToString() + ".png";
            }
            bitmap.Save(path);
        }

        static void DrawRow(ref Bitmap bitmap, int[] row, int rowNo) {
            Color colorI = Color.Black;
            Color colorO = Color.White;

            for(int i = 0; i < row.Length; i++) {
                Color color = row[i] == 1 ? colorI : colorO;
                bitmap.SetPixel(i, rowNo, color);
            }
        }

        /*
        static void PrintRow(int[] row) {
            for(int i = 0; i < row.Length; i++) {
                Console.Write(row[i] == 1 ? '█' : ' ');
            }
            Console.Write('\n');
        }
        */
        
        static int[] RandomRow(int length) {
            Random rand = new Random();
            int[] row = new int[length];

            for (int i = 0; i < length; i++) {
                row[i] = rand.Next(2);
            }

            return row;
        }

        static int[] NextRow(int[] prevRow) {
            int len = prevRow.Length;
            int[] nextRow = new int[len];

            for (int i = 0; i < len; i++) {
                int key =
                    prevRow[(i - 1 + len) % len] * 100 +
                    prevRow[i] * 10 +
                    prevRow[(i + 1 + len) % len];
                nextRow[i] = rules[key];
            }
            return nextRow;
        }
    }
}