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

        public bool _hasMoreIssues;
        public int _issueOffset;

        public BitBucketRest(string username, string password)
        {
            _username = username;
            _password = password;
            _hasMoreIssues = false;
            _issueOffset = 0;
        }

        public T Execute<T>(RestRequest request) where T : new()
        {
            var client = new RestClient();
            client.BaseUrl = baseUrl;
            client.Authenticator = new HttpBasicAuthenticator(_username, _password);
            //client.
            var response = client.Execute<T>(request);
        
            return response.Data;
        }

        public List<Repository> GetRepos()
        {
            var request = new RestRequest();
            request.Resource = "user/repositories/";
            return Execute<List<Repository>>(request);
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

            if (comment != null && toPostIssue != null)
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


        public List<Milestone> GetMilestones(string repo_slug)
        {
            var request = new RestRequest();
            request.Resource = "repositories/" + _username + "/" + repo_slug + "/issues/milestones";
            return Execute<List<Milestone>>(request);
        }

        public List<Issue> GetIssues(string repo)
        {
            var request = new RestRequest();
            request.Resource = "repositories/" + _username + "/" + repo + "/issues/?limit=50&start="+_issueOffset.ToString() ;
            IssueQuery issueQueryResults = new IssueQuery();
            issueQueryResults  = Execute<IssueQuery>(request);
            if (issueQueryResults.count > 50)
            {
                _hasMoreIssues = true;
                _issueOffset = _issueOffset + 1;
            }
            else
            {
                _hasMoreIssues = false;
                _issueOffset = 0;
            }
            return issueQueryResults.issues;
        }

        public List<Comments> GetComments(string repo_slug, int issueId)
        {
            var request = new RestRequest();
            request.Resource = "repositories/" + _username +"/" + repo_slug + "/issues/" + issueId.ToString() + "/comments";
            return Execute<List<Comments>>(request);
        }
    }
}


