namespace HangmanH1
{
    internal class Program
    {
        const string gallows = @"
╔═══╤═
║
║ 
║
║ 
║ 
║ ";
        static readonly string[] man = {"    │", "\n    O", "\n\n   ─", "\n\n    ╫", "\n\n     ─",
            "\n\n\n   ╔", "\n\n\n    ╩", "\n\n\n     ╗", "\n\n\n\n   ╜", "\n\n\n\n     ╙"};
        static string usedLetters = "";
        static int life;

        static void Main(string[] args)
        {
            do Setup();
            while (Console.ReadKey(true).Key != ConsoleKey.N);
        }

        static void Setup()
        {
            Console.CursorVisible = false;
            Console.Clear();
            life = man.Length;
            usedLetters = "";
            DrawHangman();

            string[]? words = GetWordsFromApi();
            if (words == null)
            {
                Console.WriteLine("No words found online. Using Hardcoded wordlist.");
                words = new string[] { "umbrella", "tube", "spaghetti", "tablespoon", "xylophone", "video", };
            }
            string word = GetRandomWord(words);

            while (Game(word)) ;

            if (life > 0) Write("Congratulations. You guessed the word.\n Try again?", 1, 10, ConsoleColor.Green);
            else Write($"You lost. The word was {word}.\n Try again?", 1, 10, ConsoleColor.Red);
        }

        static string[]? GetWordsFromApi()
        {
            using (HttpClient client = new())
            {
                //string url = @"https://random-word-api.herokuapp.com/word?number=10";
                string url = @"https://random-word-form.repl.co/random/noun?count=10";
                string json = client.GetStringAsync(url).Result;
                string[]? words = System.Text.Json.JsonSerializer.Deserialize<string[]>(json);
                return words;
            }
        }

        static string GetRandomWord(string[] words)
        {
            int i = new Random().Next(words.Length);
            return words[i].ToUpper();
        }

        static bool Game(string word)
        {
            if (ShowWord(word) || life == 0) return false;
            char letter = GuessLetter();
            if (!CheckLetter(word, letter))
            {
                life--;
                DrawHangman();
            }
            return true;
        }

        static bool ShowWord(string word)
        {
            bool isWon = true;
            for (int i = 0; i < word.Length; i++)
            {
                if (usedLetters.Contains(word[i]) || word[i] == '-')
                    Write(word[i], 10 + i, 2);
                else
                {
                    Write('_', 10 + i, 2);
                    isWon = false;
                }
            }
            return isWon;
        }

        static char GuessLetter()
        {
            while (true)
            {
                Write("Guess a letter", 10, 4, ConsoleColor.Yellow);
                char character = char.ToUpper(Console.ReadKey(true).KeyChar);
                if (char.IsLetter(character) && !usedLetters.Contains(character))
                {
                    usedLetters += character;
                    ShowGuessedLetters();
                    return character;
                }
            }
        }

        static bool CheckLetter(string word, char letter)
        {
            if (!word.Contains(letter))
            {
                Write($"{letter} is not in the word".PadRight(20), 10, 6, ConsoleColor.Red);
                return false;
            }
            Write($"The word contains {letter}".PadRight(20), 10, 6, ConsoleColor.Green);
            return true;
        }

        static void ShowGuessedLetters()
        {
            char[] characters = usedLetters.ToArray();
            Array.Sort(characters);
            usedLetters = new string(characters);
            Write($"Used: {usedLetters}", 10, 8, ConsoleColor.Cyan);
        }

        static void Write(object t, int x, int y, ConsoleColor c = ConsoleColor.White)
        {
            Console.ForegroundColor = c;
            Console.SetCursorPosition(x, y);
            Console.Write(t.ToString());
        }

        static void DrawHangman()
        {
            for (int i = man.Length - 1; i >= life; i--)
                Write(man[i], 0, 3, ConsoleColor.Red);
            Write(gallows, 0, 1, ConsoleColor.DarkBlue);
        }
    }
}