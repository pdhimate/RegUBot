using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoIt;
using OpenQA.Selenium;

namespace RegU.V2
{
    public class TestRunner
    {
        private const int DefaultAnswerIndex = 0;
        private const string ReguUrl = "https://regu.fisglobal.com";

        public static Dictionary<string, int> PassTest(string testUrl, string userName, string password, IWebDriver driver)
        {
            driver.Navigate().GoToUrl(ReguUrl);
            if (!Login(userName, password, driver)) return null;

            // Goto test page
            Thread.Sleep(TimeSpan.FromSeconds(2));
            driver.Navigate().GoToUrl(testUrl);

            // Answers cache: 'Question' to 'index of correct radio button' mapping
            var answersMap = new Dictionary<string, int>();

            // Get all questions
            var questions = driver.TryFindElements(".//form[@id='form']/table/tbody/tr/td[@bgcolor='#f5f5f5']");
            foreach (var question in questions)
            {
                answersMap.Add(question.Text, DefaultAnswerIndex);
            }

            // Guess answers
            SetAnswers(testUrl, driver, answersMap);

            return answersMap;
        }

        #region Local helpers

        /// <summary>
        /// Recursive method to guess and set correct answers
        /// </summary>
        /// <param name="testUrl"></param>
        /// <param name="driver"></param>
        /// <param name="answersMap"></param>
        private static void SetAnswers(string testUrl, IWebDriver driver, Dictionary<string, int> answersMap)
        {
            var rows = driver.TryFindElements(".//form[@id='form']/table/tbody/tr");
            int answerToTryIndex = 0;
            int currAnswerIndex = 0;
            bool skipToNextQuestion = false; // used after selecting an answer for the curr question
            foreach (var row in rows)
            {
                IWebElement question = row.TryFindElement("./td[@bgcolor='#f5f5f5']");
                if (question != null)
                {
                    var currQuestion = question.Text;
                    answerToTryIndex = answersMap[currQuestion];

                    // Reset cur ans index since the question has changed
                    currAnswerIndex = 0;
                    skipToNextQuestion = false;
                }

                if (skipToNextQuestion)
                {
                    continue;
                }

                IWebElement radioInput = row.TryFindElement("./td/input[@type='radio']");
                if (radioInput != null)
                {
                    var tryThisRadio = answerToTryIndex == currAnswerIndex;
                    if (tryThisRadio)
                    {
                        radioInput.Click();
                        skipToNextQuestion = true;
                    }
                    else
                    {
                        currAnswerIndex++;
                    }
                }
            }

            // Hit submit
            var continueButton = driver.TryFindElement(".//div[@id='continue']/input");
            continueButton.Click();

            // check results
            var failed = driver.TryFindElement(".//div/b[text()='MISSED QUESTIONS']");
            var retakeTest = failed != null && failed.Displayed;
            if (retakeTest)
            {
                var missedQuestions = driver.TryFindElements(".//table/tbody/tr/td[@bgcolor='#f5f5f5']");
                foreach (var missedQuestion in missedQuestions)
                {
                    string question = missedQuestion.Text;
                    answersMap[question]++; // Set the next answer to be tried
                }

                // Goto test page
                driver.Navigate().GoToUrl(testUrl);

                // Retake the test with the updated answers
                SetAnswers(testUrl, driver, answersMap);
            }
        }

        /// <summary>
        /// Tries to log in.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="driver"></param>
        /// <returns>true if sucessful </returns>
        private static bool Login(string userName, string password, IWebDriver driver)
        {
            // Doesn't work anymore, hence commented
            //// Login directly using url
            ////testUrl = "https://" + userName + ":" + password + "@" + testUrl.TrimStart("https://".ToCharArray());

            // Wait for a while, sometimes the page load is complete but it is frozen
            Thread.Sleep(TimeSpan.FromSeconds(5));

            // Auth window no longer appears hence commented this code
            //// Wait for the authentication window to appear, then send username and password
            //AutoItX.Send(userName);
            //AutoItX.Send("{TAB}");
            //AutoItX.Send(password);
            //// AutoItX.Send("{ENTER}");// not working, try clicking logn button
            //var loginButton = driver.TryFindElement(".//input[@id='regularsubmit']");
            //loginButton.Click();
            //Thread.Sleep(TimeSpan.FromSeconds(8));

            // Login
            var userInput = driver.TryFindElement(".//input[@id='username']");
            userInput.ClearAndSendKeys(userName);
            var passInput = driver.TryFindElement(".//input[@id='password']");
            passInput.ClearAndSendKeys(password);
            passInput.SendKeys(Keys.Enter);

            // Check login error
            var errorDiv = driver.TryFindElement("//div[@id='errorMessage']");
            if (errorDiv != null && errorDiv.Displayed)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("An error occurred:");
                Console.ResetColor();
                Console.WriteLine(errorDiv.Text);
                return false;
            }

            // Wait again, since after login the web driver isn't able to wait until the next page loads, for some reason
            Thread.Sleep(TimeSpan.FromSeconds(8));

            return true;
        }

        #endregion
    }
}
