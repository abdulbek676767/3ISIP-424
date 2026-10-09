using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static University university = new University();

    static void Main()
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("========== УНИВЕРСИТЕТ ==========");
            Console.WriteLine("--- Студенты ---");
            Console.WriteLine(" 1 - Добавить студента");
            Console.WriteLine(" 2 - Информация о студенте");
            Console.WriteLine(" 3 - Записать студента на курс");
            Console.WriteLine(" 4 - Курсы студента");
            Console.WriteLine("--- Преподаватели ---");
            Console.WriteLine(" 5 - Добавить преподавателя");
            Console.WriteLine(" 6 - Информация о преподавателе");
            Console.WriteLine(" 7 - Назначить преподавателя на курс");
            Console.WriteLine("--- Курсы ---");
            Console.WriteLine(" 8 - Создать курс");
            Console.WriteLine(" 9 - Информация о курсе");
            Console.WriteLine("10 - Студенты курса");
            Console.WriteLine("--- Списки ---");
            Console.WriteLine("11 - Все студенты");
            Console.WriteLine("12 - Все преподаватели");
            Console.WriteLine("13 - Все курсы");
            Console.WriteLine("14 - Все люди университета");
            Console.WriteLine(" 0 - Выход");
            Console.Write("Выберите пункт: ");

            string choice = Console.ReadLine();
            if (choice == null)
                return;
            Console.WriteLine();

            switch (choice.Trim())
            {
                case "1": AddStudent(); break;
                case "2": ShowStudent(); break;
                case "3": EnrollStudent(); break;
                case "4": ShowStudentCourses(); break;
                case "5": AddTeacher(); break;
                case "6": ShowTeacher(); break;
                case "7": AssignTeacher(); break;
                case "8": AddCourse(); break;
                case "9": ShowCourse(); break;
                case "10": ShowCourseStudents(); break;
                case "11": ShowAllStudents(); break;
                case "12": ShowAllTeachers(); break;
                case "13": ShowAllCourses(); break;
                case "14": ShowAllPeople(); break;
                case "0": return;
                default:
                    Console.WriteLine("Нет такого пункта, попробуйте еще раз");
                    break;
            }
        }
    }

    // ===== Студенты =====

    static void AddStudent()
    {
        Console.WriteLine("--- Новый студент ---");
        string name = ReadName("ФИО: ");
        int age = ReadNumber("Возраст (16-60): ", 16, 60);
        string phone = ReadPhone("Телефон: ");
        string email = ReadEmail("Email: ");
        string group = ReadText("Группа: ", 10);
        int year = ReadNumber("Курс обучения (1-6): ", 1, 6);

        Student student = university.AddStudent(name, age, phone, email, group, year);
        Console.WriteLine($"Студент добавлен, его номер: {student.Id}");
    }

    static void ShowStudent()
    {
        Student student = ChooseStudent();
        if (student == null) return;
        Console.WriteLine();
        student.ShowInfo();
    }

    static void EnrollStudent()
    {
        Student student = ChooseStudent();
        if (student == null) return;
        Course course = ChooseCourse();
        if (course == null) return;

        if (course.IsFull())
        {
            Console.WriteLine("На курсе нет свободных мест");
            return;
        }

        if (student.EnrollCourse(course))
            Console.WriteLine($"{student.FullName} записан(а) на курс \"{course.Title}\"");
        else
            Console.WriteLine("Студент уже записан на этот курс");
    }

    static void ShowStudentCourses()
    {
        Student student = ChooseStudent();
        if (student == null) return;

        List<Course> courses = student.GetCourses();
        Console.WriteLine($"\nКурсы студента {student.FullName}:");
        if (courses.Count == 0)
        {
            Console.WriteLine("  Студент не записан ни на один курс");
            return;
        }
        foreach (Course course in courses)
            Console.WriteLine("  " + course.GetShortInfo());
        Console.WriteLine($"Всего часов: {courses.Sum(c => c.Hours)}");
    }

    // ===== Преподаватели =====

    static void AddTeacher()
    {
        Console.WriteLine("--- Новый преподаватель ---");
        string name = ReadName("ФИО: ");
        int age = ReadNumber("Возраст (22-90): ", 22, 90);
        string phone = ReadPhone("Телефон: ");
        string email = ReadEmail("Email: ");
        string department = ReadText("Кафедра: ", 50);
        Console.WriteLine("Ученая степень: 1 - без степени, 2 - кандидат наук, 3 - доктор наук");
        int degreeNumber = ReadNumber("Выберите: ", 1, 3);
        Degree degree = (Degree)(degreeNumber - 1);
        int experience = ReadNumber($"Стаж в годах (0-{age - 18}): ", 0, age - 18);

        Teacher teacher = university.AddTeacher(name, age, phone, email, department, degree, experience);
        Console.WriteLine($"Преподаватель добавлен, его номер: {teacher.Id}");
    }

    static void ShowTeacher()
    {
        Teacher teacher = ChooseTeacher();
        if (teacher == null) return;
        Console.WriteLine();
        teacher.ShowInfo();
    }

    static void AssignTeacher()
    {
        Course course = ChooseCourse();
        if (course == null) return;
        Teacher teacher = ChooseTeacher();
        if (teacher == null) return;

        if (course.Teacher == teacher)
        {
            Console.WriteLine("Этот преподаватель уже ведет этот курс");
            return;
        }
        course.SetTeacher(teacher);
        Console.WriteLine($"{teacher.FullName} теперь ведет курс \"{course.Title}\"");
    }

    // ===== Курсы =====

    static void AddCourse()
    {
        Console.WriteLine("--- Новый курс ---");
        string title;
        while (true)
        {
            title = ReadText("Название: ", 50);
            if (!university.CourseExists(title)) break;
            Console.WriteLine("Курс с таким названием уже есть");
        }
        int hours = ReadNumber("Количество часов (1-500): ", 1, 500);
        int maxStudents = ReadNumber("Максимум студентов (1-100): ", 1, 100);

        Course course = university.AddCourse(title, hours, maxStudents);
        Console.WriteLine($"Курс создан, его номер: {course.Id}");

        if (university.TeachersCount > 0)
        {
            Console.Write("Назначить преподавателя сейчас? (1 - да, другое - нет): ");
            string answer = Console.ReadLine();
            if (answer != null && answer.Trim() == "1")
            {
                Teacher teacher = ChooseTeacher();
                if (teacher != null)
                {
                    course.SetTeacher(teacher);
                    Console.WriteLine($"{teacher.FullName} назначен(а) на курс");
                }
            }
        }
    }

    static void ShowCourse()
    {
        Course course = ChooseCourse();
        if (course == null) return;
        Console.WriteLine();
        course.ShowInfo();
    }

    static void ShowCourseStudents()
    {
        Course course = ChooseCourse();
        if (course == null) return;

        List<Student> students = course.GetStudents();
        Console.WriteLine($"\nСтуденты курса \"{course.Title}\":");
        if (students.Count == 0)
        {
            Console.WriteLine("  На курс еще никто не записан");
            return;
        }
        foreach (Student student in students)
            Console.WriteLine("  " + student.GetShortInfo());
    }

    // ===== Списки =====

    static void ShowAllStudents()
    {
        List<Student> students = university.GetStudents();
        Console.WriteLine($"Все студенты ({students.Count}):");
        if (students.Count == 0)
            Console.WriteLine("  Список пуст");
        foreach (Student student in students)
            Console.WriteLine("  " + student.GetShortInfo());
    }

    static void ShowAllTeachers()
    {
        List<Teacher> teachers = university.GetTeachers();
        Console.WriteLine($"Все преподаватели ({teachers.Count}):");
        if (teachers.Count == 0)
            Console.WriteLine("  Список пуст");
        foreach (Teacher teacher in teachers)
            Console.WriteLine("  " + teacher.GetShortInfo());
    }

    static void ShowAllCourses()
    {
        List<Course> courses = university.GetCourses();
        Console.WriteLine($"Все курсы ({courses.Count}):");
        if (courses.Count == 0)
            Console.WriteLine("  Список пуст");
        foreach (Course course in courses)
            Console.WriteLine("  " + course.GetShortInfo());
    }

    // полиморфизм: у всех тип Person, но ShowInfo вызывается свой для студента и преподавателя
    static void ShowAllPeople()
    {
        List<Person> people = university.GetAllPeople();
        if (people.Count == 0)
        {
            Console.WriteLine("В университете пока никого нет");
            return;
        }
        foreach (Person person in people)
        {
            person.ShowInfo();
            Console.WriteLine();
        }
    }

    // ===== Выбор из списка =====

    static Student ChooseStudent()
    {
        if (university.StudentsCount == 0)
        {
            Console.WriteLine("Сначала добавьте хотя бы одного студента");
            return null;
        }
        ShowAllStudents();
        int id = ReadNumber("Введите номер студента: ", 1, university.StudentsCount);
        return university.FindStudent(id);
    }

    static Teacher ChooseTeacher()
    {
        if (university.TeachersCount == 0)
        {
            Console.WriteLine("Сначала добавьте хотя бы одного преподавателя");
            return null;
        }
        ShowAllTeachers();
        int id = ReadNumber("Введите номер преподавателя: ", 1, university.TeachersCount);
        return university.FindTeacher(id);
    }

    static Course ChooseCourse()
    {
        if (university.CoursesCount == 0)
        {
            Console.WriteLine("Сначала создайте хотя бы один курс");
            return null;
        }
        ShowAllCourses();
        int id = ReadNumber("Введите номер курса: ", 1, university.CoursesCount);
        return university.FindCourse(id);
    }

    // ===== Проверка ввода =====

    // любой непустой текст ограниченной длины
    static string ReadText(string message, int maxLength)
    {
        while (true)
        {
            Console.Write(message);
            string input = Console.ReadLine();
            if (input == null)
                Environment.Exit(0);
            input = input.Trim();

            if (input == "")
                Console.WriteLine("Поле не может быть пустым");
            else if (input.Length > maxLength)
                Console.WriteLine($"Слишком длинно, максимум {maxLength} символов");
            else
                return input;
        }
    }

    // ФИО: только буквы, пробелы и дефис, минимум 2 буквы
    static string ReadName(string message)
    {
        while (true)
        {
            string name = ReadText(message, 60);
            bool correct = name.All(c => char.IsLetter(c) || c == ' ' || c == '-');
            if (correct && name.Count(char.IsLetter) >= 2)
                return name;
            Console.WriteLine("ФИО может содержать только буквы, пробел и дефис");
        }
    }

    // целое число в диапазоне от min до max
    static int ReadNumber(string message, int min, int max)
    {
        while (true)
        {
            string input = ReadText(message, 10);
            int number;
            if (!int.TryParse(input, out number))
                Console.WriteLine("Нужно ввести целое число");
            else if (number < min || number > max)
                Console.WriteLine($"Число должно быть от {min} до {max}");
            else
                return number;
        }
    }

    // телефон: цифры, можно + в начале, от 10 до 12 цифр
    static string ReadPhone(string message)
    {
        while (true)
        {
            string phone = ReadText(message, 15);
            string digits = phone.StartsWith("+") ? phone.Substring(1) : phone;
            if (digits.All(char.IsDigit) && digits.Length >= 10 && digits.Length <= 12)
                return phone;
            Console.WriteLine("Телефон должен состоять из 10-12 цифр, например 89991234567");
        }
    }

    // email: одна @, перед ней и после нее есть текст, после @ есть точка
    static string ReadEmail(string message)
    {
        while (true)
        {
            string email = ReadText(message, 50);
            int at = email.IndexOf('@');
            bool correct = at > 0
                && email.Count(c => c == '@') == 1
                && !email.Contains(' ')
                && email.LastIndexOf('.') > at + 1
                && !email.EndsWith(".");
            if (correct)
                return email;
            Console.WriteLine("Неверный email, пример: ivanov@mail.ru");
        }
    }
}
