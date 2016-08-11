using SamirBoulema.TGit.Helpers;
using Microsoft.VisualBasic;
using System;
using System.IO;
using Microsoft.VisualStudio.Shell;

namespace SamirBoulema.TGit.Commands
{
    public class GitFlowCommands
    {
        private readonly ProcessHelper _processHelper;
        private readonly CommandHelper _commandHelper;
        private readonly FileHelper _fileHelper;
        private readonly GitHelper _gitHelper;
        private readonly OptionFlowPageGrid _options;
        private readonly string _gitBin;
        private readonly OleMenuCommandService _mcs;

        public GitFlowCommands(ProcessHelper processHelper, CommandHelper commandHelper, GitHelper gitHelper, FileHelper fileHelper,
            OptionFlowPageGrid options, OleMenuCommandService mcs)
        {
            _processHelper = processHelper;
            _commandHelper = commandHelper;
            _gitHelper = gitHelper;
            _fileHelper = fileHelper;
            _options = options;
            _gitBin = fileHelper.GetMSysGit();
            _mcs = mcs;
        }

        public void AddCommands()
        {
            //GitFlow Commands
            //Start/Finish Feature
            var startFeature = _commandHelper.CreateCommand(StartFeatureCommand, PkgCmdIDList.StartFeature);
            startFeature.BeforeQueryStatus += _commandHelper.GitFlow_BeforeQueryStatus;
            _mcs.AddCommand(startFeature);

            var finishFeature = _commandHelper.CreateCommand(FinishFeatureCommand, PkgCmdIDList.FinishFeature);
            finishFeature.BeforeQueryStatus += _commandHelper.Feature_BeforeQueryStatus;
            _mcs.AddCommand(finishFeature);

            //Start/Finish Release
            var startRelease = _commandHelper.CreateCommand(StartReleaseCommand, PkgCmdIDList.StartRelease);
            startRelease.BeforeQueryStatus += _commandHelper.GitFlow_BeforeQueryStatus;
            _mcs.AddCommand(startRelease);

            var finishRelease = _commandHelper.CreateCommand(FinishReleaseCommand, PkgCmdIDList.FinishRelease);
            finishRelease.BeforeQueryStatus += _commandHelper.Release_BeforeQueryStatus;
            _mcs.AddCommand(finishRelease);

            //Start/Finish Hotfix
            var startHotfix = _commandHelper.CreateCommand(StartHotfixCommand, PkgCmdIDList.StartHotfix);
            startHotfix.BeforeQueryStatus += _commandHelper.GitFlow_BeforeQueryStatus;
            _mcs.AddCommand(startHotfix);

            var finishHotfix = _commandHelper.CreateCommand(FinishHotfixCommand, PkgCmdIDList.FinishHotfix);
            finishHotfix.BeforeQueryStatus += _commandHelper.Hotfix_BeforeQueryStatus;
            _mcs.AddCommand(finishHotfix);

            //Init
            var init = _commandHelper.CreateCommand(InitCommand, PkgCmdIDList.Init);
            init.BeforeQueryStatus += _commandHelper.GitHubFlow_BeforeQueryStatus;
            _mcs.AddCommand(init);

            //GitHubFlow Commands
            //Start/Finish Feature
            _commandHelper.AddCommand(StartFeatureGitHubCommand, PkgCmdIDList.StartFeatureGitHub);
            var finishFeatureGitHub = _commandHelper.CreateCommand(FinishFeatureGitHubCommand, PkgCmdIDList.FinishFeatureGitHub);
            finishFeatureGitHub.BeforeQueryStatus += _commandHelper.Feature_BeforeQueryStatus;
            _mcs.AddCommand(finishFeatureGitHub);
        }

        private void InitCommand(object sender, EventArgs e)
        {
            var solutionDir = _fileHelper.GetSolutionDir();
            if (string.IsNullOrEmpty(solutionDir)) return;

            /* 1. Switch to the develop branch
             * 2. Pull latest changes on develop
             * 3. Create and switch to a new branch
             */
            _processHelper.StartProcessGui(
                "cmd.exe",
                $"/c cd \"{solutionDir}\" && " +
                    _gitHelper.GetSshSetup() +
                    FormatCliCommand($"config --add gitflow.branch.master {_options.MasterBranch}") +
                    FormatCliCommand($"config --add gitflow.branch.develop {_options.DevelopBranch}") +
                    FormatCliCommand($"config --add gitflow.prefix.feature {_options.FeaturePrefix}") +
                    FormatCliCommand($"config --add gitflow.prefix.release {_options.ReleasePrefix}") +
                    FormatCliCommand($"config --add gitflow.prefix.hotfix {_options.HotfixPrefix}") +
                    FormatCliCommand($"checkout -b {_options.DevelopBranch} {_options.MasterBranch}", false),
                "Initializing GitFlow"
            );
        }

        private void StartFeatureCommand(object sender, EventArgs e)
        {
            var solutionDir = _fileHelper.GetSolutionDir();
            if (string.IsNullOrEmpty(solutionDir)) return;
            var featureName = Interaction.InputBox("Feature Name:", "Start New Feature");
            if (string.IsNullOrEmpty(featureName)) return;

            /* 1. Switch to the develop branch
             * 2. Pull latest changes on develop
             * 3. Create and switch to a new branch
             */
            _processHelper.StartProcessGui(
                "cmd.exe",
                $"/c cd \"{solutionDir}\" && " +
                    _gitHelper.GetSshSetup() +
                    FormatCliCommand($"checkout {_options.DevelopBranch}") +
                    FormatCliCommand("pull") +
                    FormatCliCommand($"checkout -b {_options.FeaturePrefix}{featureName} {_options.DevelopBranch}", false),
                $"Starting feature {featureName}"
            );
        }

        private void StartFeatureGitHubCommand(object sender, EventArgs e)
        {
            var solutionDir = _fileHelper.GetSolutionDir();
            if (string.IsNullOrEmpty(solutionDir)) return;
            var featureName = Interaction.InputBox("Feature Name:", "Start New Feature");
            if (string.IsNullOrEmpty(featureName)) return;

            /* 1. Switch to the master branch
             * 2. Pull latest changes on master
             * 3. Create and switch to a new branch
             */
            _processHelper.StartProcessGui(
                "cmd.exe",
                $"/c cd \"{solutionDir}\" && " +
                    _gitHelper.GetSshSetup() +
                    FormatCliCommand($"checkout {_options.MasterBranch}") +
                    FormatCliCommand("pull") +
                    FormatCliCommand($"checkout -b {_options.FeaturePrefix}{featureName} {_options.MasterBranch}", false),
                $"Starting feature {featureName}"
            );
        }

        private void FinishFeatureCommand(object sender, EventArgs e)
        {
            var solutionDir = _fileHelper.GetSolutionDir();
            if (string.IsNullOrEmpty(solutionDir)) return;
            var featureName = _gitHelper.GetCurrentBranchName(true);

            /* 1. Switch to the develop branch
             * 2. Pull latest changes on develop
             * 3. Merge the feature branch to develop
             * 4. Push all changes to develop
             * 5. Delete the local feature branch
             * 6. Delete the remote feature branch
             */
            _processHelper.StartProcessGui(
                "cmd.exe",
                $"/c cd \"{solutionDir}\" && " +
                    _gitHelper.GetSshSetup() +
                    FormatCliCommand($"checkout {_options.DevelopBranch}") +
                    FormatCliCommand("pull") +
                    FormatCliCommand($"merge --no-ff {_options.FeaturePrefix}{featureName}") +
                    FormatCliCommand($"push origin {_options.DevelopBranch}") + 
                    FormatCliCommand($"branch -d {_options.FeaturePrefix}{featureName}") +
                    FormatCliCommand($"push origin --delete {_options.FeaturePrefix}{featureName}", false), 
                $"Finishing feature {featureName}"
            );
        }

        private string FormatCliCommand(string gitCommand, bool appendNextLine = true)
        {
            return $"echo ^> {Path.GetFileNameWithoutExtension(_gitBin)} {gitCommand} && \"{_gitBin}\" {gitCommand}{(appendNextLine ? " && " : string.Empty)}";
        }

        private void FinishFeatureGitHubCommand(object sender, EventArgs e)
        {
            var solutionDir = _fileHelper.GetSolutionDir();
            if (string.IsNullOrEmpty(solutionDir)) return;
            var featureName = _gitHelper.GetCurrentBranchName(true);

            /* 1. Switch to the master branch
             * 2. Pull latest changes on master
             * 3. Merge the feature branch to master
             * 4. Push all changes to master
             * 5. Delete the local feature branch
             * 6. Delete the remote feature branch
             */
            _processHelper.StartProcessGui(
                "cmd.exe",
                $"/c cd \"{solutionDir}\" && " +
                    _gitHelper.GetSshSetup() +
                    FormatCliCommand($"checkout {_options.MasterBranch}") +
                    FormatCliCommand("pull") +
                    FormatCliCommand($"merge --no-ff {_options.FeaturePrefix}{featureName}") +
                    FormatCliCommand($"push origin {_options.MasterBranch}") +
                    FormatCliCommand($"branch -d {_options.FeaturePrefix}{featureName}") +
                    FormatCliCommand($"push origin --delete {_options.FeaturePrefix}{featureName}", false),
                $"Finishing feature {featureName}");
        }

        private void StartReleaseCommand(object sender, EventArgs e)
        {
            var solutionDir = _fileHelper.GetSolutionDir();
            if (string.IsNullOrEmpty(solutionDir)) return;
            var releaseVersion = Interaction.InputBox("Release Version:", "Start New Release");
            if (string.IsNullOrEmpty(releaseVersion)) return;

            /* 1. Switch to the develop branch
             * 2. Pull latest changes on develop
             * 3. Create and switch to a new release branch
             */
            _processHelper.StartProcessGui(
                "cmd.exe",
                $"/c cd \"{solutionDir}\" && " +
                    _gitHelper.GetSshSetup() +
                    FormatCliCommand($"checkout {_options.DevelopBranch}") +
                    FormatCliCommand("pull") +
                    FormatCliCommand($"checkout -b {_options.ReleasePrefix}{releaseVersion} {_options.DevelopBranch}", false),
                $"Starting release {releaseVersion}"
            );
        }

        private void FinishReleaseCommand(object sender, EventArgs e)
        {
            var solutionDir = _fileHelper.GetSolutionDir();
            if (string.IsNullOrEmpty(solutionDir)) return;
            var releaseName = _gitHelper.GetCurrentBranchName(true);

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
            _processHelper.StartProcessGui(
                "cmd.exe",
                $"/c cd \"{solutionDir}\" && " +
                    _gitHelper.GetSshSetup() +
                    FormatCliCommand($"checkout {_options.MasterBranch}") +
                    FormatCliCommand("pull") +
                    FormatCliCommand($"merge --no-ff {_options.ReleasePrefix}{releaseName}") +
                    FormatCliCommand($"tag {releaseName}") +
                    FormatCliCommand($"checkout {_options.DevelopBranch}") +
                    FormatCliCommand("pull") +
                    FormatCliCommand($"merge --no-ff {_options.ReleasePrefix}{releaseName}") +
                    FormatCliCommand($"push origin {_options.DevelopBranch}") +
                    FormatCliCommand($"push origin {_options.MasterBranch}") +
                    FormatCliCommand($"push origin {releaseName}") +
                    FormatCliCommand($"branch -d {_options.ReleasePrefix}{releaseName}") +
                    FormatCliCommand($"push origin --delete {_options.ReleasePrefix}{releaseName}", false),
                $"Finishing release {releaseName}"
            );
        }

        private void StartHotfixCommand(object sender, EventArgs e)
        {
            var solutionDir = _fileHelper.GetSolutionDir();
            if (string.IsNullOrEmpty(solutionDir)) return;
            var hotfixVersion = Interaction.InputBox("Hotfix Version:", "Start New Hotfix");
            if (string.IsNullOrEmpty(hotfixVersion)) return;

            /* 1. Switch to the master branch
             * 2. Pull latest changes on master
             * 3. Create and switch to a new hotfix branch
             */
            _processHelper.StartProcessGui(
                "cmd.exe",
                $"/c cd \"{solutionDir}\" && " +
                    _gitHelper.GetSshSetup() +
                    FormatCliCommand($"checkout {_options.MasterBranch}") +
                    FormatCliCommand("pull") +
                    FormatCliCommand($"checkout -b {_options.HotfixPrefix}{hotfixVersion} {_options.MasterBranch}", false),
                $"Starting hotfix {hotfixVersion}"
            );
        }

        private void FinishHotfixCommand(object sender, EventArgs e)
        {
            var solutionDir = _fileHelper.GetSolutionDir();
            if (string.IsNullOrEmpty(solutionDir)) return;
            var hotfixName = _gitHelper.GetCurrentBranchName(true);

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
            _processHelper.StartProcessGui(
                "cmd.exe",
                $"/c cd \"{solutionDir}\" && " +
                    _gitHelper.GetSshSetup() +
                    FormatCliCommand($"checkout {_options.MasterBranch}") +
                    FormatCliCommand("pull") +
                    FormatCliCommand($"merge --no-ff {_options.HotfixPrefix}{hotfixName}") +
                    FormatCliCommand($"tag {hotfixName}") +
                    FormatCliCommand($"checkout {_options.DevelopBranch}") +
                    FormatCliCommand("pull") +
                    FormatCliCommand($"merge --no-ff {_options.HotfixPrefix}{hotfixName}") +
                    FormatCliCommand($"push origin {_options.DevelopBranch}") +
                    FormatCliCommand($"push origin {_options.MasterBranch}") +
                    FormatCliCommand($"push origin {hotfixName}") +
                    FormatCliCommand($"branch -d {_options.HotfixPrefix}{hotfixName}") +
                    FormatCliCommand($"push origin --delete {_options.HotfixPrefix}{hotfixName}", false),
                $"Finishing hotfix {hotfixName}"
            );
        }
    }
}
