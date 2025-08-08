using System;
using System.Collections.Generic;
using System.IO;

namespace SchoolGradingSystem
{
    // Student class
    public class Student
    {
        public int Id { get; }
        public string FullName { get; }
        public int Score { get; }

        public Student(int id, string fullName, int score)
        {
            Id = id;
            FullName = fullName;
            Score = score;
        }

        public string GetGrade()
        {
            if (Score >= 80 && Score <= 100) return "A";
            if (Score >= 70 && Score <= 79) return "B";
            if (Score >= 60 && Score <= 69) return "C";
            if (Score >= 50 && Score <= 59) return "D";
            return "F";
        }

        public override string ToString() => $"{FullName} (ID: {Id}): Score = {Score}, Grade = {GetGrade()}";
    }

    // Custom exceptions
    public class InvalidScoreFormatException : Exception
    {
        public InvalidScoreFormatException(string message) : base(message) { }
    }

    public class MissingFieldException : Exception
    {
        public MissingFieldException(string message) : base(message) { }
    }

    // Processor
    public class StudentResultProcessor
    {
        // Reads students from a file and returns a list of Student objects
        public List<Student> ReadStudentsFromFile(string inputFilePath)
        {
            var students = new List<Student>();

            using (var reader = new StreamReader(inputFilePath))
            {
                string? line;
                int lineNumber = 0;
                while ((line = reader.ReadLine()) != null)
                {
                    lineNumber++;
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var parts = line.Split(',');

                    if (parts.Length < 3)
                        throw new MissingFieldException($"Line {lineNumber}: Missing field(s). Expected 3 values separated by commas.");

                    // Trim whitespace
                    string idPart = parts[0].Trim();
                    string namePart = parts[1].Trim();
                    string scorePart = parts[2].Trim();

                    if (string.IsNullOrEmpty(idPart) || string.IsNullOrEmpty(namePart) || string.IsNullOrEmpty(scorePart))
                        throw new MissingFieldException($"Line {lineNumber}: One or more fields are empty.");

                    if (!int.TryParse(idPart, out int id))
                        throw new InvalidScoreFormatException($"Line {lineNumber}: Invalid ID format ('{idPart}'). Expected integer.");

                    if (!int.TryParse(scorePart, out int score))
                        throw new InvalidScoreFormatException($"Line {lineNumber}: Invalid score format ('{scorePart}'). Expected integer.");

                    // Optional: validate score range 0-100
                    if (score < 0 || score > 100)
                        throw new InvalidScoreFormatException($"Line {lineNumber}: Score out of range (0-100): {score}.");

                    students.Add(new Student(id, namePart, score));
                }
            }

            return students;
        }

        // Writes report to output file
        public void WriteReportToFile(List<Student> students, string outputFilePath)
        {
            using (var writer = new StreamWriter(outputFilePath, false))
            {
                foreach (var s in students)
                {
                    writer.WriteLine($"{s.FullName} (ID: {s.Id}): Score = {s.Score}, Grade = {s.GetGrade()}");
                }
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Modify these paths as needed (relative or absolute)
            string inputFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "students_input.txt");
            string outputFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "students_report.txt");

            var processor = new StudentResultProcessor();

            try
            {
                Console.WriteLine($"Reading students from: {inputFile}");
                var students = processor.ReadStudentsFromFile(inputFile);

                Console.WriteLine("Writing report to: " + outputFile);
                processor.WriteReportToFile(students, outputFile);

                Console.WriteLine("Report written successfully. Sample output:");
                foreach (var s in students)
                {
                    Console.WriteLine($"{s.FullName} (ID: {s.Id}): Score = {s.Score}, Grade = {s.GetGrade()}");
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"Error: Input file not found: {inputFile}");
            }
            catch (InvalidScoreFormatException ex)
            {
                Console.WriteLine("Invalid score format: " + ex.Message);
            }
            catch (MissingFieldException ex)
            {
                Console.WriteLine("Missing field: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An unexpected error occurred: " + ex.Message);
            }

            Console.WriteLine("\nPress any key to exit.");
            Console.ReadKey();
        }
    }
}
