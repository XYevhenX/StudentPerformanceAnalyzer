using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using StudentPerformanceAnalyzer.Models;
using StudentPerformanceAnalyzer.Services;
using StudentPerformanceAnalyzer.Strategies;

namespace StudentPerformanceAnalyzer.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly IXmlDataService _dataService;
        private readonly SaxSearchStrategy _saxStrategy;
        private readonly DomSearchStrategy _domStrategy;
        private readonly LinqSearchStrategy _linqStrategy;

        private const string AllOption = "Всі";

        private string? _selectedFaculty;
        private string? _selectedDepartment;
        private string? _selectedCourse;
        private string _selectedMethod = "LINQ to XML";
        private string _statusMessage = "Оберіть параметри та натисніть «Пошук»";

        public ObservableCollection<string> Faculties { get; } = new();
        public ObservableCollection<string> Departments { get; } = new();
        public ObservableCollection<string> Courses { get; } = new();
        public ObservableCollection<Student> SearchResults { get; } = new();

        public List<string> SearchMethods { get; } = new()
        {
            "LINQ to XML",
            "DOM API",
            "SAX API"
        };

        public string? SelectedFaculty
        {
            get => _selectedFaculty;
            set => SetProperty(ref _selectedFaculty, value);
        }

        public string? SelectedDepartment
        {
            get => _selectedDepartment;
            set => SetProperty(ref _selectedDepartment, value);
        }

        public string? SelectedCourse
        {
            get => _selectedCourse;
            set => SetProperty(ref _selectedCourse, value);
        }

        public string SelectedMethod
        {
            get => _selectedMethod;
            set => SetProperty(ref _selectedMethod, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public ICommand LoadFiltersCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand ExportHtmlCommand { get; }

        public MainViewModel(
            IXmlDataService dataService,
            SaxSearchStrategy saxStrategy,
            DomSearchStrategy domStrategy,
            LinqSearchStrategy linqStrategy)
        {
            _dataService = dataService;
            _saxStrategy = saxStrategy;
            _domStrategy = domStrategy;
            _linqStrategy = linqStrategy;

            LoadFiltersCommand = new Command(async () => await LoadFiltersAsync());
            SearchCommand = new Command(async () => await ExecuteSearchAsync());
            ClearCommand = new Command(ExecuteClear);
            ExportHtmlCommand = new Command(async () => await ExportToHtmlAsync());
        }

        public async Task LoadFiltersAsync()
        {
            var faculties = await _dataService.GetUniqueValuesAsync("Faculty");
            Faculties.Clear();
            Faculties.Add(AllOption);
            foreach (var item in faculties)
                Faculties.Add(item);

            var departments = await _dataService.GetUniqueValuesAsync("Department");
            Departments.Clear();
            Departments.Add(AllOption);
            foreach (var item in departments)
                Departments.Add(item);

            var courses = await _dataService.GetUniqueValuesAsync("Course");
            Courses.Clear();
            Courses.Add(AllOption);
            foreach (var item in courses)
                Courses.Add(item);

            SelectedFaculty = AllOption;
            SelectedDepartment = AllOption;
            SelectedCourse = AllOption;
        }

        private async Task ExecuteSearchAsync()
        {
            var criteria = new SearchCriteria
            {
                Faculty = SelectedFaculty == AllOption ? null : SelectedFaculty,
                Department = SelectedDepartment == AllOption ? null : SelectedDepartment,
                Course = SelectedCourse == AllOption ? null : SelectedCourse
            };

            IXmlSearchStrategy baseStrategy = SelectedMethod switch
            {
                "SAX API" => _saxStrategy,
                "DOM API" => _domStrategy,
                _ => _linqStrategy
            };

            var decoratedStrategy = new SearchTimingDecorator(baseStrategy);

            using var stream = await _dataService.GetXmlStreamAsync();
            var results = decoratedStrategy.Search(stream, criteria);

            SearchResults.Clear();
            foreach (var student in results)
            {
                SearchResults.Add(student);
            }

            StatusMessage = $"Знайдено студентів: {results.Count} | Метод: {SelectedMethod} | Час: {decoratedStrategy.LastExecutionTime.TotalMilliseconds:F3} мс";
        }

        private void ExecuteClear()
        {
            SelectedFaculty = AllOption;
            SelectedDepartment = AllOption;
            SelectedCourse = AllOption;
            SearchResults.Clear();
            StatusMessage = "Параметри пошуку та результати очищено";
        }

        private async Task ExportToHtmlAsync()
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string outputPath = Path.Combine(desktopPath, "StudentsReport.html");

            await _dataService.TransformToHtmlAsync(outputPath);
            StatusMessage = $"HTML-звіт збережено на Робочий стіл: {outputPath}";

            await Launcher.Default.OpenAsync(new OpenFileRequest
            {
                File = new ReadOnlyFile(outputPath)
            });
        }
    }
}