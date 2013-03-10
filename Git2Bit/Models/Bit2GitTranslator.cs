using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Git2Bit.GitModels
{
    class Bit2GitTranslator
    {
        public static Git2Bit.GitModels.Milestone translate(Git2Bit.BitModels.Milestone bitMilestone)
        {
            Git2Bit.GitModels.Milestone milestone = new Milestone();
            milestone.title = bitMilestone.name;
            return milestone;
        }

        public static Git2Bit.GitModels.IssuePost translate(Git2Bit.BitModels.Issue bitIssue)
        {
            Git2Bit.GitModels.IssuePost issue = new IssuePost();

            // Status
            if (bitIssue.status.Equals("open"))
            {
                issue.state = "open";
            }
            else
            {
                issue.state = "closed";
            }

            // title
            issue.title = bitIssue.title;

            //issue type(bug,enhancement etc)
            issue.labels = new List<string>();
            issue.labels.Add(bitIssue.metadata.kind);

            issue.assignee = bitIssue.responsible != null ? bitIssue.responsible.display_name : null ;

            issue.body = bitIssue.content;
            return issue;

        }

        public static Git2Bit.GitModels.Comments translate(Git2Bit.BitModels.Comments bitComment)
        {
            Git2Bit.GitModels.Comments comment = new Comments();
            // Unfortunately only the user whos is porting gets accredited with the comment.
            // Wrapping original Comment information inside gitComment Content.
            comment.body = "Originally Posted By:" + bitComment.author_info.display_name + " on " + bitComment.utc_created_on + "\n\n" + bitComment.content;
            return comment;
        }


    }
}
