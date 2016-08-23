using SamirBoulema.TGit.Helpers;
using Microsoft.VisualBasic;
using System;
using System.IO;
using System.Windows.Forms;

namespace SamirBoulema.TGit.Commands
{
    public class GitFlowMenuCommands
    {
        private readonly CommandHelper _commandHelper;
        private readonly string _gitBin;
        private readonly OptionPageGrid _options;

        public GitFlowMenuCommands(CommandHelper commandHelper, OptionPageGrid options)
        {
            _commandHelper = commandHelper;
            _gitBin = FileHelper.GetMSysGit();
            _options = options;
        }

        public void AddCommands()
        {
            //GitFlow Commands
            //Start/Finish Feature
            var startFeature = _commandHelper.CreateCommand(StartFeatureCommand, PkgCmdIDList.StartFeature);
            startFeature.BeforeQueryStatus += _commandHelper.GitFlow_BeforeQueryStatus;
            _commandHelper.AddCommand(startFeature);

            var finishFeature = _commandHelper.CreateCommand(FinishFeatureCommand, PkgCmdIDList.FinishFeature);
            finishFeature.BeforeQueryStatus += _commandHelper.Feature_BeforeQueryStatus;
            _commandHelper.AddCommand(finishFeature);

            //Start/Finish Release
            var startRelease = _commandHelper.CreateCommand(StartReleaseCommand, PkgCmdIDList.StartRelease);
            startRelease.BeforeQueryStatus += _commandHelper.GitFlow_BeforeQueryStatus;
            _commandHelper.AddCommand(startRelease);

            var finishRelease = _commandHelper.CreateCommand(FinishReleaseCommand, PkgCmdIDList.FinishRelease);
            finishRelease.BeforeQueryStatus += _commandHelper.Release_BeforeQueryStatus;
            _commandHelper.AddCommand(finishRelease);

            //Start/Finish Hotfix
            var startHotfix = _commandHelper.CreateCommand(StartHotfixCommand, PkgCmdIDList.StartHotfix);
            startHotfix.BeforeQueryStatus += _commandHelper.GitFlow_BeforeQueryStatus;
            _commandHelper.AddCommand(startHotfix);

            var finishHotfix = _commandHelper.CreateCommand(FinishHotfixCommand, PkgCmdIDList.FinishHotfix);
            finishHotfix.BeforeQueryStatus += _commandHelper.Hotfix_BeforeQueryStatus;
            _commandHelper.AddCommand(finishHotfix);

            //Init
            var init = _commandHelper.CreateCommand(InitCommand, PkgCmdIDList.Init);
            init.BeforeQueryStatus += _commandHelper.GitHubFlow_BeforeQueryStatus;
            _commandHelper.AddCommand(init);

            //GitHubFlow Commands
            //Start/Finish Feature
            _commandHelper.AddCommand(StartFeatureGitHubCommand, PkgCmdIDList.StartFeatureGitHub);
            var finishFeatureGitHub = _commandHelper.CreateCommand(FinishFeatureGitHubCommand, PkgCmdIDList.FinishFeatureGitHub);
            finishFeatureGitHub.BeforeQueryStatus += _commandHelper.Feature_BeforeQueryStatus;
            _commandHelper.AddCommand(finishFeatureGitHub);
        }

        private void InitCommand(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(EnvHelper.SolutionDir)) return;

            var flowDialog = new Flow();
            if (flowDialog.ShowDialog() == DialogResult.OK)
            {
                /* 1. Add GitFlow config options
                 * 2. Checkout develop branch (create if it doesn't exist, reset if it does)
                 * 3. Push develop branch
                 */
                ProcessHelper.StartProcessGui(
                    "cmd.exe",
                    $"/c cd \"{EnvHelper.SolutionDir}\" && " +
                    GitHelper.GetSshSetup() +
                    FormatCliCommand($"config --add gitflow.branch.master {flowDialog.FlowOptions.MasterBranch}") +
                    FormatCliCommand($"config --add gitflow.branch.develop {flowDialog.FlowOptions.DevelopBranch}") +
                    FormatCliCommand($"config --add gitflow.prefix.feature {flowDialog.FlowOptions.FeaturePrefix}") +
                    FormatCliCommand($"config --add gitflow.prefix.release {flowDialog.FlowOptions.ReleasePrefix}") +
                    FormatCliCommand($"config --add gitflow.prefix.hotfix {flowDialog.FlowOptions.HotfixPrefix}") +
                    (GitHelper.RemoteBranchExists(flowDialog.FlowOptions.DevelopBranch) ?
                        "echo." : 
                        FormatCliCommand($"checkout -b {flowDialog.FlowOptions.DevelopBranch}", false)),
                    "Initializing GitFlow"
                );
            }
        }

        private void StartFeatureCommand(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(EnvHelper.SolutionDir)) return;
            var featureName = Interaction.InputBox("Feature Name:", "Start New Feature");
            if (string.IsNullOrEmpty(featureName)) return;

            var flowOptions = GitHelper.GetFlowOptions();

            /* 1. Switch to the develop branch
             * 2. Pull latest changes on develop
             * 3. Create and switch to a new branch
             */
            ProcessHelper.StartProcessGui(
                "cmd.exe",
                $"/c cd \"{EnvHelper.SolutionDir}\" && " +
                    GitHelper.GetSshSetup() +
                    FormatCliCommand($"checkout {flowOptions.DevelopBranch}") +
                    FormatCliCommand("pull") +
                    FormatCliCommand($"checkout -b {flowOptions.FeaturePrefix}{featureName} {flowOptions.DevelopBranch}", false),
                $"Starting feature {featureName}"
            );
        }

        private void StartFeatureGitHubCommand(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(EnvHelper.SolutionDir)) return;
            var featureName = Interaction.InputBox("Feature Name:", "Start New Feature");
            if (string.IsNullOrEmpty(featureName)) return;

            /* 1. Switch to the master branch
             * 2. Pull latest changes on master
             * 3. Create and switch to a new branch
             */
            ProcessHelper.StartProcessGui(
                "cmd.exe",
                $"/c cd \"{EnvHelper.SolutionDir}\" && " +
                    GitHelper.GetSshSetup() +
                    FormatCliCommand("checkout master") +
                    FormatCliCommand("pull") +
                    FormatCliCommand($"checkout -b {featureName} master", false),
                $"Starting feature {featureName}"
            );
        }

        private void FinishFeatureCommand(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(EnvHelper.SolutionDir)) return;
            var featureBranch = GitHelper.GetCurrentBranchName(false);
            var featureName = GitHelper.GetCurrentBranchName(true);
            var flowOptions = GitHelper.GetFlowOptions();

            /* 1. Switch to the develop branch
             * 2. Pull latest changes on develop
             * 3. Merge the feature branch to develop
             * 4. Push all changes to develop
             * 5. Delete the local feature branch
             * 6. Delete the remote feature branch
             */
            ProcessHelper.StartProcessGui(
                "cmd.exe",
                $"/c cd \"{EnvHelper.SolutionDir}\" && " +
                    GitHelper.GetSshSetup() +
                    FormatCliCommand($"checkout {flowOptions.DevelopBranch}") +
                    FormatCliCommand("pull") +
                    FormatCliCommand($"merge --no-ff {featureBranch}") +
                    FormatCliCommand($"push origin {flowOptions.DevelopBranch}", false),
                $"Finishing feature {featureName}",
                featureBranch, null, _options
            );
        }

        private string FormatCliCommand(string gitCommand, bool appendNextLine = true)
        {
            return $"echo ^> {Path.GetFileNameWithoutExtension(_gitBin)} {gitCommand} && \"{_gitBin}\" {gitCommand}{(appendNextLine ? " && " : string.Empty)}";
        }

        private void FinishFeatureGitHubCommand(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(EnvHelper.SolutionDir)) return;
            var featureBranch = GitHelper.GetCurrentBranchName(false);
            var featureName = GitHelper.GetCurrentBranchName(true);

            /* 1. Switch to the master branch
             * 2. Pull latest changes on master
             * 3. Merge the feature branch to master
             * 4. Push all changes to master
             * 5. Delete the local feature branch
             * 6. Delete the remote feature branch
             */
            ProcessHelper.StartProcessGui(
                "cmd.exe",
                $"/c cd \"{EnvHelper.SolutionDir}\" && " +
                    GitHelper.GetSshSetup() +
                    FormatCliCommand("checkout master") +
                    FormatCliCommand("pull") +
                    FormatCliCommand($"merge --no-ff {featureBranch}") +
                    FormatCliCommand("push origin master", false),
                $"Finishing feature {featureName}",
                featureBranch, null, _options);
        }

        private void StartReleaseCommand(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(EnvHelper.SolutionDir)) return;
            var releaseVersion = Interaction.InputBox("Release Version:", "Start New Release");
            if (string.IsNullOrEmpty(releaseVersion)) return;

            var flowOptions = GitHelper.GetFlowOptions();

            /* 1. Switch to the develop branch
             * 2. Pull latest changes on develop
             * 3. Create and switch to a new release branch
             */
            ProcessHelper.StartProcessGui(
                "cmd.exe",
                $"/c cd \"{EnvHelper.SolutionDir}\" && " +
                    GitHelper.GetSshSetup() +
                    FormatCliCommand($"checkout {flowOptions.DevelopBranch}") +
                    FormatCliCommand("pull") +
                    FormatCliCommand($"checkout -b {flowOptions.ReleasePrefix}{releaseVersion} {flowOptions.DevelopBranch}", false),
                $"Starting release {releaseVersion}"
            );
        }

        private void FinishReleaseCommand(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(EnvHelper.SolutionDir)) return;
            var releaseBranch = GitHelper.GetCurrentBranchName(false);
            var releaseName = GitHelper.GetCurrentBranchName(true);
            var flowOptions = GitHelper.GetFlowOptions();

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
            ProcessHelper.StartProcessGui(
                "cmd.exe",
                $"/c cd \"{EnvHelper.SolutionDir}\" && " +
                    GitHelper.GetSshSetup() +
                    FormatCliCommand($"checkout {flowOptions.MasterBranch}") +
                    FormatCliCommand("pull") +
                    FormatCliCommand($"merge --no-ff {releaseBranch}") +
                    FormatCliCommand($"tag {releaseName}") +
                    FormatCliCommand($"checkout {flowOptions.DevelopBranch}") +
                    FormatCliCommand("pull") +
                    FormatCliCommand($"merge --no-ff {releaseBranch}") +
                    FormatCliCommand($"push origin {flowOptions.DevelopBranch}") +
                    FormatCliCommand($"push origin {flowOptions.MasterBranch}") +
                    FormatCliCommand($"push origin {releaseName}", false),
                $"Finishing release {releaseName}",
                releaseBranch, null, _options
            );
        }

        private void StartHotfixCommand(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(EnvHelper.SolutionDir)) return;
            var hotfixVersion = Interaction.InputBox("Hotfix Version:", "Start New Hotfix");
            if (string.IsNullOrEmpty(hotfixVersion)) return;

            var flowOptions = GitHelper.GetFlowOptions();

            /* 1. Switch to the master branch
             * 2. Pull latest changes on master
             * 3. Create and switch to a new hotfix branch
             */
            ProcessHelper.StartProcessGui(
                "cmd.exe",
                $"/c cd \"{EnvHelper.SolutionDir}\" && " +
                    GitHelper.GetSshSetup() +
                    FormatCliCommand($"checkout {flowOptions.MasterBranch}") +
                    FormatCliCommand("pull") +
                    FormatCliCommand($"checkout -b {flowOptions.HotfixPrefix}{hotfixVersion} {flowOptions.MasterBranch}", false),
                $"Starting hotfix {hotfixVersion}"
            );
        }

        private void FinishHotfixCommand(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(EnvHelper.SolutionDir)) return;
            var hotfixBranch = GitHelper.GetCurrentBranchName(false);
            var hotfixName = GitHelper.GetCurrentBranchName(true);
            var flowOptions = GitHelper.GetFlowOptions();

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
            ProcessHelper.StartProcessGui(
                "cmd.exe",
                $"/c cd \"{EnvHelper.SolutionDir}\" && " +
                    GitHelper.GetSshSetup() +
                    FormatCliCommand($"checkout {flowOptions.MasterBranch}") +
                    FormatCliCommand("pull") +
                    FormatCliCommand($"merge --no-ff {hotfixBranch}") +
                    FormatCliCommand($"tag {hotfixName}") +
                    FormatCliCommand($"checkout {flowOptions.DevelopBranch}") +
                    FormatCliCommand("pull") +
                    FormatCliCommand($"merge --no-ff {hotfixBranch}") +
                    FormatCliCommand($"push origin {flowOptions.DevelopBranch}") +
                    FormatCliCommand($"push origin {flowOptions.MasterBranch}") +
                    FormatCliCommand($"push origin {hotfixName}", false),
                $"Finishing hotfix {hotfixName}",
                hotfixBranch, null, _options
            );
        }
    }
}
