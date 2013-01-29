using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RestSharp;
using Git2Bit.GitModels;
namespace Git2Bit
{
    public class GithubRest
    {
        const string baseUrl = "https://api.github.com/";

        private string _username;
        private string _password;

        public GithubRest(string username, string password)
        {
            _username = username;
            _password = password;
        }

        public T Execute<T>(RestRequest request) where T : new()
        {
            var client = new RestClient();
            client.BaseUrl = baseUrl;
            client.Authenticator = new HttpBasicAuthenticator(_username, _password);
            var response = client.Execute<T>(request);
            if (response.ErrorException != null)
            {
                throw response.ErrorException;
            }
            return response.Data;
        }

        public List<Repository> GetRepos()
        {
            var request = new RestRequest();
            request.Resource = "user/repos";
            request.AddParameter("type", "owner", ParameterType.GetOrPost);
            return Execute<List<Repository>>(request);
        }

        public List<Milestone> GetMilestones(string repo, bool open = true)
        {
            var request = new RestRequest();
            request.Resource = "repos/" + repo + "/milestones";
            if (!open)
            {
                request.AddParameter("state", "closed", ParameterType.GetOrPost);
            }
            request.AddParameter("direction", "asc", ParameterType.GetOrPost);
            return Execute<List<Milestone>>(request);
        }

        public List<Issue> GetIssues(string repo, bool open = true)
        {
            var request = new RestRequest();
            request.Resource = "repos/" + repo + "/issues";
            if (!open)
            {
                request.AddParameter("state", "closed", ParameterType.GetOrPost);
            }
            request.AddParameter("direction", "asc", ParameterType.GetOrPost);
            return Execute<List<Issue>>(request);
        }

        public List<Comments> GetComments(string repo, int issueId)
        {
            var request = new RestRequest();
            request.Resource = "repos/" + repo + "/issues/" + issueId.ToString() + "/comments";
            return Execute<List<Comments>>(request);
        }
    }
}
