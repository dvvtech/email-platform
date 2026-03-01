using Email.Api.BLL.Models;
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
        public string BuildHtmlEmail(EmailDataDto emailData)
        {
            var html = new StringBuilder();

            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html>");
            html.AppendLine("<head>");
            html.AppendLine("<meta charset='UTF-8'>");
            html.AppendLine("<style>");
            html.AppendLine("body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; }");
            html.AppendLine(".container { max-width: 600px; margin: 0 auto; padding: 20px; }");
            html.AppendLine(".header { background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 20px; border-radius: 10px 10px 0 0; }");
            html.AppendLine(".content { background: #fff; padding: 20px; border: 1px solid #e0e0e0; border-radius: 0 0 10px 10px; }");
            html.AppendLine(".section { margin-bottom: 30px; }");
            html.AppendLine(".section-title { font-size: 18px; font-weight: bold; color: #667eea; margin-bottom: 15px; border-bottom: 2px solid #667eea; padding-bottom: 5px; }");
            html.AppendLine(".color-item { margin-bottom: 10px; }");
            html.AppendLine(".color-bar { height: 20px; border-radius: 10px; margin-top: 5px; }");
            html.AppendLine(".footer { margin-top: 30px; text-align: center; color: #999; font-size: 12px; }");
            html.AppendLine("</style>");
            html.AppendLine("</head>");
            html.AppendLine("<body>");
            html.AppendLine("<div class='container'>");
            html.AppendLine("<div class='header'><h2>Результаты тестирования</h2></div>");
            html.AppendLine("<div class='content'>");

            // Информация о пользователе
            html.AppendLine("<div class='section'>");
            html.AppendLine("<div class='section-title'>📋 Информация о пользователе</div>");
            html.AppendLine($"<p><strong>Пол:</strong> {emailData.UserData.Gender}</p>");
            html.AppendLine($"<p><strong>Дата рождения:</strong> {emailData.UserData.BirthDate}</p>");
            html.AppendLine($"<p><strong>Возраст:</strong> {emailData.UserData.Age} лет</p>");
            html.AppendLine($"<p><strong>Знак зодиака:</strong> {emailData.UserData.ZodiacSign}</p>");
            html.AppendLine($"<p><strong>Выбранный тест:</strong> {emailData.UserData.SelectedTest}</p>");
            html.AppendLine("</div>");

            // Статистика цветов
            html.AppendLine("<div class='section'>");
            html.AppendLine("<div class='section-title'>🎨 Использованные цвета</div>");

            foreach (var stat in emailData.Stats.OrderByDescending(s => s.Value.Percentage))
            {
                html.AppendLine("<div class='color-item'>");
                html.AppendLine("<div style='display: flex; justify-content: space-between;'>");
                html.AppendLine($"<span><strong>{stat.Key}</strong></span>&nbsp;&nbsp;");
                html.AppendLine($"<span>{stat.Value.Percentage}% ({stat.Value.Count:N0} пикселей)</span>");
                html.AppendLine("</div>");
                html.AppendLine($"<div class='color-bar' style='background: linear-gradient(90deg, {stat.Value.Hex} {stat.Value.Percentage}%, #e0e0e0 {stat.Value.Percentage}%);'></div>");
                html.AppendLine("</div>");
            }

            int totalPixels = emailData.Stats.Sum(s => s.Value.Count);
            html.AppendLine($"<p style='margin-top: 15px;'><strong>Всего раскрашено пикселей:</strong> {totalPixels:N0}</p>");
            html.AppendLine("</div>");

            // Результаты анализа
            html.AppendLine("<div class='section'>");
            html.AppendLine("<div class='section-title'>📊 Результаты анализа</div>");
            html.AppendLine($"<p><strong>Основная характеристика:</strong><br>{emailData.Results.MainCharacteristic}</p>");

            if (emailData.Results.Strengths?.Any() == true)
            {
                html.AppendLine("<p><strong>Сильные стороны:</strong></p>");
                html.AppendLine("<ul>");
                foreach (var strength in emailData.Results.Strengths)
                {
                    html.AppendLine($"<li>✓ {strength}</li>");
                }
                html.AppendLine("</ul>");
            }

            if (emailData.Results.Recommendations?.Any() == true)
            {
                html.AppendLine("<p><strong>Рекомендации:</strong></p>");
                html.AppendLine("<ul>");
                foreach (var recommendation in emailData.Results.Recommendations)
                {
                    html.AppendLine($"<li>• {recommendation}</li>");
                }
                html.AppendLine("</ul>");
            }

            html.AppendLine("</div>");

            // Подвал
            html.AppendLine("<div class='footer'>");
            html.AppendLine("<p>С уважением,<br>Команда Медико-педагого-психологических тестов<br>Яшара Ибадова</p>");
            html.AppendLine("<p style='font-size: 10px;'>Это письмо сформировано автоматически. Пожалуйста, не отвечайте на него.</p>");
            html.AppendLine("</div>");

            html.AppendLine("</div>");
            html.AppendLine("</div>");
            html.AppendLine("</body>");
            html.AppendLine("</html>");

            return html.ToString();
        }

        /// <summary>
        /// Build complete email body with all test results
        /// </summary>
        public string BuildEmailBody(EmailDataDto emailData)
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

        private void AddUserInfo(UserDataDto userData)
        {
            _bodyBuilder.AppendLine("📋 ИНФОРМАЦИЯ О ПОЛЬЗОВАТЕЛЕ");
            _bodyBuilder.AppendLine(new string('-', 30));
            _bodyBuilder.AppendLine($"Пол: {userData.Gender}");
            _bodyBuilder.AppendLine($"Дата рождения: {userData.BirthDate}");
            _bodyBuilder.AppendLine($"Возраст: {userData.Age} лет");
            _bodyBuilder.AppendLine($"Знак зодиака: {userData.ZodiacSign}");
            _bodyBuilder.AppendLine();
        }

        private void AddTestInfo(UserDataDto userData)
        {
            _bodyBuilder.AppendLine("🎯 ИНФОРМАЦИЯ О ТЕСТЕ");
            _bodyBuilder.AppendLine(new string('-', 30));
            _bodyBuilder.AppendLine($"Выбранный тест: {userData.SelectedTest}");
            _bodyBuilder.AppendLine($"Дата прохождения: {DateTime.Now:dd.MM.yyyy HH:mm}");
            _bodyBuilder.AppendLine();
        }

        private void AddColorStatistics(Dictionary<string, ColorStatisticDto> stats)
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

        private void AddAnalysisResults(AnalysisResultDto results)
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

        private string GenerateColorBar(int percentage, int length = 20)
        {
            int filledLength = (int)Math.Round(percentage / 100.0 * length);
            return "█" + new string('█', filledLength) + new string('░', length - filledLength) + "█";
        }
    }    
}
