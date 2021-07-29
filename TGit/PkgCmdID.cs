namespace SamirBoulema.TGit
{
    static class PkgCmdIDList
    {
        public const int StartFeature = 0x100;
        public const int FinishFeature = 0x101;
        public const int StartRelease = 0x133;
        public const int FinishRelease = 0x134;
        public const int StartHotfix = 0x135;
        public const int FinishHotfix = 0x136;

        public const int StartFeatureGitHub = 0x3002;
        public const int FinishFeatureGitHub = 0x3003;

        public const int ShowChanges = 0x103;
        public const int Pull = 0x104;
        public const int Commit = 0x105;
        public const int Push = 0x106;
        public const int Fetch = 0x127;

        public const int ShowLog = 0x107;
        public const int DiskBrowser = 0x108;
        public const int RepoBrowser = 0x109;

        public const int CreateStash = 0x110;
        public const int ApplyStash = 0x111;
        public const int StashPop = 0x149;

        public const int Branch = 0x112;
        public const int Switch = 0x113;
        public const int Merge = 0x114;

        public const int Revert = 0x115;
        public const int Cleanup = 0x116;

        public const int ShowLogContext = 0x117;
        public const int DiskBrowserContext = 0x118;
        public const int RepoBrowserContext = 0x119;

        public const int BlameContext = 0x120;

        public const int MergeContext = 0x121;

        public const int PullContext = 0x122;
        public const int FetchContext = 0x128;
        public const int CommitContext = 0x123;
        public const int RevertContext = 0x124;
        public const int DiffContext = 0x125;
        public const int PrefDiffContext = 0x126;

        public const int TGitMenu = 0x1021;
        public const int TGitContextMenu = 0x1027;
        public const int TGitGitFlowMenu = 0x2000;
        public const int TGitGitHubFlowMenu = 0x3000;
        public const int TGitSvnMenu = 0x4000;

        public const int Resolve = 0x137;
        public const int Sync = 0x138;
        public const int Init = 0x139;
        public const int BrowseRef = 0x140;
        public const int Tag = 0x141;

        public const int AbortMerge = 0x142;

        public const int SvnDCommit = 0x143;
        public const int SvnFetch = 0x144;
        public const int SvnRebase = 0x145;

        public const int Rebase = 0x146;
        public const int RevGraphContext = 0x147;
        public const int RevGraph = 0x148;

        public const int AddContext = 0x150;
        public const int DeleteContext = 0x151;
        public const int DeleteKeepContext = 0x152;
    }
}