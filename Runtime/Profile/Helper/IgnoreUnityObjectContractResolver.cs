using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace UnityEssentials
{
    public class IgnoreUnityObjectContractResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var props = base.CreateProperties(type, memberSerialization);
            return props.Where(p =>
            {
                var memberType = GetMemberUnderlyingType(type, p.UnderlyingName);
                return memberType == null || !typeof(UnityEngine.Object).IsAssignableFrom(memberType);
            }).ToList();
        }

        private static Type GetMemberUnderlyingType(Type declaringType, string memberName)
        {
            if (declaringType == null || string.IsNullOrEmpty(memberName))
                return null;
            var field = declaringType.GetField(memberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (field != null)
                return field.FieldType;
            var prop = declaringType.GetProperty(memberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (prop != null)
                return prop.PropertyType;
            return null;
        }
    }

}
