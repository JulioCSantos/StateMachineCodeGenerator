using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace StateMachineCodeGeneratorSystem.Templates
{
    //public static class TemplateExtensions
    //{
    //    public static string Indent { get; set; } = "    ";

    //    public static string Tab(this StateMachineBaseTemplate tt, int tabsCount)
    //    {
    //        if (tabsCount <= 0) throw new ArgumentOutOfRangeException();

    //        tt.ClearIndent();
    //        for (int i = 0; i < tabsCount; i++)
    //        {
    //            tt.PushIndent(Indent);
    //        }
    //        return tt.CurrentIndent;
    //    }

    //    public static string PT(this StateMachineBaseTemplate tt, int tabsCount)
    //    {
    //        var offset = string.Empty;
    //        for (int i = 0; i < Math.Abs(tabsCount); i++)
    //        {
    //            if (tabsCount > 0) offset += Indent;
    //            else offset = offset.Substring(0, offset.Length - tabsCount);
    //        }
    //        return offset;
    //    }

    //    public static void WriteTokensAtTabs(this StateMachineBaseTemplate tt, params object[] tabContentPair)
    //    {
    //        if (tabContentPair == null) return;
    //        if (Math.IEEERemainder(tabContentPair.Length, 2) != 0) return;

    //        var previousIndent = tt.CurrentIndent;
    //        tt.ClearIndent();
    //        for (int i = 0; i < tabContentPair.Length; i = i + 2)
    //        {
    //            int tab;
    //            if (!int.TryParse(tabContentPair[i].ToString(), out tab)) return;
    //            var content = tabContentPair[i + 1].ToString();
    //            tt.AppendToLineAtTab(tab, content);
    //        }

    //        tt.WriteLine("");
    //        tt.PushIndent(previousIndent);
    //    }

    //    public static string AppendToLineAtTab(this StateMachineBaseTemplate tt, int tab, string textToAppend)
    //    {
    //        var lastLine = tt.GetLastLine();
    //        var allFill = string.Concat(Enumerable.Repeat(Indent, tab));

    //        if (lastLine.Length >= allFill.Length) return lastLine;
    //        var fill = allFill.Substring(0, allFill.Length - lastLine.Length);
    //        var fillAppended = fill + textToAppend;
    //        tt.Write(fillAppended);
    //        return fillAppended;
    //    }

    //    public static string GetLastLine(this StateMachineBaseTemplate tt) {
    //        var lineInfo = tt.GetType().GetProperty("GenerationEnvironment", BindingFlags.NonPublic | BindingFlags.Instance);
    //        var allText = lineInfo.GetValue(tt, null);
    //        var nl = "\n";
    //        var tab = "\t";
    //        var lines = allText.ToString().Split(Environment.NewLine.ToCharArray()[0]);
    //        var lastLine = lines.LastOrDefault().Replace(nl, "").Replace(tab, Indent);

    //        return lastLine;
    //    }

    //}

    public static class Ext
    {
        public static string Indent { get; set; } = "    ";

        public static string Tab(this StateMachineBaseTemplate tt, int tabsCount)
        {
            if (tabsCount <= 0) throw new ArgumentOutOfRangeException();

            tt.ClearIndent();
            for (int i = 0; i < tabsCount; i++)
            {
                tt.PushIndent(Indent);
            }
            return tt.CurrentIndent;
        }

        public static string PT(this StateMachineBaseTemplate tt, int tabsCount)
        {
            var offset = string.Empty;
            for (int i = 0; i < Math.Abs(tabsCount); i++)
            {
                if (tabsCount > 0) offset += Indent;
                else offset = offset.Substring(0, offset.Length - tabsCount);
            }
            return offset;
        }

        public static void WriteTokensAtTabs(this StateMachineBaseTemplate tt, params object[] tabContentPair)
        {
            if (tabContentPair == null) return;
            if (Math.IEEERemainder(tabContentPair.Length, 2) != 0) return;

            var previousIndent = tt.CurrentIndent;
            tt.ClearIndent();
            for (int i = 0; i < tabContentPair.Length; i = i + 2)
            {
                int tab;
                if (!int.TryParse(tabContentPair[i].ToString(), out tab)) return;
                var content = tabContentPair[i + 1].ToString();
                tt.AppendToLineAtTab(tab, content);
            }

            tt.WriteLine("");
            tt.PushIndent(previousIndent);
        }

        public static void WriteTokensAtTabs(this StateMachineModelBaseTemplate tt, params object[] tabContentPair)
        {
            if (tabContentPair == null) return;
            if (Math.IEEERemainder(tabContentPair.Length, 2) != 0) return;

            var previousIndent = tt.CurrentIndent;
            tt.ClearIndent();
            for (int i = 0; i < tabContentPair.Length; i = i + 2)
            {
                int tab;
                if (!int.TryParse(tabContentPair[i].ToString(), out tab)) return;
                var content = tabContentPair[i + 1].ToString();
                tt.AppendToLineAtTab(tab, content);
            }

            tt.WriteLine("");
            tt.PushIndent(previousIndent);
        }

        public static string AppendToLineAtTab(this StateMachineBaseTemplate tt, int tab, string textToAppend)
        {
            var lastLine = tt.GetLastLine();
            var allFill = string.Concat(Enumerable.Repeat(Indent, tab));

            if (lastLine.Length >= allFill.Length) return lastLine;
            var fill = allFill.Substring(0, allFill.Length - lastLine.Length);
            var fillAppended = fill + textToAppend;
            tt.Write(fillAppended);
            return fillAppended;
        }

        public static string AppendToLineAtTab(this StateMachineModelBaseTemplate tt, int tab, string textToAppend)
        {
            var lastLine = tt.GetLastLine();
            var allFill = string.Concat(Enumerable.Repeat(Indent, tab));

            if (lastLine.Length >= allFill.Length) return lastLine;
            var fill = allFill.Substring(0, allFill.Length - lastLine.Length);
            var fillAppended = fill + textToAppend;
            tt.Write(fillAppended);
            return fillAppended;
        }

        public static string GetLastLine(this StateMachineBaseTemplate tt)
        {
            var lineInfo = tt.GetType().GetProperty("GenerationEnvironment", BindingFlags.NonPublic | BindingFlags.Instance);
            var allText = lineInfo.GetValue(tt, null);
            var nl = "\n";
            var tab = "\t";
            var lines = allText.ToString().Split(Environment.NewLine.ToCharArray()[0]);
            var lastLine = lines.LastOrDefault().Replace(nl, "").Replace(tab, Indent);

            return lastLine;
        }

        public static string GetLastLine(this StateMachineModelBaseTemplate tt)
        {
            var lineInfo = tt.GetType().GetProperty("GenerationEnvironment", BindingFlags.NonPublic | BindingFlags.Instance);
            var allText = lineInfo.GetValue(tt, null);
            var nl = "\n";
            var tab = "\t";
            var lines = allText.ToString().Split(Environment.NewLine.ToCharArray()[0]);
            var lastLine = lines.LastOrDefault().Replace(nl, "").Replace(tab, Indent);

            return lastLine;
        }
    }

}
