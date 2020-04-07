# TGit
Control TortoiseGit from within Visual Studio

[![Build Status](https://dev.azure.com/sboulema/TGit/_apis/build/status/sboulema.TGit)](https://dev.azure.com/sboulema/TGit/_build/latest?definitionId=1)
[![VS Marketplace](http://vsmarketplacebadge.apphb.com/version-short/SamirBoulema.TGit.svg)](https://visualstudiogallery.msdn.microsoft.com/132a30d8-f318-4a53-8386-2c9fe52d77a1)
[![Donate](https://img.shields.io/badge/%F0%9F%92%B0-Donate-green.svg?style=flat)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=X3S369MR8JYCL&source=url)

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
- Pull changes (True/False)
- Use annotated tags (True/False)

## Keyboard shortcuts

* `(G)it (C)ommit` - Ctrl+Shift+Alt+G, C
* `(G)it (F)etch` - Ctrl+Shift+Alt+G, F
* `(G)it (L)og` - Ctrl+Shift+Alt+G, L
* `(G)it (M)erge` - Ctrl+Shift+Alt+G, M
* `(G)it (P)ull` - Ctrl+Shift+Alt+G, P
* `(G)it P(u)sh` - Ctrl+Shift+Alt+G, U
* `(G)it (R)evert` - Ctrl+Shift+Alt+G, R
* `(G)it Stas(h)-Apply` - Ctrl+Shift+Alt+G, H
* `(G)it Stash-Cre(a)te` - Ctrl+Shift+Alt+G, A
* `(G)it (S)witch` - Ctrl+Shift+Alt+G, S
* `(G)it S(y)nc` - Ctrl+Shift+Alt+S, Y
* `(G)it Clea(n)up` - Ctrl+Shift+Alt+G, N
* `(G)it R(e)solve` - Ctrl+Shift+Alt+G, E
* `(G)it Sho(w) Changes` - Ctrl+Shift+Alt+G, W
* `(G)it File (B)lame` - Ctrl+Shift+Alt+G, B
* `(G)it File (D)iff` - Ctrl+Shift+Alt+G, D
* `(G)it Rebase` - Ctrl+Shift+Alt+G, G

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

