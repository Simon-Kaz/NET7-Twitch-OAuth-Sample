using System.Security.Claims;
using API;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication("cookie")
    .AddCookie("cookie")
    // You must first create an app with Twitch and add its ID and Secret to the appSettings.
    .AddOAuth("twitch", opt =>
    {
        opt.SignInScheme = "cookie";
        opt.ClientId = builder.Configuration["twitch:clientid"];
        opt.ClientSecret = builder.Configuration["twitch:clientsecret"];
        opt.AuthorizationEndpoint = "https://id.twitch.tv/oauth2/authorize";
        opt.TokenEndpoint = "https://id.twitch.tv/oauth2/token";
        opt.CallbackPath = "/oauth/twitch-cb";
        opt.UserInformationEndpoint = "https://api.twitch.tv/helix/users";
        opt.SaveTokens = true;

        // Mapping json response values to claims so we can fetch these when listing claims on the landing page  
        opt.ClaimActions.MapJsonKey("sub", "id");
        opt.ClaimActions.MapJsonKey(ClaimTypes.Name, "login");
        opt.ClaimActions.MapJsonKey("display_name", "display_name");
        opt.ClaimActions.MapJsonKey("avatar", "profile_image_url");

        var eventHandler = new OAuthEventHandler(builder.Configuration["twitch:clientid"]);
        opt.Events = new OAuthEvents
        {
            OnRemoteFailure = eventHandler.HandleOnRemoteFailure,
            OnCreatingTicket = eventHandler.HandleOnCreatingTicket
        };
    });

var app = builder.Build();

app.UseAuthentication();

app.MapGet("/", (HttpContext context) => { return context.User.Claims.Select(x => new {x.Type, x.Value}).ToList(); });

app.MapGet("/login", () => Results.Challenge(new AuthenticationProperties {RedirectUri = "https://localhost:7179"}, new List<string> {"twitch"}));

app.Run();