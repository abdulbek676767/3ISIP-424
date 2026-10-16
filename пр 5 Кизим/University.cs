using System.Collections.Generic;
using System.Linq;

namespace UniversitySystem
{
    // хранит всех участников учебного процесса и все курсы
    class University
    {
        // Полиморфизм: студенты и преподаватели лежат в одном списке базового типа Person
        private List<Person> people = new List<Person>();
        private List<Course> courses = new List<Course>();

        // принимает любого наследника Person - и студента, и преподавателя
        public void AddPerson(Person person)
        {
            if (person != null && !people.Contains(person))
                people.Add(person);
        }

        public void AddCourse(Course course)
        {
            if (course != null && !courses.Contains(course))
                courses.Add(course);
        }

        // из общего списка выбираем только студентов или только преподавателей
        public List<Student> GetStudents()
        {
            return people.OfType<Student>().ToList();
        }

        public List<Teacher> GetTeachers()
        {
            return people.OfType<Teacher>().ToList();
        }

        public List<Course> GetCourses()
        {
            return courses.ToList();
        }

        // поиск по номеру, если не нашли - вернется null
        public Student FindStudent(int id)
        {
            return people.OfType<Student>().FirstOrDefault(s => s.Id == id);
        }

        public Teacher FindTeacher(int id)
        {
            return people.OfType<Teacher>().FirstOrDefault(t => t.Id == id);
        }

        public Course FindCourse(int id)
        {
            return courses.FirstOrDefault(c => c.Id == id);
        }

        // у двух людей не может быть одинакового email
        public bool EmailExists(string email)
        {
            return people.Any(p => p.Email.ToLower() == email.ToLower());
        }

        // два курса с одинаковым названием создать нельзя
        public bool CourseExists(string title)
        {
            return courses.Any(c => c.Title.ToLower() == title.ToLower());
        }
    }
}
