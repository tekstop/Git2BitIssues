﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Git2Bit.GitModels
{
    public class User
    {
        public string type { get; set; }
        public string received_events_url { get; set; }
        public string url { get; set; }
        public string followers_url { get; set; }
        public string avatar_url { get; set; }
        public string following_url { get; set; }
        public string login { get; set; }
        public string subscriptions_url { get; set; }
        public string starred_url { get; set; }
        public string repos_url { get; set; }
        public string organizations_url { get; set; }
        public string events_url { get; set; }
        public string gists_url { get; set; }
        public string gravatar_id { get; set; }
        public int id { get; set; }
    }

    public class Permissions
    {
        public bool admin { get; set; }
        public bool push { get; set; }
        public bool pull { get; set; }
    }
    
    public class Repository
    {
        public string trees_url { get; set; }
        public string url { get; set; }
        public int watchers { get; set; }
        public string merges_url { get; set; }
        public string assignees_url { get; set; }
        public int forks_count { get; set; }
        public string comments_url { get; set; }
        public string statuses_url { get; set; }
        public string keys_url { get; set; }
        public Permissions permissions { get; set; }
        public string updated_at { get; set; }
        public string archive_url { get; set; }
        public string branches_url { get; set; }
        public int open_issues { get; set; }
        public string clone_url { get; set; }
        public bool has_downloads { get; set; }
        public string subscribers_url { get; set; }
        public string languages_url { get; set; }
        public string tags_url { get; set; }
        public string collaborators_url { get; set; }
        public string full_name { get; set; }
        public object mirror_url { get; set; }
        public string pulls_url { get; set; }
        public string issues_url { get; set; }
        public string blobs_url { get; set; }
        public string ssh_url { get; set; }
        public string homepage { get; set; }
        public string milestones_url { get; set; }
        public string subscription_url { get; set; }
        public string teams_url { get; set; }
        public string created_at { get; set; }
        public string git_url { get; set; }
        public int size { get; set; }
        public string notifications_url { get; set; }
        public string git_tags_url { get; set; }
        public bool fork { get; set; }
        public string html_url { get; set; }
        public string svn_url { get; set; }
        public bool has_wiki { get; set; }
        public int open_issues_count { get; set; }
        public string compare_url { get; set; }
        public string issue_comment_url { get; set; }
        public string git_commits_url { get; set; }
        public string hooks_url { get; set; }
        public string name { get; set; }
        public string pushed_at { get; set; }
        public int watchers_count { get; set; }
        public string commits_url { get; set; }
        public string stargazers_url { get; set; }
        public string git_refs_url { get; set; }
        public string events_url { get; set; }
        public string forks_url { get; set; }
        public string description { get; set; }
        public bool @private { get; set; }
        public int id { get; set; }
        public int forks { get; set; }
        public string language { get; set; }
        public bool has_issues { get; set; }
        public string labels_url { get; set; }
        public string downloads_url { get; set; }
        public string contents_url { get; set; }
        public string contributors_url { get; set; }
        public string issue_events_url { get; set; }
    }

    public class Milestone
    {
        public string due_on { get; set; }
        public string url { get; set; }
        public string state { get; set; }
        //public Creator creator { get; set; }
        public string title { get; set; }
        public int number { get; set; }
        public int open_issues { get; set; }
        public string updated_at { get; set; }
        public int closed_issues { get; set; }
        public string created_at { get; set; }
        public string description { get; set; }
        public int id { get; set; }
        public string labels_url { get; set; }
    }

    public class PullRequest
    {
        public object html_url { get; set; }
        public object patch_url { get; set; }
        public object diff_url { get; set; }
    }

    public class Label
    {
        public string url { get; set; }
        public string color { get; set; }
        public string name { get; set; }
    }

    
    public class Issue
    {
        public PullRequest pull_request { get; set; }
        public string url { get; set; }
        public string state { get; set; }
        public List<Label> labels { get; set; }
        public string title { get; set; }
        public int number { get; set; }
        public User assignee { get; set; } 
        public string events_url { get; set; }
        public string comments_url { get; set; }
        public string updated_at { get; set; }
        public User user { get; set; }
        public string html_url { get; set; }
        public int comments { get; set; }
        public string closed_at { get; set; }
        public string labels_url { get; set; }
        public string created_at { get; set; }
        public Milestone milestone { get; set; }
        public int id { get; set; }
        public string body { get; set; }
    }
    

    public class IssuePost
    {
        public List<string> labels { get; set; }
        public string title { get; set; }
        public string assignee { get; set; }
        public string body { get; set; }
        public string state { get; set; }
    }

    /*
    {
  "title": "Found a bug",
  "body": "I'm having a problem with this.",
  "assignee": "octocat",
  "milestone": 1,
  "labels": [
    "Label1",
    "Label2"
  ]
    }
    */

    public class Comments
    {
        public string url { get; set; }
        public string updated_at { get; set; }
        public User user { get; set; }
        public string created_at { get; set; }
        public int id { get; set; }
        public string body { get; set; }
        public string issue_url { get; set; }
    }
}
