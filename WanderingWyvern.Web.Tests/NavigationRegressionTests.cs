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
    public async Task DesktopNavbar_RemainsStickyAtTop_WhenScrollingDown()
    {
        await using var context = await _fixture.Browser.NewContextAsync(new BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize { Width = 1280, Height = 800 }
        });
        await MarkClientReadyAsync(context);
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{_fixture.BaseUrl}/test/session-view");
        await WaitForStylesAsync(page);
        await WaitForLoadingOverlayAsync(page);

        var navMenu = page.Locator(".nav-menu");
        var initialBox = await navMenu.BoundingBoxAsync();
        Assert.IsNotNull(initialBox);
        Assert.AreEqual(0, initialBox.Y, 1.0);

        // Scroll down 600px
        await page.EvaluateAsync("window.scrollTo(0, 600)");
        await page.WaitForTimeoutAsync(150);

        var scrolledBox = await navMenu.BoundingBoxAsync();
        Assert.IsNotNull(scrolledBox);
        // The navbar must remain stuck at top: Y == 0 relative to viewport
        Assert.AreEqual(0, scrolledBox.Y, 1.0);
    }

    [TestMethod]
    public async Task NavBrand_ShowsSubtitleAlways_AndDoesNotShowHardcodedTitle()
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

        // Subtitle is always visible with "The Wandering Wyvern"
        var subtitle = page.Locator(".nav-brand-subtitle");
        await subtitle.WaitForAsync();
        var subtitleText = await subtitle.TextContentAsync();
        Assert.AreEqual("The Wandering Wyvern", subtitleText?.Trim());

        // Without a campaign loaded, no campaign title should be rendered
        var titleCount = await page.Locator(".nav-brand-title").CountAsync();
        Assert.AreEqual(0, titleCount);
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

    [TestMethod]
    public async Task DirectNavigationToCampaignInfo_RendersWithoutServerException()
    {
        await using var context = await _fixture.Browser.NewContextAsync(new BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize { Width = 1280, Height = 800 }
        });
        await MarkClientReadyAsync(context);
        var page = await context.NewPageAsync();
        
        // Navigate directly to /campaign-info
        var response = await page.GotoAsync($"{_fixture.BaseUrl}/campaign-info");
        Assert.IsNotNull(response);
        Assert.AreEqual(200, response.Status);

        // Ensure page content loaded and no unhandled server exception is displayed
        var heading = page.Locator("h1, .folder-welcome, .page-container");
        await heading.First.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 5000 });

        var bodyText = await page.InnerTextAsync("body");
        Assert.IsFalse(bodyText.Contains("An unhandled exception occurred"), "Page should not display an unhandled server exception.");
        Assert.IsFalse(bodyText.Contains("InvalidOperationException"), "Page should not display an InvalidOperationException.");
    }

    [TestMethod]
    public async Task MobileViewport_DetectHorizontalOverflow()
    {
        int[] viewports = [390, 360];
        string[] testUrls = ["/", "/help", "/campaign-info", "/test/session-view", "/test/editable-document"];

        foreach (var width in viewports)
        {
            await using var context = await _fixture.Browser.NewContextAsync(new BrowserNewContextOptions
            {
                ViewportSize = new ViewportSize { Width = width, Height = 844 }
            });
            await MarkClientReadyAsync(context);
            var page = await context.NewPageAsync();

            foreach (var url in testUrls)
            {
                await page.GotoAsync($"{_fixture.BaseUrl}{url}");
                await WaitForStylesAsync(page);
                await page.WaitForTimeoutAsync(400);

                var overflowInfo = await page.EvaluateAsync<string?>(
                    """
                    () => {
                        const docWidth = document.documentElement.clientWidth;
                        const scrollW = document.documentElement.scrollWidth;
                        const bodyScrollW = document.body.scrollWidth;
                        
                        if (scrollW > docWidth || bodyScrollW > docWidth) {
                            const overflowing = [];
                            const all = document.querySelectorAll('*');
                            for (const el of all) {
                                const rect = el.getBoundingClientRect();
                                if (rect.right > docWidth + 1) {
                                    const cls = el.className ? ('.' + String(el.className).split(' ').join('.')) : '';
                                    overflowing.push(`${el.tagName}${cls} (right: ${rect.right.toFixed(1)}, viewport: ${docWidth})`);
                                }
                            }
                            return `scrollWidth (${scrollW}) / bodyScrollW (${bodyScrollW}) > clientWidth (${docWidth}). Elements:\n` + overflowing.slice(0, 10).join('\n');
                        }
                        return null;
                    }
                    """);

                if (overflowInfo != null)
                {
                    Assert.Fail($"Viewport {width}px on URL {url} has horizontal overflow:\n{overflowInfo}");
                }
            }
        }
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
                    (getComputedStyle(nav).position === "sticky" || getComputedStyle(nav).position === "fixed");
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
