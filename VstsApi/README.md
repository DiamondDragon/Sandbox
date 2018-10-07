# Show work item console app

Simple [.NET Core](https://docs.microsoft.com/dotnet/core/) console app that uses [Microsoft.TeamFoundationServer.Client](https://www.nuget.org/packages/Microsoft.TeamFoundationServer.Client) to retrieve details about a Visual Studio Team Services work item.

## How to run

1. If you do not already have a Team Services account, [create one](https://www.visualstudio.com/docs/setup-admin/team-services/sign-up-for-visual-studio-team-services) (it's free)
2. Create a work item in your account
3. Create a personal access token ([steps](https://www.visualstudio.com/docs/setup-admin/team-services/use-personal-access-tokens-to-authenticate))
4. Install [.NET Core](https://microsoft.com/net/core) command line tools
5. Run `dotnet restore`
6. Run `dotnet run https://youraccount.visualstudio.com yourtoken 1`
