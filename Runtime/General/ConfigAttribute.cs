namespace Unibrics.Configuration.General
{
    using System;
    using Core;
    using JetBrains.Annotations;

    [AttributeUsage(AttributeTargets.Interface), MeansImplicitUse]
    public class ConfigAttribute : Attribute
    {
        public string Key { get; }
        
        public Type ImplementedBy { get; }
        
        public bool LocalOnly { get; set; }
        
        public bool IsMultiConfig { get; set; }
        
        public bool IsOptional { get; set; }

        public Priority Priority { get; set; } = Priority.Simple;

        public ConfigAttribute(string key, Type implementedBy)
        {
            Key = key;
            ImplementedBy = implementedBy;
        }
    }
}