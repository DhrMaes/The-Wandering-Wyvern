using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DhrMaes.WanderingWyvern.Web.Tests;

[TestClass]
[DoNotParallelize]
public sealed class SessionViewRegressionTests
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
    public async Task SplitView_RendersBothPanesByDefault_WithOnlyEditButtonsInPanes()
    {
        await using var context = await _fixture.Browser.NewContextAsync(new BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize { Width = 1280, Height = 800 }
        });
        await MarkClientReadyAsync(context);
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{_fixture.BaseUrl}/test/session-view");

        var outlineColumn = page.Locator(".outline-column");
        var scriptColumn = page.Locator(".script-column");

        await outlineColumn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 5000 });
        await scriptColumn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 5000 });

        // Active toolbar button is "Gesplitst"
        var activeBtn = page.Locator(".session-view-btn.is-active");
        var activeText = await activeBtn.InnerTextAsync();
        StringAssert.Contains(activeText, "Gesplitst");

        // Panes only have their "Bewerken" action button in the header
        var outlineButtons = outlineColumn.Locator(".editable-document-actions button");
        Assert.AreEqual(1, await outlineButtons.CountAsync());
        StringAssert.Contains(await outlineButtons.First.InnerTextAsync(), "Bewerken");

        var scriptButtons = scriptColumn.Locator(".editable-document-actions button");
        Assert.AreEqual(1, await scriptButtons.CountAsync());
        StringAssert.Contains(await scriptButtons.First.InnerTextAsync(), "Bewerken");
    }

    [TestMethod]
    public async Task ToolbarOutlineOnly_CollapsesScript_GivesOutlineFullView()
    {
        await using var context = await _fixture.Browser.NewContextAsync(new BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize { Width = 1280, Height = 800 }
        });
        await MarkClientReadyAsync(context);
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{_fixture.BaseUrl}/test/session-view");

        var outlineColumn = page.Locator(".outline-column");
        var scriptColumn = page.Locator(".script-column");
        await outlineColumn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 5000 });

        // Click "Alleen outline" in toolbar
        var outlineOnlyBtn = page.Locator(".session-view-controls button:has-text('Alleen outline')");
        await outlineOnlyBtn.ClickAsync();

        // Script is collapsed/hidden, outline is visible in full view
        await scriptColumn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Hidden, Timeout = 5000 });
        Assert.IsTrue(await outlineColumn.IsVisibleAsync());

        var activeBtn = page.Locator(".session-view-btn.is-active");
        var activeText = await activeBtn.InnerTextAsync();
        StringAssert.Contains(activeText, "Alleen outline");
    }

    [TestMethod]
    public async Task ToolbarScriptOnly_CollapsesOutline_GivesScriptFullView()
    {
        await using var context = await _fixture.Browser.NewContextAsync(new BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize { Width = 1280, Height = 800 }
        });
        await MarkClientReadyAsync(context);
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{_fixture.BaseUrl}/test/session-view");

        var outlineColumn = page.Locator(".outline-column");
        var scriptColumn = page.Locator(".script-column");
        await outlineColumn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 5000 });

        // Click "Alleen script" in toolbar
        var scriptOnlyBtn = page.Locator(".session-view-controls button:has-text('Alleen script')");
        await scriptOnlyBtn.ClickAsync();

        // Outline is collapsed/hidden, script is visible in full view
        await outlineColumn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Hidden, Timeout = 5000 });
        Assert.IsTrue(await scriptColumn.IsVisibleAsync());

        var activeBtn = page.Locator(".session-view-btn.is-active");
        var activeText = await activeBtn.InnerTextAsync();
        StringAssert.Contains(activeText, "Alleen script");
    }

    [TestMethod]
    public async Task ToolbarSplitView_RestoresBothPanes()
    {
        await using var context = await _fixture.Browser.NewContextAsync(new BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize { Width = 1280, Height = 800 }
        });
        await MarkClientReadyAsync(context);
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{_fixture.BaseUrl}/test/session-view");

        var outlineColumn = page.Locator(".outline-column");
        var scriptColumn = page.Locator(".script-column");
        await outlineColumn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 5000 });

        // First collapse outline via "Alleen script"
        var scriptOnlyBtn = page.Locator(".session-view-controls button:has-text('Alleen script')");
        await scriptOnlyBtn.ClickAsync();
        await outlineColumn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Hidden, Timeout = 5000 });

        // Now restore split view
        var splitBtn = page.Locator(".session-view-controls button:has-text('Gesplitst')");
        await splitBtn.ClickAsync();

        // Both columns are visible again
        await outlineColumn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 5000 });
        await scriptColumn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 5000 });

        var activeBtn = page.Locator(".session-view-btn.is-active");
        var activeText = await activeBtn.InnerTextAsync();
        StringAssert.Contains(activeText, "Gesplitst");
    }

    private static Task MarkClientReadyAsync(IBrowserContext context)
    {
        return context.AddInitScriptAsync(
            "() => sessionStorage.setItem('wandering-wyvern-client-ready', 'true')");
    }
}
