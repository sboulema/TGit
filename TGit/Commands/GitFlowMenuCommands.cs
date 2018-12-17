﻿using SamirBoulema.TGit.Helpers;
using Microsoft.VisualBasic;
using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;
using EnvDTE;
using SamirBoulema.TGit.Models;

namespace SamirBoulema.TGit.Commands
{
    public class GitFlowMenuCommands
    {
        private readonly OptionPageGrid _options;
        private readonly OleMenuCommandService _mcs;
        private readonly DTE _dte;
        private readonly EnvHelper _envHelper;

        public GitFlowMenuCommands(OleMenuCommandService mcs, DTE dte, 
            OptionPageGrid options, EnvHelper envHelper)
        {
            _mcs = mcs;
            _options = options;
            _dte = dte;
            _envHelper = envHelper;
        }

        public void AddCommands()
        {
            //GitFlow Commands
            //Start/Finish Feature
            CommandHelper.AddCommand(_mcs, StartFeatureCommand, PkgCmdIDList.StartFeature, CommandHelper.GitFlow_BeforeQueryStatus);
            CommandHelper.AddCommand(_mcs, FinishFeatureCommand, PkgCmdIDList.FinishFeature, CommandHelper.Feature_BeforeQueryStatus);

            //Start/Finish Release
            CommandHelper.AddCommand(_mcs, StartReleaseCommand, PkgCmdIDList.StartRelease, CommandHelper.GitFlow_BeforeQueryStatus);
            CommandHelper.AddCommand(_mcs, FinishReleaseCommand, PkgCmdIDList.FinishRelease, CommandHelper.Release_BeforeQueryStatus);

            //Start/Finish Hotfix
            CommandHelper.AddCommand(_mcs, StartHotfixCommand, PkgCmdIDList.StartHotfix, CommandHelper.GitFlow_BeforeQueryStatus);
            CommandHelper.AddCommand(_mcs, FinishHotfixCommand, PkgCmdIDList.FinishHotfix, CommandHelper.Hotfix_BeforeQueryStatus);

            //Init
            CommandHelper.AddCommand(_mcs, InitCommand, PkgCmdIDList.Init, CommandHelper.GitHubFlow_BeforeQueryStatus);

            //GitHubFlow Commands
            //Start/Finish Feature
            CommandHelper.AddCommand(_mcs, StartFeatureGitHubCommand, PkgCmdIDList.StartFeatureGitHub);
            CommandHelper.AddCommand(_mcs, FinishFeatureGitHubCommand, PkgCmdIDList.FinishFeatureGitHub, CommandHelper.Feature_BeforeQueryStatus);
        }

        private void InitCommand(object sender, EventArgs e)
        {
            var rootDir = _envHelper.GetGitRoot();
            if (string.IsNullOrEmpty(rootDir)) return;

            var flowDialog = new FlowDialog();
            if (flowDialog.ShowDialog() != DialogResult.OK) return;

            var versionTag = string.IsNullOrEmpty(flowDialog.GitConfig.TagPrefix) ? "\"\"" : flowDialog.GitConfig.TagPrefix;

            /* 1. Add GitFlow config options
                 * 2. Checkout develop branch (create if it doesn't exist, reset if it does)
                 * 3. Push develop branch
                 */
            var process = ProcessHelper.StartProcessGui(_dte, _envHelper,
                "cmd.exe",
                $"/c cd \"{rootDir}\" && " +
                GitHelper.GetSshSetup(_envHelper) +
                FormatCliCommand($"config --add gitflow.branch.master {flowDialog.GitConfig.MasterBranch}") +
                FormatCliCommand($"config --add gitflow.branch.develop {flowDialog.GitConfig.DevelopBranch}") +
                FormatCliCommand($"config --add gitflow.prefix.feature {flowDialog.GitConfig.FeaturePrefix}") +
                FormatCliCommand($"config --add gitflow.prefix.release {flowDialog.GitConfig.ReleasePrefix}") +
                FormatCliCommand($"config --add gitflow.prefix.hotfix {flowDialog.GitConfig.HotfixPrefix}") +
                FormatCliCommand($"config --add gitflow.prefix.versiontag {versionTag}") +
                (GitHelper.RemoteBranchExists(_envHelper, flowDialog.GitConfig.DevelopBranch) ?
                    "echo." : 
                    FormatCliCommand($"checkout -b {flowDialog.GitConfig.DevelopBranch}", false)),
                "Initializing GitFlow"
                );
            process.WaitForExit();

            _envHelper.ClearCache(CacheKeyEnum.GitConfig);
        }

        private void StartFeatureCommand(object sender, EventArgs e)
        {
            var rootDir = _envHelper.GetGitRoot();
            if (string.IsNullOrEmpty(rootDir)) return;

            var featureName = Interaction.InputBox("Feature Name:", "Start New Feature");
            if (string.IsNullOrEmpty(featureName)) return;

            var flowOptions = GitHelper.GetGitConfig(_envHelper);

            /* 1. Switch to the develop branch
             * 2. Pull latest changes on develop
             * 3. Create and switch to a new branch
             */
            ProcessHelper.StartProcessGui(_dte, _envHelper,
                "cmd.exe",
                $"/c cd \"{rootDir}\" && " +
                    GitHelper.GetSshSetup(_envHelper) +
                    FormatCliCommand($"checkout {flowOptions.DevelopBranch}") +
                    FormatCliCommand("pull") +
                    FormatCliCommand($"checkout -b {flowOptions.FeaturePrefix}{featureName} {flowOptions.DevelopBranch}", false),
                $"Starting feature {featureName}"
            );
        }

        private void StartFeatureGitHubCommand(object sender, EventArgs e)
        {
            var rootDir = _envHelper.GetGitRoot();
            if (string.IsNullOrEmpty(rootDir)) return;

            var featureName = Interaction.InputBox("Feature Name:", "Start New Feature");
            if (string.IsNullOrEmpty(featureName)) return;

            /* 1. Switch to the master branch
             * 2. Pull latest changes on master
             * 3. Create and switch to a new branch
             */
            ProcessHelper.StartProcessGui(_dte, _envHelper,
                "cmd.exe",
                $"/c cd \"{rootDir}\" && " +
                    GitHelper.GetSshSetup(_envHelper) +
                    FormatCliCommand("checkout master") +
                    FormatCliCommand("pull") +
                    FormatCliCommand($"checkout -b {featureName} master", false),
                $"Starting feature {featureName}"
            );
        }

        private void FinishFeatureCommand(object sender, EventArgs e)
        {
            var rootDir = _envHelper.GetGitRoot();
            if (string.IsNullOrEmpty(rootDir)) return;

            var featureBranch = GitHelper.GetCurrentBranchName(false, _envHelper);
            var featureName = GitHelper.GetCurrentBranchName(true, _envHelper);
            var gitConfig = GitHelper.GetGitConfig(_envHelper);

            /* 1. Switch to the develop branch
             * 2. Pull latest changes on develop
             * 3. Merge the feature branch to develop
             * 4. Push all changes to develop
             * 5. Delete the local feature branch
             * 6. Delete the remote feature branch
             */
            ProcessHelper.StartProcessGui(_dte, _envHelper,
                "cmd.exe",
                $"/c cd \"{rootDir}\" && " +
                    GitHelper.GetSshSetup(_envHelper) +
                    FormatCliCommand($"checkout {gitConfig.DevelopBranch}") +
                    FormatCliCommand("pull") +
                    FormatCliCommand($"merge --no-ff {featureBranch}", false),
                $"Finishing feature {featureName}",
                featureBranch, null, _options, FormatCliCommand($"push origin {gitConfig.DevelopBranch}")
            );
        }

        private string FormatCliCommand(string gitCommand, bool appendNextLine = true)
        {
            var git = _envHelper.GetGit();
            return $"echo ^> {Path.GetFileNameWithoutExtension(git)} {gitCommand} && \"{git}\" {gitCommand}{(appendNextLine ? " && " : string.Empty)}";
        }

        private void FinishFeatureGitHubCommand(object sender, EventArgs e)
        {
            var rootDir = _envHelper.GetGitRoot();
            if (string.IsNullOrEmpty(rootDir)) return;

            var featureBranch = GitHelper.GetCurrentBranchName(false, _envHelper);
            var featureName = GitHelper.GetCurrentBranchName(true, _envHelper);

            /* 1. Switch to the master branch
             * 2. Pull latest changes on master
             * 3. Merge the feature branch to master
             * 4. Push all changes to master
             * 5. Delete the local feature branch
             * 6. Delete the remote feature branch
             */
            ProcessHelper.StartProcessGui(_dte, _envHelper,
                "cmd.exe",
                $"/c cd \"{rootDir}\" && " +
                    GitHelper.GetSshSetup(_envHelper) +
                    FormatCliCommand("checkout master") +
                    FormatCliCommand("pull") +
                    FormatCliCommand($"merge --no-ff {featureBranch}", false),
                $"Finishing feature {featureName}",
                featureBranch, null, _options, FormatCliCommand("push origin master"));
        }

        private void StartReleaseCommand(object sender, EventArgs e)
        {
            var rootDir = _envHelper.GetGitRoot();
            if (string.IsNullOrEmpty(rootDir)) return;

            var releaseVersion = Interaction.InputBox("Release Version:", "Start New Release");
            if (string.IsNullOrEmpty(releaseVersion)) return;

            var flowOptions = GitHelper.GetGitConfig(_envHelper);

            /* 1. Switch to the develop branch
             * 2. Pull latest changes on develop
             * 3. Create and switch to a new release branch
             */
            ProcessHelper.StartProcessGui(_dte, _envHelper,
                "cmd.exe",
                $"/c cd \"{rootDir}\" && " +
                    GitHelper.GetSshSetup(_envHelper) +
                    FormatCliCommand($"checkout {flowOptions.DevelopBranch}") +
                    FormatCliCommand("pull") +
                    FormatCliCommand($"checkout -b {flowOptions.ReleasePrefix}{releaseVersion} {flowOptions.DevelopBranch}", false),
                $"Starting release {releaseVersion}"
            );
        }

        private void FinishReleaseCommand(object sender, EventArgs e)
        {
            var rootDir = _envHelper.GetGitRoot();
            if (string.IsNullOrEmpty(rootDir)) return;

            var releaseBranch = GitHelper.GetCurrentBranchName(false, _envHelper);
            var releaseName = GitHelper.GetCurrentBranchName(true, _envHelper);
            var gitConfig = GitHelper.GetGitConfig(_envHelper);

            /* 1. Switch to the master branch
             * 2. Pull latest changes on master
             * 3. Merge the release branch to master
             * 4. Tag the release
             * 5. Switch to the develop branch
             * 6. Pull latest changes on develop
             * 7. Merge the release branch to develop
             * 8. Push all changes to develop
             * 9. Push all changes to master
             * 10. Push the tag
             * 11. Delete the local release branch
             * 12. Delete the remote release branch
             */
            ProcessHelper.StartProcessGui(_dte, _envHelper,
                "cmd.exe",
                $"/c cd \"{rootDir}\" && " +
                    GitHelper.GetSshSetup(_envHelper) +
                    FormatCliCommand($"checkout {gitConfig.MasterBranch}") +
                    FormatCliCommand("pull") +
                    FormatCliCommand($"merge --no-ff {releaseBranch}") +
                    FormatCliCommand($"tag {gitConfig.TagPrefix}{releaseName}") +
                    FormatCliCommand($"checkout {gitConfig.DevelopBranch}") +
                    FormatCliCommand("pull") +
                    FormatCliCommand($"merge --no-ff {releaseBranch}", false),
                $"Finishing release {releaseName}",
                releaseBranch, null, _options,
                    FormatCliCommand($"push origin {gitConfig.DevelopBranch}") +
                    FormatCliCommand($"push origin {gitConfig.MasterBranch}") +
                    FormatCliCommand($"push origin {gitConfig.TagPrefix}{releaseName}")
            );
        }

        private void StartHotfixCommand(object sender, EventArgs e)
        {
            var rootDir = _envHelper.GetGitRoot();
            if (string.IsNullOrEmpty(rootDir)) return;

            var hotfixVersion = Interaction.InputBox("Hotfix Version:", "Start New Hotfix");
            if (string.IsNullOrEmpty(hotfixVersion)) return;

            var flowOptions = GitHelper.GetGitConfig(_envHelper);

            /* 1. Switch to the master branch
             * 2. Pull latest changes on master
             * 3. Create and switch to a new hotfix branch
             */
            ProcessHelper.StartProcessGui(_dte, _envHelper,
                "cmd.exe",
                $"/c cd \"{rootDir}\" && " +
                    GitHelper.GetSshSetup(_envHelper) +
                    FormatCliCommand($"checkout {flowOptions.MasterBranch}") +
                    FormatCliCommand("pull") +
                    FormatCliCommand($"checkout -b {flowOptions.HotfixPrefix}{hotfixVersion} {flowOptions.MasterBranch}", false),
                $"Starting hotfix {hotfixVersion}"
            );
        }

        private void FinishHotfixCommand(object sender, EventArgs e)
        {
            var rootDir = _envHelper.GetGitRoot();
            if (string.IsNullOrEmpty(rootDir)) return;

            var hotfixBranch = GitHelper.GetCurrentBranchName(false, _envHelper);
            var hotfixName = GitHelper.GetCurrentBranchName(true, _envHelper);
            var gitConfig = GitHelper.GetGitConfig(_envHelper);

            /* 1. Switch to the master branch
             * 2. Pull latest changes on master
             * 3. Merge the hotfix branch to master
             * 4. Tag the hotfix
             * 5. Switch to the develop branch
             * 6. Pull latest changes on develop
             * 7. Merge the hotfix branch to develop
             * 8. Push all changes to develop
             * 9. Push all changes to master
             * 10. Push the tag
             * 11. Delete the local hotfix branch
             * 12. Delete the remote hotfix branch
             */
            ProcessHelper.StartProcessGui(_dte, _envHelper,
                "cmd.exe",
                $"/c cd \"{rootDir}\" && " +
                    GitHelper.GetSshSetup(_envHelper) +
                    FormatCliCommand($"checkout {gitConfig.MasterBranch}") +
                    FormatCliCommand("pull") +
                    FormatCliCommand($"merge --no-ff {hotfixBranch}") +
                    FormatCliCommand($"tag {gitConfig.TagPrefix}{hotfixName}") +
                    FormatCliCommand($"checkout {gitConfig.DevelopBranch}") +
                    FormatCliCommand("pull") +
                    FormatCliCommand($"merge --no-ff {hotfixBranch}", false),
                $"Finishing hotfix {hotfixName}",
                hotfixBranch, null, _options, 
                    FormatCliCommand($"push origin {gitConfig.DevelopBranch}") +
                    FormatCliCommand($"push origin {gitConfig.MasterBranch}") +
                    FormatCliCommand($"push origin {gitConfig.TagPrefix}{hotfixName}")
            );
        }
    }
}
