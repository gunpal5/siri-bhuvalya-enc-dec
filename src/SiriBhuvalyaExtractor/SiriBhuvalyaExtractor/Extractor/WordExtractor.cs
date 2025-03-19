﻿using System.Text;
using SiriBhuvalyaExtractor.WordMatcher;

public class SanskritWordLookup
{
    private readonly List<SanskritWord> _dictionary;
    private readonly Dictionary<string, string> _vowelToMatraMap;

    public SanskritWordLookup(List<SanskritWord> dictionary)
    {
        _dictionary = dictionary;
        _vowelToMatraMap = InitializeVowelToMatraMap();

    }

    public List<string> ExtractWords(List<string> sanskritLetters)
    {
        List<string> extractedWords = new List<string>();
        int startIndex = 0;

        while (startIndex < sanskritLetters.Count)
        {
            string longestWord = FindLongestWordStartingAt(sanskritLetters, startIndex);
            
            if (!string.IsNullOrEmpty(longestWord))
            {
                extractedWords.Add(longestWord);
                
                // Calculate how many letters were consumed to form this word
                int lettersConsumed = CountLettersInWord(sanskritLetters, startIndex, longestWord);
                startIndex += lettersConsumed;
            }
            else
            {
                // No word found starting at this position, skip one character
                startIndex++;
            }
        }

        return extractedWords;
    }

    private string FindLongestWordStartingAt(List<string> letters, int startIndex)
    {
        string longestWord = string.Empty;
        StringBuilder currentCombination = new StringBuilder();
        
        for (int endIndex = startIndex; endIndex < letters.Count; endIndex++)
        {
            // Process current letter (handle half consonants and matras)
            ProcessLetter(currentCombination, letters[endIndex], endIndex>0 ? letters[endIndex-1]:"");
            
            // Get the current word formed
            string currentWord = currentCombination.ToString();
            
            // Check if this word exists in dictionary
            if (ExistsInDictionary(currentWord) &&currentWord.Length<71 && 
                (currentWord.Length > longestWord.Length || string.IsNullOrEmpty(longestWord)))
            {
                longestWord = currentWord;
            }
        }
        
        return longestWord;
    }

    private void ProcessLetter(StringBuilder builder, string letter, string previousLetter = "")
    {
        // Handle special cases for half consonants and matras
        // This is a simplified version - actual logic may be more complex
        
        // If the letter is a matra, it may need special handling
        if (!_vowelToMatraMap.ContainsValue(previousLetter)  && _vowelToMatraMap.TryGetValue(letter, out string matra))
        {
            // Apply the matra to the previous consonant
            // This is simplified and would need to be adapted to your specific Sanskrit rules
            builder.Remove(builder.Length-1,1).Append(matra);
        }
        // Handle half consonants (consonants without vowels)
        else if (IsHalfConsonant(letter))
        {
            builder.Append(letter);
        }
        else
        {
            // Regular letter
            builder.Append(letter);
        }
    }

    private bool IsHalfConsonant(string letter)
    {
        // Implement logic to determine if a letter is a half consonant
        // This will depend on your specific representation of Sanskrit characters
        return letter.EndsWith("्"); // Example: checking for virama/halant
    }

    private int CountLettersInWord(List<string> letters, int startIndex, string word)
    {
        // Count how many letters from the input list were consumed to form this word
        // This is necessary because a single Sanskrit akshara can be made up of 
        // multiple characters (consonant + vowel mark, etc.)
        
        int letterCount = 0;
        StringBuilder builder = new StringBuilder();
        
        for (int i = startIndex; i < letters.Count; i++)
        {
            ProcessLetter(builder, letters[i]);
            letterCount++;
            
            if (builder.ToString() == word)
                break;
        }
        
        return letterCount;
    }

    private bool ExistsInDictionary(string word)
    {
        // For better performance, you might want to create a HashSet or Dictionary
        // Assuming SanskritWord has a Text property
        return _dictionary.Any(w => w.Word == word);
    }
    
    private Dictionary<string, string> InitializeVowelToMatraMap()
    {
        // Initialize Sanskrit vowels to corresponding matras mapping
        return new Dictionary<string, string>
        {
            // Independent vowels -> Dependent vowel marks (matras)
            { "अ", "" },      // a -> no visible matra
            { "आ", "ा" },     // ā -> ā-matra
            { "आा", "ा" },    // duplicate ā
            { "इ", "ि" },     // i -> i-matra
            { "ई", "ी" },     // ī -> ī-matra
            { "ईी", "ी" },    // duplicate ī
            { "उ", "ु" },     // u -> u-matra
            { "ऊ", "ू" },     // ū -> ū-matra
            { "ऊू", "ू" },    // duplicate ū
            { "ऋ", "ृ" },     // ṛ -> ṛ-matra
            { "ॠ", "ॄ" },     // ṝ -> ṝ-matra
            { "ॠॄ", "ॄ" },    // duplicate ṝ
            { "ळ्", "" },      // ḷ (half consonant)
            { "ळु", "ु" },     // ḷu -> dependent vowel mark
            { "ळू", "ू" },     // ḷū -> dependent vowel mark
            { "ए", "े" },     // e -> e-matra
            { "एा", "े" },     // extended e
            { "एाा", "े" },    // further extended e
            { "ऐ", "ै" },     // ai -> ai-matra
            { "ऐो", "ै" },     // extended ai
            { "ऐोो", "ै" },    // further extended ai
            { "ओ", "ो" },     // o -> o-matra
            { "ओो", "ो" },     // extended o
            { "ओोो", "ो" },    // further extended o
            { "औ", "ौ" },     // au -> au-matra
            { "औौ", "ौ" },     // extended au
            { "औौौ", "ौ" },    // further extended au
            
            
            // Special cases
            { "ं", "ं" },     // anusvāra
            { "ः", "ः" },     // visarga
            { "…", "…" },     // horizontal ellipsis
            { "::", "::" },   // double colon
        };
    }

    // Rest of the word lookup implementation...

    // Helper method to check if a character is a vowel
    public bool IsVowel(string letter)
    {
        return _vowelToMatraMap.ContainsKey(letter);
    }

    // Helper method to check if a character is a matra
    public bool IsMatra(string letter)
    {
        return _vowelToMatraMap.ContainsValue(letter);
    }
    // Helper method to get the corresponding matra for a vowel
    public string GetMatraForVowel(string vowel)
    {
        if (_vowelToMatraMap.TryGetValue(vowel, out string matra))
        {
            return matra;
        }
        return string.Empty;
    }

}