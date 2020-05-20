using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using LibGit2Sharp;
using Microsoft.TeamFoundation.SourceControl.WebApi;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var organization = "diamond-dragon";

            var personalAccessToken = "wvlwkkk2fveptkxmtcz55wo3v5zxbcrj6bryr7sd3fybpuotubla";
            //var repositoryUrl = $"https://{organization}.visualstudio.com";
            var repositoryUrl = $"https://dev.azure.com/{organization}";

            GetRepositories2(repositoryUrl, personalAccessToken).Wait();
            Clone(personalAccessToken, "https://diamond-dragon.visualstudio.com/_git/StoryLine");
        }

        /// <summary>
        /// See https://docs.microsoft.com/en-us/rest/api/vsts/
        /// </summary>
        /// <param name="repositoryUrl"></param>
        /// <param name="personarAccessToken"></param>
        /// <returns></returns>
        private static async Task GetRepositories2(string repositoryUrl, string personarAccessToken)
        {

                try
                {
                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Accept.Add(
                            new MediaTypeWithQualityHeaderValue("application/json"));

                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                            Convert.ToBase64String(
                                System.Text.Encoding.ASCII.GetBytes(
                                    string.Format("{0}:{1}", "", personarAccessToken))));

                        using (var response = await client.GetAsync(repositoryUrl + "/DefaultCollection/_apis/projects"))
                        {
                            response.EnsureSuccessStatusCode();
                            string responseBody = await response.Content.ReadAsStringAsync();
                            Console.WriteLine(responseBody);
                        }


                        using (var response = await client.GetAsync(repositoryUrl + "/0c54ceac-18bc-4353-98ef-582eb436028f/_apis/git/repositories?api-version=4.1-preview"))
                        {
                            response.EnsureSuccessStatusCode();
                            string responseBody = await response.Content.ReadAsStringAsync();
                            Console.WriteLine(responseBody);
                        }

                    // id: "240a742c-61ac-4d24-b9a3-56ca791f8070"
                    // remoteUrl: "https://diamond-dragon.visualstudio.com/StoryLine/_git/StoryLine.Rest"
                }
            }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
        }
        private static void Clone(string tokem, string repositoryUrl)
        {
            var cloneOptions = new CloneOptions
            {
                CredentialsProvider = (url, user, cred) => new UsernamePasswordCredentials
                {
                    Username = tokem,
                    Password = string.Empty
                }
            };

            var sourcesFolder = @"d:\Rie\test";


            Repository.Clone(repositoryUrl, sourcesFolder, cloneOptions);


            using (var repository = new Repository(sourcesFolder))
            {
                var branchName = "origin/master";
                var branch = repository.Branches[branchName];

                if (branch == null)
                    return;

                var localBranchName = "master";

                var localbranch = repository.Branches[localBranchName];

                if (localbranch == null)
                {
                    localbranch = repository.CreateBranch(localBranchName, branch.Tip);

                    var updatedBranch = repository.Branches.Update(localbranch,
                        b =>
                        {
                            b.TrackedBranch = branch.CanonicalName;
                        });

                    LibGit2Sharp.Commands.Checkout(repository, updatedBranch);
                }
                else
                {
                    LibGit2Sharp.Commands.Checkout(repository, localbranch);
                }
            }
        }
    }
}