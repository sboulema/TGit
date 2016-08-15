# TGit
Control TortoiseGit from within Visual Studio

[![tgit MyGet Build Status](https://www.myget.org/BuildSource/Badge/tgit?identifier=3efad407-2f96-4e1e-a0cc-beb30194d3cb)](https://www.myget.org/)

# Getting started
1. Install the [TGit extension](https://visualstudiogallery.msdn.microsoft.com/132a30d8-f318-4a53-8386-2c9fe52d77a1)
2. Install [TortoiseGit](https://code.google.com/p/tortoisegit/)
3. Install [MSysGit](http://msysgit.github.io/)

# Options
TGit has two options: 

"Default commit message":

You can change this to regular text or use the following macros:
https://msdn.microsoft.com/en-us/library/c02as0cs.aspx

"Close on end":

You can change what happens to the TortoiseGit dialogs after their operations:

0: Close manually

1: Auto-close if no further options are available

2: Auto-close if no errors 

# Gitflow
So what exactly are those [GitFlow](http://nvie.com/posts/a-successful-git-branching-model/) menu items doing?

##### Start New Feature
1. Switch to the develop branch
2. Pull latest changes on develop
3. Create and switch to a new branch

##### Finish Feature
1. Switch to the develop branch
2. Pull latest changes on develop
3. Merge the feature branch to develop
4. Delete the local feature branch
5. Delete the remote feature branch
6. Push all changes to develop

##### Start New Release
1. Switch to the develop branch
2. Pull latest changes on develop
3. Create and switch to a new release branch

##### Finish Release
1. Switch to the master branch
2. Pull latest changes on master
3. Merge the release branch to master
4. Tag the release
5. Switch to the develop branch
6. Pull latest changes on develop
7. Merge the release branch to develop
8. Delete the local release branch
9. Delete the remote release branch
10. Push all changes to develop
11. Push all changes to master
12. Push the tag
