using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SearchTitleVideo.Loggers;

/*
Данная задача является примером того как с помощью автотеста можно осуществлять поиск названия видеоролика 
по ключевому Трейлер

Шаги:
1. Запуск браузера 
2. Открытие сайта www.youtube.com
3. Поиск видео по ключевому слову
4. Слово нашлось/не нашлось
5. Закрытие браузера

- Шаги 3 и 4 залогированы и находятся в цикле с лимитом в 20 итераций, если ключевое слово е найдется
- В зависимости от исхода Шага 4, в лог записывается соответствующая строка после завершения рана автотеста
 */

namespace SearchTitleVideo;

public class Tests
{
    private const int MaxStep = 20;

    private const string YouTubeLink = "https://www.youtube.com";
    private const string SearchWordString = "Трейлер";

    private readonly By _searchWord = By.XPath($"//yt-formatted-string[contains(text(),'{SearchWordString}')]");
    private readonly By _pageTitle = By.XPath($"//*[contains(text(),'{SearchWordString}')]/ancestor::a");
    private readonly By _updatePage = By.XPath("//*[@class='style-scope ytd-logo']");

    private IWebDriver _driver;
    private LoggerSystem _loggerSystem;

    [SetUp]
    public void Setup()
    {
        _loggerSystem = new LoggerSystem();
        _driver = new ChromeDriver();
        _driver.Navigate().GoToUrl(YouTubeLink);
        _driver.Manage().Window.Maximize();
    }

    [Test]
    public void RunTest()
    {
        var step = 0;

        while (step < MaxStep)
        {
            step++;
            LogPage(step);
            if (step > 1)
            {
                GetUpdatePage();
            }

            var exist = Search();
            if (!exist) continue;
            GoToFoundPage();
            LogFinalResult(true, step);
            return;
        }

        LogFinalResult(false, step);
    }

    private string GetVideoTitle()
    {
        return _driver.FindElement(_pageTitle).Text;
    }

    private void LogPage(int step)
    {
        _loggerSystem.Write($"Поиск слова {SearchWordString}. Попытка #{step}");
    }

    private void GetUpdatePage()
    {
        var firstNameArticle = _driver.FindElement(_updatePage);
        firstNameArticle.Click();
        Thread.Sleep(400);
    }

    private bool Search()
    {
        Thread.Sleep(400);
        var placeHolderContent = _driver.FindElements(_searchWord);
        var exist = placeHolderContent.Count > 0;
        return exist;
    }

    private void GoToFoundPage()
    {
        var placeHolderContent = _driver.FindElement(_pageTitle);
        placeHolderContent.Click();
    }

    private void LogFinalResult(bool exist, int currentStep)
    {
        _loggerSystem.Write(
            exist
                ? $"Чтобы попасть на видеоролик {GetVideoTitle()} в названии необходимо сделать {currentStep} переходов"
                : $"Не удалось найти Навания видеоролика по ключевому слову {SearchWordString}. Было потрачено на поиск {MaxStep} шагов");
    }

    [TearDown]
    public void TearDown()
    {
        _loggerSystem.Close();
        _driver.Quit();
    }
}