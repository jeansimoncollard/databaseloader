using DatabaseLoader;
using System;

namespace DatabaseLoaderStartupProject
{
    class Program
    {
        static void Main(string[] args)
        {
            IOracleLoader DatabaseLoader = new OracleLoader();
            DatabaseLoader.Unload("user id=hr; password=password; data source=206.167.194.154/XE;", @"F:\Programmation\dataLoader\fileExamples\Document 1.dataload");
            DatabaseLoader.Load("user id=hr; password=password; data source=206.167.194.154/XE;", @"F:\Programmation\dataLoader\fileExamples\Document 1.dataload", false);
            Console.Read();

            DatabaseLoader.Unload("user id=hr; password=password; data source=206.167.194.154/XE;", @"F:\Programmation\dataLoader\fileExamples\Document 1.dataload");
            DatabaseLoader.BulkLoad("user id=hr; password=password; data source=206.167.194.154/XE;", @"F:\Programmation\dataLoader\fileExamples\Document 1.dataload", false);
            Console.Read();

            DatabaseLoader.Unload("user id=hr; password=password; data source=206.167.194.154/XE;", @"F:\Programmation\dataLoader\fileExamples\Document 1.dataload");
            DatabaseLoader.ConventionalLoad("user id=hr; password=password; data source=206.167.194.154/XE;", @"F:\Programmation\dataLoader\fileExamples\Document 1.dataload", false);
            Console.Read();
        }
    }
}
