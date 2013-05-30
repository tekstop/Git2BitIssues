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
using System.ComponentModel;
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
        public List<Git2Bit.BitModels.Issue> bitIssues;
        public List<Git2Bit.BitModels.Milestone> bitMilestones;
        public Dictionary<int, List<Git2Bit.BitModels.Comments>> bitComments;
        
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
            bitComments = new Dictionary<int, List<BitModels.Comments>>();
            // Get the Bit Milestones
            bitMilestones = bit.GetMilestones(selectedBitRepository);
            if (bitMilestones == null)
            {
                logger.AppendText("The " + selectedBitRepository + " has no milestones.\n");

            }
            else
            {
                logger.AppendText("The " + selectedBitRepository + " has " + bitMilestones.Count.ToString() + " milestones.\n");

            }
            bitIssues = bit.GetIssues(selectedBitRepository);
            while (bit._hasMoreIssues)
            {
                bitIssues.Concat(bit.GetIssues(selectedBitRepository));
            }
            logger.AppendText("The " + selectedBitRepository + " has " + bitIssues.Count.ToString() + " issues.\n");
            // Get individual Comments for each issue
            foreach (Git2Bit.BitModels.Issue issue in bitIssues)
            {
                if (issue.comment_count > 0)
                {
                    // has comments:
                    List<Git2Bit.BitModels.Comments> comments = bit.GetComments(selectedBitRepository, issue.local_id);
                    logger.AppendText("Issue " + issue.local_id.ToString() + " has " + comments.Count.ToString() + " comments.\n");
                    bitComments[issue.local_id] = comments;
                }
            }
            bitGetIssuesButton.IsEnabled = false;
        }

        private void gitGetIssuesButton_Click(object sender, RoutedEventArgs e)
        {
            busyIndicator.IsBusy = true;
            busyIndicator.BusyContent = "Getting Git Issues ...";
            BackgroundWorker bg = new BackgroundWorker();
            List<String> arguments = new List<String>();
            arguments.Add(gitUsername.Text);
            arguments.Add(gitPassword.Password);
            bg.DoWork += bg_gitIssuesWork;
            bg.RunWorkerCompleted += bg_removeBusy;
            bg.RunWorkerAsync(arguments);

            
        }

        private void bg_removeBusy(object sender, RunWorkerCompletedEventArgs e)
        {
            busyIndicator.IsBusy = false;
            gitIssuesButton.IsEnabled = true;
            
        }

        private void bg_gitIssuesWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            List<String> list = e.Argument as List<String>;
            GithubRest git = new GithubRest(list[0], list[1]);
            gitComments = new Dictionary<int, List<GitModels.Comments>>();
            openGitMilestones = git.GetMilestones(selectedGitRepositiry);
            if (openGitMilestones != null)
            {
                String textData = "The " + selectedGitRepositiry + " has " + openGitMilestones.Count.ToString() + " open milestones.\n";
                Dispatcher.Invoke(new Action(() => logger.AppendText(textData)));
              
            }
            closedGitMilestones = git.GetMilestones(selectedGitRepositiry, false);
            if (closedGitMilestones != null)
            {
                String textData = ("The " + selectedGitRepositiry + " has " + closedGitMilestones.Count.ToString() + " closed milestones.\n");
                Dispatcher.Invoke(new Action(() => logger.AppendText(textData)));
            }
            openGitIssues = git.GetIssues(selectedGitRepositiry);
            if (openGitIssues != null)
            {
                String textData = ("The " + selectedGitRepositiry + " has " + openGitIssues.Count.ToString() + " open issues.\n");
                Dispatcher.Invoke(new Action(() => logger.AppendText(textData)));
                // Get individual Comments for each open issue
                foreach (Git2Bit.GitModels.Issue issue in openGitIssues)
                {
                    if (issue.comments > 0)
                    {
                        // has comments:
                        List<Git2Bit.GitModels.Comments> comments = git.GetComments(selectedGitRepositiry, issue.number);
                        String textData1 = ("Issue " + issue.number.ToString() + " has " + comments.Count.ToString() + " comments.\n");
                        Dispatcher.Invoke(new Action(() => logger.AppendText(textData1)));
                        gitComments[issue.number] = comments;
                    }
                }
            }
            closedGitIssues = git.GetIssues(selectedGitRepositiry, false);
            if (closedGitIssues != null)
            {
                logger.AppendText("The " + selectedGitRepositiry + " has " + closedGitIssues.Count.ToString() + " closed issues.\n");
                // Get individual Comments for each open issue
                foreach (Git2Bit.GitModels.Issue issue in closedGitIssues)
                {
                    if (issue.comments > 0)
                    {
                        // has comments:
                        List<Git2Bit.GitModels.Comments> comments = git.GetComments(selectedGitRepositiry, issue.number);
                        String textData1 = ("Issue " + issue.number.ToString() + " has " + comments.Count.ToString() + " comments.\n");
                        Dispatcher.Invoke(new Action(() => logger.AppendText(textData1)));
                        gitComments[issue.number] = comments;
                    }
                }
            }
                     
        }

        private void gitRepositories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Enable gitIssueButton_Click with the selected repository
            gitIssuesButton.IsEnabled = true;
            selectedGitRepositiry = (string)gitRepositories.SelectedItem;
            // TODO
            portBitIssuesToGit.IsEnabled = false;
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
            if (bitMilestones != null && bitMilestones.Count > 0)
            {
                logger.AppendText("This repo has Milestones. Porting them:->\n");
                foreach (Git2Bit.BitModels.Milestone amilestone in bitMilestones)
                {
                    Git2Bit.GitModels.MilestonePost gitMilestone = git.PostMilestone(selectedGitRepositiry, amilestone);
                    logger.AppendText("Ported closed Milestone: " + gitMilestone.title + "\n");
                }
            }

        }

        private void portGitIssuesToBit_Click(object sender, RoutedEventArgs e)
        {
            busyIndicator.IsBusy = true;
            busyIndicator.BusyContent = "Posting Git Issues to Bit ...";
            BackgroundWorker bg = new BackgroundWorker();
            List<String> arguments = new List<String>();
            arguments.Add(bitUsername.Text);
            arguments.Add(bitPassword.Password);
            bg.DoWork += bg_bitIssuesWork;
            bg.RunWorkerCompleted += bg_removeBusy;
            bg.RunWorkerAsync(arguments);
            
        }
        private void bg_bitIssuesWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            List<String> list = e.Argument as List<String>;
            BitBucketRest bit = new BitBucketRest(list[0],list[1]);
            if (closedGitMilestones != null && closedGitMilestones.Count > 0)
            {
                String textData1 = ("This repo has closed Milestones. Porting them:->\n");
                Dispatcher.Invoke(new Action(() => logger.AppendText(textData1)));
                foreach (Git2Bit.GitModels.Milestone amilestone in closedGitMilestones)
                {
                    Git2Bit.BitModels.Milestone bitMilestone = bit.PostMilestone(selectedBitRepository, amilestone);
                    if (bitMilestone != null)
                    {
                        String textData2 = ("Ported closed Milestone: " + bitMilestone.name + "\n");
                        Dispatcher.Invoke(new Action(() => logger.AppendText(textData2)));
                    }
                }
            }

            if (openGitMilestones != null && openGitMilestones.Count > 0)
            {
                String textData1 = ("This repo has open Milestones. Porting them:->\n");
                Dispatcher.Invoke(new Action(() => logger.AppendText(textData1)));
                foreach (Git2Bit.GitModels.Milestone amilestone in openGitMilestones)
                {
                    Git2Bit.BitModels.Milestone bitMilestone = bit.PostMilestone(selectedBitRepository, amilestone);
                    if (bitMilestone != null)
                    {
                        String textData2 = ("Ported open Milestone: " + bitMilestone.name + "\n");
                        Dispatcher.Invoke(new Action(() => logger.AppendText(textData2)));
                    }
                }
            }

            if (closedGitIssues != null && closedGitIssues.Count > 0)
            {
                String textData1 = ("This repo has closed Git Issues. Porting them:->\n");
                Dispatcher.Invoke(new Action(() => logger.AppendText(textData1)));
                foreach (Git2Bit.GitModels.Issue aissue in closedGitIssues)
                {
                    List<Git2Bit.GitModels.Comments> issueComments = null;
                    if (gitComments.ContainsKey(aissue.number))
                    {
                        issueComments = gitComments[aissue.number];
                    }

                    Git2Bit.BitModels.Issue bitIssue = bit.PostIssue(selectedBitRepository, aissue, issueComments);
                    if (bitIssue != null)
                    {
                        String textData2 = ("Ported closed Issue: " + bitIssue.title + "\n");
                        Dispatcher.Invoke(new Action(() => logger.AppendText(textData2)));
                    }
                }
            }

            if (openGitIssues != null && openGitIssues.Count > 0)
            {
                String textData1 = ("This repo has open Git Issues. Porting them:->\n");
                Dispatcher.Invoke(new Action(() => logger.AppendText(textData1)));
                foreach (Git2Bit.GitModels.Issue aissue in openGitIssues)
                {
                    List<Git2Bit.GitModels.Comments> issueComments = null;
                    if (gitComments.ContainsKey(aissue.number))
                    {
                        issueComments = gitComments[aissue.number];
                    }

                    Git2Bit.BitModels.Issue bitIssue = bit.PostIssue(selectedBitRepository, aissue, issueComments);
                    if (bitIssue != null)
                    {
                        String textData2 = ("Ported open Issue: " + bitIssue.title + "\n");
                        Dispatcher.Invoke(new Action(() => logger.AppendText(textData2)));
                    }
                }
            }
        }
    }
}
