using System.Net.Http.Headers;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;

namespace API;

public class OAuthEventHandler
{
    private readonly string _clientId;

    public OAuthEventHandler(string clientId)
    {
        _clientId = clientId;
    }
    
    public async Task HandleOnCreatingTicket(OAuthCreatingTicketContext context)
    {
        // context contains AccessToken and RefreshToken. Below is a sample use of these to get the user details from the helix users endpoint
        // This also sets the claims from user data so we have something to show on landing page (lists all available claims)
        
        using var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);
        request.Headers.Add("Client-Id", _clientId);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        using var response = await context.Backchannel.SendAsync(request, context.HttpContext.RequestAborted);
        response.EnsureSuccessStatusCode();

        using var user = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        context.RunClaimActions(user.RootElement.GetProperty("data")[0]);
    }
    
    public async Task HandleOnRemoteFailure(RemoteFailureContext context)
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "text/html";
        await context.Response.WriteAsync("<html><body>");
        await context.Response.WriteAsync("A remote failure has occurred: <br>" +
                                          context.Failure?.Message.Split(Environment.NewLine).Select(s => HtmlEncoder.Default.Encode(s) + "<br>")
                                              .Aggregate((s1, s2) => s1 + s2));

        if (context.Properties != null)
        {
            await context.Response.WriteAsync("Properties:<br>");
            foreach (var pair in context.Properties.Items)
            {
                await context.Response.WriteAsync($"-{HtmlEncoder.Default.Encode(pair.Key)}={HtmlEncoder.Default.Encode(pair.Value)}<br>");
            }
        }

        await context.Response.WriteAsync("<a href=\"/\">Home</a>");
        await context.Response.WriteAsync("</body></html>");
        context.HandleResponse();
    }
}