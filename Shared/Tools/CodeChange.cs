using System;
using System.Reflection;
using HarmonyLib;

namespace Shared.Tools
{
    public class CodeChange
    {
        public readonly MethodInfo Method;
        public readonly ConstructorInfo Constructor;
        public readonly string Expected;
        public readonly string Actual;

        public readonly Type PatchType;

        public CodeChange(MethodInfo method, ConstructorInfo constructor, string expected, string actual, Type patchType)
        {
            Method = method;
            Constructor = constructor;
            Expected = expected;
            Actual = actual;
            PatchType = patchType;
        }

        public override string ToString() => $"Code change in {Method?.FullDescription() ?? Constructor?.FullDescription() ?? "the target of " + PatchType.Name}; expected {Expected}, actual {Actual}";
    }
}