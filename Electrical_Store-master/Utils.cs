using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Electrical_Store
{
    public static class Utils
    {
        // Иванов Иван Иванович -> Иванов И.И.
        public static string MaskName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName)) return "";
            string[] parts = fullName.Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 3) return $"{parts[0]} {parts[1][0]}.{parts[2][0]}.";
            if (parts.Length == 2) return $"{parts[0]} {parts[1][0]}.";
            return fullName;
        }

        // +7.906.100.67.88 -> +7.***.***.67.88
        public static string MaskPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return "";
            // Убираем все, кроме цифр для анализа
            string digits = System.Text.RegularExpressions.Regex.Replace(phone, @"[^\d]", "");
            if (digits.Length == 11)
            {
                return $"+{digits[0]}.***.***.{digits.Substring(7, 2)}.{digits.Substring(9, 2)}";
            }
            return phone; // Если формат неизвестен, возвращаем как есть
        }
    }
}
