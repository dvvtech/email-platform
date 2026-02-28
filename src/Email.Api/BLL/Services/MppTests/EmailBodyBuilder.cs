using Email.Models.MppTests;
using System.Text;

namespace Email.Api.BLL.Services.MppTests
{
    /// <summary>
    /// Service for formatting email content with test results
    /// </summary>
    public class EmailBodyBuilder
    {
        private readonly StringBuilder _bodyBuilder;
        private readonly string _userName;

        public EmailBodyBuilder(string userName = "Уважаемый пользователь")
        {
            _userName = userName;
            _bodyBuilder = new StringBuilder();
        }

        /// <summary>
        /// Build complete email body with all test results
        /// </summary>
        public string BuildEmailBody(EmailData emailData)
        {
            AddHeader();
            AddUserInfo(emailData.UserData);
            AddTestInfo(emailData.UserData);
            AddColorStatistics(emailData.Stats);
            AddAnalysisResults(emailData.Results);
            AddFooter();

            return _bodyBuilder.ToString();
        }

        private void AddHeader()
        {
            _bodyBuilder.AppendLine($"Здравствуйте, {_userName}!");
            _bodyBuilder.AppendLine();
            _bodyBuilder.AppendLine("Вы получили это письмо, потому что прошли Медико-педагого-психологический тест Яшара Ибадова.");
            _bodyBuilder.AppendLine();
            _bodyBuilder.AppendLine("Ниже представлены результаты вашего тестирования:");
            _bodyBuilder.AppendLine(new string('-', 50));
            _bodyBuilder.AppendLine();
        }

        private void AddUserInfo(UserData userData)
        {
            _bodyBuilder.AppendLine("📋 ИНФОРМАЦИЯ О ПОЛЬЗОВАТЕЛЕ");
            _bodyBuilder.AppendLine(new string('-', 30));
            _bodyBuilder.AppendLine($"Пол: {userData.Gender}");
            _bodyBuilder.AppendLine($"Дата рождения: {userData.BirthDate}");
            _bodyBuilder.AppendLine($"Возраст: {userData.Age} лет");
            _bodyBuilder.AppendLine($"Знак зодиака: {userData.ZodiacSign}");
            _bodyBuilder.AppendLine();
        }

        private void AddTestInfo(UserData userData)
        {
            _bodyBuilder.AppendLine("🎯 ИНФОРМАЦИЯ О ТЕСТЕ");
            _bodyBuilder.AppendLine(new string('-', 30));
            _bodyBuilder.AppendLine($"Выбранный тест: {userData.SelectedTest}");
            _bodyBuilder.AppendLine($"Дата прохождения: {DateTime.Now:dd.MM.yyyy HH:mm}");
            _bodyBuilder.AppendLine();
        }

        private void AddColorStatistics(Dictionary<string, ColorStatistic> stats)
        {
            if (stats == null || stats.Count == 0)
            {
                _bodyBuilder.AppendLine("🎨 ИСПОЛЬЗОВАННЫЕ ЦВЕТА");
                _bodyBuilder.AppendLine(new string('-', 30));
                _bodyBuilder.AppendLine("Цвета не были использованы в тесте.");
                _bodyBuilder.AppendLine();
                return;
            }

            _bodyBuilder.AppendLine("🎨 ИСПОЛЬЗОВАННЫЕ ЦВЕТА");
            _bodyBuilder.AppendLine(new string('-', 30));

            var sortedStats = stats.OrderByDescending(s => s.Value.Percentage);

            foreach (var stat in sortedStats)
            {
                string colorBar = GenerateColorBar(stat.Value.Percentage);
                _bodyBuilder.AppendLine($"{stat.Key}: {stat.Value.Percentage}% ({stat.Value.Count} пикселей)");
                _bodyBuilder.AppendLine($"  {colorBar}");
            }

            // Добавляем общую статистику
            int totalPixels = stats.Sum(s => s.Value.Count);
            _bodyBuilder.AppendLine();
            _bodyBuilder.AppendLine($"Всего раскрашено пикселей: {totalPixels:N0}");
            _bodyBuilder.AppendLine();
        }

        private void AddAnalysisResults(AnalysisResult results)
        {
            _bodyBuilder.AppendLine("📊 РЕЗУЛЬТАТЫ АНАЛИЗА");
            _bodyBuilder.AppendLine(new string('-', 30));

            // Основная характеристика
            _bodyBuilder.AppendLine("Основная характеристика:");
            _bodyBuilder.AppendLine($"> {results.MainCharacteristic}");
            _bodyBuilder.AppendLine();

            // Сильные стороны
            if (results.Strengths != null && results.Strengths.Any())
            {
                _bodyBuilder.AppendLine("Сильные стороны:");
                foreach (var strength in results.Strengths)
                {
                    _bodyBuilder.AppendLine($"  ✓ {strength}");
                }
                _bodyBuilder.AppendLine();
            }

            // Рекомендации
            if (results.Recommendations != null && results.Recommendations.Any())
            {
                _bodyBuilder.AppendLine("Рекомендации:");
                foreach (var recommendation in results.Recommendations)
                {
                    _bodyBuilder.AppendLine($"  • {recommendation}");
                }
                _bodyBuilder.AppendLine();
            }
        }

        private void AddFooter()
        {
            _bodyBuilder.AppendLine(new string('=', 50));
            _bodyBuilder.AppendLine();
            _bodyBuilder.AppendLine("С уважением,");
            _bodyBuilder.AppendLine("Команда Медико-педагого-психологических тестов");
            _bodyBuilder.AppendLine("Яшара Ибадова");
            _bodyBuilder.AppendLine();
            _bodyBuilder.AppendLine("---");
            _bodyBuilder.AppendLine("Это письмо сформировано автоматически. Пожалуйста, не отвечайте на него.");
        }

        private string GetGenderText(string gender)
        {
            return gender?.ToLower() switch
            {
                "male" => "Мужской",
                "female" => "Женский",
                "м" => "Мужской",
                "ж" => "Женский",
                _ => "Не указан"
            };
        }

        private string GenerateColorBar(int percentage, int length = 20)
        {
            int filledLength = (int)Math.Round(percentage / 100.0 * length);
            return "█" + new string('█', filledLength) + new string('░', length - filledLength) + "█";
        }
    }

    /// <summary>
    /// Data models for email content
    /// </summary>
    public class EmailData
    {
        public UserData UserData { get; set; }
        public Dictionary<string, ColorStatistic> Stats { get; set; }
        public AnalysisResult Results { get; set; }
    }
}
