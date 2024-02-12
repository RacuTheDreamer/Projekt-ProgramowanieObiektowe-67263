using System.Collections; // packreference w app.csproject są potrzebne do odczytania jsona i polaczenia z baza danych
                          // non update jest potrzebny zeby nie wyszukiwało plików .json w folderze bin
using Microsoft.Extensions.Configuration;

namespace DbProject
{
    class Program
    {
        static async Task Main()
        {
            IConfiguration configuration = new ConfigurationBuilder()
              .AddJsonFile("appsettings.json", optional: false)
              .Build();
 
             
            string connectionString = configuration.GetConnectionString("MyDatabase");
            Console.WriteLine(connectionString);

            string userInput;

            Console.WriteLine("Welcome to Db tableHandler Program");
            Console.WriteLine("First create you need to create a table");
            string tableName;
            string tableFields;

            do
            {
                Console.WriteLine("Please provide a table name (or exit the program using [S]):");
                tableName = Console.ReadLine();



            } while (!validateInput(tableName));

            do
            {
                Console.WriteLine("define your table collumn names and types as you would in your db example: name VARCHAR(255), lastName VARCHAR(255)");
                tableFields = Console.ReadLine();



            } while (!validateInput(tableFields));

            var table = new Table(tableName, tableFields, connectionString);
            await table.CreateTableInDbAsync();

            do
            {
                Console.WriteLine("here are your options:");
                Console.WriteLine("#1[C]reate table, requries you to give me table name and its fields with its types like you would in your db in that order");
                Console.WriteLine("#2[D]elete table i need a name for this one or i will delete the last one [C]reated");
                Console.WriteLine("#3[Q]ustom querry provide me querry string ");
                Console.WriteLine("#4[S]top the program");

                userInput = Console.ReadLine();

                switch (userInput.ToUpper())
                {
                    case "C":
                        Console.WriteLine("whats your table name?");
                        tableName = Console.ReadLine();

                        while (string.IsNullOrWhiteSpace(tableName) && tableName.ToLower() != "s")
                        {
                            Console.WriteLine("please provide proper name or stop using [S]");
                            tableName = Console.ReadLine();
                        }

                        Console.WriteLine("define your table collumn names and types as you would in your db example: name VARCHAR(255), lastName VARCHAR(255)");
                        tableFields = Console.ReadLine();

                        while (string.IsNullOrWhiteSpace(tableFields) && tableFields.ToLower() != "s")
                        {
                            Console.WriteLine("please provide proper column names or exit using [S]");
                            tableFields = Console.ReadLine();
                        }
                        table = new Table(tableName, tableFields, connectionString);
                        await table.CreateTableInDbAsync();
                        break;


                    case "D":
                        Console.WriteLine("provide table name otherwise last one [C]reated will be deleted");
                        tableName = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(tableName))
                        {
                            await table.DeleteTableInDbAsync();
                        }
                        else
                        {
                            await table.DeleteTableInDbAsync(tableName);
                        }
                        break;


                    case "Q":
                        Console.WriteLine("provide custom querry as you would in your database");
                        var querry = Console.ReadLine();
                        while (string.IsNullOrWhiteSpace(querry) && querry.ToLower() != "s")
                        {
                            Console.WriteLine("please provide querry or use [S]");
                            querry = Console.ReadLine();
                        }
                        await table.ExecuteCustomQueryAsync(querry);
                        break;


                    case "S":
                        break;

                    default:
                        Console.WriteLine("Unrecognized option. Please try again.");
                        break;
                }

                if (userInput.ToLower() == "s")
                {
                    Console.WriteLine("Stopping the program.");
                    break;
                }

            } while (true);
        
        }
        public static bool validateInput(string inputValue)
        {

                if (string.IsNullOrWhiteSpace(inputValue))
                {
                    Console.WriteLine("Please try again or use [Ctrl + C] if you're trying to exit.");
                    return false;
                }
                if (inputValue.ToLower() == "s")
                {
                    Console.WriteLine("Exiting the program.");
                    return true;
                }
                if (int.TryParse(inputValue, out _))
                {
                    return false;
                }
                    Console.WriteLine("Valid input for table fields.");
                    return true;


        }
    }

}
