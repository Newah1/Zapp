using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Zapp.Models.PPL;

namespace Zapp.Services;

public class PPLService : IPPLService
{
    private ChromeDriver _chromeDriver;
    private string _cookieString;
    private string _arrAffinity;
    private string _arrAffinitySameSite;
    private string _aspxAuth;
    private readonly PPLCredentials _pplCredentials;

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
        // let's go to the usage page...
        Console.WriteLine(cookie.Value);
        _chromeDriver
            .Navigate()
            .GoToUrl("https://www.pplelectric.com/my-account/account-management/daily-usage");
        try
        {
            _arrAffinity = _chromeDriver.Manage().Cookies.GetCookieNamed("ARRAffinity").Value;
            _arrAffinitySameSite = _chromeDriver
                .Manage()
                .Cookies
                .GetCookieNamed("ARRAffinitySameSite")
                .Value;
            _aspxAuth = _chromeDriver.Manage().Cookies.GetCookieNamed(".ASPXAUTH").Value;
        }
        catch (Exception e)
        {
            Console.WriteLine("Bad cookies fetch" + e.Message);
        }
        Console.WriteLine($"{_arrAffinity} {_arrAffinitySameSite} {_aspxAuth} {cookieValue}");
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

    public async Task<DailyUsageModel> GetDailyUsage(DateTime startDate, DateTime endDate)
    {
        var response = new DailyUsageModel();

        var url = "https://www.pplelectric.com/euweb/secured/Service.asmx/GetDailyUsage";

        if (string.IsNullOrEmpty(_cookieString))
        {
            return response;
        }

        var cookieContainer = new CookieContainer();
        cookieContainer.Add(new Uri("https://www.pplelectric.com"), new System.Net.Cookie("ARRAffinity", _arrAffinity));
        cookieContainer.Add(new Uri("https://www.pplelectric.com"), new System.Net.Cookie("ARRAffinitySameSite", _arrAffinitySameSite));
        cookieContainer.Add(new Uri("https://www.pplelectric.com"), new System.Net.Cookie(".ASPXAUTH",_aspxAuth));
        
        HttpClientHandler handler = new HttpClientHandler()
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            CookieContainer = cookieContainer
        };
        
        
        using HttpClient httpClient = new HttpClient(handler);
        var json = JsonConvert.SerializeObject(
            new { usageInput = new { StartDate = startDate.ToString("d"), EndDate = endDate.ToString("d") } }
        );
        var request = new HttpRequestMessage
        {
            RequestUri = new Uri("https://www.pplelectric.com/euweb/secured/Service.asmx/GetDailyUsage"),
            Method = HttpMethod.Post,
            Headers =
            {
                { "authority", "www.pplelectric.com" },
                { "accept", "application/json, text/javascript, */*; q=0.01" },
                { "accept-encoding", "gzip, deflate, br" },
                { "accept-language", "en-US,en;q=0.9" },
                { "origin", "https://www.pplelectric.com" },
                { "referer", "https://www.pplelectric.com/my-account/account-management/daily-usage" },
                { "sec-ch-ua", "\"Not_A Brand\";v=\"8\", \"Chromium\";v=\"120\", \"Brave\";v=\"120\"" },
                { "sec-ch-ua-mobile", "?1" },
                { "sec-ch-ua-platform", "\"Android\"" },
                { "sec-fetch-dest", "empty" },
                { "sec-fetch-mode", "cors" },
                { "sec-fetch-site", "same-origin" },
                { "sec-gpc", "1" },
                { "x-requested-with", "XMLHttpRequest" }
            },
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        
        var responseHttp = await httpClient.SendAsync(request);

        if (responseHttp.IsSuccessStatusCode)
        {
            Console.WriteLine(await responseHttp.Content.ReadAsStringAsync());
            
            var dailyUsage = await responseHttp.Content.ReadFromJsonAsync<DailyUsageModel>();
            return dailyUsage;
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
        var driverWait = new WebDriverWait(_chromeDriver, TimeSpan.FromSeconds(5))
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
