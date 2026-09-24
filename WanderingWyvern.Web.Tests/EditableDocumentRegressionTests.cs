using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DhrMaes.WanderingWyvern.Web.Tests;

[TestClass]
[DoNotParallelize]
public sealed class EditableDocumentRegressionTests
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
    public async Task EditButtonIsAlignedWithHeaderInRow()
    {
        await using var context = await _fixture.Browser.NewContextAsync(new BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize { Width = 1280, Height = 800 }
        });
        await MarkClientReadyAsync(context);
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{_fixture.BaseUrl}/test/editable-document");

        // Wait for the button to appear in the DOM
        var button = page.Locator(".editable-document-actions button");
        await button.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 5000 });

        var headerRow = page.Locator(".editable-document-header-row");
        var headerContent = page.Locator(".editable-document-header-content");
        
        var rowBox = await headerRow.BoundingBoxAsync();
        var buttonBox = await button.BoundingBoxAsync();
        var contentBox = await headerContent.BoundingBoxAsync();

        Assert.IsNotNull(rowBox);
        Assert.IsNotNull(buttonBox);
        Assert.IsNotNull(contentBox);

        // Ensure the button is to the right of the header
        Assert.IsTrue(buttonBox.X > contentBox.X + contentBox.Width, "Button should be positioned to the right of the header content.");

        // Ensure the button does not overlap vertically in a way that breaks layout, 
        // e.g. its bottom should be close to the header row's bottom because of baseline alignment,
        // or its top is within the row bounds.
        Assert.IsTrue(buttonBox.Y >= rowBox.Y, "Button should not float above the header row.");
        Assert.IsTrue(buttonBox.Y + buttonBox.Height <= rowBox.Y + rowBox.Height, "Button should fit within the header row's vertical bounds.");
    }

    [TestMethod]
    public async Task EditMode_SupportsFullscreenToggleAndGenerousHeight()
    {
        await using var context = await _fixture.Browser.NewContextAsync(new BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize { Width = 1280, Height = 800 }
        });
        await MarkClientReadyAsync(context);
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{_fixture.BaseUrl}/test/editable-document");

        var editButton = page.Locator(".editable-document-actions button");
        await editButton.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 5000 });
        await editButton.ClickAsync();

        // Editor container and textarea should be displayed
        var editor = page.Locator(".editable-document-edit");
        var textarea = page.Locator(".editable-document-edit textarea");
        await editor.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 5000 });

        var initialTextareaBox = await textarea.BoundingBoxAsync();
        Assert.IsNotNull(initialTextareaBox);
        // Default height is 60vh (at 800px viewport, should be ~480px, well over 400px)
        Assert.IsTrue(initialTextareaBox.Height >= 400, $"Textarea should have generous default height, got {initialTextareaBox.Height}px");

        // Click fullscreen toggle button
        var fullscreenBtn = page.Locator(".fullscreen-toggle-btn").First;
        await fullscreenBtn.ClickAsync();

        var fullscreenEditor = page.Locator(".editable-document-edit.is-fullscreen");
        await fullscreenEditor.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 5000 });

        var fsTextareaBox = await textarea.BoundingBoxAsync();
        Assert.IsNotNull(fsTextareaBox);
        Assert.IsTrue(fsTextareaBox.Height > initialTextareaBox.Height, "Fullscreen textarea should be taller than windowed mode");
        Assert.IsTrue(fsTextareaBox.Width >= 1200, "Fullscreen textarea should span almost full viewport width");

        // Click toggle button to exit fullscreen
        await fullscreenBtn.ClickAsync();
        await page.Locator(".editable-document-edit:not(.is-fullscreen)").WaitForAsync(new LocatorWaitForOptions { Timeout = 5000 });
    }

    private static Task MarkClientReadyAsync(IBrowserContext context)
    {
        return context.AddInitScriptAsync(
            "() => sessionStorage.setItem('wandering-wyvern-client-ready', 'true')");
    }
}
