using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace OsmiumMine.Core.Services.Security
{
    public class SecurityRuleCollection : IList<SecurityRule>
    {
        // TODO: Use concurrent list?
        private List<SecurityRule> _ruleCollection = new List<SecurityRule>();

        public SecurityRule this[int index]
        {
            get
            {
                return _ruleCollection[index];
            }

            set
            {
                _ruleCollection[index] = value;
            }
        }

        public int Count => _ruleCollection.Count;

        public bool IsReadOnly => false;

        public void Add(SecurityRule item)
        {
            _ruleCollection.Add(item);
            _ruleCollection = _ruleCollection.OrderBy(x => x.Priority).ToList();
        }

        public void Clear()
        {
            _ruleCollection.Clear();
        }

        public bool Contains(SecurityRule item)
        {
            return _ruleCollection.Contains(item);
        }

        public void CopyTo(SecurityRule[] array, int arrayIndex)
        {
            _ruleCollection.CopyTo(array, arrayIndex);
        }

        public IEnumerator<SecurityRule> GetEnumerator()
        {
            return _ruleCollection.GetEnumerator();
        }

        public int IndexOf(SecurityRule item)
        {
            return _ruleCollection.IndexOf(item)
;
        }

        public void Insert(int index, SecurityRule item)
        {
            _ruleCollection.Insert(index, item);
        }

        public bool Remove(SecurityRule item)
        {
            return _ruleCollection.Remove(item);
        }

        public void RemoveAt(int index)
        {
            _ruleCollection.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _ruleCollection.GetEnumerator();
        }
    }
}