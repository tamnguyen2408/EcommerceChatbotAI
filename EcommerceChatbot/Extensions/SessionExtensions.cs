﻿using Microsoft.AspNetCore.Http;
using System.Text.Json;


namespace EcommerceChatbot.Extensions
{
    public static class SessionExtensions
    {
        // Phương thức để lưu đối tượng vào session dưới dạng JSON
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        // Phương thức để lấy đối tượng từ session dưới dạng JSON
        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }
    }
}
