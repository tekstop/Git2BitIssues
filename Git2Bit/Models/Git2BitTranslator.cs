using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Git2Bit.BitModels
{
    class Git2BitTranslator
    {
        public static Git2Bit.BitModels.Milestone translate(Git2Bit.GitModels.Milestone gitMilestone)
        {
            Git2Bit.BitModels.Milestone milestone = new Milestone();
            milestone.name = gitMilestone.title;
            return milestone;
        }

        public static Git2Bit.BitModels.Issue translate(Git2Bit.GitModels.Issue gitIssue)
        {
            Git2Bit.BitModels.Issue issue = new Issue();
            
            // Status
            if (gitIssue.state.Equals("open"))
            {
                issue.status = "open";
            }
            else
            {
                issue.status = "resolved";
            }

            // Priority -> Defaults to Major
            issue.priority = "major";

            // title
            issue.title = gitIssue.title;

            // assignee
            if (gitIssue.assignee != null)
            {
                issue.responsible.username = gitIssue.assignee.login;
            }

            // map bug and enhancement labels
            if (gitIssue.labels != null)
            {
                foreach (Git2Bit.GitModels.Label alabel in gitIssue.labels)
                {
                    if (alabel.name.Equals("enhancement"))
                    {
                        issue.metadata.kind = "enhancement";
                        break;
                    }
                    else if (alabel.name.Equals("bug"))
                    {
                        issue.metadata.kind = "bug";
                        break;
                    }
                    else if (alabel.name.Equals("task"))
                    {
                        issue.metadata.kind = "task";
                        break;
                    }
                }
            }

            // milesetone
            if (gitIssue.milestone != null)
            {
                issue.metadata.milestone = gitIssue.milestone.title;
            }

            return issue;

        }
    }
}
