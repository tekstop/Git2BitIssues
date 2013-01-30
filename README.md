Git2BitIssues
=============

Port GitHub Issues to BitBucket

I recently ported a private repository from GitHub to BitBucket and realized that though BitBucket had an easy way of 
directly importing Github repositories, there was no easy way to port the existing issues and milestones which exist in 
GitHub.

Hence, Git2BitIssues! This is a simple converter for GitHub Issues and Milestones to BitBucket Issues and Milestones. 

* This is currently only for Windows (Win 7, VS 2010).
* Uses RestSharp
* Ports from only GitHub to BitBucket

=========================================================================

Porting
=======
* Create BitBucket Repository from GitHub. Ensure that the BitBucket Repo has Issues.
* Run Git2BitIssues
 * Enter your github username/password and Select Get Repos
 * It gets the repositories you own. Select the one you want to port and press Get Issues.
 * Now, Enter you BitBucket username/password and select Get repos.
 * Select the repo you want to port the issues and milestones and press Port Issues.

You should be all set!

===========================================================================


Work In Progress
================

* Windows 8 App (Mid Feb, 2013)
* Mac OSX App (Mid Feb, 2013)
* BitBucket -> GitHub (Mar, 2013)


Please feel free to create issues

