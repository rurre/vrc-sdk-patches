using System;

namespace Pumkin.VrcSdkPatches
{
    public class PatchFailException : Exception
    {
        public PatchFailException(string msg) : base(msg) {}
    }
}