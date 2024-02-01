using Test.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents();

//Adds localization services (probably not needed for our approach)
builder.Services.AddLocalization();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error", createScopeForErrors: true);
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

/* This middleware help us to understand the problem
 * Every request happens with the correct accepted language
 * expect of the last one with the Path /_blazor.
 * The last one has "en-US,en;q=0.9,de;q=0.8"
 */
app.Use((ctx, next) =>
{
	var logger = ctx.RequestServices.GetRequiredService<ILogger<Program>>();
	logger.LogInformation("Request Path: {Request}\n\tAccepted Language: {Header}", ctx.Request.Path,
		ctx.Request.Headers.AcceptLanguage);
	return next(ctx);
});

//Use request localization (--> locale in browser)
app.UseRequestLocalization(["en-US", "de-DE"]);


app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode();

app.Run();