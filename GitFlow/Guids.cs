// Guids.cs
// MUST match guids.h
using System;

namespace FundaRealEstateBV.GitFlow
{
    static class GuidList
    {
        public const string guidGitFlowPkgString = "6c58d6e5-11ba-43ba-9a63-a624f264ec06";
        public const string guidGitFlowCmdSetString = "6a672d8c-b58f-4329-9bc9-015fc15f9d1a";

        public static readonly Guid guidGitFlowCmdSet = new Guid(guidGitFlowCmdSetString);
    };
}