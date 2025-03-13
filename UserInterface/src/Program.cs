using ServiceDefaults;
using UserInterface.Components;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// TODO: Add a service to detect code smells.
// builder.Services.AddSingleton<CodeSmellDetectionService>();

// TODO: Add upload file functionality.
// builder.Services.AddSingleton<UploadFileService>();

// TODO: Add a service to handle the API calls to Azure OpenAI.
// builder.Services.AddSingleton<AzureOpenAIService>();
var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    _ = app.UseExceptionHandler("/Error", createScopeForErrors: true);

    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    _ = app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
