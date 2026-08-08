using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DhrMaes.WanderingWyvern.Web.Tests;

[TestClass]
[DoNotParallelize]
public sealed class NavigationRegressionTests
{
    private static BrowserFixture _fixture = null!;

    [ClassInitialize]
    public static async Task ClassInitialize(TestContext _)
    {
        _fixture = new BrowserFixture();
        await _fixture.InitializeAsync();
    }

    [ClassCleanup]
    public static async Task ClassCleanup()
    {
        await _fixture.DisposeAsync();
    }

    [TestMethod]
    public async Task DesktopNavbarKeepsHelpAtBottom()
    {
        await using var context = await _fixture.Browser.NewContextAsync(new BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize { Width = 1280, Height = 800 }
        });
        await MarkClientReadyAsync(context);
        var page = await context.NewPageAsync();
        await page.GotoAsync(_fixture.BaseUrl);
        await WaitForStylesAsync(page);
        await WaitForLoadingOverlayAsync(page);

        var metrics = await ReadMetricsAsync(page);

        Assert.IsTrue(metrics.DrawerOpen);
        AssertInRange(metrics.NavBottom - metrics.HelpBottom, 30, 34);
    }

    [TestMethod]
    public async Task MobileNavbarOpensAsIndependentDrawerWithPinnedHelp()
    {
        await using var context = await _fixture.Browser.NewContextAsync(new BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize { Width = 390, Height = 844 }
        });
        await MarkClientReadyAsync(context);
        var page = await context.NewPageAsync();
        await page.GotoAsync(_fixture.BaseUrl);
        await WaitForStylesAsync(page);
        await WaitForLoadingOverlayAsync(page);

        Assert.IsFalse(await page.Locator(".nav-drawer").EvaluateAsync<bool>("drawer => drawer.classList.contains('is-open')"));

        await page.Locator(".nav-drawer-toggle").ClickAsync();
        await page.Locator(".nav-drawer.is-open").WaitForAsync();

        var metrics = await ReadMetricsAsync(page);

        Assert.AreEqual("hidden", await page.EvaluateAsync<string>("() => getComputedStyle(document.body).overflowY"));
        AssertInRange(metrics.DrawerBottom - metrics.HelpBottom, 30, 34);
        Assert.IsTrue(metrics.LinksBottom < metrics.HelpTop);
    }

    [TestMethod]
    public async Task StartupLoadingOverlayFillsAndCentersTheViewport()
    {
        await using var context = await _fixture.Browser.NewContextAsync(new BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize { Width = 390, Height = 844 }
        });
        var page = await context.NewPageAsync();
        await page.GotoAsync(_fixture.BaseUrl);
        await WaitForStylesAsync(page);

        var metrics = await page.Locator("#app-loading").EvaluateAsync<LoadingMetrics>(
            """
            loading => {
                const rect = loading.getBoundingClientRect();
                const spinner = loading.querySelector(".app-loading-spinner").getBoundingClientRect();
                return {
                    top: rect.top,
                    left: rect.left,
                    width: rect.width,
                    height: rect.height,
                    spinnerCenterX: spinner.left + spinner.width / 2,
                    spinnerCenterY: spinner.top + spinner.height / 2
                };
            }
            """);

        Assert.AreEqual(0, metrics.Top);
        Assert.AreEqual(0, metrics.Left);
        Assert.AreEqual(390, metrics.Width);
        Assert.AreEqual(844, metrics.Height);
        AssertInRange(metrics.SpinnerCenterX, 0, metrics.Width);
        AssertInRange(metrics.SpinnerCenterY, 0, metrics.Height);
    }

    [TestMethod]
    public async Task ReturningToOverviewKeepsLoadingOverlayHidden()
    {
        await using var context = await _fixture.Browser.NewContextAsync(new BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize { Width = 1280, Height = 800 }
        });
        await MarkClientReadyAsync(context);
        var page = await context.NewPageAsync();
        await page.GotoAsync(_fixture.BaseUrl);
        await WaitForStylesAsync(page);
        await WaitForLoadingOverlayAsync(page);

        await page.Locator(".nav-help a").ClickAsync();
        await page.WaitForURLAsync($"{_fixture.BaseUrl}/help");
        await page.Locator(".nav-link[href='/']").ClickAsync();
        await page.WaitForURLAsync($"{_fixture.BaseUrl}/");
        await WaitForLoadingOverlayAsync(page);

        Assert.IsTrue(await page.Locator("#app-loading").EvaluateAsync<bool>(
            "loading => loading.classList.contains('is-hidden')"));
    }

    private static void AssertInRange(double actual, double minimum, double maximum)
    {
        Assert.IsTrue(
            actual >= minimum && actual <= maximum,
            $"Expected {actual} to be between {minimum} and {maximum}.");
    }

    private static async Task WaitForStylesAsync(IPage page)
    {
        await page.WaitForFunctionAsync(
            """
            () => {
                const loading = document.querySelector("#app-loading");
                const nav = document.querySelector(".nav-menu");
                return (!loading || getComputedStyle(loading).position === "fixed") &&
                    getComputedStyle(nav).position === "sticky";
            }
            """);
    }

    private static async Task WaitForLoadingOverlayAsync(IPage page)
    {
        await page.WaitForFunctionAsync(
            """
            () => {
                const loading = document.querySelector("#app-loading");
                return !loading ||
                    loading.classList.contains("is-hidden") ||
                    getComputedStyle(loading).pointerEvents === "none";
            }
            """);
    }

    private static Task MarkClientReadyAsync(IBrowserContext context)
    {
        return context.AddInitScriptAsync(
            "() => sessionStorage.setItem('wandering-wyvern-client-ready', 'true')");
    }

    private static async Task<NavbarMetrics> ReadMetricsAsync(IPage page)
    {
        return await page.EvaluateAsync<NavbarMetrics>(
            """
            () => {
                const drawer = document.querySelector(".nav-drawer");
                const content = document.querySelector(".nav-drawer-content").getBoundingClientRect();
                const links = document.querySelector(".nav-drawer-links").getBoundingClientRect();
                const help = document.querySelector(".nav-help").getBoundingClientRect();
                const nav = document.querySelector(".nav-menu").getBoundingClientRect();
                return {
                    drawerOpen: drawer.classList.contains("is-open"),
                    navBottom: nav.bottom,
                    drawerBottom: content.bottom,
                    linksBottom: links.bottom,
                    helpTop: help.top,
                    helpBottom: help.bottom
                };
            }
            """);
    }

    private sealed class NavbarMetrics
    {
        public bool DrawerOpen { get; set; }
        public double NavBottom { get; set; }
        public double DrawerBottom { get; set; }
        public double LinksBottom { get; set; }
        public double HelpTop { get; set; }
        public double HelpBottom { get; set; }
    }

    private sealed class LoadingMetrics
    {
        public double Top { get; set; }
        public double Left { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double SpinnerCenterX { get; set; }
        public double SpinnerCenterY { get; set; }
    }
}
