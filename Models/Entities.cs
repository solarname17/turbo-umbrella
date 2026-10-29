using System.ComponentModel.DataAnnotations;

namespace Kadry.Models
{
    // Общий интерфейс для справочников: подразделения, должности,
    // профессии, образование. Позволяет использовать один общий
    // контроллер и одно общее представление для всех четырёх таблиц.
    public interface ILookupEntity
    {
        int Id { get; set; }
        string Name { get; set; }
    }

    public class Department : ILookupEntity
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Введите название подразделения")]
        public string Name { get; set; } = string.Empty;

        public List<Employee> Employees { get; set; } = new();
    }

    public class Position : ILookupEntity
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Введите название должности")]
        public string Name { get; set; } = string.Empty;

        public List<Employee> Employees { get; set; } = new();
    }

    public class Profession : ILookupEntity
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Введите название профессии")]
        public string Name { get; set; } = string.Empty;

        public List<Employee> Employees { get; set; } = new();
    }

    public class Education : ILookupEntity
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Введите уровень образования")]
        public string Name { get; set; } = string.Empty;

        public List<Employee> Employees { get; set; } = new();
    }

    public class Employee
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Введите ФИО")]
        [Display(Name = "ФИО")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Выберите пол")]
        [Display(Name = "Пол")]
        public string Gender { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Дата рождения")]
        public DateTime BirthDate { get; set; } = DateTime.Today.AddYears(-30);

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Дата приёма")]
        public DateTime HireDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Введите оклад")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Оклад должен быть больше нуля")]
        [Display(Name = "Оклад")]
        public decimal Salary { get; set; }

        [Display(Name = "Серия паспорта")]
        public string PassportSeries { get; set; } = string.Empty;

        [Display(Name = "Номер паспорта")]
        public string PassportNumber { get; set; } = string.Empty;

        [Display(Name = "Кем выдан")]
        public string PassportIssuedBy { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        [Display(Name = "Дата выдачи")]
        public DateTime PassportIssueDate { get; set; } = DateTime.Today;

        [Display(Name = "Адрес")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Выберите подразделение")]
        [Display(Name = "Подразделение")]
        public int DepartmentId { get; set; }
        public Department? Department { get; set; }

        [Required(ErrorMessage = "Выберите должность")]
        [Display(Name = "Должность")]
        public int PositionId { get; set; }
        public Position? Position { get; set; }

        [Required(ErrorMessage = "Выберите профессию")]
        [Display(Name = "Профессия")]
        public int ProfessionId { get; set; }
        public Profession? Profession { get; set; }

        [Required(ErrorMessage = "Выберите образование")]
        [Display(Name = "Образование")]
        public int EducationId { get; set; }
        public Education? Education { get; set; }
    }
}
