using DocumentExplorer.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Syncfusion.Blazor;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json.Serialization;
using System.IO;
using Syncfusion.Licensing;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddServerSideBlazor().AddHubOptions(o => { o.MaximumReceiveMessageSize = 102400000; });
builder.Services.AddSyncfusionBlazor();
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient();
builder.Services.AddSession();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAllOrigins", builder =>
        {
            builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
        });
    });
builder.Services.AddControllers().AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ContractResolver = new DefaultContractResolver();
    });
if (File.Exists(Directory.GetCurrentDirectory() + "/SyncfusionLicense.txt"))
{
    string licenseKey = System.IO.File.ReadAllText(Directory.GetCurrentDirectory() + "/SyncfusionLicense.txt");
    SyncfusionLicenseProvider.RegisterLicense(licenseKey);
}
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseSession();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
app.MapControllers();

app.MapDefaultControllerRoute();
//app.MapBlazorHub();
app.UseCors();

app.Run();
