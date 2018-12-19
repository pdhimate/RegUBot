using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using AutoIt;
using Newtonsoft.Json;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;

namespace RegU.V2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter user name (e number): ");
            string userName = Console.ReadLine();
            string password = ReadMaskedPassword();

            Console.WriteLine("Enter test url: ");
            string testUrl = Console.ReadLine(); // testUrl = @"https://regu.fisglobal.com/cgi/portal.cgi/custom6325600/testlink?test=custom6325600";

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Note: Please logout of any existing sessions.");
            Console.ResetColor();
            var driver = ChooseBrowser();

            TestRunner.PassTest(testUrl, userName, password, driver);

            //// TODO: Save answers in text file. 1) Incorect answer indexes being returned. 2) testId is empty. 3) Format the answers file to be more readable
            //var queryStringParams = HttpUtility.ParseQueryString(testUrl);
            //var testId = queryStringParams["testUrl"];
            //var answersFilePath = $"answers-{testId}.txt";
            //File.WriteAllText(answersFilePath, "Test URL : " + testUrl + Environment.NewLine + Environment.NewLine);
            //File.AppendAllText(answersFilePath, "Answers : " + Environment.NewLine);
            //var answers = TestRunner.PassTest(testUrl, userName, password, driver);
            //var answersText = JsonConvert.SerializeObject(answers);
            //File.AppendAllText(answersFilePath, answersText);

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("You have passed!");
            driver.Close();
            Console.ReadLine();
        }

        private static readonly IReadOnlyDictionary<int, string> Browsers = new Dictionary<int, string>
        {
            { 1, "Chrome" },
            { 2, "Firefox" },
            { 3, "Internet Explorer" },
        };

        private static IWebDriver ChooseBrowser()
        {
            IWebDriver driver;

            ReadBrowsers: Console.WriteLine("Select browser:");
            try
            {
                foreach (var kvp in Browsers)
                {
                    Console.WriteLine($"{kvp.Key}. {kvp.Value}");
                }

                var key = Console.ReadKey();
                Console.WriteLine();

                switch (key.Key)
                {
                    case ConsoleKey.NumPad1:
                    case ConsoleKey.D1:
                        // set proxy
                        var options = new ChromeOptions();
                        // There are no more proxy issues , hence commented this code
                        ////var proxy = new Proxy();
                        ////proxy.Kind = ProxyKind.ProxyAutoConfigure;
                        ////proxy.ProxyAutoConfigUrl = "we1proxy01:8080";
                        ////proxy.IsAutoDetect = true;
                        ////proxy.HttpProxy = "we1proxy01:8080";
                        ////proxy.SslProxy = "we1proxy01:8080";
                        ////options.Proxy = proxy;
                        options.AddArgument("ignore-certificate-errors");
                        driver = new ChromeDriver(options);
                        break;

                    case ConsoleKey.NumPad2:
                    case ConsoleKey.D2:
                        driver = new FirefoxDriver();
                        break;

                    case ConsoleKey.NumPad3:
                    case ConsoleKey.D3:
                        driver = new InternetExplorerDriver();
                        break;

                    default:
                        goto ReadBrowsers;
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Could not open the selected browser. Try another one." + Environment.NewLine + "Error: " + ex.Message);
                Console.ResetColor();
                Debug.WriteLine(ex.Message);
                goto ReadBrowsers;
            }

            return driver;
        }

        private static string ReadMaskedPassword()
        {
            string pass = string.Empty;
            Console.WriteLine("Enter your password: ");
            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    break;
                }
                if (key.Key == ConsoleKey.Backspace)
                {
                    if (pass.Length > 0)
                    {
                        pass = pass.Substring(0, (pass.Length - 1));
                        Console.Write("\b \b");
                    }
                }
                else
                {
                    pass += key.KeyChar;
                    Console.Write("*");
                }
            }

            return pass;
        }

    }
}
