using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities
{
    namespace ConfigurableParser
    {
        interface Action
        {
            void DoAction(string line, Dictionary<string, string> variables, List<string> results);
        }

        class ActionColonParse : Action
        {
            public string LeftAs { get; set; } = "Name";
            public string RightAs { get; set; } = "Value";
            public ActionColonParse(string leftAs, string rightAs)
            {
                LeftAs = leftAs;
                RightAs = rightAs;
            }
            public void DoAction(string line, Dictionary<string, string> variables, List<string> results)
            {
                var parts = line.Split(':', 2);
                var l = parts.Length >= 1 ? parts[0] : "";
                var r = parts.Length >= 2 ? parts[1] : "";
                if (variables.ContainsKey(LeftAs)) variables[LeftAs] = l; else variables.Add(LeftAs, l);
                if (variables.ContainsKey(RightAs)) variables[RightAs] = r; else variables.Add(RightAs, r);
            }
        }
        class ActionWriteResults : Action
        {
            public List<string> Variables;
            public ActionWriteResults(List<string> varibles)
            {
                Variables = varibles;
            }
            public void DoAction(string line, Dictionary<string, string> variables, List<string> results)
            {
                var report = "";
                foreach (var item in Variables)
                {
                    if (variables.ContainsKey(item))
                    {
                        report += variables[item];
                    }
                    else
                    {
                        report += item;
                    }
                }
                results.Add(report);
            }
        }
        interface Rule
        {
            bool Matches(string line);
        }

        internal class RuleBoolean : Rule
        {
            public enum RuleType { And, Or, Not };
            private RuleType CurrRule { get; set; } = RuleType.And;
            private List<Rule> Rules { get; set; }
            public RuleBoolean(RuleType rule, List<Rule> rules)
            {
                CurrRule = rule;
                Rules = rules;
            }
            public bool Matches(string line)
            {
                bool andValue = true;
                bool orValue = false;
                bool notValue = false;
                foreach (var rule in Rules)
                {
                    var value = rule.Matches(line);
                    andValue = andValue && value;
                    orValue = orValue || value;
                    notValue = !value;
                }

                bool retval = true;
                switch (CurrRule)
                {
                    case RuleType.And: retval = andValue; break;
                    case RuleType.Not: retval = notValue; break;
                    case RuleType.Or: retval = orValue; break;
                }
                return retval;
            }
        }
        internal class RuleIndent : Rule
        {
            public enum IndentRule { Indent, NoIndent };
            private IndentRule CurrIndentRule { get; set; } = IndentRule.Indent;
            public RuleIndent(IndentRule indentRule)
            {
                CurrIndentRule = indentRule;
            }
            public bool Matches(string line)
            {
                bool hasIndent = line.StartsWith("    ");
                bool retval = true;
                switch (CurrIndentRule)
                {
                    case IndentRule.NoIndent: retval = !hasIndent; break;
                    case IndentRule.Indent: retval = hasIndent; break;
                }
                return retval;
            }
        }
        internal class RuleMatches : Rule
        {
            private string CurrMatchRule { get; set; } = "";
            public RuleMatches(string match)
            {
                CurrMatchRule = match;
            }
            public bool Matches(string line)
            {
                if (string.IsNullOrEmpty(CurrMatchRule)) return true;

                bool retval = line.Contains(CurrMatchRule);
                return retval;
            }
        }

        internal class Command
        {
            public Rule MatchRule { get; set; }
            public List<Action> MatchActions { get; set; }
            public Command(Rule rule, List<Action> actions)
            {
                MatchRule = rule;
                MatchActions = actions;
            }
        }

        class Parser
        {
            public List<Command> Commands = new List<Command>();
            public Dictionary<string, string> Variables = new Dictionary<string, string>();
            public List<string> Results = new List<string>();
            public Parser(List<Command> commands)
            {
                Commands = commands;
            }

            public void ParseLine(string line)
            {
                foreach (var command in Commands)
                {
                    var isMatch = command.MatchRule.Matches(line);
                    if (isMatch)
                    {
                        foreach (var action in command.MatchActions)
                        {
                            action.DoAction(line, Variables, Results);
                        }
                        break; // Break after matching!
                    }
                }
            }

        }

        internal static class Make
        {
            public static Parser Create_MatchSsidEncrypt()
            {
                Rule getSsidName = new RuleBoolean(RuleBoolean.RuleType.And,
                        new List<Rule>()
                        {
                            new RuleIndent(RuleIndent.IndentRule.NoIndent),
                            new RuleMatches("SSID")
                        });

                var cmdSsid = new Command(getSsidName, new List<Action>() { new ActionColonParse("junk", "ssid") });

                Rule getEncryption = new RuleBoolean(RuleBoolean.RuleType.And,
                        new List<Rule>()
                        {
                            new RuleIndent(RuleIndent.IndentRule.Indent),
                            new RuleMatches("Encryption")
                        });
                var cmdenc = new Command(getEncryption, new List<Action>() {
                    new ActionColonParse("junk", "encryption"),
                    new ActionWriteResults(new List<string>() { "FOUND! ", "encryption", " IN ", "ssid" })
                }); ;
                var retval = new Parser(new List<Command>() { cmdSsid, cmdenc });

                return retval;
            }
        }
    }
}
