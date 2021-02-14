using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using OrgDB_WPF;

public class EditingEmployee : INotifyPropertyChanged
{

    private Guid id;
    private string name;
    private string surname;
    private int age;
    private int salary;
    private Guid departmentid;
    private string departmentname;
    private string post;
    private Employee.post_enum post_Enum;

    public Guid ID { get { return id; } set { id = value; OnPropertyChanged("ID"); } }
    public string Name { get { return name; } set { name = value; OnPropertyChanged("Name"); } }
    public string Surname { get { return surname; } set { surname = value; OnPropertyChanged("Surname"); } }
    public int Age { get { return age; } set { age = value; OnPropertyChanged("Age"); } }
    public int Salary { get { return salary; } set { salary = value; OnPropertyChanged("Salary"); } }
    public Guid DepartmentID { get { return departmentid; } set { departmentid = value; OnPropertyChanged("DepartmentID"); } }
    public string DepartmentName { get { return departmentname; } set { departmentname = value; OnPropertyChanged("DepartmentName"); }  }
    public string Post { get { return post; } set { post = value; OnPropertyChanged("Post"); } }
    public Employee.post_enum Post_Enum { get { return post_Enum; } set { post_Enum = value; } }
   
    
    public void Flush()
    {
        ID = Guid.Empty;
        Name = "";
        Surname = "";
        Age = 0;
        Salary = 0;
        DepartmentID = Guid.Empty;
        DepartmentName = "";
        Post = "";
      
    }

    #region Реализация INotifyPropertyChanged

    /// <summary>
    /// Обработчик изменения свойства
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Событие изменения свойства
    /// </summary>
    /// <param name="prop"></param>
    public void OnPropertyChanged([CallerMemberName] string prop = "")
    {
        if (PropertyChanged != null)
            PropertyChanged(this, new PropertyChangedEventArgs(prop));
    }

    #endregion Реализация INotifyPropertyChanged

}
