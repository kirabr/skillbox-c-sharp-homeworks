using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrgDB_WPF
{
    public class EmployeeSortField : SortingField
    {
        public enum SortFieldVariant 
        {
            Name,
            Surname,
            Age,
            Salary,
            Post_enum,
            DepartmentName
        }
        
        public EmployeeSortField(SortingField.Field sortField, SortDirectionVariant sortDirection = SortDirectionVariant.Asc) 
            : base(sortField, sortDirection){}
 
    }
}
