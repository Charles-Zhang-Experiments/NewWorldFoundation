using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using Humanizer;

namespace NWF.Shared.Helpers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter)]
    public class CommentAttribute : Attribute
    {
        public string Comment { get; }
        public string ItemName { get; set; }
        public CommentAttribute(string comment, [CallerMemberName] string propertyName = null /* Works for properties and fields except Record type constructor parameters*/)
        {
            Comment = comment;
            ItemName = propertyName;
        }
    }

    public static class CLI
    {
        #region Shorthand Entry
        /// <summary>
        /// Returns whether caller should continue
        /// </summary>
        public static bool AutoParse<TType>(string[] arguments, out TType result)
        {
            result = Parse<TType>(arguments);

            if (arguments.Length == 0 || (GetAllValues(result).TryGetValue("Help", out object help) && (bool)help))
            {       
                Console.WriteLine(Document<TType>());
                return false;
            }
            return true;
        }
        public static Dictionary<string, object>GetAllValues<TType>(TType instance)
        {
            Type type = instance.GetType();
            var properties = type.GetProperties()
                .ToDictionary(p => p.Name.ToLower(), p => p.GetValue(instance));
            var attributes = type.GetFields()
                .ToDictionary(p => p.Name.ToLower(), p => p.GetValue(instance));
            return properties
                .Concat(attributes)
                .ToDictionary();
        }
        #endregion

        #region Main Methods
        /// <summary>
        /// Maps properties in order.
        /// </summary>
        public static TType ParsePositional<TType>(params string[] arguments)
        {
            var type = typeof(TType);
            var properties = type.GetProperties();
            if (arguments.Length != properties.Length)
                throw new ArgumentException($"Invalid number of arguments, expect {properties.Length}, get {arguments.Length}");

            var instance = Activator.CreateInstance(type);
            for (int i = 0; i < properties.Length; i++)
            {
                System.Reflection.PropertyInfo property = properties[i];
                property.SetValue(instance, ConvertValue($"Property {property.Name}", property.PropertyType, new string[] { arguments[i] }));
            }
            return (TType)instance;
        }
        /// <summary>
        /// Parse arguments in a keyword list fashion: e.g. --Keyword [values].
        /// Keyword is case insensitive.
        /// 
        /// Also handles toggles. Automatically handles array.
        /// </summary>
        public static TType Parse<TType>(params string[] arguments)
        {
            var type = typeof(TType);
            var properties = type.GetProperties()
                .ToDictionary(p => p.Name.ToLower(), p => p);
            var attributes = type.GetFields()
                .ToDictionary(p => p.Name.ToLower(), p => p);

            // Instantiate
            object instance;
            if (type.GetConstructor(Type.EmptyTypes) == null)
                instance = System.Runtime.Serialization.FormatterServices.GetUninitializedObject(type);
            else
                instance = Activator.CreateInstance(type);

            // Initialize all arrays
            foreach (var p in properties.Where(p => p.Value.PropertyType.IsArray))
                p.Value.SetValue(instance, Array.CreateInstance(p.Value.PropertyType.GetElementType(), 0));
            foreach (var a in attributes.Where(a => a.Value.FieldType.IsArray))
                a.Value.SetValue(instance, Array.CreateInstance(a.Value.FieldType.GetElementType(), 0));

            Dictionary<string, string[]> mapping = MapMany(true, arguments);
            foreach ((string Key, string[] Values) in mapping)
            {
                string lowerKey = Key.ToLower();
                if (properties.TryGetValue(lowerKey, out System.Reflection.PropertyInfo property))
                    property.SetValue(instance, ConvertValue($"Property {property.Name}", property.PropertyType, Values));
                else if (attributes.TryGetValue(lowerKey, out System.Reflection.FieldInfo attribute))
                    attribute.SetValue(instance, ConvertValue($"Attribute {attribute.Name}", attribute.FieldType, Values));
                else
                    throw new ArgumentException($"Cannot find definition for: {Key} in type {type.Name}");
            }
            return (TType)instance;
        }

        /// <summary>
        /// Map 1-1 dictionary;
        /// Don't allow toggles.
        /// </summary>
        public static Dictionary<string, string> Map(params string[] arguments)
        {
            if (arguments.Length % 2 != 0)
                throw new ArgumentException("Non-even number of arguments.");

            Dictionary<string, string> map = new();
            for (int i = 0; i < arguments.Length; i += 2)
                map[arguments[i]] = arguments[i + 1];
            return map;
        }
        /// <summary>
        /// Map 1-many dictionary; Conventional --keyword;
        /// Also handles toggles.
        /// </summary>
        public static Dictionary<string, string[]> MapMany(bool trim, params string[] arguments)
        {
            Dictionary<string, List<string>> map = new();
            string key = null;
            for (int i = 0; i < arguments.Length; i++)
            {
                if (arguments[i].StartsWith("--"))
                {
                    key = arguments[i];
                    if (trim)
                        key = key.TrimStart('-');
                    map[key] = new();
                }
                else
                    map[key].Add(arguments[i]);
            }
            return map.ToDictionary(m => m.Key, m => m.Value.ToArray());
        }
        /// <summary>
        /// Generate documentation for type
        /// </summary>
        public static string Document<TType>()
        {
            Type type = typeof(TType);

            string programName = Regex.Replace(type.Name, @"Arguments$", string.Empty);
            string typeComment = type.GetCustomAttribute<CommentAttribute>()?.Comment;
            var fieldComments = type.GetFields()
                .Select(f => f.GetCustomAttribute<CommentAttribute>())
                .Where(a => a != null);
            var propertyComments = type.GetProperties()
                .Select(p => p.GetCustomAttribute<CommentAttribute>())
                .Where(a => a != null);
            var constructorParameterComments = type.GetConstructors()
                .Single()
                .GetParameters()    // Attributes on record type constructor parameters are not carried to the corresponding properties
                .Select(p => Make(p.GetCustomAttribute<CommentAttribute>(), p.Name))
                .Where(a => a != null);
            var optionComments = fieldComments
                .Concat(propertyComments)
                .Concat(constructorParameterComments)
                .ToArray();

            var properties = type.GetProperties()
                .ToDictionary(p => p.Name, p => p.PropertyType);
            var attributes = type.GetFields()
                .ToDictionary(p => p.Name, p => p.FieldType);
            var typeMapping = properties.Concat(attributes)
                .ToDictionary(p => p.Key, p => p.Value);

            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"{programName}:");
            if (typeComment != null)
                builder.AppendLine($"  {typeComment}");
            builder.AppendLine();
            builder.AppendLine("Options: ");
            foreach (CommentAttribute item in optionComments)
                builder.AppendLine($"  --{item.ItemName}: {item.Comment}");
            builder.AppendLine();
            builder.AppendLine("Usage Examples: ");
            for (int i = 0; i < new Random().Next(1, optionComments.Length); i++)
                builder.AppendLine($"  {programName} {GenerateUsageExample(optionComments, typeMapping)}");
            return builder.ToString().TrimEnd();

            static CommentAttribute Make(CommentAttribute original, string name)
            {
                original.ItemName = name;
                return original;
            }
            static string GenerateUsageExample(CommentAttribute[] optionComments, Dictionary<string, Type> typeMapping)
            {
                StringBuilder example = new();
                Random.Shared.Shuffle(optionComments);
                int sampleSize = new Random().Next(Math.Min(1, optionComments.Length), optionComments.Length + 1);
                foreach (CommentAttribute item in optionComments.Take(sampleSize))
                {
                    if (typeMapping[item.ItemName] == typeof(bool))
                        example.Append($"--{item.ItemName}");
                    else
                        example.Append($"--{item.ItemName} <{item.ItemName.Humanize()} Value> ");
                }
                return example.ToString().TrimEnd();
            }
        }
        #endregion

        #region Auxiliary - May Remove
        /// <summary>
        /// Get from name; Conventional --Key.
        /// </summary>
        public static string[] Get(string name, params string[] arguments)
        {
            if (!name.StartsWith("--"))
                throw new ArgumentException($"Invalid name `{name}`, expect key.");
            if (!arguments.Contains(name))
                throw new ArgumentException($"Invalid name `{name}`, do not exist.");

            int index = Array.IndexOf(arguments, name);
            if (index == arguments.Length - 1)
                throw new ArgumentException($"Not enough arugments after {name}.");

            List<string> values = new();
            for (int i = index + 1; i < arguments.Length; i++)
            {
                if (arguments[i].StartsWith("--"))
                    break;
                values.Add(arguments[i]);
            }
            return values.ToArray();
        }
        /// <summary>
        /// Get from name; Conventional --Key.
        /// </summary>
        public static string GetSingle(string name, params string[] arguments)
        {
            if (!name.StartsWith("--"))
                throw new ArgumentException($"Invalid name `{name}`, expect key.");
            if (!arguments.Contains(name))
                throw new ArgumentException($"Invalid name `{name}`, do not exist.");

            int index = Array.IndexOf(arguments, name);
            if (index == arguments.Length - 1)
                throw new ArgumentException($"Not enough arugments after {name}.");

            return arguments[index + 1];
        }
        #endregion

        #region Helpers
        private static object ConvertValue(string name, Type type, string[] values)
        {
            if (type.IsArray)
            {
                Type elementType = type.GetElementType();
                var array = Array.CreateInstance(elementType, values.Length);
                for (int i = 0; i < values.Length; i++)
                {
                    string valueString = values[i];
                    object value = ConvertSingle(elementType, valueString);
                    array.SetValue(value, i);
                }
                return array;
            }
            else if (type != typeof(string) && !type.IsValueType)
                throw new ArgumentException($"{name} must be value type.");
            else
            {
                if (values.Length == 0)
                {
                    if (type == typeof(bool))
                        return true;
                    else
                        throw new ArgumentException($"Invalid number of values for type {type.Name}");
                }
                else if (values.Length == 1)
                {
                    return ConvertSingle(type, values.Single());
                }
                else
                    throw new ArgumentException($"Values \"{string.Join(", ", values)}\" are too many for type {type.Name}.");
            }
        }
        private static object ConvertSingle(Type type, string value)
        {
            TypeConverter typeConverter = TypeDescriptor.GetConverter(type);
            object objValue = typeConverter.ConvertFromString(value);
            return objValue;
        }
        #endregion
    }
}
