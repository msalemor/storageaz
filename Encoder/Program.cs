using Encoder.Models;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Encoder
{
    class Program
    {
        static void Main(string[] args)
        {
            // Get an ID from the tickcount
            var id = Environment.TickCount;

            // Create a new contact
            var contact = new Contact
            {
                ID = id,
                LastName = "Jane " + id.ToString(),
                FirstName = "Doe " + id,
                Gender = (id % 2 == 0) ? "M" : "F",
                DofB = new DateTime(1980, 1, 1)
            };

            // Serialize the contact to JSON and Save it to a file
            SerializeAndSaveToFile(id, contact);

            // Read the file and deserialize
            DeserializeAndTest(id);
        }

        private static void SerializeAndSaveToFile(int id, Contact contact)
        {
            // Serialize to JSON
            var json = JsonConvert.SerializeObject(contact);
            // Serialize to Base64
            var b64bytes = Convert.ToBase64String(Encoding.ASCII.GetBytes(json));
            // Write Base64 to file
            var file = $"contact{id}.b64";
            File.WriteAllBytes(file, Encoding.ASCII.GetBytes(b64bytes));
            Console.WriteLine($"File: {file} created");
        }

        private static void DeserializeAndTest(int id)
        {
            // Read the Base64 file
            var file = $"contact{id}.b64";
            var buffer = File.ReadAllBytes(file);
            // Deserialize Base64
            var base64string = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
            byte[] base64bytes = Convert.FromBase64String(base64string);
            // Deserialize to JSON
            var json = Encoding.UTF8.GetString(base64bytes, 0, base64bytes.Length);
            // Deserialize JSON to .Net object
            var contact = JsonConvert.DeserializeObject<Contact>(json);
            // Test that the IDs match
            if (id == contact.ID)
            {
                Console.WriteLine("Test passed.");
            }

        }
    }
}
