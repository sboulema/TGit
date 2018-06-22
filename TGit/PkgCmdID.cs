namespace SamirBoulema.TGit
{
    static class PkgCmdIDList
    {
        public const uint StartFeature = 0x100;
        public const uint FinishFeature = 0x101;
        public const uint StartRelease = 0x133;
        public const uint FinishRelease = 0x134;
        public const uint StartHotfix = 0x135;
        public const uint FinishHotfix = 0x136;

        public const uint StartFeatureGitHub = 0x3002;
        public const uint FinishFeatureGitHub = 0x3003;

        public const uint ShowChanges = 0x103;
        public const uint Pull = 0x104;
        public const uint Commit = 0x105;
        public const uint Push = 0x106;
        public const uint Fetch = 0x127;

        public const uint ShowLog = 0x107;
        public const uint DiskBrowser = 0x108;
        public const uint RepoBrowser = 0x109;

        public const uint CreateStash = 0x110;
        public const uint ApplyStash = 0x111;

        public const uint Branch = 0x112;
        public const uint Switch = 0x113;
        public const uint Merge = 0x114;

        public const uint Revert = 0x115;
        public const uint Cleanup = 0x116;

        public const uint ShowLogContext = 0x117;
        public const uint DiskBrowserContext = 0x118;
        public const uint RepoBrowserContext = 0x119;

        public const uint BlameContext = 0x120;

        public const uint MergeContext = 0x121;

        public const uint PullContext = 0x122;
        public const uint FetchContext = 0x128;
        public const uint CommitContext = 0x123;
        public const uint RevertContext = 0x124;
        public const uint DiffContext = 0x125;
        public const uint PrefDiffContext = 0x126;

        public const uint TGitMenu = 0x1021;
        public const uint TGitContextMenu = 0x1027;
        public const uint TGitGitFlowMenu = 0x2000;
        public const uint TGitGitHubFlowMenu = 0x3000;
        public const uint TGitSvnMenu = 0x4000;

        public const uint Resolve = 0x137;
        public const uint Sync = 0x138;
        public const uint Init = 0x139;
        public const uint BrowseRef = 0x140;
        public const uint Tag = 0x141;

        public const uint AbortMerge = 0x142;

        public const uint SvnDCommit = 0x143;
        public const uint SvnFetch = 0x144;
        public const uint SvnRebase = 0x145;
    }
}