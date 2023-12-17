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
    private Dictionary<string, string> _cookies;
    private readonly PPLCredentials _pplCredentials;

    public PPLService(PPLCredentials pplCredentials)
    {
        _pplCredentials = pplCredentials;
        _cookies = new Dictionary<string, string>();
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
            _cookies["ARRAffinity"] = _chromeDriver.Manage().Cookies.GetCookieNamed("ARRAffinity").Value;
            _cookies["ARRAffinitySameSite"] = _chromeDriver
                .Manage()
                .Cookies
                .GetCookieNamed("ARRAffinitySameSite")
                .Value;
            _cookies[".ASPXAUTH"] = _chromeDriver.Manage().Cookies.GetCookieNamed(".ASPXAUTH").Value;
            _cookies["ASP.NET_SessionId"] = _chromeDriver
                .Manage()
                .Cookies
                .GetCookieNamed("ASP.NET_SessionId")
                .Value;
        }
        catch (Exception e)
        {
            Console.WriteLine("Bad cookies fetch" + e.Message);
        }
        _cookies["Authorization"] = cookieValue;
        return cookieValue;
    }

    public async Task<Zapp.Models.PPL.BillToDateModel> GetBillToDate()
    {
        var response = new BillToDateModel();

        if (string.IsNullOrEmpty(_cookies["Authorization"]))
        {
            return response;
        }
        using HttpClient httpClient = new HttpClient();

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            _cookies["Authorization"]
        );
        var responseHttp = await httpClient.GetAsync(
            "https://api.pplcloud.net/selfserve/api/account/6716108115/bill-to-date"
        );

        if (responseHttp.IsSuccessStatusCode)
        {
            var billToDateModel = await responseHttp.Content.ReadFromJsonAsync<BillToDateModel>();
            if (billToDateModel == null)
                return response;
            return billToDateModel;
        }

        return response;
    }

    public async Task<DailyUsageModel> GetDailyUsage(DateTime startDate, DateTime endDate)
    {
        var response = new DailyUsageModel();

        var url = "https://www.pplelectric.com/euweb/secured/Service.asmx/GetDailyUsage";

        if (_cookies.ContainsKey("Authorization"))
        {
            return response;
        }

        var cookieContainer = new CookieContainer();
        cookieContainer.Add(
            new Uri("http://www.pplelectric.com", UriKind.Absolute),
            new System.Net.Cookie("ARRAffinity", _cookies["ARRAffinity"])
        );
        cookieContainer.Add(
            new Uri("http://www.pplelectric.com", UriKind.Absolute),
            new System.Net.Cookie("ARRAffinitySameSite", _cookies["ARRAffinitySameSite"])
        );
        cookieContainer.Add(
            new Uri("http://www.pplelectric.com", UriKind.Absolute),
            new System.Net.Cookie(".ASPXAUTH", _cookies[".ASPXAUTH"])
        );
        cookieContainer.Add(
            new Uri("http://www.pplelectric.com", UriKind.Absolute),
            new System.Net.Cookie("Authorization", _cookies["Authorization"])
        );
        cookieContainer.Add(
            new Uri("http://www.pplelectric.com", UriKind.Absolute),
            new System.Net.Cookie("ASP.NET_SessionId", _cookies["ASP.NET_SessionId"])
        );

        HttpClientHandler handler = new HttpClientHandler()
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            CookieContainer = cookieContainer
        };

        using HttpClient httpClient = new HttpClient(handler);
        var json = JsonConvert.SerializeObject(
            new
            {
                usageInput = new
                {
                    StartDate = startDate.ToString("d"),
                    EndDate = endDate.ToString("d")
                }
            }
        );
        var request = new HttpRequestMessage
        {
            RequestUri = new Uri(
                "https://www.pplelectric.com/euweb/secured/Service.asmx/GetDailyUsage"
            ),
            Method = HttpMethod.Post,
            Headers =
            {
                { "accept", "application/json" },
                { "accept-encoding", "gzip, deflate, br" },
                { "path", "/euweb/secured/Service.asmx/GetDailyUsage" },
                { "scheme", "https" },
                { "accept-language", "en-US,en;q=0.9" },
                { "origin", "https://www.pplelectric.com" },
                {
                    "referer",
                    "https://www.pplelectric.com/my-account/account-management/daily-usage"
                },
                {
                    "sec-ch-ua",
                    "\"Not_A Brand\";v=\"8\", \"Chromium\";v=\"120\", \"Brave\";v=\"120\""
                },
                { "sec-ch-ua-mobile", "?1" },
                { "sec-ch-ua-platform", "\"Android\"" },
                { "sec-fetch-dest", "empty" },
                { "sec-fetch-mode", "cors" },
                { "sec-fetch-site", "same-origin" },
                { "sec-gpc", "1" },
                { "x-requested-with", "XMLHttpRequest" },
                {
                    "user-agent",
                    "Mozilla/5.0 (Linux; Android 6.0; Nexus 5 Build/MRA58N) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Mobile Safari/537.36"
                }
            },
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        var responseHttp = await httpClient.SendAsync(request);

        Console.WriteLine(await responseHttp.Content.ReadAsStringAsync());
        if (responseHttp.IsSuccessStatusCode)
        {
            Console.WriteLine(await responseHttp.Content.ReadAsStringAsync());

            var dailyUsage = await responseHttp.Content.ReadFromJsonAsync<DailyUsageModel>();

            _chromeDriver.Close();
            return dailyUsage;
        }
        _chromeDriver.Close();
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
