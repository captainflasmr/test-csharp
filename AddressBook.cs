using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Data.Sqlite;

public class AddressBook
{
    private readonly string _connectionString;

    public AddressBook(string dbPath)
    {
        _connectionString = $"Data Source={dbPath}";
        InitializeDatabase();

        // Import contacts from CSV if the table is empty
        if (GetAllContacts().Count == 0)
        {
            string csvPath = Path.Combine(AppContext.BaseDirectory, "default_contacts.csv");
            ImportContactsFromCsv(csvPath);
        }
    }

    private void InitializeDatabase()
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        string sql = @"CREATE TABLE IF NOT EXISTS Contacts (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Name TEXT NOT NULL,
            Email TEXT,
            Phone TEXT
        );";
        using var cmd = conn.CreateCommand();
        cmd.CommandText = sql;
        cmd.ExecuteNonQuery();
    }

    public void AddContact(Contact contact)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "INSERT INTO Contacts (Name, Email, Phone) VALUES (@Name, @Email, @Phone)";
        cmd.Parameters.AddWithValue("@Name", contact.Name);
        cmd.Parameters.AddWithValue("@Email", contact.Email);
        cmd.Parameters.AddWithValue("@Phone", contact.Phone);
        cmd.ExecuteNonQuery();
    }

    public List<Contact> GetAllContacts()
    {
        var contacts = new List<Contact>();
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT Id, Name, Email, Phone FROM Contacts";
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            contacts.Add(new Contact
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Email = reader.GetString(2),
                Phone = reader.GetString(3)
            });
        }
        return contacts;
    }

    public List<Contact> SearchContacts(string name)
    {
        var contacts = new List<Contact>();
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT Id, Name, Email, Phone FROM Contacts WHERE Name LIKE @Name";
        cmd.Parameters.AddWithValue("@Name", "%" + name + "%");
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            contacts.Add(new Contact
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Email = reader.GetString(2),
                Phone = reader.GetString(3)
            });
        }
        return contacts;
    }

    public void ImportContactsFromCsv(string csvPath)
    {
        if (!File.Exists(csvPath))
            return;

        using var reader = new StreamReader(csvPath);
        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            var parts = line.Split(',');
            if (parts.Length < 3) continue;
            var contact = new Contact
            {
                Name = parts[0].Trim(),
                Email = parts[1].Trim(),
                Phone = parts[2].Trim()
            };
            AddContact(contact);
        }
    }
}
