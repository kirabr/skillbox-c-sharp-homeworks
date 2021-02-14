using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrgDB_WPF.Sorting
{
    class DepartmentSortingField : SortingField
    {
        public enum SortFieldVariant
        {
            Name,
            Location
        }

        public DepartmentSortingField(SortingField.Field sortField, SortDirectionVariant sortDirection = SortDirectionVariant.Asc)
            : base(sortField, sortDirection) { }
    }
}
