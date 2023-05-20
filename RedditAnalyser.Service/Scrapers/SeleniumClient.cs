using System.Drawing;
using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace RedditAnalyzer.Service.Scrapers;

internal class SeleniumClient : IDisposable
{
    private readonly ChromeDriver Driver;
    private readonly WebDriverWait Wait;
    private readonly Actions Actions;
    private readonly IJavaScriptExecutor JavaScript;
    private readonly double Timeout = 30;
    private bool disposed = false;

    public SeleniumClient()
    {
        var service = ChromeDriverService.CreateDefaultService();
        service.SuppressInitialDiagnosticInformation = true;
        service.EnableVerboseLogging = false;
        service.EnableAppendLog = false;

        var options = new ChromeOptions();
        options.AddArgument("--headless");
        options.AddArgument("--log-level=3");
        options.AddArgument("--disable-logging");
        options.SetLoggingPreference(LogType.Driver, LogLevel.Off);

        Driver = new ChromeDriver(service, options);
        Driver.Manage().Window.Size = new Size(800, 1000);
        Actions = new Actions(Driver);
        JavaScript = (IJavaScriptExecutor)Driver;
        Wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(Timeout));
    }

    public void NavigateTo(string url)
    {
        if (string.IsNullOrEmpty(url))
            throw new ArgumentNullException($"{nameof(url)}: {url}");

        Driver.Navigate().GoToUrl(url);
        WaitForPageLoad();
        AddMouseTracker();
    }

    public void MaxScrollToEnd()
    {
        int tries = 0;
        var pageSize = GetPageSize();

        while (tries <= 3)
        {
            ScrollToEnd();
            Thread.Sleep(200);
            var currentSize = GetPageSize();

            if (currentSize > pageSize)
            {
                pageSize = currentSize;
            }
            else
            {
                tries++;
            }
        }
    }

    public void ScrollToEnd()
    {
        JavaScript.ExecuteScript("window.scrollTo(0, document.body.scrollHeight)");
    }

    public void Screenshot(string filename)
    {
        Screenshot screenshot = ((ITakesScreenshot)Driver).GetScreenshot();
        screenshot.SaveAsFile(filename, ScreenshotImageFormat.Png);
    }

    public string GetPagesHtmlAsString() => Driver.PageSource;

    public HtmlDocument GetCurrentHtml()
    {
        var htmlString = Driver.PageSource;
        var html = new HtmlDocument();
        html.LoadHtml(htmlString);

        return html;
    }

    public void ClickElementByXPath(string xPath)
    {
        var element = Driver.FindElement(By.XPath(xPath));
        ScrollIntoView(element);
        Actions.Click(element).Perform();
        Thread.Sleep(100);
    }

    public void HoverElementByXPath(string xPath)
    {
        //Problem: OPs timestamp doesnt work
        //Solution save comments link and open in non auth mode

        //Screenshot("ss0.png");
        var element = Wait.Until(driver => driver.FindElement(By.XPath(xPath)));
        ScrollIntoView(element);
        //Console.WriteLine(element.Location.ToString());
        //Console.WriteLine(element.Size.ToString());
        //Screenshot("ss1.png");
        Thread.Sleep(100);
        //Screenshot("ss1.png");

        Actions.MoveToElement(element).MoveByOffset(10, 0).Perform();
        //var cursorX = (long)JavaScript.ExecuteScript("return window.cursorPosition.x;");
        //var cursorY = (long)JavaScript.ExecuteScript("return window.cursorPosition.y;");
        //Console.WriteLine($"After X: {cursorX}, Y: {cursorY}");
        //var essa = 0;
    }

    public void LoginToReddit()
    {
        NavigateTo("https://www.reddit.com/account/login/?experiment_d2x_safari_onetap=enabled&experiment_d2x_google_sso_gis_parity=enabled&experiment_d2x_am_modal_design_update=enabled&experiment_mweb_sso_login_link=enabled&shreddit=true");

        var usernameInput = Driver.FindElement(By.XPath("//input[@name='username']"));
        var passwordInput = Driver.FindElement(By.XPath("//input[@name='password']"));
        var button = Driver.FindElement(By.XPath("//button[@type='submit']"));

        Actions.Click(usernameInput).Perform();
        Actions.SendKeys("252807").Perform();

        Actions.Click(passwordInput).Perform();
        Actions.SendKeys("123qazxS#").Perform();

        var currentUrl = Driver.Url;
        Actions.Click(button).Perform();
        WaitForUrlToChange(currentUrl);
    }

    public void AcceptCookies()
    {
        var html = GetCurrentHtml();

        var buttons = html.DocumentNode.SelectNodes(".//body//button[@type='submit']");

        if (buttons is null) return;

        var acceptButton = buttons.SingleOrDefault(b => b.InnerText.Contains("Accept all"));

        if (acceptButton is null) return;

        ClickElementByXPath(acceptButton.XPath);
    }

    private void AddMouseTracker()
    {
        JavaScript.ExecuteScript(@"
            window.cursorPosition = { x: 0, y: 0 };
            window.addEventListener('mousemove', function (event) {
                window.cursorPosition.x = event.clientX + window.pageXOffset;
                window.cursorPosition.y = event.clientY + window.pageYOffset;
            });
        ");
    }

    private void ScrollIntoView(IWebElement element)
    {
        JavaScript.ExecuteScript("arguments[0].scrollIntoView(true);", element);
        JavaScript.ExecuteScript("window.scrollBy(0, -100);");
    }

    private int GetPageSize() =>
        Convert.ToInt32(JavaScript.ExecuteScript("return document.body.scrollHeight;"));

    private void WaitForPageLoad()
    {
        Wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
    }

    private bool ElementHasChildren(IWebElement element) => 
        element.FindElements(By.CssSelector("*")).Count != 0;

    private IWebElement? WaitForElement(By locator)
    {
        return Wait.Until(Driver =>
        {
            var element = Driver.FindElement(locator);
            return element.Displayed ? element : null;
        });
    }

    private IReadOnlyCollection<IWebElement>? WaitForAllElements(By locator)
    {
        return Wait.Until(Driver =>
        {
            var elements = Driver.FindElements(locator);
            return elements.Count > 0 ? elements : null;
        });
    }

    private void WaitForUrlToChange(string currentUrl)
    {
        Wait.Until(driver => driver.Url != currentUrl);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposed) return;

        if (disposing)
        {
            Driver.Quit();
            Driver.Dispose();
        }

        disposed = true;
    }

    ~SeleniumClient()
    {
        Dispose(false);
    }
}
