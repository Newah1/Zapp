using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Zapp.Models.PPL;

namespace Zapp.Services;

public class PPLService : IPPLService
{
    private ChromeDriver _chromeDriver;
    private string _cookieString;
    private PPLCredentials _pplCredentials;

    public PPLService(PPLCredentials pplCredentials)
    {
        _pplCredentials = pplCredentials;
    }

    public string GetRecaptchaToken()
    {
        _chromeDriver = new ChromeDriver();
        // try maximum of 5 times.
        var loginWorked = false;
        for (var i = 0; i < 5; i++)
        {
            try
            {
                TryLogin();

                WaitForLogin();
                loginWorked = true;
                // if login didn't throw, it worked...
                break;
            }
            catch (Exception e)
            {
                _chromeDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);
                Console.WriteLine(e.Message);
                continue;
            }
        }

        if (!loginWorked)
        {
            return string.Empty;
        }

        // get cookies for auth
        var cookie = _chromeDriver.Manage().Cookies.GetCookieNamed("Authorization");

        var cookieValue = cookie.Value;
        Console.WriteLine(cookie.Value);
        _chromeDriver.Close();
        _cookieString = cookieValue;
        return cookieValue;
    }

    public async Task<Zapp.Models.PPL.BillToDateModel> GetBillToDate()
    {
        var response = new BillToDateModel();

        if (string.IsNullOrEmpty(_cookieString))
        {
            return response;
        }
        using HttpClient httpClient = new HttpClient();

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            _cookieString
        );
        var responseHttp = await httpClient.GetAsync(
            "https://api.pplcloud.net/selfserve/api/account/6716108115/bill-to-date"
        );

        if (responseHttp.IsSuccessStatusCode)
        {
            var billToDateModel = await responseHttp.Content.ReadFromJsonAsync<BillToDateModel>();
            return billToDateModel;
        }

        return response;
    }

    private void TryLogin()
    {
        _chromeDriver.Navigate().GoToUrl("https://selfserve.pplelectric.com/sign-in");
        var emailInput = _chromeDriver.FindElement(By.Id("Email or Username"));
        emailInput.SendKeys(_pplCredentials.PPLUsername);

        var passInput = _chromeDriver.FindElement(By.Id("Password"));
        passInput.SendKeys(_pplCredentials.PPLPassword);
        // wait 2 seconds
        _chromeDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);
        var submitButton = _chromeDriver.FindElement(By.XPath("//button[@type='submit']"));
        submitButton.Click();
    }

    private void WaitForLogin()
    {
        var driverWait = new WebDriverWait(_chromeDriver, TimeSpan.FromSeconds(10))
        {
            PollingInterval = TimeSpan.FromMicroseconds(300)
        };
        driverWait.Until(condition =>
        {
            try
            {
                var element = _chromeDriver.FindElement(
                    By.XPath("//div[contains(@class, 'amount-due-section')]")
                );
                return element.Displayed;
            }
            catch (Exception e)
            {
                Console.WriteLine("Not found element" + e.Message);
                return false;
            }
        });
    }
}
