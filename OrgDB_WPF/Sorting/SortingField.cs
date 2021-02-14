using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrgDB_WPF
{
    public abstract class SortingField
    {
        public enum SortDirectionVariant { Asc, Desc }
        public enum Field { }

        protected SortDirectionVariant sortDirection;
        protected Field sortField;

        public SortDirectionVariant SortDirection { get { return sortDirection; }  set { sortDirection = value; } }
        public Field SortField { get { return sortField; }  set { sortField = value; } }
        
        public SortingField(Field sortField, SortDirectionVariant sortDirection)
        {
            SortField = sortField;
            SortDirection = sortDirection;
        }
        
    }
}
