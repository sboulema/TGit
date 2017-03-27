# TGit
Control TortoiseGit from within Visual Studio

[![Build status](https://ci.appveyor.com/api/projects/status/9qp6jfgql4irdy30?svg=true)](https://ci.appveyor.com/project/sboulema/tgit) 
[![VS Marketplace](http://vsmarketplacebadge.apphb.com/version-short/SamirBoulema.TGit.svg)](https://visualstudiogallery.msdn.microsoft.com/132a30d8-f318-4a53-8386-2c9fe52d77a1)
[![Beerpay](https://beerpay.io/sboulema/TGit/badge.svg?style=flat)](https://beerpay.io/sboulema/TGit)

## Getting started
1. Install the [TGit extension](https://visualstudiogallery.msdn.microsoft.com/132a30d8-f318-4a53-8386-2c9fe52d77a1)
2. Install [TortoiseGit](https://code.google.com/p/tortoisegit/)
3. Install [MSysGit](http://msysgit.github.io/)

## Options
TGit has several options: 

**Default commit message:**

You can change this to regular text or the following macros:
- $(FeatureName): Name of the current Feature branch
- $(BranchName): Name of the current branch
- [Common Macros for Build Commands and Properties](https://msdn.microsoft.com/en-us/library/c02as0cs.aspx)

**Close dialog after operation:**

You can change what happens to the TortoiseGit dialogs after their operations:

0: Close manually

1: Auto-close if no further options are available

2: Auto-close if no errors 

**Finish Feature tweaks:**

You can change what happens when using the finish feature command:
- Delete local branch (True/False)
- Delete remote branch (True/False)
- Push changes (True/False)

## Keyboard shortcuts

* `(G)it (C)ommit` - Ctrl+G, Ctrl+C
* `(G)it (F)etch` - Ctrl+G, Ctrl+F
* `(G)it (L)og` - Ctrl+G, Ctrl+L
* `(G)it (M)erge` - Ctrl+G, Ctrl+M
* `(G)it (P)ull` - Ctrl+G, Ctrl+P
* `(G)it P(u)sh` - Ctrl+G, Ctrl+U
* `(G)it (R)evert` - Ctrl+G, Ctrl+R
* `(G)it Stas(h)-Apply` - Ctrl+G, Ctrl+H
* `(G)it Stash-Cre(a)te` - Ctrl+G, Ctrl+A
* `(G)it (S)witch` - Ctrl+G, Ctrl+S
* `(G)it S(y)nc` - Ctrl+S, Ctrl+Y
* `(G)it Clea(n)up` - Ctrl+G, Ctrl+N
* `(G)it R(e)solve` - Ctrl+G, Ctrl+E
* `(G)it Sho(w) Changes` - Ctrl+G, Ctrl+W
* `(G)it File (B)lame` - Ctrl+G, Ctrl+B
* `(G)it File (D)iff` - Ctrl+G, Ctrl+D

To make these shortcuts work you will have to redefine the Ctrl+G (Go To Line) shortcut to be e.g. Ctrl+G, Ctrl+G. 
To do this go to Tools -> Options -> Keyboard and change the shortcut for Edit.GoTo

## Gitflow
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

---

![VS2017 Partner](http://i.imgur.com/wlgwRF1.png)

