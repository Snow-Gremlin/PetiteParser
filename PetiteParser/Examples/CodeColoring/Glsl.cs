using PetiteParser.Loader;
using PetiteParser.Tokenizer;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Examples.CodeColoring {
    public class Glsl: IColorer {

        static private Tokenizer createTokenizer() =>
            Loader.LoadTokenizer(
                "> (Start);",
                "(Start): 'a'..'z', 'A'..'Z', '_' => (Id): 'a'..'z', 'A'..'Z', '0'..'9', '_' => [Id];",
                "(Start): '0'..'9' => (Int): '0'..'9' => (Int) => [Num];",
                "(Int): '.' => (FloatDot): '0'..'9' => (Float): '0'..'9' => (Float) => [Num];",
                "(Start): '<>{}()[]\\-+*%!&|=.,?:;' => (Symbol): '<>{}()[]\\-+*%!&|=.,?:;' => [Symbol];",
                "(Start): '/' => (Slash): '/' => (Comment) => !'\\n' => [Comment];",
                "(Slash) => [Symbol];",
                "(Slash): '#' => (Preprocess): !'\\n' => [Preprocess];",
                "(Start): ' \\n\\r\\t' => (Whitespace): ' \\n\\r\\t' => [Whitespace];",
                "[Id] = 'float', 'double', 'int', 'void', 'bool', 'true', 'false',",
                "       'mat2', 'mat3', 'mat4', 'dmat2', 'dmat3', 'dmat4',",
                "       'mat2x2', 'mat2x3', 'mat2x4', 'dmat2x2', 'dmat2x3', 'dmat2x4',",
                "       'mat3x2', 'mat3x3', 'mat3x4', 'dmat3x2', 'dmat3x3', 'dmat3x4',",
                "       'mat4x2', 'mat4x3', 'mat4x4', 'dmat4x2', 'dmat4x3', 'dmat4x4',",
                "       'vec2', 'vec3', 'vec4', 'ivec2', 'ivec3', 'ivec4', 'bvec2', 'bvec3',",
                "       'bvec4', 'dvec2', 'dvec3', 'dvec4', 'uint', 'uvec2', 'uvec3', 'uvec4',",
                "       'sampler1D', 'sampler2D', 'sampler3D', 'samplerCube',",
                "       'sampler1DShadow', 'sampler2DShadow', 'samplerCubeShadow',",
                "       'sampler1DArray', 'sampler2DArray',",
                "       'sampler1DArrayShadow', 'sampler2DArrayShadow',",
                "       'isampler1D', 'isampler2D', 'isampler3D', 'isamplerCube',",
                "       'isampler1DArray', 'isampler2DArray',",
                "       'usampler1D', 'usampler2D', 'usampler3D', 'usamplerCube',",
                "       'usampler1DArray', 'usampler2DArray',",
                "       'sampler2DRect', 'sampler2DRectShadow',",
                "       'isampler2DRect', 'usampler2DRect',",
                "       'samplerBuffer', 'isamplerBuffer', 'usamplerBuffer',",
                "       'sampler2DMS', 'isampler2DMS', 'usampler2DMS',",
                "       'sampler2DMSArray', 'isampler2DMSArray', 'usampler2DMSArray',",
                "       'samplerCubeArray', 'samplerCubeArrayShadow',",
                "       'isamplerCubeArray', 'usamplerCubeArray' => [Type];",
                "[Id] = 'attribute', 'break', 'case', 'centroid', 'const',",
                "       'continue', 'default', 'discard', 'do', 'else', 'flat', 'for',",
                "       'highp', 'if', 'in', 'inout', 'invariant', 'layout', 'lowp',",
                "       'mediump', 'noperspective', 'out', 'patch', 'precision', 'return',",
                "       'sample', 'smooth', 'struct', 'subroutine', 'switch', 'uniform',",
                "       'varying', 'while' => [Reserved];",
                "[Id] = 'gl_FragColor', 'gl_Position' => [Builtin]");

        static private Tokenizer singleton;

        public Glsl() { }

        public override string ToString() => "GLSL";

        public IEnumerable<Formatted> Colorize(params string[] input) {
            singleton ??= createTokenizer();
            return this.colorize(singleton.Tokenize(string.Join(Environment.NewLine, input)));
        }

        private IEnumerable<Formatted> colorize(IEnumerable<Token> tokens) {
            foreach (Token token in tokens) {
                foreach (Formatted fmt in this.colorize(token))
                    yield return fmt;
            }
        }

        private IEnumerable<Formatted> colorize(Token token) {
            switch (token.Name) {
                case "Builtin":    yield return new Formatted(token, 0x441111); break;
                case "Comment":    yield return new Formatted(token, 0x777777); break;
                case "Id":         yield return new Formatted(token, 0x111111); break;
                case "Num":        yield return new Formatted(token, 0x119911); break;
                case "Preprocess": yield return new Formatted(token, 0x773377); break;
                case "Reserved":   yield return new Formatted(token, 0x111199); break;
                case "Symbol":     yield return new Formatted(token, 0x661111); break;
                case "Type":       yield return new Formatted(token, 0x117711); break;
                case "Whitespace": yield return new Formatted(token, 0x111111); break;
            }
        }

        public string ExampleCode =>
            "";
    }
}
