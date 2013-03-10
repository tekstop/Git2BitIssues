﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using RestSharp;
namespace Git2Bit
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // GIT STUFF
        public List<Git2Bit.GitModels.Repository> GitRepos;
        public List<Git2Bit.GitModels.Milestone> openGitMilestones;
        public List<Git2Bit.GitModels.Milestone> closedGitMilestones;
        public List<Git2Bit.GitModels.Issue> openGitIssues;
        public List<Git2Bit.GitModels.Issue> closedGitIssues;
        public string selectedGitRepositiry;
        public Dictionary<int,List<Git2Bit.GitModels.Comments>> gitComments;

        // BIT STUFF
        public List<Git2Bit.BitModels.Repository> BitRepos;
        private List<Git2Bit.BitModels.Issue> _bitIssues;
        public List<Git2Bit.BitModels.Issue> BitIssues
        {
            set
            {
                if(!string.IsNullOrEmpty(selectedGitRepositiry) && value.Count > 0)
                    portBitIssuesToGit.IsEnabled = true;
                else
                    portBitIssuesToGit.IsEnabled = false;

                _bitIssues = value;
            }
            get
            {
                return _bitIssues;
            }
        }
        public string selectedBitRepository;
      
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        
        private void gitRepos_Click(object sender, RoutedEventArgs e)
        {
            GithubRest git = new GithubRest(gitUsername.Text, gitPassword.Password);
            GitRepos = git.GetRepos();
            
            // Create a list of Repos that have issues
            List<string> repos_names = new List<string>();
            foreach (Git2Bit.GitModels.Repository repo in GitRepos)
            {
                if(repo.has_issues) {
                    repos_names.Add(repo.full_name);
                }
            }
             
            gitRepositories.ItemsSource = repos_names;
        }

        private void bitRepos_Click(object sender, RoutedEventArgs e)
        {
            BitBucketRest bit = new BitBucketRest(bitUsername.Text, bitPassword.Password);
            BitRepos = bit.GetRepos();

            // Create a list of Repos that have issues
            List<string> repos_names = new List<string>();
            foreach (Git2Bit.BitModels.Repository repo in BitRepos)
            {
                if (repo.has_issues)
                {
                    repos_names.Add(repo.slug);
                }
            }

            bitRepositories.ItemsSource = repos_names;
        }


        private void bitGetIssuesButton_Click(object sender, RoutedEventArgs e)
        {
            BitBucketRest bit = new BitBucketRest(bitUsername.Text,bitPassword.Password);
            string cnt = string.Empty; 
            BitIssues = bit.GetIssues(selectedBitRepository,out cnt);

#if DEBUG
            logger.AppendText("raw response: " + cnt);
#endif

            foreach (Git2Bit.BitModels.Issue issue in BitIssues)
            {
                    logger.AppendText("Issue: " + issue.content.ToString() + Environment.NewLine);
            }
        }

        private void gitGetIssuesButton_Click(object sender, RoutedEventArgs e)
        {
            GithubRest git = new GithubRest(gitUsername.Text, gitPassword.Password);
            gitComments = new Dictionary<int, List<GitModels.Comments>>();
            openGitMilestones = git.GetMilestones(selectedGitRepositiry);
            logger.AppendText("The " + selectedGitRepositiry + " has " + openGitMilestones.Count.ToString() + " open milestones.\n");
            closedGitMilestones = git.GetMilestones(selectedGitRepositiry, false);
            logger.AppendText("The " + selectedGitRepositiry + " has " + closedGitMilestones.Count.ToString() + " closed milestones.\n");
            openGitIssues = git.GetIssues(selectedGitRepositiry);
            logger.AppendText("The " + selectedGitRepositiry + " has " + openGitIssues.Count.ToString() + " open issues.\n");
            closedGitIssues = git.GetIssues(selectedGitRepositiry, false);
            logger.AppendText("The " + selectedGitRepositiry + " has " + closedGitIssues.Count.ToString() + " closed issues.\n");

            // Get individual Comments for each open issue
            foreach (Git2Bit.GitModels.Issue issue in openGitIssues)
            {
                if (issue.comments > 0)
                {
                    // has comments:
                    List<Git2Bit.GitModels.Comments> comments = git.GetComments(selectedGitRepositiry, issue.number);
                    logger.AppendText("Issue " + issue.number.ToString() + " has " + comments.Count.ToString() + " comments.\n");
                    gitComments[issue.number] = comments;
                }
            }

            // Get individual Comments for each open issue
            foreach (Git2Bit.GitModels.Issue issue in closedGitIssues)
            {
                if (issue.comments > 0)
                {
                    // has comments:
                    List<Git2Bit.GitModels.Comments> comments = git.GetComments(selectedGitRepositiry, issue.number);
                    logger.AppendText("Issue " + issue.number.ToString() + " has " + comments.Count.ToString() + " comments.\n");
                    gitComments[issue.number] = comments;
                }
            }


            gitIssuesButton.IsEnabled = false;
        }

        private void gitRepositories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Enable gitIssueButton_Click with the selected repository
            gitIssuesButton.IsEnabled = true;
            selectedGitRepositiry = (string)gitRepositories.SelectedItem;
        }

        private void bitRepositories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            portGitIssues.IsEnabled = true;
            bitGetIssuesButton.IsEnabled = true;
            selectedBitRepository = (string)bitRepositories.SelectedItem;
        }

        private void portBitIssuesToGit_Click(object sender, RoutedEventArgs e)
        {
            GithubRest git = new GithubRest(gitUsername.Text, gitPassword.Password);
            string raw = string.Empty;

            foreach (var bitIssue in BitIssues)
            {
                try
                {
                    git.PostIssue(selectedGitRepositiry, bitIssue,null, out raw);
#if DEBUG
                    logger.AppendText("raw response portBitIssuesToGit : " + raw + Environment.NewLine);
#endif
                    logger.AppendText(string.Format("Ported bit issue {0} to git", bitIssue.title) + Environment.NewLine);
                }
                catch (Exception ex)
                {
                    logger.AppendText(string.Format("An error occured while trying to migrate issue {0}. Error: {1}." + Environment.NewLine, bitIssue.title, ex.Message));
                }
            }



        }

        private void portGitIssuesToBit_Click(object sender, RoutedEventArgs e)
        {
            BitBucketRest bit = new BitBucketRest(bitUsername.Text, bitPassword.Password);
            if (closedGitMilestones != null && closedGitMilestones.Count > 0)
            {
                logger.AppendText("This repo has closed Milestones. Porting them:->\n");
                foreach (Git2Bit.GitModels.Milestone amilestone in closedGitMilestones)
                {
                    Git2Bit.BitModels.Milestone bitMilestone = bit.PostMilestone(selectedBitRepository,amilestone);
                    logger.AppendText("Ported closed Milestone: " + bitMilestone.name + "\n");
                }
            }

            if (openGitMilestones != null && openGitMilestones.Count > 0)
            {
                logger.AppendText("This repo has open Milestones. Porting them:->\n");
                foreach (Git2Bit.GitModels.Milestone amilestone in openGitMilestones)
                {
                    Git2Bit.BitModels.Milestone bitMilestone = bit.PostMilestone(selectedBitRepository, amilestone);
                    logger.AppendText("Ported open Milestone: " + bitMilestone.name + "\n");
                }
            }

            if (closedGitIssues != null && closedGitIssues.Count > 0)
            {
                logger.AppendText("This repo has closed Git Issues. Porting them:->\n");
                foreach (Git2Bit.GitModels.Issue aissue in closedGitIssues)
                {
                    List<Git2Bit.GitModels.Comments> issueComments = null;
                    if (gitComments.ContainsKey(aissue.number))
                    {
                        issueComments = gitComments[aissue.number];
                    }

                    Git2Bit.BitModels.Issue bitIssue = bit.PostIssue(selectedBitRepository, aissue, issueComments);
                    logger.AppendText("Ported closed Issue: " + bitIssue.title + "\n");
                }
            }

            if (openGitIssues != null && openGitIssues.Count > 0)
            {
                logger.AppendText("This repo has open Git Issues. Porting them:->\n");
                foreach (Git2Bit.GitModels.Issue aissue in openGitIssues)
                {
                    List<Git2Bit.GitModels.Comments> issueComments = null;
                    if (gitComments.ContainsKey(aissue.number))
                    {
                        issueComments = gitComments[aissue.number];
                    }

                    Git2Bit.BitModels.Issue bitIssue = bit.PostIssue(selectedBitRepository, aissue, issueComments);
                    logger.AppendText("Ported open Issue: " + bitIssue.title + "\n");
                }
            }
            

        }
    }
}
