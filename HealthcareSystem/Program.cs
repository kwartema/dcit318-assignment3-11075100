using System;
using System.Collections.Generic;
using System.Linq;

namespace HealthcareSystem
{
    // a. Generic repository for entity management
    public class Repository<T>
    {
        private List<T> items = new List<T>();

        public void Add(T item) => items.Add(item);

        public List<T> GetAll() => new List<T>(items);

        public T? GetById(Func<T, bool> predicate) => items.FirstOrDefault(predicate);

        public bool Remove(Func<T, bool> predicate)
        {
            var item = items.FirstOrDefault(predicate);
            if (item != null)
            {
                items.Remove(item);
                return true;
            }
            return false;
        }
    }

    // b. Patient class
    public class Patient
    {
        public int Id;
        public string Name;
        public int Age;
        public string Gender;

        public Patient(int id, string name, int age, string gender)
        {
            Id = id;
            Name = name;
            Age = age;
            Gender = gender;
        }

        public override string ToString() =>
            $"Patient [ID={Id}, Name={Name}, Age={Age}, Gender={Gender}]";
    }

    // c. Prescription class
    public class Prescription
    {
        public int Id;
        public int PatientId;
        public string MedicationName;
        public DateTime DateIssued;

        public Prescription(int id, int patientId, string medicationName, DateTime dateIssued)
        {
            Id = id;
            PatientId = patientId;
            MedicationName = medicationName;
            DateIssued = dateIssued;
        }

        public override string ToString() =>
            $"Prescription [ID={Id}, PatientId={PatientId}, Medication={MedicationName}, Date={DateIssued.ToShortDateString()}]";
    }

    // g. HealthSystemApp
    public class HealthSystemApp
    {
        private Repository<Patient> _patientRepo = new Repository<Patient>();
        private Repository<Prescription> _prescriptionRepo = new Repository<Prescription>();
        private Dictionary<int, List<Prescription>> _prescriptionMap = new Dictionary<int, List<Prescription>>();

        public void SeedData()
        {
            _patientRepo.Add(new Patient(1, "John Doe", 30, "Male"));
            _patientRepo.Add(new Patient(2, "Jane Smith", 25, "Female"));
            _patientRepo.Add(new Patient(3, "Alex Johnson", 40, "Male"));

            _prescriptionRepo.Add(new Prescription(1, 1, "Amoxicillin", DateTime.Now.AddDays(-10)));
            _prescriptionRepo.Add(new Prescription(2, 1, "Ibuprofen", DateTime.Now.AddDays(-5)));
            _prescriptionRepo.Add(new Prescription(3, 2, "Paracetamol", DateTime.Now.AddDays(-7)));
            _prescriptionRepo.Add(new Prescription(4, 3, "Metformin", DateTime.Now.AddDays(-3)));
            _prescriptionRepo.Add(new Prescription(5, 2, "Vitamin C", DateTime.Now.AddDays(-1)));
        }

        public void BuildPrescriptionMap()
        {
            _prescriptionMap.Clear();
            foreach (var prescription in _prescriptionRepo.GetAll())
            {
                if (!_prescriptionMap.ContainsKey(prescription.PatientId))
                {
                    _prescriptionMap[prescription.PatientId] = new List<Prescription>();
                }
                _prescriptionMap[prescription.PatientId].Add(prescription);
            }
        }

        public void PrintAllPatients()
        {
            foreach (var patient in _patientRepo.GetAll())
            {
                Console.WriteLine(patient);
            }
        }

        public List<Prescription> GetPrescriptionsByPatientId(int patientId)
        {
            if (_prescriptionMap.ContainsKey(patientId))
            {
                return _prescriptionMap[patientId];
            }
            return new List<Prescription>();
        }

        public void PrintPrescriptionsForPatient(int id)
        {
            var prescriptions = GetPrescriptionsByPatientId(id);
            if (prescriptions.Count == 0)
            {
                Console.WriteLine("No prescriptions found for this patient.");
            }
            else
            {
                foreach (var prescription in prescriptions)
                {
                    Console.WriteLine(prescription);
                }
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var app = new HealthSystemApp();

            app.SeedData();
            app.BuildPrescriptionMap();

            Console.WriteLine("All Patients:");
            app.PrintAllPatients();

            Console.WriteLine("\nPrescriptions for Patient ID 2:");
            app.PrintPrescriptionsForPatient(2);
        }
    }
}
