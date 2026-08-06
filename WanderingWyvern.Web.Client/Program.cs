using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using DhrMaes.WanderingWyvern.Core.Services;
using DhrMaes.WanderingWyvern.Web.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped<BrowserCampaignContentStore>();
builder.Services.AddScoped<ICampaignContentStore>(sp => sp.GetRequiredService<BrowserCampaignContentStore>());
builder.Services.AddScoped<CampaignMarkdownRenderer>();
builder.Services.AddScoped<CampaignClientState>();

await builder.Build().RunAsync();
