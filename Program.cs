using System;
using System.IO;
using System.Collections.Generic;

class Program
{
    static void Main(string[] args)
    {
        string dbPath = Path.Combine(Environment.CurrentDirectory, "addressbook.db");
        var addressBook = new AddressBook(dbPath);

        while (true)
        {
            Console.WriteLine("\nAddress Book Menu:");
            Console.WriteLine("1. Add Contact");
            Console.WriteLine("2. List Contacts");
            Console.WriteLine("3. Search Contacts");
            Console.WriteLine("4. Exit");
            Console.Write("Select an option: ");
            string? option = Console.ReadLine();

            switch (option)
            {
                case "1":
                    AddContact(addressBook);
                    break;
                case "2":
                    ListContacts(addressBook);
                    break;
                case "3":
                    SearchContacts(addressBook);
                    break;
                case "4":
                    Console.WriteLine("Goodbye!");
                    return;
                default:
                    Console.WriteLine("Invalid option. Try again.");
                    break;
            }
        }
    }

    static void AddContact(AddressBook addressBook)
    {
        Console.Write("Name: ");
        string? name = Console.ReadLine();
        Console.Write("Email: ");
        string? email = Console.ReadLine();
        Console.Write("Phone: ");
        string? phone = Console.ReadLine();
        addressBook.AddContact(new Contact { Name = name ?? "", Email = email ?? "", Phone = phone ?? "" });
        Console.WriteLine("Contact added.");
    }

    static void ListContacts(AddressBook addressBook)
    {
        List<Contact> contacts = addressBook.GetAllContacts();
        if (contacts.Count == 0)
        {
            Console.WriteLine("No contacts found.");
            return;
        }
        foreach (var c in contacts)
        {
            Console.WriteLine($"{c.Id}: {c.Name}, {c.Email}, {c.Phone}");
        }
    }

    static void SearchContacts(AddressBook addressBook)
    {
        Console.Write("Enter name to search: ");
        string? name = Console.ReadLine();
        List<Contact> contacts = addressBook.SearchContacts(name ?? "");
        if (contacts.Count == 0)
        {
            Console.WriteLine("No contacts found.");
            return;
        }
        foreach (var c in contacts)
        {
            Console.WriteLine($"{c.Id}: {c.Name}, {c.Email}, {c.Phone}");
        }
    }
}
