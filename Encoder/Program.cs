using Encoder.Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace Encoder
{
    class Program
    {
        static void Main(string[] args)
        {
            var id = Environment.TickCount;
            var contact = new Contact
            {
                ID = id,
                LastName = "Jane " + id.ToString(),
                FirstName = "Doe " + id,
                Gender = (id % 2 == 0) ? "M" : "F",
                DofB = new DateTime(1980, 1, 1)
            };

            SerializeAndSaveToFile(id, contact);
            DeserializeAndTest(id);
        }

        private static void SerializeAndSaveToFile(int id, Contact contact)
        {
            var json = JsonConvert.SerializeObject(contact);
            var b64bytes = Convert.ToBase64String(Encoding.ASCII.GetBytes(json));
            var file = $"contact{id}.b64";
            File.WriteAllBytes(file, Encoding.ASCII.GetBytes(b64bytes));
        }

        private static void DeserializeAndTest(int id)
        {
            var file = $"contact{id}.b64";
            var buffer = File.ReadAllBytes(file);
            var base64string = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
            byte[] base64bytes = Convert.FromBase64String(base64string);
            var json = Encoding.UTF8.GetString(base64bytes, 0, base64bytes.Length);
            var contact = JsonConvert.DeserializeObject<Contact>(json);
        }
    }
}
