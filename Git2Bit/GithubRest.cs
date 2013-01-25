using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RestSharp;
using Git2Bit.Models;
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

       
    }
}
