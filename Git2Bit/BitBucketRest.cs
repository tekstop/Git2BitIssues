using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RestSharp;
using Git2Bit.BitModels;
namespace Git2Bit
{
    class BitBucketRest
    {
        const string baseUrl = "https://api.bitbucket.org/1.0/";

        private string _username;
        private string _password;

        public BitBucketRest(string username, string password)
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
            request.Resource = "user/repositories/";
            return Execute<List<Repository>>(request);
        }

        public Issue PostIssue(string repo_slug, Git2Bit.GitModels.Issue gitIssue)
        {
            var request = new RestRequest();
            request.Resource = "repositories/" + _username + "/" + repo_slug + "/issues/";
            request.Method = Method.POST;
            
            // Convert Issue:
            BitModels.Issue toPostIssue = Git2Bit.BitModels.Git2BitTranslator::translate(gitIssue);
            
            request.AddParameter("status",toPostIssue.status,ParameterType.GetOrPost);
            request.AddParameter("priority",toPostIssue.priority,ParameterType.GetOrPost);
            request.AddParameter("title",toPostIssue.title,ParameterType.GetOrPost);


        }


        public List<Milestone> GetMilestones(string repo, bool open = true)
        {
            var request = new RestRequest();
            request.Resource = "repos/" + repo + "/milestones";
            if (!open)
            {
                request.AddParameter("state", "closed", ParameterType.GetOrPost);
            }
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


