using System;
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

        private void gitIssuesButton_Click(object sender, RoutedEventArgs e)
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
            selectedBitRepository = (string)bitRepositories.SelectedItem;
        }

        private void portGitIssues_Click(object sender, RoutedEventArgs e)
        {
            BitBucketRest bit = new BitBucketRest(bitUsername.Text, bitPassword.Password);
            if (closedGitMilestones.Count > 0)
            {
                logger.AppendText("This repo has closed Milestones. Porting them:->\n");
                foreach (Git2Bit.GitModels.Milestone amilestone in closedGitMilestones)
                {
                    Git2Bit.BitModels.Milestone bitMilestone = bit.PostMilestone(selectedBitRepository,amilestone);
                    logger.AppendText("Ported closed Milestone: " + bitMilestone.name + "\n");
                }
            }

            if (openGitMilestones.Count > 0)
            {
                logger.AppendText("This repo has open Milestones. Porting them:->\n");
                foreach (Git2Bit.GitModels.Milestone amilestone in openGitMilestones)
                {
                    Git2Bit.BitModels.Milestone bitMilestone = bit.PostMilestone(selectedBitRepository, amilestone);
                    logger.AppendText("Ported open Milestone: " + bitMilestone.name + "\n");
                }
            }

            if (closedGitIssues.Count > 0)
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

            if (openGitIssues.Count > 0)
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
