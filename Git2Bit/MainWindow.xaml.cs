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

        public List<Git2Bit.Models.Repository> GitRepos;
        
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
            foreach (Git2Bit.Models.Repository repo in GitRepos)
            {
                if(repo.has_issues) {
                    repos_names.Add(repo.full_name);
                }
            }
             
            gitRepositories.ItemsSource = repos_names;
        }

        private void bitRepos_Click(object sender, RoutedEventArgs e)
        {

        }

        

      
     
    }
}
