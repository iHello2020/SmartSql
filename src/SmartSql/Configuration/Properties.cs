﻿using SmartSql.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SmartSql.Configuration
{
    public class Properties
    {
        private IDictionary<string, string> _properties;
        private Regex _propertyTokens;
        public Properties()
        {
            _properties = new Dictionary<string, string>();
            var regOptions = RegexOptions.Multiline | RegexOptions.CultureInvariant | RegexOptions.Compiled;
            _propertyTokens = new Regex(@"^\$\{([\p{L}\p{N}_]+)\}", regOptions);
        }

        public void Import(IEnumerable<KeyValuePair<string, object>> properties)
        {
            foreach (var property in properties)
            {
                _properties.Add(property.Key, property.Value.ToString());
            }
        }
        public void Import(IEnumerable<KeyValuePair<string, string>> properties)
        {
            foreach (var property in properties)
            {
                _properties.Add(property.Key, property.Value);
            }
        }

        public string GetPropertyValue(string propExp)
        {
            if (!_propertyTokens.IsMatch(propExp)) { return propExp; }
            return _propertyTokens.Replace(propExp, match =>
            {
                string propName = match.Groups[1].Value;
                if (!_properties.TryGetValue(propName, out var propVal))
                {
                    throw new SmartSqlException($"can not find Property.Name:{propName}.");
                }
                return propVal;
            });
        }
    }
}