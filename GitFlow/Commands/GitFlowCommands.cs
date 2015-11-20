using EnvDTE;
using SamirBoulema.TGIT.Helpers;
using Microsoft.VisualBasic;
using System;

namespace SamirBoulema.TGIT.Commands
{
    public class GitFlowCommands
    {
        private ProcessHelper processHelper;
        private CommandHelper commandHelper;
        private FileHelper fileHelper;
        private GitHelper gitHelper;
        private DTE dte;
        private OptionPageGrid options;
        private string gitBin;

        public GitFlowCommands(ProcessHelper processHelper, CommandHelper commandHelper, GitHelper gitHelper, FileHelper fileHelper,
            DTE dte, OptionPageGrid options)
        {
            this.processHelper = processHelper;
            this.commandHelper = commandHelper;
            this.gitHelper = gitHelper;
            this.fileHelper = fileHelper;
            this.dte = dte;
            this.options = options;
            gitBin = fileHelper.GetMSysGit();
        }

        public void AddCommands()
        {
            commandHelper.AddCommand(StartFeatureCommand, PkgCmdIDList.StartFeature);
            commandHelper.AddCommand(FinishFeatureCommand, PkgCmdIDList.FinishFeature);
            commandHelper.AddCommand(StartReleaseCommand, PkgCmdIDList.StartRelease);
            commandHelper.AddCommand(FinishReleaseCommand, PkgCmdIDList.FinishRelease);
            commandHelper.AddCommand(StartHotfixCommand, PkgCmdIDList.StartHotfix);
            commandHelper.AddCommand(FinishHotfixCommand, PkgCmdIDList.FinishHotfix);

            commandHelper.AddCommand(StartFeatureGitHubCommand, PkgCmdIDList.StartFeatureGitHub);
            commandHelper.AddCommand(FinishFeatureGitHubCommand, PkgCmdIDList.FinishFeatureGitHub);
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
                string.Format("/c cd \"{1}\" && " +
                    "echo ^> git checkout {2} && \"{4}\" checkout {2} && " +
                    "echo ^> git pull && \"{4}\" pull && " +
                    "echo ^> git checkout -b {3}/{0} {2} && \"{4}\" checkout -b {3}/{0} {2}",
                    featureName, solutionDir, options.DevelopBranch, options.FeatureBranch, gitBin),
                string.Format("Starting feature {0}", featureName)
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
                $"echo ^> git checkout {options.MasterBranch} && \"{gitBin}\" checkout {options.MasterBranch} && " +
                $"echo ^> git pull && \"{gitBin}\" pull && " +
                $"echo ^> git checkout -b {options.FeatureBranch}/{featureName} {options.MasterBranch} && \"{gitBin}\" checkout -b {options.FeatureBranch}/{featureName} {options.MasterBranch}",
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
             * 4. Delete the local feature branch
             * 5. Delete the remote feature branch
             * 6. Push all changes to develop
             */
            processHelper.StartProcessGui(
                "cmd.exe",
                string.Format("/c cd \"{1}\" && " +
                    "echo ^> git checkout {2} && \"{4}\" checkout {2} && " +
                    "echo ^> git pull && \"{4}\" pull && " +
                    "echo ^> git merge --no-ff {3}/{0} && \"{4}\" merge --no-ff {3}/{0} && " +
                    "echo ^> git branch -d {3}/{0} && \"{4}\" branch -d {3}/{0} && " +
                    "echo ^> git push origin --delete {3}/{0} && \"{4}\" push origin --delete {3}/{0} && " +
                    "echo ^> git push origin {2} && \"{4}\" push origin {2}",
                    featureName, solutionDir, options.DevelopBranch, options.FeatureBranch, gitBin),
                string.Format("Finishing feature {0}", featureName));
        }

        private void FinishFeatureGitHubCommand(object sender, EventArgs e)
        {
            string solutionDir = fileHelper.GetSolutionDir();
            if (string.IsNullOrEmpty(solutionDir)) return;
            string featureName = gitHelper.GetCurrentBranchName(true);

            /* 1. Switch to the master branch
             * 2. Pull latest changes on master
             * 3. Merge the feature branch to master
             * 4. Delete the local feature branch
             * 5. Delete the remote feature branch
             * 6. Push all changes to master
             */
            processHelper.StartProcessGui(
                "cmd.exe",
                $"/c cd \"{solutionDir}\" && " +
                $"echo ^> git checkout {options.MasterBranch} && \"{gitBin}\" checkout {options.MasterBranch} && " +
                $"echo ^> git pull && \"{gitBin}\" pull && " +
                $"echo ^> git merge --no-ff {options.FeatureBranch}/{featureName} && \"{gitBin}\" merge --no-ff {options.FeatureBranch}/{featureName} && " +
                $"echo ^> git branch -d {options.FeatureBranch}/{featureName} && \"{gitBin}\" branch -d {options.FeatureBranch}/{featureName} && " +
                $"echo ^> git push origin --delete {options.FeatureBranch}/{featureName} && \"{gitBin}\" push origin --delete {options.FeatureBranch}/{featureName} && " +
                $"echo ^> git push origin {options.MasterBranch} && \"{gitBin}\" push origin {options.MasterBranch}",
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
                string.Format("/c cd \"{1}\" && " +
                    "echo ^> git checkout {2} && \"{4}\" checkout {2} && " +
                    "echo ^> git pull && \"{4}\" pull && " +
                    "echo ^> git checkout -b {3}/{0} {2} && \"{4}\" checkout -b {3}/{0} {2}",
                    releaseVersion, solutionDir, options.DevelopBranch, options.ReleaseBranch, gitBin),
                string.Format("Starting release {0}", releaseVersion)
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
             * 8. Delete the local release branch
             * 9. Delete the remote release branch
             * 10. Push all changes to develop
             * 11. Push all changes to master
             * 12. Push the tag
             */
            processHelper.StartProcessGui(
                "cmd.exe",
                string.Format("/c cd \"{1}\" && " +
                    "echo ^> git checkout {4} && \"{5}\" checkout {4} && " +
                    "echo ^> git pull && \"{5}\" pull && " +
                    "echo ^> git merge --no-ff {3}/{0} && \"{5}\" merge --no-ff {3}/{0} && " +
                    "echo ^> git tag {0} && \"{5}\" tag {0} && " +
                    "echo ^> git checkout {2} && \"{5}\" checkout {2} && " +
                    "echo ^> git pull && \"{5}\" pull && " +
                    "echo ^> git merge --no-ff {3}/{0} && \"{5}\" merge --no-ff {3}/{0} && " +
                    "echo ^> git branch -d {3}/{0} && \"{5}\" branch -d {3}/{0} && " +
                    "echo ^> git push origin --delete {3}/{0} && \"{5}\" push origin --delete {3}/{0} && " +
                    "echo ^> git push origin {2} && \"{5}\" push origin {2} && " +
                    "echo ^> git push origin {4} && \"{5}\" push origin {4} && " +
                    "echo ^> git push origin {0} && \"{5}\" push origin {0}",
                    releaseName, solutionDir, options.DevelopBranch, options.ReleaseBranch, options.MasterBranch, gitBin),
                string.Format("Finishing release {0}", releaseName));
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
                string.Format("/c cd \"{1}\" && " +
                    "echo ^> git checkout {2} && \"{4}\" checkout {2} && " +
                    "echo ^> git pull && \"{4}\" pull && " +
                    "echo ^> git checkout -b {3}/{0} {2} && \"{4}\" checkout -b {3}/{0} {2}",
                    hotfixVersion, solutionDir, options.MasterBranch, options.HotfixBranch, gitBin),
                string.Format("Starting hotfix {0}", hotfixVersion)
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
             * 4. Tag the release
             * 5. Switch to the develop branch
             * 6. Pull latest changes on develop
             * 7. Merge the hotfix branch to develop
             * 8. Delete the local hotfix branch
             * 9. Delete the remote hotfix branch
             * 10. Push all changes to develop
             * 11. Push all changes to master
             * 12. Push the tag
             */
            processHelper.StartProcessGui(
                "cmd.exe",
                string.Format("/c cd \"{1}\" && " +
                    "echo ^> git checkout {4} && \"{5}\" checkout {4} && " +
                    "echo ^> git pull && \"{5}\" pull && " +
                    "echo ^> git merge --no-ff {3}/{0} && \"{5}\" merge --no-ff {3}/{0} && " +
                    "echo ^> git tag {0} && \"{5}\" tag {0} && " +
                    "echo ^> git checkout {2} && \"{5}\" checkout {2} && " +
                    "echo ^> git pull && \"{5}\" pull && " +
                    "echo ^> git merge --no-ff {3}/{0} && \"{5}\" merge --no-ff {3}/{0} && " +
                    "echo ^> git branch -d {3}/{0} && \"{5}\" branch -d {3}/{0} && " +
                    "echo ^> git push origin --delete {3}/{0} && \"{5}\" push origin --delete {3}/{0} && " +
                    "echo ^> git push origin {2} && \"{5}\" push origin {2} && " +
                    "echo ^> git push origin {4} && \"{5}\" push origin {4} && " +
                    "echo ^> git push origin {0} && \"{5}\" push origin {0}",
                    hotfixName, solutionDir, options.DevelopBranch, options.HotfixBranch, options.MasterBranch, gitBin),
                string.Format("Finishing hotfix {0}", hotfixName));
        }
    }
}
