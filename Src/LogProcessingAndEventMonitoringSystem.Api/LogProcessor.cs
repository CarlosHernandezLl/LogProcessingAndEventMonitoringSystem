using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using LogProcessingAndEventMonitoringSystem.Core;

namespace LogProcessingAndEventMonitoringSystem
{
    public class LogProcessor
    {
        private readonly List<ILogProcessingRule> _rules;

        public LogProcessor()
        {
            _rules = LoadRulesFromAssembly("LogProcessingAndEventMonitoringSystem.Plugins.dll");
            Console.WriteLine($"[LogProcessor] Cargadas {_rules.Count} reglas dinámicamente.");
        }


        private List<ILogProcessingRule> LoadRulesFromAssembly(string assemblyPath)
        {
            var rules = new List<ILogProcessingRule>();
            try
            {
                var assembly = Assembly.LoadFrom(assemblyPath);
                var ruleTypes = assembly.GetTypes()
                                        .Where(t => typeof(ILogProcessingRule).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);
                foreach (var type in ruleTypes)
                {
                    if (Activator.CreateInstance(type) is ILogProcessingRule ruleInstance)
                    {
                        rules.Add(ruleInstance);
                        Console.WriteLine($"[LogProcessor] Regla cargada: {type.FullName}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading rules from assembly: {ex.Message}");
            }
            return rules;
        
        }


        public List<string?> ProccessLogEntry(LogEntry logEntry)
        {
            var failureMessages = new List<string?>();
            foreach (var rule in _rules)
            {
                if (!rule.Apply(logEntry, out var failureMessage) && failureMessage != null)
                {
                    failureMessages.Add(failureMessage);
                }
            }
            return failureMessages;
        }

    }

}