using System;
using System.Collections.Generic;
using System.Linq;

class PlayfairCipher
{
    private char[,] table;
    private string key;

    public PlayfairCipher(string key)
    {
        this.key = PrepareKey(key);
        this.table = CreateTable(this.key);
    }

    private string PrepareKey(string key)
    {
        key = key.ToUpper().Replace("Ё", "Е");
        return new string(key.Distinct().ToArray());
    }

    private char[,] CreateTable(string key)
    {
        string alphabet = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";
        string combinedKey = key + new string(alphabet.Except(key).ToArray());
        char[,] table = new char[6, 6];

        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                if (i * 6 + j < combinedKey.Length)
                {
                    table[i, j] = combinedKey[i * 6 + j];
                }
                else
                {
                    table[i, j] = ' ';
                }
            }
        }
        return table;
    }

    private (int, int) FindPosition(char c)
    {
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                if (table[i, j] == c)
                {
                    return (i, j);
                }
            }
        }
        throw new Exception("Символ не найден в таблице.");
    }

    public string Encrypt(string plaintext)
    {
        plaintext = plaintext.ToUpper().Replace("Ё", "Е").Replace(" ", "");
        if (plaintext.Length % 2 != 0)
        {
            plaintext += 'Х';
        }

        string ciphertext = "";
        for (int i = 0; i < plaintext.Length; i += 2)
        {
            char char1 = plaintext[i];
            char char2 = plaintext[i + 1];

            if (char1 == char2)
            {
                char2 = 'Х';
                i--;
            }

            var (row1, col1) = FindPosition(char1);
            var (row2, col2) = FindPosition(char2);

            if (row1 == row2)
            {
                ciphertext += table[row1, (col1 + 1) % 6];
                ciphertext += table[row2, (col2 + 1) % 6];
            }
            else if (col1 == col2)
            {
                ciphertext += table[(row1 + 1) % 6, col1];
                ciphertext += table[(row2 + 1) % 6, col2];
            }
            else
            {
                ciphertext += table[row1, col2];
                ciphertext += table[row2, col1];
            }
        }

        return ciphertext;
    }

    public string Decrypt(string ciphertext)
    {
        string plaintext = "";
        for (int i = 0; i < ciphertext.Length; i += 2)
        {
            char char1 = ciphertext[i];
            char char2 = ciphertext[i + 1];
            var (row1, col1) = FindPosition(char1);
            var (row2, col2) = FindPosition(char2);

            if (row1 == row2)
            {
                plaintext += table[row1, (col1 + 5) % 6];
                plaintext += table[row2, (col2 + 5) % 6];
            }
            else if (col1 == col2)
            {
                plaintext += table[(row1 + 5) % 6, col1];
                plaintext += table[(row2 + 5) % 6, col2];
            }
            else
            {
                plaintext += table[row1, col2];
                plaintext += table[row2, col1];
            }
        }

        return plaintext;
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.Write("Введите ключ: ");
        string key = Console.ReadLine();

        PlayfairCipher playfair = new PlayfairCipher(key);

        Console.WriteLine("Выберите действие: 1 - Зашифровать, 2 - Расшифровать");
        string choice = Console.ReadLine();

        if (choice == "1")
        {
            Console.Write("Введите текст для шифрования: ");
            string plaintext = Console.ReadLine();
            string encrypted = playfair.Encrypt(plaintext);
            Console.WriteLine($"Зашифрованный текст: {encrypted}");
        }
        else if (choice == "2")
        {
            Console.Write("Введите текст для расшифровки: ");
            string ciphertext = Console.ReadLine();
            string decrypted = playfair.Decrypt(ciphertext);
            Console.WriteLine($"Расшифрованный текст: {decrypted}");
        }
        else
        {
            Console.WriteLine("Неверный выбор.");
        }
    }
}