# TGIT
Control TortoiseGIT from within Visual Studio

[![tgit MyGet Build Status](https://www.myget.org/BuildSource/Badge/tgit?identifier=3efad407-2f96-4e1e-a0cc-beb30194d3cb)](https://www.myget.org/)

# Getting started
1. Install the TGIT extension (https://visualstudiogallery.msdn.microsoft.com/be8a61ca-9358-4f43-80e3-4fc73b09dff3)
2. Install TortoiseGIT (https://code.google.com/p/tortoisegit/)
3. Install MSysGit (http://msysgit.github.io/)

# Options
TGIT has several GitFlow options: "Develop branch prefix", "Feature branches prefix", "Master branch prefix", "Release branches prefix".
These options determine the prefix of the branches used in the gitflow branching model.
http://nvie.com/posts/a-successful-git-branching-model/

TGIT also has an option to specify a default commit message. You can change this to regular text or use the folowing macros:
https://msdn.microsoft.com/en-us/library/c02as0cs.aspx

# Gitflow
So what exactly are those gitflow menu items doing?

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
