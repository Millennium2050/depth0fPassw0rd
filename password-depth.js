/**
 * Password Depth System - A new paradigm for password complexity
 * Instead of measuring by length and symbols, we measure by DEPTH
 * Each depth layer represents an independent transformation/randomization
 */

const crypto = require('crypto');

class PasswordDepthSystem {
    /**
     * Generates a password with specified depth
     * Each depth level adds a layer of random transformation
     */
    generatePasswordWithDepth(depth, baseLength = 8) {
        if (depth < 1) throw new Error("Depth must be at least 1");

        let password = this.generateRandomBase(baseLength);

        for (let layer = 1; layer < depth; layer++) {
            password = this.applyDepthLayer(password, layer);
        }

        return password;
    }

    /**
     * Analyzes a password and calculates its depth
     * Depth is determined by detecting layers of transformation patterns
     */
    analyzePasswordDepth(password) {
        if (!password || password.length === 0) {
            return {
                depth: 0,
                layers: [],
                description: "Empty password",
                strength: "None"
            };
        }

        let depth = 0;
        const detectedLayers = [];

        // Layer 1: Base random characters (always present if password exists)
        depth++;
        detectedLayers.push("Base random layer");

        // Layer 2: Check for character diversity
        if (this.hasCharacterDiversity(password)) {
            depth++;
            detectedLayers.push("Character diversity layer");
        }

        // Layer 3: Check for positional randomness
        if (this.hasPositionalRandomness(password)) {
            depth++;
            detectedLayers.push("Positional randomness layer");
        }

        // Layer 4: Check for encoding patterns
        if (this.hasEncodingPattern(password)) {
            depth++;
            detectedLayers.push("Encoding pattern layer");
        }

        // Layer 5: Check for cryptographic depth (hashed segments)
        if (this.hasCryptographicDepth(password)) {
            depth++;
            detectedLayers.push("Cryptographic depth layer");
        }

        return {
            depth,
            layers: detectedLayers,
            description: `Password has ${depth} layers of depth`,
            strength: this.calculateStrength(depth)
        };
    }

    /**
     * Verifies if a password meets minimum depth requirements
     */
    verifyDepthRequirement(password, minimumDepth) {
        const analysis = this.analyzePasswordDepth(password);
        return analysis.depth >= minimumDepth;
    }

    // Private Helper Methods

    generateRandomBase(length) {
        const chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        const bytes = crypto.randomBytes(length);

        return Array.from(bytes)
            .map(b => chars[b % chars.length])
            .join('');
    }

    applyDepthLayer(input, layerNumber) {
        // Each layer applies a different transformation
        switch (layerNumber % 4) {
            case 0:
                return this.interleaveWithSymbols(input);
            case 1:
                return this.applyRotation(input);
            case 2:
                return this.addHashSegment(input);
            case 3:
                return this.applyXorTransform(input);
            default:
                return input;
        }
    }

    interleaveWithSymbols(input) {
        const symbols = "!@#$%^&*-_+=";
        const bytes = crypto.randomBytes(input.length);
        let result = "";

        for (let i = 0; i < input.length; i++) {
            result += input[i];
            if (i % 2 === 0) {
                result += symbols[bytes[i] % symbols.length];
            }
        }

        return result;
    }

    applyRotation(input) {
        const rotation = crypto.randomBytes(1)[0] % input.length;
        return input.substring(rotation) + input.substring(0, rotation);
    }

    addHashSegment(input) {
        const hash = crypto.createHash('sha256').update(input).digest('hex');
        const hashSegment = hash.substring(0, 4);
        return input + hashSegment;
    }

    applyXorTransform(input) {
        const xorKey = crypto.randomBytes(1)[0];
        return Array.from(input)
            .map(c => String.fromCharCode(c.charCodeAt(0) ^ xorKey))
            .join('');
    }

    hasCharacterDiversity(password) {
        const hasUpper = /[A-Z]/.test(password);
        const hasLower = /[a-z]/.test(password);
        const hasDigit = /[0-9]/.test(password);
        const hasSymbol = /[^a-zA-Z0-9]/.test(password);

        return [hasUpper, hasLower, hasDigit, hasSymbol].filter(x => x).length >= 2;
    }

    hasPositionalRandomness(password) {
        if (password.length < 3) return false;

        // Check if characters don't follow a simple sequential pattern
        let sequentialCount = 0;
        for (let i = 0; i < password.length - 1; i++) {
            if (Math.abs(password.charCodeAt(i) - password.charCodeAt(i + 1)) === 1) {
                sequentialCount++;
            }
        }

        return sequentialCount < password.length / 2;
    }

    hasEncodingPattern(password) {
        // Check for hex patterns, base64 patterns, etc.
        const hasSymbols = /[!@#$%^&*\-_+=]/.test(password);
        const isBase64Like = password.length % 4 === 0 && /^[A-Za-z0-9+/]+$/.test(password);

        return hasSymbols || isBase64Like;
    }

    hasCryptographicDepth(password) {
        // Check if password contains patterns typical of hashed content
        if (password.length < 8) return false;

        // Look for hexadecimal segments or high entropy
        const hexCharCount = (password.match(/[0-9A-Fa-f]/g) || []).length;
        return hexCharCount > password.length * 0.5;
    }

    calculateStrength(depth) {
        if (depth === 1) return "Very Weak";
        if (depth === 2) return "Weak";
        if (depth === 3) return "Moderate";
        if (depth === 4) return "Strong";
        if (depth >= 5) return "Very Strong";
        return "Unknown";
    }

    // Utility method to format analysis result
    formatAnalysis(analysis) {
        const layerInfo = analysis.layers.map(l => `  - ${l}`).join('\n');
        return `${analysis.description}\nStrength: ${analysis.strength}\nDetected Layers:\n${layerInfo}`;
    }
}

// Example usage
function main() {
    const depthSystem = new PasswordDepthSystem();

    console.log("=== Password Depth System ===\n");

    // Generate passwords with different depths
    console.log("Generating passwords with varying depths:\n");
    for (let depth = 1; depth <= 5; depth++) {
        const password = depthSystem.generatePasswordWithDepth(depth, 5);
        console.log(`Depth ${depth}: ${password}`);
    }

    console.log("\n" + "=".repeat(50) + "\n");

    // Analyze existing passwords
    console.log("Analyzing password depths:\n");

    const testPasswords = [
        "hello",
        "Hello123",
        "H3ll@W0rld!",
        "P@ssw0rd!2023#Secure",
        "aB3$x9Kp2Ff8e4A1"
    ];

    testPasswords.forEach(pwd => {
        const analysis = depthSystem.analyzePasswordDepth(pwd);
        console.log(`Password: '${pwd}'`);
        console.log(depthSystem.formatAnalysis(analysis));
        console.log();
    });

    // Verify depth requirements
    console.log("=".repeat(50) + "\n");
    console.log("Verifying depth requirements (minimum depth: 3):\n");

    testPasswords.forEach(pwd => {
        const meetsRequirement = depthSystem.verifyDepthRequirement(pwd, 3);
        console.log(`'${pwd}' - ${meetsRequirement ? "✓ PASS" : "✗ FAIL"}`);
    });
}

// Run the example if this file is executed directly
if (require.main === module) {
    main();
}

// Export for use as a module
module.exports = PasswordDepthSystem;
