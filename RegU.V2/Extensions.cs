using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace RegU.V2
{
    public static class Extensions
    {
        public static IWebElement TryFindElement(this IWebDriver driver, string xpath)
        {
            IWebElement element = null;
            try
            {
                element = driver.FindElement(By.XPath(xpath));
            }
            catch
            {
                // ignored
            }

            return element;
        }
        public static ReadOnlyCollection<IWebElement> TryFindElements(this IWebDriver driver, string xpath)
        {
            ReadOnlyCollection<IWebElement> elements = null;
            try
            {
                elements = driver.FindElements(By.XPath(xpath));
            }
            catch
            {
                // ignored
            }

            return elements;
        }

        public static IWebElement TryFindElement(this IWebElement driver, string xpath)
        {
            IWebElement element = null;
            try
            {
                element = driver.FindElement(By.XPath(xpath));
            }
            catch
            {
                // ignored
            }

            return element;
        }

        public static ReadOnlyCollection<IWebElement> TryFindElements(this IWebElement driver, string xpath)
        {
            ReadOnlyCollection<IWebElement> elements = null;
            try
            {
                elements = driver.FindElements(By.XPath(xpath));
            }
            catch
            {
                // ignored
            }

            return elements;
        }

        public static void ClearAndSendKeys(this IWebElement element, string text)
        {
            try
            {
                element.Clear();
                element.SendKeys(text);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error occured: " + e.Message);
            }
        }
    }
}
