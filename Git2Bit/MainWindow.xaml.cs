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

namespace Git2Bit
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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
            Git2Bit.Models.Repositories repos = git.GetRepos();

        }

        private void bitRepos_Click(object sender, RoutedEventArgs e)
        {

        }

        

      
     
    }
}
