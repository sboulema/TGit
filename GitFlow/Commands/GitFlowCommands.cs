using SamirBoulema.TGIT.Helpers;
using Microsoft.VisualBasic;
using System;
using System.IO;
using Microsoft.VisualStudio.Shell;

namespace SamirBoulema.TGIT.Commands
{
    public class GitFlowCommands
    {
        private readonly ProcessHelper processHelper;
        private readonly CommandHelper commandHelper;
        private readonly FileHelper fileHelper;
        private readonly GitHelper gitHelper;
        private readonly OptionPageGrid options;
        private readonly string gitBin;
        private readonly OleMenuCommandService mcs;

        public GitFlowCommands(ProcessHelper processHelper, CommandHelper commandHelper, GitHelper gitHelper, FileHelper fileHelper,
            OptionPageGrid options, OleMenuCommandService mcs)
        {
            this.processHelper = processHelper;
            this.commandHelper = commandHelper;
            this.gitHelper = gitHelper;
            this.fileHelper = fileHelper;
            this.options = options;
            gitBin = fileHelper.GetMSysGit();
            this.mcs = mcs;
        }

        public void AddCommands()
        {
            //GitFlow Commands
            //Start/Finish Feature
            commandHelper.AddCommand(StartFeatureCommand, PkgCmdIDList.StartFeature);
            OleMenuCommand finishFeature = commandHelper.CreateCommand(FinishFeatureCommand, PkgCmdIDList.FinishFeature);
            finishFeature.BeforeQueryStatus += commandHelper.Feature_BeforeQueryStatus;
            mcs.AddCommand(finishFeature);

            //Start/Finish Release
            commandHelper.AddCommand(StartReleaseCommand, PkgCmdIDList.StartRelease);
            OleMenuCommand finishRelease = commandHelper.CreateCommand(FinishReleaseCommand, PkgCmdIDList.FinishRelease);
            finishRelease.BeforeQueryStatus += commandHelper.Release_BeforeQueryStatus;
            mcs.AddCommand(finishRelease);

            //Start/Finish Hotfix
            commandHelper.AddCommand(StartHotfixCommand, PkgCmdIDList.StartHotfix);
            OleMenuCommand finishHotfix = commandHelper.CreateCommand(FinishHotfixCommand, PkgCmdIDList.FinishHotfix);
            finishHotfix.BeforeQueryStatus += commandHelper.Hotfix_BeforeQueryStatus;
            mcs.AddCommand(finishHotfix);

            //GitHubFlow Commands
            //Start/Finish Feature
            commandHelper.AddCommand(StartFeatureGitHubCommand, PkgCmdIDList.StartFeatureGitHub);
            OleMenuCommand finishFeatureGitHub = commandHelper.CreateCommand(FinishFeatureGitHubCommand, PkgCmdIDList.FinishFeatureGitHub);
            finishFeatureGitHub.BeforeQueryStatus += commandHelper.Feature_BeforeQueryStatus;
            mcs.AddCommand(finishFeatureGitHub);
        }

        private void StartFeatureCommand(object sender, EventArgs e)
        {
            string solutionDir = fileHelper.GetSolutionDir();
            if (string.IsNullOrEmpty(solutionDir)) return;
            string featureName = Interaction.InputBox("Feature Name:", "Start New Feature");
            if (string.IsNullOrEmpty(featureName)) return;

            /* 1. Switch to the develop branch
             * 2. Pull latest changes on develop
             * 3. Create and switch to a new branch
             */
            processHelper.StartProcessGui(
                "cmd.exe",
                $"/c cd \"{solutionDir}\" && " +
                    FormatCLICommand($"checkout {options.DevelopBranch}") +
                    FormatCLICommand("pull") +
                    FormatCLICommand($"checkout -b {options.FeatureBranch}/{featureName} {options.DevelopBranch}", true),
                $"Starting feature {featureName}"
            );
        }

        private void StartFeatureGitHubCommand(object sender, EventArgs e)
        {
            string solutionDir = fileHelper.GetSolutionDir();
            if (string.IsNullOrEmpty(solutionDir)) return;
            string featureName = Interaction.InputBox("Feature Name:", "Start New Feature");
            if (string.IsNullOrEmpty(featureName)) return;

            /* 1. Switch to the master branch
             * 2. Pull latest changes on master
             * 3. Create and switch to a new branch
             */
            processHelper.StartProcessGui(
                "cmd.exe",
                $"/c cd \"{solutionDir}\" && " +
                    FormatCLICommand($"checkout {options.MasterBranch}") +
                    FormatCLICommand("pull") +
                    FormatCLICommand($"checkout -b {options.FeatureBranch}/{featureName} {options.MasterBranch}", true),
                $"Starting feature {featureName}"
            );
        }

        private void FinishFeatureCommand(object sender, EventArgs e)
        {
            string solutionDir = fileHelper.GetSolutionDir();
            if (string.IsNullOrEmpty(solutionDir)) return;
            string featureName = gitHelper.GetCurrentBranchName(true);

            /* 1. Switch to the develop branch
             * 2. Pull latest changes on develop
             * 3. Merge the feature branch to develop
             * 4. Push all changes to develop
             * 5. Delete the local feature branch
             * 6. Delete the remote feature branch
             */
            processHelper.StartProcessGui(
                "cmd.exe",
                $"/c cd \"{solutionDir}\" && " +
                    FormatCLICommand($"checkout {options.DevelopBranch}") +
                    FormatCLICommand("pull") +
                    FormatCLICommand($"merge --no-ff {options.FeatureBranch}/{featureName}") +
                    FormatCLICommand($"push origin {options.DevelopBranch}") + 
                    FormatCLICommand($"branch -d {options.FeatureBranch}/{featureName}") +
                    FormatCLICommand($"push origin --delete {options.FeatureBranch}/{featureName}", true), 
                $"Finishing feature {featureName}"
            );
        }

        private string FormatCLICommand(string gitCommand, bool appendNextLine = false)
        {
            return $"echo ^> {Path.GetFileNameWithoutExtension(gitBin)} {gitCommand} && \"{gitBin}\" {gitCommand}{(appendNextLine ? " && " : string.Empty)}";
        }

        private void FinishFeatureGitHubCommand(object sender, EventArgs e)
        {
            string solutionDir = fileHelper.GetSolutionDir();
            if (string.IsNullOrEmpty(solutionDir)) return;
            string featureName = gitHelper.GetCurrentBranchName(true);

            /* 1. Switch to the master branch
             * 2. Pull latest changes on master
             * 3. Merge the feature branch to master
             * 4. Push all changes to master
             * 5. Delete the local feature branch
             * 6. Delete the remote feature branch
             */
            processHelper.StartProcessGui(
                "cmd.exe",
                $"/c cd \"{solutionDir}\" && " +
                    FormatCLICommand($"checkout {options.MasterBranch}") +
                    FormatCLICommand("pull") +
                    FormatCLICommand($"merge --no-ff {options.FeatureBranch}/{featureName}") +
                    FormatCLICommand($"push origin {options.MasterBranch}") +
                    FormatCLICommand($"branch -d {options.FeatureBranch}/{featureName}") +
                    FormatCLICommand($"push origin --delete {options.FeatureBranch}/{featureName}", true),
                $"Finishing feature {featureName}");
        }

        private void StartReleaseCommand(object sender, EventArgs e)
        {
            string solutionDir = fileHelper.GetSolutionDir();
            if (string.IsNullOrEmpty(solutionDir)) return;
            string releaseVersion = Interaction.InputBox("Release Version:", "Start New Release");
            if (string.IsNullOrEmpty(releaseVersion)) return;

            /* 1. Switch to the develop branch
             * 2. Pull latest changes on develop
             * 3. Create and switch to a new release branch
             */
            processHelper.StartProcessGui(
                "cmd.exe",
                $"/c cd \"{solutionDir}\" && " +
                    FormatCLICommand($"checkout {options.DevelopBranch}") +
                    FormatCLICommand("pull") +
                    FormatCLICommand($"checkout -b {options.ReleaseBranch}/{releaseVersion} {options.DevelopBranch}", true),
                $"Starting release {releaseVersion}"
            );
        }

        private void FinishReleaseCommand(object sender, EventArgs e)
        {
            string solutionDir = fileHelper.GetSolutionDir();
            if (string.IsNullOrEmpty(solutionDir)) return;
            string releaseName = gitHelper.GetCurrentBranchName(true);

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
            processHelper.StartProcessGui(
                "cmd.exe",
                $"/c cd \"{solutionDir}\" && " +
                    FormatCLICommand($"checkout {options.MasterBranch}") +
                    FormatCLICommand("pull") +
                    FormatCLICommand($"merge --no-ff {options.ReleaseBranch}/{releaseName}") +
                    FormatCLICommand($"tag {releaseName}") +
                    FormatCLICommand($"checkout {options.DevelopBranch}") +
                    FormatCLICommand("pull") +
                    FormatCLICommand($"merge --no-ff {options.ReleaseBranch}/{releaseName}") +
                    FormatCLICommand($"push origin {options.DevelopBranch}") +
                    FormatCLICommand($"push origin {options.MasterBranch}") +
                    FormatCLICommand($"push origin {releaseName}") +
                    FormatCLICommand($"branch -d {options.ReleaseBranch}/{releaseName}") +
                    FormatCLICommand($"push origin --delete {options.ReleaseBranch}/{releaseName}", true),
                $"Finishing release {releaseName}"
            );
        }

        private void StartHotfixCommand(object sender, EventArgs e)
        {
            string solutionDir = fileHelper.GetSolutionDir();
            if (string.IsNullOrEmpty(solutionDir)) return;
            string hotfixVersion = Interaction.InputBox("Hotfix Version:", "Start New Hotfix");
            if (string.IsNullOrEmpty(hotfixVersion)) return;

            /* 1. Switch to the master branch
             * 2. Pull latest changes on master
             * 3. Create and switch to a new hotfix branch
             */
            processHelper.StartProcessGui(
                "cmd.exe",
                $"/c cd \"{solutionDir}\" && " +
                    FormatCLICommand($"checkout {options.MasterBranch}") +
                    FormatCLICommand("pull") +
                    FormatCLICommand($"checkout -b {options.HotfixBranch}/{hotfixVersion} {options.MasterBranch}", true),
                $"Starting hotfix {hotfixVersion}"
            );
        }

        private void FinishHotfixCommand(object sender, EventArgs e)
        {
            string solutionDir = fileHelper.GetSolutionDir();
            if (string.IsNullOrEmpty(solutionDir)) return;
            string hotfixName = gitHelper.GetCurrentBranchName(true);

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
            processHelper.StartProcessGui(
                "cmd.exe",
                $"/c cd \"{solutionDir}\" && " +
                    FormatCLICommand($"checkout {options.MasterBranch}") +
                    FormatCLICommand("pull") +
                    FormatCLICommand($"merge --no-ff {options.HotfixBranch}/{hotfixName}") +
                    FormatCLICommand($"tag {hotfixName}") +
                    FormatCLICommand($"checkout {options.DevelopBranch}") +
                    FormatCLICommand("pull") +
                    FormatCLICommand($"merge --no-ff {options.HotfixBranch}/{hotfixName}") +
                    FormatCLICommand($"push origin {options.DevelopBranch}") +
                    FormatCLICommand($"push origin {options.MasterBranch}") +
                    FormatCLICommand($"push origin {hotfixName}") +
                    FormatCLICommand($"branch -d {options.HotfixBranch}/{hotfixName}") +
                    FormatCLICommand($"push origin --delete {options.HotfixBranch}/{hotfixName}", true),
                $"Finishing hotfix {hotfixName}"
            );
        }
    }
}
