using System;
using System.Collections.Generic;
using System.Linq;

namespace UniversitySystem
{
    // Абстракция: общие данные и поведение всех людей в университете.
    // Класс абстрактный - создать просто "человека" нельзя,
    // можно создать только студента или преподавателя.
    abstract class Person
    {
        // Инкапсуляция: прочитать данные можно откуда угодно,
        // а изменить - только внутри класса (private set)
        public int Id { get; private set; }
        public string FullName { get; private set; }
        public int Age { get; private set; }
        public string Phone { get; private set; }
        public string Email { get; private set; }

        // protected - этот конструктор могут вызвать только наследники
        protected Person(int id, string fullName, int age, string phone, string email)
        {
            Id = id;
            FullName = fullName;
            Age = age;
            Phone = phone;
            Email = email;
        }

        // каждый наследник обязан сказать, кто он
        public abstract string GetRole();

        // Полиморфизм: общая часть информации,
        // наследники переопределяют метод и добавляют свои данные
        public virtual void ShowInfo()
        {
            Console.WriteLine($"{GetRole()} №{Id}");
            Console.WriteLine($"  ФИО:               {FullName}");
            Console.WriteLine($"  Возраст:           {Age}");
            Console.WriteLine($"  Телефон:           {Phone}");
            Console.WriteLine($"  Email:             {Email}");
        }

        // краткая информация в одну строку - для списков
        public override string ToString()
        {
            return $"№{Id} {FullName}, возраст {Age}";
        }
    }

    // результат записи студента на курс
    enum EnrollResult
    {
        Success,          // записан
        AlreadyEnrolled,  // уже был записан на этот курс
        CourseFull        // на курсе нет свободных мест
    }

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

        // запись на курс: дважды на один курс и на курс без мест записаться нельзя
        public EnrollResult EnrollCourse(Course course)
        {
            if (courses.Contains(course))
                return EnrollResult.AlreadyEnrolled;
            if (course.IsFull())
                return EnrollResult.CourseFull;

            courses.Add(course);
            course.AddStudent(this);
            return EnrollResult.Success;
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

    // ученая степень преподавателя
    enum AcademicDegree
    {
        None,       // без степени
        Candidate,  // кандидат наук
        Doctor      // доктор наук
    }

    // Наследование: преподаватель тоже Person, но со своими данными
    class Teacher : Person
    {
        private static int nextId = 1;

        public string Department { get; private set; }
        public AcademicDegree Degree { get; private set; }
        public int Experience { get; private set; }

        // курсы, которые ведет преподаватель
        private List<Course> courses = new List<Course>();

        public Teacher(string fullName, int age, string phone, string email, string department, AcademicDegree degree, int experience)
            : base(nextId++, fullName, age, phone, email)
        {
            Department = department;
            Degree = degree;
            Experience = experience;
        }

        public override string GetRole()
        {
            return "Преподаватель";
        }

        // internal: эти методы вызывает только Course, когда назначает преподавателя
        internal void AddCourse(Course course)
        {
            if (!courses.Contains(course))
                courses.Add(course);
        }

        internal void RemoveCourse(Course course)
        {
            courses.Remove(course);
        }

        public List<Course> GetCourses()
        {
            return courses.OrderBy(c => c.Title).ToList();
        }

        public string GetDegreeText()
        {
            switch (Degree)
            {
                case AcademicDegree.Candidate:
                    return "кандидат наук";
                case AcademicDegree.Doctor:
                    return "доктор наук";
                default:
                    return "без степени";
            }
        }

        public override void ShowInfo()
        {
            base.ShowInfo();
            Console.WriteLine($"  Кафедра:           {Department}");
            Console.WriteLine($"  Ученая степень:    {GetDegreeText()}");
            Console.WriteLine($"  Стаж (лет):        {Experience}");

            if (courses.Count == 0)
            {
                Console.WriteLine("  Ведет курсы:       пока нет");
                return;
            }
            Console.WriteLine("  Ведет курсы:");
            foreach (Course course in GetCourses())
                Console.WriteLine($"    - {course.Title}");
        }

        public override string ToString()
        {
            return base.ToString() + $", кафедра \"{Department}\"";
        }
    }

    class Course
    {
        private static int nextId = 1;

        public int Id { get; private set; }
        public string Title { get; private set; }
        public int Hours { get; private set; }
        public int MaxStudents { get; private set; }
        public Teacher Teacher { get; private set; }

        // студенты курса закрыты, записать можно только через Student.EnrollCourse
        private List<Student> students = new List<Student>();

        // у каждого курса есть преподаватель, поэтому без него курс не создается
        public Course(string title, int hours, int maxStudents, Teacher teacher)
        {
            Id = nextId++;
            Title = title;
            Hours = hours;
            MaxStudents = maxStudents;
            SetTeacher(teacher);
        }

        public bool IsFull()
        {
            return students.Count >= MaxStudents;
        }

        // internal: вызывается только из Student.EnrollCourse,
        // чтобы списки у студента и у курса всегда совпадали
        internal void AddStudent(Student student)
        {
            if (!students.Contains(student))
                students.Add(student);
        }

        // назначить или сменить преподавателя курса
        public void SetTeacher(Teacher teacher)
        {
            if (teacher == null || teacher == Teacher)
                return;

            if (Teacher != null)
                Teacher.RemoveCourse(this);

            Teacher = teacher;
            teacher.AddCourse(this);
        }

        public List<Student> GetStudents()
        {
            return students.OrderBy(s => s.FullName).ToList();
        }

        private string GetTeacherName()
        {
            if (Teacher == null)
                return "не назначен";
            return Teacher.FullName;
        }

        public void ShowInfo()
        {
            Console.WriteLine($"Курс №{Id}: {Title}");
            Console.WriteLine($"  Часов:               {Hours}");
            Console.WriteLine($"  Преподаватель:       {GetTeacherName()}");
            if (Teacher != null)
            {
                Console.WriteLine($"  Кафедра:             {Teacher.Department}");
                Console.WriteLine($"  Ученая степень:      {Teacher.GetDegreeText()}");
            }
            Console.WriteLine($"  Записано студентов:  {students.Count} из {MaxStudents}");
            Console.WriteLine($"  Свободных мест:      {MaxStudents - students.Count}");
        }

        public override string ToString()
        {
            return $"№{Id} {Title}, {Hours} ч., преподаватель: {GetTeacherName()}, мест занято {students.Count}/{MaxStudents}";
        }
    }

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

    class Program
    {
        static University university = new University();

        static void Main()
        {
            bool work = true;
            while (work)
            {
                ShowMenu();
                string choice = ReadText("Выберите пункт меню: ", 10);
                Console.WriteLine();

                switch (choice)
                {
                    case "1": AddStudent(); break;
                    case "2": ShowPersonInfo(ChooseStudent()); break;
                    case "3": EnrollStudent(); break;
                    case "4": ShowStudentCourses(); break;
                    case "5": AddTeacher(); break;
                    case "6": ShowPersonInfo(ChooseTeacher()); break;
                    case "7": AssignTeacher(); break;
                    case "8": AddCourse(); break;
                    case "9": ShowCourseInfo(); break;
                    case "10": ShowCourseStudents(); break;
                    case "11": ShowAllStudents(); break;
                    case "12": ShowAllTeachers(); break;
                    case "13": ShowAllCourses(); break;
                    case "0":
                        work = false;
                        Console.WriteLine("Работа программы завершена");
                        break;
                    default:
                        Console.WriteLine("Ошибка: такого пункта нет в меню");
                        break;
                }
            }
        }

        static void ShowMenu()
        {
            Console.WriteLine();
            Console.WriteLine("============ УНИВЕРСИТЕТ ============");
            Console.WriteLine("Студенты:");
            Console.WriteLine("   1 - Добавить студента");
            Console.WriteLine("   2 - Информация о студенте");
            Console.WriteLine("   3 - Записать студента на курс");
            Console.WriteLine("   4 - Курсы студента");
            Console.WriteLine("Преподаватели:");
            Console.WriteLine("   5 - Добавить преподавателя");
            Console.WriteLine("   6 - Информация о преподавателе");
            Console.WriteLine("   7 - Назначить преподавателя на курс");
            Console.WriteLine("Курсы:");
            Console.WriteLine("   8 - Создать курс");
            Console.WriteLine("   9 - Информация о курсе");
            Console.WriteLine("  10 - Студенты курса");
            Console.WriteLine("Списки:");
            Console.WriteLine("  11 - Все студенты");
            Console.WriteLine("  12 - Все преподаватели");
            Console.WriteLine("  13 - Все курсы");
            Console.WriteLine("   0 - Выход");
            Console.WriteLine("=====================================");
        }

        // ===== Студенты =====

        static void AddStudent()
        {
            Console.WriteLine("--- Добавление студента ---");
            string name = ReadName("ФИО: ");
            int age = ReadNumber("Возраст (16-60): ", 16, 60);
            string phone = ReadPhone("Телефон: ");
            string email = ReadEmail("Email: ");
            string group = ReadGroup("Группа: ");
            int year = ReadNumber("Курс обучения (1-6): ", 1, 6);

            Student student = new Student(name, age, phone, email, group, year);
            university.AddPerson(student);
            Console.WriteLine($"Студент добавлен под номером {student.Id}");
        }

        // Полиморфизм: метод принимает базовый тип Person,
        // а ShowInfo() вызывается свой - студента или преподавателя
        static void ShowPersonInfo(Person person)
        {
            if (person == null)
                return;
            Console.WriteLine();
            person.ShowInfo();
        }

        static void EnrollStudent()
        {
            Console.WriteLine("--- Запись студента на курс ---");
            Student student = ChooseStudent();
            if (student == null)
                return;
            Course course = ChooseCourse();
            if (course == null)
                return;

            // результат записи - значение перечисления, поэтому разбираем его через switch
            switch (student.EnrollCourse(course))
            {
                case EnrollResult.Success:
                    Console.WriteLine($"{student.FullName} записан(а) на курс \"{course.Title}\"");
                    break;
                case EnrollResult.AlreadyEnrolled:
                    Console.WriteLine($"{student.FullName} уже записан(а) на курс \"{course.Title}\"");
                    break;
                case EnrollResult.CourseFull:
                    Console.WriteLine($"На курсе \"{course.Title}\" нет свободных мест");
                    break;
            }
        }

        static void ShowStudentCourses()
        {
            Student student = ChooseStudent();
            if (student == null)
                return;

            List<Course> courses = student.GetCourses();
            Console.WriteLine($"\nКурсы студента {student.FullName}:");
            if (courses.Count == 0)
            {
                Console.WriteLine("  Студент пока не записан ни на один курс");
                return;
            }
            foreach (Course course in courses)
                Console.WriteLine("  " + course);
            Console.WriteLine($"Всего курсов: {courses.Count}, часов: {courses.Sum(c => c.Hours)}");
        }

        // ===== Преподаватели =====

        static void AddTeacher()
        {
            Console.WriteLine("--- Добавление преподавателя ---");
            string name = ReadName("ФИО: ");
            int age = ReadNumber("Возраст (22-90): ", 22, 90);
            string phone = ReadPhone("Телефон: ");
            string email = ReadEmail("Email: ");
            string department = ReadTitle("Кафедра: ", 50);
            AcademicDegree degree = ReadDegree();
            int experience = ReadNumber($"Стаж работы в годах (0-{age - 18}): ", 0, age - 18);

            Teacher teacher = new Teacher(name, age, phone, email, department, degree, experience);
            university.AddPerson(teacher);
            Console.WriteLine($"Преподаватель добавлен под номером {teacher.Id}");
        }

        static AcademicDegree ReadDegree()
        {
            Console.WriteLine("Ученая степень:");
            Console.WriteLine("  1 - без степени");
            Console.WriteLine("  2 - кандидат наук");
            Console.WriteLine("  3 - доктор наук");
            switch (ReadNumber("Выберите (1-3): ", 1, 3))
            {
                case 2:
                    return AcademicDegree.Candidate;
                case 3:
                    return AcademicDegree.Doctor;
                default:
                    return AcademicDegree.None;
            }
        }

        static void AssignTeacher()
        {
            Console.WriteLine("--- Назначение преподавателя на курс ---");
            Course course = ChooseCourse();
            if (course == null)
                return;
            Teacher teacher = ChooseTeacher();
            if (teacher == null)
                return;

            if (course.Teacher == teacher)
            {
                Console.WriteLine($"{teacher.FullName} уже ведет курс \"{course.Title}\"");
                return;
            }
            course.SetTeacher(teacher);
            Console.WriteLine($"Теперь курс \"{course.Title}\" ведет {teacher.FullName}");
        }

        // ===== Курсы =====

        static void AddCourse()
        {
            Console.WriteLine("--- Создание курса ---");
            if (university.GetTeachers().Count == 0)
            {
                Console.WriteLine("У каждого курса должен быть преподаватель.");
                Console.WriteLine("Сначала добавьте преподавателя (пункт 5)");
                return;
            }

            string title = ReadCourseTitle();
            int hours = ReadNumber("Количество часов (1-500): ", 1, 500);
            int maxStudents = ReadNumber("Количество мест (1-100): ", 1, 100);
            Console.WriteLine("Кто будет вести курс?");
            Teacher teacher = ChooseTeacher();
            if (teacher == null)
            {
                Console.WriteLine("Курс не создан: без преподавателя курс создать нельзя");
                return;
            }

            Course course = new Course(title, hours, maxStudents, teacher);
            university.AddCourse(course);
            Console.WriteLine($"Курс \"{course.Title}\" создан под номером {course.Id}");
        }

        static void ShowCourseInfo()
        {
            Course course = ChooseCourse();
            if (course == null)
                return;
            Console.WriteLine();
            course.ShowInfo();
        }

        static void ShowCourseStudents()
        {
            Course course = ChooseCourse();
            if (course == null)
                return;

            List<Student> students = course.GetStudents();
            Console.WriteLine($"\nСтуденты курса \"{course.Title}\":");
            if (students.Count == 0)
            {
                Console.WriteLine("  На этот курс пока никто не записан");
                return;
            }
            foreach (Student student in students)
                Console.WriteLine("  " + student);
            Console.WriteLine($"Всего студентов: {students.Count}");
        }

        // ===== Полные списки =====

        static void ShowAllStudents()
        {
            List<Student> students = university.GetStudents();
            Console.WriteLine($"Все студенты ({students.Count}):");
            if (students.Count == 0)
                Console.WriteLine("  Список пуст");
            foreach (Student student in students)
                Console.WriteLine("  " + student);
        }

        static void ShowAllTeachers()
        {
            List<Teacher> teachers = university.GetTeachers();
            Console.WriteLine($"Все преподаватели ({teachers.Count}):");
            if (teachers.Count == 0)
                Console.WriteLine("  Список пуст");
            foreach (Teacher teacher in teachers)
                Console.WriteLine("  " + teacher);
        }

        static void ShowAllCourses()
        {
            List<Course> courses = university.GetCourses();
            Console.WriteLine($"Все курсы ({courses.Count}):");
            if (courses.Count == 0)
                Console.WriteLine("  Список пуст");
            foreach (Course course in courses)
                Console.WriteLine("  " + course);
        }

        // ===== Выбор по номеру (0 - отмена) =====

        static Student ChooseStudent()
        {
            List<Student> students = university.GetStudents();
            if (students.Count == 0)
            {
                Console.WriteLine("Студентов пока нет. Сначала добавьте студента (пункт 1)");
                return null;
            }

            ShowAllStudents();
            int id = ReadId("Введите номер студента", students.Max(s => s.Id));
            return university.FindStudent(id); // при отмене id = 0, такого номера нет - вернется null
        }

        static Teacher ChooseTeacher()
        {
            List<Teacher> teachers = university.GetTeachers();
            if (teachers.Count == 0)
            {
                Console.WriteLine("Преподавателей пока нет. Сначала добавьте преподавателя (пункт 5)");
                return null;
            }

            ShowAllTeachers();
            int id = ReadId("Введите номер преподавателя", teachers.Max(t => t.Id));
            return university.FindTeacher(id);
        }

        static Course ChooseCourse()
        {
            List<Course> courses = university.GetCourses();
            if (courses.Count == 0)
            {
                Console.WriteLine("Курсов пока нет. Сначала создайте курс (пункт 8)");
                return null;
            }

            ShowAllCourses();
            int id = ReadId("Введите номер курса", courses.Max(c => c.Id));
            return university.FindCourse(id);
        }

        // номер из списка от 1 до maxId, 0 - отмена
        static int ReadId(string message, int maxId)
        {
            int id = ReadNumber($"{message} (0 - отмена): ", 0, maxId);
            if (id == 0)
                Console.WriteLine("Действие отменено");
            return id;
        }

        // ===== Проверка ввода =====

        // непустая строка не длиннее maxLength
        static string ReadText(string message, int maxLength)
        {
            while (true)
            {
                Console.Write(message);
                string input = Console.ReadLine();
                if (input == null)
                    Environment.Exit(0); // ввод закончился (Ctrl+Z) - просто закрываем программу
                input = input.Trim();

                if (input == "")
                    Console.WriteLine("Ошибка: значение не может быть пустым");
                else if (input.Length > maxLength)
                    Console.WriteLine($"Ошибка: слишком длинно, максимум {maxLength} символов");
                else
                    return input;
            }
        }

        // целое число от min до max
        static int ReadNumber(string message, int min, int max)
        {
            while (true)
            {
                string input = ReadText(message, 11);
                int number;
                if (!int.TryParse(input, out number))
                    Console.WriteLine("Ошибка: нужно ввести целое число");
                else if (number < min || number > max)
                    Console.WriteLine($"Ошибка: число должно быть от {min} до {max}");
                else
                    return number;
            }
        }

        // текст, в котором обязательно есть буквы (кафедра, название курса)
        static string ReadTitle(string message, int maxLength)
        {
            while (true)
            {
                string input = ReadText(message, maxLength);
                if (input.Any(char.IsLetter))
                    return input;
                Console.WriteLine("Ошибка: значение должно содержать буквы");
            }
        }

        // название курса не должно повторяться
        static string ReadCourseTitle()
        {
            while (true)
            {
                string title = ReadTitle("Название курса: ", 50);
                if (!university.CourseExists(title))
                    return title;
                Console.WriteLine("Ошибка: курс с таким названием уже есть");
            }
        }

        // ФИО: от 2 до 4 слов, только буквы (внутри слова можно дефис)
        static string ReadName(string message)
        {
            while (true)
            {
                string input = ReadText(message, 80);
                string[] words = input.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                bool correct = words.Length >= 2 && words.Length <= 4 && words.All(w => IsNameWord(w));
                if (correct)
                    return string.Join(" ", words);
                Console.WriteLine("Ошибка: введите фамилию и имя (можно и отчество) буквами через пробел");
            }
        }

        static bool IsNameWord(string word)
        {
            return char.IsLetter(word[0])
                && char.IsLetter(word[word.Length - 1])
                && word.All(c => char.IsLetter(c) || c == '-');
        }

        // телефон: в начале можно +, дальше от 10 до 12 цифр
        // пробелы, скобки и дефисы убираем: +7 (999) 123-45-67 -> +79991234567
        static string ReadPhone(string message)
        {
            while (true)
            {
                string input = ReadText(message, 20);
                string phone = input.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");
                string digits = phone.StartsWith("+") ? phone.Substring(1) : phone;
                if (digits.Length >= 10 && digits.Length <= 12 && digits.All(c => c >= '0' && c <= '9'))
                    return phone;
                Console.WriteLine("Ошибка: телефон должен содержать 10-12 цифр, например 89991234567");
            }
        }

        // email: одна @, имя до нее, домен с точкой после, без пробелов и не занят
        static string ReadEmail(string message)
        {
            while (true)
            {
                string email = ReadText(message, 50).ToLower();
                int at = email.IndexOf('@');
                int dot = email.LastIndexOf('.');
                bool correct = at > 0
                    && at == email.LastIndexOf('@')
                    && dot > at + 1
                    && dot < email.Length - 1
                    && !email.Contains(" ");

                if (!correct)
                    Console.WriteLine("Ошибка: неверный email, пример: ivanov@mail.ru");
                else if (university.EmailExists(email))
                    Console.WriteLine("Ошибка: такой email уже есть в системе");
                else
                    return email;
            }
        }

        // группа: буквы, цифры и дефис, например 3ИСИП-424
        static string ReadGroup(string message)
        {
            while (true)
            {
                string group = ReadText(message, 15).ToUpper();
                bool correct = group.All(c => char.IsLetterOrDigit(c) || c == '-')
                    && group.Any(char.IsLetter)
                    && group.Any(char.IsDigit);
                if (correct)
                    return group;
                Console.WriteLine("Ошибка: группа пишется буквами и цифрами, например 3ИСИП-424");
            }
        }
    }
}
