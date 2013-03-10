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
            //client.
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

        public List<Issue> GetIssues(string repo,out string content)
        {
            string cnt = string.Empty;
            var request = new RestRequest();
            request.Resource = string.Format("repositories/{0}/{1}/issues/?start={2}",_username,repo,0);
            request.OnBeforeDeserialization = resp => { cnt = resp.Content; };

            var results = Execute<IssueSearchResults>(request);
            content = cnt;

            return results.issues;
        }

        public Issue PostIssue(string repo_slug, Git2Bit.GitModels.Issue gitIssue, List<Git2Bit.GitModels.Comments> comment = null)
        {
            var request = new RestRequest();
            request.Resource = "repositories/" + _username + "/" + repo_slug + "/issues/";
            request.Method = Method.POST;
            
            // Convert Issue:
            BitModels.Issue toPostIssue = Git2Bit.BitModels.Git2BitTranslator.translate(gitIssue);
            
            request.AddParameter("status",toPostIssue.status,ParameterType.GetOrPost);
            request.AddParameter("priority",toPostIssue.priority,ParameterType.GetOrPost);
            request.AddParameter("title",toPostIssue.title,ParameterType.GetOrPost);
            request.AddParameter("responsible", toPostIssue.responsible.username, ParameterType.GetOrPost);
            request.AddParameter("content", toPostIssue.content, ParameterType.GetOrPost);
            request.AddParameter("kind", toPostIssue.metadata.kind, ParameterType.GetOrPost);
            request.AddParameter("milestone", toPostIssue.metadata.milestone, ParameterType.GetOrPost);
            toPostIssue =  Execute<Issue>(request);

            if (comment != null)
            {
                // Insert all the Comments
                foreach (Git2Bit.GitModels.Comments comm in comment)
                {
                    BitModels.Comments aComm = Git2Bit.BitModels.Git2BitTranslator.translate(comm);
                    var request1 = new RestRequest();
                    request1.Resource = "repositories/" + _username + "/" + repo_slug + "/issues/" + toPostIssue.local_id.ToString() + "/comments";
                    request1.Method = Method.POST;
                    request1.AddParameter("content", aComm.content, ParameterType.GetOrPost);
                    aComm = Execute<Comments>(request1);
                }
            }
            return toPostIssue;


        }

        public Milestone PostMilestone(string repo_slug, Git2Bit.GitModels.Milestone gitMilestone)
        {
            var request = new RestRequest();
            request.Resource = "repositories/" + _username + "/" + repo_slug + "/issues/milestones";
            request.Method = Method.POST;
            Milestone bitMilestone = Git2Bit.BitModels.Git2BitTranslator.translate(gitMilestone);
            request.AddParameter("name", bitMilestone.name, ParameterType.GetOrPost);
            return Execute<Milestone>(request);
        }


        /*public List<Milestone> GetMilestones(string repo, bool open = true)
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
        }*/
    }
}


