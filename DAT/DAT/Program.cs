using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DAT
{
    class Program
    {
        static void Main(string[] args)
        {
            DataTable table = new DataTable();

            // Prompt
            Console.Write("Enter (absolute) file path: ");

            try
            {
                string path = Console.ReadLine();
                string filePath = Path.GetFullPath(path);

                if (!string.IsNullOrEmpty(filePath))
                {
                    #region Get File Values

                    if (File.Exists(filePath))
                    {
                        bool firstIteration = true;

                        //if File row contains Data...
                        foreach (var line in File.ReadLines(filePath, Encoding.UTF8))
                        {
                            if (!string.IsNullOrEmpty(line)) //if row contains data
                            {
                                string[] elements = Regex.Split(line, "\\s+", RegexOptions.None); //get row elements

                                //Get Row Elements
                                if (elements.Length > 0)
                                {
                                    if (firstIteration) //First Row (Headers)
                                    {
                                        foreach (string column in elements)
                                        {
                                            table.Columns.Add(column, typeof(string)); //add column headers
                                        }
                                        firstIteration = false;
                                    }
                                    else //Next Rows (Body)
                                    {
                                        table.Rows.Add(elements); //add rows
                                    }
                                }
                            }
                        }

                        #region Get DataTable Values

                        //Create List that holds 3 Columns (Dy, Mxt, Mnt)
                        List<string[]> listTable = table.AsEnumerable().Select(r => new[] { r.Field<string>("Dy"), r.Field<string>("MxT"), r.Field<string>("MnT") }).ToList();

                        //Retrieve maximum Spread, Day of the Month, Max & Min from ListTable
                        var result = listTable.Select(value => new
                        {
                            day = value[0], //day of the month
                            max = value[1], //max temp
                            min = value[2], //min temp
                            spread = ((int.Parse(Regex.Match(value[1], @"\d+").Value)) - (int.Parse(Regex.Match(value[2], @"\d+").Value))) //spread
                        }).OrderByDescending(value => value.spread).FirstOrDefault();

                        //Output Result
                        Console.WriteLine(result != null ? "Result: " + result.day + " " + result.spread : "No Result");

                        #endregion
                    }
                    else
                    {
                        Console.WriteLine("File Not Found.");
                    }

                    #endregion
                }
                else
                {
                    Console.WriteLine("Invalid File path.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.ReadLine();
        }
    }
}