# Twitch.tv OAuth Implementation using .NET 7 Web API - no dependencies

This project showcases a simple implementation of Twitch.tv OAuth using .NET 7 Web API using no dependencies.
The implementation allows users to log in to the application using their Twitch.tv credentials, and then retrieve their basic profile information using Twitch.tv API.
It uses cookies as the default authentication scheme.

## Getting Started

To get started with the project, clone the repository and open the solution in an IDE.

Make sure to create a Twitch.tv application in the developer portal. Set the callback url to ```https://localhost:7179/oauth/twitch-cb``` and obtain client ID and client secret, which will be used for the OAuth implementation.   

Update the `appsettings.json` file with your Twitch.tv client ID and client secret.

```json 
"twitch": {
    "clientid": "YOUR_CLIENT_ID",
    "clientsecret": "YOUR_CLIENT_SECRET"
  }
```

Once you have updated the appsettings.json file, build and run the application.

## How to Use

Once the project is running, open your browser and navigate to `https://localhost:7179/login` - This will start the Twitch.tv OAuth authentication flow.

Once you have successfully logged in and authorised the application, you will be redirected to the localhost:7179 landing page where you can view your basic Twitch.tv profile information as it was extracted from the Users endpoint (via the `OnCreatingTicket` event handler).

Technologies Used

    .NET 7 Web API
    Twitch.tv API