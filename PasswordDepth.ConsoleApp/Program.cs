
using PasswordDepth;
using System.Security.Cryptography;
using System.Text;


var depthSystem = new PasswordDepthSystem();

    Console.WriteLine("=== Password Depth System ===\n");

    // Generate passwords with different depths
    Console.WriteLine("Generating passwords with varying depths:\n");
    for (int depth = 1; depth <= 5; depth++)
    {
        string password = depthSystem.GeneratePasswordWithDepth(depth, 5);
        Console.WriteLine($"Depth {depth}: {password}");
    }

    Console.WriteLine("\n" + new string('=', 50) + "\n");

    // Analyze existing passwords
    Console.WriteLine("Analyzing password depths:\n");

    string[] testPasswords = {
                    "hello",
                    "Hello123",
                    "H3ll@W0rld!",
                    "P@ssw0rd!2023#Secure",
                    "aB3$x9Kp2Ff8e4A1"
                };

    foreach (var pwd in testPasswords)
    {
        var analysis = depthSystem.AnalyzePasswordDepth(pwd);
        Console.WriteLine($"Password: '{pwd}'");
        Console.WriteLine(analysis);
        Console.WriteLine();
    }

    // Verify depth requirements
    Console.WriteLine(new string('=', 50) + "\n");
    Console.WriteLine("Verifying depth requirements (minimum depth: 3):\n");

    foreach (var pwd in testPasswords)
    {
        bool meetsRequirement = depthSystem.VerifyDepthRequirement(pwd, 3);
        Console.WriteLine($"'{pwd}' - {(meetsRequirement ? "✓ PASS" : "✗ FAIL")}");
    }

    Console.ReadKey();


namespace PasswordDepth
{
    /// <summary>
    /// Password Depth System - A new paradigm for password complexity
    /// Instead of measuring by length and symbols, we measure by DEPTH
    /// Each depth layer represents an independent transformation/randomization
    /// </summary>
    public class PasswordDepthSystem
    {
        private readonly RandomNumberGenerator _rng;

        public PasswordDepthSystem()
        {
            _rng = RandomNumberGenerator.Create();
        }

        /// <summary>
        /// Generates a password with specified depth
        /// Each depth level adds a layer of random transformation
        /// </summary>
        public string GeneratePasswordWithDepth(int depth, int baseLength = 8)
        {
            if (depth < 1) throw new ArgumentException("Depth must be at least 1");

            string password = GenerateRandomBase(baseLength);

            for (int layer = 1; layer < depth; layer++)
            {
                password = ApplyDepthLayer(password, layer);
            }

            return password;
        }

        /// <summary>
        /// Analyzes a password and calculates its depth
        /// Depth is determined by detecting layers of transformation patterns
        /// </summary>
        public PasswordDepthAnalysis AnalyzePasswordDepth(string password)
        {
            if (string.IsNullOrEmpty(password))
                return new PasswordDepthAnalysis { Depth = 0, Description = "Empty password" };

            int depth = 0;
            List<string> detectedLayers = new List<string>();

            // Layer 1: Base random characters (always present if password exists)
            depth++;
            detectedLayers.Add("Base random layer");

            // Layer 2: Check for character diversity
            if (HasCharacterDiversity(password))
            {
                depth++;
                detectedLayers.Add("Character diversity layer");
            }

            // Layer 3: Check for positional randomness
            if (HasPositionalRandomness(password))
            {
                depth++;
                detectedLayers.Add("Positional randomness layer");
            }

            // Layer 4: Check for encoding patterns
            if (HasEncodingPattern(password))
            {
                depth++;
                detectedLayers.Add("Encoding pattern layer");
            }

            // Layer 5: Check for cryptographic depth (hashed segments)
            if (HasCryptographicDepth(password))
            {
                depth++;
                detectedLayers.Add("Cryptographic depth layer");
            }

            return new PasswordDepthAnalysis
            {
                Depth = depth,
                Layers = detectedLayers,
                Description = $"Password has {depth} layers of depth",
                Strength = CalculateStrength(depth)
            };
        }

        /// <summary>
        /// Verifies if a password meets minimum depth requirements
        /// </summary>
        public bool VerifyDepthRequirement(string password, int minimumDepth)
        {
            var analysis = AnalyzePasswordDepth(password);
            return analysis.Depth >= minimumDepth;
        }

        #region Private Helper Methods

        private string GenerateRandomBase(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var bytes = new byte[length];
            _rng.GetBytes(bytes);

            return new string(bytes.Select(b => chars[b % chars.Length]).ToArray());
        }

        private string ApplyDepthLayer(string input, int layerNumber)
        {
            // Each layer applies a different transformation
            switch (layerNumber % 4)
            {
                case 0:
                    return InterleaveWithSymbols(input);
                case 1:
                    return ApplyRotation(input);
                case 2:
                    return AddHashSegment(input);
                case 3:
                    return ApplyXorTransform(input);
                default:
                    return input;
            }
        }

        private string InterleaveWithSymbols(string input)
        {
            const string symbols = "!@#$%^&*-_+=";
            var result = new StringBuilder();
            var bytes = new byte[input.Length];
            _rng.GetBytes(bytes);

            for (int i = 0; i < input.Length; i++)
            {
                result.Append(input[i]);
                if (i % 2 == 0)
                {
                    result.Append(symbols[bytes[i] % symbols.Length]);
                }
            }
            return result.ToString();
        }

        private string ApplyRotation(string input)
        {
            var bytes = new byte[1];
            _rng.GetBytes(bytes);
            int rotation = bytes[0] % input.Length;

            return input.Substring(rotation) + input.Substring(0, rotation);
        }

        private string AddHashSegment(string input)
        {
            using (var sha256 = SHA256.Create())
            {
                var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                var hashSegment = BitConverter.ToString(hash).Replace("-", "").Substring(0, 4);
                return input + hashSegment;
            }
        }

        private string ApplyXorTransform(string input)
        {
            var bytes = new byte[1];
            _rng.GetBytes(bytes);
            byte xorKey = bytes[0];

            var transformed = input.Select(c => (char)(c ^ xorKey)).ToArray();
            return new string(transformed);
        }

        private bool HasCharacterDiversity(string password)
        {
            bool hasUpper = password.Any(char.IsUpper);
            bool hasLower = password.Any(char.IsLower);
            bool hasDigit = password.Any(char.IsDigit);
            bool hasSymbol = password.Any(c => !char.IsLetterOrDigit(c));

            return new[] { hasUpper, hasLower, hasDigit, hasSymbol }.Count(x => x) >= 2;
        }

        private bool HasPositionalRandomness(string password)
        {
            if (password.Length < 3) return false;

            // Check if characters don't follow a simple sequential pattern
            int sequentialCount = 0;
            for (int i = 0; i < password.Length - 1; i++)
            {
                if (Math.Abs(password[i] - password[i + 1]) == 1)
                    sequentialCount++;
            }

            return sequentialCount < password.Length / 2;
        }

        private bool HasEncodingPattern(string password)
        {
            // Check for hex patterns, base64 patterns, etc.
            return password.Any(c => "!@#$%^&*-_+=".Contains(c)) ||
                   (password.Length % 4 == 0 && password.All(c => char.IsLetterOrDigit(c) || c == '+' || c == '/'));
        }

        private bool HasCryptographicDepth(string password)
        {
            // Check if password contains patterns typical of hashed content
            if (password.Length < 8) return false;

            // Look for hexadecimal segments or high entropy
            int hexCharCount = password.Count(c => "0123456789ABCDEFabcdef".Contains(c));
            return hexCharCount > password.Length * 0.5;
        }

        private string CalculateStrength(int depth)
        {
            return depth switch
            {
                1 => "Very Weak",
                2 => "Weak",
                3 => "Moderate",
                4 => "Strong",
                >= 5 => "Very Strong",
                _ => "Unknown"
            };
        }

        #endregion
    }

    /// <summary>
    /// Result of password depth analysis
    /// </summary>
    public class PasswordDepthAnalysis
    {
        public int Depth { get; set; }
        public List<string> Layers { get; set; } = new List<string>();
        public string Description { get; set; }
        public string Strength { get; set; }

        public override string ToString()
        {
            var layerInfo = string.Join("\n  - ", Layers);
            return $"{Description}\nStrength: {Strength}\nDetected Layers:\n  - {layerInfo}";
        }
    }

   
}
