using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Git2Bit.BitModels
{
    public class Repository
    {
        public string scm { get; set; }
        public bool has_wiki { get; set; }
        public string last_updated { get; set; }
        public object creator { get; set; }
        public string created_on { get; set; }
        public string owner { get; set; }
        public string logo { get; set; }
        public string email_mailinglist { get; set; }
        public bool is_mq { get; set; }
        public int size { get; set; }
        public bool read_only { get; set; }
        public object fork_of { get; set; }
        public object mq_of { get; set; }
        public int followers_count { get; set; }
        public string state { get; set; }
        public string utc_created_on { get; set; }
        public string website { get; set; }
        public string description { get; set; }
        public bool has_issues { get; set; }
        public bool is_fork { get; set; }
        public string slug { get; set; }
        public bool is_private { get; set; }
        public string name { get; set; }
        public string language { get; set; }
        public string utc_last_updated { get; set; }
        public bool email_writers { get; set; }
        public bool no_public_forks { get; set; }
        public string resource_uri { get; set; }
    }

    public class Milestone
    {
        public string name { get; set; }
        public int id { get; set; }
    }

    public class User
    {
        public string username { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public bool is_team { get; set; }
        public string avatar { get; set; }
        public string resource_uri { get; set; }
    }

    public class Metadata
    {
        public string kind { get; set; }
        public object version { get; set; }
        public object component { get; set; }
        public object milestone { get; set; }
    }

    public class Issue
    {
        public string status { get; set; }
        public string priority { get; set; }
        public string title { get; set; }
        public User reported_by { get; set; }
        public string utc_last_updated { get; set; }
        public User responsible { get; set; }
        public int comment_count { get; set; }
        public Metadata metadata { get; set; }
        public string content { get; set; }
        public string created_on { get; set; }
        public int local_id { get; set; }
        public int follower_count { get; set; }
        public string utc_created_on { get; set; }
        public string resource_uri { get; set; }
        public bool is_spam { get; set; }

    }

    public class Comments
    {
        public string content { get; set; }
        public User author_info { get; set; }
        public int comment_id { get; set; }
        public string utc_updated_on { get; set; }
        public string utc_created_on { get; set; }
        public bool is_spam { get; set; }
    }

}
