using System;
using System.Collections.Generic;
using System.Linq;

namespace UniversitySystem
{
    // Наследование: студент получает от Person ФИО, возраст и контакты
    // и добавляет только свое - группу, курс обучения и список курсов
    class Student : Person
    {
        // счетчик общий для всех студентов, поэтому номера не повторяются
        private static int nextId = 1;

        public string Group { get; private set; }
        public int Year { get; private set; }

        // список курсов закрыт, изменить его можно только через EnrollCourse
        private List<Course> courses = new List<Course>();

        public Student(string fullName, int age, string phone, string email, string group, int year)
            : base(nextId++, fullName, age, phone, email)
        {
            Group = group;
            Year = year;
        }

        public override string GetRole()
        {
            return "Студент";
        }

        public bool IsEnrolled(Course course)
        {
            return courses.Contains(course);
        }

        // запись на курс: дважды на один курс и на курс без мест записаться нельзя
        public bool EnrollCourse(Course course)
        {
            if (course == null || IsEnrolled(course) || course.IsFull())
                return false;

            courses.Add(course);
            course.AddStudent(this);
            return true;
        }

        // отдаем новый список, чтобы снаружи нельзя было изменить настоящий
        public List<Course> GetCourses()
        {
            return courses.OrderBy(c => c.Title).ToList();
        }

        public override void ShowInfo()
        {
            base.ShowInfo();
            Console.WriteLine($"  Группа:            {Group}");
            Console.WriteLine($"  Курс обучения:     {Year}");
            Console.WriteLine($"  Количество курсов: {courses.Count}");
        }

        public override string ToString()
        {
            return base.ToString() + $", группа {Group}";
        }
    }
}
