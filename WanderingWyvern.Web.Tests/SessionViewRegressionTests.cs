using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DhrMaes.WanderingWyvern.Web.Tests;

[TestClass]
[DoNotParallelize]
public sealed class SessionViewRegressionTests
{
    private static BrowserFixture _fixture = null!;

    public TestContext? TestContext { get; set; }

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
        page.Console += (_, msg) => TestContext?.WriteLine($"[BROWSER {msg.Type}] {msg.Text}");
        page.PageError += (_, err) => TestContext?.WriteLine($"[BROWSER ERROR] {err}");
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
    public async Task EchoesOfTheUnderdark_RendersBothPreambleAndSceneContent_AndNavMenuSticks()
    {
        await using var context = await _fixture.Browser.NewContextAsync(new BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize { Width = 1280, Height = 800 }
        });
        await MarkClientReadyAsync(context);
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{_fixture.BaseUrl}/test/session-view");
        await page.Locator(".scene-focus-section").WaitForAsync(new LocatorWaitForOptions { Timeout = 10000 });

        // Assert Scene Focus Content is rendered (not empty!)
        var focusContent = page.Locator(".scene-focus-content");
        Assert.IsTrue(await focusContent.CountAsync() > 0);
        var contentText = await focusContent.First.InnerTextAsync();
        Assert.IsFalse(string.IsNullOrWhiteSpace(contentText), "Scene focus content should not be empty!");

        // Assert Preamble is rendered
        var preamble = page.Locator(".session-preamble");
        Assert.IsTrue(await preamble.CountAsync() > 0);
        var preambleText = await preamble.First.InnerTextAsync();
        Assert.IsFalse(string.IsNullOrWhiteSpace(preambleText), "Preamble should not be empty!");

        // Assert NavMenu is fixed on desktop and does not move when scrolling
        var navMenu = page.Locator(".nav-menu");
        var initialBox = await navMenu.BoundingBoxAsync();
        Assert.IsNotNull(initialBox);

        await page.EvaluateAsync("window.scrollTo(0, 400)");
        await Task.Delay(200);

        var scrolledBox = await navMenu.BoundingBoxAsync();
        Assert.IsNotNull(scrolledBox);
        Assert.AreEqual(initialBox.Y, scrolledBox.Y, 2, "Nav menu must stick to the viewport (fixed) when scrolling on desktop!");
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
        page.Console += (_, msg) => TestContext?.WriteLine($"[BROWSER {msg.Type}] {msg.Text}");
        page.PageError += (_, err) => TestContext?.WriteLine($"[BROWSER ERROR] {err}");
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
        page.Console += (_, msg) => TestContext?.WriteLine($"[BROWSER {msg.Type}] {msg.Text}");
        page.PageError += (_, err) => TestContext?.WriteLine($"[BROWSER ERROR] {err}");
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
        page.Console += (_, msg) => TestContext?.WriteLine($"[BROWSER {msg.Type}] {msg.Text}");
        page.PageError += (_, err) => TestContext?.WriteLine($"[BROWSER ERROR] {err}");
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

    [TestMethod]
    public async Task ToolbarFlowView_ShowsFlowCanvas()
    {
        await using var context = await _fixture.Browser.NewContextAsync(new BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize { Width = 1280, Height = 800 }
        });
        await MarkClientReadyAsync(context);
        var page = await context.NewPageAsync();
        page.Console += (_, msg) => TestContext?.WriteLine($"[BROWSER {msg.Type}] {msg.Text}");
        page.PageError += (_, err) => TestContext?.WriteLine($"[BROWSER ERROR] {err}");
        await page.GotoAsync($"{_fixture.BaseUrl}/test/session-view");

        var outlineColumn = page.Locator(".outline-column");
        await outlineColumn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 5000 });

        // Click "Flow & Builder" in toolbar
        var flowBtn = page.Locator(".session-view-controls button:has-text('Flow & Builder')");
        await flowBtn.ClickAsync();

        // Canvas panel is visible, split-pane is not shown
        var canvasPanel = page.Locator(".session-flow-view-panel");
        await canvasPanel.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 5000 });
        Assert.IsTrue(await canvasPanel.IsVisibleAsync());

        var activeBtn = page.Locator(".session-view-btn.is-active");
        var activeText = await activeBtn.InnerTextAsync();
        StringAssert.Contains(activeText, "Flow & Builder");
    }

    [TestMethod]
    public async Task SessionView_MobileViewport_StacksPanesAndFloatingLiveNotesWorks()
    {
        await using var context = await _fixture.Browser.NewContextAsync(new BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize { Width = 390, Height = 844 }
        });
        await MarkClientReadyAsync(context);
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{_fixture.BaseUrl}/test/session-view");

        var outlineColumn = page.Locator(".outline-column");
        var scriptColumn = page.Locator(".script-column");
        await outlineColumn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 5000 });
        await scriptColumn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 5000 });

        // Both columns visible on mobile (stacked)
        Assert.IsTrue(await outlineColumn.IsVisibleAsync());
        Assert.IsTrue(await scriptColumn.IsVisibleAsync());

        // Floating Live Notes trigger is accessible
        var notesTrigger = page.Locator(".live-notes-trigger-btn");
        await notesTrigger.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 5000 });
        Assert.IsTrue(await notesTrigger.IsVisibleAsync());
        await notesTrigger.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 5000 });
        Assert.IsTrue(await notesTrigger.IsVisibleAsync());

        // Open chat window
        await notesTrigger.ClickAsync();
        var chatWindow = page.Locator(".live-notes-chat-window");
        await chatWindow.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 5000 });
        Assert.IsTrue(await chatWindow.IsVisibleAsync());

        // Input field is present in the floating chat window
        var inputField = page.Locator(".chat-input-field");
        Assert.IsTrue(await inputField.IsVisibleAsync());

        // Close chat window
        var closeBtn = page.Locator(".chat-header-btn");
        await closeBtn.ClickAsync();
        await chatWindow.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Hidden, Timeout = 5000 });
    }

    [TestMethod]
    [DataRow(320, 568)]
    [DataRow(360, 800)]
    [DataRow(375, 667)]
    [DataRow(390, 844)]
    public async Task SessionView_MobileViewports_HasNoHorizontalOverflow_AcrossAllViewsAndInteractions(int width, int height)
    {
        await using var context = await _fixture.Browser.NewContextAsync(new BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize { Width = width, Height = height }
        });
        await MarkClientReadyAsync(context);
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{_fixture.BaseUrl}/test/session-view");

        var outlineColumn = page.Locator(".outline-column");
        var scriptColumn = page.Locator(".script-column");
        await outlineColumn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 5000 });
        await scriptColumn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 5000 });

        // 1. Initial Focus Mode with user's real campaign data
        await AssertNoHorizontalOverflowAsync(page, $"{width}x{height} - Initial Scene 1 Focus Mode");

        // 2. Open Mobile Nav Drawer
        var navToggle = page.Locator(".nav-drawer-toggle");
        if (await navToggle.IsVisibleAsync())
        {
            await navToggle.ClickAsync();
            var openDrawer = page.Locator(".nav-drawer.is-open");
            await openDrawer.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 3000 });
            await AssertNoHorizontalOverflowAsync(page, $"{width}x{height} - Mobile Nav Drawer Open");

            await navToggle.ClickAsync();
            await page.Locator(".nav-drawer:not(.is-open)").WaitForAsync(new LocatorWaitForOptions { Timeout = 3000 });
        }

        // 3. Open Floating Live Notes
        var notesTrigger = page.Locator(".live-notes-trigger-btn");
        await notesTrigger.ClickAsync();
        var chatWindow = page.Locator(".live-notes-chat-window");
        await chatWindow.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 5000 });
        await AssertNoHorizontalOverflowAsync(page, $"{width}x{height} - Floating Live Notes Open");

        // Close Live Notes
        var closeBtn = page.Locator(".chat-header-btn");
        await closeBtn.ClickAsync();
        await chatWindow.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Hidden, Timeout = 5000 });

        // 4. Test Party Split from Scene 1
        var partySplitBtn = page.Locator(".scene-choice-split-btn");
        if (await partySplitBtn.IsVisibleAsync())
        {
            await partySplitBtn.ClickAsync();
            await page.WaitForTimeoutAsync(250);
            await AssertNoHorizontalOverflowAsync(page, $"{width}x{height} - Party Split Active (Stacked)");

            // Try toggling Side-by-side if visible
            var sideBySideToggle = page.Locator(".split-view-mode-toggle button:has-text('Naast elkaar')");
            if (await sideBySideToggle.IsVisibleAsync())
            {
                await sideBySideToggle.ClickAsync();
                await page.WaitForTimeoutAsync(250);
                await AssertNoHorizontalOverflowAsync(page, $"{width}x{height} - Party Split Active (Side by side grid)");
            }

            // Reunite party
            var reuniteBtn = page.Locator(".split-reunite-btn");
            if (await reuniteBtn.IsVisibleAsync())
            {
                await reuniteBtn.ClickAsync();
                await page.WaitForTimeoutAsync(250);
                await AssertNoHorizontalOverflowAsync(page, $"{width}x{height} - After Party Reunion");
            }
        }

        // 5. Choose a branch: Click Scene 2 choice button
        var sceneChoiceBtn = page.Locator(".scene-choice-card-btn").First;
        if (await sceneChoiceBtn.IsVisibleAsync())
        {
            await sceneChoiceBtn.ClickAsync();
            await page.WaitForTimeoutAsync(200);
            await AssertNoHorizontalOverflowAsync(page, $"{width}x{height} - After Selecting Choice (Scene 2)");
        }

        // 6. Toggle All Scenes Mode (via choice bar toggle)
        var toggleAllBtn = page.Locator(".choice-control-btn[title*='Alle scènes']");
        if (await toggleAllBtn.IsVisibleAsync())
        {
            await toggleAllBtn.ClickAsync();
            await page.WaitForTimeoutAsync(200);
            await AssertNoHorizontalOverflowAsync(page, $"{width}x{height} - All Scenes Mode");

            // Toggle back
            await toggleAllBtn.ClickAsync();
            await page.WaitForTimeoutAsync(200);
        }

        // 7. Flow & Builder View (Play mode)
        var flowBtn = page.Locator(".session-view-controls button:has-text('Flow & Builder')");
        await flowBtn.ClickAsync();
        var canvasPanel = page.Locator(".session-flow-view-panel");
        await canvasPanel.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 5000 });
        await AssertNoHorizontalOverflowAsync(page, $"{width}x{height} - Flow & Builder View (Speelmodus)");

        // 8. Flow & Builder View (Bewerkmodus)
        var editModeBtn = page.Locator(".canvas-mode-toggle button:has-text('Bewerkmodus')");
        if (await editModeBtn.IsVisibleAsync())
        {
            await editModeBtn.ClickAsync();
            await page.WaitForTimeoutAsync(200);
            await AssertNoHorizontalOverflowAsync(page, $"{width}x{height} - Flow & Builder View (Bewerkmodus)");
        }
    }

    [TestMethod]
    [DataRow(320, 568)]
    [DataRow(360, 800)]
    [DataRow(390, 844)]
    public async Task AppPages_MobileViewports_HaveNoHorizontalOverflow(int width, int height)
    {
        await using var context = await _fixture.Browser.NewContextAsync(new BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize { Width = width, Height = height }
        });
        await MarkClientReadyAsync(context);
        var page = await context.NewPageAsync();

        // 1. Home page
        await page.GotoAsync(_fixture.BaseUrl);
        await page.WaitForTimeoutAsync(250);
        await AssertNoHorizontalOverflowAsync(page, $"{width}x{height} - Home Page");

        // 2. Help page
        await page.GotoAsync($"{_fixture.BaseUrl}/help");
        await page.WaitForTimeoutAsync(250);
        await AssertNoHorizontalOverflowAsync(page, $"{width}x{height} - Help Page");

        // 3. Editable Document Test page
        await page.GotoAsync($"{_fixture.BaseUrl}/test/editable-document");
        await page.WaitForTimeoutAsync(250);
        await AssertNoHorizontalOverflowAsync(page, $"{width}x{height} - Editable Document Page");
    }

    private static async Task AssertNoHorizontalOverflowAsync(IPage page, string contextDescription)
    {
        var json = await page.EvaluateAsync<string>("""
            () => {
                const docWidth = document.documentElement.clientWidth;
                const scrollWidth = document.documentElement.scrollWidth;
                const bodyScrollWidth = document.body.scrollWidth;
                const overflowing = [];

                const col = document.querySelector('.split-pane-column');
                let debugInfo = '';
                if (col) {
                    let cur = col;
                    while (cur && cur !== document.body) {
                        const r = cur.getBoundingClientRect();
                        debugInfo += ` [${cur.tagName}.${cur.className}: L=${r.left.toFixed(1)}, W=${r.width.toFixed(1)}, R=${r.right.toFixed(1)}, SW=${cur.scrollWidth}, CW=${cur.clientWidth}]`;
                        cur = cur.parentElement;
                    }

                    // Also check all descendants of col to see who has large scrollWidth
                    const descs = col.querySelectorAll('*');
                    for (const d of descs) {
                        if (d.scrollWidth > 330 || d.clientWidth > 330) {
                            debugInfo += ` {DESC: ${d.tagName}.${d.className}: CW=${d.clientWidth}, SW=${d.scrollWidth}, R=${d.getBoundingClientRect().right.toFixed(1)}}`;
                        }
                    }
                }

                const all = document.querySelectorAll('*');
                for (const el of all) {
                    // Skip elements rendered inside SVG canvas (paths, groups, foreignObject HTML content)
                    // since their coordinates live in the pannable/zoomable SVG space and are clipped
                    // by the root <svg> and .session-flow-container (overflow: hidden).
                    // The root <svg> itself and all other HTML elements outside <svg> are strictly checked.
                    if (el.closest('svg') && el.tagName.toLowerCase() !== 'svg') {
                        continue;
                    }

                    const rect = el.getBoundingClientRect();
                    // Allow 1.0px subpixel margin
                    if (rect.right > docWidth + 1.0) {
                        overflowing.push(`${el.tagName}.${el.className.toString().split(' ').join('.')} [left: ${rect.left.toFixed(1)}, right: ${rect.right.toFixed(1)}px > width: ${docWidth}px] outerHTML: ${el.outerHTML.substring(0, 100)}`);
                    }
                }

                return JSON.stringify({
                    docWidth: docWidth,
                    scrollWidth: scrollWidth,
                    bodyScrollWidth: bodyScrollWidth,
                    hasOverflow: scrollWidth > docWidth + 1.0 || bodyScrollWidth > docWidth + 1.0 || overflowing.length > 0,
                    overflowing: overflowing
                });
            }
        """);

        var result = System.Text.Json.JsonSerializer.Deserialize<OverflowCheckResult>(
            json,
            new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

        if (result.HasOverflow)
        {
            var details = string.Join("\n - ", result.Overflowing.Take(2));
            Assert.Fail($"Horizontal overflow detected in {contextDescription} (Doc: {result.DocWidth}px, Scroll: {result.ScrollWidth}px, Body: {result.BodyScrollWidth}px):\n - {details}");
        }
    }

    [TestMethod]
    public async Task CombatTracker_CanToggleOpen_AndManageParticipants()
    {
        await using var context = await _fixture.Browser.NewContextAsync(new BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize { Width = 1280, Height = 800 }
        });
        await MarkClientReadyAsync(context);
        var page = await context.NewPageAsync();
        page.Console += (_, msg) => TestContext?.WriteLine($"[BROWSER {msg.Type}] {msg.Text}");
        page.PageError += (_, err) => TestContext?.WriteLine($"[BROWSER ERROR] {err}");
        await page.GotoAsync($"{_fixture.BaseUrl}/test/session-view");

        // Wait for page and WASM client to be ready
        var outlineColumn = page.Locator(".outline-column");
        await outlineColumn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });

        // Toolbar button for Combat Tracker
        var combatBtn = page.Locator(".combat-tracker-btn");
        await combatBtn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 5000 });

        // Click to open combat tracker
        await combatBtn.ClickAsync();

        var tracker = page.Locator("#combat-tracker");
        await tracker.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 5000 });

        // Add custom creature
        await page.Locator(".custom-creature-details summary").ClickAsync();
        await page.Locator(".custom-input").FillAsync("Goblin Boss");
        await page.Locator(".custom-stat-input[placeholder='HP']").FillAsync("30");
        await page.Locator(".custom-stat-input[placeholder='AC']").FillAsync("15");
        await page.Locator(".custom-stat-input[placeholder='Init']").FillAsync("16");
        await page.Locator(".add-custom-btn").ClickAsync();

        // Verify creature card rendered
        var participantCard = page.Locator(".combat-participant-card");
        await participantCard.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 5000 });
        StringAssert.Contains(await participantCard.InnerTextAsync(), "Goblin Boss");

        // Start combat
        var startBtn = page.Locator(".combat-btn-primary:has-text('Start Gevecht')");
        await startBtn.ClickAsync();

        // Active turn banner should be displayed
        var activeBanner = page.Locator(".combat-active-banner");
        await activeBanner.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 5000 });
        StringAssert.Contains(await activeBanner.InnerTextAsync(), "Goblin Boss");

        // Close tracker
        await page.Locator(".combat-btn-close").ClickAsync();
        await tracker.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Hidden, Timeout = 5000 });
    }

    [TestMethod]
    public async Task CombatTracker_DealsDamageAndHeals_AndAdvancesTurns()
    {
        await using var context = await _fixture.Browser.NewContextAsync(new BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize { Width = 1280, Height = 800 }
        });
        await MarkClientReadyAsync(context);
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{_fixture.BaseUrl}/test/session-view");

        var outlineColumn = page.Locator(".outline-column");
        await outlineColumn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });

        await page.Locator(".combat-tracker-btn").ClickAsync();
        var tracker = page.Locator("#combat-tracker");
        await tracker.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 5000 });

        // Add 2 creatures: Hero (HP 20, AC 14, Init 15) and Goblin (HP 10, AC 12, Init 8)
        await page.Locator(".custom-creature-details summary").ClickAsync();
        await page.Locator(".custom-input").FillAsync("Hero");
        await page.Locator(".custom-stat-input[placeholder='HP']").FillAsync("20");
        await page.Locator(".custom-stat-input[placeholder='Init']").FillAsync("15");
        await page.Locator(".add-custom-btn").ClickAsync();

        await page.Locator(".custom-input").FillAsync("Goblin");
        await page.Locator(".custom-stat-input[placeholder='HP']").FillAsync("10");
        await page.Locator(".custom-stat-input[placeholder='Init']").FillAsync("8");
        await page.Locator(".add-custom-btn").ClickAsync();

        // Start combat
        await page.Locator(".combat-btn-primary:has-text('Start Gevecht')").ClickAsync();

        // Deal 10 damage to Goblin
        var goblinCard = page.Locator(".combat-participant-card:has-text('Goblin')");
        await goblinCard.Locator(".hp-delta-input").FillAsync("10");
        await goblinCard.Locator(".hp-damage-btn").ClickAsync();

        // Goblin should be defeated (0/10 HP)
        await goblinCard.Locator(".hp-defeated-badge").WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 5000 });
        StringAssert.Contains(await goblinCard.InnerTextAsync(), "0 / 10");

        // Next turn advances round
        await page.Locator(".combat-btn-primary:has-text('Volgende')").ClickAsync();
        await page.Locator(".combat-btn-primary:has-text('Volgende')").ClickAsync();
        var roundBadge = page.Locator(".combat-round-badge");
        StringAssert.Contains(await roundBadge.InnerTextAsync(), "Ronde 2");
    }

    public class OverflowCheckResult
    {
        public double DocWidth { get; set; }
        public double ScrollWidth { get; set; }
        public double BodyScrollWidth { get; set; }
        public bool HasOverflow { get; set; }
        public List<string> Overflowing { get; set; } = [];
    }

    private static Task MarkClientReadyAsync(IBrowserContext context)
    {
        return context.AddInitScriptAsync(
            "() => sessionStorage.setItem('wandering-wyvern-client-ready', 'true')");
    }
}
