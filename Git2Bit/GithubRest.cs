﻿using System;
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
            if (199 < (int)response.StatusCode && (int)response.StatusCode < 209)
            {
                //success
                return response.Data;
            }
            else
            {
                //error
                if (response.ErrorException != null)
                {
                    throw response.ErrorException;
                }
                else
                {
                    throw new Exception(string.Format("{0}({1})",response.StatusDescription,(int)response.StatusCode));
                }
            }
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

        public void PostIssue(string repo_slug, Git2Bit.BitModels.Issue bitIssue, List<Git2Bit.BitModels.Comments> bitComments,out string raw)
        {
            string cnt = string.Empty;
            var request = new RestRequest();
            request.Resource = "repos/" + repo_slug + "/issues";
            request.Method = Method.POST;
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("Accept", "application/json");
            request.OnBeforeDeserialization = resp => { cnt = resp.Content; };
            
            GitModels.IssuePost toPostIssue = Git2Bit.GitModels.Bit2GitTranslator.translate(bitIssue);

            request.AddBody(toPostIssue);
            /*
            request.AddParameter("assignee", toPostIssue.assignee, ParameterType.GetOrPost);
            request.AddParameter("milestone", toPostIssue.milestone, ParameterType.GetOrPost);
            
            request.AddParameter("state", toPostIssue.state, ParameterType.GetOrPost);
            //request.AddParameter("priority", toPostIssue.priority, ParameterType.GetOrPost);
            request.AddParameter("title", toPostIssue.title, ParameterType.GetOrPost);
            //request.AddParameter("responsible", toPostIssue.responsible.username, ParameterType.GetOrPost);
            request.AddParameter("body", toPostIssue.body, ParameterType.GetOrPost);
            //request.AddParameter("kind", toPostIssue.metadata.kind, ParameterType.GetOrPost);
            //request.AddParameter("milestone", toPostIssue.metadata.milestone, ParameterType.GetOrPost);
             */
            Execute<IssuePost>(request);
            raw = cnt;
        }

        public List<Comments> GetComments(string repo, int issueId)
        {
            var request = new RestRequest();
            request.Resource = "repos/" + repo + "/issues/" + issueId.ToString() + "/comments";
            return Execute<List<Comments>>(request);
        }
    }
}
