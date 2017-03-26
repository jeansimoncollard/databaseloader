using DatabaseLoader;
using DatabaseLoader.MsSql;
using System;

namespace DatabaseLoaderStartupProject
{
    class Program
    {
        static void Main(string[] args)
        {
            var DatabaseLoader = new MsSqlLoader();
            DatabaseLoader.Unload("Server=jean-simon-pc;database=AdventureWorks2014;User Id=jeansimon; Password=password;", @"F:\Programmation\dataLoader\fileExamples\gender=m.dataload");
            DatabaseLoader.Load("Server=jean-simon-pc;database=AdventureWorks2014;User Id=jeansimon; Password=password;", @"F:\Programmation\dataLoader\fileExamples\gender=m.dataload", true);
            Console.Read();
            //DatabaseLoader.Unload("Server=jean-simon-pc;database=AdventureWorks2014;User Id=jeansimon; Password=password;", @"F:\Programmation\dataLoader\fileExamples\gender=m.dataload");
        }
    }
}
