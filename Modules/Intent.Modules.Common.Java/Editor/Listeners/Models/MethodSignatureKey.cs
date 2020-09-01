using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JavaParserLib.Listeners.Models
{
    public class MethodSignatureKey
    {
        private readonly string _returnTypeStr;
        private readonly string _name;
        private readonly string[] _paramList;

        public MethodSignatureKey(string returnTypeStr, string name, string[] paramList)
        {
            _returnTypeStr = returnTypeStr;
            _name = name;
            _paramList = paramList;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            
            var other = obj as MethodSignatureKey;
            if (other == null)
            {
                return false;
            }

            return _returnTypeStr.Equals(other._returnTypeStr)
                && _name.Equals(other._name)
                && _paramList.SequenceEqual(other._paramList);
        }

        public override int GetHashCode()
        {
            return _returnTypeStr.GetHashCode() + _name.GetHashCode() + _paramList.Sum(s => s.GetHashCode());
        }
    }
}
