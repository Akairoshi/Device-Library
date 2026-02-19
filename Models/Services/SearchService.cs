using System.Collections;
using System.Reflection;

namespace Device_Library.Models.Data
{
    public static class SearchService
    {
        // основной метод поиска
        public static List<DeviceParams> SearchDeviceFromList(List<DeviceParams> devices, string userInput)
        {
            if (devices == null || string.IsNullOrWhiteSpace(userInput))
                return [];

            // вернуть объект, который содержит ИМЕННО ТО ЧТО ИСКАЛ человек
            var exactMatches = devices
                .Where(device => device != null && CheckContains(device, userInput))
                .ToList();

            if(exactMatches.Count != 0)
                return exactMatches;

            var words = userInput.Split([' '], StringSplitOptions.RemoveEmptyEntries);

            // вернуть объект, которые содержат все слова из запроса.
            return [.. devices.Where(device => device != null && words.All(word => CheckContains(device, word)))];
        }

        public static bool CheckContains(object obj, string searchWord)
        {
            if (obj == null) return false;

            Type type = obj.GetType();

            // если это строка — проверяем
            if (obj is string strVal)
            {
                return strVal.Contains(searchWord, StringComparison.OrdinalIgnoreCase);
            }
            if (type.IsPrimitive || type == typeof(decimal))
            {
                string valStr = obj.ToString()!;

                // Если искомое слово — это число, то проверяем на полное совпадение
                if (int.TryParse(searchWord, out _))
                {
                    return valStr.Equals(searchWord, StringComparison.OrdinalIgnoreCase);
                }
                return valStr.Contains(searchWord, StringComparison.OrdinalIgnoreCase);
            }
            // если это enum - проверяем
            if (type.IsEnum)
            {
                return obj.ToString()!.Contains(searchWord, StringComparison.OrdinalIgnoreCase);
            }

            // если это коллекции - проход
            if (obj is IEnumerable enumerable)
            {
                foreach (var item in enumerable)
                {
                    if (CheckContains(item, searchWord)) return true;
                }
                return false; // Если в коллекции ничего не нашлось
            }

            // Сложные объекты, классы, структуры — проход

            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in properties)
            {
                // Пропускаем индексаторы, чтобы не словить ошибку
                if (prop.GetIndexParameters().Length > 0) continue;

                var value = prop.GetValue(obj);
                if (CheckContains(value!, searchWord)) return true;
            }

            return false;
        }
    }
}